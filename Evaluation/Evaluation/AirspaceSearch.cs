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
        private readonly IEnumerable<KnownAirline> knownAirlines;
        private readonly IEnumerable<RadarAirplane> radarAirplanes;

        private SearchableDictionary<RadarAirplane> byAirline;
        private SearchableDictionary<RadarAirplane> byFlightnumber;

        public AirspaceSearch(FuzzySearch fuzzySearch, IEnumerable<KnownAirline> knownAirlines, IEnumerable<RadarAirplane> radarAirplanes)
        {
            this.knownAirlines = knownAirlines;
            this.radarAirplanes = radarAirplanes;

            InitFuzzySearchCollections(fuzzySearch);
        }

        private void InitFuzzySearchCollections(FuzzySearch fuzzySearch)
        {
            // Airspace to FuzzySearch
            var airlineNames = new Dictionary<string, RadarAirplane>();
            var flightNumbers = new Dictionary<string, RadarAirplane>();
            foreach (var radarAirplane in radarAirplanes)
            {
                string airlineName = radarAirplane?.Airplane?.Flight?.Airline?.Name;
                //string airlineIata = radarAirplane?.Airplane?.Flight?.Airline?.AirlineIATA;
                string flightNumber = radarAirplane?.Airplane?.Flight?.GetFlightNumber();

                if (airlineName != null)
                {
                    // TODO: airline to callsign airline (and maybe check both?)
                    // TODO: maybe extract "shortname" from flightIdentification, if airlineName is null
                    airlineNames.Add(airlineName, radarAirplane);
                }
                if (flightNumber != null)
                {
                    flightNumbers.Add(flightNumber, radarAirplane);
                }
            }

            byAirline = new SearchableDictionary<RadarAirplane>(fuzzySearch, airlineNames);
            byFlightnumber = new SearchableDictionary<RadarAirplane>(fuzzySearch, flightNumbers);
        }

        public RadarAirplane FindByCallSign(CallSign callSign)
        {
            var planeByAirline = byAirline.fuzzySearching(callSign.Airline, 0.9f);
            var planeByNumber = byFlightnumber.fuzzySearching(callSign.FlightNumber, 0.9f);

            if (planeByAirline == null)
            {
                return planeByNumber;
            }
            if (planeByNumber == null)
            {
                return planeByAirline;
            }
            if (planeByAirline == planeByNumber)
            {
                return planeByAirline;
            }

            // todo: handle different results
            Console.WriteLine("FindInAirSpace found two different matching Airplanes :/");
            // todo: check which one of both matches both callsign values
            return null;
        }

    }
}
