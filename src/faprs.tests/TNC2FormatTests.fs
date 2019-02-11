module TNC2FormatTests

open Expecto
open faprs.domain.TNC2MON
open faprs.domain.Common
open faprs.domain.APRSData
open System

[<Tests>]
let tests =
    testList "TNC Format Tests" [
        testCase "Can build a simple Position Report with latitude and longitude" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create "KG7SIO"
                    Destination = CallSign.create "KG7SIL"
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReport { Position = { Latitude = { Degrees = 111.11M; Hemisphere = North }; Longitude = { Degrees = 222.22M; Hemisphere = East} } })
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet (sprintf "%s>%s,%s:%s" "KG7SIO" "KG7SIL" "WIDE1-1" "=111.11 N/222.22 E-") (sprintf "TNC2 formats didnt match")
        testCase "Can build a simple Position Report with latitude and longitude and lower callsign goes to upper" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create "kg7sio"
                    Destination = CallSign.create "kg7sil"
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReport { Position = { Latitude = { Degrees = 111.11M; Hemisphere = North }; Longitude = { Degrees = 222.22M; Hemisphere = East} } })
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet (sprintf "%s>%s,%s:%s" "KG7SIO" "KG7SIL" "WIDE1-1" "=111.11 N/222.22 E-") (sprintf "TNC2 formats didnt match")

    ]