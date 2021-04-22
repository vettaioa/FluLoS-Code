using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUIS.Model
{
    public class LuisJsonResponse
    {
        public string query { get; set; }
        public Prediction prediction { get; set; }
    }

    public class Prediction
    {
        public string topIntent { get; set; }
        public Intents intents { get; set; }
        public Entities entities { get; set; }
    }

    public class Intents
    {
        public IntentScore Turn { get; set; }
        public IntentScore FlightLevel { get; set; }
        public IntentScore None { get; set; }
        public IntentScore Contact { get; set; }
        public IntentScore Squawk { get; set; }
    }

    public class IntentScore
    {
        public double score { get; set; }
    }

    public class Entities
    {
        // Machine Learned
        public IList<MLCallSign> MLCallSign { get; set; }
        public IList<MLContact> MLContact { get; set; }
        public IList<MLFlightLevel> MLFlightLevel { get; set; }
        public IList<MLSquawk> MLSquawk { get; set; }
        public IList<MLTurn> MLTurn { get; set; }

        // Regex
        public IList<string> RegexTurnDegrees { get; set; }
        public IList<string> RegexFlightLevel { get; set; }
        public IList<string> RegexSquawkNr { get; set; }
        public IList<string> RegexTurnHeading { get; set; }
        public IList<string> RegexTurnTo { get; set; }

        // Lists
        public IList<List<string>> ListAirlines { get; set; }

        // Prebuilt
        public IList<string> geographyV2 { get; set; }

    }

    public class MLCallSign
    {
        public IList<string> Airline { get; set; }
        public IList<string> FlightNr { get; set; }
    }

    public class MLContact
    {
        public IList<string> Frequency { get; set; }
        public IList<string> Place { get; set; }
    }

    public class MLFlightLevel
    {
        public IList<string> Level { get; set; }
        public IList<string> Instruction { get; set; }
    }

    public class MLSquawk
    {
        public IList<string> Code { get; set; }
    }

    public class MLTurn
    {
        public IList<string> Direction { get; set; }
        public IList<MLSubSubEntity> Degrees { get; set; }
        public IList<MLSubSubEntity> Heading { get; set; }
        public IList<MLSubSubEntity> Place { get; set; }
    }

    public class MLSubSubEntity
    {
        public IList<string> Value { get; set; }
    }
}
