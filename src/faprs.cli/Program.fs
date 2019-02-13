// Learn more about F# at http://fsharp.org

open System
open Argu
open CommandArguments
open faprs.domain.APRSData
open faprs.domain.TNC2MON
open faprs.domain

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

    [<EntryPoint>]
    let main argv =
        let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
        let parser = ArgumentParser.Create<SourcePathArguments>(programName = "faprs", errorHandler = errorHandler)

        try
            let results = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
            printfn "Got parse results %A" <| results.GetAllResults()

            let sender = results.GetResult(Sender)
            let destination = results.GetResult(Destination)
            let path = Common.Path.WIDEnN //only this for now TODO
            let pRpt = results.GetResult(CommandArguments.PositionReport)
            let lat : Latitude = { Degrees = pRpt.GetResult(CommandArguments.Latitude); Hemisphere = LatitiudeHemisphere.North }
            let lon : Longitude = { Degrees = pRpt.GetResult(CommandArguments.Longitude); Hemisphere = LongitudeHemisphere.West }
            let symbol = getSymbolCode (pRpt.GetResult(CommandArguments.Symbol))
            let comment = pRpt.GetResult(CommandArguments.Comment)
            let packet = 
                {
                    Sender = CallSign.create sender
                    Destination = CallSign.create destination
                    Path = path
                    Message = Some (PositionReport { Position = { Latitude = lat; Longitude = lon }; Symbol = symbol; Comment = PositionReportComment.create comment})
                }
            Console.info <| (sprintf "%s" (packet.ToString()))
        with e ->
            printfn "%s" e.Message

        0