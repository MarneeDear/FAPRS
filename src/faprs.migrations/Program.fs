module Program

open System.Reflection
open SimpleMigrations
open Microsoft.Data.Sqlite
open SimpleMigrations.DatabaseProvider
open SimpleMigrations.Console
open System

[<EntryPoint>]
let main argv =
    
    let mutable database_location : string = "DataSource=..\..\database\database.sqlite"
    if argv.Length > 0 then
        database_location <- argv.[0]
    Console.WriteLine database_location
    let assembly = Assembly.GetExecutingAssembly()
    use db = new SqliteConnection (database_location) //"DataSource=..\..\database\database.sqlite"
    let provider = SqliteDatabaseProvider(db)
    let migrator = SimpleMigrator(assembly, provider)
    let consoleRunner = ConsoleRunner(migrator)
    consoleRunner.Run(Array.empty)
    0