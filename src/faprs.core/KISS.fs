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

    type Command =
        | Data
        | TxDelay
        | Persistence
        | SlotTime
        | TxTail
        | FullDuplex
        | SetHardware 
        | Return
        member this.ToChar() =
            match this with
            | TxDelay       -> 'd'
            | Persistence   -> 'p'
            | SlotTime      -> 's'
            | TxTail        -> 't'
            | FullDuplex    -> 'f'
            | SetHardware   -> 'h'
            | _             -> ' '
        member this.ToHex() =
            match this with
            | Data          -> 0x00
            | TxDelay       -> 0x01
            | Persistence   -> 0x02
            | SlotTime      -> 0x03
            | TxTail        -> 0x04
            | FullDuplex    -> 0x05
            | SetHardware   -> 0x06
            | Return        -> 0xFF
     
    let getCommand c =
        match c with
        | 'd'   -> TxDelay
        | 'p'   -> Persistence
        | 's'   -> SlotTime
        | 't'   -> TxTail
        | 'f'   -> FullDuplex
        | 'h'   -> SetHardware
        | _     -> failwith "Unknown KISS command character."

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

    *)
    type SpecialCharacters =
        | FEND
        | FESC
        | TFEND
        | TFESC
        member this.ToHexCode() =
            match this with
            | FEND  -> 0xC0
            | FESC  -> 0xDB
            | TFEND -> 0xDC
            | TFESC -> 0xDD

    type Packet =
        {
            Port : int8
            Command : Command
            Data : string //TODO AX25 data frame
        }
        member this.ToTx() = //TODO give this a better name
            // FEND | PORT | COMMAND | DATA (AX.25) | FEND
            sprintf "%i%i%iDATA_TODO%i" (FEND.ToHexCode()) this.Port (Command.Data.ToHex()) (FEND.ToHexCode())
            //() //TODO build the farme or packet or whatever it is called