using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Threading.Tasks;

namespace STT_SDK_TEST
{
    // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-speech-to-text?tabs=windowsinstall&pivots=programming-language-csharp

    class Program
    {

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // TODO: publish trained model and edit subscription key if needed
        //       (currently this uses the standard model)


        private const string SUB_KEY = "4237d01e4dce42cc9f8a649de2d3b5e5";
        private const string ENDPOINT = "";
        private const string REGION = "westeurope";
        private const string TESTFILE = "sm1_01_113.wav";

        async static Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription(SUB_KEY, REGION);
            //speechConfig.EndpointId = ENDPOINT;
            //await FromMic(speechConfig);
            await FromFile(speechConfig);
        }

        async static Task FromFile(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromWavFileInput(TESTFILE);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }


        async static Task FromMic(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Console.WriteLine("Speak into your microphone.");
            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }
    }
}
