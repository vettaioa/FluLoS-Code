using Evaluation.Model;
using Pipeline.Model;
using SharedModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline.WebUI
{
    delegate void AudioInputHandler(MemoryStream wavAudioStream, string uid);

    class PipelineWebEndpoint
    {
        public event AudioInputHandler AudioInputReceived;

        private HttpListener listener;
        private bool running = true;

        private static SpeechToTextConfig speechToTextConfig;
        private static ContextExtractor contextExtractor;
        private static HttpListenerContext httpListenerContext;

        private IEnumerable<RadarAirplane> radarAirplanes;

        private IDictionary<string, PipelineResult> pipelineResults = new ConcurrentDictionary<string, PipelineResult>();

        public PipelineWebEndpoint(AppConfiguration config, IEnumerable<RadarAirplane> radarAirplanes)
        {
            speechToTextConfig = config.SpeechToText;
            speechToTextConfig.SpeechToTextMode = SpeechToTextMode.FileSingle;
            contextExtractor = new ContextExtractor(config);

            listener = new HttpListener();
            listener.Prefixes.Add("http://+:8080/");
            //listener.Prefixes.Add("https://*:8081/");

            this.radarAirplanes = radarAirplanes;
        }

        public async Task Run()
        {
            listener.Start();
            Console.WriteLine("Listening for HTTP requests");

            while (running)
            {

                var context = await listener.GetContextAsync();

                var path = context.Request.Url.AbsolutePath.Trim('/');
                switch (path)
                {
                    case "":
                        HandleStaticFile(context, "index.html");
                        break;
                    case "index.html":
                    case "app.js":
                    case "recorder.js":
                        HandleStaticFile(context, path);
                        break;
                    case "airspace":
                        await HandleAirspaceAsync(context);
                        break;
                    case "process":
                        HandleSpeechInputAsync(context);
                        break;
                    case "output":
                        await HandleOutputAsync(context);
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                }

                context.Response.Close();
            }
            listener.Close();
        }

        public void Stop()
        {
            running = false;
            listener.Stop();
            Console.WriteLine("Stopped listening for HTTP requests");
        }


        public void PipelineOutputReceived(PipelineOutputType outputType, string uid, string jsonData)
        {
            if (!pipelineResults.ContainsKey(uid))
            {
                pipelineResults[uid] = new PipelineResult { uid = uid };
            }

            var result = pipelineResults[uid];
            switch (outputType)
            {
                case PipelineOutputType.TRANSCRIPTIONS:
                    result.transcriptionsJson = jsonData;
                    break;
                case PipelineOutputType.CONTEXTS:
                    result.contextsJson = jsonData;
                    break;
                case PipelineOutputType.EVALUATIONFLAGS:
                    result.evaluationflagsJson = jsonData;
                    break;
                case PipelineOutputType.VALIDATEDMERGED:
                    result.evaluationflagsJson = jsonData;
                    break;
            }
        }

        private static void HandleStaticFile(HttpListenerContext context, string fileName)
        {
            string path = $"WebUI\\{fileName}";

            string mimeType = "text/plain";
            switch (Path.GetExtension(path))
            {
                case ".html":
                    mimeType = "text/html";
                    break;
                case ".js":
                    mimeType = "application/javascript";
                    break;
                case ".css":
                    mimeType = "text/css";
                    break;
            }
            context.Response.ContentType = mimeType;
            //context.Response.ContentLength64 = (new FileInfo(path)).Length;

            try
            {
                using (var file = File.OpenRead(path))
                {
                    context.Response.ContentLength64 = file.Length;
                    file.CopyTo(context.Response.OutputStream);
                }
                context.Response.OutputStream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to read Static File", e);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private async Task HandleAirspaceAsync(HttpListenerContext context)
        {
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.OutputStream, new { airplanes = radarAirplanes });

        }

        private void HandleSpeechInputAsync(HttpListenerContext context)
        {
            httpListenerContext = context;
            if (context.Request != null)
            {
                Console.WriteLine("Request received");
                var uid = Guid.NewGuid().ToString();


                var memoryStream = new MemoryStream();

                using (var inputStream = context.Request.InputStream)
                {
                    inputStream.CopyTo(memoryStream);
                }

                AudioInputReceived?.Invoke(memoryStream, uid);

                byte[] buffUid = Encoding.UTF8.GetBytes(uid);
                context.Response.ContentType = "text/plain";
                context.Response.OutputStream.Write(buffUid, 0, buffUid.Length);


            }
        }

        private async Task HandleOutputAsync(HttpListenerContext context)
        {
            string uid = context.Request.QueryString["uid"];

            PipelineResult result = pipelineResults[uid];

            if (result.IsComplete())
            {
                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.OutputStream, result);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // request too early
            }
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

    internal class PipelineResult
    {
        public string uid { get; set; }
        public string transcriptionsJson { get; set; }
        public string contextsJson { get; set; }
        public string evaluationflagsJson { get; set; }
        public string validatedmergedJson { get; set; }

        public bool IsComplete()
        {
            return transcriptionsJson != null && contextsJson != null && evaluationflagsJson != null && validatedmergedJson != null;
        }
    }
}
