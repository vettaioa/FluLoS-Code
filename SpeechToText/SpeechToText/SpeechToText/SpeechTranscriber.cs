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

            return await Recognize(recognizer, nBest);
        }

        /// <summary>
        /// Transcribes audio from a single audio file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<string[]> TranscribeAudioFile(string filePath, int nBest = DEFAULT_NBEST)
        {
            if (File.Exists(filePath))
            {
                using var audioConfig = AudioConfig.FromWavFileInput(filePath);
                using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

                return await Recognize(recognizer, nBest);
            }
            return null;
        }

        /// <summary>
        /// Transcribes multiple audio files within a directory
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<string[,]> TranscribeAudioDirectory(string dirPath, int nBest = DEFAULT_NBEST)
        {
            if(Directory.Exists(dirPath))
            {
                string[] dirFiles = Directory.GetFiles(dirPath);
                string[,] results = new string[dirFiles.Length, nBest];
                try
                {
                    for(int i = 0; i < dirFiles.Length; i++)
                    {
                        string[] fileTranscriptions = await TranscribeAudioFile(dirFiles[i], nBest);
                        for(int j = 0; j < fileTranscriptions.Length; j++)
                        {
                            results[i, j] = fileTranscriptions[j];
                        }
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
                string[] nBestResults = new string[nBest];
                for(int i = 0; i < nBest; i++)
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
