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
            EvaluationResult result = EvaluationResult.Error;

            // TODO: set the overall result? "valid" and "invalid"? do we even need them?

            if (messageContext != null)
            {
                // get airplanes in range
                IEnumerable<RadarAirplane> raderAirplanes = await radarScanner.GetRadarAirplanes(47, 48, 7, 10);

                if (messageContext.CallSign != null)
                {
                    // TODO: correction i.e. if airline missing (only airplane with nr 2322 in radar)
                    // as skyguide mentioned, airline or flight nr get omnitted often

                    // if == null -> result |= EvaluationResult.NotInAirspace;
                }
                else
                {
                    // no callsign = can't do radar check
                    result |= EvaluationResult.InvalidCallSign;
                }

                // evaluate intent info
                if(messageContext.Intents != null || messageContext.Intents.Count > 0)
                {
                    foreach (IntentType intentType in messageContext.Intents.Keys)
                    {
                        MessageIntent intent = messageContext.Intents[intentType];
                        try
                        {
                            switch (intentType)
                            {
                                case IntentType.Squawk:
                                    if (!IntentInfoValidator.IsSquawkValid((intent as SquawkIntent), config.SquawkCodes))
                                        result |= EvaluationResult.InvalidSquawkCode;
                                    break;
                                case IntentType.Contact:
                                    if (!IntentInfoValidator.IsContactInfoValid((intent as ContactIntent), config.ContactFrequencies, config.ContactPlaces))
                                        result |= EvaluationResult.InvalidContactInfo;
                                    break;
                                case IntentType.FlightLevel:
                                    if (!IntentInfoValidator.IsFlightLevelValid((intent as FlightLevelIntent), config.FlightLevelMin, config.FlightLevelMax))
                                        result |= EvaluationResult.InvalidFlightLevel;
                                    break;
                                case IntentType.Turn:
                                    if (!IntentInfoValidator.IsTurnInfoValid((intent as TurnIntent)))
                                        result |= EvaluationResult.InvalidTurnInfo;
                                    break;
                                case IntentType.None:
                                    // TODO: Handle??? set anything in returned result?
                                    break;
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    result |= EvaluationResult.NoIntent;
                }
            }

            return result;
        }
    }
}
