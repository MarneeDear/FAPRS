module TNC2FormatTests

open Expecto

[<Tests>]
let tests =
    testList "idunno" [
        testCase "helloworld" <| fun _ -> 
        let imhere = true
        Expect.isTrue imhere "Hello World"
    ]