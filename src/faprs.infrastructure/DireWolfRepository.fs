namespace faprs.infrastructure.DireWolf

open faprs.core
open System.IO

module KissUtil =
    open faprs.infrastructure.TNC2MONRepository

    //Write a TNC2MON packet to a file that will be read and transmitted by Dire Wolf vis the kissutil
    //See Dire Wolf User Guide section 14.6.3 Transmit frames from files
    let writeKissUtilRecord (commands: KISS.Command list option) (packets: TNC2MON.Packet list) (saveTo:string) timestamp =
        let file = Path.Combine(Path.GetFullPath(saveTo), sprintf "%s%s" timestamp "faprs.txt")
        
        let kiss =
            commands
            |> Option.defaultValue []
            |> List.map (fun c -> string (c.ToString()))        
        
        let frames = 
            packets 
            |> List.map (fun p -> p.ToString())
        
        File.WriteAllLines (file, kiss @ frames) |> ignore //put the commands first and then the frames

    //All received frames are displayed in the usual monitor format, preceded with the channel number inside of [ ].
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //See Dire Wolf User Guide 14.6 kissutil – KISS TNC troubleshooting and Application Interface
    let processKissUtilFrames path (file: string option) =
        let d = new DirectoryInfo(path);//Assuming Test is your Folder
        let files = d.GetFiles()  //GetFiles("*.txt"); //Getting Text files
        let getFrames fileName = 
            File.ReadAllLines (Path.Combine(path, fileName))
            |> Array.map (fun f -> parseIntoAPRSMessage f)
        match file with
        | Some f    -> getFrames f
        | None      -> files 
                        |> Array.map (fun f -> f.Name)
                        |> Array.map getFrames
                        |> Array.head
        