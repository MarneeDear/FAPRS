namespace faprs.infrastructure

open faprs.core
open faprs.core.TNC2MonActivePatterns
open faprs.core.APRSData
open faprs.core.Participant

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
    open System
        
    let mapPositionReport rpt =
        let pos =
            let posRec lat lon = 
                {
                    Latitude = FormattedLatitude.check lat //TODO how to create this when the latiude should have already been formatted? How to check?
                    Longitude = FormattedLongitude.check lon 
                }
            match (|Latitude|_|) rpt, (|Longitude|_|) rpt with
            | Some lat, Some lon    -> posRec lat lon
            | None, Some _          -> failwith "Latitude was not in expected format."
            | Some _, None          -> failwith "Longitude was not in exptected format."
            | None, None            -> failwith "Neither Latitude nor Longitude were in expected format."
        let sym =
            match (|Symbol|_|) rpt with
            | Some s    -> s //Defaults to house if no match found -- TODO do I want to do this?
            | None      -> Common.SymbolCode.House
        let comment =
            let c =
                match (|Comment|_|) (sym.ToChar()) rpt with
                | Some c    -> PositionReportComment.create c
                | None      -> PositionReportComment.create String.Empty
            match c with
            | Some c    -> c
            | None      -> failwith "Position Report Comment must be less than 43 characters long."
        {
            Position = pos
            Symbol = sym
            Comment = comment
        }
        |> PositionReportWithoutTimeStamp

    let mapUnformattedMessage (msg:string) =
        Unformatted (UnformattedMessage.create msg)

    let mapParticipantReport (rpt:string) =
        if rpt.Length = 255 then
            let timestamp = RecordedOn.create (Some (RecordedOn.revert (rpt.Substring(0, 8))))
            let id = ParticipantID.create (rpt.Substring(8, 5))
            let st1 = int (rpt.Substring(13, 1))
            let st2 = int (rpt.Substring(14, 1))
            let msg = rpt.Substring(15)
            let c = match rpt.Substring(rpt.Length - 2, 1) with
                    | "C"   -> true
                    | _     -> false
            let psts =
                 ParticipantStatus.fromStatusCombo (st1, st2, msg)
            {
                TimeStamp = timestamp
                ParticipantID = id.Value
                ParticipantStatus = psts
                Cancelled = c                
            }
            |> ParticipantStatusReport |> Some
        else 
            None

    //Examples
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //[0] KG7SIO-7>APRD15,WIDE1-1:=3216.4N/11057.3Wb
    //TODO use ROP and a pipeline -- how best to do that?
    let convertRecordToAPRSData (record:string) =
        let frame rcrd =
            match (|Frame|_|) rcrd with
            | Some f    -> f |> Ok
            | None      -> "Frame not in expected format." |> Error
        
        let msg frame =
            match (|Message|_|) frame with
            | Some m    -> m |> Ok
            | None      -> "No message part found." |> Error

        let participant (msg:string) =
            match msg.Substring(0, 1) with
            | id when id.Equals("=") -> Ok (mapPositionReport (msg.Substring(1))) //We have a lat/lon position report without timestamot. Let's try to parse it.
            | id when id.Equals(":") -> Ok (mapUnformattedMessage (msg.Substring(1))) //we have an unformatted messsage. Let's try to parse it
            | id when id.Equals("{") -> //Ok (mapParticipantReport (msg.Substring(1))) //We have a participant report. Let's try to parse it
                                        let pRpt = (mapParticipantReport (msg.Substring(1)))
                                        match pRpt with
                                        | Some r -> Ok r
                                        | None -> Error "Participant report not in expected format"
            | _                      -> Error "Message does not start with a recognized APRS data identifier" //TODO Maybe we just want to log this
        
        frame record
        |> Result.bind msg
        |> Result.bind participant
            //All received frames are displayed in the usual monitor format, preceded with the channel number inside of [ ].
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //See Dire Wolf User Guide 14.6 kissutil – KISS TNC troubleshooting and Application Interface
    //TODO this needs to process each file not a specific file name -- we wont know file name at runtime
    let processKissUtilFrames path (file: string option) =
        let d = new DirectoryInfo(path);//Assuming Test is your Folder
        let files = d.GetFiles()  //GetFiles("*.txt"); //Getting Text files
        let getFrames fileName = 
            File.ReadAllLines (Path.Combine(path, fileName))
            |> Array.map (fun f -> convertRecordToAPRSData f)
        match file with
        | Some f    -> getFrames f
        | None      -> files 
                        |> Array.map (fun f -> f.Name)
                        |> Array.map getFrames
                        |> Array.head
        