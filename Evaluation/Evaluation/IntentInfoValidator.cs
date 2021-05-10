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

        internal static FlightLevelValidationResult ValidateFlightLevel(FlightLevelIntent flightLevelIntent, RadarAirplane airplane, short? min, short? max)
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
                short parsedIntentLevel = 0;

                if (flightLevelIntent.Level != null && Regex.IsMatch(flightLevelIntent.Level, levelRegex))
                {
                    if (short.TryParse(flightLevelIntent.Level, out parsedIntentLevel) && parsedIntentLevel >= min && parsedIntentLevel <= max)
                        result |= FlightLevelValidationResult.FlightLevelValid;
                }

                // check instruction (only if flight level was valid, because only then a well-founded statement can be made)
                if (result.HasFlag(FlightLevelValidationResult.FlightLevelValid) && flightLevelIntent.Instruction != null && airplane != null && airplane.Position != null&& airplane.Position.Altitude > 0)
                {
                    // check wether the instruction (climb/maintain/descend) is correct considering the current height
                    int? radarFlightLevel = ConvertMetersToFlightLevel(airplane.Position.Altitude);

                    if (radarFlightLevel != null)
                    {
                        switch (flightLevelIntent.Instruction)
                        {
                            case FlightLevelIntent.FlightLevelInstruction.Climb:
                                if (parsedIntentLevel > radarFlightLevel)
                                    result |= FlightLevelValidationResult.InstructionValid;
                                break;
                            case FlightLevelIntent.FlightLevelInstruction.Descend:
                                if (parsedIntentLevel < radarFlightLevel)
                                    result |= FlightLevelValidationResult.InstructionValid;
                                break;
                            case FlightLevelIntent.FlightLevelInstruction.Maintain:
                                if (parsedIntentLevel == radarFlightLevel)
                                    result |= FlightLevelValidationResult.InstructionValid;
                                break;
                        }
                    }
                }
            }

            return result;
        }

        internal static TurnValidationResult ValidateTurn(TurnIntent turnIntent, RadarAirplane airplane, string[] validPlaces)
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

                if (turnIntent.Direction != null && Regex.IsMatch(turnIntent.Direction, directionRegex, RegexOptions.IgnoreCase))
                {
                    // direction can only be verified, if a heading has been set
                    // this compares the direction (left/right) with current heading of plane from radar
                    if (result.HasFlag(TurnValidationResult.HeadingValid) && airplane != null && airplane.Position != null && airplane.Position.Heading >= 0 && airplane.Position.Heading <= 360)
                    {
                        int intentHeading;

                        if(int.TryParse(turnIntent.Heading, out intentHeading))
                        {
                            int? headingDifference = null;

                            try
                            {
                                headingDifference = Convert.ToInt32((intentHeading - airplane.Position.Heading));
                            }
                            catch { }

                            if(headingDifference != null)
                            {
                                if (headingDifference < 0)
                                    headingDifference += 360;

                                if(headingDifference > 180 && turnIntent.Direction == "left")
                                {
                                    // turn should be left
                                    result |= TurnValidationResult.DirectionValid;
                                }
                                else if(headingDifference < 180 && turnIntent.Direction == "right")
                                {
                                    // turn should be right
                                    result |= TurnValidationResult.DirectionValid;
                                }
                            }
                        }

                    }
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

        private static int? ConvertMetersToFlightLevel(int meters)
        {
            int? flightLevel = null;

            try
            {
                // Flight Level = Hundreds of Feet (i.e. FL240 = 24000 feet)
                // 1 foot = 3.280839895 meters
                // -> FL = feet / 100 = (meters * 3.280839895) / 100
                flightLevel = Convert.ToInt32((meters * 3.280839895) / 100);
            }
            catch { }

            return flightLevel;
        }
    }
}
