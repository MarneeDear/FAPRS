namespace faprs.domain

//TODO module APRSDataExtensions

[<AutoOpen>]
module APRSData = 

    type LatitiudeHemisphere =
        | North     
        | South     
        member this.ToHemisphereChar() =
            match this with
            | North _   -> 'N'
            | South _   -> 'S'

    type LongitudeHemisphere = 
        | East      
        | West      
        member this.ToHemisphereChar() =
            match this with
            | East _    -> 'E'
            | West _    -> 'W'


    type Latitude =
        {
            Degrees : decimal
            Hemisphere : LatitiudeHemisphere
        }
        override this.ToString() =
            sprintf "%.2f %c" this.Degrees (this.Hemisphere.ToHemisphereChar())


    type Longitude =
        {
            Degrees : decimal
            Hemisphere : LongitudeHemisphere
        }
        override this.ToString() =
            sprintf "%.2f %c" this.Degrees (this.Hemisphere.ToHemisphereChar())

//TODO constrain the size of the Degrees field
// Latitude is expressed as a fixed 8-character field, in degrees and decimal
// minutes (to two decimal places), followed by the letter N for north or S for
// south.
// Longitude is expressed as a fixed 9-character field, in degrees and decimal
// minutes (to two decimal places), followed by the letter E for east or W for
// west.

    type Position =
        {
            Latitude : Latitude //TODO make latititude and longitude their own things
            Longitude : Longitude
        }
//TODO support more position report types -- data extensions
    type PositionReport =
        {
            Position : Position
            //TODO Comment =%s/%s-
        }
        override this.ToString() =
            sprintf "=%s/%s" (this.Position.Latitude.ToString()) (this.Position.Longitude.ToString())

    let positionReport lat lon =
        {
            Position = { Latitude = lat; Longitude = lon }
        }

    type Message =
        | PlainText         of string
        | PositionReport    of PositionReport
        override this.ToString() =
            match this with 
            | PlainText m -> m
            | PositionReport p -> p.ToString()
