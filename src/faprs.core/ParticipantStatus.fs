namespace faprs.core

(*
5 APRS DATA IN THE AX.25 INFORMATION FIELD

Generic Data Format

In general, the AX.25 Information field can contain some or all of the
following information:
• APRS Data Type Identifier
• APRS Data
• APRS Data Extension
• Comment

Generic APRS Information Field
        DataTypeID  APRSData    APRSDataExtension   Comment
Bytes:  1           n           7                   n

APRS Data Type Identifier
Every APRS packet contains an APRS Data Type Identifier (DTI). This
determines the format of the remainder of the data in the Information field

Participant Status 
* User-defined data type
* 255 chars max
* Participant #
* Time last seen at comm station
* Status (Continued, injured, waiting for help, taking a break) -- this will need status codes
    * description
* Sender identifies the comm staion Or include GPS to identify the way point
* Destination identifies the HQ??

Participant Status Field
        TIMESTAMP   PARTICIPANT-ID      STATUS-1    STATUS-2    MESSAGE
BYTES   8-fixed     5-fixed             1-fixed     1-fixed     0-239 

 NOT USING THIS PROBABLY Date/Time format 2009-06-15T13:45:30 -- yyyy-MM-ddTHH:mm:ss

TIMESTAMP
Month/Day/Hours/Minutes (MDHM) format is a fixed 8-character field,
consisting of the month (01–12) and day-of-the-month (01–31), followed by
the time in hours and minutes zulu. For example:

10092345 is 23 hours 45 minutes zulu on October 9th.

If cycling through messages, participantStatus expires at midnight, 
but can be renewed for the next day 

Experimental User-Defined types start with {{ (double curly braces)

*)
module Participant = 
    open System
    open System.Linq.Expressions

    //TODO return failed state if not correct length 
    type ParticipantID = private ParticipantID of string
    module ParticipantID =
        let create (nbr:string) =
            match nbr with 
            | n when nbr.Length < 6 -> ParticipantID (sprintf "%5s" n) //Fixed width 5 chars
            | n -> ParticipantID (n.Substring(0, 5)) //TODO should probably return an error
        let value (ParticipantID n) = n

    //10092345 is 23 hours 45 minutes zulu on October 9th.
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
        | CompletedOrLeftCourse     of ParticipantStatusMessage
        | Unknown                   of ParticipantStatusMessage
        member this.ToStatusCombination() =
            match this with
            | Continued m       -> (1, 1, ParticipantStatusMessage.value m)
            | Injured s         ->  match s with
                                    | Continued m               -> (1, 2, ParticipantStatusMessage.value m)
                                    | Resting m                 -> (3, 2, ParticipantStatusMessage.value m)
                                    | NeedsEmergencySupport m   -> (4, 2, ParticipantStatusMessage.value m)
                                    | Unknown m                 -> (0, 2, ParticipantStatusMessage.value m) 
                                    | _                         -> (0, 2, String.Empty)
            | Resting m                 -> (3, 3, ParticipantStatusMessage.value m)
            | NeedsEmergencySupport m   -> (4, 4, ParticipantStatusMessage.value m)
            | CompletedOrLeftCourse m   -> (5, 5, ParticipantStatusMessage.value m)
            | Unknown m                 -> (0, 0, ParticipantStatusMessage.value m)
        member this.ToOptionName () =
            match this with
            | Continued s               -> "Continued"
            | Injured s                 -> "Injured"
            | Resting s                 -> "Resting"
            | NeedsEmergencySupport s   -> "Needs Emergency Support"
            | CompletedOrLeftCourse s   -> "Completed or Left the Course"
            | Unknown s                 -> "Unkown"
        static member fromStatusCombo s =
            match s with
            | (1, 1, m) -> (Continued (ParticipantStatusMessage.create m))
            | (1, 2, m) -> (Injured (Continued (ParticipantStatusMessage.create m)))
            | (3, 2, m) -> (Injured (Resting (ParticipantStatusMessage.create m)))
            | (4, 2, m) -> (Injured (NeedsEmergencySupport (ParticipantStatusMessage.create m)))
            | (0, 2, m) -> (Injured (Unknown (ParticipantStatusMessage.create m)))
            | (3, 3, m) -> (Resting (ParticipantStatusMessage.create m))
            | (4, 4, m) -> (NeedsEmergencySupport (ParticipantStatusMessage.create m))
            | (5, 5, m) -> (CompletedOrLeftCourse (ParticipantStatusMessage.create m))
            | (0, 0, m) -> (Unknown (ParticipantStatusMessage.create m))
            | (_, _, m) -> (Unknown (ParticipantStatusMessage.create m))

    type ParitcipantStatusReport =
        {
            TimeStamp           : RecordedOn
            ParticipantID       : ParticipantID
            ParticipantStatus   : ParticipantStatus
            Cancelled           : bool
        }
        override this.ToString() =
            let (status1, status2, msg) = this.ParticipantStatus.ToStatusCombination()
            //match this.ParticipantStatus.ToStatusCombination() with
            //| Some (status1, status2, msg) -> sprintf "{{%s%s%i%i%s%s" (RecordedOn.value this.TimeStamp) (ParticipantID.value this.ParticipantID) status1 status2 msg (if this.Cancelled then "C" else String.Empty)
            //| None -> String.Empty
            sprintf "{{%s%s%i%i%s%s" (RecordedOn.value this.TimeStamp) (ParticipantID.value this.ParticipantID) status1 status2 msg (if this.Cancelled then "C" else String.Empty)