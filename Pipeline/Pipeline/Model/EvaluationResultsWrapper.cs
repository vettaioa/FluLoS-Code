using Evaluation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Model
{
    class EvaluationResultsWrapper
    {
        public EvaluationResult[] LuisEvaluations { get; set; }
        public EvaluationResult[] RmlEvaluations { get; set; }

        public EvaluationResultsWrapper(EvaluationResult[] luisEvaluations, EvaluationResult[] rmlEvaluations)
        {
            LuisEvaluations = luisEvaluations;
            RmlEvaluations = rmlEvaluations;
        }
    }
}
