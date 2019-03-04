module AX25Tests

open Expecto
open faprs.core.AX25
open System
open faprs.core

//From https://www.ietf.org/rfc/rfc1549.txt
//CALCULATOR
//https://www.scadacore.com/tools/programming-calculators/online-checksum-calculator/
[<Literal>]
let PPPGOODFCS16    = 0xf0b8us  //* Good final FCS value */ TODO find out if this is right
[<Literal>]
let PPPINITFCS16    = 0xffffus  //Initial FCS value

[<Tests>]
let CRC16Tests =
    testList "Compute CRC-16 CCITT" [
        testCase "Can compute test string" <| fun _ ->
            //* check on input *
            //trialfcs = pppfcs16( PPPINITFCS16, cp, len + 2 );
            //if ( trialfcs == PPPGOODFCS16 )
            //    printf("Good FCS0);
            let result = FrameCheckSequence.CRC_16_with_table (Seq.singleton PPPINITFCS16)
            Expect.equal result PPPGOODFCS16 "Frame check sequence was wrong."
            //Expect.isTrue false "make fail until I can get a real test"
    ]