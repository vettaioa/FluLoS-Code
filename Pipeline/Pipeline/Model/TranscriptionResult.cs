using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Model
{
    class TranscriptionResult
    {
        public string FilePath { get; set; }
        public string[] Transcriptions { get; set; }
    }
}
