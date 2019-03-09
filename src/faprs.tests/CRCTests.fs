module CRCTests

open Expecto
open faprs.crc
open faprs.core.FrameCheckSequence
open System
open System.Text

//MCRF$XX seems to be the only one that works consistently with https://crccalc.com/
//It does seem to be the right one to use, anyway
//Reference http://ww1.microchip.com/downloads/en/AppNotes/00752a.pdf
(*
This polynomial is also known as CRC CCITT-16.  
The interrogator applies the same polynomial to the incoming and transmitting data. 

http://practicingelectronics.com/articles/article-100003/article.php
The AX.25 CRC is described by the characteristics given in the following table.

Names associated with the AX.25 CRC protocol 	CRC-CCITT, CRC-16-X25, CRC-16-CCITT
Degree of polynomial 	                        16
Generator polynomial 	                        G(x) = x16+x12+x5+1
Polynomial in hex notation 	                    0x1021
Bit order 	                                    Reflected input byte 
                                                (LSB of byte sent first)
Initial value 	                                0xFF
Output XOR mask 	                            0xFF 
                                                (bitwise-invert shift register to 
                                                produce frame check sequence) 
*)


[<Tests>]
let CRCSharpTests =
    testList "CRC 16 Mcrf4Xx" [
    testCase "Subscribe to PewDiePie!" <| fun _ ->
        let crc_params = CrcStdParams.StandardParameters.[CrcAlgorithms.Crc16X25]
        let crc = new Crc(crc_params)
        let input_bytes = Encoding.ASCII.GetBytes("123456789") 
        let result = crc.ComputeCrc(0xFFFFUL, input_bytes, 0, input_bytes.Length)
        Console.WriteLine(sprintf "Computed CRC :: 0x%s" (result.ToString("X4")))
        Expect.equal result 50189UL "Result did not match expected MCRF4XX calculation"
    //testCase "Table" <| fun _ ->
    //    let crc_params = CrcStdParams.StandardParameters.[CrcAlgorithms.Crc16Mcrf4Xx]
    //    let crc = new Crc(crc_params)
    //    Console.WriteLine(sprintf "TABLE %s" "Hi")
    //    Console.WriteLine(sprintf "%A" crc._table)
    testCase "AX25FCS" <| fun _ -> 
        let input = 
            Encoding.ASCII.GetBytes("123456789") |> Seq.map uint16
        let result = CRC_16_with_table input
        Console.WriteLine(sprintf "AX25 FCS :: 0x%s" (result.ToString("X4")))
        Expect.equal (result.ToString("X4")) "906E" "The CRC was not right"
    ]