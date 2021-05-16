using Evaluation.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline
{
    class ContextMerger
    {
        /// <summary>
        /// Checks all extracted context data and only picks the ones that have a positive valiadation result.
        /// </summary>
        /// <param name="validatedContexts"></param>
        /// <returns>Context with only successfully validated data</returns>
        public static MessageContext Merge((MessageContext Context, EvaluationResult Validation)?[] validatedContexts)
        {
            MessageContext result = new MessageContext();
            result.Intents = new Dictionary<IntentType, MessageIntent>();

            foreach (var validatedContext in validatedContexts)
            {
                if (validatedContext != null && validatedContext.Value.Context != null && validatedContext.Value.Validation != null)
                {
                    if (string.IsNullOrWhiteSpace(result.Message))
                        result.Message = validatedContext.Value.Context.Message;

                    // set callsign if not set yet
                    if (result.CallSign == null || string.IsNullOrWhiteSpace(result.CallSign.Airline) || string.IsNullOrWhiteSpace(result.CallSign.FlightNumber))
                    {
                        if (validatedContext.Value.Validation.RadarAirplane != null && validatedContext.Value.Validation.RadarAirplane.Airplane.Flight != null)
                        {
                            RadarAirplane airplane = validatedContext.Value.Validation.RadarAirplane;

                            result.CallSign = new CallSign();

                            if (airplane.Airplane != null && airplane.Airplane.Flight != null)
                            {
                                if (airplane.Airplane.Flight.Airline != null && string.IsNullOrWhiteSpace(result.CallSign.Airline))
                                    result.CallSign.Airline = airplane.Airplane.Flight.Airline.Name;

                                if (string.IsNullOrWhiteSpace(result.CallSign.FlightNumber))
                                    result.CallSign.FlightNumber = airplane.Airplane.Flight.GetFlightNumber();
                            }
                        }
                    }

                    // check if new intent details available
                    if (validatedContext.Value.Context.Intents != null && validatedContext.Value.Context.Intents.Count > 0)
                    {
                        foreach (IntentType intent in validatedContext.Value.Context.Intents.Keys)
                        {
                            MessageIntent intentDetails = validatedContext.Value.Context.Intents[intent];
                            EvaluationResult evaluationResult = validatedContext.Value.Validation;

                            if (intentDetails != null && evaluationResult != null)
                            {
                                switch (intent)
                                {
                                    case IntentType.Contact:
                                        ContactIntent validContact = ExtractValidData(intentDetails as ContactIntent, validatedContext.Value.Validation.ContactResult);
                                        if (validContact != null)
                                        {
                                            ContactIntent contactResult;

                                            if (result.Intents.ContainsKey(intent))
                                            {
                                                contactResult = result.Intents[intent] as ContactIntent;
                                            }
                                            else
                                            {
                                                contactResult = new ContactIntent();
                                                result.Intents.Add(intent, contactResult);
                                            }

                                            if (string.IsNullOrEmpty(contactResult.Frequency) && !string.IsNullOrWhiteSpace(validContact.Frequency))
                                                contactResult.Frequency = validContact.Frequency;

                                            if (string.IsNullOrEmpty(contactResult.Place) && !string.IsNullOrWhiteSpace(validContact.Place))
                                                contactResult.Place = validContact.Place;
                                        }
                                        break;
                                    case IntentType.FlightLevel:
                                        FlightLevelIntent validLevel = ExtractValidData(intentDetails as FlightLevelIntent, validatedContext.Value.Validation.FlightLevelResult);
                                        if (validLevel != null)
                                        {
                                            FlightLevelIntent levelResult;

                                            if (result.Intents.ContainsKey(intent))
                                            {
                                                levelResult = result.Intents[intent] as FlightLevelIntent;
                                            }
                                            else
                                            {
                                                levelResult = new FlightLevelIntent();
                                                result.Intents.Add(intent, levelResult);
                                            }

                                            if (string.IsNullOrEmpty(levelResult.Level) && !string.IsNullOrWhiteSpace(validLevel.Level))
                                                levelResult.Level = validLevel.Level;

                                            if (levelResult.Instruction == null && validLevel.Instruction != null)
                                                levelResult.Instruction = validLevel.Instruction;
                                        }
                                        break;
                                    case IntentType.Squawk:
                                        SquawkIntent validSquawk = ExtractValidData(intentDetails as SquawkIntent, validatedContext.Value.Validation.SquawkResult);
                                        if (validSquawk != null)
                                        {
                                            SquawkIntent squawkResult;

                                            if (result.Intents.ContainsKey(intent))
                                            {
                                                squawkResult = result.Intents[intent] as SquawkIntent;
                                            }
                                            else
                                            {
                                                squawkResult = new SquawkIntent();
                                                result.Intents.Add(intent, squawkResult);
                                            }

                                            if (string.IsNullOrEmpty(squawkResult.Code) && !string.IsNullOrWhiteSpace(validSquawk.Code))
                                                squawkResult.Code = validSquawk.Code;
                                        }
                                        break;
                                    case IntentType.Turn:
                                        TurnIntent validTurn = ExtractValidData(intentDetails as TurnIntent, validatedContext.Value.Validation.TurnResult);
                                        if (validTurn != null)
                                        {
                                            TurnIntent turnResult;

                                            if (result.Intents.ContainsKey(intent))
                                            {
                                                turnResult = result.Intents[intent] as TurnIntent;
                                            }
                                            else
                                            {
                                                turnResult = new TurnIntent();
                                                result.Intents.Add(intent, turnResult);
                                            }

                                            if (string.IsNullOrEmpty(turnResult.Degrees) && !string.IsNullOrWhiteSpace(validTurn.Degrees))
                                                turnResult.Degrees = validTurn.Degrees;

                                            if (string.IsNullOrEmpty(turnResult.Direction) && !string.IsNullOrWhiteSpace(validTurn.Direction))
                                                turnResult.Direction = validTurn.Direction;

                                            if (string.IsNullOrEmpty(turnResult.Heading) && !string.IsNullOrWhiteSpace(validTurn.Heading))
                                                turnResult.Heading = validTurn.Heading;

                                            if (string.IsNullOrEmpty(turnResult.Place) && !string.IsNullOrWhiteSpace(validTurn.Place))
                                                turnResult.Place = validTurn.Place;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static ContactIntent ExtractValidData(ContactIntent contactInfo, ContactValidationResult validation)
        {
            ContactIntent resultContact = null;     // the result only containing valid data

            if (validation != ContactValidationResult.Invalid)
            {
                resultContact = new ContactIntent();

                if (!string.IsNullOrWhiteSpace(contactInfo.Frequency) && validation.HasFlag(ContactValidationResult.FrequencyValid))
                    resultContact.Frequency = contactInfo.Frequency;

                if (!string.IsNullOrWhiteSpace(contactInfo.Place) && validation.HasFlag(ContactValidationResult.PlaceValid))
                    resultContact.Place = contactInfo.Place;
            }

            return resultContact;
        }

        private static FlightLevelIntent ExtractValidData(FlightLevelIntent levelInfo, FlightLevelValidationResult validation)
        {
            FlightLevelIntent resultLevel = null;     // the result only containing valid data

            if (validation != FlightLevelValidationResult.Invalid)
            {
                resultLevel = new FlightLevelIntent();

                if (!string.IsNullOrWhiteSpace(levelInfo.Level) && validation.HasFlag(FlightLevelValidationResult.FlightLevelValid))
                    resultLevel.Level = levelInfo.Level;

                if (levelInfo.Instruction != null && validation.HasFlag(FlightLevelValidationResult.InstructionValid))
                    resultLevel.Instruction = levelInfo.Instruction;
            }

            return resultLevel;
        }

        private static SquawkIntent ExtractValidData(SquawkIntent squawkInfo, SquawkValidationResult validation)
        {
            SquawkIntent resultSquawk = null;     // the result only containing valid data

            if (validation != SquawkValidationResult.Invalid)
            {
                resultSquawk = new SquawkIntent();

                if (!string.IsNullOrWhiteSpace(squawkInfo.Code) && validation.HasFlag(SquawkValidationResult.CodeValid))
                    resultSquawk.Code = squawkInfo.Code;
            }

            return resultSquawk;
        }

        private static TurnIntent ExtractValidData(TurnIntent turnInfo, TurnValidationResult validation)
        {
            TurnIntent resultTurn = null;     // the result only containing valid data

            if (validation != TurnValidationResult.Invalid)
            {
                resultTurn = new TurnIntent();

                if (!string.IsNullOrWhiteSpace(turnInfo.Degrees) && validation.HasFlag(TurnValidationResult.DegreesValid))
                    resultTurn.Degrees = turnInfo.Degrees;

                if (!string.IsNullOrWhiteSpace(turnInfo.Direction) && validation.HasFlag(TurnValidationResult.DirectionValid))
                    resultTurn.Direction = turnInfo.Direction;

                if (!string.IsNullOrWhiteSpace(turnInfo.Heading) && validation.HasFlag(TurnValidationResult.HeadingValid))
                    resultTurn.Heading = turnInfo.Heading;

                if (!string.IsNullOrWhiteSpace(turnInfo.Place) && validation.HasFlag(TurnValidationResult.PlaceValid))
                    resultTurn.Place = turnInfo.Place;
            }

            return resultTurn;
        }
    }
}
