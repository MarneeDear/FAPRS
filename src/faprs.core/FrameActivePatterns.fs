namespace faprs.core

open System

module FrameActivePatterns = 

//TODO map into their respective types, especially the non-string values
    //Remove the channel from the frame
    let (|Frame|_|) (record:string) =
        if String.IsNullOrWhiteSpace(record) then None
        else if record.IndexOf(" ") < 1 then None
        else
            let frame = (record.Substring(record.IndexOf(" "))).Trim()
            if frame.Length > 0 then Some frame else None

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
    let (|Latitude|_|) (msg:string) = 
        if msg.StartsWith("=") || msg.StartsWith("!") then 
            //We have a valid Lat/Long Position Report Format — without Timestamp
            match msg.IndexOf(@"N/"), msg.IndexOf(@"S/") with
            | -1, -1 -> None
            | n, s when n = -1 -> Some (msg.Substring(msg.IndexOf("=") + 1, s)) //Southern Hemisphere
            | n, s when s = -1 -> Some (msg.Substring(msg.IndexOf("=") + 1, n)) //Northern Hemisphere
            | _, _ -> None
        else
            //We do not have a valid position report
            None

    //Only supports Lat/Long Position Report Format — without Timestamp
    //See APRS 1.01 spec, section 8 POSITION AND DF REPORT DATA FORMATS
    let (|Longitude|_|) (msg:string) =
        if msg.StartsWith("=") || msg.StartsWith("!") then 
            //We have a valid Lat/Long Position Report Format — without Timestamp
            let start =
                match msg.IndexOf(@"N/"), msg.IndexOf(@"S/") with
                | -1, -1 -> -1
                | n, s when n = -1 -> s
                | n, s when s = -1 -> n 
                | _, _ -> -1

            match msg.IndexOf('W'), msg.IndexOf('E'), start with
            | -1, -1, -1 -> None
            | w, e, s when e = -1 && s > -1 -> Some (msg.Substring(start + 2, w - start - 1)) //Western Hemisphere
            | w, e, s when w = -1 && s > -1 -> Some (msg.Substring(start + 2, e - start - 1)) //Eastern Hemisphere 
            | w, e, s when w < e && s > -1  -> Some (msg.Substring(start + 2, w - start - 1)) //Western Hemisphere 
            | w, e, s when e < w && s > -1  -> Some (msg.Substring(start + 2, e - start - 1)) //Eastern Hemisphere 
            | _, _, _ -> None
        else
            //We do not have a position report
            None

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

    let (|Comment|_|) (msg:string) (symbol:char) =
        let comment = msg.Substring(msg.IndexOf(symbol) + 1).Trim()
        if comment = String.Empty then None else Some comment