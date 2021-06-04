# LUIS App Exports

These exports can be imported to get the LUIS application used in this work.
This can be achieved in the applications overview at https://www.luis.ai/applications.
After importing, the model must me manually trained and published in LUIS.

## Versions
- **0.1**: Intents are trained with machine learned entities and subentities.
- **0.2-features**: Like 0.1 but intents have machine learned entities as ML-Features.
- **0.2-entities**: Like 0.1 but with additional (separate) list and regex entities to help identify intents.
- **0.3**: Combination of 0.2-features and 0.2-entities.
- **0.4**: The list and regex entities are assigned to machine learned entities as ML-Features.

## Definition of Version 0.4

### Intents

| Intent | Features | Utterances |
|-|-|-|
| Contact | MLContact | - olympic 072 contact nattenheim 135.4<br>- alitalia 5f contact milano 12585 bye<br>- cross air 269w contact marseille on 125.85 good bye<br>- sabena 4ad contact rhein frequency 132.4 tschuss<br>- air berlin 5081 geneva 13315<br>- air hamburg 2546 marseille<br>- alitalia 401 zurich 134.6<br>- french air force 4234 reims 134.4 good bye<br>- hapag lloyd 355 zurich frequency 1337 tschuss<br>- swissair 9360 contact rhein radar 127.37 |
| FlightLevel | MLFlightLevel | - swiss 806 identified climb now flight level 340<br>- alitalia 851c buongiorno maintain level 320<br>- olympic 087 identified maintain flight level 350<br>- aegean 87h descend now level 300<br>- iberia 3473 bonjour climb to flight level 390<br>- cross air 512 bonjour cleared kines st prex fribourg flight level 310<br>- general motors 4209 continue descent to level 340<br>- lufthansa 4416 climb flight level 290<br>- luxair 2663 bonjour continue climb to flight level 350<br>- united 69 descend to flight level 290<br>- aero lloyd 517 good morning maintain flight level 320<br>- airfrans 3261 climb to flight level 340<br>- bonjour swissair 687 maintain flight level 280 |
| None |  | - app developers drink coffee<br>- music is categorized into genres<br>- trampolines are mostly played with by kids<br>- penguins are animals<br>- trains on tracks |
| Squawk | MLSquawk | - good evening olympic 2d squawk 6751<br>- alitalia 210z squawk 4673<br>- swiss 213k good evening squawk 8315<br>- good afternoon sabena 63w squawk 1499<br>- airfrans 016 bonjour squawk 6784<br>- bonjour alitalia 228 squawk 5772<br>- air malta 104 good afternoon squawk 5730<br>- lufthansa 4356 good morning squawk 2712<br>- good morning adria 1642 squawk 2710 |
| Turn | MLTurn | - lufthansa 6b turn now left to dkb<br>- air portugal 41ac turn right 10 degrees<br>- aero lloyd 560 further left by 10 degrees<br>- alitalia 292 turn right to nattenheim<br>- ltu 1602 right by 20 degrees please<br>- hapag lloyd 171 turn left to bilsa<br>- jet set 207 left direct hochwald<br>- french lines 9165 fly heading 095<br>- lufthansa 5504 turn right heading 200<br>- olympic 144 turn left heading 115 for separation |




### Entities

**Machine Learned**
| Entity        | Sub-Entity                                                 | Feature                     |
|---------------|------------------------------------------------------------|-----------------------------|
| MLCallSign    |                                                            |                             |
|&emsp;├──      | Model class library used by multiple solutions (C# .NET 5) | ListAirlines                |
|&emsp;└──      | FlightNr                                                   |                             |
| MLContact     |                                                            | ListContactIdentifiers      |
|&emsp;├──      | Frequency                                                  | RegexContactFrequency       |
|&emsp;└──      | Place                                                      | geographyV2                 |
| MLFlightLevel |                                                            |                             |
|&emsp;├──      | Level                                                      | RegexFlightLevel            |
|&emsp;└──      | Instruction                                                | ListFlightLevelInstructions |
| MLSquawk      |                                                            |                             |
|&emsp;└──      | Code                                                       | RegexSquawkNr               |
| MLTurn        |                                                            |                             |
|&emsp;├──      | Direction                                                  | ListTurnDirections          |
|&emsp;├──      | Degrees                                                    | RegexTurnDegrees            |
|&emsp; \|      |&emsp; └── Value                                             |                             |
|&emsp;├──      | Heading                                                    | RegexTurnHeading            |
|&emsp; \|      |&emsp; └── Value                                             |                             |
|&emsp;└──      | Place                                                      | RegexTurnPlace              |
|               |&emsp; └── Value                                             |                             |




**Regex**
- RegexContactFrequency: (\d){3}(\.)?(\d){2}
- RegexFlightLevel: (flight )?level (\d+)
- RegexSquawkNr: squawk \d{4}
- RegexTurnDegrees: \d{2} degrees
- RegexTurnHeading: heading \d{3}
- RegexTurnPlace: to (A-Za-z0-9)+



**Lists**
- ListAirlines: contains callsigns of airlines from folder ../airlines.json
- ListContactIdentifiers: contact, frequency
- ListFlightLevelInstructions: climb, descend (with descent as synonym), maintain
- ListTurnDirections: left, right



**Pre-Defined**
- geographyV2 (to find places i.e. in 'contact rhein')
