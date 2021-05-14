using Evaluation.Model;
using Newtonsoft.Json.Converters;
using Pipeline.Model;
using SharedModel;
using SpeechToText.Model;
using System;
using System.Collections.Generic;
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


                // evaluate all context extraction results
                EvaluationResult[] luisEvaluations = new EvaluationResult[contextResults.Length];
                EvaluationResult[] rmlEvaluations = new EvaluationResult[contextResults.Length];

                for(int i = 0; i < contextResults.Length; i++)
                {
                    luisEvaluations[i] = contextEvaluator.Evaluate(contextResults[i].LuisResult);
                    rmlEvaluations[i] = contextEvaluator.Evaluate(contextResults[i].RmlResult);
                }
                EvaluationResultsWrapper evaluationResults = new EvaluationResultsWrapper(luisEvaluations, rmlEvaluations);
                string evaluationJson = Newtonsoft.Json.JsonConvert.SerializeObject(evaluationResults, Newtonsoft.Json.Formatting.Indented, new StringEnumConverter());
                Console.WriteLine("Evaluation results:");
                Console.WriteLine(evaluationJson);
                WriteToOutputDirectory(config.Evaluation.ValidationOutputDirectory, transcriptionResult.FilePath, evaluationJson);


                // extract correct data by considering the evaluation results (priorizing the first correct occurence of a field)
                (MessageContext, EvaluationResult)?[] luisValidatedContexts = new (MessageContext, EvaluationResult)?[contextResults.Length];
                (MessageContext, EvaluationResult)?[] rmlValidatedContexts = new (MessageContext, EvaluationResult)?[contextResults.Length];
                for (int i = 0; i < luisEvaluations.Length; i++)
                {
                    luisValidatedContexts[i] = (contextResults[i].LuisResult, luisEvaluations[i]);
                }
                for (int i = 0; i < rmlEvaluations.Length; i++)
                {
                    luisValidatedContexts[i] = (contextResults[i].LuisResult, luisEvaluations[i]);
                }

                MessageContext bestLuisContext = ContextMerger.Merge(luisValidatedContexts);
                MessageContext bestRmlContext = ContextMerger.Merge(rmlValidatedContexts);
                (MessageContext LuisContext, MessageContext RmlContext) bestContexts = (bestLuisContext, bestRmlContext);
                string bestContextJson = Newtonsoft.Json.JsonConvert.SerializeObject(bestContexts, Newtonsoft.Json.Formatting.Indented, new StringEnumConverter());
                Console.WriteLine("Validated Results:");
                Console.WriteLine(bestContextJson);
                WriteToOutputDirectory(config.Evaluation.MergeOutputDirectory, transcriptionResult.FilePath, evaluationJson);
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
