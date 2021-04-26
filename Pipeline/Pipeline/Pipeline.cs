using Pipeline.Model;
using System;
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
            speechToText.MessageRecognized += ProcessMessage;

            await speechToText.Run();
        }

        private void ProcessMessage(string[] variants)
        {
            var contextResults = contextExtractor.Extract(variants);
            if (contextResults != null)
            {
                Console.WriteLine(JsonSerializer.Serialize(contextResults));
            }
            else
            {
                Console.WriteLine("No results found!");
            }
        }
    }
}
