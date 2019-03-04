# F# for APRS

A work in progress.

A system for sending APRS messages built in F#, because functional programming and static typing make everything better.

## The architecture

### System

TODO draw a diagram

#### Requirements

* .NET Core SDK 2.2 and above
* [Dire Wolf v1.5](https://github.com/wb2osz/direwolf/releases/tag/1.5)

#### Dependencies

See the project files, but here are the highlights.

* .NET Core SDK v2.2 and above 
* [Argu](https://github.com/fsprojects/Argu) for the CLI
* [Expecto](https://github.com/haf/expecto) for unit tests

Future enhancements will include a [Fable-based, Elmish, progressive web app](https://elmish.github.io/elmish/).

### Program

The design calls for 3 main parts:

* A service that runs on the APRS server (work in progress).
  * This service picks up new APRS messages from Dire Wolf. The messages are written in TNC2MON format to a designated folder by the DireWolf `kissutil`.
* A CLI that can be used to write TNC2MON format frames.
  * They can be written to a folder monitored by the `kissutil`. When the `kissutil` detects a new file DireWolf will process the frames and transmit.
  * Presently only produces TNC2MON formatted messages with `Lat/Lon Position Report without Timestamp`, and a plain text message
* A progressive web app that can be used to compose APRS packets that will be used by the `kissutil`. (not started)
  * The service will serve the web app.

## How to setup and run

First, make sure you have .NET Core SDK 2.2 or above installed.

Currently, there is only a CLI that can be used to create a `TNC2MON` formatted KISS frame suitable for the Dire Wolf `kissutil`, but a progressive web app (in Fable) is planned.

The CLI can produce a frame with a simple message or a Lat/Lon position report without timestamp as per the APRS 1.01 specification.

After cloning this repo you can restore the dependencies, run the tests, or run the CLI with `dotnet`.

### Run the CLI and see the possible commands

From the root project folder (the folder that was created when you cloned this repo)

```bash
dotnet run --project src/faprs.cli/ -- --help
```

This will restore dependencies, compile the code, and run the CLI (please be patient).

You should see the help output looking like this:

```bash
USAGE: faprs [--help] --sender <sender> --destination <destination> [--path <path>] [--custommessage <msg>] [--savefilepath <save>] [<subcommand> [<options>]]

SUBCOMMANDS:

    --positionreport, --rpt <rpt>
                          There are subargument -- TODO

    Use 'faprs <subcommand> --help' for additional information.

OPTIONS:

    --sender, -s <sender> Your Call Sign
    --destination, -d <destination>
                          To whom is this intended? This could also be a an application from the To Calls list http://aprs.org/aprs11/tocalls.txt
    --path <path>         Only option is WIDE1-1
    --custommessage, --msg <msg>
                          Anything you want but cuts off at X length
    --savefilepath, --save-to <save>
                          Send output to a file in this location to be used by Dire Wolf's kissutil
    --help                display this list of options.
```

There are also sub-commands for the `position report`. You can get help for those too.

```bash
dotnet run --project src/faprs.cli/ -- --rpt --help
```

You should see some helpful stuff like this.

```bash
USAGE: faprs --positionreport [--help] latitude <latitude> <hemisphere> longitude <longitude> <hemisphere> [symbol <symbol>] [comment <comment>]

OPTIONS:

    latitude <latitude> <hemisphere>
                          Your current latitude in this format 3050.15 N
    longitude <longitude> <hemisphere>
                          Your current longitude in this format 2093.13 E
    symbol <symbol>       Optional. Default is House (-). If you want to use House, do not use the symbol argument because dashes do not parse.
    comment <comment>     Optional. What do you want to say? <comment> must be 43 characters or fewer.
    --help                display this list of options.
```

#### Create a TNC2MON formatted frame with position report

```bash
dotnet run --project src/faprs.cli/ -- --save-to XMIT --sender KG7SIO --destination APDW15 --path WIDE1-1 --rpt latitude 3000.5 N longitude 1000.5 W symbol b comment "Subscribe to PewDiePie."

```

This will create a TNC2MON formatted frame with a lat/lon position report that looks like this:

```text
KG7SIO>APDW15:WIDE1-1:=3000.5N/1000.5WbSubscribe to PewDiePie.
```

The CLI will save it to the folder (and path) specified in `--save-to`. In this case the XMIT folder (if you have one) in your `present working directory.`

Let's break this down:

* Who is sending this packet?
  * `KG7SIO`
* The destination in this case is the Dire Wolf v1.5 `TOCALL` as [specified in APRS 1.1.](http://www.aprs.org/aprs11/tocalls.txt). The destination field can be overridden to indicate the sending application.
  * `APDW15`
* Your position is `3000.5 degrees N` and `1000.5 degrees W`
* Your APRS symbol is `b` for `bicycle`
* Your comment is `The sleeper has awakened.`

## Developers and contributors

Contributors welcome. Please follow the [F# Style Guide](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/) and [open source contributors guide.](https://opensource.guide/how-to-contribute/)

### How to get started

1. Clone this repo
2. Run `dotnet build` or run the tests like below under `Run the tests`

Or

There is a `DOCKERFILE` if you are so inclined.

Run service:  `dotnet run -p src/faprs.service` or `dotnet watch -p src/faprs.service/ run`

### Run the tests

```bash
dotnet run -p src/faprs.tests -f netcoreapp2.2
```

This will restore dependencies, compile all projects, and run tests.

#### Run the tests while changing code

```bash
dotnet watch -p src/faprs.tests run -f netcoreapp2.2
```

The tests will re-run every time you save changes, including adding more tests.
