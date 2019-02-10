namespace faprs.domain

//TODO module APRSDataExtensions

[<AutoOpen>]
module APRSData = 

    type Hemisphere =
        | North     of char
        | South     of char
        | East      of char
        | West      of char
        member this.ToHemisphereChar() =
            match this with
            | North _   -> 'N'
            | South _   -> 'S'
            | East _    -> 'E'
            | West _    -> 'W'


    type Coordinate =
        {
            Degrees : decimal
            Hemisphere : Hemisphere
        }

//TODO constrain the size of the Degrees field
// Latitude is expressed as a fixed 8-character field, in degrees and decimal
// minutes (to two decimal places), followed by the letter N for north or S for
// south.
// Longitude is expressed as a fixed 9-character field, in degrees and decimal
// minutes (to two decimal places), followed by the letter E for east or W for
// west.

    type CoordinateType =
        | Latitude  of Coordinate
        | Longitude of Coordinate

    type Position =
        {
            Latitude : Coordinate
            Longitude : Coordinate
        }
        member this.LatitudeToString() =
            match this.Latitude.Hemisphere with
            | North _   -> ()
            | South _   -> ()
            | _         -> failwith "Latitude hemisphere must be either North or South."
//TODO is throwing an exception here the right way to handle this? ROP?

            sprintf "%.2f %c" this.Latitude.Degrees (this.Latitude.Hemisphere.ToHemisphereChar())
        member this.LongitudeToString() =
            match this.Longitude.Hemisphere with
            | East _    -> ()
            | West _    -> ()
            | _         -> failwith "Longitude hemisphere must be either East or West."

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
