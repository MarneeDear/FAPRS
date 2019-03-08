# About FAPRS.CRC

This is a port of [https://github.com/meetanthony/crccsharp](crccsharp), from meetanthony, on Github. 

It is also referenced on [Online CRC Calculator](https://crccalc.com/), which I used to validate the CRC calculation results from faprs.crc.

# Changes

* Made `Crc.ComputeCrc` public so I could use it outside that class.
* Made `Crc._table` public so I could print out the table for troubleshooting.

# Important Notes

faprs.crc is used to calculate the Frame Check Sequence in AX.25.

The only CRC calculation algorithm that I could get to work consistently was `MCRF4XX`, which works for me because that is what I need for AX.25

# CRC References

[PPP in HDLC Framing](https://www.ietf.org/rfc/rfc1549.txt)

[Online CRC Calculator](https://crccalc.com/)

[Cyclic Redundancy Check](https://en.wikipedia.org/wiki/Cyclic_redundancy_check#Polynomial_representations_of_cyclic_redundancy_checks)