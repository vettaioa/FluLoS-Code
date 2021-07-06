using Evaluation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Evaluation
{
    public class RadarScanner
    {
        private readonly string getAirplanesInRangeUrl;
        private readonly string getAirplaneUrl;

        private readonly HttpClient client = new HttpClient();

        public RadarScanner(string getAirplanesInRangeUrl, string getAirplaneUrl)
        {
            this.getAirplanesInRangeUrl = getAirplanesInRangeUrl;
            this.getAirplaneUrl = getAirplaneUrl;
        }

        /// <summary>
        /// Returns all known Airplanes in the requested Airspace
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<RadarAirplane>> GetRadarAirplanes(short minLatitude, short maxLatitude, short minLongitude, short maxLongitude)
        {
            try
            {
                var airplanesInRange = await GetAirplanesInRange(minLatitude, maxLatitude, minLongitude, maxLongitude);

                return airplanesInRange
                        .Select(plane => (plane, GetAirplane(plane.Id)))
                        .Select(t => new RadarAirplane { Airplane = t.Item2.Result, Position = t.plane.Position });
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<AirplaneInRange>> GetAirplanesInRange(short minLatitude, short maxLatitude, short minLongitude, short maxLongitude)
        {
            var timeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var jsonStream = await client.GetStreamAsync($"{getAirplanesInRangeUrl}/{minLatitude}/{minLongitude}/{maxLatitude}/{maxLongitude}/0/{timeMillis}");

            return await JsonSerializer.DeserializeAsync<List<AirplaneInRange>>(jsonStream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new AirPlaneInRangeJsonConverter() }
            });
        }

        private async Task<Airplane> GetAirplane(int airplaneId)
        {
            var jsonStream = await client.GetStreamAsync($"{getAirplaneUrl}/{airplaneId}");

            return await JsonSerializer.DeserializeAsync<Airplane>(jsonStream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

    }
}
