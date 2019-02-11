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

    type Longitude =
        {
            Degrees : decimal
            Hemisphere : LongitudeHemisphere
        }


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
        member this.LatitudeToString() =
            sprintf "%.2f %c" this.Latitude.Degrees (this.Latitude.Hemisphere.ToHemisphereChar())
        member this.LongitudeToString() =
            sprintf "%.2f %c" this.Longitude.Degrees (this.Longitude.Hemisphere.ToHemisphereChar())

//TODO support more position report types -- data extensions
    type PositionReport =
        {
            Position : Position
            //TODO Comment
        }
        override this.ToString() =
            sprintf "=%s/%s-" (this.Position.LatitudeToString()) (this.Position.LongitudeToString())

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
