using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Model
{
    class ContextExtractionResult
    {
        public string Transcription { get; set; }
        public MessageContext RmlResult { get; set; }
        public MessageContext LuisResult { get; set; }

        public ContextExtractionResult(string transcription, MessageContext rml, MessageContext luis)
        {
            Transcription = transcription;
            RmlResult = rml;
            LuisResult = luis;
        }
    }
}
