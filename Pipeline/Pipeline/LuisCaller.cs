﻿using LUIS;
using LUIS.Model;
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

        public LuisResult Call(string input)
        {
            return interpreter.Interpret(input);
        }
    }
}