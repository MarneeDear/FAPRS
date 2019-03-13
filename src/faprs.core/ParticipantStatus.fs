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
* Destination identifies the HQ
 

 NOT USING THIS PROBABLY Date/Time format 2009-06-15T13:45:30 -- yyyy-MM-ddTHH:mm:ss

Month/Day/Hours/Minutes (MDHM) format is a fixed 8-character field,
consisting of the month (01–12) and day-of-the-month (01–31), followed by
the time in hours and minutes zulu. For example:

10092345 is 23 hours 45 minutes zulu on October 9th.

If cycling through messages, participantStatus expires at midnight, 
but can be renewed for the next day 

Experimental User-Defined types start with {{ (double curly braces)

*)
module ParticipantStatus = 
    open System

    type RecordedOn = private RecordedOn of string
    module RecordedOn =
        let create =
            let utcNow = DateTime.UtcNow
            sprintf "%0i%0i%0i%0i" utcNow.Day utcNow.Month utcNow.Hour utcNow.Minute
        let value (RecordedOn d) = d

    type ParticipantStatusMessage = private ParticipantStatusMessage of string
    module ParticipantStatusMessage =
        let create (s:string) =
            match (s.Trim()) with
            | s when s.Length <= 244 -> ParticipantStatusMessage s
            | _ -> ParticipantStatusMessage (s.Substring(0, 243))
        let value (ParticipantStatusMessage s) = s

    type ParticipantStatus =
        | Continued     of ParticipantStatusMessage
        | Injured       of ParticipantStatus
        | Resting       of ParticipantStatusMessage
        | NeedsSupport  of ParticipantStatusMessage
        member this.ToStatusCombination() =
            match this with
            | Continued m       -> (1, 1, ParticipantStatusMessage.value m)
            | Injured s         ->  match s with
                                    | Continued m       -> (1, 2, ParticipantStatusMessage.value m)
                                    | Resting m         -> (3, 2, ParticipantStatusMessage.value m)
                                    | NeedsSupport m    -> (4, 2, ParticipantStatusMessage.value m)
                                    | _                 -> failwith "Injured can only be continued, resting, needs support."
            | Resting m         -> (3, 3, ParticipantStatusMessage.value m)
            | NeedsSupport m    -> (4, 4, ParticipantStatusMessage.value m)