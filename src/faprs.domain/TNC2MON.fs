namespace faprs.domain

open System

module TNC2MON = 

    type Packet = 
        {
            Sender : CallSign //9 bytes
            Destination : CallSign //9 bytes
            Path : Path //81 bytes, TODO this can be a list 
            Message : Message option
        }
        override this.ToString() =
            let message =
                match this.Message with
                | Some p    -> p.ToString()
                | None      -> String.Empty
            sprintf "%s>%s,%s:%s" (CallSign.value this.Sender) (CallSign.value this.Destination) (this.Path.ToString()) message