using Cleanup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline
{
    class CleanUpCaller
    {
        public CleanUpCaller()
        {
            
        }

        public string Call(string input)
        {
            return TextCleaner.Clean(input);
        }
    }
}
