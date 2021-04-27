using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Model
{
    class ContextExtractionResult
    {
        public MessageContext RmlResult { get; set; }
        public MessageContext LuisResult { get; set; }

        public ContextExtractionResult(MessageContext rml, MessageContext luis)
        {
            RmlResult = rml;
            LuisResult = luis;
        }
    }
}
