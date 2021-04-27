using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pipeline.Model
{
    class Configuration
    {
        public bool RunWebPipeline { get; set; }
        public SpeechToTextConfig SpeechToText { get; set; }
        public LuisConfig Luis { get; set; }
        public string InputLabelDirectory { get; set; }
        public EvaluationConfig Evaluation { get; set; }
        // TODO: cleanup, deltalsit etc. params
    }

    class SpeechToTextConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SpeechToTextMode SpeechToTextMode { get; set; }
        public string AzureApiKeysFile { get; set; }
        public string AzureRegion { get; set; }
        public string InputAudioFile { get; set; }
        public string InputAudioDirectory { get; set; }
    }

    class LuisConfig
    {
        public string ApiUrl { get; set; }
        public string PublishedSlot { get; set; }
        public string AzureApiKeysFile { get; set; }
    }

    class EvaluationConfig
    {
        public string AirplanesInRangeUrl { get; set; }
        public string AirplaneDetailsUrl { get; set; }
    }

    enum SpeechToTextMode
    {
        /// <summary>
        /// Single utterance from microphone
        /// </summary>
        MicrophoneSingle,

        /// <summary>
        /// Single audio testfile
        /// </summary>
        FileSingle,

        /// <summary>
        /// Folder of audio testfiles
        /// </summary>
        FileMulti,

        /// <summary>
        /// Folder of son labeldata
        /// </summary>
        LabelledData
    }
}
