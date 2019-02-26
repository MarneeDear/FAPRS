module Index

open Giraffe.GiraffeViewEngine

let index =
    [
        section [_class "hero is-light"] [
            div [_class "hero-body"] [
                div [_class "container"] [
                    div [_class "columns is-vcentered"] [
                        div [_class "column"] [
                            p [_class "title"] [rawText "Welcome to FAPRS!"]
                            p [_class "subtitle"] [rawText "F# for APRS. This is a work in progress."]
                        ]
                    ]
                ]
            ]
        ]
        section [_class "section"] [
            div [_class "tile is-ancestor"] [
                div [_class "tile is-parent is-3"] [
                    article [_class "tile is-child notification is-info box"] [
                        a [_class "title"; _href "/position_reports"] [encodedText "Position Reports"]
                    ]
                ]
                div [_class "tile is-parent is-3"] [
                    article [_class "tile is-child notification is-info box"] [
                        a [_class "title"; _href "/race_reports"] [encodedText "Race Reports"]
                    ]
                ]
                div [_class "tile is-parent is-3"] [
                    article [_class "tile is-child notification is-info box"] [
                        a [_class "title"; _href "/messages"] [encodedText "Messages"]
                    ]
                ]
                div [_class "tile is-parent is-3"] [
                    article [_class "tile is-child notification is-info box"] [
                        a [_class "title"; _href "/weather_reports"] [encodedText "Weather Reports"]
                    ]
                ]
            ]            
        ]
        section [_class "section"] [
            h1 [_class "title"] [rawText "Resources"]
            div [_class "tile is-ancestor"] [
                div [_class "tile is-parent is-4"] [
                    article [_class "tile is-child notification is-dark box"] [
                        a [_class "title"; _href "https://github.com/MarneeDear/FAPRS"] [rawText "FAPRS Source"]
                    ]
                ]
                div [_class "tile is-parent is-4"] [
                    article [_class "tile is-child notification is-dark box"] [
                        a [_class "title "; _href "https://github.com/wb2osz/direwolf"] [rawText "Dire Wolf"]
                    ]
                ]
                div [_class "tile is-parent is-4"] [
                    article [_class "tile is-child notification is-dark box"] [
                        a [_class "title"; _href "http://www.aprs.org/" ] [rawText "APRS Spec"]
                    ]
                ]

            ]
        ]
    ]

let layout =
    App.layout index