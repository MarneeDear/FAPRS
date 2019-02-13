namespace faprs.domain

//TODO build this out to make it possible to send directly to a TNC instead of using Direwolf kissutil
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
