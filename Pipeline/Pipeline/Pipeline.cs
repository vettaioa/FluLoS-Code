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
            if (transcriptionResult != null && transcriptionResult.Transcriptions != null)
            {
                ContextResultWrapper[] contextResults = contextExtractor.Extract(transcriptionResult.Transcriptions);
                if (contextResults != null)
                {
                    string resultsJson = SerializeToJson(contextResults);
                    Console.WriteLine("Extracted context:");
                    Console.WriteLine(resultsJson);
                    WriteToOutput(OutputType.CONTEXTS, transcriptionResult.FilePath, resultsJson);

                    if (config.Evaluation.RunEvaluation)
                    {

                        // evaluate all context extraction results
                        EvaluationResult[] luisEvaluations = new EvaluationResult[contextResults.Length];
                        EvaluationResult[] rmlEvaluations = new EvaluationResult[contextResults.Length];

                        for (int i = 0; i < contextResults.Length; i++)
                        {
                            luisEvaluations[i] = contextEvaluator.Evaluate(contextResults[i].LuisContext);
                            rmlEvaluations[i] = contextEvaluator.Evaluate(contextResults[i].RmlContext);
                        }
                        string evaluationJson = SerializeToJson(new EvaluationResultsWrapper(luisEvaluations, rmlEvaluations));
                        Console.WriteLine("Evaluation results:");
                        Console.WriteLine(evaluationJson);
                        WriteToOutput(OutputType.EVALUATIONFLAGS, transcriptionResult.FilePath, evaluationJson);


                        // extract correct data by considering the evaluation results (priorizing the first correct occurence of a field)
                        (MessageContext, EvaluationResult)?[] luisValidatedContexts = new (MessageContext, EvaluationResult)?[contextResults.Length];
                        (MessageContext, EvaluationResult)?[] rmlValidatedContexts = new (MessageContext, EvaluationResult)?[contextResults.Length];
                        for (int i = 0; i < luisEvaluations.Length; i++)
                        {
                            luisValidatedContexts[i] = (contextResults[i].LuisContext, luisEvaluations[i]);
                        }
                        for (int i = 0; i < rmlEvaluations.Length; i++)
                        {
                            rmlValidatedContexts[i] = (contextResults[i].RmlContext, rmlEvaluations[i]);
                        }

                        MessageContext bestLuisContext = ContextMerger.Merge(luisValidatedContexts);
                        MessageContext bestRmlContext = ContextMerger.Merge(rmlValidatedContexts);
                        string bestContextJson = SerializeToJson(new ContextResultWrapper() { LuisContext = bestLuisContext, RmlContext = bestRmlContext });
                        Console.WriteLine("Validated Results:");
                        Console.WriteLine(bestContextJson);
                        WriteToOutput(OutputType.VALIDATEDMERGED, transcriptionResult.FilePath, bestContextJson);
                    }
                }
                else
                {
                    Console.WriteLine("No results found!");
                }
            }
            else
            {
                Console.WriteLine("No transcriptions received!");
            }
        }

        protected enum OutputType { CONTEXTS, EVALUATIONFLAGS, VALIDATEDMERGED }
        protected void WriteToOutput(OutputType outputType, string fileName, string jsonData)
        {
            switch (outputType)
            {
                case OutputType.CONTEXTS:
                    WriteToOutputDirectory(config.ContextOutputDirectory, fileName, jsonData);
                    break;
                case OutputType.EVALUATIONFLAGS:
                    WriteToOutputDirectory(config.Evaluation.FlagsOutputDirectory, fileName, jsonData);
                    break;
                case OutputType.VALIDATEDMERGED:
                    WriteToOutputDirectory(config.Evaluation.MergedOutputDirectory, fileName, jsonData);
                    break;
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

        private string SerializeToJson(object obj)
        {
            // using Newtonsoft because build-in JsonSerializer cannot handle dictionnaries with enum as key
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new StringEnumConverter());
        }

    }


}
