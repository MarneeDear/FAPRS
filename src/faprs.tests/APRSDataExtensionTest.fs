module APRSDataTests

open Expecto
open faprs.domain.TNC2MON
open faprs.domain.Common
open faprs.domain.APRSData
open faprs.domain

[<Literal>] 
let RANDOM_SEED = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

[<Tests>]
let APRSDataTests =
    testList "APRS Data Tests" [
        testCase "Position Report comment longer than 43 characters fails" <| fun _ ->
            Expect.throws (fun _ -> (PositionReportComment.create RANDOM_SEED) |> ignore) "Comment was longer than 43 characters but it dit not fail"
    ]