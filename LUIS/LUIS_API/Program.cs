using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace LUIS_API
{
    class Program
    {

        private const string URL = "https://westeurope.api.cognitive.microsoft.com/luis/prediction/v3.0/apps/468f22b2-8753-4107-bb47-a4218efa8d62/slots/staging/predict?subscription-key=b6ecc84a0ac849819321f0c30eaf2301&verbose=true&show-all-intents=true&log=true&query=";
        private const string QUERY = "lufthansa 3556 rhein radar identified turn right by 15 degrees and climb flight level 290";

        static void Main(string[] args)
        {
            var request = WebRequest.Create(URL + QUERY);

            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            Console.WriteLine(data);
        }
    }
}
