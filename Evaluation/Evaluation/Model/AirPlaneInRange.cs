
namespace Evaluation.Model
{
    class AirplaneInRange
    {
        public int Id { get; set; }
        public AirplanePosition Position { get; set; }
    }

    public class AirplanePosition
    {
        public int Altitude { get; set; }
        public double Heading { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool OnGround { get; set; }
        public string PointType { get; set; }
        public double Speed { get; set; }
        public string Squawk { get; set; }
        public long Timestamp { get; set; }
        public double VerticalRate { get; set; }
    }
}
