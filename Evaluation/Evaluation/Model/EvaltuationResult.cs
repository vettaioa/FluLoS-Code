using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Model
{
    /// <summary>
    /// Flags for evaluation result -> may contain multiple set fields
    /// </summary>
    [Flags]
    public enum EvaluationResult
    {
        Error = 0,
        Invalid = 1,
        Valid = 2,
        NoIntent = 4,
        NotInAirspace = 8,
        InvalidCallSign = 16,
        InvalidSquawkCode = 32,
        InvalidContactInfo = 64,
        InvalidTurnInfo = 128,
        InvalidFlightLevel = 256
    }
}
