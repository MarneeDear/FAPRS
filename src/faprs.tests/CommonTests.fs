module CommonTests

open Expecto
open faprs.core.Common

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
            Expect.isNone (CallSign.create "") "Call Sign is too short but was not caught"
        testCase "Fail to create too long call sign" <| fun _ ->
            Expect.isNone (CallSign.create "1234567890") "Call Sign is too long but was not caught"
        testCase "Can create a call sign with 1 to 9 characters" <| fun _ ->
            Expect.equal (CallSign.value((CallSign.create "KG7SIO").Value)) "KG7SIO" "Call Sign was not created"
    ]

[<Tests>]
let PathTests =
    testList "Path Tests" [
        testCase "WIDE11 return WIDE1-1" <| fun _ ->
            Expect.equal (WIDE11.ToString()) "WIDE1-1" "Did not return the expected string representation"
        testCase "WIDE21 return WIDE2-1" <| fun _ ->
            Expect.equal (WIDE21.ToString()) "WIDE2-1" "Did not return the expected string representation"
        testCase "WIDE22 return WIDE2-2" <| fun _ ->
            Expect.equal (WIDE22.ToString()) "WIDE2-2" "Did not return the expected string representation"
    ]