
using System.Text.RegularExpressions;

namespace Evaluation.Model
{
    public class Airplane
    {
        //public int? AirplaneICAO { get; set; }
        //public int? BuildYear { get; set; }
        public Flight Flight { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
    }

    public class Flight
    {
        public long? ActualDepartureTime { get; set; }
        public Airline Airline { get; set; }
        public string ArrivalAirport { get; set; }
        public string DepartureAirport { get; set; }
        //public long? EstimatedArrivalTime { get; set; }
        public string FlightIdentification { get; set; }
        //public long? LastChangeTimestamp { get; set; }
        //public long? ScheduledArrivalTime { get; set; }
        //public long? ScheduledDepartureTime { get; set; }

        public string GetFlightNumber()
        {
            if (FlightIdentification != null)
            {
                return Regex.Replace(FlightIdentification, "^[a-zA-Z]{2,3}", "");
            }
            return null;
        }

    }

    public class Airline
    {
        public string AirlineIATA { get; set; }
        public string AirlineICAO { get; set; }
        public string Callsign { get; set; }
        public AirlineCountry Country { get; set; }
        public string Name { get; set; }
    }

    public class AirlineCountry
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
