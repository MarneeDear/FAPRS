namespace RaceReport

open Saturn
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open faprs.core.Participant
open faprs.infrastructure
open ViewModels
open System

module Controller =

    let indexAction (ctx: HttpContext) = 
        task {
            let model = 
                {
                    ParticipantNumber = "00999"
                    StationId = "KG7SIO"
                    Status1 = "Injured"
                    Status2 = "Resting"
                    Timestamp = DateTime.Now
                    StatusMessage = "For display. Not a real status."
                }
            return Views.index model
        }

    let resource = controller {
        index indexAction
    }
