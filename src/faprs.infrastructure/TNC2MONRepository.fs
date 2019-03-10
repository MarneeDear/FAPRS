namespace faprs.infrastructure

open faprs.core

//TODO Kiss settings
(*
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
*)

//TODO do this async maybe?
module TNC2MONRepository =
    open System.IO

    //Write a TNC2MON packet to a file that will be read and transmitted by Dire Wolf vis the kissutil
    //See Dire Wolf User Guide section 14.6.3 Transmit frames from files
    let writeKissUtilFrame (commands: KISS.Command list option) (packets: TNC2MON.Packet list) (saveTo:string) timestamp =
        let file = Path.Combine(Path.GetFullPath(saveTo), sprintf "%s%s" timestamp "faprs.txt")
        
        let kiss =
            commands
            |> Option.defaultValue []
            |> List.map (fun c -> string (c.ToChar()))        
        
        let frames = 
            packets 
            |> List.map (fun p -> p.ToString())
        
        File.WriteAllLines (file, kiss @ frames) |> ignore //put the commands first and then the frames

    //turn the kissutil frame into a list of frame elements
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //[0] KG7SIO-7>APRD15,WIDE1-1:=3216.4N/11057.3Wb
    let parseFrame (frame:string) =
        frame.Split(' ')

    //TODO map the frame elements into a valid TNC2MON packet
    let mapToPacket frameElements =
        ()

    //All received frames are displayed in the usual monitor format, preceded with the channel number inside of [ ].
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //See Dire Wolf User Guide 14.6 kissutil – KISS TNC troubleshooting and Application Interface
    //let readKissUtilFrames file =
    //    let frames = File.ReadAllLines file 
    //    frames |> List.map (fun f -> parseFrame)