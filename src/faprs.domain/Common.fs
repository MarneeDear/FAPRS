namespace faprs.domain

[<AutoOpen>]
module Common =
    //9 byte field
    type CallSign = string

//        https://stackoverflow.com/questions/791706/how-do-i-customize-output-of-a-custom-type-using-printf
    //9 byte field
    type Path =
        | WIDEnN    of string
        | RELAY     of string //Obsolete?
        | ECHO      of string
        | WIDE      of string //Obsolete?
        | TRACE     of string //Obsolete?
        | GATE      of string
        override this.ToString() =
            match this with
            | WIDEnN  _  -> "WIDE1-1"
            | _         -> failwith "Path type not found"