module AX25Tests

open Expecto
open faprs.core.AX25
open System
open faprs.core
open faprs.core

//From https://www.ietf.org/rfc/rfc1549.txt
//CALCULATORS
//https://www.scadacore.com/tools/programming-calculators/online-checksum-calculator/
//https://crccalc.com/
[<Literal>]
let PPPGOODFCS16    = 0xB518us //= 0xf0b8us  //* Good final FCS value */ TODO find out if this is right
//[<Literal>]
let PPPINITFCS16    = 0xffffus  //Initial FCS value

//[<Tests>]
//let CRC16Tests =
//    testList "Compute CRC-16 CCITT" [
//        testCase "Can compute test string" <| fun _ ->
//            let result = FrameCheckSequence.CRC_16_with_table (Seq.singleton (uint16 PPPINITFCS16))
//            Expect.equal (result.ToString("x2")) (PPPGOODFCS16.ToString("x2")) "Frame check sequence was wrong."
//        testCase "Manual compute" <| fun _ ->
//            //let b = 1uy //WORKS FOR CRC-16/MCRF4XX 
//            let barray = BitConverter.GetBytes(0x6e4cus) // [|1uy|] //; 1uy; 1uy; 1uy|]

//            let mutable code = PPPINITFCS16
//            for b in barray do
//                Console.WriteLine(b)
//                code <- (code >>> 8) ^^^ FrameCheckSequence.fcstab.[int ((code ^^^ (uint16 b)) &&& 0xffus)]
//                Console.WriteLine(FrameCheckSequence.fcstab.[int ((code ^^^ (uint16 b)) &&& 0xffus)])
//                Console.WriteLine(sprintf "INDEX :: %i" (int ((code ^^^ (uint16 b)) &&& 0xffus)))
//                Console.WriteLine(sprintf "CODE :: %i " code)
//                Console.WriteLine(sprintf "CODE STRING :: %s" (code.ToString("x2")))
//            Console.WriteLine(sprintf "FINAL :: %i" (code ^^^ PPPINITFCS16))
//            Console.WriteLine(sprintf "FINAL STRING :: %s" ((code ^^^ PPPINITFCS16).ToString("x2")))

//            Expect.isTrue false "Manual test"
//        testCase "With polynomial" <| fun _ ->
//            let result = FrameCheckSequence.CRC_16_calculated_polynomial (Seq.singleton 0x1us)
//            Expect.equal (result.ToString("x2")) (0x1us.ToString("x2")) "WHats?"
//    ]