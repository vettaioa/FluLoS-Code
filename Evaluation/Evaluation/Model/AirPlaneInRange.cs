
namespace Evaluation.Model
{
    public class AirPlaneInRange
    {
        public int Id { get; set; }
        public AirPlanePosition Position { get; set; }
    }

    public class AirPlanePosition
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
