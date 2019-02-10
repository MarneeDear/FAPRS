namespace faprs.domain

module AX25 =

    type AX25Packet =
        {
            Flag : byte
            Address : byte
            Control : byte
            ProtocolIdentifier : byte
            Information: byte
            FrameCheckSequence: byte
        }