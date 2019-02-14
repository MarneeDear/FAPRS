module CommandArguments

open Argu

[<CliPrefix(CliPrefix.None)>]
type PositionReportArguments =
    | [<Mandatory>] Latitude    of latitude:float * hemisphere:char
    | [<Mandatory>] Longitude   of longitude:float * hemisphere:char
    | Symbol                    of symbol:char
    | Comment                   of comment:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Latitude _    -> "Your current latitude in this format 3050.15 N"
            | Longitude _   -> "Your current longitude in this format 2093.13 E"
            | Symbol _      -> "Optional. Default is House (-). If you want to use House, do not use the symbol argument because dashes do not parse."
            | Comment _     -> "Optional. What do you want to say? <comment> must be 43 characters or fewer."
and SourcePathArguments =
    | [<Mandatory>] [<AltCommandLine("-s")>] Sender         of sender:string
    | [<Mandatory>] [<AltCommandLine("-d")>] Destination    of destination:string
    | Path                                                  of path:string
    | [<AltCommandLine("--rpt")>] PositionReport            of rpt:ParseResults<PositionReportArguments>
    | CustomMessage                                         of msg:string
    | [<AltCommandLine("--save-to")>] SaveFilePath          of save:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Sender _          -> "Your Call Sign"
            | Destination _     -> "To whom is this intended"
            | Path _            -> "Only option is WIDE1-1" 
            | PositionReport _  -> "There are subargument -- TODO" //TODO
            | CustomMessage _   -> "Anything you want but cuts off at X length" //TODO
            | SaveFilePath _    -> "Send output to a file in this location to be used by Dire Wolf's kissutil"