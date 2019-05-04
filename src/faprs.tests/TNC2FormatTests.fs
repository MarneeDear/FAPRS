module TNC2FormatTests

open Expecto
open faprs.core.TNC2MON
open faprs.core.Common
open faprs.core.APRSData
open System

[<Literal>]
let SENDER = "kg7sio"
[<Literal>]
let DESTINATION = "apdw15" //DireWolf v1.5 ToCall
[<Literal>]
let PATH = "WIDE1-1"
let LATITUDE = 36.0591117 //DD decimal degrees
let LONGITUDE = -112.1093343 //DD decimal degrees
let LONGITUDE_HEMISPHERE = LongitudeHemisphere.East // 'E'
let LATITUDE_HEMISPHERE =  LatitiudeHemisphere.North // 'N'
let POSITION_REPORT_HOUSE = sprintf "=%.2f%c/%.2f%c-" LATITUDE (LATITUDE_HEMISPHERE.ToHemisphereChar()) LONGITUDE (LONGITUDE_HEMISPHERE.ToHemisphereChar())
let FINAL_POSITION_REPORT = "=3603.55N/11206.56W-"
let TNC2_FINAL = (sprintf "%s>%s,%s:%s" (SENDER.ToUpper()) (DESTINATION.ToUpper()) PATH FINAL_POSITION_REPORT)


let PACKET_POSITION_REPORT_HOUSE =
    { 
        Position = { 
            Latitude = FormattedLatitude.create LATITUDE 
            Longitude = FormattedLongitude.create LONGITUDE 
        } 
        Symbol = SymbolCode.House
        Comment = PositionReportComment.create String.Empty
    }

//TODO introduce property based testsing?

[<Tests>]
let TNC2MONFormatTests =
    testList "TNC2MON Format Tests" [
        testCase "Can build a packet with Position Report with latitude and longitude and upper call sign" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create (SENDER.ToUpper())
                    Destination = CallSign.create (DESTINATION.ToUpper())
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReportWithoutTimeStamp PACKET_POSITION_REPORT_HOUSE)
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2MON formats didnt match")
        testCase "Can build a packet with Position Report with latitude and longitude and lower callsign goes to upper" <| fun _ ->
            let packet = 
                {
                    Sender      = CallSign.create SENDER
                    Destination = CallSign.create DESTINATION
                    Path        = WIDEnN //"WIDE1-1"
                    Message     = Some (PositionReportWithoutTimeStamp PACKET_POSITION_REPORT_HOUSE)
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2 formats didnt match")
    ]
