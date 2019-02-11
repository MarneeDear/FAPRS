module TNC2FormatTests

open Expecto
open faprs.domain.TNC2MON
open faprs.domain.Common
open faprs.domain.APRSData
open System

[<Literal>]
let SENDER = "kg7sio"
[<Literal>]
let DESTINATION = "kg7sil"
[<Literal>]
let PATH = "WIDE1-1"
let LATITUDE = 111.11M
let LONGITUDE = 222.22M
let LONGITUDE_HEMISPHERE = LongitudeHemisphere.East // 'E'
let LATITUDE_HEMISPHERE =  LatitiudeHemisphere.North // 'N'
let POSITION_REPORT = sprintf "=%.2f %c/%.2f %c" LATITUDE (LATITUDE_HEMISPHERE.ToHemisphereChar()) LONGITUDE (LONGITUDE_HEMISPHERE.ToHemisphereChar())
let TNC2_FINAL = (sprintf "%s>%s,%s:%s" (SENDER.ToUpper()) (DESTINATION.ToUpper()) PATH POSITION_REPORT)

[<Tests>]
let tests =
    testList "TNC Format Tests" [
        testCase "Can build a simple Position Report with latitude and longitude" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create (SENDER.ToUpper())
                    Destination = CallSign.create (DESTINATION.ToUpper())
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReport { Position = { Latitude = { Degrees = LATITUDE; Hemisphere = LATITUDE_HEMISPHERE }; Longitude = { Degrees = LONGITUDE; Hemisphere = LONGITUDE_HEMISPHERE} } })
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2 formats didnt match")
        testCase "Can build a simple Position Report with latitude and longitude and lower callsign goes to upper" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create SENDER
                    Destination = CallSign.create DESTINATION
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReport { Position = { Latitude = { Degrees = LATITUDE; Hemisphere = LATITUDE_HEMISPHERE }; Longitude = { Degrees = LONGITUDE; Hemisphere = LONGITUDE_HEMISPHERE} } })
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2 formats didnt match")

    ]