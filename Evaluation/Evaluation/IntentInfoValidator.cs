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
        internal static bool IsSquawkValid(SquawkIntent squawk, string[] validCodes = null)
        {
            bool isValid = false;
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
                            isValid = true;
                    }
                    else
                    {
                        // no predefined squawk codes in config -> accept any 4 digit code
                        isValid = true;
                    }
                }
            }

            return isValid;
        }
        internal static bool IsContactInfoValid(ContactIntent contactIntent, int[] validFrequencies = null, string[] validPlaces = null)
        {
            bool isValid = false;
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
                                isValid = true;
                        }
                    }
                    else
                    {
                        // no predefined contact frequencies in config -> accept any with valid format
                        isValid = true;
                    }
                }

                // check place format first
                if (contactIntent.Place != null && Regex.IsMatch(contactIntent.Place, placeRegex))
                {
                    if (validPlaces != null && validPlaces.Length > 0)
                    {
                        // if there are predefined contact places in config -> check if matches one of them
                        if (validPlaces.Any(p => string.Compare(p, contactIntent.Place, true) == 0))
                            isValid = true;
                    }
                    else
                    {
                        // no predefined contact frequencies in config -> accept any with valid format
                        isValid = true;
                    }
                }
            }

            return isValid;
        }

        internal static bool IsFlightLevelValid(FlightLevelIntent flightLevelIntent, short? min, short? max)
        {
            bool isValid = false;
            string levelRegex = @"^\d{2,3}$";

            if (min == null)
                min = 0;
            if (max == null)
                max = 500;

            // flightLevelIntent.Instruction is omitted here, as it is already parsed
            // and the flight level is more relevant

            if (flightLevelIntent != null && flightLevelIntent.Level != null && Regex.IsMatch(flightLevelIntent.Level, levelRegex))
            {
                short parsedLevel;
                if (short.TryParse(flightLevelIntent.Level, out parsedLevel) && parsedLevel >= min && parsedLevel <= max)
                    isValid = true;
            }

            return isValid;
        }

        internal static bool IsTurnInfoValid(TurnIntent turnIntent)
        {
            bool isValid = false;
            string placeRegex = @"^(A-Za-z)+$";
            string directionRegex = @"^(left|right)$";
            string degreesRegex = @"^[0-2]?[0-9]{1,2}|3[0-5][0-9]|360$";

            if(turnIntent != null)
            {
                bool hasValidPlace = (turnIntent.Place != null && Regex.IsMatch(turnIntent.Place, placeRegex));
                bool hasValidDirection = (turnIntent.Direction != null && Regex.IsMatch(turnIntent.Direction, directionRegex, RegexOptions.IgnoreCase));
                bool hasValidDegrees = (turnIntent.Degrees != null && Regex.IsMatch(turnIntent.Degrees, degreesRegex));
                bool hasValidHeading = (turnIntent.Heading != null && Regex.IsMatch(turnIntent.Heading, degreesRegex));

                // direction is omitted, as the exact value of heading or degrees is more important
                if (hasValidPlace || hasValidDegrees || hasValidHeading)
                    isValid = true;
            }

            return isValid;
        }
    }
}
