# F# for APRS

A work in progress.

A system for sending APRS messages built in F#, because functional programming and static typing make everything better.

## The architecture

### System

#### Requirements

* .NET Core SDK 2.2 and above

### Program

## How to setup and run

Currently there is only a CLI that can be used to create a `TNC2MON` formatted KISS frame suitable for the Dire Wolf `kissutil`.

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
dotnet run --project src/faprs.cli/ -- --sender KG7SIO --destination APDW15 --path WIDE1-1 --rpt latitude 3000.5 N longitude 1000.5 W symbol b comment "The sleeper has awakened." --save-to XMIT

```

This will create a TNC2MON formatted frame with a lat/lon position report that looks like this:

```text
KG7SIO>APDW15:WIDE1-1:=3000.5N/1000.5WbThe sleeper has awakened.
```

The CLI will save it to the folder (and path) specified in `--save-to`. In this case the XMIT folder (if you have one) in your `present working directory.`

Let's break this down:

* Who is sending this packet?
  * KG7SIO
* The destination in this case is the Dire Wolf TOCALL as [specified in APRS 1.1.](http://www.aprs.org/aprs11/tocalls.txt). The destination field can be overridden to indicate the sending application.
* Your position is 3000.5 degrees N and 1000.5 degrees W
* Your APRS symbol is b for `bicycle`
* Your comment is "The sleeper has awakened."


## Developers and contributors

### Run the tests

dotnet watch -p src/faprs.tests run -f netcoreapp2.1
