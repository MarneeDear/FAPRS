![alt text][logo]

[logo]: https://raw.githubusercontent.com/MarneeDear/FAPRS/master/logo.png "FAPRS"

# F# for APRS

A work in progress.

A system for sending APRS messages built in F#, because functional programming and static typing make everything better.

> Mac and Linux users: You too can work on this code. Install the .NET Core SDK for your system and you can do all the things. I recommend installing Visual Studio Code with the Ionide plugin for development.

## Prior Art

Here is a similar system using a Kantronics packet radio setup that functions like a BBS with keyboard-to-keyboard communications.

[![Packet Radio](http://img.youtube.com/vi/FJEVWMuz6Xg/0.jpg)](http://www.youtube.com/watch?v=FJEVWMuz6Xg "Kantronics Packet Radio Mail and BBS Operations")

## The architecture

### System

TODO draw a diagram

#### Requirements

* .NET Core SDK 2.2 and above
* [Dire Wolf v1.5](https://github.com/wb2osz/direwolf/releases/tag/1.5)
* SQLite3

#### Dependencies

See the project files, but here are the highlights.

* .NET Core SDK v2.2 and above 
* [Argu](https://github.com/fsprojects/Argu) for the CLI
* [Expecto](https://github.com/haf/expecto) for unit tests

Future enhancements will include a [Fable-based, Elmish, progressive web app](https://elmish.github.io/elmish/) and a Saturn based web server.

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

After cloning this repo you can restore the dependencies, run the migrations, run the tests, or run the CLI with `dotnet`.

### Run the migrations

```bash
dotnet run -p src/faprs.migrations
```

This should create two tables.

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
                          Position Reports require a Latitude and Longitude. See Position Report usage for more.

    Use 'faprs <subcommand> --help' for additional information.

OPTIONS:

    --sender, -s <sender> Your Call Sign
    --destination, -d <destination>
                          To whom is this intended? This could also be a an application from the To Calls list http://aprs.org/aprs11/tocalls.txt
    --path <path>         Only option is WIDE1-1
    --custommessage, --msg <msg>
                          Unformatted message. Anything you want but cuts off at 63 chars. in length
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
                          Your current latitude in this format 36.106964 N
    longitude <longitude> <hemisphere>
                          Your current longitude in this format 112.112999 E
    symbol <symbol>       Optional. Default is House (-). If you want to use House, do not use the symbol argument because dashes do not parse.
    comment <comment>     Optional. What do you want to say? <comment> must be 43 characters or fewer.
    --help                display this list of options.
```

#### Create a TNC2MON formatted frame with position report

```bash
dotnet run --project src/faprs.cli/ -- --save-to XMIT --sender KG7SIO-7 --destination APDW15 --path WIDE1-1 --rpt latitude 36.106964 longitude -112.112999 symbol b comment "Join Oro Valley Amateur Radio Club"
```

This will create a TNC2MON formatted frame with a lat/lon position report that looks like this:

```text
KG7SIO>APDW15:WIDE1-1:=36.106964N/112.112999WbJoin Oro Valley Amateur Radio Club.
```

The CLI will save it to the folder (and path) specified in `--save-to`. In this case the XMIT folder (if you have one) in your `present working directory.`

Let's break this down:

* Who is sending this packet?
  * `KG7SIO`
* The destination in this case is the Dire Wolf v1.5 `TOCALL` as [specified in APRS 1.1.](http://www.aprs.org/aprs11/tocalls.txt). The destination field can be overridden to indicate the sending application.
  * `APDW15`
* Your position is `36.106964 degrees N` and `112.112999 degrees W`
* Your APRS symbol is `b` for `bicycle`
* Your comment is `Join Oro Valley Amateur Radio Club`

#### Create a TNC2MON formatted frame with unformatted message (string)

```bash
dotnet run --project src/faprs.cli/ -- --save-to XMIT --sender KG7SIO-7 --destination APDW15 --path WIDE1-1 --msg "Join Oro Valley Amateur Radio Club"

```

This will create a TNC2MON formatted frame with a custom message that looks like this:

```text
KG7SIO>APDW15,WIDE1-1:Join Oro Valley Amateur Radio Club
```

The CLI will save it to the folder (and path) specified in `--save-to`. In this case the XMIT folder (if you have one) in your `present working directory.`

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

#### Run the tests while changing code (dotnet watch)

```bash
dotnet watch -p src/faprs.tests run -f netcoreapp2.2
```

The tests will re-run every time you save changes, including adding more tests.

## Deploy to a Raspberry PI

TBD

## Working with Dire Wolf and the `kissutil`

Follow the Dire Wolf user guide to install Dire Wolf on your system. Follow the user guide 
to configure and start Dire Wolf.

### Start the `kissutil` specifying the read and write folder

```bash
kissutil -o REC -f XMIT
```

`-o` specifies the folder to which Dire Wolf will write the received APRS messages.

`-f` specifies the folder from which Dire Wolf will read the messages FAPRS will send.

### User FAPRS to send a message through `kissutil`

Write a Position Report without Timestamp to the `XMIT` folder.

```bash
dotnet run --project src/faprs.cli/ -- --save-to XMIT --sender KG7SIO --destination APDW15 --path WIDE1-1 --msg "Join Oro Valley Amateur Radio Club"
```

## DireWolf tips

Debugging tip:  Use the direwolf "-d n" command line option to print the KISS frames in hexadecimal so we can see what is being sent.

## Run the web project

`faprs.service` provides a web interface to enter messages you want to send over APRS and to see messages received over APRS.

You can run it with `dotnet run` and `dotnet watch`.

Run with re-loading after changes.

```bash
dotnet watch -p src/faprs.service/ run
```

Just run.

```bash
dotnet run -p src/faprs.service/
```
