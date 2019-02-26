namespace faprs.core

[<AutoOpen>]
module Common =
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
        | '-' -> House
        | 'b' -> Bicycle
        | 'O' -> Balloon
        | 'h' -> Hospital
        | 'j' -> Jeep
        | 'k' -> Truck
        | '<' -> Motorcycle
        | '[' -> Jogger
        | _   -> House

    //9 byte field
    type CallSign = private CallSign of string          
    module CallSign =
        let create (s:string) = 
            match (s.Trim()) with
            | s when s.Length > 0 && s.Length < 10  -> CallSign s
            | _                                     -> failwith "Call Sign cannot be empty and must be 1 - 9 characters. See APRS 1.01."
        let value (CallSign s) = s.ToUpper() // MUST BE ALL CAPS        
        
