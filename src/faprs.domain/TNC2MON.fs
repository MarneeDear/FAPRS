namespace faprs.domain

open System

module TNC2MON = 

    type Packet = 
        {
            Sender : CallSign //9 bytes
            Destination : CallSign //9bytes
            Path : Path //81 bytes
            Message : Message option
        }

    // let getPositionReport sender destination lat lon path =
    //     let positionReport =
    //         {
    //             Position = { Lattitude = lat; Longitude = lon }
    //         }
    //     {
    //         Sender = sender
    //         Destination = destination
    //         Path = path
    //         Message = Some (Message.PositionReport positionReport)
    //     }

    let buildTncPaketAscii packet =
        sprintf "%s>%s,%s:%s" packet.Sender packet.Destination (packet.Path.ToString()) (Option.defaultWith String.Empty packet.Message)
        