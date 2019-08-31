open Expecto
open System
open System.Reflection
open Microsoft.Data.Sqlite
open SimpleMigrations.DatabaseProvider
open SimpleMigrations
open SimpleMigrations.Console


[<EntryPoint>]
let main argv =

    //LOAD faprs.migration and use the simple migrations console runner to create a new test database or migrate an existing one
    let mutable database_location : string = "DataSource=database.sqlite"
    if argv.Length > 0 then
        database_location <- argv.[0]
    Console.WriteLine database_location
    let assembly = Assembly.Load("faprs.migrations") // Assembly.GetExecutingAssembly()
    use db = new SqliteConnection (database_location) //"DataSource=..\..\database\database.sqlite"
    let provider = SqliteDatabaseProvider(db)
    let migrator = SimpleMigrator(assembly, provider)
    let consoleRunner = ConsoleRunner(migrator)
    consoleRunner.Run(Array.empty)
    //END migrate database

    //RUN TESTS
    let writeResults = TestResults.writeNUnitSummary ("FaprsTestResults.xml", "Expecto.Tests")
    let config = defaultConfig.appendSummaryHandler writeResults
    runTestsInAssembly config argv
    //runTestsInAssembly defaultConfig argv