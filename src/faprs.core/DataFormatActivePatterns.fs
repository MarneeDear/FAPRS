namespace faprs.core

open System

module DataFormatActivePatterns =

    //Part of the message data type
    //See Appendix 1 page 100 APRS 1.01
    (*            
    Message Format
            | : | Addressee | : | Message Text  | Message ID | Message Number (xxxxx)
    bytes   | 1 |    9      | 1 |    0-67       |     {      |      1-5
    KG7SIO   :HELLO WORLD{12345
    0123456789012345678901234567890
    *)
    let (|Addressee|_|) (msg:string) =
        match msg.IndexOf(":") with
        | i when i = 9  -> Some (msg.Substring(0,9).Trim())
        | _             -> None

    let (|Message|_|) (msg:string) = //msg.Substring(10, j - 10)
        match msg.IndexOf(":"), msg.IndexOf("{") with
        | i, j when i = 9 && j > 9 && j < i + 67    -> Some (msg.Substring(i + 1, j - i - 1))
        | _                                         -> None

    let (|MessageNumber|_|) (msg:string) =
        match msg.IndexOf(":"), msg.IndexOf("{") with
        | i, j when i = 9 && j > 9 && j < i + 67 -> Some (sprintf "%5s" (msg.Substring(j+1).Trim()))
        | _, _                                   -> None

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
        | "W" -> SymbolCode.fromSymbol (msg.Substring(19,1).ToCharArray().[0]) //  getSymbolCode (msg.Substring(19,1).ToCharArray().[0])
        | "E" -> SymbolCode.fromSymbol (msg.Substring(19,1).ToCharArray().[0])
        | _ -> None

    let (|Comment|_|) (symbol:char) (msg:string) =
        let comment = msg.Substring(msg.IndexOf(symbol) + 1).Trim()
        if comment = 
            String.Empty 
        then    
            None 
        else 
            Some comment



