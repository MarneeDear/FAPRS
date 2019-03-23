namespace faprs.core

open System

module FrameActivePatterns = 

//TODO map into their respective types, especially the non-string values
    //Remove the channel from the frame
    let (|Frame|_|) (record:string) =
        match record with
        | r when String.IsNullOrWhiteSpace(r) -> None
        | r when r.IndexOf(" ") < 1 -> None //maybe return r because there was no channel and that's ok?
        | r when (r.Substring(r.IndexOf(" "))).Trim().Length > 0 -> Some ((r.Substring(r.IndexOf(" "))).Trim())
        | _ -> None
        //if String.IsNullOrWhiteSpace(record) then None
        //else if record.IndexOf(" ") < 1 then None
        //else
        //    let frame = (record.Substring(record.IndexOf(" "))).Trim()
        //    if frame.Length > 0 then Some frame else None

    let (|Address|_|) (frame:string) =
        if frame.IndexOf(":") < 1 then 
            None
        else
            Some (frame.Substring(0, frame.IndexOf(":")))

    let (|Sender|_|) (address:string) =
        if address.IndexOf(">") < 1 then None
        else Some (address.Substring(0, address.IndexOf(">")))

    let (|Destination|_|) (address:string) =
        if address.IndexOf(">") < 1 || address.IndexOf(",") < 1 then None
        else Some (address.Substring(address.IndexOf(">") + 1, address.IndexOf(",") - address.IndexOf(">") - 1))

    //Returns a list of the paths if parsed
    let (|Path|_|) (address:string) =
        if not (address.IndexOf(">") = -1) && address.IndexOf(",") > address.IndexOf(">") then
            Some (address.Substring(address.IndexOf(",") + 1).Split(','))
        else
            None

    let (|Message|_|) (frame:string) =
        if frame.IndexOf(":") < 1 then 
            None
        else
            Some (frame.Substring(frame.IndexOf(":") + 1))

    //Only supports Lat/Long Position Report Format — without Timestamp
    //See APRS 1.01 spec, section 8 POSITION AND DF REPORT DATA FORMATS
    //TODO make the data type identifies types
    //TODO According to APRS spec the Longitude is 8 chars fixed-length. Can just use the length to parse.
    //Eample: =3603.55N/112006.56W-
    let (|Latitude|_|) (msg:string) = 
        let parseLatitude (posRpt:string) =
            posRpt.Substring(1, 8) //TODO check that it is at least sort of an APRS latitude or longitude
        match getAPRSDataTypeIdentifier (msg.Substring(0,1)) with
        | Some id   ->  match id with
                        | PositionReportWithoutTimeStampWithMessaging   -> Some (parseLatitude msg)
                        | PositionReportWithoutTimeStampNoMessaging     -> Some (parseLatitude msg)
                        | _                                             -> None
         | None     -> None //We do not have a position report and therefore no latitude
        //if msg.StartsWith("=") || msg.StartsWith("!") then 
        //    //We have identigied a Lat/Long Position Report Format — without Timestamp
        //    match msg.IndexOf(@"N/"), msg.IndexOf(@"S/") with
        //    | -1, -1 -> None
        //    | n, s when n = -1 -> Some (msg.Substring(msg.IndexOf("=") + 1, s)) //Southern Hemisphere
        //    | n, s when s = -1 -> Some (msg.Substring(msg.IndexOf("=") + 1, n)) //Northern Hemisphere
        //    | _, _ -> None
        //else
        //    //We do not have a position report
        //    None

    //Only supports Lat/Long Position Report Format — without Timestamp
    //See APRS 1.01 spec, section 8 POSITION AND DF REPORT DATA FORMATS
    //TODO According to APRS spec the Longitude is 9 chars fixed-length. Can just use the length to parse.
    //Example: =3603.55N/112006.56W-
    let (|Longitude|_|) (msg:string) =
        let parseLongitude (posRpt:string) =
            posRpt.Substring(9, 9) //TODO check that it is at least sort of an APRS latitude or longitude
        match getAPRSDataTypeIdentifier (msg.Substring(9,1)) with
        | Some id   ->  match id with
                        | PositionReportWithoutTimeStampWithMessaging   -> Some (parseLongitude msg)
                        | PositionReportWithoutTimeStampNoMessaging     -> Some (parseLongitude msg)
                        | _                                             -> None
         | None     -> None //We do not have a position report and therefore no latitude
        //if msg.StartsWith("=") || msg.StartsWith("!") then 
        //    //We have a valid Lat/Long Position Report Format — without Timestamp
        //    let start =
        //        match msg.IndexOf(@"N/"), msg.IndexOf(@"S/") with
        //        | -1, -1 -> -1
        //        | n, s when n = -1 -> s
        //        | n, s when s = -1 -> n 
        //        | _, _ -> -1

        //    match msg.IndexOf('W'), msg.IndexOf('E'), start with
        //    | -1, -1, -1 -> None
        //    | w, e, st when e = -1 && st > -1 -> Some (msg.Substring(st + 2, w - st - 1)) //Western Hemisphere
        //    | w, e, st when w = -1 && st > -1 -> Some (msg.Substring(st + 2, e - st - 1)) //Eastern Hemisphere 
        //    | w, e, st when w < e && st > -1  -> Some (msg.Substring(st + 2, w - st - 1)) //Western Hemisphere 
        //    | w, e, st when e < w && st > -1  -> Some (msg.Substring(st + 2, e - st - 1)) //Eastern Hemisphere 
        //    | _, _, _ -> None
        //else
        //    //We do not have a position report
        //    None

    //TODO can do this by string position because the lat/lon is a fixed length
    let (|Symbol|_|) (msg:string) =
        let rpt =
            match msg.IndexOf(@"N/"), msg.IndexOf(@"S/") with
            | -1, -1 -> String.Empty
            | n, s when n = -1 -> (msg.Substring(s + 2)) //s
            | n, s when s = -1 -> (msg.Substring(n + 2)) //n 
            | _, _ -> String.Empty

        match rpt.IndexOf('W'), rpt.IndexOf('E'), rpt with
        | -1, -1, _ -> None
        | w, e, r when e = -1 -> Some (r.Substring(w + 1, 1).Chars(0)) //w
        | w, e, r when w = -1 -> Some (r.Substring(e + 1, 1).Chars(0)) //e
        | w, e, r when w < e ->  Some (r.Substring(w + 1, 1).Chars(0)) //w
        | w, e, r when e < w ->  Some (r.Substring(e + 1, 1).Chars(0)) //e
        | _, _, _ -> None

    let (|Comment|_|) (symbol:char) (msg:string) =
        let comment = msg.Substring(msg.IndexOf(symbol) + 1).Trim()
        if comment = 
            String.Empty 
        then    
            None 
        else 
            Some comment

    