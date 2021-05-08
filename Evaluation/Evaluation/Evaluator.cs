using Evaluation.Model;
using FuzzySearching;
using SharedModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Evaluation
{
    public class Evaluator
    {
        private readonly EvaluationConfig config;
        private AirspaceSearch airspaceSearch;

        public Evaluator(EvaluationConfig config)
        {
            this.config = config;

            InitAirspaceSearch(); // loads the current airspace state
        }

        public EvaluationResult Evaluate(MessageContext messageContext)
        {
            if (airspaceSearch == null)
            {
                throw new ApplicationException("radar data not loaded yet");
            }

            EvaluationResult result = new EvaluationResult();

            // TODO: set the overall result? "valid" and "invalid"? do we even need them?

            if (messageContext != null)
            {
                if (messageContext.CallSign != null) // no callsign = can't do radar check
                {
                    RadarAirplane radarAirplane = airspaceSearch.FindByCallSign(messageContext.CallSign);

                    //string flightNumber = radarAirplane.Airplane.Flight.GetFlightNumber();
                    //if (flightNumber == messageContext.CallSign.FlightNumber)
                    //{
                    //    result.CallSignResult |= CallSignResult.FlightNumberValid;
                    //}
                    //else
                    //{
                    //    // todo: return corrected flightnumber
                    //}

                    // todo: validate intent values based on radarAirplane data 
                }

                // evaluate intent info
                if(messageContext.Intents != null || messageContext.Intents.Count > 0)
                {
                    foreach (IntentType intentType in messageContext.Intents.Keys)
                    {
                        MessageIntent intent = messageContext.Intents[intentType];
                        if (intent != null)
                        {
                            switch (intentType)
                            {
                                case IntentType.Squawk:
                                    result.SquawkResult = IntentInfoValidator.ValidateSquawk((intent as SquawkIntent), config.Rules.SquawkCodes);
                                    break;
                                case IntentType.Contact:
                                    result.ContactResult = IntentInfoValidator.ValidateContact((intent as ContactIntent), config.Rules.ContactFrequencies, config.Rules.ContactPlaces);
                                    break;
                                case IntentType.FlightLevel:
                                    result.FlightLevelResult = IntentInfoValidator.ValidateFlightLevel((intent as FlightLevelIntent), config.Rules.FlightLevelMin, config.Rules.FlightLevelMax);
                                    break;
                                case IntentType.Turn:
                                    result.TurnResult = IntentInfoValidator.ValidateTurn(intent as TurnIntent, config.Rules.TurnPlaces);
                                    break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private async void InitAirspaceSearch()
        {
            var currentDir = Directory.GetCurrentDirectory();

            // FuzzySearch
            var phonetics = loadFromJsonFile<Dictionary<string, string>>(Path.Combine(currentDir, config.PhoneticsFile));
            var fuzzySearch = new FuzzySearch(phonetics);

            // Airspace
            var radarScanner = new RadarScanner(config.AirplanesInRangeUrl, config.AirplaneDetailsUrl);
            var radarAirplanes = await radarScanner.GetRadarAirplanes(config.LatitudeMinMax[0], config.LatitudeMinMax[1],
                                                                      config.LongitudeMinMax[0], config.LongitudeMinMax[1]);

            var knownAirlines = loadFromJsonFile<IEnumerable<KnownAirline>>(Path.Combine(currentDir, config.AirlinesFile));

            airspaceSearch = new AirspaceSearch(fuzzySearch, knownAirlines, radarAirplanes);

        }

        private static T loadFromJsonFile<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
        }
    }
}
