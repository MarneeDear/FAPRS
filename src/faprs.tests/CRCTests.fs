module CRCTests

open Expecto
open faprs.crc
open System
open System.Text

//MCRF$XX seems to be the only one that works consistently with https://crccalc.com/
//It does seem to be the right one to use, anyway
//Reference http://ww1.microchip.com/downloads/en/AppNotes/00752a.pdf
(*
This polynomial is also known as CRC CCITT-16.  
The interrogator applies the same polynomial to the incoming and transmitting data. 
*)


[<Tests>]
let CRCSharpTests =
    testList "CRC 16 Mcrf4Xx" [
    testCase "Subscribe to PewDiePie!" <| fun _ ->
        let crc_params = CrcStdParams.StandardParameters.[CrcAlgorithms.Crc16Mcrf4Xx]
        let crc = new Crc(crc_params)
        let input_bytes = Encoding.ASCII.GetBytes("Subscribe to PewDiePie!") 
        let result = crc.ComputeCrc(0xFFFFUL, input_bytes, 0, input_bytes.Length)
        //Console.WriteLine(sprintf "Computed CRC :: %s" (result.ToString("x2")))
        Expect.equal result 50189UL "Result did not match expected MCRF4XX calculation"
    //testCase "Table" <| fun _ ->
    //    let crc_params = CrcStdParams.StandardParameters.[CrcAlgorithms.Crc16Mcrf4Xx]
    //    let crc = new Crc(crc_params)
    //    Console.WriteLine(sprintf "TABLE %s" "Hi")
    //    Console.WriteLine(sprintf "%A" crc._table)
    ]