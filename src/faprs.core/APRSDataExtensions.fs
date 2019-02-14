namespace faprs.core

//TODO module APRSDataExtensions
//TDO how to model SSIDs ?  APRS SSID Recommendations

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

//TODO constrain the size of the Degrees field
// Latitude is expressed as a fixed 8-character field, in degrees and decimal
// minutes (to two decimal places), followed by the letter N for north or S for
// south.
// Longitude is expressed as a fixed 9-character field, in degrees and decimal
// minutes (to two decimal places), followed by the letter E for east or W for
// west.

    type Position =
        {
            Latitude : Latitude 
            Longitude : Longitude
        }

//TODO support more position report types -- data extensions
    type PositionReport =
        {
            Position : Position
            Symbol : SymbolCode
            Comment : PositionReportComment
        }
        override this.ToString() =
            //Must end in a Symbol Code
            //Position coordinates are a combination of latitude and longitude, separated
            //by a display Symbol Table Identifier, and followed by a Symbol Code. 
            sprintf "=%s/%s%c%s" (this.Position.Latitude.ToString()) (this.Position.Longitude.ToString()) (this.Symbol.ToChar()) (PositionReportComment.value this.Comment)

    type Message =
        | PlainText         of string
        | PositionReport    of PositionReport
        override this.ToString() =
            match this with 
            | PlainText m -> m
            | PositionReport p -> p.ToString()
