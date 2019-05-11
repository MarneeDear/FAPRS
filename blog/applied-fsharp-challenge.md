# F# for APRS
###### A system for sending and receiving APRS messages integrated with DireWolf, and built on .NET Core in F#. 

_This is also my submission for the [Applied F# Challenge](http://foundation.fsharp.org/applied_fsharp_challenge) - F# in your organization or domain category._

## Applied F# Challenge

#### Author

Marnee Dearman (KG7SIO)

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

### Logical dsesign

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

The DireWolf `kissutil` makes it easy to send and receive messages. The `kissutil` reads TNC2MON formated messages from files in a folder you specify and then sends them to the DireWolf TNC for sending out over the radio. The `kissutil` also writes received packets to a folder you specify. The Direwolf integration reads from, and writes to, these files.

The records are in the TNC2MON format, which I will talk about more later.

### Application architecture

The overall pattern is following Onion Architecture/Clean Architecture. It consists of:

* A core library where the data models live (faprs.core)
* An infrastructure library where data operations and business logic happens (faprs.infrastructure)
* A CLI for creating messages for the `kissutil` (faprs.cli)
* A web service that hosts a web interface for creating and displaying messages to send and receive through the `kissutil`(faprs.service)
* A SQL Lite database (database.sqlite) for storing sent and received messages.
* Tests (faprs.tests)
* Database migrations (faprs.migrations)

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

#### DESTINATION

The `DESTINATION` can be the call sign of a particular station for which a message is intended, but `DESTINATION` is overloaded and can be used to pass on other encoded bits of information. One common usage is to identify the sending application and version number. You can see a list of TAPR approved to-calls [here](http://aprs.org/aprs11/tocalls.txt).

By default, the `fapr.cli` will uses the DireWolf TOCALL, `APDW15`.

#### PATH

The path is also known as the digipath, and specifies if and how an APRS package should be repeated (re-transmitted) when received by a digital repeater (digipeater). This is intended to avoid repeating packets redundantly, and reduce the amount of traffic on the APRS network. The digipeater will be configured to re-transmit according to the PATH depending on its location and general network conditions in order to help prevent network congestion. 

> PATH settings determine what kind and how many digipeaters will be used to deliver your packets to their destination.

FAPRS only supports `WIDE1-1` at the moment, which means (from [this Stack Overflow answer](https://ham.stackexchange.com/questions/6213/help-understanding-path-taken-by-aprs-packet?rq=1))

> It requests that a "wide" digipeater (one with a wide coverage area, like on a mountaintop) repeat the packet, but only once; if a second "wide" digipeater should hear the rebroadcast packet, then the second digipeater wouldn't repeat it.

The PATH part is best defined and explained [here](http://wa8lmf.net/DigiPaths/) and [here](https://ham.stackexchange.com/questions/6213/help-understanding-path-taken-by-aprs-packet?rq=1).




## Other Resources

> [![History of APRS](https://img.youtube.com/vi/OgFBXfwmKYc/0.jpg)](https://www.youtube.com/watch?v=OgFBXfwmKYc "Everything Ham Radio Podcast")

> _click the video to watch on You Tube_

### Prior Art

[Python APRS Module](https://github.com/ampledata/aprs)

[APRSdroid](https://aprsdroid.org/)
