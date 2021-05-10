using SharedModel;
using SpeechToText.Model;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpeechToText
{
    class TestProgram
    {
        private const string REGION = "westeurope";
        private const string TESTFILE = "../../../../../data/audio/test/sm1_01_113.wav";
        private const string CREDENTIALS_PATH = "../../../../../flulos_credentials.json";

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Setting up transcriber...");
                SpeechToTextConfig speechConfig = new SpeechToTextConfig()
                {
                    AzureApiKeysFile = CREDENTIALS_PATH,
                    AzureRegion = REGION
                };
                SpeechTranscriber transcriber = new SpeechTranscriber(speechConfig);

                Console.WriteLine("Transcribing file...");
                FileResult result = await transcriber.TranscribeAudioFile(TESTFILE);

                if (result != null && result.Transcriptions != null)
                {
                    Console.WriteLine("Results:");
                    foreach (string transcription in result.Transcriptions)
                    {
                        Console.WriteLine("- {0}", transcription);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown error occured!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error occured: {0}", ex.Message);
            }
        }
    }
}
