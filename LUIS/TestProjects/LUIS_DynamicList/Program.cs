using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LUIS_DynamicList
{
    class Program
    {
        private const string PREDICTION_ENDPOINT = "https://westeurope.api.cognitive.microsoft.com/";
        private const string API_SLOT = "Staging";  // either production or staging -> https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-publish-app#publishing-slots
        private const string CREDENTIALS_PATH = "../../../../../flulos_credentials.json";

        private static LUISRuntimeClient runtimeClient;


        static void Main(string[] args)
        {
            if (File.Exists(CREDENTIALS_PATH))
            {
                FlulosCredentials flulosCredentials = System.Text.Json.JsonSerializer.Deserialize<FlulosCredentials>(File.ReadAllText(CREDENTIALS_PATH));

                // authenticate to service
                ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(flulosCredentials.LUIS_subscription);
                runtimeClient = new LUISRuntimeClient(credentials) { Endpoint = PREDICTION_ENDPOINT };


                // prepare data for utterance without errors
                var querySimple = "aegean 3535 climb to flight level 320";


                // prepare data for utterance with small error
                var queryWithError = "aegean 3535 lime to flight level 320";
                var correctWord = "climb";
                var correctionListName = "ListDynamicSynonyms";     // list entity must already exist in LUIS
                var errorSynonyms = new List<string>();
                errorSynonyms.Add("lime");
                errorSynonyms.Add("comb");

                var requestLists = new List<RequestList>();
                requestLists.Add(new RequestList(correctWord, correctWord, errorSynonyms));

                var dynamicLists = new List<DynamicList>();
                dynamicLists.Add(new DynamicList(correctionListName, requestLists));


                //show - all - intents = true
                // make request for utterance without errors
                var requestSimple = new PredictionRequest { Query = querySimple };
                var predictionSimple = GetPrediction(requestSimple, flulosCredentials.LUIS_appid);
                Console.WriteLine("***********     SIMPLE      ***********");
                Console.WriteLine(JsonConvert.SerializeObject(predictionSimple.Result, Formatting.Indented));


                // make request for utterance with small error
                var requestDynamicList = new PredictionRequest
                {
                    Query = queryWithError,
                    DynamicLists = dynamicLists
                };
                var predictionDynamicList = GetPrediction(requestDynamicList, flulosCredentials.LUIS_appid);
                Console.WriteLine("***********   DYNAMICLIST   ***********");
                Console.WriteLine(JsonConvert.SerializeObject(predictionDynamicList.Result, Formatting.Indented));
            }
            else
            {
                Console.WriteLine("Credentials not found!");
            }
        }

        private async static Task<PredictionResponse> GetPrediction(PredictionRequest request, string appID)
        {
            return await runtimeClient.Prediction.GetSlotPredictionAsync(new Guid(appID), API_SLOT, request);
        }
    }
}