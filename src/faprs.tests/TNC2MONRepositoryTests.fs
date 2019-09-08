module TNC2MONRepositoryTests

open Expecto
open faprs.core.TNC2MON
open faprs.core.Common
open faprs.core.APRSData
open faprs.infrastructure.TNC2MONRepository
open faprs.infrastructure.DireWolf.KissUtil
open System
open System.IO

[<Literal>]
let FILE_PATH = @"."
[<Literal>]
let REC = "REC"
[<Literal>]
let XMIT = "XMIT"

[<Literal>]
let SENDER = "kg7sio"
[<Literal>]
let DESTINATION = "apdw15"  //DireWolf v1.15 ToCall 
[<Literal>]
let PATH = "WIDE1-1"
let LATITUDE = 36.0591117
let LONGITUDE = -112.1093343
let LONGITUDE_HEMISPHERE = LongitudeHemisphere.East // 'E'
let LATITUDE_HEMISPHERE =  LatitudeHemisphere.North // 'N'
let POSITION_REPORT_HOUSE = sprintf "=%.2f%c/%.2f%c-" LATITUDE (LATITUDE_HEMISPHERE.ToHemisphereChar()) LONGITUDE (LONGITUDE_HEMISPHERE.ToHemisphereChar())
let TNC2_FINAL = (sprintf "%s>%s,%s:%s" (SENDER.ToUpper()) (DESTINATION.ToUpper()) PATH POSITION_REPORT_HOUSE)

let PACKET_POSITION_REPORT_HOUSE =
    { 
        Position = { 
            Latitude = FormattedLatitude.create LATITUDE  //{ Degrees = LATITUDE; Hemisphere = LATITUDE_HEMISPHERE }
            Longitude = FormattedLongitude.create LONGITUDE //{ Degrees = LONGITUDE; Hemisphere = LONGITUDE_HEMISPHERE} 
        } 
        Symbol = SymbolCode.House
        Comment = None //(PositionReportComment.create String.Empty).Value
    }

let PACKET = 
    {
        Sender      = (CallSign.create (SENDER.ToUpper())).Value
        Destination = (CallSign.create (DESTINATION.ToUpper())).Value
        Path        = WIDEnN WIDE11 //"WIDE1-1"
        Information = Some (PositionReportWithoutTimeStamp PACKET_POSITION_REPORT_HOUSE)
    }

[<Tests>]
let WriteTNC2RecordTests =
    testList "Write record to a kissutil file" [
        testCase "A file is created in the path provided" <| fun _ ->
            let timestamp = (DateTime.Now.ToString("yyyyMMddHHmmssff"))
            writeKissUtilRecord None [PACKET] FILE_PATH timestamp
            Expect.isTrue (File.Exists(Path.Combine(Path.GetFullPath(FILE_PATH), sprintf "%sfaprs.txt" timestamp))) "The frame file was created."
    ]

[<Tests>]
let ProcessTNC2RecordTests =
    //TODO FAPRS only accepts certain formats
    testList "Read records in a received kissutil file" [
        //testCase "Can read files created by faprs and create an array of valid aprs messages" <| fun _ ->
        //    let timestamp = (DateTime.Now.ToString("yyyyMMddHHmmssff"))
        //    writeKissUtilRecord None [PACKET] (Path.Combine(FILE_PATH, XMIT)) timestamp
        //    let frames = processKissUtilFrames (Path.Combine(FILE_PATH, XMIT)) (Some (sprintf "%sfaprs.txt" timestamp))
        //    frames |> Array.iter (fun f -> Expect.isOk f "frame was not parsed")
        testCase "Can read files created by DireWolf and create an array of valid aprs messages" <| fun _ ->
            let frames = processKissUtilFrames (Path.Combine(FILE_PATH, REC)) None              
            frames |> Array.iter (fun f -> Expect.isOk f "frame was not parsed")
    ]

//[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
//[<Tests>]
//let ReadTNC2PacketTests =
//    testList "Read kissutil frames and create TNC2MON packets" [
//        testCase ""
//    ]