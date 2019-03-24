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
            let lat = posRpt.Substring(1, 8)
            match lat.EndsWith("N"), lat.EndsWith("S") with
            | true, false   -> Some lat
            | false, true   -> Some lat
            | _             -> None
        match getAPRSDataTypeIdentifier (msg.Substring(0,1)) with
        | Some id   ->  match id with
                        | PositionReportWithoutTimeStampWithMessaging   -> (parseLatitude msg)
                        | PositionReportWithoutTimeStampNoMessaging     -> (parseLatitude msg)
                        | _                                             -> None
         | None     -> None //We do not have a position report and therefore no latitude

    //Only supports Lat/Long Position Report Format — without Timestamp
    //See APRS 1.01 spec, section 8 POSITION AND DF REPORT DATA FORMATS
    //TODO According to APRS spec the Longitude is 9 chars fixed-length. Can just use the length to parse.
    //Example: =3603.55N/112006.56W-
    let (|Longitude|_|) (msg:string) =
        let parseLongitude (posRpt:string) =
            let lon = posRpt.Substring(10, 9) 
            match lon.EndsWith("W"), lon.EndsWith("E") with 
            | true, false   -> Some lon
            | false, true   -> Some lon
            | _             -> None
            
        match msg.Substring(9,1) with
        | "/" -> parseLongitude msg
        | _ -> None

    //TODO can do this by string position because the lat/lon is a fixed length
    //Should be char 20
    //Example: =3603.55N/112006.56W-
    let (|Symbol|_|) (msg:string) =
        //TODO check that the previous char was a W or E meaning that it was probably and APRS lat/lon
        match msg.Substring(18,1) with
        | "W" -> getSymbolCode (msg.Substring(19,1).ToCharArray().[0])
        | "E" -> getSymbolCode (msg.Substring(19,1).ToCharArray().[0])
        | _ -> None

    let (|Comment|_|) (symbol:char) (msg:string) =
        let comment = msg.Substring(msg.IndexOf(symbol) + 1).Trim()
        if comment = 
            String.Empty 
        then    
            None 
        else 
            Some comment

    