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
            speechToText.MessageRecognized += ProcessMessage;

            await speechToText.Run();
        }

        private void ProcessMessage(string[] variants)
        {
            foreach(string variant in variants)
            {
                Console.WriteLine();
                Console.WriteLine("Processing Message:");
                Console.WriteLine(variant);

                var prepared = Prepare(variant);
                Console.WriteLine(" ===> ");
                Console.WriteLine(prepared);

                var rmlContext = rml.Call(prepared);
                var luisContext = luis.Call(prepared);
                Console.WriteLine(" ===> ");
                Console.WriteLine("TODO");
            }
        }

        private string Prepare(string input)
        {
            var prepared = cleanUp.Call(input);

            prepared = deltaListReplace.Call(prepared);

            return prepared;
        }
    }
}
