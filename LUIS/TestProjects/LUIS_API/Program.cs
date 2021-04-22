using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace LUIS_API
{
    class Program
    {

        private const string URL = "https://westeurope.api.cognitive.microsoft.com/luis/prediction/v3.0/apps/";
        private const string QUERY = "lufthansa 3556 rhein radar identified turn right by 15 degrees and climb flight level 290";
        private const string CREDENTIALS_PATH = "../../../../../../flulos_credentials.json";

        static void Main(string[] args)
        {
            if(File.Exists(CREDENTIALS_PATH))
            {
                FlulosCredentials flulosCredentials = JsonSerializer.Deserialize<FlulosCredentials>(File.ReadAllText(CREDENTIALS_PATH));

                string requestURL = URL
                    + flulosCredentials.LUIS_appid
                    + "/slots/staging/predict?subscription-key="
                    + flulosCredentials.LUIS_subscription
                    + "&verbose=true&show-all-intents=true&log=true&query="
                    + QUERY;

                var request = WebRequest.Create(requestURL);

                using var webResponse = request.GetResponse();
                using var webStream = webResponse.GetResponseStream();

                using var reader = new StreamReader(webStream);
                var data = reader.ReadToEnd();

                Console.WriteLine(data);
                Console.WriteLine();

                //var data = "{\"query\":\"lufthansa 3556 rhein radar identified turn right by 15 degrees and climb flight level 290\",\"prediction\":{\"topIntent\":\"Turn\",\"intents\":{\"Turn\":{\"score\":0.99614793},\"FlightLevel\":{\"score\":0.99293},\"None\":{\"score\":0.0026656773},\"Contact\":{\"score\":0.0019903472},\"Squawk\":{\"score\":0.001772674}},\"entities\":{\"MLCallSign\":[{\"Airline\":[\"lufthansa\"],\"FlightNr\":[\"3556\"],\"$instance\":{\"Airline\":[{\"type\":\"Airline\",\"text\":\"lufthansa\",\"startIndex\":0,\"length\":9,\"score\":0.97272694,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"FlightNr\":[{\"type\":\"FlightNr\",\"text\":\"3556\",\"startIndex\":10,\"length\":4,\"score\":0.972846,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}]}}],\"MLFlightLevel\":[{\"Instruction\":[\"climb\"],\"Level\":[\"290\"],\"$instance\":{\"Instruction\":[{\"type\":\"Instruction\",\"text\":\"climb\",\"startIndex\":67,\"length\":5,\"score\":0.94608176,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"Level\":[{\"type\":\"Level\",\"text\":\"290\",\"startIndex\":86,\"length\":3,\"score\":0.9925363,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}]}}],\"MLTurn\":[{\"Direction\":[\"right\"],\"Degrees\":[{\"Value\":[\"15\"],\"$instance\":{\"Value\":[{\"type\":\"Value\",\"text\":\"15\",\"startIndex\":52,\"length\":2,\"score\":0.92356783,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}]}}],\"$instance\":{\"Direction\":[{\"type\":\"Direction\",\"text\":\"right\",\"startIndex\":43,\"length\":5,\"score\":0.962767,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"Degrees\":[{\"type\":\"Degrees\",\"text\":\"15 degrees\",\"startIndex\":52,\"length\":10,\"score\":0.86231714,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}]}}],\"ListAirlines\":[[\"lufthansa\"]],\"RegexTurnDegrees\":[\"15 degrees\"],\"RegexFlightLevel\":[\"flight level 290\"],\"$instance\":{\"MLCallSign\":[{\"type\":\"MLCallSign\",\"text\":\"lufthansa 3556\",\"startIndex\":0,\"length\":14,\"score\":0.9588746,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"MLFlightLevel\":[{\"type\":\"MLFlightLevel\",\"text\":\"climb flight level 290\",\"startIndex\":67,\"length\":22,\"score\":0.96721953,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"MLTurn\":[{\"type\":\"MLTurn\",\"text\":\"turn right by 15 degrees\",\"startIndex\":38,\"length\":24,\"score\":0.7318336,\"modelTypeId\":1,\"modelType\":\"Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"ListAirlines\":[{\"type\":\"ListAirlines\",\"text\":\"lufthansa\",\"startIndex\":0,\"length\":9,\"modelTypeId\":5,\"modelType\":\"List Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"RegexTurnDegrees\":[{\"type\":\"RegexTurnDegrees\",\"text\":\"15 degrees\",\"startIndex\":52,\"length\":10,\"modelTypeId\":8,\"modelType\":\"Regex Entity Extractor\",\"recognitionSources\":[\"model\"]}],\"RegexFlightLevel\":[{\"type\":\"RegexFlightLevel\",\"text\":\"flight level 290\",\"startIndex\":73,\"length\":16,\"modelTypeId\":8,\"modelType\":\"Regex Entity Extractor\",\"recognitionSources\":[\"model\"]}]}}}}";
                LuisJsonResponse parsedResponse = JsonSerializer.Deserialize<LuisJsonResponse>(data);

                LuisResult structuredResult = new LuisResult(parsedResponse);

                Console.WriteLine("Top Intent = " + structuredResult.TopIntent);
                if (structuredResult.CallSign != null)
                    Console.WriteLine("Airline = {0} \t FlightNr = {1}", structuredResult.CallSign.Airline, structuredResult.CallSign.FlightNr);
            }
            else
            {
                Console.WriteLine("Credentials not found!");
            }
        }
    }
}
