module CommonTests

open Expecto
open fapr.core.Common

[<Tests>]
let SymbolCodeTests =
    testList "Symbol Code Tests" [
        testCase "House" <| fun _ ->
            Expect.equal (SymbolCode.House.ToChar()) '-' "House code should be -"
        testCase "Bicycle" <| fun _ ->
            Expect.equal (SymbolCode.Bicycle.ToChar()) 'b' "Bicycle code should be b"
        testCase "Balloon" <| fun _ ->
            Expect.equal (SymbolCode.Balloon.ToChar()) 'O' "Balloon code should be O"
        testCase "Hospital" <| fun _ ->
            Expect.equal (SymbolCode.Hospital.ToChar()) 'h' "Hospital code should be h"
        testCase "Jeep" <| fun _ ->
            Expect.equal (SymbolCode.Jeep.ToChar()) 'j' "Jeep code should be j"
        testCase "Truck" <| fun _ ->
            Expect.equal (SymbolCode.Truck.ToChar()) 'k' "Truck code should be k"
        testCase "Motorcycle" <| fun _ ->
            Expect.equal (SymbolCode.Motorcycle.ToChar()) '<' "Motorcyle code should be <"
        testCase "Jogger" <| fun _ ->
            Expect.equal (SymbolCode.Jogger.ToChar()) '[' "Jogger code should be ["
    ]

[<Tests>]
let CallSignTests =
    testList "Call Sign Tests" [
        testCase "Fail to create a too short call sign" <| fun _ ->
            Expect.throws (fun _ -> (CallSign.create "") |> ignore) "Call Sign is too short but was not caught"
        testCase "Fail to create too long call sign" <| fun _ ->
            Expect.throws (fun _ -> ((CallSign.create "1234567890") |> ignore)) "Call Sign is too long but was not caught"
        testCase "Can create a call sign with 1 to 9 characters" <| fun _ ->
            Expect.equal (CallSign.value((CallSign.create "KG7SIO"))) "KG7SIO" "Call Sign was not created"
    ]

[<Tests>]
let PathTests =
    testList "Path Tests" [
        testCase "WIDEnN return WIDE1-1" <| fun _ ->
            Expect.equal (Path.WIDEnN.ToString()) "WIDE1-1" "WIDE1-1 only supported"
        testCase "Non-WIDEnN fails - ECHO" <| fun _ ->
            Expect.throws (fun _ -> Path.ECHO.ToString() |> ignore) "ECHO is not supported but it did not fail"
        testCase "Non-WIDEnN fails - GATE" <| fun _ ->
            Expect.throws (fun _ -> Path.GATE.ToString() |> ignore) "GATE is not supported but it did not fail"
    ]