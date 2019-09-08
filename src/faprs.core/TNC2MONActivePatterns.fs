namespace faprs.core

open System

module TNC2MonActivePatterns = 

//TODO map into their respective types, especially the non-string values
    //Remove the channel from the frame
    let (|Frame|_|) (record:string) =
        match record with
        | r when String.IsNullOrWhiteSpace(r) -> None
        | r when r.IndexOf(" ") < 1 -> None //maybe return r because there was no channel and that's ok?
        | r when (r.Substring(r.IndexOf(" "))).Trim().Length > 0 -> Some ((r.Substring(r.IndexOf(" "))).Trim())
        | _ -> None

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

    let (|Information|_|) (frame:string) =
        if frame.IndexOf(":") < 1 then 
            None
        else
            Some (frame.Substring(frame.IndexOf(":") + 1))

    