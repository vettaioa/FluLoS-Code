using Pipeline.Model;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pipeline
{
    class Program
    {
        private const string CONFIG_FILE = "configuration.json";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Loading App Configuration...");
            if(File.Exists(CONFIG_FILE))
            {
                Configuration config = null;
                try
                {
                    config = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(CONFIG_FILE));
                }
                catch { }

                if (config == null)
                {
                    Console.WriteLine("Failed to load app config!");
                }
                else
                {
                    Console.WriteLine("Initializing Pipeline...");
                    var pipeline = new Pipeline(config);

                    Console.WriteLine("Starting Pipeline Process...");
                    await pipeline.Run();

                    Console.WriteLine("Pipeline stopped");
                }
            }
            else
            {
                Console.WriteLine("App Configuration not found!");
            }
        }
    }
}
