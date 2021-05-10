using Newtonsoft.Json.Converters;
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
        private ContextEvaluator contextEvaluator;

        public Pipeline(AppConfiguration config)
        {
            this.config = config;
            contextExtractor = new ContextExtractor(config);
            speechToText = new SpeechToTextRunner(config.SpeechToText, config.InputLabelDirectory);
            contextEvaluator = new ContextEvaluator(config.Evaluation);
        }

        public async Task Run()
        {
            Console.WriteLine("Running Speech to Text...");
            speechToText.SpeechTranscribed += ProcessTranscriptions;

            await speechToText.Run();
        }

        private void ProcessTranscriptions(TranscriptionResult transcriptionResult)
        {
            ContextExtractionResult[] contextResults = contextExtractor.Extract(transcriptionResult.Transcriptions);
            if (contextResults != null)
            {
                Console.WriteLine("Extracted context:");
                // using Newtonsoft because build-in JsonSerializer cannot handle dictionnaries with enum as key
                string resultsJson = Newtonsoft.Json.JsonConvert.SerializeObject(contextResults, Newtonsoft.Json.Formatting.Indented, new StringEnumConverter());

                Console.WriteLine(resultsJson);
                WriteToOutputDirectory(config.ContextOutputDirectory, transcriptionResult.FilePath, resultsJson);

                Console.WriteLine("Evaluation results:");
                foreach (ContextExtractionResult contextResult in contextResults)
                {
                    var evalResultLuis = contextEvaluator.Evaluate(contextResult.LuisResult);
                    var evalResultRml = contextEvaluator.Evaluate(contextResult.RmlResult);

                    string evaluationJson = Newtonsoft.Json.JsonConvert.SerializeObject(new EvaluationResultWrapper { LuisEvaluation = evalResultLuis, RmlEvaluation = evalResultRml}, Newtonsoft.Json.Formatting.Indented, new StringEnumConverter());
                    Console.WriteLine(evaluationJson);
                    WriteToOutputDirectory(config.Evaluation.OutputDirectory, transcriptionResult.FilePath, evaluationJson);
                }
            }
            else
            {
                Console.WriteLine("No results found!");
            }
        }

        private void WriteToOutputDirectory(string outputDirectory, string filename, string jsonData)
        {
            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                try
                {
                    Directory.CreateDirectory(outputDirectory);

                    StringBuilder sbFilePath = new StringBuilder();
                    sbFilePath.Append(outputDirectory);
                    if (config.SpeechToText.SpeechToTextMode == SpeechToTextMode.MicrophoneSingle)
                    {
                        sbFilePath.Append(DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                    }
                    else
                    {
                        sbFilePath.Append(Path.GetFileNameWithoutExtension(filename));
                    }
                    sbFilePath.Append(".json");
                    File.WriteAllText(sbFilePath.ToString(), jsonData);
                }
                catch { }
            }
        }
    }
}
