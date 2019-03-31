# F# for APRS
###### A system for sending and receiving APRS messages integrated with DireWolf, and built on .NET Core in F#. 

_This is also my submission for the [Applied F# Challenge](http://foundation.fsharp.org/applied_fsharp_challenge) - F# in your organization or domain category._

## Applied F# Challenge

#### Author

Marnee Dearman (KG7SIO)

#### Domain

Amateur radio communications protocols, specifically the very popular packet radio protocol, APRS

#### Purpose

* Demonstrate the power of F# in domain modelling that makes modelling protocols clear TODO
Show how great F# is as domain modelling, making it a wonderful choice for developing for protocols -- functional domain modelling for communications protocols
* Provide a cross-platform, simple, easy to use application for sending and receiving APRS messages.
* Provide an automated way to track race participants during amateur radio supported long-distance races (e.g, 24 Hours in the Old Pueblo, C/V 50/50). 
* Provide a framework for developing custom APRS applications.
* Get more people involved in amateur radio

The code uses a lot of functional programming techniques and F# libraries to get things done, but here are the highlights:

* [Designing with types](https://fsharpforfunandprofit.com/posts/designing-with-types-intro/)
  * Single case unions
  * Constrained strings
  * Making impossible states impossible
* [Active patterns]() for string parsing and validation
* [Argu]() for command line parsing
* [Expecto]() for testing
* [Saturn]() for building the web app/web API/web service


## Amateur Radio and APRS

### What is amateur radio?

What Wikipedia thinks it is:

> Amateur radio, also known as ham radio, describes the use of radio frequency spectrum for purposes of non-commercial exchange of messages, wireless experimentation, self-training, private recreation, radiosport, contesting, and emergency communication

What the [Radio Society of Great Britain]() thinks it is (great video introduction):

> [![Packet Radio](https://img.youtube.com/vi/8x6x_6mDVlQ/0.jpg)](https://youtu.be/8x6x_6mDVlQ "Ham radio is awesome.")

> _click the video to watch on You Tube_

### What is APRS?

What Wikipedia thinks it is:

> *Automatic Packet Reporting System* is an amateur radio-based system for real time digital communications of information of immediate value in the local area. Data can include object Global Positioning System coordinates, weather station telemetry, text messages, announcements, queries, and other telemetry.

What I think it is:

> APRS is an amateur radio protocol that is really cool and awesome and makes me look like an real Engineer.

The APRS protocol was developed by [TAPR (Tucson Amateur Packet Radio)](https://www.tapr.org/aprs_information.html). 

The specification versions:
* [APRS v1.0](http://www.aprs.org/doc/APRS101.PDF)
* [APRS v1.1](http://www.aprs.org/aprs11.html)
* [APRS v1.2](http://www.aprs.org/aprs12.html)

This video is a nice demonstration of what you can do with APRS. The system demonstrated in the video is similar to my F# for APRS system design.

> [![Packet Radio](https://img.youtube.com/vi/FJEVWMuz6Xg/0.jpg)](http://www.youtube.com/watch?v=FJEVWMuz6Xg "Kantronics Packet Radio Mail and BBS Operations")

> _click the video to watch on You Tube_




