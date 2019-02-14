open Expecto

[<EntryPoint>]
let main argv =
    let writeResults = TestResults.writeNUnitSummary ("FaprsTestResults.xml", "Expecto.Tests")
    let config = defaultConfig.appendSummaryHandler writeResults
    runTestsInAssembly config argv
    //runTestsInAssembly defaultConfig argv