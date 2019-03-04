namespace faprs.core

//TODO build this out to make it possible to send directly to a TNC instead of using Direwolf kissutil

(*

CRC16 or CRC32
Cyclic redundancy check
https://en.wikipedia.org/wiki/Cyclic_redundancy_check
    A cyclic redundancy check (CRC) is an error-detecting code 
    commonly used in digital networks and storage devices to detect 
    accidental changes to raw data. Blocks of data entering these 
    systems get a short check value attached, based on the remainder 
    of a polynomial division of their contents.

POLYNOMIAL REPRESENTATIONS
https://en.wikipedia.org/wiki/Cyclic_redundancy_check#Polynomial_representations_of_cyclic_redundancy_checks

Previous art:

The DireWolf implementation using table
https://github.com/wb2osz/direwolf/blob/e51002ac0c60617582f26ddc363830edef1c4e81/fcs_calc.c#L76

F# implementation using polynomial calculation
http://fssnip.net/8z/title/CRC16
https://stackoverflow.com/questions/7383781/c-sharp-to-f-crc16-and-works-different-how-works

PPP in HDLC Framing
https://www.ietf.org/rfc/rfc1549.txt
uses a table
Page 13

the FCS was designed so that a particular pattern results
   when the FCS operation passes over the complemented FCS.  A good
   frame is indicated by this "good FCS" value.

A.1 FCS Computation Method

   The following code provides a table lookup computation for
   calculating the Frame Check Sequence as data arrives at the
   interface.  This implementation is based on [9], [10], and [11].  The
   table is created by the code in section B.2.

   **************************

#define AX25_MAX_ADDRS      10	    /* Destination, Source, 8 digipeaters. */
#define AX25_MAX_INFO_LEN   2048	/* Maximum size for APRS. */
				                    /* AX.25 starts out with 256 as the default max */
				                    /* length but the end stations can negotiate */
				                    /* something different. */
				                    /* version 0.8:  Change from 256 to 2028 to */
				                    /* handle the larger paclen for Linux AX25. */

				                    /* These don't include the 2 bytes for the */
				                    /* HDLC frame FCS. */
#define MAX_FRAME_LEN ((AX25_MAX_PACKET_LEN) + 2)	
#define MIN_FRAME_LEN ((AX25_MIN_PACKET_LEN) + 2)
#define AX25_MAX_PACKET_LEN ( AX25_MAX_ADDRS * 7 + 2 + 3 + AX25_MAX_INFO_LEN)
                                10 * 7 + 2 + 3 + 2048



*)
module FrameCheckSequence =
    (*
     * FCS lookup table as calculated by the table generator in section B.2
     *)
    let fcstab = [| 
        0x0000us; 0x1189us; 0x2312us; 0x329bus; 0x4624us; 0x57adus; 0x6536us; 0x74bfus;
        0x8c48us; 0x9dc1us; 0xaf5aus; 0xbed3us; 0xca6cus; 0xdbe5us; 0xe97eus; 0xf8f7us;
        0x1081us; 0x0108us; 0x3393us; 0x221aus; 0x56a5us; 0x472cus; 0x75b7us; 0x643eus;
        0x9cc9us; 0x8d40us; 0xbfdbus; 0xae52us; 0xdaedus; 0xcb64us; 0xf9ffus; 0xe876us;
        0x2102us; 0x308bus; 0x0210us; 0x1399us; 0x6726us; 0x76afus; 0x4434us; 0x55bdus;
        0xad4aus; 0xbcc3us; 0x8e58us; 0x9fd1us; 0xeb6eus; 0xfae7us; 0xc87cus; 0xd9f5us;
        0x3183us; 0x200aus; 0x1291us; 0x0318us; 0x77a7us; 0x662eus; 0x54b5us; 0x453cus;
        0xbdcbus; 0xac42us; 0x9ed9us; 0x8f50us; 0xfbefus; 0xea66us; 0xd8fdus; 0xc974us;
        0x4204us; 0x538dus; 0x6116us; 0x709fus; 0x0420us; 0x15a9us; 0x2732us; 0x36bbus;
        0xce4cus; 0xdfc5us; 0xed5eus; 0xfcd7us; 0x8868us; 0x99e1us; 0xab7aus; 0xbaf3us;
        0x5285us; 0x430cus; 0x7197us; 0x601eus; 0x14a1us; 0x0528us; 0x37b3us; 0x263aus;
        0xdecdus; 0xcf44us; 0xfddfus; 0xec56us; 0x98e9us; 0x8960us; 0xbbfbus; 0xaa72us;
        0x6306us; 0x728fus; 0x4014us; 0x519dus; 0x2522us; 0x34abus; 0x0630us; 0x17b9us;
        0xef4eus; 0xfec7us; 0xcc5cus; 0xddd5us; 0xa96aus; 0xb8e3us; 0x8a78us; 0x9bf1us;
        0x7387us; 0x620eus; 0x5095us; 0x411cus; 0x35a3us; 0x242aus; 0x16b1us; 0x0738us;
        0xffcfus; 0xee46us; 0xdcddus; 0xcd54us; 0xb9ebus; 0xa862us; 0x9af9us; 0x8b70us;
        0x8408us; 0x9581us; 0xa71aus; 0xb693us; 0xc22cus; 0xd3a5us; 0xe13eus; 0xf0b7us;
        0x0840us; 0x19c9us; 0x2b52us; 0x3adbus; 0x4e64us; 0x5fedus; 0x6d76us; 0x7cffus;
        0x9489us; 0x8500us; 0xb79bus; 0xa612us; 0xd2adus; 0xc324us; 0xf1bfus; 0xe036us;
        0x18c1us; 0x0948us; 0x3bd3us; 0x2a5aus; 0x5ee5us; 0x4f6cus; 0x7df7us; 0x6c7eus;
        0xa50aus; 0xb483us; 0x8618us; 0x9791us; 0xe32eus; 0xf2a7us; 0xc03cus; 0xd1b5us;
        0x2942us; 0x38cbus; 0x0a50us; 0x1bd9us; 0x6f66us; 0x7eefus; 0x4c74us; 0x5dfdus;
        0xb58bus; 0xa402us; 0x9699us; 0x8710us; 0xf3afus; 0xe226us; 0xd0bdus; 0xc134us;
        0x39c3us; 0x284aus; 0x1ad1us; 0x0b58us; 0x7fe7us; 0x6e6eus; 0x5cf5us; 0x4d7cus;
        0xc60cus; 0xd785us; 0xe51eus; 0xf497us; 0x8028us; 0x91a1us; 0xa33aus; 0xb2b3us;
        0x4a44us; 0x5bcdus; 0x6956us; 0x78dfus; 0x0c60us; 0x1de9us; 0x2f72us; 0x3efbus;
        0xd68dus; 0xc704us; 0xf59fus; 0xe416us; 0x90a9us; 0x8120us; 0xb3bbus; 0xa232us;
        0x5ac5us; 0x4b4cus; 0x79d7us; 0x685eus; 0x1ce1us; 0x0d68us; 0x3ff3us; 0x2e7aus;
        0xe70eus; 0xf687us; 0xc41cus; 0xd595us; 0xa12aus; 0xb0a3us; 0x8238us; 0x93b1us;
        0x6b46us; 0x7acfus; 0x4854us; 0x59ddus; 0x2d62us; 0x3cebus; 0x0e70us; 0x1ff9us;
        0xf78fus; 0xe606us; 0xd49dus; 0xc514us; 0xb1abus; 0xa022us; 0x92b9us; 0x8330us;
        0x7bc7us; 0x6a4eus; 0x58d5us; 0x495cus; 0x3de3us; 0x2c6aus; 0x1ef1us; 0x0f78us
        |]

    [<Literal>]
    let PPPINITFCS16    = 0xffffus  //Initial FCS value
    [<Literal>]
    let P               = 0x8408us //The HDLC polynomial: x**0 + x**5 + x**12 + x**16 (0x8408).
    //[<Literal>]
    //let PPPGOODFCS16 = 0xf0b8us  //Good final FCS value

    (*
        DIREWOLF IMPLEMENTATION
        unsigned short fcs_calc (unsigned char *data, int len)
        {
	        unsigned short crc = 0xffff;
	        int j;

	        for (j=0; j<len; j++) {

  	          crc = ((crc) >> 8) ^ ccitt_table[((crc) ^ data[j]) & 0xff];
	        }

	        return ( crc ^ 0xffff );
        }    
    *)
    let CRC_16_with_table msg =
        let mutable code    = PPPINITFCS16  //Initial FCS value
        for b in msg do
            code <- (code >>> 8) ^^^ fcstab.[int ((code ^^^ b) &&& 0xffus)]
        code ^^^ PPPINITFCS16

        //PPPINITFCS16 :: msg
        //|> Seq.fold (fun acc elem -> ) //Can I use fold here?

    let CRC_16_calculated_polynomial msg =
        let polynomial      = P             //The HDLC CRC-16-CCITT (CRC-CCITT) revered polynomial representation 
        let mutable code    = PPPINITFCS16  //Initial FCS value
        for b in msg do
            //code <- code ^^^ uint16 b
            code <- code ^^^ b
            for j in [0..7] do
                if (code &&& 1us <> 0us) then
                    code <- (code >>> 1) ^^^ polynomial
                else
                    code <- code >>> 1
        code



