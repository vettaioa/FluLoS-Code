using Pipeline.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline
{
    class ContextExtractor
    {
        private CleanUpCaller cleanUp;
        private DeltaListReplaceCaller deltaListReplace;
        private RmlCaller rml;
        private LuisCaller luis;

        public ContextExtractor(AppConfiguration config)
        {
            cleanUp = new CleanUpCaller();
            deltaListReplace = new DeltaListReplaceCaller();
            rml = new RmlCaller(config.Rml);
            luis = new LuisCaller(config.Luis);
        }

        public ContextExtractionResult[] Extract(string[] variants)
        {
            // output of S2T result
            Console.WriteLine("Transcription variants:");
            Array.ForEach(variants, v => Console.WriteLine($"    - {v}"));

            // preparation / cleaning
            Console.WriteLine("Preparing/cleaning data...");
            string[] preparedVariants = new string[variants.Length];
            for (int i = 0; i < variants.Length; i++)
            {
                preparedVariants[i] = Prepare(variants[i]);
            }
            Console.WriteLine("Prepared/cleaned variants:");
            Array.ForEach(preparedVariants, v => Console.WriteLine($"    - {v}"));

            // extract context
            Console.WriteLine("Extracting context...");

            var contexts = new ContextExtractionResult[preparedVariants.Length];
            for (int i = 0; i < contexts.Length; i++)
            {
                contexts[i] = Analyze(preparedVariants[i]);
            }

            return contexts;
        }

        private string Prepare(string input)
        {
            var prepared = cleanUp.Call(input);

            prepared = deltaListReplace.Call(prepared);

            return prepared;
        }

        private ContextExtractionResult Analyze(string prepared)
        {
            // RML
            Console.WriteLine("Extracting context with RML...");
            var rmlResult = rml.Call(prepared);

            // LUIS
            Console.WriteLine("Extracting context with LUIS...");
            var luisResult = luis.Call(prepared);

            return new ContextExtractionResult(rmlResult, luisResult);
        }
    }
}
