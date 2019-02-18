open System.IO
open System

//SHOULD I USE ACTIVE PATTERNS??

let start = "[0] KG7SIO-7>APRD15,WIDE1-1:=3216.4N'11057.3Wbblah:blah /fishcakes"
//let start = ""
//let start = "[0] "
//let start = "AB CD:ABC DABCD]"

let framepos = start.IndexOf(" ")
//let split = start.Split(' ')
let frame = start.Substring(framepos) //split.[1]
let msgPos = frame.IndexOf(":")
let msg = frame.Substring(msgPos + 1) //frame.Split(':').[1]
let rpt =
    if msg.StartsWith("=") || msg.StartsWith("!") || msg.StartsWith("@") then 
        let lat = msg.Split('/').[0].Remove(0,1)

        let npos = lat.IndexOf('N')
        let spos = lat.IndexOf('S')

        let latCheck =
            match npos, spos with
            | -1, -1 -> failwith "Latitude in the wrong format"
            | n, s when n = -1 -> ()
            | n, s when s = -1 -> ()
            | _, _ -> failwith "Latitude in the wrong format"

        let tempLon = msg.Split('/').[1]
        printf "temp Longitude %s" tempLon
        
        //Get the first occurrent of W or E. Take the position that is the smallest
        let wpos = tempLon.IndexOf('W')
        let epos = tempLon.IndexOf('E')
        
        printf "wpos %i %s" wpos Environment.NewLine
        printf "epos %i %s" epos Environment.NewLine

        let lonEnd = 
            match wpos, epos with
            | -1, -1 -> failwith "Longitude in the wrong format"
            | w, e when e = -1 -> w
            | w, e when w = -1 -> e
            | w, e when w < e -> w
            | w, e when e < w -> e
            | _, _ -> failwith "Longitude in the wrong format"

        printf "lonEnd %i %s" lonEnd Environment.NewLine
        let lon = tempLon.Remove(lonEnd + 1)

        //get the character right after the longitude
        let sym = tempLon.Substring(lonEnd + 1, 1)

        let cpos = msg.IndexOf(sym)
        let comment = msg.Substring(cpos + 1)

        Some (lat , lon , sym, comment)
    else
        None
    
[<Literal>]
let BAD_ADDRESS = "KG7SIO-7+APRD15-WIDE1-CPXX*,qAX,CWOP-2"
//",KG7SIO-7>APRD15-WIDE1-CPXX*,qAX,CWOP-2"
//"KG7SIO-7+APRD15-WIDE1-CPXX*,qAX,CWOP-2"
let GOOD_ADDRESS = "KG7SIO-7>APRD15,WIDE1-1,TCPXX*,qAX,CWOP-2"

//let addrsa = frame.Split(':').[0]

let bad_path =
    if not (BAD_ADDRESS.IndexOf(">") = -1) && BAD_ADDRESS.IndexOf(",") > BAD_ADDRESS.IndexOf(">") then
        Some (BAD_ADDRESS.Substring(BAD_ADDRESS.IndexOf(",") + 1).Split(','))
    else
        None

let good_path =
    if not (GOOD_ADDRESS.IndexOf(">") = -1) && GOOD_ADDRESS.IndexOf(",") > GOOD_ADDRESS.IndexOf(">") then
        Some (GOOD_ADDRESS.Substring(GOOD_ADDRESS.IndexOf(",") + 1).Split(','))
    else
        None

