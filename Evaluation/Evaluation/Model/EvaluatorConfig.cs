using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Model
{
    public class EvaluatorConfig
    {
        public string AirplanesInRangeUrl { get; set; }
        public string AirplaneDetailsUrl { get; set; }
        public short FlightLevelMin { get; set; }
        public short FlightLevelMax { get; set; }
        public short[] ContactFrequencies { get; set; }
        public string[] ContactPlaces { get; set; }
        public short[] SquawkCodes { get; set; }
    }
}
