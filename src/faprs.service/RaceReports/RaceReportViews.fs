namespace RaceReport

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine
    open ViewModels
    open System.Text.Unicode

    let index (model:AllMessages) = 
        let content = [
            div [ _class "columns"] [
                div [_class "column"] [
                    p [_class "notification is-primary"] [ encodedText "Participant Number" ]
                    p [_class "notification is-primary"] [ encodedText model.ParticipantNumber ]
                ]
                div [_class "column"] [
                    p [_class "notification is-warning"] [ encodedText "Station Id" ]
                    p [_class "notification is-warning"] [ encodedText model.StationId ]
                ]
                div [_class "column"] [
                    p [_class "notification is-danger"] [ encodedText "Status 1" ]
                    p [_class "notification is-danger"] [ encodedText model.Status1 ]
                ]
                div [_class "column"] [
                    p [_class "notification is-danger"] [ encodedText "Status 2" ]
                    p [_class "notification is-danger"] [ encodedText model.Status2 ]
                ]
                div [_class "column"] [
                    p [_class "notification is-success"] [ encodedText "Time reported at station" ]
                    p [_class "notification is-success"] [ encodedText (model.Timestamp.ToString "yyyy-mm-dd HH:MM") ]
                ]
                div [_class "column"] [
                    p [_class "notification is-info"] [ encodedText "Status Message" ]
                    p [_class "notification is-info"] [ encodedText model.StatusMessage ]
                ]

            ]
        ]
        App.layout content