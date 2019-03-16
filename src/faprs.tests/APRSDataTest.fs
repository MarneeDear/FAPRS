module APRSDataTests

open Expecto
open faprs.core.APRSData

[<Literal>] 
let BIG_MESSAGE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

[<Tests>]
let APRSDataTests =
    testList "Position Report Tests" [
        testCase "Position Report comment longer than 43 characters fails" <| fun _ ->
            Expect.throws (fun _ -> (PositionReportComment.create BIG_MESSAGE) |> ignore) "Comment was longer than 43 characters but it dit not fail"
    ]