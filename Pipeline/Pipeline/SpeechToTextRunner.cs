using Pipeline.Model;
using SpeechToText;
using SpeechToText.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    delegate void TranscriptionHandler(TranscriptionResult result);     // possible (nbest) results

    class SpeechToTextRunner
    {
        public event TranscriptionHandler SpeechTranscribed;

        private SpeechTranscriber transcriber;
        private SpeechToTextConfig config;

        public SpeechToTextRunner(SpeechToTextConfig config)
        {
            this.config = config;
            transcriber = new SpeechTranscriber(config.AzureRegion, config.AzureApiKeysFile);
        }

        public async Task Run()
        {
            string[] micResults = null;
            FileResult[] fileResults = null;

            switch (config.SpeechToTextMode)
            {
                case SpeechToTextMode.MicrophoneSingle:
                    micResults = await transcriber.TranscribeMicrophone();
                    break;
                case SpeechToTextMode.FileSingle:
                    FileResult singleFileResult = await transcriber.TranscribeAudioFile(config.InputAudioFile);
                    if (singleFileResult != null)
                        fileResults = new FileResult[] { singleFileResult };
                    break;
                case SpeechToTextMode.FileMulti:
                case SpeechToTextMode.LabelledData:
                    fileResults = await transcriber.TranscribeAudioDirectory(config.InputAudioDirectory);
                    break;
            }

            if(micResults != null)
            {
                TranscriptionResult result = new TranscriptionResult { Transcriptions = micResults };
                SpeechTranscribed?.Invoke(result);
            }
            else if (fileResults != null && fileResults.Length > 0)
            {
                foreach(FileResult fileResult in fileResults)
                {
                    TranscriptionResult result = new TranscriptionResult
                    {
                        Transcriptions = fileResult.Transcriptions,
                        FilePath = fileResult.FilePath
                    };
                    SpeechTranscribed?.Invoke(result);
                }
            }
        }
    }
}
