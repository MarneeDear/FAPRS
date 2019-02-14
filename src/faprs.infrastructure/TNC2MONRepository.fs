namespace faprs.infrastructure

open faprs.core

//TODO do this async maybe?
module TNC2MONRepository =
    open System.IO

    //Write a TNC2MON packet to a file that will be read and transmitted by Dire Wolf vis the kissutil
    //See Dire Wolf User Guide section 14.6.3 Transmit frames from files
    let writePacketToKissUtilFile (packets: TNC2MON.Packet list) (saveTo:string) timestamp =
        let file = Path.Combine(Path.GetFullPath(saveTo), sprintf "%s%s" timestamp "faprs.txt")
        let frames = packets |> List.map (fun p -> p.ToString())
        File.WriteAllLines (file, frames) |> ignore //TODO this will overwrite the file. do we want to do that or create a new file?