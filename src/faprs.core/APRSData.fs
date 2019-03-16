namespace faprs.core

//TODO module APRSDataExtensions
//TODO how to model SSIDs ?  APRS SSID Recommendations
//TODO include SSIDs
//http://www.aprs.org/aprs11/SSIDs.txt

[<AutoOpen>]
module APRSData = 

    type PositionReportComment = private PositionReportComment of string
    module PositionReportComment =
        let create (s:string) =
            match (s.Trim()) with
            | s when s.Length < 44  -> PositionReportComment s
            | _                     -> failwith "Position Report Comment must be less than 43 characters long."

        let value (PositionReportComment c) = c //Was trimmed during create

    type LatitiudeHemisphere =
        | North     
        | South     
        member this.ToHemisphereChar() =
            match this with
            | North _   -> 'N'
            | South _   -> 'S'

    let getLatHemisphere h =
        match h with
        | 'N' -> LatitiudeHemisphere.North
        | 'S' -> LatitiudeHemisphere.South
        | _ -> failwith "Latitude must be in northern (N) or southern (S) hemisphere."

    type LongitudeHemisphere = 
        | East      
        | West      
        member this.ToHemisphereChar() =
            match this with
            | East _    -> 'E'
            | West _    -> 'W'

    let getLonHemisphere h =
        match h with
        | 'E' -> LongitudeHemisphere.East
        | 'W' -> LongitudeHemisphere.West
        | _ -> failwith "Longitude must be in eastern (E) or western (W) hemisphere."

    let hemisphereToString degrees hemisphereChar =
        sprintf "%.2f%c" degrees hemisphereChar

    //TODO constrain the size of the Degrees field
    (* 
    APRS101.pdf Chapter 6: Time and Position Formats
    Latitude is expressed as a fixed 8-character field, in degrees and decimal
    minutes (to two decimal places), followed by the letter N for north or S for
    south.

    For example:
    07201.75W is 72 degrees 1 minute 45 seconds west.
    In generic format examples, the longitude is shown as the 9-character string
    dddmm.hhW (i.e. degrees, minutes and hundredths of a minute west).

    *)
    type Latitude =
        {
            Degrees : float
            Hemisphere : LatitiudeHemisphere
        }
        override this.ToString() =
            hemisphereToString this.Degrees (this.Hemisphere.ToHemisphereChar())
            //sprintf "%.2f%c" this.Degrees (this.Hemisphere.ToHemisphereChar())

    type Longitude =
        {
            Degrees : float
            Hemisphere : LongitudeHemisphere
        }
        override this.ToString() =
            hemisphereToString this.Degrees (this.Hemisphere.ToHemisphereChar())
            //sprintf "%.2f%c" this.Degrees (this.Hemisphere.ToHemisphereChar())

    (* 
    APRS101.pdf Chapter 6: Time and Position Formats
    Longitude is expressed as a fixed 9-character field, in degrees and decimal
    minutes (to two decimal places), followed by the letter E for east or W for
    west.

    For example:
    07201.75W is 72 degrees 1 minute 45 seconds west.
    In generic format examples, the longitude is shown as the 9-character string
    dddmm.hhW (i.e. degrees, minutes and hundredths of a minute west).
    *)
    type Position =
        {
            Latitude : Latitude 
            Longitude : Longitude
        }

    //TODO support more position report types -- data extensions
    (*
    APRS101.pdf Chapter 6: Time and Position Formats
    Must end in a Symbol Code
    Position coordinates are a combination of latitude and longitude, separated
    by a display Symbol Table Identifier, and followed by a Symbol Code. 
    *)
    type PositionReportWithoutTimeStamp =
        {
            Position : Position
            Symbol : SymbolCode
            Comment : PositionReportComment
        }
        override this.ToString() =
            sprintf "=%s/%s%c%s" (this.Position.Latitude.ToString()) (this.Position.Longitude.ToString()) (this.Symbol.ToChar()) (PositionReportComment.value this.Comment)
    
    type UnformattedMessage = private UnformattedMessage of string
    module UnformattedMessage =
        let create (m:string) =
            match (m.Trim()) with
            | m when m.Length <= 255 -> UnformattedMessage m //AX.25 field is 256 chars but the message has to accomodate the { for user defined messages
            | _ -> UnformattedMessage (m.Substring(0, 255)) //TODO or throw an exception?
        let value (UnformattedMessage m) = sprintf ":%s" m

    type Message =
        | Unformatted                       of UnformattedMessage
        | PositionReportWithoutTimeStamp    of PositionReportWithoutTimeStamp
        | ParticipantStatusReport           of Participant.ParitipantStatusReport
        override this.ToString() =
            match this with 
            | Unformatted m                     -> UnformattedMessage.value m // (:) is the aprs data type ID for message
            | PositionReportWithoutTimeStamp r  -> r.ToString()
            | ParticipantStatusReport r         -> r.ToString()

    type SSID =
        | PrimaryStation 
        | Generic_1
        | Generic_2
        | Generic_3
        | Generic_4
        | Other
        | SpecialActivity
        | HumanPortable
        | SecondMainMobile
        | PrimaryMobile
        | InternetLink
        | Aircraft
        | TTDevices
        | WeatherStation
        | FullTimeDriver
        | Generic_15
        member this.ToInt() =
            match this with
            | PrimaryStation    -> 0
            | Generic_1         -> 1
            | Generic_2         -> 2
            | Generic_3         -> 3
            | Generic_4         -> 4
            | Other             -> 5
            | SpecialActivity   -> 6
            | HumanPortable     -> 7
            | SecondMainMobile  -> 8
            | PrimaryMobile     -> 9
            | InternetLink      -> 10
            | Aircraft          -> 11
            | TTDevices         -> 12
            | WeatherStation    -> 13
            | FullTimeDriver    -> 14
            | Generic_15        -> 15

    let getSSID ssid =
        match ssid with
        | 0     -> PrimaryStation
        | 1     -> Generic_1
        | 2     -> Generic_2
        | 3     -> Generic_3
        | 4     -> Generic_4
        | 5     -> Other
        | 6     -> SpecialActivity
        | 7     -> HumanPortable
        | 8     -> SecondMainMobile
        | 9     -> PrimaryMobile
        | 10    -> InternetLink
        | 11    -> Aircraft
        | 12    -> TTDevices
        | 13    -> WeatherStation
        | 14    -> FullTimeDriver
        | 15    -> Generic_15
        | _     -> failwith "Unknown SSID number."




