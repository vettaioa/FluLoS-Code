using LUIS;
using Pipeline.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline
{
    class LuisCaller
    {
        private UtteranceInterpreter interpreter;

        public LuisCaller(LuisConfig config)
        {
            interpreter = new UtteranceInterpreter(config.AzureApiKeysFile, config.ApiUrl, config.PublishedSlot);
        }

        public string Call(string input)
        {
            return interpreter.Interpret(input);
        }
    }
}
