# Data for Evaluation of Labal-Data

<!--

"iata":"kl", "icao":"klm", "name":"klm", "callSign":"klm"
"iata":"af", "icao":"afr", "name":"air france", "callSign":"airfrans"
iata: sn, icao: sab, name & callsign: sabena
"iata":"lh", "icao":"dlh", "name":"lufthansa", "callSign":"lufthansa" },
iata: YP, icao: AEF, name & callsign: AERO LLOYD
IATA: FV, ICAO: VIV, name: viva air, callsign: VIVA
iata: OA, ICAO: OAL, name & callsign: OLYMPIC


"Flight": {
    "ActualDepartureTime": null,
    "Airline": null,
    "ArrivalAirport": null,
    "DepartureAirport": null,
    "FlightIdentification": "VLG1873"
},
-->


## Airplanes
- klm 376
- airfrans 3261
- sabena 826A
- lufthansa 4652
- aero lloyd 517
- klm 264
- viva 9081
- lufthansa 3556

for callsign improvement:
- lufthansa 5905
- olympic 6474
- sabena 158
- saudia 158
- speedbird 566
- speedbird 56


## Callsign Improvement
- sm2_09_031.wav (InvalidFlightNr) lufthansa 5305 squawk 2756 -> sollte zu lufthansa 5905 verbessert werden
- sm2_09_028.wav (InvalidAirline) alitalia 6474 squawk 2762 -> sollte zu olympic 6474 verbessert werden
- gf1_01_036.wav (InvalidFlightNr) saudia 155 bonjour squawk 5743 -> sollte zu saudia 158 verbessert werden, obwohl es auch sabena 158 gibt
- gm1_02_011.json (InvalidFlightNr) speedbird 563 bonjour squawk 5721 -> sollte zu speedbird 56 verbessert werden, obwohl es auch speedbird 566 gibt
- gm1_01_164.json (Invalid) portugalia 543 climb to flight level 350 -> sollte nicht korrigiert werden, da es nicht im Flugraum ist


## Squawk
- gm1_01_071.wav (Valid) klm 376 squawk 5766
- zf1_01_009.wav (Invalid) airfrans 3261 squawk now 2724
<!--
- zf1_04_079.wav airfrans ah 356 good morning ah flight correction squawk 7536
- zf1_08_067.wav airfrans 356 good morning squawk 7536
- sm2_09_031.wav lufthansa 5305 squawk 2756
-->


## Contact
- gm2_01_156.wav (Valid) lufthansa 4652 contact marseille 125.85
- gm1_02_087.wav (InvalidFrequency) aero lloyd 517 contact zurich on 134134.6 good bye
- gm2_02_099.wav (InvalidPlace) airfrans 3261 affirm in contact ah geneva 125.85 au revoir



## Flight Level
- gm1_01_092.wav (Valid) klm 376 climb to flight level 330
- zf1_08_102.wav (InvalidInstruction) sabena 826A climb to flight level 320 trasadingen karlsruhe
<!--
- gm1_01_079.wav klm 376 you're identified cleared st prex arbos epinal climb to flight level 320
- gf1_01_068.wav aero lloyd 560 bonjour identified cleared passeiry bilsa flight level 330
-->


## Turn
- sm1_02_105.wav (Valid) klm 264 turn left to ntm
- sm1_06_149.wav (ValidDegrees, ValidDirection, InvalidPlace, InvalidHeading) sabena 826A turn left by 10 degrees
- gf1_01_064.wav (ValidHeading, InvalidDirection, InvalidDegrees, InvalidPlace, ) viva 9081 turn left heading 350
- sm1_02_150.wav (InvalidAll) sabena 801 turn right to trasadingen
- sm1_06_084.wav (ValidPlace, ValidDirection, InvalidHeading, InvalidDegrees) sabena 7816 turn left to nattenheim
<!--
- sm1_04_005.wav sabena 481 turn left to dinkelsbuhl
- sm1_05_149.wav sabena 801 right turn to trasadingen
-->


## Multi
- sm1_03_118.wav (ValidDegrees, ValidDirection, ValidClimbInstruction, ValidFlightLevel) lufthansa 3556 rhein radar identified turn right by 15 degrees and climb flight level 290

