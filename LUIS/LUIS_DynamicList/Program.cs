﻿using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
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
            // authenticate to service
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(API_KEY);
            runtimeClient = new LUISRuntimeClient(credentials) { Endpoint = PREDICTION_ENDPOINT };


            // prepare data for utterance without errors
            var querySimple = "aegean 3535 climb to flight level 320";


            // prepare data for utterance with small error
            var queryWithError = "aegean 3535 lime to flight level 320";
            var correctWord = "climb";
            var correctionListName = "ClimbSynonymList";
            var errorSynonyms = new List<string>();
            errorSynonyms.Add("lime");
            errorSynonyms.Add("comb");

            var requestLists = new List<RequestList>();
            requestLists.Add(new RequestList(correctWord, correctWord, errorSynonyms));

            var dynamicLists = new List<DynamicList>();
            dynamicLists.Add(new DynamicList(correctionListName, requestLists));


            // make request for utterance without errors
            var requestSimple = new PredictionRequest { Query = querySimple };
            var predictionSimple = GetPrediction(requestSimple);
            Console.WriteLine("***********     SIMPLE      ***********");
            Console.WriteLine(JsonConvert.SerializeObject(predictionSimple.Result, Formatting.Indented));


            // make request for utterance with small error
            var requestDynamicList = new PredictionRequest
            {
                Query = queryWithError,
                DynamicLists = dynamicLists
            };
            var predictionDynamicList = GetPrediction(requestDynamicList);
            Console.WriteLine("***********   DYNAMICLIST   ***********");
            Console.WriteLine(JsonConvert.SerializeObject(predictionDynamicList.Result, Formatting.Indented));
        }

        private async static Task<PredictionResponse> GetPrediction(PredictionRequest request)
        {
            return await runtimeClient.Prediction.GetSlotPredictionAsync(APP_ID, API_SLOT, request);
        }
    }
}