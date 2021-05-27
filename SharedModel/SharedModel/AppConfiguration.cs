using SharedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedModel
{
    public class AppConfiguration
    {
        public bool RunWebPipeline { get; set; }
        public SpeechToTextConfig SpeechToText { get; set; }
        public LuisConfig Luis { get; set; }
        public RmlConfig Rml { get; set; }
        public string InputLabelDirectory { get; set; }
        public string ContextOutputDirectory { get; set; }
        public EvaluationConfig Evaluation { get; set; }
    }

    public class SpeechToTextConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SpeechToTextMode SpeechToTextMode { get; set; }
        public string AzureApiKeysFile { get; set; }
        public string AzureRegion { get; set; }
        public string InputAudioFile { get; set; }
        public string InputAudioDirectory { get; set; }
        public string InputTranscriptionDirectory { get; set; }
        public string OutputNBestDirectory { get; set; }
    }

    public class LuisConfig
    {
        public string ApiUrl { get; set; }
        public string PublishedSlot { get; set; }
        public string AzureApiKeysFile { get; set; }
    }

    public class RmlConfig
    {
        public string RmlFile { get; set; }
        public string ExtCallDllDirectory { get; set; }
        public string PhoneticsFile { get; set; }
        public string AirlinesFile { get; set; }
    }

    public class EvaluationConfig
    {
        public bool RunEvaluation { get; set; }
        public string FlagsOutputDirectory { get; set; }
        public string MergedOutputDirectory { get; set; }
        public string AirplanesInRangeUrl { get; set; }
        public string AirplaneDetailsUrl { get; set; }
        public short[] LatitudeMinMax { get; set; }
        public short[] LongitudeMinMax { get; set; }

        public string PhoneticsFile { get; set; }
        public string AirlinesFile { get; set; }

        public EvaluationRules Rules { get; set; }
    }

    public class EvaluationRules
    {
        public short? FlightLevelMin { get; set; }
        public short? FlightLevelMax { get; set; }
        public int[] ContactFrequencies { get; set; }
        public string[] ContactPlaces { get; set; }
        public string[] TurnPlaces { get; set; }
        public string[] SquawkCodes { get; set; }
    }

    public enum SpeechToTextMode
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
        /// Folder of hand labelled data
        /// </summary>
        LabelledData,

        /// <summary>
        /// Folder of existing s2t transcriptions
        /// </summary>
        ExistingTranscriptions
    }
}
