using Pipeline.Model;
using SpeechToText;
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
            string[] transcriptions = null;
            string[][] transcriptionsMultifile = null;

            switch (config.SpeechToTextMode)
            {
                case SpeechToTextMode.MicrophoneSingle:
                    transcriptions = await transcriber.TranscribeMicrophone();
                    break;
                case SpeechToTextMode.FileSingle:
                    transcriptions = await transcriber.TranscribeAudioFile(config.InputAudioFile);
                    break;
                case SpeechToTextMode.FileMulti:
                case SpeechToTextMode.LabelledData:
                    transcriptionsMultifile = await transcriber.TranscribeAudioDirectory(config.InputAudioDirectory);
                    break;
            }

            if (transcriptions != null)
            {
                TranscriptionResult result = new TranscriptionResult();
                result.Transcriptions = transcriptions;
                result.FilePath = config.InputAudioFile;

                SpeechTranscribed?.Invoke(result);
            }
            else if (transcriptionsMultifile != null && transcriptionsMultifile.Length > 0)
            {
                string[] dirFiles = null;
                try
                {
                    dirFiles = Directory.GetFiles(config.InputAudioDirectory);
                    Array.Sort(dirFiles);
                }
                catch { }

                for (int i = 0; i < transcriptionsMultifile.Length; i++)
                {
                    string[] currentTranscriptions = transcriptionsMultifile[i];

                    TranscriptionResult result = new TranscriptionResult();
                    result.Transcriptions = currentTranscriptions;
                    if (dirFiles != null && dirFiles.Length > i - 1)
                        result.FilePath = dirFiles[i];

                    SpeechTranscribed?.Invoke(result);
                }
            }
        }
    }
}
