using LUIS.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LUIS
{
    public class UtteranceInterpreter
    {
        private string requestURL;

        /// <summary>
        /// Creates instance of UtteranceInterpreter
        /// </summary>
        /// <param name="azureApiKeyFile">Filepath of file with azure subscription keys</param>
        /// <param name="apiUrl">Full URL of REST API (i.e. "https://westeurope.api.cognitive.microsoft.com/luis/prediction/v3.0/apps/")</param>
        /// <param name="publishedSlot">Slot of published endpoint ("staging" or "production")</param>
        /// <exception cref="ArgumentNullException">Any parameter is null</exception>
        /// <exception cref="ArgumentException">Azure api key file is not found</exception>
        public UtteranceInterpreter(LuisConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (!File.Exists(config.AzureApiKeysFile))
                throw new ArgumentException("AzureApiKeysFile not found");

            AzureCredentials azureCredentials = JsonSerializer.Deserialize<AzureCredentials>(File.ReadAllText(config.AzureApiKeysFile));


            requestURL = config.ApiUrl
                    + azureCredentials.LUIS_appid
                    + "/slots/"
                    + config.PublishedSlot
                    + "/predict?subscription-key="
                    + azureCredentials.LUIS_subscription
                    + "&verbose=true&show-all-intents=true&log=true&query=";
        }
    
        public LuisResult Interpret(string utterance)
        {
            LuisResult result = null;
            string jsonData = null;

            try
            {
                WebRequest request = WebRequest.Create(requestURL + utterance);
                using (WebResponse webResponse = request.GetResponse())
                using (Stream webStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(webStream))
                {
                    jsonData = reader.ReadToEnd();
                }
            }
            catch
            {
                Console.WriteLine("Error calling LUIS API.");
            }

            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                try
                {
                    LuisJsonResponse response = JsonSerializer.Deserialize<LuisJsonResponse>(jsonData);
                    if (response != null)
                        result = new LuisResult(response);
                }
                catch
                {
                    Console.WriteLine("Error parsing result JSON.");
                }
            }

            return result;
        }
    }
}
