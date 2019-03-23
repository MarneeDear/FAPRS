// Learn more about F# at http://fsharp.org

open System
open Argu
open CommandArguments
open faprs.core.TNC2MON
open faprs.core
open faprs.core.APRSData
open faprs.infrastructure.TNC2MONRepository

(*

EXAMPLES: 
$ dotnet run --sender KG7SIO --destination KG7SIL --positionreport latitude 3216.4 longitude 11057.3
$ dotnet run --sender KG7SIO --destination KG7SIL --positionreport latitude 3216.4 longitude 11057.3 symbol b comment "Hello world!"
$ dotnet run --sender KG7SIO --destination KG7SIL --rpt latitude 3216.4 N longitude 11057.3 W

Use the short argument flags and save a kissutl frame to the current directors (.). Use the defualts for symbol (House) and comment
$ dotnet run -s KG7SIO -d KG7SIL --save-to . --rpt latitude 3216.4 N longitude 11057.3 W

On the Pi 3
$ ./faprs.cli -s KG7SIO -d KG7SIL --save-to ~/faprs-stuff/kiss-output/ --rpt latitude 3216.4 N longitude 11057.3 W

NOTE you can't use - (dash) in symbol because Argu won't parse it

*)

module Console =

    let log =
        let lockObj = obj()
        fun color s ->
            lock lockObj (fun _ ->
                Console.ForegroundColor <- color
                printfn "%s" s
                Console.ResetColor())

    let complete = log ConsoleColor.Magenta
    let ok = log ConsoleColor.Green
    let info = log ConsoleColor.Blue
    let warn = log ConsoleColor.Yellow
    let error = log ConsoleColor.Red

module Main =

    let composePositionReportMessage (pRpt: ParseResults<PositionReportArguments>) = 
        let latArgu = pRpt.GetResult(CommandArguments.Latitude)
        let lonArgu = pRpt.GetResult(CommandArguments.Longitude)
        let lat = FormattedLatitude.create latArgu
        let lon = FormattedLongitude.create lonArgu
        let symbol = getSymbolCode ((pRpt.TryGetResult(CommandArguments.Symbol)) |> Option.defaultValue '-')
        let comment = pRpt.TryGetResult(CommandArguments.Comment) |> Option.defaultValue String.Empty
        { 
            Position = { Latitude = lat; Longitude = lon }
            Symbol = symbol 
            Comment = PositionReportComment.create comment
        }

    [<EntryPoint>]
    let main argv =
        let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
        let parser = ArgumentParser.Create<SourcePathArguments>(programName = "faprs", errorHandler = errorHandler)

        try
            let results = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
            printfn "Got parse results %A" <| results.GetAllResults()

            let saveTo = results.TryGetResult(CommandArguments.SaveFilePath)
            
            let sender = results.GetResult(Sender)
            let destination = results.GetResult(Destination)
            let path = Common.Path.WIDEnN //only this for now TODO

            let pRpt = results.TryGetResult(CommandArguments.PositionReport)
            let msg = results.TryGetResult(CommandArguments.CustomMessage)

            let messageData =
                match pRpt, msg with
                | Some _, Some _        -> failwith "Cannot use both Position Report and Custom Message at the same time."
                | Some rptArgs, None    -> PositionReportWithoutTimeStamp (composePositionReportMessage rptArgs) 
                | None _, Some msg      -> Unformatted (UnformattedMessage.create msg)
                | None, None            -> failwith "Must provide a position report or a custom message."
            
            let packet =
                {
                    Sender = CallSign.create sender
                    Destination = CallSign.create destination
                    Path = path
                    Message = Some messageData
                }

            match saveTo with
            | Some path -> writeKissUtilRecord None [packet] path (DateTime.Now.ToString("yyyyMMddHHmmssff")) 
            | None      -> ()
        with e ->
            Console.error <| (sprintf "%s" e.Message)

        0