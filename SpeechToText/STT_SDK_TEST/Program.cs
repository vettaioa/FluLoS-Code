using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Threading.Tasks;

namespace STT_SDK_TEST
{
    // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-speech-to-text?tabs=windowsinstall&pivots=programming-language-csharp

    class Program
    {
        private const string SUB_KEY = "4237d01e4dce42cc9f8a649de2d3b5e5";
        private const string REGION = "westeurope";
        private const string ENDPOINT = "c53a10b1-77cf-4252-aecd-e5910c17d799";
        private const string TESTFILE = "sm1_01_113.wav";

        async static Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription(SUB_KEY, REGION);

            speechConfig.OutputFormat = OutputFormat.Detailed;      // to get multiple results from SDK
            speechConfig.EndpointId = ENDPOINT;

            // to get confidence for every word
            // credits: https://stackoverflow.com/a/61567877/3218281
            speechConfig.SetServiceProperty("wordLevelConfidence", "true", ServicePropertyChannel.UriQueryParameter);

            //await FromMic(speechConfig);
            await FromFile(speechConfig);
        }


        // TODO: handle errors according to sample
        // https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/quickstart/csharp/dotnet/from-file/helloworld/Program.cs



        async static Task FromFile(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromWavFileInput(TESTFILE);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            WriteResult(result);
        }

        async static Task FromMic(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();            
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Console.WriteLine("Speak into your microphone.");
            var result = await recognizer.RecognizeOnceAsync();
            WriteResult(result);
        }

        private static void WriteResult(SpeechRecognitionResult result)
        {
            Console.WriteLine($"RECOGNIZED: {result.Text}");
            Console.WriteLine("Alternatives:");

            // NOTE: microsoft recommends using the restult itself, even if an alternative from Best() has higher confidence
            // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/faq-stt -> "Q: I get several results for each phrase with the detailed output format. Which one should I use?"

            foreach (var alt in result.Best())
            {
                Console.WriteLine($"\t{alt.Confidence}\t{alt.Text}");
            }

            // to get confidence for every word via json
            // credits: https://stackoverflow.com/a/61567877/3218281
            string json = result.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);
            Console.WriteLine(json);

            Console.WriteLine();
        }
    }
}
