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
                        HandleSpeechInput(context);
                        break;
                    case "output":
                        HandleOutput(context);
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


        public void PipelineOutputReceived(PipelineOutputType outputType, string fileName, string jsonData)
        {
            var uid = Path.GetFileNameWithoutExtension(fileName);
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
                    result.validatedmergedJson = jsonData;
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

        private void HandleSpeechInput(HttpListenerContext context)
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
                context.Response.ContentLength64 = buffUid.Length;
                using (var outputStream = context.Response.OutputStream)
                {
                    outputStream.Write(buffUid, 0, buffUid.Length);
                }


            }
        }

        private void HandleOutput(HttpListenerContext context)
        {
            string uid = context.Request.QueryString["uid"];
            string type = context.Request.QueryString["type"];

            if (uid == null || type == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // invalid request
                return;
            }
            if(!pipelineResults.ContainsKey(uid))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            PipelineResult pipelineResult = pipelineResults[uid];

            string result = pipelineResult.GetValueByTypeName(type);
            if (result == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Locked; // request too early
                return;
            }

            byte[] buffResult = Encoding.UTF8.GetBytes(result);
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffResult.Length;
            using (var outputStream = context.Response.OutputStream)
            {
                context.Response.OutputStream.Write(buffResult, 0, buffResult.Length);
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

        public string GetValueByTypeName(string type)
        {
            switch (type)
            {
                case "transcription":
                    return transcriptionsJson;
                case "context":
                    return contextsJson;
                case "evaluationflags":
                    return evaluationflagsJson;
                case "validatedmerged":
                    return validatedmergedJson;
            }
            return null;
        }

        //public bool IsComplete()
        //{
        //    return transcriptionsJson != null && contextsJson != null && evaluationflagsJson != null && validatedmergedJson != null;
        //}
    }
}
