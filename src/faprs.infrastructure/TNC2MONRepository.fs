namespace faprs.infrastructure

open faprs.core
open faprs.core.TNC2MonActivePatterns
open faprs.core.DataFormatActivePatterns
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
        let posResult =
            let posRec lat lon = 
                {
                    Latitude = FormattedLatitude.check lat //TODO how to create this when the latiude should have already been formatted? How to check?
                    Longitude = FormattedLongitude.check lon 
                }
            match (|Latitude|_|) rpt, (|Longitude|_|) rpt with
            | Some lat, Some lon    -> Ok (posRec lat lon)
            | None, Some _          -> Error "Latitude was not in expected format."
            | Some _, None          -> Error "Longitude was not in exptected format."
            | None, None            -> Error "Neither Latitude nor Longitude were in expected format."
        let sym =
            match (|Symbol|_|) rpt with
            | Some s    -> s //Defaults to house if no match found -- TODO do I want to do this?
            | None      -> Common.SymbolCode.House
        let comment = //TODO handle the case where the comment is not accepted length
            //let c =
            match (|Comment|_|) (sym.ToChar()) rpt with
            | Some c    -> PositionReportComment.create c
            | None      -> None //PositionReportComment.create String.Empty

        match posResult with
        | Ok p ->   {
                        Position = p
                        Symbol = sym
                        Comment = comment
                    } |> PositionReportWithoutTimeStamp |> Ok
        | Error msg -> Error msg

    let mapMessage (msg:string) = //Can I do this recursivley
        //let partMsg (part, partName) =
        //    match part with
        //    | Some p -> String.Empty
        //    | None -> sprintf "%s%s" partName "part of message not in expected format."
            //match (a, m, n) with
            //| None, Some _, Some _ -> "Addressee"
            //| Some _, None, Some _ -> "Message"
            //| Some _, Some _, None -> "Message Number"

        match (|Addressee|_|) msg, (|Message|_|) msg, (|MessageNumber|_|) msg with
        | Some a, Some m, Some n -> 
                                match CallSign.create a with
                                | Some c -> 
                                            {
                                                Addressee = c
                                                MessageText = MessageText.create m
                                                MessageNumber = MessageNumber.create n
                                            } |> Information.Message |> Ok
                                | None -> Error "Addressee call sign not in expected format."
        //| _, _, _ -> [(a, "Addressee"; (m, "Message"); (n, "Message Number")] > List.fold (fun acc elem -> partMsg elem acc)
        | None, Some _, Some _ -> Error "Addressee part of message not in expected format."
        | Some _, None, Some _ -> Error "Message part of message not in expected format."
        | Some _, Some _, None -> Error "Message Number part of message not in expected format."
        | _, _, _              -> Error "Message not in expected format."

    //let mapUnsupportedMessage (msg:string) =
    //    Unsupported (UnformattedMessage.create msg)

    //18 USER-DEFINED DATA FORMAT --experimental designator
    //APRS 1.01 For experimentation, or prior to being issued a User ID, anyone may utilize
    //the User ID character of { without prior notification or approval (i.e. packets
    //beginning with {{ are experimental, and may be sent by anyone).
    let mapParticipantReport (rpt:string) =
        match rpt.Substring(0, 2) with
        | id when id.Equals("{P")    ->
                                        if rpt.Length >= 15 && rpt.Length <= 253 then
                                            let timestamp = RecordedOn.create (Some (RecordedOn.revert (rpt.Substring(2, 8))))
                                            let id = ParticipantID.create (rpt.Substring(10, 5))
                                            let st1 = int (rpt.Substring(15, 1))
                                            let st2 = int (rpt.Substring(16, 1))
                                            let msg = rpt.Substring(17)

                                            //let cancelled = match rpt.Substring(rpt.Length - 2, 1) with
                                            //                | "C"   -> true 
                                            //                | _     -> false

                                            let psts =
                                                 ParticipantStatus.fromStatusCombo (st1, st2, msg)
                                            {
                                                TimeStamp = timestamp
                                                ParticipantID = id.Value
                                                ParticipantStatus = psts
                                                //Cancelled = false                
                                            }
                                            |> ParticipantStatusReport |> Ok
                                        else 
                                            Error "Participant report not in expected format. Message length exceeded 253 characters."
        | _                         -> Error "Participant report not in expected format. Message did not start with expected identifier -- {P."

    //Examples
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //[0] KG7SIO-7>APRD15,WIDE1-1:=3216.4N/11057.3Wb
    //TODO use ROP and a pipeline -- how best to do that?
    (*
        Ident Data Type 
        0x1c Current Mic-E Data (Rev 0 beta) 
        < Station Capabilities
        0x1d Old Mic-E Data (Rev 0 beta) 
        = Position without timestamp (with APRS messaging)
        ! Position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station
        > Status
        double-quote [Unused] 
        ? Query
        # Peet Bros U-II Weather Station 
        @ Position with timestamp (with APRS messaging)
        $ Raw GPS data or Ultimeter 2000 A–S [Do not use]
        % Agrelo DFJr / MicroFinder T Telemetry data
        & [Reserved — Map Feature] U–Z [Do not use]
        ' Old Mic-E Data (but Current data for TM-D700) 
        [ Maidenhead grid locator beacon (obsolete)
        ( [Unused]  //
        \ [Unused]
        ) Item 
        ] [Unused]
        * Peet Bros U-II Weather Station 
        ^ [Unused]
        + [Reserved — Shelter data with time] 
        _ Weather Report (without position)
        , Invalid data or test data 
        ‘ Current Mic-E Data (not used in TM-D700)
        - [Unused] a–z [Do not use]
        . [Reserved — Space weather] 
        { User-Defined APRS packet format
        / Position with timestamp (no APRS messaging) 
        | [Do not use — TNC stream switch character]
        0–9 [Do not use] 
        } Third-party traffic
        : Message ~ [Do not use — TNC stream switch character]
        ; Object
    *)
    let convertRecordToAPRSData (record:string) =
        let frame rcrd =
            match (|Frame|_|) rcrd with
            | Some f    -> f |> Ok
            | None      -> "Frame not in expected format." |> Error
        
        let msg frame =
            match (|Information|_|) frame with
            | Some m    -> m |> Ok
            | None      -> "No message part found." |> Error

        let data (info:string) =
            match info.Substring(0, 1) with
            | id when id.Equals("=") -> mapPositionReport (info.Substring 1) //We have a lat/lon position report without timestamot. Let's try to parse it.
            | id when id.Equals(":") -> mapMessage (info.Substring 1) //|> Ok //we have Message data type. Lets try to parse it
            | id when id.Equals("{") -> //Ok (mapParticipantReport (msg.Substring(1))) //We have user-defined data. Maybe it's a participant report. Let's try to parse it
                                        mapParticipantReport (info.Substring 1)
                                        //let pRpt = (mapParticipantReport (msg.Substring 1))
                                        //match pRpt with
                                        //| Some r -> Ok r
                                        //| None -> Error "Participant report not in expected format"
            //| _                      -> mapUnsupportedMessage(info.Substring 1) |> Ok //if not in supported format just turn it into a message so it can be logged

        frame record
        |> Result.bind msg
        |> Result.bind data
    
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
        