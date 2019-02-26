namespace RaceReport

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index = 
        let content = [
            encodedText "You are almost there!"
        ]
        App.layout content