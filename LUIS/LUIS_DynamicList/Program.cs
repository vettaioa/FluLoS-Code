using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LUIS_DynamicList
{
    class Program
    {
        private static Guid APP_ID = new Guid("f2a69cf1-fad9-4bf1-b3d6-b61e6a445819");
        private const string API_KEY = "b6ecc84a0ac849819321f0c30eaf2301";
        private const string PREDICTION_ENDPOINT = "https://westeurope.api.cognitive.microsoft.com/";
        private const string API_SLOT = "Staging";  // either production or staging -> https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-publish-app#publishing-slots

        private static LUISRuntimeClient runtimeClient;

        static void Main(string[] args)
        {
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(API_KEY);
            runtimeClient = new LUISRuntimeClient(credentials) { Endpoint = PREDICTION_ENDPOINT };

            var request = new PredictionRequest { Query = "aegean 3535 climb to flight level 320" };
            var prediction = GetPredition(request);
            var predictionResult = prediction.Result;
            Console.Write(JsonConvert.SerializeObject(predictionResult, Formatting.Indented));
        }

        private async static Task<PredictionResponse> GetPredition(PredictionRequest request)
        {
            return await runtimeClient.Prediction.GetSlotPredictionAsync(APP_ID, API_SLOT, request);
        }
    }
}