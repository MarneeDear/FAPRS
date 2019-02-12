module APRSDataTests

open Expecto
open faprs.domain.TNC2MON
open faprs.domain.Common
open faprs.domain.APRSData
open faprs.domain

[<Literal>] 
let BIG_MESSAGE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

[<Tests>]
let APRSDataTests =
    testList "APRS Data Tests" [
        testCase "Position Report comment longer than 43 characters fails" <| fun _ ->
            Expect.throws (fun _ -> (PositionReportComment.create BIG_MESSAGE) |> ignore) "Comment was longer than 43 characters but it dit not fail"
    ]