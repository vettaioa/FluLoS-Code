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
        /// <param name="config">Configuration parameters for LUIS</param>
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
            catch (Exception ex)
            {
                Console.WriteLine("Error calling LUIS API: {0}", ex.Message);
            }

            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                try
                {
                    LuisJsonResponse response = JsonSerializer.Deserialize<LuisJsonResponse>(jsonData);
                    if (response != null)
                        result = new LuisResult(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing result JSON.");
                }
            }

            return result;
        }
    }
}
