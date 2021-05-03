using Pipeline.Model;
using SharedModel;
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
        private AppConfiguration config;
        private SpeechToTextRunner speechToText;
        private ContextExtractor contextExtractor;

        public Pipeline(AppConfiguration config)
        {
            this.config = config;
            contextExtractor = new ContextExtractor(config);
            speechToText = new SpeechToTextRunner(config.SpeechToText, config.InputLabelDirectory);
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
                if (!string.IsNullOrWhiteSpace(config.ContextOutputDirectory))
                {
                    try
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
                    catch { }
                }

                // TODO: evaluate results (and maybe write results to file???)
                // TODO: check for set flags in result
            }
            else
            {
                Console.WriteLine("No results found!");
            }
        }
    }
}
