namespace faprs.domain

//TODO build this out to make it possible to send directly to a TNC instead of using Direwolf kissutil

(*

https://en.wikipedia.org/wiki/AX.25
https://tapr.org/pub_ax25.html

APR1.01 
3 APRS AND AX.25

The AX.25 Frame All APRS transmissions use AX.25 UI-frames, with 9 fields of data:
AX.25 UI-FRAME FORMAT

*)
module AX25 =

    type Packet =
        {
            Flag : byte[]
            Address : byte[]
            Control : byte[]
            ProtocolIdentifier : byte[]
            Information: byte[]
            FrameCheckSequence: byte[]
        }
