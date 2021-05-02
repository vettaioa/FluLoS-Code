using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Model
{
    [Flags]
    public enum EvaluationResult
    {
        Error = 0,
        Invalid = 1,
        Valid = 2,
        NotInAirspace = 4,
        InvalidCallSign = 8,
        InvalidIntent = 16
    }
}
