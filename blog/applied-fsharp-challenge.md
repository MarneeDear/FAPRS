# F# for APRS
###### A system for sending and receiving APRS messages integrated with DireWolf, and built on .NET Core in F#. 

_This is also my submission for the [Applied F# Challenge](http://foundation.fsharp.org/applied_fsharp_challenge) - F# in your organization or domain category._

## Applied F# Challenge

#### Author

Marnee Dearman (KG7SIO)

#### Github Repository

[MarneeDear/FAPRS](https://github.com/MarneeDear/FAPRS)

#### Domain

Amateur radio communications protocols, specifically the very popular packet radio protocol, APRS -- Automatic Packet Reporting System. 

#### Purpose

* Demonstrate the power of functional data modeling in communications protocol applications in general, and APRS specifically
* Provide a cross-platform, simple, easy to use application for sending and receiving APRS messages.
* Provide an automated way to track race participants during amateur radio supported long-distance races (e.g, 24 Hours in the Old Pueblo, CV 50/50). 
* Provide a framework for developing custom APRS applications.
* Expose more people to F#
* Get more people involved in amateur radio

#### The Highlights

The code uses a lot of functional programming techniques and F# libraries to get things done, but here are the highlights:

* [Designing with types](https://fsharpforfunandprofit.com/posts/designing-with-types-intro/)
  * Single case unions
  * Constrained strings
  * Making impossible states impossible
* [Active patterns](https://fsharpforfunandprofit.com/posts/convenience-active-patterns/) for string parsing and validation
* [Railway oriented programming](https://fsharpforfunandprofit.com/posts/recipe-part2/)
* [Argu](http://fsprojects.github.io/Argu/) for command line parsing
* [Expecto](https://github.com/haf/expecto) for testing
* [Saturn](https://saturnframework.org/) for building the web app/web API/web service


## Amateur Radio and APRS

### What is amateur radio?

What Wikipedia thinks it is:

> Amateur radio, also known as ham radio, describes the use of radio frequency spectrum for purposes of non-commercial exchange of messages, wireless experimentation, self-training, private recreation, radiosport, contesting, and emergency communication

What the [Radio Society of Great Britain]() thinks it is (great video introduction):

> [![Packet Radio](https://img.youtube.com/vi/8x6x_6mDVlQ/0.jpg)](https://youtu.be/8x6x_6mDVlQ "Ham radio is awesome.")

> _click the video to watch on You Tube_

### What is APRS?

What Wikipedia thinks it is:

> *Automatic Packet Reporting System* is an amateur radio-based system for real time digital communications of information of immediate value in the local area. Data can include object Global Positioning System coordinates, weather station telemetry, text messages, announcements, queries, and other telemetry.

This video is a nice demonstration of what you can do with APRS. The system demonstrated in the video is similar to my F# for APRS system design.

> [![Packet Radio](https://img.youtube.com/vi/FJEVWMuz6Xg/0.jpg)](http://www.youtube.com/watch?v=FJEVWMuz6Xg "Kantronics Packet Radio Mail and BBS Operations")

> _click the video to watch on You Tube_

## System design

FAPRS will act like a digipeater in it's own network.

### Motivation, purpose, and use cases

Hams often volunteer for middle-of-nowhere long distances races. You know the kind? Where 20 persons run 50 miles through the desert for fun? The participants need trail support and we Hams are best-equipped to run the communications system. We setup stations with mobile radios at known locations along the trail, make sure we can communicate with each other, and setup a process by which we share participant status and location, so we can keep track of them and request medical support if needed. It's great fun, and many of these events wouldn't happen if it weren't for us Hams.

Most of the time we use voice to communicate. This works well enough, but I have worked enough of these races to know that it can be a challenge to keep track of all of the runners. The problem is this system requires a lot of coordination and sometimes we talk over each other or don't always hear the messages. The problems only increase as the number of participants increase. 

> It doesn't scale well.

I wanted to develop a system using packet radio that could automatically send and receive participant status reports.

### Logical design

#### Requirements 

* Store sent and received messages and re-send periodically
* A management interface for entering participant status reports and general informational messages
* Prioritize emergency messages and new reports
* De-prioritize older messages and expire messages after a certain period of time
* See a list of messages that were sent and received
* Be accessible over WiFi so a keyboard and monitor are not required

### Physical design

![Imgur](https://i.imgur.com/9w47hfD.png)

I designed this system with a few things in mind:

* It will be used in remote locations
* Low-cost 
* Highly mobile -- can carry all of the parts in a backpack easily
* Compatible with common hand-held VHF radios like my Baofeng UV-82* 
* Low-power and possible to run off a portable solar cell

The Raspberry Pi 3 provides these services

* WiFi hot-spot
* DireWolf -- the Terminal Node Controller 
    * Handles encoding and decoding packets and sending them out the audio port to the radio
* FAPRS
    * Self-hosted web service providing
        * Management interface
        * Message scheduler and processor
    * CLI
        * Can be used to manually craft APRS messages for testing and troubleshooting
    * Database (SQL Lite) for storing sent and received messages
* Wired to a VHF radio through the audio port
    * A USB sound card may be used
    * Need both an audio-in and audio-out (mic and speaker) so that messages can both be received and transmitted.

### My equipment

 * [RaspberryPi 3](https://www.raspberrypi.org/products/raspberry-pi-3-model-b/)
 * [Baofeng UV-82](https://baofengtech.com/uv82)
 * [BTech APRS K2 Cable (connect radio to RaspberryPi or laptop via the audio port)](https://baofengtech.com/aprs-k2-trrs-cable)
 * My laptop
 * My mobile phone (Android)

### DireWolf integration

The DireWolf integration ended up being really simple because the developer provided a KISS utility that reads TNC2MON formatted messages from a file and converts them to KISS format to be transmitted via the TNC (in this case DireWolf is also running as a TNC).

#### kissutil

The DireWolf `kissutil` makes it easy to send and receive messages. The `kissutil` reads TNC2MON formated messages from files in a folder you specify and then sends them to the DireWolf TNC for sending out over the radio. The `kissutil` also writes received packets to a folder you specify. The Direwolf integration reads from, and writes to, these folders.

The records are in the TNC2MON format, which I will talk about more later.

### Application architecture

The overall pattern is following Onion Architecture/Clean Architecture. It consists of:

* A core library where the data models live (`faprs.core`)
* An infrastructure library where data operations and business logic happens (`faprs.infrastructure`)
* A CLI for creating messages for the `kissutil` (`faprs.cli`)
    * The CLI uses [Argu](http://fsprojects.github.io/Argu/)
* A web service that hosts a web interface for creating and displaying messages to send and receive through the `kissutil`(`faprs.service`)
    * The web service uses [Saturn](https://saturnframework.org/)
* A SQL Lite database (database.sqlite) for storing sent and received messages.
* Tests (`faprs.tests`)
* Database migrations (`faprs.migrations`)

## APRS specification and implementation

The APRS protocol was developed by [TAPR (Tucson Amateur Packet Radio)](https://www.tapr.org/aprs_information.html). 

There are 3 specification versions.

* [APRS v1.0](http://www.aprs.org/doc/APRS101.PDF)
* [APRS v1.1](http://www.aprs.org/aprs11.html)
* [APRS v1.2](http://www.aprs.org/aprs12.html)

Versions 1.1 and 1.2 specify mostly additional features.

### TNC 2 Monitor format (TNC2MON)

The `kissutil` accepts APRS packets in the TNC 2 Monitor format. This format is defined in the
APRS version 1.0.1 specification under the section `Network Tunneling and Third-Party Digipeating`. 

FAPRS takes message details and outputs a TNC2MON formatted packet.

It looks like this:

> *SENDER*>*DESTINATION*,*PATH*:*MESSAGE*

The packet consists of a source, a destination, a path, and a message. The message can be user-defined, but is most often a Position Report. There are a number of Position Report formats as defined by the APRS spec. 

#### SENDER

The `SENDER` is always the transmitting station's call sign. 

For example, my station's call sign is `KG7SIO`, because that was the call sign assigned to me by the FCC when I got my license.

Since sender is a call sign, I created a CallSign type (in the `Common` namespace in `faprs.core`). I used a `single case union type` as described by Scott Wlashcin [here](https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/).

```fsharp
//9 byte field
type CallSign = private CallSign of string          
module CallSign =
    open System

    let create (s:string) = 
        match (s.Trim()) with
        | c when not (String.IsNullOrEmpty(c)) && c.Length < 10     -> Some (CallSign c)
        | _                                                         -> None // "Call Sign cannot be empty and must be 1 - 9 characters. See APRS 1.01."
    let value (CallSign s) = s.ToUpper() // MUST BE ALL CAPS        
```

This makes it so that anywhere I need a CallSign I can be guaranteed to have a properly formated call sign.

#### DESTINATION

The `DESTINATION` can be the call sign of a particular station for which a message is intended, but `DESTINATION` is overloaded and can be used to pass on other encoded bits of information. One common usage is to identify the sending application and version number. You can see a list of TAPR approved to-calls [here](http://aprs.org/aprs11/tocalls.txt).

By default, the `fapr.cli` will uses the DireWolf TOCALL, `APDW15`.

Since destination is also a call sign, I can use the CallSign type.

#### PATH

The `PATH`` is also known as the digipath, and specifies if and how an APRS package should be repeated (re-transmitted) when received by a digital repeater (digipeater). This is intended to avoid repeating packets redundantly, and reduce the amount of traffic on the APRS network. The digipeater will be configured to re-transmit according to the PATH depending on its location and general network conditions in order to help prevent network congestion. 

> PATH settings determine what kind and how many digipeaters will be used to deliver your packets to their destination.

> It requests that a "wide" digipeater (one with a wide coverage area, like on a mountaintop) repeat the packet, but only once; if a second "wide" digipeater should hear the rebroadcast packet, then the second digipeater wouldn't repeat it.

The PATH part is best defined and explained [here](http://wa8lmf.net/DigiPaths/) and [here](https://ham.stackexchange.com/questions/6213/help-understanding-path-taken-by-aprs-packet?rq=1).

```fsharp
type WIDEnN =
    | WIDE11
    | WIDE21
    | WIDE22
    override this.ToString() =
        match this with 
        | WIDE11    -> "WIDE1-1"
        | WIDE21    -> "WIDE2-1"
        | WIDE22    -> "WIDE2-2"
    static member fromString p =
        match p with
        | "WIDE1-1" -> WIDE11
        | "WIDE2-1" -> WIDE21
        | "WIDE2-2" -> WIDE22
        | _         -> WIDE11 //Use this as the default

//9 byte field
//aka the UNPROTO path
//http://wa8lmf.net/DigiPaths/index.htm#Recommended
type Path =
    | WIDEnN of WIDEnN
    override this.ToString() =
        match this with
        | WIDEnN p ->   match p with
                        | WIDE11    -> WIDE11.ToString()
                        | WIDE21    -> WIDE21.ToString()
                        | WIDE22    -> WIDE22.ToString()
```

The PATH part used to have a number of types, but those were mode obsolete and the recommend PATH is to only use one of the `WIDEnN` options. That is how I modeled it here, while also allowing for the possibility of supporting other PATH types.

#### MESSAGE

The `MESSAGE` is also known at the `payload`. This is the data you want to transmit `and the fun part`.

The `MESSAGE` is also where it starts to get more complicated. As `APRS` started as a position reporting system, APRS specifies a number of standard message formats for identifying a station's position, but also provides for user-defined messages, weather reports, telemetry, and plain old messages (as if you were tweeting). 

FAPRS supports a number of message formats. I will describe three of them

### APRS data formats

All of the support message formats are defined by a union type. Each of the options has its own type.

```fsharp
type Message =
    | Unformatted                       of UnformattedMessage
    | PositionReportWithoutTimeStamp    of PositionReportWithoutTimeStamp
    | ParticipantStatusReport           of Participant.ParitcipantStatusReport
    | Unsupported                       of UnformattedMessage
    override this.ToString() =
        match this with 
        | Unformatted m                     -> UnformattedMessage.value m // (:) is the aprs data type ID for message
        | PositionReportWithoutTimeStamp r  -> r.ToString()
        | ParticipantStatusReport r         -> r.ToString()
        | Unsupported u                     -> UnformattedMessage.value u //This is where anything that cant be parsed will end up
```

#### Unformatted Message

An unformatted message must start with `:`, and has a size constraint, but otherwise can contain anything. I used a single case union type for this one, too.

```fsharp
type UnformattedMessage = private UnformattedMessage of string
module UnformattedMessage =
    let create (m:string) =
        match (m.Trim()) with
        | m when m.Length <= 255    -> UnformattedMessage m //AX.25 field is 256 chars but the message has to accommodate the { for user defined messages
        | _                         -> UnformattedMessage (m.Substring(0, 255)) 
    let value (UnformattedMessage m) = sprintf ":%s" m
```

#### Lat/Long Position Report Format — without Timestamp

This format is defined in `APRS 1.01` `6 TIME AND POSITION FORMATS` and `8 POSITION AND DF REPORT DATA FORMATS`

It looks like this:

```text
        ! or |
        =    | Latitude |  /  | Longitude | Symbol | Comment (max 43 chars)
Bytes    1         8       1        9          1           0-43
```

Example:

```text
!4903.50N/07201.75W-Test
```

The latitude and longitude are expected to be in the APRS format defined in `6 TIME AND POSITION FORMATS`.

I created a PositionReportWithoutTimeStamp record type that includes the fields of the position report.

```fsharp
type PositionReportWithoutTimeStamp =
    {
        Position    : Position
        Symbol      : SymbolCode
        Comment     : PositionReportComment
    }
    override this.ToString() =
        sprintf "=%s/%s%c%s" (FormattedLatitude.value this.Position.Latitude) (FormattedLongitude.value this.Position.Longitude) (this.Symbol.ToChar()) (PositionReportComment.value this.Comment)  
```

`Position` is the latitude and longitude in APRS format.

The `PositionReportWithoutTimeStamp` will also return the string representation of a position report.

##### Latitude and longitude 

Latitude and longitude take a decimal coordinate and convert it to the APRS format.

    Latitude is expressed as a fixed 8-character field, in degrees and decimal
    minutes (to two decimal places), followed by the letter N for north or S for
    south.

    Longitude is expressed as a fixed 9-character field, in degrees and decimal
    minutes (to two decimal places), followed by the letter E for east or W for
    west.

I created two single case union types called `FormattedLatitude` and `FormattedLatitude` that do the conversion and create a formatted latitude or longitude. The hemisphere designation is further constrained by a type.

```fsharp
type LatitiudeHemisphere =
    | North     
    | South     
    member this.ToHemisphereChar() =
        match this with
        | North _   -> 'N'
        | South _   -> 'S'
    static member fromHemisphere h =
        match h with
        | 'N'   -> Some LatitiudeHemisphere.North
        | 'S'   -> Some LatitiudeHemisphere.South
        | _     -> None //"Latitude must be in northern (N) or southern (S) hemisphere."

type FormattedLatitude = private FormattedLatitude of string
module FormattedLatitude =
    let create (d:float) =
        let deg, min, sec = calcDegMinSec d
        FormattedLatitude (sprintf "%02i%02i.%02i%c" deg min sec (if d > 0.0 then (North.ToHemisphereChar()) else (South.ToHemisphereChar())))
    let check (d:string) =
        FormattedLatitude d 
    let value (FormattedLatitude d) = d
```

##### Symbol

APRS defines a list of symbols that can be rendered on a map and represent a station. There are a number of symbols defined, but FAPRS only supports some of them.

```fsharp
    //This is only a subset of the codes because I don't want to support all of them at this time
type SymbolCode =
    | House 
    | Bicycle 
    | Balloon 
    | Hospital
    | Jeep 
    | Truck
    | Motorcycle
    | Jogger
    member this.ToChar() =
        match this with
        | House         -> '-'
        | Bicycle       -> 'b'
        | Balloon       -> 'O'
        | Hospital      -> 'h'
        | Jeep          -> 'j'
        | Truck         -> 'k'
        | Motorcycle    -> '<'
        | Jogger        -> '['
    static member fromSymbol s =
        match s with
        | '-' -> Some House
        | 'b' -> Some Bicycle
        | 'O' -> Some Balloon
        | 'h' -> Some Hospital
        | 'j' -> Some Jeep
        | 'k' -> Some Truck
        | '<' -> Some Motorcycle
        | '[' -> Some Jogger
        | _   -> None
```

##### Comment

The `COMMENT` field has a size constraint, but otherwise can contain anything. I used a single case union type for this one, too.

```fsharp
type PositionReportComment = private PositionReportComment of string
module PositionReportComment =
    let create (s:string) =
        match (s.Trim()) with
        | s when s.Length < 44  -> Some (PositionReportComment s)
        | _                     -> None //failwith "Position Report Comment must be less than 43 characters long."

    let value (PositionReportComment c) = c //Was trimmed during create
```

#### Participant Status Report

The `Participant Status Report` is a user-defined format that I created in order to facilitate tracking race participant. 

User-defined formating is defined in APRS 1.01 `18 USER DEFINED FORMATS`. The first 3 characters of a user-defined data format are the data identifiers. 

* { APRS Data Type Identifier.
* U A one-character User ID.
* X A one-character user-defined packet type.

The APRS spec makes allowances for experimental user-defined formats. Experimental formats must start with `{{`. I chose `P` as the experimental user-defined data identifier. This makes the full identifier `{{P`.

The APRS data needs to fit inside the [AX.25 information field](https://en.wikipedia.org/wiki/AX.25), which is defined as 1-256 bytes. 

Participant Status should include these parts:
* User-defined data type
* 253 chars max
* Participant number (bib number)
* Time last seen at comm station
* Status (continued, injured, waiting for help, taking a break) 
* Sender should identify the transmitting station
* Destination identifies the comm HQ or one of the stations in the network

```text
Participant Status Field
        TIMESTAMP   PARTICIPANT-ID      STATUS-1    STATUS-2    MESSAGE
BYTES   8-fixed     5-fixed             1-fixed     1-fixed     0-238 
```

Example including the data identifier:

```text
{{P100923450004211In good shape!
```

    IDENTIFIER  TIMESTAMP           PARTICIPANT-ID      STATUS-1    STATUS-2    MESSAGE
    {{P         10092345            00042               1           1           In good shape!
                2019-10-09 23:34                        Continued   Continued

The `TIMESTAMP` field is an APRS formatted timestamp.

    TIMESTAMP
    Month/Day/Hours/Minutes (MDHM) format is a fixed 8-character field,
    consisting of the month (01–12) and day-of-the-month (01–31), followed by
    the time in hours and minutes zulu. For example: 10092345 is 23 hours 45 minutes zulu on October 9th.

I created a new record type called `ParitcipantStatusReport` that defines 4 fields.

```fsharp
type ParitcipantStatusReport =
    {
        TimeStamp           : RecordedOn
        ParticipantID       : ParticipantID
        ParticipantStatus   : ParticipantStatus
    }
    override this.ToString() =
        let (status1, status2, msg) = this.ParticipantStatus.ToStatusCombination()
        sprintf "{{P%s%s%i%i%s%s" (RecordedOn.value this.TimeStamp) (ParticipantID.value this.ParticipantID) status1 status2 msg
```

`RecordedOn` is a single case union type that converts a DateTime value to an APRS formatted timestamp. It creates a timestamp, and it will revert a timestamp back to a DateTime.

```fsharp
type RecordedOn = private RecordedOn of string
module RecordedOn =
    let revert (timestamp:string) = //TODO would this be better in an active pattern?
        let mm = (timestamp.Substring(0, 2))
        let dd = (timestamp.Substring(2, 2))
        let HH = (timestamp.Substring(4, 2))
        let MM = (timestamp.Substring(6, 2))
        let dt = sprintf "%i-%s-%sT%s:%s" DateTime.Today.Year mm dd HH MM
        DateTime.Parse(dt)
    let create (date:DateTime option) =
        match date with
        | Some d    -> RecordedOn (sprintf "%02i%02i%02i%02i" d.Month d.Day d.Hour d.Minute)
        | None      -> let utcNow = DateTime.Now
                        RecordedOn (sprintf "%02i%02i%02i%02i" utcNow.Month utcNow.Day utcNow.Hour utcNow.Minute)
    let value (RecordedOn d) = d
```

`ParticipantID` is fixed-width, can be any characters, and is 0 padded to fill the empty space.

```fsharp
type ParticipantID = private ParticipantID of string
module ParticipantID =
    let create (nbr:string) =
        match nbr with 
        | n when String.IsNullOrWhiteSpace(n)   -> None
        | n when nbr.Length < 6                 -> Some (ParticipantID (sprintf "%5s" n)) //Fixed width 5 chars
        | _                                     -> None 
    let value (ParticipantID n) = n
```

`ParticipantStatus` is fixed-width can limited to a set of statuses in a combination of one or two status options, plus a free form message. I modeled this as a tuple of (status, status, message).

Only `Injured` has a sub status combination. For example, an `Injured` participant could also be `Continued`, `Resting`, `NeedsEmergencySupport`, `DroppedOut`, `Unknown`.

```fsharp
type ParticipantStatusMessage = private ParticipantStatusMessage of string
module ParticipantStatusMessage =
    let create (s:string) =
        match (s.Trim()) with
        | s when s.Length <= 239 -> ParticipantStatusMessage s
        | _ -> ParticipantStatusMessage (s.Substring(0, 239))
    let value (ParticipantStatusMessage s) = s

type ParticipantStatus =
    | Continued                 of ParticipantStatusMessage
    | Injured                   of ParticipantStatus
    | Resting                   of ParticipantStatusMessage
    | NeedsEmergencySupport     of ParticipantStatusMessage
    | Completed                 of ParticipantStatusMessage
    | DroppedOut                of ParticipantStatusMessage
    | Unknown                   of ParticipantStatusMessage
    member this.ToStatusCombination() =
        match this with
        | Continued m       -> (1, 1, ParticipantStatusMessage.value m)
        | Injured s         ->  match s with
                                | Continued m               -> (2, 1, ParticipantStatusMessage.value m)
                                | Resting m                 -> (2, 3, ParticipantStatusMessage.value m)
                                | NeedsEmergencySupport m   -> (2, 4, ParticipantStatusMessage.value m)
                                | DroppedOut m              -> (2, 6, ParticipantStatusMessage.value m)
                                | Unknown m                 -> (2, 0, ParticipantStatusMessage.value m) 
                                | _                         -> (2, 0, String.Empty)
        | Resting m                 -> (3, 3, ParticipantStatusMessage.value m)
        | NeedsEmergencySupport m   -> (4, 4, ParticipantStatusMessage.value m)
        | Completed m               -> (5, 5, ParticipantStatusMessage.value m)
        | DroppedOut m
        | Unknown m                 -> (0, 0, ParticipantStatusMessage.value m)
    member this.ToOptionName () =
        match this with
        | Continued s               -> "Continued"
        | Injured s                 -> "Injured"
        | Resting s                 -> "Resting"
        | NeedsEmergencySupport s   -> "Needs Emergency Support"
        | Completed s               -> "Completed"
        | DroppedOut s              -> "Dropped Out"
        | Unknown s                 -> "Unknown"
    static member fromStatusCombo s =
        match s with
        | (1, 1, m) -> (Continued (ParticipantStatusMessage.create m))
        | (2, 1, m) -> (Injured (Continued (ParticipantStatusMessage.create m)))
        | (2, 3, m) -> (Injured (Resting (ParticipantStatusMessage.create m)))
        | (2, 4, m) -> (Injured (NeedsEmergencySupport (ParticipantStatusMessage.create m)))
        | (2, 0, m) -> (Injured (Unknown (ParticipantStatusMessage.create m)))
        | (3, 3, m) -> (Resting (ParticipantStatusMessage.create m))
        | (4, 4, m) -> (NeedsEmergencySupport (ParticipantStatusMessage.create m))
        | (5, 5, m) -> (Completed (ParticipantStatusMessage.create m))
        | (6, 6, m) -> (DroppedOut (ParticipantStatusMessage.create m))
        | (0, 0, m) -> (Unknown (ParticipantStatusMessage.create m))
        | (_, _, m) -> (Unknown (ParticipantStatusMessage.create m))
```

#### The final participant status report data frame

The final output for the `kissutil` will look like this:

> KG7SIO>APDW15,WIDE1-1:{{P100923450004211In good shape!

## Parsing received messages

FAPRS can produce an APRS message, and it can parse a TNC2MON formatted frame with a number of APRS data formats. To do this I used [Active Patterns](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/active-patterns). The parser assumes the frames were produced by the DireWolf `kissutil`, which are in this format:

> [0] K1NRO-1>APDW15,WIDE2-2:!4238.80NS07105.63W#PHG5630

This is the same as what FAPRS produces for the `kissutil`, but includes the channel on which the message was received -- `[0]`.

### Start by getting the frame without the channel

```fsharp
//Remove the channel from the frame
let (|Frame|_|) (record:string) =
    match record with
    | r when String.IsNullOrWhiteSpace(r) -> None
    | r when r.IndexOf(" ") < 1 -> None //maybe return r because there was no channel and that's ok?
    | r when (r.Substring(r.IndexOf(" "))).Trim().Length > 0 -> Some ((r.Substring(r.IndexOf(" "))).Trim())
    | _ -> None
```

### Get the address field -- the sender and destination

```fsharp
let (|Address|_|) (frame:string) =
    if frame.IndexOf(":") < 1 then 
        None
    else
        Some (frame.Substring(0, frame.IndexOf(":")))
```

### Get the sender and destination out of the Address

```fsharp
let (|Sender|_|) (address:string) =
    if address.IndexOf(">") < 1 then None
    else Some (address.Substring(0, address.IndexOf(">")))

let (|Destination|_|) (address:string) =
    if address.IndexOf(">") < 1 || address.IndexOf(",") < 1 then None
    else Some (address.Substring(address.IndexOf(">") + 1, address.IndexOf(",") - address.IndexOf(">") - 1))
```

### Get the Path out of the Frame

```fsharp
let (|Path|_|) (address:string) =
    if not (address.IndexOf(">") = -1) && address.IndexOf(",") > address.IndexOf(">") then
        Some (address.Substring(address.IndexOf(",") + 1).Split(','))
    else
        None
```

### Get the Message out of the Frame

```fsharp
let (|Message|_|) (frame:string) =
    if frame.IndexOf(":") < 1 then 
        None
    else
        Some (frame.Substring(frame.IndexOf(":") + 1))
```

### Parse a position report

#### Get the Latitude out of the Message

```fsharp
let (|Latitude|_|) (msg:string) = 
    let parseLatitude (posRpt:string) =
        let lat = posRpt.Substring(1, 8)
        match lat.EndsWith("N"), lat.EndsWith("S") with
        | true, false   -> Some lat
        | false, true   -> Some lat
        | _             -> None
    match getAPRSDataTypeIdentifier (msg.Substring(0,1)) with
    | Some id   ->  match id with
                    | PositionReportWithoutTimeStampWithMessaging   -> (parseLatitude msg)
                    | PositionReportWithoutTimeStampNoMessaging     -> (parseLatitude msg)
                    | _                                             -> None
        | None     -> None //We do not have a position report and therefore no latitude
```

#### Get the Longitude out of the Message

```fsharp
let (|Longitude|_|) (msg:string) =
    let parseLongitude (posRpt:string) =
        let lon = posRpt.Substring(10, 9) 
        match lon.EndsWith("W"), lon.EndsWith("E") with 
        | true, false   -> Some lon
        | false, true   -> Some lon
        | _             -> None
        
    match msg.Substring(9,1) with
    | "/" -> parseLongitude msg
    | _ -> None
```

#### Get the Symbol out of the Message

```fsharp
let (|Symbol|_|) (msg:string) =
    //TODO check that the previous char was a W or E meaning that it was probably and APRS lat/lon
    match msg.Substring(18,1) with
    | "W" -> SymbolCode.fromSymbol (msg.Substring(19,1).ToCharArray().[0]) //  getSymbolCode (msg.Substring(19,1).ToCharArray().[0])
    | "E" -> SymbolCode.fromSymbol (msg.Substring(19,1).ToCharArray().[0])
    | _ -> None
```

#### Get the Comment out of the Message

```fsharp
let (|Comment|_|) (symbol:char) (msg:string) =
    let comment = msg.Substring(msg.IndexOf(symbol) + 1).Trim()
    if comment = 
        String.Empty 
    then    
        None 
    else 
        Some comment
```

#### The Tests

To see the active patterns in action, check out `faprs.tests` `TNC2MONActivePatternsTests`.

For example

```fsharp
testCase "Can get message part of well formed frame with message" <| fun _ ->
    let result =
        match "[0] KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2:=03216.4N/011057.3Wb,b>,lah:blah /fishcakes" with
        | TNC2MonActivePatterns.Message m -> m
        | _ -> String.Empty
    Expect.equal result "=03216.4N/011057.3Wb,b>,lah:blah /fishcakes" "Message does not match"
```

## Other Resources

> [![History of APRS](https://img.youtube.com/vi/OgFBXfwmKYc/0.jpg)](https://www.youtube.com/watch?v=OgFBXfwmKYc "Everything Ham Radio Podcast")

> _click the video to watch on You Tube_

### Prior Art

[Python APRS Module](https://github.com/ampledata/aprs)

[APRSdroid](https://aprsdroid.org/)
