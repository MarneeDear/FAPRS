namespace PositionReport

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index = 
        let content = [
            encodedText "You are here!"
        ]
        App.layout content