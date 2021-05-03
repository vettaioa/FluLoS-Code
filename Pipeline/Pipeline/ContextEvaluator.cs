using Evaluation;
using Evaluation.Model;
using Pipeline.Model;
using SharedModel;
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
                evaluator = new Evaluator(config);
            }
        }

        public async Task<EvaluationResult> Evaluate(MessageContext context)
        {
            return await evaluator.Evaluate(context);
        }
    }
}
