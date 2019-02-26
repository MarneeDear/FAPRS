namespace PositionReport

open Saturn
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http

module Controller =

    let indexAction (ctx: HttpContext) = 
        task {
            return Views.index
        }

    let resource = controller {
        index indexAction
    }