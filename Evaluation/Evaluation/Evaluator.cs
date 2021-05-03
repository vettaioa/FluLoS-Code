using Evaluation.Model;
using SharedModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Evaluation
{
    public class Evaluator
    {

        public Evaluator(EvaluationConfig config)
        {
            // TODO
        }

        public EvaluationResult Evaluate(MessageContext messageContext)
        {
            EvaluationResult result = EvaluationResult.Error;

            // TODO: get airplanes in range

            // TODO: evaluate if is possible

            return result;
        }

        private AirplaneInRange[] GetAirplanesInRange()
        {
            AirplaneInRange[] airPlanes = null;
            // Returns null if failes, or empty array if no airplanes in range

            try
            {
                // TODO
            }
            catch { }

            return airPlanes;
        }
    }
}
