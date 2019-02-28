namespace faprs.core

//TODO build this out to make it possible to send directly to a TNC instead of using Direwolf kissutil

(*

https://en.wikipedia.org/wiki/AX.25
https://tapr.org/pub_ax25.html

AX.25.2.2.pdf
3. Frame Structure
Link layer packet radio transmissions are sent in small blocks of data, called frames.

Flag        Address         Control     PID     Info        FCS     Flag
01111110    112/224 Bits    8/16 Bits   8 Bits  N*8 Bits 1  6 Bits  01111110
0x7E                                                                0x7E

APR1.01 
3 APRS AND AX.25

The AX.25 Frame All APRS transmissions use AX.25 UI-frames, with 9 fields of data:
AX.25 UI-FRAME FORMAT

*)
module AX25 =
    open System

    type Packet =
        {
            Flag : int8
            Address : byte[]
            Control : byte[]
            ProtocolIdentifier : byte[]
            Information: byte[]
            FrameCheckSequence: byte[]
        }
        member this.ToByteArray() =
            Array.empty
            |> Array.append (BitConverter.GetBytes(0x7E))
            |> Array.append this.Address
            |> Array.append this.Control

