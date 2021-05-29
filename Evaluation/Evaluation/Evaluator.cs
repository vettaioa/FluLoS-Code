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

            if (messageContext != null)
            {
                if (messageContext.CallSign != null)
                {
                    result.RadarAirplane = airspaceSearch.FindByCallSign(messageContext.CallSign);
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
                                    result.FlightLevelResult = IntentInfoValidator.ValidateFlightLevel((intent as FlightLevelIntent), result.RadarAirplane, config.Rules.FlightLevelMin, config.Rules.FlightLevelMax);
                                    break;
                                case IntentType.Turn:
                                    result.TurnResult = IntentInfoValidator.ValidateTurn(intent as TurnIntent, result.RadarAirplane, config.Rules.TurnPlaces);
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
            IEnumerable<RadarAirplane> radarAirplanes = null;
            if (config.UseMockedAirspace)
            {
                // use a mocked airspace from predefined json
                radarAirplanes = JsonSerializer.Deserialize<IEnumerable<RadarAirplane>>(File.ReadAllText(config.MockedAirspaceFile));
            }
            else
            {
                // call radar to get current airspace
                var radarScanner = new RadarScanner(config.AirplanesInRangeUrl, config.AirplaneDetailsUrl);
                radarAirplanes = await radarScanner.GetRadarAirplanes(config.LatitudeMinMax[0], config.LatitudeMinMax[1],
                                                                          config.LongitudeMinMax[0], config.LongitudeMinMax[1]);
            }


            var knownAirlines = loadFromJsonFile<IEnumerable<KnownAirline>>(Path.Combine(currentDir, config.AirlinesFile));

            airspaceSearch = new AirspaceSearch(fuzzySearch, knownAirlines, radarAirplanes);

        }

        private static T loadFromJsonFile<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, ReadCommentHandling = JsonCommentHandling.Skip });
        }
    }
}
