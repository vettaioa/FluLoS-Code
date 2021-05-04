using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Model
{
    public class EvaluationResult
    {
        public RadarAirplane RadarAirplane { get; set; }
        public SquawkValidationResult SquawkResult { get; set; }
        public ContactValidationResult ContactResult { get; set; }
        public FlightLevelValidationResult FlightLevelResult { get; set; }
        public TurnValidationResult TurnResult { get; set; }
    }

    [Flags]
    public enum SquawkValidationResult
    {
        Invalid = 0,
        CodeValid = 1
    }

    [Flags]
    public enum ContactValidationResult
    {
        Invalid = 0,
        FrequencyValid = 1,
        PlaceValid = 2
    }

    [Flags]
    public enum FlightLevelValidationResult
    {
        Invalid = 0,
        FlightLevelValid = 1,
        InstructionValid = 2
    }

    [Flags]
    public enum TurnValidationResult
    {
        Invalid = 0,
        DegreesValid = 1,
        HeadingValid = 2,
        DirectionValid = 4,
        PlaceValid = 8
    }
}
