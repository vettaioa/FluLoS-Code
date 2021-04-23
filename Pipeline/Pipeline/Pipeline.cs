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

            // RML
            Console.WriteLine("Extracting context with RML...");
            MessageContext[] rmlContexts = new MessageContext[preparedVariants.Length];
            for (int i = 0; i < preparedVariants.Length; i++)
            {
                rmlContexts[i] = luis.Call(preparedVariants[i]);
            }
            Console.WriteLine("TODO: show result");

            // LUIS
            Console.WriteLine("Extracting context with LUIS...");
            MessageContext[] luisContexts = new MessageContext[preparedVariants.Length];
            for (int i = 0; i < preparedVariants.Length; i++)
            {
                luisContexts[i] = luis.Call(preparedVariants[i]);
            }
            Console.WriteLine("TODO: show result");
        }

        private string Prepare(string input)
        {
            var prepared = cleanUp.Call(input);

            prepared = deltaListReplace.Call(prepared);

            return prepared;
        }
    }
}
