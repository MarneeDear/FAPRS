module FrameActivePatternsTests

open Expecto
open faprs.core.FrameActivePatterns
open System
open faprs.core

//TODO would this be a smarter way to do this? https://stackoverflow.com/questions/2671491/f-active-pattern-list-filter-or-equivalent?rq=1

let wasFound pattern value = 
  match pattern value with
  | Some _ -> true
  | None -> false

[<Tests>]
let FrameParsingTests =
    testList "Parse kiss util frames" [
        testCase "Can I do this?" <| fun _ ->
            Expect.isTrue (wasFound (|Frame|_|) "[0] KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2:=3216.4N/11057.3Wbblah:blah /fishcakes") "DID THIS THING WORK"
        testCase "Good record returns a good frame" <| fun _ -> 
            let result = 
                match "[0] KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2:=3216.4N/11057.3Wbblah:blah /fishcakes" with
                | Frame f   -> f
                | _         -> String.Empty
            Expect.equal result "KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2:=3216.4N/11057.3Wbblah:blah /fishcakes" "Could not parse frame" 
        testCase "Empty record should return None" <| fun _ ->
            let result =
                match "" with
                | Frame f   -> Some f
                | _         -> None
            Expect.isNone result "Empty record should have returned None"
        testCase "Channel only record should return None" <| fun _ ->
            let result = 
                match "[0]" with
                | Frame f   -> Some f
                | _         -> None
            Expect.isNone result "Channel-only record should have returned None"
        testCase "Bad record full of randos should return None" <| fun _ ->
            let result = 
                match "klghfdlkhg>fdgkgkh)(),,<<sdglkgffkgdfhg=:gfgdgfddfgd" with
                | Frame f   -> Some f
                | _         -> None
            Expect.isNone result "Rando string should not have been parsed"
    ]

[<Tests>]
let AddressParsingTests =
    testList "Parse address elements" [
        testCase "Can parse address in good record with message" <| fun _ ->
            let result =
                match "KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2:=3216.4N/11057.3Wb,b>,lah:blah /fishcakes" with
                | Address a -> a
                | _         -> String.Empty
            Expect.equal result "KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2" "Address was not parsed correctly"
        testCase "Can parse sender in good address" <| fun _ ->
            let result =
                match "KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2" with
                | Sender s  -> s
                | _         -> String.Empty
            Expect.equal result "KG7SIO-7" "Sender did not match."
        testCase "Sender in malformed address is not parsed" <| fun _ ->
            let result = 
                match "KG7SIO-7+APRD15-WIDE1-CPXX*,qAX,CWOP-2" with
                | Address a -> Some a
                | _         -> None
            Expect.isNone result "Address should not have been parsed."  
        testCase "Can parse Destination in good address" <| fun _ ->
            let result =
                match "KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2" with
                | Destination d -> d
                | _             -> String.Empty
            Expect.equal result "APRD15" "Destination did not match."
        testCase "Destination in malformed address is not parsed" <| fun _ ->
            let result =
                match "KG7SIO-7+APRD15-WIDE1-CPXX*,qAX,CWOP-2" with
                | Destination d -> Some d
                | _             -> None
            Expect.isNone result "Destination should not have been parsed"
        testCase "Can parse Path in good address" <| fun _ ->
            let result =
                match "KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2" with
                | Path p    -> p |> Array.toSeq
                | _         -> Seq.empty
            Expect.containsAll result (["WIDE1-1"; "TCPXX*"; "qAX"; "CWOP-2"] |> List.toSeq)  "Did not contain expected paths"   
            //(["WIDE1-1"; "TCPXX*"; "qAX"; "CWOP-2"] |> List.toSeq)             
            //|> List.iter (fun p -> Expect.contains path p "Path not found in expected paths list")
        testCase "Path in malformed address is not parsed" <| fun _ ->
            let result =
                match "KG7SIO-7+APRD15-WIDE1-CPXX*,qAX,CWOP-2" with
                | Path p    -> Some (p |> Array.toSeq)
                | _         -> None 
            Expect.isNone result "Path should not have been parsed"
    ]

[<Tests>]
let MessageParsingTests =
    testList "Message Parsing Tests" [
        testCase "Can get message part of well formed frame with message" <| fun _ ->
            let result =
                match "[0] KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2:=03216.4N/011057.3Wb,b>,lah:blah /fishcakes" with
                | FrameActivePatterns.Message m -> m
                | _ -> String.Empty
            Expect.equal result "=03216.4N/011057.3Wb,b>,lah:blah /fishcakes" "Message does not match"
        testCase "Can get Latitude from well formed message position report" <| fun _ ->
            let result =
                match "=03216.4N/011057.3Wb,b>,lah:blah /fishcakes" with
                | Latitude l -> l
                | _ -> String.Empty
            Expect.equal result "03216.4N" "Latitude did not match"
        testCase "Latitude in malformed position report cannot be parsed" <| fun _ ->
            let result =
                match "=vvv03216.4N'0011057.3Wb,b>,lah:blah /fishcakes" with
                | Latitude l -> Some l
                | _ -> None
            Expect.isNone result "Latitude should not have been parsed"
        testCase "Can get Longitude from well formed position report" <| fun _ -> 
            let result =
                match "=03216.4N/011057.3Wb,b>,lah:blah /fishcakes" with
                | Longitude l -> l
                | _ -> String.Empty
            Expect.equal result "011057.3W" "Longitude did not match"
        testCase "Longitude in malformed position report cannot be parsed" <| fun _ ->
            let result =
                match "-03216.4W/011057.3Sb,b>,lah:blah /fishcakes" with
                | Longitude l -> Some l
                | _ -> None
            Expect.isNone result "Longitude should not have been parsed."
        testCase "Can get Symbol from well formed position report" <| fun _ ->
            let result =
                match "=03216.4N/011057.3Eb,b>,lah:blah /fishcakes" with
                | Symbol s -> Some s
                | _ -> None
            Expect.equal result (Some Bicycle) "Symbol did not match"
        testCase "Symbol in malformed position report cannot be parsed" <| fun _ ->
            let result =
                match "=03216.4I`011057.3Lb,b>,lah:blah /fishcakes" with
                | Symbol b -> Some b
                | _ -> None
            Expect.isNone result "Symbol should not have been parsed"
        testCase "Can get comment from well formed message with position report" <| fun _ ->
            let result = 
                match ("=03216.4N/011057.3Wb,b>,lah:blah /fishcakes") with
                | Comment 'b' c -> c 
                | _ -> String.Empty
            Expect.equal result ",b>,lah:blah /fishcakes" "Comment did not match"
        //testCase "Can parse well formed position report in well formed frame" <| fun _ ->
        //    let 
    ]
