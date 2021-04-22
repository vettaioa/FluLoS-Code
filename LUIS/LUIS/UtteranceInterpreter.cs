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

        public UtteranceInterpreter(string azureApiKeyFile, string apiUrl, string publishedSlot)
        {
            if (string.IsNullOrWhiteSpace(azureApiKeyFile))
                throw new ArgumentNullException("azureApiKeyFile");

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
    
        public string Interpret(string utterance)
        {
            WebRequest request = WebRequest.Create(requestURL + utterance);

            using WebResponse webResponse = request.GetResponse();
            using Stream webStream = webResponse.GetResponseStream();

            using StreamReader reader = new StreamReader(webStream);
            string data = reader.ReadToEnd();

            // TODO: parse and return idk

            return data;
        }
    }
}
