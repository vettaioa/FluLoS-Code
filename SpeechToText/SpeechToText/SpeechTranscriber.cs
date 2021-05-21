using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SharedModel;
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
        private const string FILE_EXTENSION = ".wav";
        private SpeechConfig speechConfig;
        private string outputDirectory;

        /// <summary>
        /// Creates instance of SpeechTranscriber
        /// </summary>
        /// <param name="azureRegion">Azure region string (i.e. westeurope, eastus, ...)</param>
        /// <param name="azureApiKeyFile">Filepath of file with azure subscription keys</param>
        /// <exception cref="ArgumentNullException">Any parameter is null</exception>
        /// <exception cref="ArgumentException">Azure api key file is not found</exception>
        public SpeechTranscriber(SpeechToTextConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (!File.Exists(config.AzureApiKeysFile))
                throw new ArgumentException("AzureApiKeysFile not found");

            outputDirectory = config.OutputNBestDirectory;

            AzureCredentials azureCredentials = JsonSerializer.Deserialize<AzureCredentials>(File.ReadAllText(config.AzureApiKeysFile));
            speechConfig = SpeechConfig.FromSubscription(azureCredentials.S2T_subscription, config.AzureRegion);
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


            string[] transcriptions = await Recognize(recognizer, nBest);

            WriteOutput(outputDirectory, DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".json", transcriptions);

            return transcriptions;
        }

        /// <summary>
        /// Transcribes audio from a single audio file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<FileResult> TranscribeAudioFile(string filePath, int nBest = DEFAULT_NBEST)
        {
            if (File.Exists(filePath) && Path.GetExtension(filePath) == FILE_EXTENSION)
            {
                using var audioConfig = AudioConfig.FromWavFileInput(filePath);
                using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

                string[] transcriptions = await Recognize(recognizer, nBest);

                WriteOutput(outputDirectory, Path.GetFileNameWithoutExtension(filePath) + ".json", transcriptions);

                return new FileResult() { FilePath = filePath, Transcriptions = transcriptions };
            }
            else
            {
                Console.WriteLine("  Skipping because it is not a {0} file", FILE_EXTENSION);
            }
            return null;
        }

        /// <summary>
        /// Transcribes multiple audio files
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<FileResult[]> TranscribeAudioFiles(string[] filePaths, int nBest = DEFAULT_NBEST)
        {
            FileResult[] results = null;

            if (filePaths != null && filePaths.Length > 0)
            {
                results = new FileResult[filePaths.Length];
                for (int i = 0; i < filePaths.Length; i++)
                {
                    Console.WriteLine("  {0}/{1}: {2}", i+1, filePaths.Length, filePaths[i]);
                    FileResult result = await TranscribeAudioFile(filePaths[i], nBest);
                    results[i] = result;
                }
            }

            return results;
        }

        /// <summary>
        /// Transcribes multiple audio files within a directory
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="nBest"></param>
        /// <returns></returns>
        public async Task<FileResult[]> TranscribeAudioDirectory(string dirPath, int nBest = DEFAULT_NBEST)
        {
            if (Directory.Exists(dirPath))
            {
                try
                {
                    string[] dirFiles = Directory.GetFiles(dirPath);
                    return await TranscribeAudioFiles(dirFiles, nBest);
                }
                catch (Exception ex) when (ex is IOException || ex is SystemException)
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

            if (!string.IsNullOrWhiteSpace(json))
            {
                SpeechJsonResult jsonResult = JsonSerializer.Deserialize<SpeechJsonResult>(json);
                if (jsonResult != null && jsonResult.NBest != null)
                {
                    int arraySize = nBest;
                    if (jsonResult.NBest.Count < nBest)
                        arraySize = jsonResult.NBest.Count;     // in case speech api returns less results

                    string[] nBestResults = new string[arraySize];
                    for (int i = 0; i < arraySize; i++)
                    {
                        if (i < jsonResult.NBest.Count)
                        {
                            nBestResults[i] = jsonResult.NBest[i].Lexical;
                        }
                    }
                    return nBestResults;
                }
            }

            return null;
        }

        private void WriteOutput(string directory, string filename, string[] transcriptions)
        {
            try
            {
                Directory.CreateDirectory(directory);

                JsonSerializerOptions jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
                string json = JsonSerializer.Serialize(transcriptions, jsonOptions);

                File.WriteAllText(Path.Combine(directory, filename), json);
            }
            catch { }
        }
    }
}
