module ParticipantStatusRepositoryTests

open Expecto
open faprs.infrastructure.ParticipantSatusRepository
open faprs.core.Participant
open System
open FSharp.Control.Tasks
open System.Threading.Tasks

[<Literal>]
let CONN = "DataSource=../../database.sqlite"

[<Tests>]
let ParticipantStatusRepoTests =
    testList "Participant database layer tests" [
        testCase "Can create a participant status message" <| fun _ ->
        let result = 
            task {
                let data =
                    {
                        TimeStamp = RecordedOn.create None
                        ParticipantID = (ParticipantID.create "TEST").Value
                        ParticipantStatus = ParticipantStatus.Continued (ParticipantStatusMessage.create "We have a winner!")
                        Cancelled = false
                    }

                let (st1:int, st2:int, m:string) = data.ParticipantStatus.ToStatusCombination()
                let record =
                    {
                        message_id          = 0L
                        timestamp           = RecordedOn.value data.TimeStamp
                        tx_station_callsign = "KG7SIO"
                        participant_id      = ParticipantID.value data.ParticipantID
                        status_1            = st1
                        status_2            = st2
                        status_message      = m
                        created_date        = DateTimeOffset.Now.ToUnixTimeSeconds()
                        created_by          = "TEST"
                        cancelled           = false
                        cancelled_on        = 0L
                        cancelled_by        = String.Empty
                    }
                
                return! saveParticipantStatusRecord CONN record
                //match result with
                //| Ok r ->   Console.WriteLine(sprintf "RESULTS %i" r)
                //            Expect.isTrue (r > 100) "Participant was not saved"
                //| Error msg -> failwith msg.Message
                //Expect.isOk result "Participant was not saved."
            }
        Expect.isOk result.Result "Participant was not created."
    ]