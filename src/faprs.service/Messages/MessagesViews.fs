namespace Messages

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index = 
        let content = [
            encodedText "Hello world is all you need"
        ]
        App.layout content