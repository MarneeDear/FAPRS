namespace faprs.core

(*
Input, starting with a lower case letter is interpreted as being a command. 
Whitespace, as shown in the examples, is optional.
letter meaning example
------- ----------- -----------
d txDelay, 10ms units d 30
p Persistence p 63
s Slot time, 10ms units s 10
t txTail, 10ms units t 5
f Full duplex f 0
h set Hardware h (hardware-specific)
*)
module KISS =

    type Command =
        | D
        | P
        | S
        | T
        | F
        | H        
        member this.ToChar() =
            match this with
            | D -> 'd'
            | P -> 'p'
            | S -> 's'
            | T -> 't'
            | F -> 'f'
            | H -> 'h'
     
    let getCommand c =
        match c with
        | 'd'   -> D
        | 'p'   -> P
        | 's'   -> S
        | 't'   -> T
        | 'f'   -> F
        | 'h'   -> H
        | _     -> failwith "Unknown KISS command character."