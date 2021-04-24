using Pipeline.Model;
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
        private HttpListener listener;
        private bool running = true;

        public PipelineWebEndpoint(Configuration config)
        {
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
                        HandleSpeechInput(context);
                        break;
                }
                context.Response.Close();
            }
            
        }

        public void Stop()
        {
            running = false;
            listener.Stop();
        }

        private static void HandleIndexPage(HttpListenerContext context)
        {
            context.Response.ContentType = "text/html";

            var file = File.OpenRead(@"WebUI\index.html");
            file.CopyTo(context.Response.OutputStream);
        }
        private static void HandleSpeechInput(HttpListenerContext context)
        {
            Console.WriteLine(context.Request.ContentType);

            //context.Request.InputStream
            // containing the audio stram in audio/webm;codecs=opus

            // the following example code produces a working .webm audio file
            //var file = File.OpenWrite(@"audio.webm");
            //context.Request.InputStream.CopyTo(file);

            var data = new string[] { "Recognized Text:" };

            using StreamWriter writer = new StreamWriter(context.Response.OutputStream);
            writer.Write(JsonSerializer.Serialize(data));
        }
    }
}
