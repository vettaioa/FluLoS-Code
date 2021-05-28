# Evaluation Data Folder

Files to verify if evaulation works correctly and its output.


### Folder Structure
```
.
├── labelled/                 Manually labelled expected evaluation flags for the context results
├── mock-utterances/          Manually created utterances for the mocked airspace
├── validated-context/        Merged contexts containing only validated data
├── validation-flags/         Result of the validation
├── airlines.json             Existing airline codes with IATA, ICAO and callsign (source: https://en.wikipedia.org/wiki/List_of_airline_codes)
├── labeldata_airspace.json   Mocked airspace for the labeldata
├── mock_airspace.json        Mocked airspace for the manually created evaluation utterances
```
