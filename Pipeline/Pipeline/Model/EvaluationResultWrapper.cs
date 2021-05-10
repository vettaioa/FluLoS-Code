using Evaluation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Model
{
    public class EvaluationResultWrapper
    {
        public EvaluationResult LuisEvaluation { get; set; }
        public EvaluationResult RmlEvaluation { get; set; }
    }
}
