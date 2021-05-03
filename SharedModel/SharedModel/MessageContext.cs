using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel
{
    public class MessageContext
    {
        public string Message { get; set; }
        public CallSign CallSign { get; set; }
        public Dictionary<IntentType, MessageIntent> Intents { get; set; }
    }

    public class CallSign
    {
        public string Airline { get; set; }
        public string FlightNumber { get; set; }
    }

    public abstract class MessageIntent
    {
        public float Score { get; set; }
    }

    public class FlightLevelIntent : MessageIntent
    {
        public FlightLevelInstruction? Instruction { get; set; }
        public string Level { get; set; }
        public enum FlightLevelInstruction
        {
            Maintain,
            Climb,
            Descend
        }
    }

    public class TurnIntent : MessageIntent
    {
        public string Direction { get; set; }
        public string Degrees { get; set; }
        public string Heading { get; set; }
        public string Place { get; set; }
    }

    public class ContactIntent : MessageIntent
    {
        public string Frequency { get; set; }
        public string Place { get; set; }
    }

    public class SquawkIntent : MessageIntent
    {
        public string Code { get; set; }
    }

    public class NoneIntent : MessageIntent
    {
    }
}
