using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SpeechToText.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpeechToText
{
    public class SpeechTranscriber
    {
        private const int DEFAULT_NBEST = 3;
        private SpeechConfig speechConfig;


        /// <summary>
        /// Creates instance of SpeechTranscriber
        /// </summary>
        /// <param name="azureRegion">Azure region string (i.e. westeurope, eastus, ...)</param>
        /// <param name="azureApiKeyFile">Filepath of file with azure subscription keys</param>
        /// <exception cref="ArgumentNullException">Any parameter is null</exception>
        /// <exception cref="ArgumentException">Azure api key file is not found</exception>
        public SpeechTranscriber(string azureRegion, string azureApiKeyFile)
        {
            if (string.IsNullOrWhiteSpace(azureRegion))
                throw new ArgumentNullException("azureRegion");

            if (string.IsNullOrWhiteSpace(azureApiKeyFile))
                throw new ArgumentNullException("azureApiKeyFile");

            if(!File.Exists(azureApiKeyFile))
                throw new ArgumentException("azureApiKeyFile not found");

            AzureCredentials azureCredentials = JsonSerializer.Deserialize<AzureCredentials>(File.ReadAllText(azureApiKeyFile));

            speechConfig = SpeechConfig.FromSubscription(azureCredentials.S2T_subscription, azureRegion);
            speechConfig.OutputFormat = OutputFormat.Detailed;      // to get multiple (nBest) results from SDK
            speechConfig.EndpointId = azureCredentials.S2T_endpoint;

            // to get confidence for every word
            // credits: https://stackoverflow.com/a/61567877/3218281
            speechConfig.SetServiceProperty("wordLevelConfidence", "true", ServicePropertyChannel.UriQueryParameter);
        }

        /// <summary>
        /// Transcribes audio captured from microphone
        /// </summary>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<string[]> TranscribeMicrophone(int nBest = DEFAULT_NBEST)
        {
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Console.WriteLine("Speak into microphone...");
            return await Recognize(recognizer, nBest);
        }

        /// <summary>
        /// Transcribes audio from a single audio file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<FileResult> TranscribeAudioFile(string filePath, int nBest = DEFAULT_NBEST)
        {
            if (File.Exists(filePath))
            {
                using var audioConfig = AudioConfig.FromWavFileInput(filePath);
                using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

                string[] transcriptions = await Recognize(recognizer, nBest);

                return new FileResult() { FilePath = filePath, Transcriptions = transcriptions };
            }
            return null;
        }

        /// <summary>
        /// Transcribes multiple audio files within a directory
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<FileResult[]> TranscribeAudioDirectory(string dirPath, int nBest = DEFAULT_NBEST)
        {
            if(Directory.Exists(dirPath))
            {
                string[] dirFiles = Directory.GetFiles(dirPath);
                FileResult[] results = new FileResult[dirFiles.Length];
                try
                {
                    for (int i = 0; i < dirFiles.Length; i++)
                    {
                        FileResult result = await TranscribeAudioFile(dirFiles[i], nBest);
                        results[i] = result;
                    }
                    return results;
                }
                catch(Exception ex) when (ex is IOException || ex is SystemException)
                {
                    Console.WriteLine($"Error getting files from directory {dirPath}");
                }
            }
            return null;
        }

        /// <summary>
        /// Recognizes speech and gives back strings of possible transcriptions
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        private async Task<string[]> Recognize(SpeechRecognizer recognizer, int nBest = DEFAULT_NBEST)
        {
            SpeechRecognitionResult recognitionResult = await recognizer.RecognizeOnceAsync();

            // to get confidence for every word via json
            // credits: https://stackoverflow.com/a/61567877/3218281
            string json = recognitionResult.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);

            SpeechJsonResult jsonResult = JsonSerializer.Deserialize<SpeechJsonResult>(json);
            if(jsonResult != null && jsonResult.NBest != null)
            {
                int arraySize = nBest;
                if (jsonResult.NBest.Count < nBest)
                    arraySize = jsonResult.NBest.Count;     // in case speech api returns less results

                string[] nBestResults = new string[arraySize];
                for(int i = 0; i < arraySize; i++)
                {
                    if(i < jsonResult.NBest.Count)
                    {
                        nBestResults[i] = jsonResult.NBest[i].Lexical;
                    }
                }
                return nBestResults;
            }

            return null;
        }
    }
}
