namespace faprs.infrastructure

open faprs.core

//TODO Kiss settings
(*
Input, starting with a lower case letter is interpreted as being a command. Whitespace, as shown in the examples, is optional.
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
    let writePacketToKissUtilFile (packets: TNC2MON.Packet list) (saveTo:string) timestamp =
        let file = Path.Combine(Path.GetFullPath(saveTo), sprintf "%s%s" timestamp "faprs.txt")
        let frames = packets |> List.map (fun p -> p.ToString())
        File.WriteAllLines (file, frames) |> ignore //TODO this will overwrite the file. do we want to do that or create a new file?