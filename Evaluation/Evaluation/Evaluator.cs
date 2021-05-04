using Evaluation.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
    public class Evaluator
    {
        private EvaluationConfig config;
        private RadarScanner radarScanner;

        public Evaluator(EvaluationConfig config)
        {
            this.config = config;
            radarScanner = new RadarScanner(config.AirplanesInRangeUrl, config.AirplaneDetailsUrl);
        }

        public async Task<EvaluationResult> Evaluate(MessageContext messageContext)
        {
            EvaluationResult result = new EvaluationResult();

            // TODO: set the overall result? "valid" and "invalid"? do we even need them?

            if (messageContext != null)
            {
                // get airplanes in range
                IEnumerable<RadarAirplane> raderAirplanes = await radarScanner.GetRadarAirplanes(47, 48, 7, 10);
                // radarAirplanes an airspacesearch übergeh und en fuzzy search
                // Fuzzy search (siehe deltaliste)

                if (messageContext.CallSign != null)
                {
                    // TODO: correction i.e. if airline missing (only airplane with nr 2322 in radar)
                    // as skyguide mentioned, airline or flight nr get omnitted often

                    // TODO: only check intents if callsign and radar airplane found?
                }
                else
                {
                    // no callsign = can't do radar check
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
                                    result.SquawkResult = IntentInfoValidator.ValidateSquawk((intent as SquawkIntent), config.SquawkCodes);
                                    break;
                                case IntentType.Contact:
                                    result.ContactResult = IntentInfoValidator.ValidateContact((intent as ContactIntent), config.ContactFrequencies, config.ContactPlaces);
                                    break;
                                case IntentType.FlightLevel:
                                    result.FlightLevelResult = IntentInfoValidator.ValidateFlightLevel((intent as FlightLevelIntent), config.FlightLevelMin, config.FlightLevelMax);
                                    break;
                                case IntentType.Turn:
                                    result.TurnResult = IntentInfoValidator.ValidateTurn(intent as TurnIntent, config.TurnPlaces);
                                    break;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
