using LUIS.Model;
using System;

namespace LUIS
{
    class TestProgram
    {
        private const string CREDENTIALS_PATH = "../../../../../flulos_credentials.json";
        private const string URL = "https://westeurope.api.cognitive.microsoft.com/luis/prediction/v3.0/apps/";
        private const string SLOT = "staging";
        private const string QUERY = "lufthansa 3556 rhein radar identified turn right by 15 degrees and climb flight level 290";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Setting up interpreter...");
                UtteranceInterpreter interpreter = new UtteranceInterpreter(CREDENTIALS_PATH, URL, SLOT);

                Console.WriteLine("Structuring utterance...");
                LuisResult result = interpreter.Interpret(QUERY);

                if (result != null)
                {
                    Console.WriteLine("Results");

                    Console.WriteLine("- Top Intent: {0}", result.TopIntent.ToString());

                    if (result.IntentScores != null && result.IntentScores.Count > 0)
                    {
                        Console.WriteLine("- Intent Scores:");
                        foreach (IntentType intent in result.IntentScores.Keys)
                        {
                            Console.WriteLine("     - {0}  = {1}", intent.ToString(), result.IntentScores[intent]);
                        }
                    }

                    if (result.CallSign != null)
                    {
                        Console.WriteLine("- Callsign:");
                        Console.WriteLine("     - Airline  = {0}", result.CallSign.Airline);
                        Console.WriteLine("     - FlightNr = {0}", result.CallSign.FlightNr);
                    }
                }
                else
                {
                    Console.WriteLine("No result received.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: {0}", ex.Message);
            }
        }
    }
}
