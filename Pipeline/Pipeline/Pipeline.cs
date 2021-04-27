using Pipeline.Model;
using SpeechToText.Model;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pipeline
{
    class Pipeline
    {
        private Configuration config;
        private SpeechToTextRunner speechToText;
        private ContextExtractor contextExtractor;

        public Pipeline(Configuration config)
        {
            this.config = config;
            contextExtractor = new ContextExtractor(config);
            speechToText = new SpeechToTextRunner(config.SpeechToText);
        }

        public async Task Run()
        {
            Console.WriteLine("Running Speech to Text...");
            speechToText.SpeechTranscribed += ProcessTranscriptions;

            await speechToText.Run();
        }

        private void ProcessTranscriptions(TranscriptionResult transcriptionResult)
        {
            var contextResults = contextExtractor.Extract(transcriptionResult.Transcriptions);
            if (contextResults != null)
            {
                Console.WriteLine("Extracted context:");
                JsonSerializerOptions jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
                string resultsJson = JsonSerializer.Serialize(contextResults, jsonOptions);
                Console.WriteLine(resultsJson);
                if(!string.IsNullOrWhiteSpace(config.ContextOutputDirectory))
                {
                    Directory.CreateDirectory(config.ContextOutputDirectory);

                    StringBuilder sbFilePath = new StringBuilder();
                    sbFilePath.Append(config.ContextOutputDirectory);
                    if (config.SpeechToText.SpeechToTextMode == SpeechToTextMode.MicrophoneSingle)
                    {
                        sbFilePath.Append(DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                    }
                    else
                    {
                        sbFilePath.Append(Path.GetFileNameWithoutExtension(transcriptionResult.FilePath));
                    }
                    sbFilePath.Append(".json");
                    File.WriteAllText(sbFilePath.ToString(), resultsJson);
                }
            }
            else
            {
                Console.WriteLine("No results found!");
            }
        }
    }
}
