namespace faprs.core

[<AutoOpen>]
module Common =
    open System.Net.Http

//        https://stackoverflow.com/questions/791706/how-do-i-customize-output-of-a-custom-type-using-printf
    //9 byte field
    //TODO get string rep. of all path types
    //aka the UNPROTO path
    //The digipeater path to the destination callsign
    type Path =
        | WIDEnN    //of string
        //| RELAY     //Obsolete?
        | ECHO      
        //| WIDE      //Obsolete?
        //| TRACE     //Obsolete?
        | GATE      
        override this.ToString() =
            match this with
            | WIDEnN  _ -> "WIDE1-1"
            | _         -> failwith "Path type not found"

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
            | House     -> '-'
            | Bicycle   -> 'b'
            | Balloon   -> 'O'
            | Hospital  -> 'h'
            | Jeep      -> 'j'
            | Truck     -> 'k'
            | Motorcycle -> '<'
            | Jogger    -> '['

    let getSymbolCode symbol =
        match symbol with
        | '-' -> Some House
        | 'b' -> Some Bicycle
        | 'O' -> Some Balloon
        | 'h' -> Some Hospital
        | 'j' -> Some Jeep
        | 'k' -> Some Truck
        | '<' -> Some Motorcycle
        | '[' -> Some Jogger
        | _   -> None

    //9 byte field
    type CallSign = private CallSign of string          
    module CallSign =
        let create (s:string) = 
            match (s.Trim()) with
            | s when s.Length > 0 && s.Length < 10  -> CallSign s
            | _                                     -> failwith "Call Sign cannot be empty and must be 1 - 9 characters. See APRS 1.01."
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
