module ParticipantStatusRepositoryTests

open Expecto
open faprs.infrastructure.ParticipantSatusRepository
open faprs.core.Participant
open System
open FSharp.Control.Tasks
open System.Threading.Tasks
open System.Numerics

[<Literal>]
let CONN = "DataSource=database.sqlite"

[<Tests>]
let ParticipantStatusRepoTests =
    testSequenced <| testList "Participant database layer tests" [
        testCase "Can create a participant status message" <| fun _ ->
            let result = 
                task {
                    let data =
                        {
                            TimeStamp = RecordedOn.create None
                            ParticipantID = (ParticipantID.create "TEST").Value
                            ParticipantStatus = ParticipantStatus.Continued (ParticipantStatusMessage.create "We have a winner!")
                            //Cancelled = false
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
                }
            Expect.isOk result.Result "Participant was not created."
        testCase "Can get a list of participant status records" <| fun _ ->
            let result = 
                let q =
                    task {
                        return! getAllActiveParticipantStatusRecords CONN 
                    }
                Expect.isOk q.Result "Query should have returned at least one participant status report"
                match q.Result with
                | Ok r -> r
                | Error msg -> Seq.empty
            Expect.isFalse (result |> Seq.isEmpty) "Query should have returned at least one participant status report"
        testCase "Can cancel a participant status report" <| fun _ ->
            let getResult = 
                let q =
                    task {
                        return! getAllActiveParticipantStatusRecords CONN 
                    }
                match q.Result with
                | Ok r -> r |> Seq.head
                | Error msg -> failwith "There are no participant status messages to cancel"   
            let cancel =
                {
                    message_id      = getResult.message_id
                    cancelled_on    = DateTimeOffset.Now.ToUnixTimeSeconds()
                    cancelled_by    = "TEST"
                }
            let cancelResult = 
                task {
                    return! cancelParticipantStatusRecord CONN cancel
                }
            Expect.isOk cancelResult.Result "Query should have returned at least one participant status report"
    ]