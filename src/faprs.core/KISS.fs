namespace faprs.core

(*
14.6 kissutil – KISS TNC troubleshooting and Application Interface
14.6.1 Interactive mode
Input, starting with a lower case letter is interpreted as being a command. 
Whitespace, as shown in the examples, is optional.
letter meaning example
------- ----------- -----------
d txDelay, 10ms units d 30
p Persistence p 63
s Slot time, 10ms units s 10
t txTail, 10ms units t 5
f Full duplex f 0
h set Hardware h (hardware-specific)

Command codes

Any of these codes may be sent from the host to the TNC, but only the "Data frame" code should be sent from the TNC to the host.
Hex value 	Name 	        Bytes 	Description
0x00 	    Data frame 	    Varies 	This frame contains data that should be sent out of the TNC. The maximum number of bytes is determined by the amount of memory in the TNC.
0x01 	    TX DELAY 	    1 	    The amount of time to wait between keying the transmitter and beginning to send data (in 10 ms units).
0x02 	    P 	            1    	The persistence parameter. Persistence=Data*256-1. Used for CSMA.
0x03 	    SlotTime 	    1 	    Slot time in 10 ms units. Used for CSMA.
0x04 	    TXtail 	        1 	    The length of time to keep the transmitter keyed after sending the data (in 10 ms units).
0x05 	    FullDuplex 	    1 	    0 means half duplex, anything else means full duplex.
0x06 	    SetHardware 	Varies 	Device dependent.
0xFF 	    Return 	        1 	    Exit KISS mode. This applies to all ports and requires a port code of 0xF. 

*)
module KISS =
    open System

    type Command =
        | Data
        | TxDelay of int
        | Persistence
        | SlotTime 
        | TxTail
        | FullDuplex
        | SetHardware 
        | Return
        override this.ToString () =
            match this with
            | TxDelay d     -> sprintf "d %i" d
            | Persistence   -> "p" //TODO
            | SlotTime      -> "s"
            | TxTail        -> "t"
            | FullDuplex    -> "f"
            | SetHardware   -> "h"
            | _             -> String.Empty
        //TODO
        //member this.ToHex() =
        //    match this with
        //    | Data          -> 0x00
        //    | TxDelay       -> 0x01
        //    | Persistence   -> 0x02
        //    | SlotTime      -> 0x03
        //    | TxTail        -> 0x04
        //    | FullDuplex    -> 0x05
        //    | SetHardware   -> 0x06
        //    | Return        -> 0xFF
        //member this.ToBytes() =
        //    match this with
        //    | Data          -> BitConverter.GetBytes(0x00)
        //    | TxDelay       -> BitConverter.GetBytes(0x01)
        //    | Persistence   -> BitConverter.GetBytes(0x02)
        //    | SlotTime      -> BitConverter.GetBytes(0x03)
        //    | TxTail        -> BitConverter.GetBytes(0x04)
        //    | FullDuplex    -> BitConverter.GetBytes(0x05)
        //    | SetHardware   -> BitConverter.GetBytes(0x06)
        //    | Return        -> BitConverter.GetBytes(0xFF)
    //TODO
    //let getCommand c =
    //    match c with
    //    | 'd'   -> TxDelay
    //    | 'p'   -> Persistence
    //    | 's'   -> SlotTime
    //    | 't'   -> TxTail
    //    | 'f'   -> FullDuplex
    //    | 'h'   -> SetHardware
    //    | _     -> failwith "Unknown KISS command character."

    (*
    
    As defined here https://en.wikipedia.org/wiki/KISS_(TNC)
    and here http://www.ka9q.net/papers/kiss.html

    Special characters
    Hex value 	Abbreviation 	Description
    0xC0 	    FEND 	        Frame End
    0xDB 	    FESC 	        Frame Escape
    0xDC 	    TFEND 	        Transposed Frame End
    0xDD 	    TFESC 	        Transposed Frame Escape
    
    Frame structure
    Begin 	Command 	                Data0..DataN 	            End
    FEND 	High nibble – Port Index
            Low nibble – Command 	    Data (AX.25 wrapped?)       FEND

    If a FEND ever appears in the data, it is translated into the two byte sequence 
    
    FESC TFEND (Frame Escape, Transposed Frame End). 
    
    Likewise, if the FESC character ever appears in the user data, 
    it is replaced with the two character sequence 
    
    FESC TFESC (Frame Escape, Transposed Frame Escape).

    *)
    type SpecialCharacters =
        | FEND
        | FESC
        | TFEND
        | TFESC
        member this.ToHexCode() =
            match this with
            | FEND  -> 0xC0us
            | FESC  -> 0xDBus
            | TFEND -> 0xDCus
            | TFESC -> 0xDDus
        member this.ToBytes() =
            match this with
            | FEND  -> BitConverter.GetBytes(0xC0us)
            | FESC  -> BitConverter.GetBytes(0xDBus)
            | TFEND -> BitConverter.GetBytes(0xDCus)
            | TFESC -> BitConverter.GetBytes(0xDDus)

    type Port =
        | P00
        | P01
        | P02
        | P03
        member this.ToBytes() =
            match this with
            | P00 -> BitConverter.GetBytes(0us)
            | P01 -> BitConverter.GetBytes(1us)
            | P02 -> BitConverter.GetBytes(2us)
            | P03 -> BitConverter.GetBytes(3us)

    //TODO
    //type Packet =
    //    {
    //        Port : Port
    //        Command : Command
    //        //Data : string //TODO AX25 data frame
    //        Data : AX25.Packet
    //    }
    //    member this.ToTx() = //TODO give this a better name
    //        //TODO replace payload FEND codes with FESC, TFEND and FESC,TFESC 
    //        //TODO I think this needs to be a byte array?
    //        // FEND | PORT | COMMAND | DATA (AX.25) | FEND
    //        Array.empty 
    //        |> Array.append (FEND.ToBytes())
    //        |> Array.append (this.Port.ToBytes())
    //        |> Array.append (this.Command.ToBytes())
    //        //|> Array.append  //(System.Text.ASCIIEncoding.ASCII.GetBytes(this.Data)) TODO get from AX.25
    //        |> Array.append (FEND.ToBytes())
