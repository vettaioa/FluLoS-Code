using LUIS.Model;
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
        public UtteranceInterpreter(string azureApiKeyFile, string apiUrl, string publishedSlot)
        {
            if (string.IsNullOrWhiteSpace(azureApiKeyFile))
                throw new ArgumentNullException("azureApiKeyFile");

            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentNullException("apiUrl");

            if (string.IsNullOrWhiteSpace(publishedSlot))
                throw new ArgumentNullException("publishedSlot");

            if (!File.Exists(azureApiKeyFile))
                throw new ArgumentException("azureApiKeyFile not found");

            AzureCredentials azureCredentials = JsonSerializer.Deserialize<AzureCredentials>(File.ReadAllText(azureApiKeyFile));


            requestURL = apiUrl
                    + azureCredentials.LUIS_appid
                    + "/slots/"
                    + publishedSlot
                    + "staging/predict?subscription-key="
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
            catch { }

            if(!string.IsNullOrWhiteSpace(jsonData))
            {
                try
                {
                    LuisJsonResponse response = JsonSerializer.Deserialize<LuisJsonResponse>(jsonData);
                    if (response != null)
                        result = new LuisResult(response);
                }
                catch { }
            }

            return result;
        }
    }
}
