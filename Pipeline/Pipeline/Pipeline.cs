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

        public Pipeline(Configuration config)
        {
            this.config = config;
            cleanUp = new CleanUpCaller();
            speechToText = new SpeechToTextRunner(config.SpeechToText);
            deltaListReplace = new DeltaListReplaceCaller();
            rml = new RmlCaller();
        }

        public async Task Run()
        {
            speechToText.MessageRecognized += ProcessMessage;

            await speechToText.Run();
        }

        private void ProcessMessage(string[] transcriptions)
        {
            foreach(string transcription in transcriptions)
            {
                Console.WriteLine();
                Console.WriteLine("Processing Message:");
                Console.WriteLine(transcription);

                var prepared = Prepare(transcription);
                Console.WriteLine(" ===> ");
                Console.WriteLine(prepared);

                var structured = Structure(prepared);
                Console.WriteLine(" ===> ");
                Console.WriteLine(structured);
            }
        }

        private string Prepare(string input)
        {
            var prepared = cleanUp.Call(input);

            prepared = deltaListReplace.Call(prepared);

            return prepared;
        }

        private string Structure(string input)
        {
            return rml.Call(input);
        }
    }
}
