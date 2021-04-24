using Pipeline.Model;
using System;
using System.Threading.Tasks;

namespace Pipeline
{
    class Pipeline
    {
        private Configuration config;
        private SpeechToTextRunner speechToText;
        private CleanUpCaller cleanUp;
        private DeltaListReplaceCaller deltaListReplace;
        private RmlCaller rml;
        private LuisCaller luis;

        public Pipeline(Configuration config)
        {
            this.config = config;
            cleanUp = new CleanUpCaller();
            speechToText = new SpeechToTextRunner(config.SpeechToText);
            deltaListReplace = new DeltaListReplaceCaller();
            rml = new RmlCaller();
            luis = new LuisCaller(config.Luis);
        }

        public async Task Run()
        {
            Console.WriteLine("Running Speech to Text...");
            speechToText.MessageRecognized += ProcessMessage;

            await speechToText.Run();
        }

        private void ProcessMessage(string[] variants)
        {
            // output of S2T result
            Console.WriteLine("Transcription variants:");
            Array.ForEach(variants, v => Console.WriteLine($"    - {v}"));

            // preparation / cleaning
            Console.WriteLine("Preparing/cleaning data...");
            string[] preparedVariants = new string[variants.Length];
            for(int i = 0; i < variants.Length; i++)
            {
                preparedVariants[i] = Prepare(variants[i]);
            }
            Console.WriteLine("Prepared/cleaned variants:");
            Array.ForEach(preparedVariants, v => Console.WriteLine($"    - {v}"));

            // extract context
            Console.WriteLine("Extracting context with...");

            var contexts = new (MessageContext Rml, MessageContext Luis)[preparedVariants.Length];
            for (int i = 0; i < contexts.Length; i++)
            {
                contexts[i] = Analyze(preparedVariants[i]);
            }
            
        }

        private string Prepare(string input)
        {
            var prepared = cleanUp.Call(input);

            prepared = deltaListReplace.Call(prepared);

            return prepared;
        }

        private (MessageContext RmlContext, MessageContext LuisContext) Analyze(string prepared)
        {
            // RML
            Console.WriteLine("Extracting context with RML...");
            var rmlResult = rml.Call(prepared);

            // LUIS
            Console.WriteLine("Extracting context with LUIS...");
            var luisResult = luis.Call(prepared);

            return (rmlResult, luisResult);
        }
    }
}
