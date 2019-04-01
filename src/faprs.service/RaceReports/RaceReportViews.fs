namespace RaceReport

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index = 
        let content = [
            encodedText "Status report"
            br []
            encodedText "Participant number (bib number)"
            br []
            encodedText "Status"
            br []
            encodedText "Time through way point"
            br []
            encodedText "Status message"
            br []
        ]
        App.layout content