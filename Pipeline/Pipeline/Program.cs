using System;

namespace Pipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Pipeline...");
            var pipeline = new Pipeline();

            Console.WriteLine("Starting Pipeline Process...");
            pipeline.Run();

            Console.WriteLine("Pipeline stopped");
        }
    }
}
