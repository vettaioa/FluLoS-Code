using Evaluation.Model;
using FuzzySearching;
using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Evaluation
{
    class AirspaceSearch
    {
        private readonly IEnumerable<RadarAirplane> radarAirplanes;

        private SearchableDictionary<RadarAirplane> byCallSign;
        private SearchableDictionary<RadarAirplane> byFlightNumber;

        public AirspaceSearch(FuzzySearch fuzzySearch, IEnumerable<KnownAirline> knownAirlines, IEnumerable<RadarAirplane> radarAirplanes)
        {
            this.radarAirplanes = radarAirplanes;

            InitFuzzySearchCollections(fuzzySearch, knownAirlines);
        }

        public RadarAirplane FindByCallSign(CallSign callSign)
        {
            string fuzzyCallsign = callSign.Airline + " " +  callSign.FlightNumber;
            
            RadarAirplane foundPlane = byCallSign.fuzzySearching(fuzzyCallsign, 0.7f);

            if(foundPlane == null)
            {
                // try to find by flightnumber only
                foundPlane = byFlightNumber.fuzzySearching(callSign.FlightNumber, 0.7f);
            }

            return foundPlane;
        }

        private void InitFuzzySearchCollections(FuzzySearch fuzzySearch, IEnumerable<KnownAirline> knownAirlines)
        {
            // Airspace to FuzzySearch
            var fuzzyCallsigns = new Dictionary<string, RadarAirplane>();
            var flightNumbers = new Dictionary<string, RadarAirplane>();
            foreach (var radarAirplane in radarAirplanes)
            {
                string airlineCallsign = FindCallSignForRadarAirplane(knownAirlines, radarAirplane);
                string flightNumber = radarAirplane?.Airplane?.Flight?.GetFlightNumber()?.ToLower();

                if (!string.IsNullOrEmpty(airlineCallsign) && !string.IsNullOrEmpty(flightNumber))
                {
                    string fuzzyCallsign = airlineCallsign + " " + flightNumber;
                    fuzzyCallsigns.Add(fuzzyCallsign, radarAirplane);
                }
                if (!string.IsNullOrEmpty(flightNumber))
                {
                    if (!flightNumbers.ContainsKey(flightNumber))
                    {
                        flightNumbers.Add(flightNumber, radarAirplane);
                    }
                    else
                    {
                        flightNumbers[flightNumber] = null; // set to null, because flightnumber is not unique
                    }
                }
            }

            byCallSign = new SearchableDictionary<RadarAirplane>(fuzzySearch, fuzzyCallsigns);
            byFlightNumber = new SearchableDictionary<RadarAirplane>(fuzzySearch, flightNumbers);
        }

        private static string FindCallSignForRadarAirplane(IEnumerable<KnownAirline> knownAirlines, RadarAirplane radarAirplane)
        {
            if(radarAirplane.Airplane?.Flight != null)
            {
                string iata = radarAirplane.Airplane.Flight.Airline?.AirlineIATA?.ToLower();
                string icao = radarAirplane.Airplane.Flight.Airline?.AirlineICAO?.ToLower();

                string identification = radarAirplane.Airplane.Flight.FlightIdentification?.ToLower();
                string airlineCode = Regex.Match(identification, "^([a-zA-Z]{2,3})").Value.ToLower();

                // replace unkown iata or icao code if flight identification contains iata or icao code
                if(iata == null && airlineCode.Length == 2)
                {
                    iata = airlineCode;
                }
                else if (icao == null && airlineCode.Length == 3)
                {
                    icao = airlineCode;
                }

                var foundAirlines = knownAirlines.Where(ka => ka.Iata == iata || ka.Icao == icao);

                if (foundAirlines.Count() == 0)
                {
                    // no airline found matching the criterias => using callsign from radar
                    string callSign = radarAirplane.Airplane.Flight.Airline?.Callsign?.ToLower();

                    if (callSign != null && callSign.Length > 1)
                    {
#if DEBUG
                        Console.WriteLine($"[DEBUG] Taking CallSign {callSign} from Radar");
#endif
                        return callSign;
                    }
                    return null; // no airline callsign found
                }
                else if (foundAirlines.Count() > 1)
                {
                    var foundAirlinesExact = foundAirlines.Where(ka => ka.Iata == iata && ka.Icao == icao);

                    if(foundAirlinesExact.Count() > 0)
                    {
                        foundAirlines = foundAirlinesExact;
                    }
                }
                
                KnownAirline foundAirline = foundAirlines.First();
                if(foundAirline.CallSign == null)
                {
                    return foundAirline.Name; // found airline has no known callsign
                }
                else
                {
                    return foundAirline.CallSign;
                }
            }

            
#if DEBUG
            Console.WriteLine($"[DEBUG] Couldn't find Airline CallSign for {radarAirplane?.Airplane?.Flight?.FlightIdentification}");
#endif
            return null; // no matching airline found
        }

    }
}
