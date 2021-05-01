using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int Speed { get; set; }
        public string Squawk { get; set; }
        public long Timestamp { get; set; }
        public int VerticalRate { get; set; }
    }
}
