// Learn more about F# at http://fsharp.org

open System
open Argu
open CommandArguments
open faprs.core.TNC2MON
open faprs.core
open faprs.core.KISS
open faprs.core.APRSData
open faprs.infrastructure.DireWolf.KissUtil

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
        let symbol = SymbolCode.fromSymbol ((pRpt.TryGetResult(CommandArguments.Symbol)) |> Option.defaultValue '-')
        let comment = pRpt.TryGetResult(CommandArguments.Comment) |> Option.defaultValue String.Empty
        { 
            Position = { Latitude = lat; Longitude = lon }
            Symbol = (if symbol.IsSome then symbol.Value else SymbolCode.House)
            Comment = PositionReportComment.create comment
        }

    let composeMessage (msg:ParseResults<CustomMessageArguments>) =
        let addressee = msg.GetResult(CommandArguments.Addressee)
        let message = msg.GetResult(CommandArguments.Message)
        let callsign = 
            match CallSign.create addressee with
            | Some c -> c
            | None -> failwith "ADDRESSEE cannot be empty and must be 1 - 9 characters." //TODO use a proper flow/pipeline with result type instead?
        {
            Addressee = callsign
            MessageText = MessageText.create message
            MessageNumber = MessageNumber.create String.Empty
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
            let destination = 
                let r = results.TryGetResult(Destination)
                match r with
                | Some d    -> d
                | None      -> "APDW15"

            let path = Common.Path.WIDEnN //only this for now TODO

            let pRpt = results.TryGetResult(CommandArguments.PositionReport)
            let msg = results.TryGetResult(CommandArguments.CustomMessage)

            let information =
                match pRpt, msg with
                | Some _, Some _        -> failwith "Cannot use both Position Report and Custom Message at the same time."
                | Some rptArgs, None    -> PositionReportWithoutTimeStamp (composePositionReportMessage rptArgs) 
                | None _, Some msg      -> Message (composeMessage msg) //Unformatted (UnformattedMessage.create msg)
                | None, None            -> failwith "Must provide a position report or a message."
            
            let senderCallSign = 
                match CallSign.create sender with
                | Some c -> c
                | None -> failwith "SENDER cannot be empty and must be 1 - 9 characters. See APRS 1.01."

            let destCall =
                match CallSign.create destination with
                | Some c -> c
                | None -> failwith "DESTINATION cannot be empty and must be 1 - 9 characters. See APRS 1.01."

            let packet =
                {
                    Sender      = senderCallSign
                    Destination = destCall
                    Path        = WIDEnN WIDE11
                    Information = Some information
                }

            let txDelay =                 
                //Some [ TxDelay 0; TxDelay 0; ] //2 seconds in 10 ms units
                None

            match saveTo with
            | Some path -> writeKissUtilRecord txDelay [packet] path (DateTime.Now.ToString("yyyyMMddHHmmssff")) 
            | None      -> ()
        with e ->
            Console.error <| (sprintf "%s" e.Message)

        0