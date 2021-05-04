using Evaluation.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Evaluation
{
    class IntentInfoValidator
    {
        internal static SquawkValidationResult ValidateSquawk(SquawkIntent squawk, string[] validCodes = null)
        {
            SquawkValidationResult result = SquawkValidationResult.Invalid;
            string squawkRegex = @"^\d{4}$";

            if (squawk != null && squawk.Code != null)
            {
                // sqawk codes are always 4 digits
                if (Regex.IsMatch(squawk.Code, squawkRegex))
                {
                    if (validCodes != null && validCodes.Length > 0)
                    {
                        // if there are predefined squawk codes in config -> check if matches one of them
                        if (validCodes.Contains(squawk.Code))
                            result |= SquawkValidationResult.CodeValid;
                    }
                    else
                    {
                        // no predefined squawk codes in config -> accept any 4 digit code
                        result |= SquawkValidationResult.CodeValid;
                    }
                }
            }

            return result;
        }
        internal static ContactValidationResult ValidateContact(ContactIntent contactIntent, int[] validFrequencies = null, string[] validPlaces = null)
        {
            ContactValidationResult result = ContactValidationResult.Invalid;
            string frequencyRegex = @"^1(\d){2}\.?\d{2}$";
            string placeRegex = @"^(A-Za-z)+$";

            if (contactIntent != null)
            {
                // check frequency format first
                if (contactIntent.Frequency != null && Regex.IsMatch(contactIntent.Frequency, frequencyRegex))
                {
                    if (validFrequencies != null && validFrequencies.Length > 0)
                    {
                        // if there are predefined contact frequencies in config -> check if matches one of them
                        int parsedFreq;
                        if (int.TryParse(contactIntent.Frequency.Replace(".", ""), out parsedFreq))
                        {
                            if (validFrequencies.Contains(parsedFreq))
                                result |= ContactValidationResult.FrequencyValid;
                        }
                    }
                    else
                    {
                        // no predefined contact frequencies in config -> accept any with valid format
                        result |= ContactValidationResult.FrequencyValid;
                    }
                }

                // check place format first
                if (contactIntent.Place != null && Regex.IsMatch(contactIntent.Place, placeRegex))
                {
                    if (validPlaces != null && validPlaces.Length > 0)
                    {
                        // if there are predefined contact places in config -> check if matches one of them
                        if (validPlaces.Any(p => string.Compare(p, contactIntent.Place, true) == 0))
                            result |= ContactValidationResult.PlaceValid;
                    }
                    else
                    {
                        // no predefined contact frequencies in config -> accept any with valid format
                        result |= ContactValidationResult.PlaceValid;
                    }
                }
            }

            return result;
        }

        internal static FlightLevelValidationResult ValidateFlightLevel(FlightLevelIntent flightLevelIntent, short? min, short? max)
        {
            FlightLevelValidationResult result = FlightLevelValidationResult.Invalid;
            string levelRegex = @"^\d{2,3}$";

            if (min == null)
                min = 0;
            if (max == null)
                max = 500;

            // check flight level
            if (flightLevelIntent != null)
            {
                if (flightLevelIntent.Level != null && Regex.IsMatch(flightLevelIntent.Level, levelRegex))
                {
                    short parsedLevel;
                    if (short.TryParse(flightLevelIntent.Level, out parsedLevel) && parsedLevel >= min && parsedLevel <= max)
                        result |= FlightLevelValidationResult.FlightLevelValid;
                }

                // check instruction
                if (flightLevelIntent.Instruction != null)
                {
                    // TODO: check wether its correct considering the current height
                    result |= FlightLevelValidationResult.InstructionValid;
                }
            }

            return result;
        }

        internal static TurnValidationResult ValidateTurn(TurnIntent turnIntent, string[] validPlaces)
        {
            TurnValidationResult result = TurnValidationResult.Invalid;
            string placeRegex = @"^(A-Za-z)+$";
            string directionRegex = @"^(left|right)$";
            string degreesRegex = @"^[0-2]?[0-9]{1,2}|3[0-5][0-9]|360$";

            if(turnIntent != null)
            {
                if (turnIntent.Degrees != null && Regex.IsMatch(turnIntent.Degrees, degreesRegex))
                    result |= TurnValidationResult.DegreesValid;

                if (turnIntent.Heading != null && Regex.IsMatch(turnIntent.Heading, degreesRegex))
                    result |= TurnValidationResult.HeadingValid;

                if (turnIntent.Direction != null && result.HasFlag(TurnValidationResult.HeadingValid) && Regex.IsMatch(turnIntent.Direction, directionRegex, RegexOptions.IgnoreCase))
                {
                    // direction can only be verified, if a heading has been set
                    // this compares the direction (left/right) with current heading of plane from radar

                    // TODO: implement
                    result |= TurnValidationResult.DirectionValid;
                }

                if (turnIntent.Place != null && Regex.IsMatch(turnIntent.Place, placeRegex))
                {
                    if (validPlaces != null && validPlaces.Length > 0)
                    {
                        // if there are predefined turn to places in config -> check if matches one of them
                        if (validPlaces.Contains(turnIntent.Place, StringComparer.OrdinalIgnoreCase))
                            result |= TurnValidationResult.PlaceValid;
                    }
                    else
                    {
                        // no predefined turn to place in config -> accept any with valid format
                        result |= TurnValidationResult.PlaceValid;
                    }
                }
            }

            return result;
        }
    }
}
