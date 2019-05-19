namespace RaceReport

module ViewModels = 
    open System

    [<CLIMutable>]
    type AllMessages = 
        {
            ParticipantNumber : string
            StationId : string
            Status1 : string
            Status2 : string
            Timestamp : DateTime
            StatusMessage : string
        }
