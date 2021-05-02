using Evaluation;
using Pipeline.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline
{
    class ContextEvaluator
    {
        private Evaluator evaluator;

        public ContextEvaluator(EvaluationConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            else
            {
                evaluator = new Evaluator(new Evaluation.Model.EvaluatorConfig()
                {
                    AirplaneDetailsUrl = config.AirplaneDetailsUrl,
                    AirplanesInRangeUrl = config.AirplanesInRangeUrl,
                    ContactFrequencies = config.ContactFrequencies,
                    ContactPlaces = config.ContactPlaces,
                    FlightLevelMax = config.FlightLevelMax,
                    FlightLevelMin = config.FlightLevelMin
                });
            }
        }

        public void Evaluate(MessageContext context)
        {
            // TODO
            evaluator.Evaluate();
        }
    }
}
