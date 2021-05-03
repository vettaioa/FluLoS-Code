using Pipeline.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pipeline.WebUI
{
    class PipelineWebEndpoint
    {
        private const string REQUEST_MIME = "audio/webm;codecs=opus";
        private const string AUDIO_FILENAME = "audio.webm";

        private HttpListener listener;
        private bool running = true;

        private static SpeechToTextConfig speechToTextConfig;
        private static ContextExtractor contextExtractor;
        private static HttpListenerContext httpListenerContext;

        public PipelineWebEndpoint(AppConfiguration config)
        {
            speechToTextConfig = config.SpeechToText;
            speechToTextConfig.SpeechToTextMode = SpeechToTextMode.FileSingle;
            contextExtractor = new ContextExtractor(config);

            listener = new HttpListener();
            listener.Prefixes.Add("http://+:8080/");
            //listener.Prefixes.Add("https://*:8081/");
        }

        public async Task Run()
        {
            listener.Start();
            Console.WriteLine("Listening for HTTP requests");

            while(running)
            {
                var context = await listener.GetContextAsync();
                
                switch(context.Request.Url.AbsolutePath.Trim('/'))
                {
                    case "":
                    case "index.html":
                        HandleIndexPage(context);
                        break;
                    case "process":
                        await HandleSpeechInputAsync(context);
                        break;
                }
                context.Response.Close();
            }
            
        }

        public void Stop()
        {
            running = false;
            listener.Stop();
            Console.WriteLine("Stopped listening for HTTP requests");
        }

        private static void HandleIndexPage(HttpListenerContext context)
        {
            context.Response.ContentType = "text/html";

            var file = File.OpenRead(@"WebUI\index.html");
            file.CopyTo(context.Response.OutputStream);
        }

        private static async Task HandleSpeechInputAsync(HttpListenerContext context)
        {
            httpListenerContext = context;
            if (context.Request != null)
            {
                Console.WriteLine("Request received");
                if(context.Request.ContentType != null && context.Request.ContentType == REQUEST_MIME)
                {
                    Console.WriteLine("Processing audio...");
                    bool audioSaved = false;
                    try
                    {
                        using (var file = File.OpenWrite(AUDIO_FILENAME))
                        using (var stream = context.Request.InputStream)
                        {
                            stream.CopyTo(file);
                        }
                        audioSaved = true;
                    }
                    catch { }

                    if (audioSaved)
                    {
                        Console.WriteLine("Running Speech to Text...");
                        speechToTextConfig.InputAudioFile = AUDIO_FILENAME;
                        SpeechToTextRunner speechToText = new SpeechToTextRunner(speechToTextConfig);
                        speechToText.SpeechTranscribed += ProcessTranscriptions;
                        await speechToText.Run();
                    }
                    else
                    {
                        Console.WriteLine("Error saving audio to file!");
                    }
                }
                else
                {
                    Console.WriteLine("Wrong format, must be {0}", REQUEST_MIME);
                }
            }

            // context.Request.InputStream
            // containing the audio stram in audio/webm;codecs=opus

            // the following example code produces a working .webm audio file
            //var file = File.OpenWrite(@"audio.webm");
            //context.Request.InputStream.CopyTo(file);
        }

        private static void ProcessTranscriptions(TranscriptionResult transcriptionResult)
        {
            var contextResults = contextExtractor.Extract(transcriptionResult.Transcriptions);
            using StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream);
            if (contextResults != null && contextResults.Length > 0)
            {
                JsonSerializerOptions jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
                Console.WriteLine(JsonSerializer.Serialize(contextResults, jsonOptions));
            }
            else
            {
                writer.Write("No results found");
            }
        }
    }
}
