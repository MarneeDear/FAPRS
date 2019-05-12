module APRSDataTests

open Expecto
open faprs.core.APRSData
open System

[<Literal>] 
let BIG_MESSAGE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

(*
TIMESTAMP
Month/Day/Hours/Minutes (MDHM) format is a fixed 8-character field,
consisting of the month (01–12) and day-of-the-month (01–31), followed by
the time in hours and minutes zulu. For example:
*)

[<Literal>]
let GOOD_PARTICIPANT_CONTINUED_STATUS = "1234511We have a winner!"
let TEST_DATE = Convert.ToDateTime("2019-01-01T00:00:00")
[<Literal>]
let TEST_TIMESTAMP = "01010000"
[<Literal>]
let GOOD_PARTICIPANT_STATUS_MSG = "Runner has cut on leg. Not actively bleeding."
[<Literal>]
let TOO_LONG_STATUS_MSG = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu"
[<Literal>]
let GOOD_PARTICIPANT_NBR = "12345"

             //latitidue   longitude
let N_E_DC = ((32.0616684, 118.7777786),   ("3203.42N", "11846.40E")) //Nanking, China
let S_E_DC = ((-33.8678513, 151.2073212) , ("3352.04S", "15112.26E")) //Sydney, Australia

let N_W_DC = ((33.4483795, -112.0740433), ("3326.54N", "11204.27W")) //Phoenix, Arizona
let S_W_DC = ((-33.4569397, -70.6482697), ("3327.25S", "07038.54W")) //Santiago, Chile

[<Tests>]
let LocationTests =
    testList "Location tests" [
        testCase "Can convert Nanking latitiude to APRS formatted longitude" <| fun _ -> 
            let dc, dms = N_E_DC
            let dc_lat, _ = dc
            let dms_lat, _ = dms
            "Nanking latitude not converted to expected format."
            |> Expect.equal (FormattedLatitude.value (FormattedLatitude.create dc_lat) ) dms_lat
        testCase "Can convert Phoenix latitude to APRS format" <| fun _ ->
            let dc, dms = S_E_DC
            let dc_lat, _ = dc
            let dms_lat, _ = dms
            "Phoenix latitude not converted to expected format."
            |> Expect.equal (FormattedLatitude.value (FormattedLatitude.create dc_lat) ) dms_lat
        testCase "Can convert Sydney latitude to APRS formatted longitude" <| fun _ -> 
            let dc, dms = N_W_DC
            let dc_lat, _ = dc
            let dms_lat, _ = dms
            "Sydney latitude not converted to expected format."
            |> Expect.equal (FormattedLatitude.value (FormattedLatitude.create dc_lat) ) dms_lat
        testCase "Can convert Santiago latitude to APRS format" <| fun _ ->
            let dc, dms = S_W_DC
            let dc_lat, _ = dc
            let dms_lat, _ = dms
            "Santiago latitude not converted to expected format."
            |> Expect.equal (FormattedLatitude.value (FormattedLatitude.create dc_lat) ) dms_lat
        testCase "Can convert Nanking longitude to APRS formatted longitude" <| fun _ -> 
            let dc, dms = N_E_DC
            let _, dc_long = dc
            let _, dms_long = dms
            "Nanking longitude not converted to expected format."
            |> Expect.equal (FormattedLongitude.value (FormattedLongitude.create dc_long) ) dms_long
        testCase "Can convert Phoenix longitude to expected format." <| fun _ ->
            let dc, dms = N_W_DC
            let _, dc_long = dc
            let _, dms_long = dms
            "Phoenix longitude not converted to expected format."
            |> Expect.equal (FormattedLongitude.value (FormattedLongitude.create dc_long) ) dms_long
        testCase "Can convert Sydney longitude to APRS formatted longitude" <| fun _ -> 
            let dc, dms = S_E_DC
            let _, dc_long = dc
            let _, dms_long = dms
            "Sydney longitude not converted to expected format."
            |> Expect.equal (FormattedLongitude.value (FormattedLongitude.create dc_long) ) dms_long
        testCase "Can convert Santiago longitude to expecte format." <| fun _ ->
            let dc, dms = S_W_DC
            let _, dc_long = dc
            let _, dms_long = dms
            "Santiago longitude not converted to expected format."
            |> Expect.equal (FormattedLongitude.value (FormattedLongitude.create dc_long) ) dms_long
        ]

[<Tests>]
let APRSDataTests =
    testList "Position Report Tests" [
        testCase "Position Report comment longer than 43 characters fails" <| fun _ ->
            Expect.isNone (PositionReportComment.create BIG_MESSAGE) "Comment was longer than 43 characters but it dit not fail"
    ]