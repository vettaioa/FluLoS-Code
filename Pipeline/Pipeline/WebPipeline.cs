using Evaluation;
using Evaluation.Model;
using Pipeline.WebUI;
using SharedModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pipeline
{
    class WebPipeline : Pipeline
    {
        private PipelineWebEndpoint webEndpoint;

        public WebPipeline(AppConfiguration config): base(config)
        {
            Console.WriteLine("Initializing Web UI Endpoint");

            var airplanes = GetRadarAirplanes().GetAwaiter().GetResult();
            webEndpoint = new PipelineWebEndpoint(config, airplanes);

            // TODO: call Run() method eg. on webEndpoint event
        }

        public void StartWebEndpoint()
        {
            webEndpoint.Run();
        }

        public void StopWebEndpoint()
        {
            webEndpoint.Stop();
        }


        protected override void WriteToOutput(PipelineOutputType outputType, string fileName, string jsonData)
        {
            webEndpoint.PipelineOutputReceived(outputType, fileName, jsonData);
            base.WriteToOutput(outputType, fileName, jsonData); // output as file
        }

        private async Task<IEnumerable<RadarAirplane>> GetRadarAirplanes() // Workaround to get the current Airspace for the Web Demo
        {
            IEnumerable<RadarAirplane> radarAirplanes = null;
            if (config.Evaluation.UseMockedAirspace)
            {
                // use a mocked airspace from predefined json
                radarAirplanes = JsonSerializer.Deserialize<IEnumerable<RadarAirplane>>(File.ReadAllText(config.Evaluation.MockedAirspaceFile));
            }
            else
            {
                // call radar to get current airspace
                var radarScanner = new RadarScanner(config.Evaluation.AirplanesInRangeUrl, config.Evaluation.AirplaneDetailsUrl);
                radarAirplanes = await radarScanner.GetRadarAirplanes(config.Evaluation.LatitudeMinMax[0], config.Evaluation.LatitudeMinMax[1],
                                                                          config.Evaluation.LongitudeMinMax[0], config.Evaluation.LongitudeMinMax[1]);
            }

            return radarAirplanes;
        }
    }
}
