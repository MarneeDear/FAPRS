module TNC2MONRepositoryTests

open Expecto
open faprs.core.TNC2MON
open faprs.core.Common
open faprs.core.APRSData
open faprs.infrastructure.TNC2MONRepository
open System
open System.IO

[<Literal>]
let FILE_PATH = @"."

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

let PACKET = 
    {
        Sender      = CallSign.create (SENDER.ToUpper())
        Destination = CallSign.create (DESTINATION.ToUpper())
        Path        = WIDEnN //"WIDE1-1"
        Message     = Some (PositionReport PACKET_POSITION_REPORT_HOUSE)
    }

[<Tests>]
let WriteTNC2PacketTests =
    testList "Write packet to a kissutil frame file" [
        testCase "A file is created in the path provided" <| fun _ ->
            let timestamp = (DateTime.Now.ToString("yyyyMMddHHmmssff"))
            writeKissUtilFrame None [PACKET] FILE_PATH timestamp
            Expect.isTrue (File.Exists(Path.Combine(Path.GetFullPath(FILE_PATH), sprintf "%s%s" timestamp "faprs.txt"))) "The frame file was created."
    ]

//[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
//[<Tests>]
//let ReadTNC2PacketTests =
//    testList "Read kissutil frames and create TNC2MON packets" [
//        testCase ""
//    ]