(*

https://en.wikipedia.org/wiki/AX.25
https://tapr.org/pub_ax25.html

AX.25.2.2.pdf
3. Frame Structure
Link layer packet radio transmissions are sent in small blocks of data, called frames.

Flag        Address         Control     PID     Info        FCS     Flag
01111110    112/224 Bits    8/16 Bits   8 Bits  N*8 Bits 1  6 Bits  01111110
0x7E                                                                0x7E

APR1.01 
3 APRS AND AX.25

The AX.25 Frame All APRS transmissions use AX.25 UI-frames with 9 fields of data:
AX.25 UI-FRAME FORMAT


*)
module AX25 =
    open System

    let FLAG = (BitConverter.GetBytes(0x7EB))

    type Packet =
        {
            //Flag : int8
            Address : byte[]
            Control : byte[]
            ProtocolIdentifier : byte[]
            Information: byte[]
            FrameCheckSequence: byte[]
        }
        member this.ToByteArray() =
            //TODO teh byte arrays need to be ordered in LSB and MSB according to the spec
            //AX25.2.2.pdf All fields except the Frame Check Sequence (FCS) are transmitted low-order bit first. 
            //FCS is transmitted bit 15 first.
            Array.empty
            |> Array.append FLAG
            |> Array.append this.Address
            |> Array.append this.Control
            |> Array.append this.ProtocolIdentifier
            |> Array.append this.Information
            |> Array.append this.FrameCheckSequence
            |> Array.append FLAG

