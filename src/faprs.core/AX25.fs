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

The AX.25 Frame All APRS transmissions use AX.25 UI-frames with 9 fields of data:
AX.25 UI-FRAME FORMAT


*)
module AX25 =
    open System
    open faprs.crc.FrameCheckSequence

    //Address Field encoding https://tapr.org/pub_ax25.html#2.2.13
    type Address = 
        {
            Destination : string
            Source : string
        }

    let FLAG = 0x7Eus //(BitConverter.GetBytes(0x7Eus))
    //WORK IN PROGRESS -- MMD 3/10/2019
    //TODO Should the fields be in strings and use Encoding.ASCII.GetBytes to convert to byte arrays?
    //https://stackoverflow.com/questions/4198908/c-sharp-convert-string-to-uint
    //type Packet =
    //    {
    //        //Flag : int8
    //        Address : string 
    //        Control : string //Control Field encoding
    //        ProtocolIdentifier : uint16 //Protocol Identifier https://tapr.org/pub_ax25.html#2.2.4
    //        Information: string
    //        //FrameCheckSequence: byte[]
    //    }
    //    member this.ToSeq() =
    //        //TODO teh byte arrays need to be ordered in LSB and MSB according to the spec
    //        //AX25.2.2.pdf All fields except the Frame Check Sequence (FCS) are transmitted low-order bit first. 
    //        //FCS is transmitted bit 15 first.
    //        //TODO bit stuffing
    //        let ax25_data = 
    //            [
    //                FLAG
    //                this.Address
    //                this.Control
    //                this.ProtocolIdentifier
    //                this.Information
    //            ]
    //        FLAG :: ((ax25_data |> List.toSeq |> CRC_16_with_table) :: ax25_data) 
    //        |> List.toSeq

    type Packet =
        {
            //Flag : int8
            Address : uint16 //byte[]
            Control : uint16 //byte[]
            ProtocolIdentifier : uint16 //byte[]
            Information: uint16 //byte[]
            //FrameCheckSequence: byte[]
        }
        member this.ToSeq() =
            //TODO teh byte arrays need to be ordered in LSB and MSB according to the spec
            //AX25.2.2.pdf All fields except the Frame Check Sequence (FCS) are transmitted low-order bit first. 
            //FCS is transmitted bit 15 first.
            let ax25_data = 
                [
                    FLAG
                    this.Address
                    this.Control
                    this.ProtocolIdentifier
                    this.Information
                ]
            FLAG :: ((ax25_data |> List.toSeq |> CRC_16_with_table) :: ax25_data) 
            |> List.toSeq

