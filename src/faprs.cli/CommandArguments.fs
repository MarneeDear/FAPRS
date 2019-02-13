module CommandArguments

open Argu

type PositionReportArguments =
    | [<CliPrefix(CliPrefix.None)>] Latitude    of latitude:float
    | [<CliPrefix(CliPrefix.None)>] Longitude   of longitude:float
    | [<CliPrefix(CliPrefix.None)>] Symbol      of symbol:char
    | [<CliPrefix(CliPrefix.None)>] Comment     of comment:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Latitude _    -> "Your current latitude"
            | Longitude _   -> "Your current longitude"
            | Symbol _      -> "Default is House (-)" 
            | Comment _     -> "What do you want to say. <comment> be 43 characters or fewer."
and SourcePathArguments =
    | [<Mandatory>] Sender      of sender:string
    | [<Mandatory>] Destination of destination:string
    | Path of path:string
    | PositionReport    of rpt:ParseResults<PositionReportArguments>
    | CustomMessage     of msg:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Sender _          -> "Your Call Sign"
            | Destination _     -> "To whom is this intended"
            | Path _            -> "Only option is WIDE1-1" 
            | PositionReport _  -> "There are subargument -- TODO"
            | CustomMessage _   -> "Anything you want but cuts off at X length"