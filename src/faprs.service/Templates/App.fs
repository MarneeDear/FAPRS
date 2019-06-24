module App

open Giraffe.GiraffeViewEngine

let layout (content: XmlNode list) =
    html [_class "has-navbar-fixed-top"] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1" ]
            title [] [encodedText "FAPRS"]
            //link [_rel "stylesheet"; _href "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" ]
            link [_rel "stylesheet"; _href "bulma.min.css" ] //_href "https://cdnjs.cloudflare.com/ajax/libs/bulma/0.6.1/css/bulma.min.css" ]
            link [_rel "stylesheet"; _href "/app.css" ]
            link [_rel "shortcut icon"; _href "/favicon.ico"]

        ]
        body [] [
            yield nav [ _class "navbar is-fixed-top has-shadow" ] [
                div [_class "navbar-brand"] [
                    a [_class "navbar-item"; _href "/"] [
                        img [_src "https://avatars0.githubusercontent.com/u/35305523?s=200"; _width "28"; _height "28"]
                    ]
                    div [_class "navbar-burger burger"; attr "data-target" "navMenu"] [
                        span [] []
                        span [] []
                        span [] []
                    ]
                ]
                div [_class "navbar-menu"; _id "navMenu"] [
                    div [_class "navbar-start"] [
                        a [_class "navbar-item"; _href "https://github.com/MarneeDear/FAPRS/blob/master/README.md"] [rawText "Getting started"]
                    ]
                ]
            ]
            yield! content
            yield footer [_class "footer is-fixed-bottom"] [
                div [_class "container"] [
                    div [_class "content has-text-centered"] [
                        p [] [
                            encodedText "Powered by "
                            a [_href "https://github.com/SaturnFramework/Saturn"] [rawText "Saturn"]
                            encodedText " - F# MVC framework created by "
                            a [_href "http://lambdafactory.io"] [rawText "λFactory"]
                            encodedText ", DireWolf, and Amateur Radio - KG7SIO"
                        ]
                    ]
                ]
            ]
            yield script [_src "/app.js"] []
        ]
    ]
