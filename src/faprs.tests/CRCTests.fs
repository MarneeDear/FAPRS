module CRCTests

open Expecto
open faprs.core.FrameCheckSequence
open System
open System.Text
open System.Collections
open System.ComponentModel

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
    testList "CRC 16 AX25" [
    //TTODO: his doesnt work for AX25 algorithm. I am not going to use this library, but I will try
    //to port it to F# and make sure it work for AX25 at least
    //testCase "123456789" <| fun _ ->
    //    let crc_params = CrcStdParams.StandardParameters.[CrcAlgorithms.Crc16X25]
    //    let crc = new Crc(crc_params)
    //    let input_bytes = Encoding.ASCII.GetBytes("123456789") 
    //    let result = crc.ComputeCrc(0xFFFFUL, input_bytes, 0, input_bytes.Length)
    //    Console.WriteLine(sprintf "Computed CRC :: 0x%s" (result.ToString("X4")))
    //    Expect.equal (result.ToString("X4")) "906E" "The CRC was not right"
    //testCase "Table" <| fun _ ->
    //    let crc_params = CrcStdParams.StandardParameters.[CrcAlgorithms.Crc16Mcrf4Xx]
    //    let crc = new Crc(crc_params)
    //    Console.WriteLine(sprintf "TABLE %s" "Hi")
    //    Console.WriteLine(sprintf "%A" crc._table)
    testCase "AX25FCS 123456789" <| fun _ -> 
        let input = 
            Encoding.ASCII.GetBytes("123456789") |> Seq.map uint16
        let result = CRC_16_with_table input
        Console.WriteLine(sprintf "AX25 FCS :: 0x%s" (result.ToString("X4")))
        Expect.equal (result.ToString("X4")) "906E" "The CRC was not right"
        (*
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        *)
    //testCase "AX25FCS ABC" <| fun _ -> 
    //    let input = 
    //        Encoding.ASCII.GetBytes("ABC") |> Seq.map uint16
    //    let result = CRC_16_with_table input
    //    Console.WriteLine(sprintf "AX25 FCS :: 0x%s" (result.ToString("X4")))
    //    let bitarray = new BitArray([|1;1;1;1;0;1;0;0;1;1;1;1;1;0;0;1|])
    //    let mutable ret = byte |> Array.init ((bitarray.Length - 1) / 8 + 1)
    //    bitarray.CopyTo(ret, 0)
    //    Console.WriteLine(sprintf "bitarray :: %s" )
    //    Expect.equal (result.ToString("X4")) (BitConverter.ToString(ret).Replace("-", ""))  "The CRC was not right"
        //()
    ]