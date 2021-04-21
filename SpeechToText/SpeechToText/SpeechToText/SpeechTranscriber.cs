using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpeechToText
{
    class SpeechTranscriber
    {
        private const int DEFAULT_NBEST = 3;
        private SpeechConfig speechConfig;

        /// <summary>
        /// Creates instance of SpeechTranscriber
        /// </summary>
        /// <param name="azureRegion">Azure region string (i.e. westeurope, eastus, ...)</param>
        /// <param name="azureSubscription">Azure subscription key</param>
        /// <param name="azureEndpoint">Azure endpoint of custom speech resource</param>
        public SpeechTranscriber(string azureRegion, string azureSubscription, string azureEndpoint)
        {
            if (string.IsNullOrWhiteSpace(azureRegion))
                throw new ArgumentNullException("azureRegion");

            if (string.IsNullOrWhiteSpace(azureSubscription))
                throw new ArgumentNullException("azureSubscription");

            if (string.IsNullOrWhiteSpace(azureEndpoint))
                throw new ArgumentNullException("azureEndpoint");
                
            speechConfig = SpeechConfig.FromSubscription(azureSubscription, azureRegion);

            speechConfig.OutputFormat = OutputFormat.Detailed;      // to get multiple results from SDK
            speechConfig.EndpointId = azureEndpoint;

            // to get confidence for every word
            // credits: https://stackoverflow.com/a/61567877/3218281
            speechConfig.SetServiceProperty("wordLevelConfidence", "true", ServicePropertyChannel.UriQueryParameter);
        }

        /// <summary>
        /// Transcribes audio captured from microphone
        /// </summary>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<IList<string>> TranscribeMicrophone(int nBest = DEFAULT_NBEST)
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
        public async Task<IList<string>> TranscribeAudioFile(string filePath, int nBest = DEFAULT_NBEST)
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
        public async Task<IList<IList<string>>> TranscribeAudioDirectory(string dirPath, int nBest = DEFAULT_NBEST)
        {
            if(Directory.Exists(dirPath))
            {
                IList<IList<string>> results = new List<IList<string>>();
                try
                {
                    foreach (string file in Directory.GetFiles(dirPath))
                    {
                        results.Add(await TranscribeAudioFile(file, nBest));
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
        private async Task<IList<string>> Recognize(SpeechRecognizer recognizer, int nBest = DEFAULT_NBEST)
        {
            SpeechRecognitionResult recognitionResult = await recognizer.RecognizeOnceAsync();

            // to get confidence for every word via json
            // credits: https://stackoverflow.com/a/61567877/3218281
            string json = recognitionResult.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);

            SpeechJsonResult jsonResult = JsonSerializer.Deserialize<SpeechJsonResult>(json);
            if(jsonResult != null && jsonResult.NBest != null)
            {
                IList<string> nBestResults = new List<string>();
                for(int i = 0; i < nBest; i++)
                {
                    if(i < jsonResult.NBest.Count)
                    {
                        nBestResults.Add(jsonResult.NBest[i].Lexical);
                    }
                }
                return nBestResults;
            }

            return null;
        }
    }
}
