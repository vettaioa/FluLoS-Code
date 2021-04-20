using System;

namespace Pipeline
{
    class Pipeline
    {
        private SpeechToTextRunner speechToText;
        private CleanUpCaller cleanUp;
        private DeltaListReplaceCaller deltaListReplace;
        private RmlCaller rml;
        

        public Pipeline()
        {
            cleanUp = new CleanUpCaller();
            speechToText = new SpeechToTextRunner();
            deltaListReplace = new DeltaListReplaceCaller();
            rml = new RmlCaller();
        }

        public void Run()
        {
            speechToText.MessageRecognized += ProcessMessage;

            speechToText.Run(); // STT is running in own thread
        }

        private void ProcessMessage(string message)
        {
            Console.WriteLine();
            Console.WriteLine("Processing Message:");
            Console.WriteLine(message);

            var prepared = Prepare(message);
            Console.WriteLine(" ===> ");
            Console.WriteLine(prepared);

            var structured = Structure(prepared);
            Console.WriteLine(" ===> ");
            Console.WriteLine(structured);
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
