namespace faprs.core

[<AutoOpen>]
module Common =

(*
helful guide to some of the design decision made here
//https://stackoverflow.com/questions/791706/how-do-i-customize-output-of-a-custom-type-using-printf

*)

    type WIDEnN =
        | WIDE11
        | WIDE21
        | WIDE22
        override this.ToString() =
            match this with 
            | WIDE11    -> "WIDE1-1"
            | WIDE21    -> "WIDE2-1"
            | WIDE22    -> "WIDE2-2"
        static member fromString p =
            match p with
            | "WIDE1-1" -> WIDE11
            | "WIDE2-1" -> WIDE21
            | "WIDE2-2" -> WIDE22
            | _         -> WIDE11 //Use this as the default

    //9 byte field
    //aka the UNPROTO path
    //http://wa8lmf.net/DigiPaths/index.htm#Recommended
    type Path =
        | WIDEnN of WIDEnN
        override this.ToString() =
            match this with
            | WIDEnN p ->   match p with
                            | WIDE11    -> WIDE11.ToString()
                            | WIDE21    -> WIDE21.ToString()
                            | WIDE22    -> WIDE22.ToString()

    //This is only a subset of the codes because I don't want to support all of them
    type SymbolCode =
        | House 
        | Bicycle 
        | Balloon 
        | Hospital
        | Jeep 
        | Truck
        | Motorcycle
        | Jogger
        member this.ToChar() =
            match this with
            | House         -> '-'
            | Bicycle       -> 'b'
            | Balloon       -> 'O'
            | Hospital      -> 'h'
            | Jeep          -> 'j'
            | Truck         -> 'k'
            | Motorcycle    -> '<'
            | Jogger        -> '['
        static member fromSymbol s =
            match s with
            | '-' -> Some House
            | 'b' -> Some Bicycle
            | 'O' -> Some Balloon
            | 'h' -> Some Hospital
            | 'j' -> Some Jeep
            | 'k' -> Some Truck
            | '<' -> Some Motorcycle
            | '[' -> Some Jogger
            | _   -> None

    //let getSymbolCode symbol =
    //    match symbol with
    //    | '-' -> Some House
    //    | 'b' -> Some Bicycle
    //    | 'O' -> Some Balloon
    //    | 'h' -> Some Hospital
    //    | 'j' -> Some Jeep
    //    | 'k' -> Some Truck
    //    | '<' -> Some Motorcycle
    //    | '[' -> Some Jogger
    //    | _   -> None

    //9 byte field
    type CallSign = private CallSign of string          
    module CallSign =
        open System

        let create (s:string) = 
            match (s.Trim()) with
            | c when not (String.IsNullOrEmpty(c)) && c.Length < 10     -> Some (CallSign c)
            | _                                                         -> None // "Call Sign cannot be empty and must be 1 - 9 characters. See APRS 1.01."
        let value (CallSign s) = s.ToUpper() // MUST BE ALL CAPS        
    
    //5 APRS DATA IN THE AX.25 INFORMATION FIELD
    //APRS Data Type
    //Identifier
    //Every APRS packet contains an APRS Data Type Identifier (DTI). This
    //determines the format of the remainder of the data in the Information field, as
    //follows:
    //APRS Data Type Identifiers
    type APRSDataTypeIdentifier =
        | PositionReportWithoutTimeStampNoMessaging //!
        | PositionReportWithoutTimeStampWithMessaging //=
        | PositionReportWithTimeStampWithMessaging //@ //TODO not supported
        | PositionReportWithTimeStampNoMessaging // / //TODO not supported
        | UserDefined // {
        | Message // :
        override this.ToString() =
            match this with
            | PositionReportWithoutTimeStampNoMessaging     -> "!"
            | PositionReportWithoutTimeStampWithMessaging   -> "="
            | PositionReportWithTimeStampWithMessaging      -> "@"
            | PositionReportWithTimeStampNoMessaging        -> "/"
            | UserDefined                                   -> "{"
            | Message                                       -> ":"
            
    
    let getAPRSDataTypeIdentifier id =
        match id with
        | "!"   -> Some PositionReportWithoutTimeStampNoMessaging
        | "="   -> Some PositionReportWithoutTimeStampWithMessaging
        | "@"   -> Some PositionReportWithTimeStampWithMessaging
        | "/"   -> Some PositionReportWithTimeStampNoMessaging
        | "{"   -> Some UserDefined
        | ":"   -> Some Message
        | _     -> None
