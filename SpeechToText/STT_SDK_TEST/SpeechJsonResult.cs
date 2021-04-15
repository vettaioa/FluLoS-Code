using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STT_SDK_TEST
{
    public class SpeechJsonResult
    {
        public string DisplayText { get; set; }
        public int Duration { get; set; }
        public string Id { get; set; }
        public List<NBestItem> NBest { get; set; }
        public int Offset { get; set; }
        public string RecognitionStatus { get; set; }
    }
    public class NBestItem
    {
        public double Confidence { get; set; }
        public string Display { get; set; }
        public string ITN { get; set; }
        public string Lexical { get; set; }
        public string MaskedITN { get; set; }
        public List<WordsItem> Words { get; set; }
    }

    public class WordsItem
    {
        public double Confidence { get; set; }
        public string Word { get; set; }
    }
}
