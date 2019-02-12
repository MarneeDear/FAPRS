namespace faprs.domain

[<AutoOpen>]
module Common =
//        https://stackoverflow.com/questions/791706/how-do-i-customize-output-of-a-custom-type-using-printf
    //9 byte field
    //TODO get string rep. of all path types
    type Path =
        | WIDEnN    //of string
        //| RELAY     //Obsolete?
        | ECHO      
        //| WIDE      //Obsolete?
        //| TRACE     //Obsolete?
        | GATE      
        override this.ToString() =
            match this with
            | WIDEnN  _  -> "WIDE1-1"
            | _         -> failwith "Path type not found"

    //This is only a subset of the codes because I don't want to support all of them
    type SymbolCode =
        | House 
        | Bicycle 
        | Balloon 
        | Hospital
        | Jeep 
        | Truck
        | Motocycle
        | Jogger
        member this.ToChar() =
            match this with
            | House     -> '-'
            | Bicycle   -> 'b'
            | Balloon   -> 'O'
            | Hospital  -> 'h'
            | Jeep      -> 'j'
            | Truck     -> 'k'
            | Motocycle -> '<'
            | Jogger    -> '['

    //9 byte field
    //TODO limit to 9 bytes instead of 6 chars. Some senders use an SSID
    type CallSign = private CallSign of string          
    module CallSign =
        let create (s:string) = 
            match (s.Trim()) with
            | s when s.Length > 0 && s.Length < 7   -> CallSign s
            | _                                     -> failwith "Call Sign cannot be empty and must be less than 7 characters long."
        let value (CallSign s) = s.ToUpper() // MUST BE ALL CAPS        
        
