module CommandArguments

open Argu

//TODO should restructure the cli because only certain reports require a sender and destination
//DireWolf will not support all types?

[<CliPrefix(CliPrefix.None)>]
type PositionReportArguments =
    | [<Mandatory>] [<AltCommandLine("--lat")>] Latitude    of latitude:float //* hemisphere:char
    | [<Mandatory>] [<AltCommandLine("--lon")>] Longitude   of longitude:float //* hemisphere:char
    |               [<AltCommandLine("-s")>]    Symbol      of symbol:char
    |               [<AltCommandLine("-c")>]    Comment     of comment:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Latitude _    -> "Your current latitude in decimal coordinates (simple standard) format"
            | Longitude _   -> "Your current longitude in decimal coordinates (simple standard) format"
            | Symbol _      -> "Optional. Default is House (-). If you want to use House, do not use the symbol argument because dashes do not parse."
            | Comment _     -> "Optional. What do you want to say? <comment> must be 43 characters or fewer."
and CustomMessageArguments =
    | [<Mandatory>] [<AltCommandLine("-a")>] Addressee  of addressee:string
    | [<Mandatory>] [<AltCommandLine("-m")>] Message    of message:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Addressee _   -> "For whom is the message intended (Call Sign)"
            | Message _     -> "The message you want to send"
and SourcePathArguments =
    | [<Mandatory>] [<AltCommandLine("-s")>]    Sender          of sender:string
    | [<AltCommandLine("-d")>]                  Destination     of destination:string
    | [<AltCommandLine("-p")>]                  Path            of path:string
    | [<AltCommandLine("--rpt")>]               PositionReport  of rpt:ParseResults<PositionReportArguments>
    | [<AltCommandLine("--msg")>]               CustomMessage   of msg:ParseResults<CustomMessageArguments>
    | [<AltCommandLine("--save-to")>]           SaveFilePath    of save:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Sender _          -> "Your Call Sign"
            | Destination _     -> "To whom is this intended? This could also be a an application from the To Calls list http://aprs.org/aprs11/tocalls.txt"
            | Path _            -> "Only option is WIDE1-1" 
            | PositionReport _  -> "Position Reports require a Latitude and Longitude. See Position Report usage for more." //TODO
            | CustomMessage _   -> "Unformatted message. Anything you want but cuts off at 63 chars. in length" //TODO
            | SaveFilePath _    -> "Send output to a file in this location to be used by Dire Wolf's kissutil"