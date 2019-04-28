module ParticipantStatusTests

open Expecto
open faprs.core.Participant
open System

(*
TIMESTAMP
Month/Day/Hours/Minutes (MDHM) format is a fixed 8-character field,
consisting of the month (01–12) and day-of-the-month (01–31), followed by
the time in hours and minutes zulu. For example:
*)

[<Literal>]
let GOOD_PARTICIPANT_CONTINUED_STATUS = "1234511We have a winner!"
let TEST_DATE = Convert.ToDateTime("2019-01-01T00:00:00")
[<Literal>]
let TEST_TIMESTAMP = "01010000"
[<Literal>]
let GOOD_PARTICIPANT_STATUS_MSG = "Runner has cut on leg. Not actively bleeding."
[<Literal>]
let TOO_LONG_STATUS_MSG = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu"
[<Literal>]
let GOOD_PARTICIPANT_NBR = "12345"

[<Tests>]
let ParticipantTests =
    testList "Participant status tests" [
        testCase "Can create a good paricipant message" <| fun _ ->
            let data =
                {
                    TimeStamp = RecordedOn.create None
                    ParticipantID = ParticipantID.create "12345"
                    ParticipantStatus = ParticipantStatus.Continued (ParticipantStatusMessage.create "We have a winner!")
                    Cancelled = false
                }
            Expect.stringContains (data.ToString()) GOOD_PARTICIPANT_CONTINUED_STATUS "Message worked"
        testCase "Can create specific recorded-on timestamp"  <| fun _ ->
            //Console.WriteLine(sprintf "%A" TEST_DATE)
            Expect.equal (RecordedOn.value (RecordedOn.create (Some TEST_DATE))) TEST_TIMESTAMP "The timestamp matched."
        testCase "Can create not too long participant status message" <| fun _ ->
            let result = ParticipantStatusMessage.create GOOD_PARTICIPANT_STATUS_MSG
            let msg_len = 
                result
                |> ParticipantStatusMessage.value
                |> String.length
            Expect.equal msg_len 45 "Message character count is OK"
            Expect.equal (ParticipantStatusMessage.value result) GOOD_PARTICIPANT_STATUS_MSG "Message stayed intact"
        testCase "Too long message is cut off at 239 chars" <| fun _ ->
            let msg_len = 
                (ParticipantStatusMessage.create TOO_LONG_STATUS_MSG)
                |> ParticipantStatusMessage.value
                |> String.length
            Expect.equal msg_len 239 "Message character count was shortened"
        testCase "Can create participant number" <| fun _ ->
            let result = ParticipantID.create GOOD_PARTICIPANT_NBR
            Expect.equal (ParticipantID.value result) GOOD_PARTICIPANT_NBR "Participant number string was created"
        testCase "Short participant number is padded tp length 5 chars" <| fun _ ->
            Expect.equal (ParticipantID.value (ParticipantID.create "9")) "    9" "Participant number is not fixed-length of 5"
        testCase "Can revert a properly formated timestamp to datetime" <| fun _ ->
            Expect.equal (RecordedOn.revert TEST_TIMESTAMP) TEST_DATE "Reverted timestamp equals input datetime"
    ]