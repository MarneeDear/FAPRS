module TNC2MONRepositoryTests

open Expecto
open faprs.core.TNC2MON
open faprs.core.Common
open faprs.core.APRSData
open faprs.core

[<Literal>]
let PATH = @""

[<Tests>]
let WriteTNC2PacketTests =
    testList "Write packet to file" [
        testCase "A file is created in the path provided" <| fun _ ->
            
    ]