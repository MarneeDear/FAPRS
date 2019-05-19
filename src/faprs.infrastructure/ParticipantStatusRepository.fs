namespace faprs.infrastructure

open faprs.core
open faprs.core.TNC2MonActivePatterns
open faprs.core.APRSData
open faprs.core.Participant
open Microsoft.Data.Sqlite
open faprs.infrastructure.database

module ParticipantSatusRepository =
    open Dapper
    open System.Data.Common
    open System.Collections.Generic
    open FSharp.Control.Tasks.ContextInsensitive
    open System.Threading.Tasks
    open System

    [<CLIMutable>]
    type CancelMessageRecord =
        {
            message_row_id      : int64
            cancelled           : bool
            //cancelled_reason    : string
            cancelled_on        : Nullable<DateTime>
            cancelled_by        : string
        }

    [<CLIMutable>]
    type ParticipantStatusRecord =
        {
            message_id          : int64
            timestamp           : string
            tx_station_callsign : string
            participant_id      : string
            status_1            : int
            status_2            : int
            status_message      : string
            created_date        : int64
            created_by          : string //user identifier -- optional
            cancelled           : bool
            //cancelled_reason    : string
            cancelled_on        : int64
            cancelled_by        : string
        }
    
    [<CLIMutable>]
    type CancelPaticipantStatusRecord =
        {
            message_id          : int64
            cancelled_on        : int64
            cancelled_by        : string
        }

    //GET LIST OF MESSAGES
    let getAllActiveParticipantStatusRecords connectionString : Task<Result<ParticipantStatusRecord seq, exn>> =
        task {
            use connection = new SqliteConnection(connectionString)
            return! query connection "SELECT s.rowid as message_id, timestamp, tx_station_callsign, participant_id, 
                                      status_1, status_2, status_message, created_date, created_by,
                                      c.cancelled, c.cancelled_on, c.cancelled_by
                                      FROM status_message s
                                      LEFT OUTER JOIN cancelled_message c ON s.rowid = c.message_rowid" None
                                      //WHERE date(timestamp, '+ 1 day') >= date('now')" None
                                      //WHERE date(timestamp, '+ 14 day') >= date('now')
        }

    //SAVE PARTICIPANT STATUS
    //This will be append only by design
    //If a participant status changes the operator should cancel the old status and send a new one. 
    //We want the history of the participant and to be able to see that history
    let saveParticipantStatusRecord connectionString (v:ParticipantStatusRecord) : Task<Result<int,exn>> =
        task {
          use connection = new SqliteConnection(connectionString)
          return! execute connection "INSERT INTO status_message(timestamp, tx_station_callsign, participant_id, 
                                      status_1, status_2, status_message, created_date, created_by) 
                                      VALUES (@timestamp, @tx_station_callsign, @participant_id, 
                                      @status_1, @status_2, @status_message, @created_date, @created_by)" v
        }

    //SET the cancelled status
    //This will also stop the message from transmitting
    let cancelParticipantStatusRecord connectionString (v:CancelPaticipantStatusRecord) : Task<Result<int,exn>> =
        task {
          use connection = new SqliteConnection(connectionString) //, @cancelled, @cancelled_reason, @cancelled_on
          return! execute connection "INSERT INTO cancelled_message(message_rowid, cancelled, cancelled_on, cancelled_by)
                                      VALUES (@message_id, 1, @cancelled_on, @cancelled_by)" v
        }

    //SAVE MESSAGE
    //For non-participant status related information

    //CANCEL MESSAGE

    //CANCEL ALL MESSAGES

    //CONTINUE MESSAGE