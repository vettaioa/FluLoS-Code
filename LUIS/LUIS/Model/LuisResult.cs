using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUIS.Model
{
    public enum IntentType
    {
        None,
        Contact,
        FlightLevel,
        Squawk,
        Turn
    }

    public class CallSign
    {
        public string Airline { get; set; }
        public string FlightNumber { get; set; }
    }

    public class ContactInfo
    {
        public string Frequency { get; set; }
        public string Place { get; set; }
    }
    
    public class FlightLevelInfo
    {
        public string Instruction { get; set; }
        public string Level { get; set; }
    }

    public class TurnInfo
    {
        public string Direction { get; set; }
        public string Degrees { get; set; }
        public string Heading { get; set; }
        public string Place { get; set; }
    }

    public class LuisResult
    {
        public string Utterance { get; set; }
        public IntentType TopIntent { get; set; }
        public Dictionary<IntentType, float> IntentScores { get; set; }
        public CallSign CallSign { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public FlightLevelInfo FlightLevelInfo { get; set; }
        public string SquawkCode { get; set; }
        public TurnInfo TurnInfo { get; set; }
        public Dictionary<string, string> EntitiesNonML { get; set; }    // only in a dictionary as reference, as regex is used in RML

        /// <summary>
        /// Maps a json response from the LUIS API to a useful structure
        /// because the json has lots of unnecessary lists/arrays
        /// </summary>
        /// <param name="jsonResponse">Response from LUIS API</param>
        public LuisResult(LuisJsonResponse jsonResponse)
        {
            Utterance = jsonResponse.query;

            if (jsonResponse.prediction != null)
            {
                try
                {
                    TopIntent = (IntentType)Enum.Parse(typeof(IntentType), jsonResponse.prediction.topIntent);
                }
                catch { }

                // set intent scores in dictionary
                if (jsonResponse.prediction.intents != null)
                {
                    IntentScores = new Dictionary<IntentType, float>();

                    if (jsonResponse.prediction.intents.None != null)
                        IntentScores.Add(IntentType.None, jsonResponse.prediction.intents.None.score);

                    if (jsonResponse.prediction.intents.Contact != null)
                        IntentScores.Add(IntentType.Contact, jsonResponse.prediction.intents.Contact.score);

                    if (jsonResponse.prediction.intents.FlightLevel != null)
                        IntentScores.Add(IntentType.FlightLevel, jsonResponse.prediction.intents.FlightLevel.score);

                    if (jsonResponse.prediction.intents.Squawk != null)
                        IntentScores.Add(IntentType.Squawk, jsonResponse.prediction.intents.Squawk.score);

                    if (jsonResponse.prediction.intents.Turn != null)
                        IntentScores.Add(IntentType.Turn, jsonResponse.prediction.intents.Turn.score);
                }

                if (jsonResponse.prediction.entities != null)
                {

                    // set entities (extracted data)
                    if (jsonResponse.prediction.entities.MLCallSign != null && jsonResponse.prediction.entities.MLCallSign.Count > 0)
                    {
                        // MLCallSign
                        CallSign = new CallSign();
                        if (jsonResponse.prediction.entities.MLCallSign[0].Airline != null && jsonResponse.prediction.entities.MLCallSign[0].Airline.Count > 0)
                            CallSign.Airline = jsonResponse.prediction.entities.MLCallSign[0].Airline[0];
                        if (jsonResponse.prediction.entities.MLCallSign[0].FlightNr != null && jsonResponse.prediction.entities.MLCallSign[0].FlightNr.Count > 0)
                            CallSign.FlightNumber = jsonResponse.prediction.entities.MLCallSign[0].FlightNr[0];
                    }

                    if (jsonResponse.prediction.entities.MLContact != null && jsonResponse.prediction.entities.MLContact.Count > 0)
                    {
                        // MLContact
                        ContactInfo = new ContactInfo();
                        if (jsonResponse.prediction.entities.MLContact[0].Frequency != null && jsonResponse.prediction.entities.MLContact[0].Frequency.Count > 0)
                            ContactInfo.Frequency = jsonResponse.prediction.entities.MLContact[0].Frequency[0];
                        if (jsonResponse.prediction.entities.MLContact[0].Place != null && jsonResponse.prediction.entities.MLContact[0].Place.Count > 0)
                            ContactInfo.Place = jsonResponse.prediction.entities.MLContact[0].Place[0];
                    }

                    if (jsonResponse.prediction.entities.MLFlightLevel != null && jsonResponse.prediction.entities.MLFlightLevel.Count > 0)
                    {
                        // MLFlightLevel
                        FlightLevelInfo = new FlightLevelInfo();
                        if (jsonResponse.prediction.entities.MLFlightLevel[0].Instruction != null && jsonResponse.prediction.entities.MLFlightLevel[0].Instruction.Count > 0)
                            FlightLevelInfo.Instruction = jsonResponse.prediction.entities.MLFlightLevel[0].Instruction[0];
                        if (jsonResponse.prediction.entities.MLFlightLevel[0].Level != null && jsonResponse.prediction.entities.MLFlightLevel[0].Level.Count > 0)
                            FlightLevelInfo.Level = jsonResponse.prediction.entities.MLFlightLevel[0].Level[0];
                    }

                    if (jsonResponse.prediction.entities.MLSquawk != null && jsonResponse.prediction.entities.MLSquawk.Count > 0)
                    {
                        // MLSquawk
                        if (jsonResponse.prediction.entities.MLSquawk[0].Code != null && jsonResponse.prediction.entities.MLSquawk[0].Code.Count > 0)
                            SquawkCode = jsonResponse.prediction.entities.MLSquawk[0].Code[0];
                    }

                    if (jsonResponse.prediction.entities.MLTurn != null && jsonResponse.prediction.entities.MLTurn.Count > 0)
                    {
                        // MLTurn
                        TurnInfo = new TurnInfo();
                        if (jsonResponse.prediction.entities.MLTurn[0].Direction != null && jsonResponse.prediction.entities.MLTurn[0].Direction.Count > 0)
                            TurnInfo.Direction = jsonResponse.prediction.entities.MLTurn[0].Direction[0];
                        if (jsonResponse.prediction.entities.MLTurn[0].Degrees != null && jsonResponse.prediction.entities.MLTurn[0].Degrees.Count > 0 && jsonResponse.prediction.entities.MLTurn[0].Degrees[0].Value != null && jsonResponse.prediction.entities.MLTurn[0].Degrees[0].Value.Count > 0)
                            TurnInfo.Degrees = jsonResponse.prediction.entities.MLTurn[0].Degrees[0].Value[0];
                        if (jsonResponse.prediction.entities.MLTurn[0].Heading != null && jsonResponse.prediction.entities.MLTurn[0].Heading.Count > 0 && jsonResponse.prediction.entities.MLTurn[0].Heading[0].Value != null && jsonResponse.prediction.entities.MLTurn[0].Heading[0].Value.Count > 0)
                            TurnInfo.Heading = jsonResponse.prediction.entities.MLTurn[0].Heading[0].Value[0];
                        if (jsonResponse.prediction.entities.MLTurn[0].Place != null && jsonResponse.prediction.entities.MLTurn[0].Place.Count > 0 && jsonResponse.prediction.entities.MLTurn[0].Place[0].Value != null && jsonResponse.prediction.entities.MLTurn[0].Place[0].Value.Count > 0)
                            TurnInfo.Place = jsonResponse.prediction.entities.MLTurn[0].Place[0].Value[0];
                    }


                    // Non Machine-Learned Entities (for reference only)
                    EntitiesNonML = new Dictionary<string, string>();
                    if (jsonResponse.prediction.entities.RegexFlightLevel != null && jsonResponse.prediction.entities.RegexFlightLevel.Count > 0)
                        EntitiesNonML.Add("RegexFlightLevel", jsonResponse.prediction.entities.RegexFlightLevel[0]);

                    if (jsonResponse.prediction.entities.RegexTurnDegrees != null && jsonResponse.prediction.entities.RegexTurnDegrees.Count > 0)
                        EntitiesNonML.Add("RegexTurnDegrees", jsonResponse.prediction.entities.RegexTurnDegrees[0]);
                    
                    if (jsonResponse.prediction.entities.RegexSquawkNr != null && jsonResponse.prediction.entities.RegexSquawkNr.Count > 0)
                        EntitiesNonML.Add("RegexSquawkNr", jsonResponse.prediction.entities.RegexSquawkNr[0]);
                    
                    if (jsonResponse.prediction.entities.RegexTurnHeading != null && jsonResponse.prediction.entities.RegexTurnHeading.Count > 0)
                        EntitiesNonML.Add("RegexTurnHeading", jsonResponse.prediction.entities.RegexTurnHeading[0]);
                    
                    if (jsonResponse.prediction.entities.RegexTurnTo != null && jsonResponse.prediction.entities.RegexTurnTo.Count > 0)
                        EntitiesNonML.Add("RegexTurnTo", jsonResponse.prediction.entities.RegexTurnTo[0]);
                    
                    if (jsonResponse.prediction.entities.geographyV2 != null && jsonResponse.prediction.entities.geographyV2.Count > 0 && jsonResponse.prediction.entities.geographyV2[0] != null)
                        EntitiesNonML.Add("geographyV2", jsonResponse.prediction.entities.geographyV2[0].value);
                    
                    if (jsonResponse.prediction.entities.ListAirlines != null && jsonResponse.prediction.entities.ListAirlines.Count > 0 && jsonResponse.prediction.entities.ListAirlines[0] != null && jsonResponse.prediction.entities.ListAirlines[0].Count > 0)
                        EntitiesNonML.Add("ListAirlines", jsonResponse.prediction.entities.ListAirlines[0][0]); 
                }
            }
        }
    }
}
