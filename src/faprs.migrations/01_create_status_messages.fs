namespace fapr.migrations
open SimpleMigrations

[<Migration(01L, "Create status_message")>]
type CreateStatusMessage() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE status_message(
      timestamp TEXT NOT NULL,
      tx_station_callsign TEXT NOT NULL,
      participant_id TEXT NOT NULL,
      status_1 INTEGER NOT NULL,
      status_2 INTEGER NULL,
      status_message TEXT NOT NULL,
      created_date INTEGER NOT NULL,
      created_by TEXT NULL,
      UNIQUE (timestamp, tx_station_callsign, participant_id)
    );
    
    CREATE TABLE cancelled_message(
      message_row_id INTEGER PRIMARY KEY,
      cancelled INTEGER DEFAULT 0,
      cancelled_on INTEGER NULL,
      cancelled_by TEXT NULL,
      UNIQUE (message_row_id)
    );")

  override __.Down() =
    base.Execute(@"DROP TABLE status_message; DROP TABLE cancelled_message")
