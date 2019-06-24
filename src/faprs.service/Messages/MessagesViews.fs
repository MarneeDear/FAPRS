﻿namespace Messages

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index (v:string list) = 
        let content = [
            yield div [ _class "hero is-warning" ] [ 
                div [ _class "hero-body"] [ 
                    p [ _class "title" ] [encodedText "You will see all of the latest APRS messages or IPFS files hashes"]
                    ]
                ] 
            yield div [_class "box"] [
                for f in v do
                    yield p [ _class "subtitle is-2" ] [ encodedText f]
                ]
            ]
        App.layout content