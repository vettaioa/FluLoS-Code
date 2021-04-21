using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace LUIS_API
{
    class Program
    {

        private const string URL = "https://westeurope.api.cognitive.microsoft.com/luis/prediction/v3.0/apps/";
        private const string QUERY = "lufthansa 3556 rhein radar identified turn right by 15 degrees and climb flight level 290";
        private const string CREDENTIALS_PATH = "../../../../../flulos_credentials.json";

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
            }
            else
            {
                Console.WriteLine("Credentials not found!");
            }
        }
    }
}
