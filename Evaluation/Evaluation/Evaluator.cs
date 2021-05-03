using Evaluation.Model;
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
        private EvaluatorConfig config;

        public Evaluator(EvaluatorConfig config)
        {
            this.config = config;
        }

        public EvaluationResult Evaluate()
        {
            EvaluationResult result = EvaluationResult.Error;

            // TODO: get airplanes in range


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
