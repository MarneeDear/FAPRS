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
* [Active patterns](https://fsharpforfunandprofit.com/posts/convenience-active-patterns/) for string parsing and validation
* [Argu](http://fsprojects.github.io/Argu/) for command line parsing
* [Expecto](https://github.com/haf/expecto) for testing
* [Saturn](https://saturnframework.org/) for building the web app/web API/web service


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

The APRS protocol was developed by [TAPR (Tucson Amateur Packet Radio)](https://www.tapr.org/aprs_information.html). 

The specification versions:
* [APRS v1.0](http://www.aprs.org/doc/APRS101.PDF)
* [APRS v1.1](http://www.aprs.org/aprs11.html)
* [APRS v1.2](http://www.aprs.org/aprs12.html)

This video is a nice demonstration of what you can do with APRS. The system demonstrated in the video is similar to my F# for APRS system design.

> [![Packet Radio](https://img.youtube.com/vi/FJEVWMuz6Xg/0.jpg)](http://www.youtube.com/watch?v=FJEVWMuz6Xg "Kantronics Packet Radio Mail and BBS Operations")

> _click the video to watch on You Tube_

## System Design

### Motivation, purpose, and use cases

Hams often volunteer for middle-of-nowhere long distances races. You know the kind? Where you 20 persons run 50 miles through the desert for fun? The participants need trail support and we Hams are well-equipped to run the communications system. We setup stations with mobile radios at known locations along the trail, make sure we can communicate with each other, and setup a process by which we share participant status and location, so we can keep track of them and request medical support if needed. It's great fun and many of these events wouldn't happen if it weren't for us Hams.

Most of the time we use voice to communicate. This works well enough, but I have worked enough of these races to know that it can be a challenge to keep track of all of the runners. You need a system, and everyone has their own way, of course -- some I like better than others. So, I came up with an idea, a solution, an even better way, maybe ... .

The best race I worked was the one where each station was announcing when a runner came through. Every station knew where 


### Physical Design

![Imgur](https://i.imgur.com/ECJd3WF.png)

I designed this system with a few things in mind:

* Inexpensive
* Mobile
* Possible to use with any radio
* It will be used in remote locations
* Low power

The Raspberry Pi provides these services





* Needs to store received messages and re-broadcast
* 