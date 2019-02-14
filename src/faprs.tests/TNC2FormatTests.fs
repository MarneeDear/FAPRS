module TNC2FormatTests

open Expecto
open fapr.core.TNC2MON
open fapr.core.Common
open fapr.core.APRSData
open System
open fapr.core

[<Literal>]
let SENDER = "kg7sio"
[<Literal>]
let DESTINATION = "kg7sil"
[<Literal>]
let PATH = "WIDE1-1"
let LATITUDE = 111.11
let LONGITUDE = 222.2
let LONGITUDE_HEMISPHERE = LongitudeHemisphere.East // 'E'
let LATITUDE_HEMISPHERE =  LatitiudeHemisphere.North // 'N'
let POSITION_REPORT_HOUSE = sprintf "=%.2f%c/%.2f%c-" LATITUDE (LATITUDE_HEMISPHERE.ToHemisphereChar()) LONGITUDE (LONGITUDE_HEMISPHERE.ToHemisphereChar())
let TNC2_FINAL = (sprintf "%s>%s,%s:%s" (SENDER.ToUpper()) (DESTINATION.ToUpper()) PATH POSITION_REPORT_HOUSE)

let PACKET_POSITION_REPORT_HOUSE =
    { 
        Position = { 
            Latitude = { Degrees = LATITUDE; Hemisphere = LATITUDE_HEMISPHERE }
            Longitude = { Degrees = LONGITUDE; Hemisphere = LONGITUDE_HEMISPHERE} 
        } 
        Symbol = SymbolCode.House
        Comment = PositionReportComment.create String.Empty
    }

//TODO introduce property based testsing?

[<Tests>]
let TNCFormatTests =
    testList "TNC Format Tests" [
        testCase "Can build a packet with Position Report with latitude and longitude and upper call sign" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create (SENDER.ToUpper())
                    Destination = CallSign.create (DESTINATION.ToUpper())
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReport PACKET_POSITION_REPORT_HOUSE)
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2 formats didnt match")
        testCase "Can build a packet with Position Report with latitude and longitude and lower callsign goes to upper" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create SENDER
                    Destination = CallSign.create DESTINATION
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReport PACKET_POSITION_REPORT_HOUSE)
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2 formats didnt match")
    ]


// [<Tests>]
//     let 