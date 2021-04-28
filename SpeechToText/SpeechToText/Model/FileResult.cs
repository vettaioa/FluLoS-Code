using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToText.Model
{
    public class FileResult
    {
        public string FilePath { get; set; }
        public string[] Transcriptions { get; set; }
    }
}
