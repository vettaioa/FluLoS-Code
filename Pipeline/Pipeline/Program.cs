﻿using Pipeline.Model;
using SharedModel;
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
                AppConfiguration config = null;
                try
                {
                    config = JsonSerializer.Deserialize<AppConfiguration>(File.ReadAllText(CONFIG_FILE));
                }
                catch { }

                if (config == null)
                {
                    Console.WriteLine("Failed to load app config!");
                }
                else
                {
                    if(config.RunWebPipeline)
                    {
                        Console.WriteLine("Initializing Web Pipeline...");
                        var pipeline = new WebPipeline(config);

                        Console.WriteLine("Starting Pipeline Web Endpoint...");
                        pipeline.StartWebEndpoint();

                        Console.ReadKey();
                        pipeline.StopWebEndpoint();
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
            }
            else
            {
                Console.WriteLine("App Configuration not found!");
            }
        }
    }
}
