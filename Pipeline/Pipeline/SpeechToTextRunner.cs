using Pipeline.Model;
using SpeechToText;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    delegate void MessageHandler(string[] variants);     // possible (nbest) results

    class SpeechToTextRunner
    {
        public event MessageHandler MessageRecognized;

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
            string[,] transcriptionsMultifile = null;

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
                MessageRecognized?.Invoke(transcriptions);
            }
            else if (transcriptionsMultifile != null && transcriptionsMultifile.Length > 0)
            {
                for (int i = 0; i < transcriptionsMultifile.Length; i++)
                {
                    // select specific row i from the array
                    string[] currentTranscriptions = Enumerable.Range(0, transcriptionsMultifile.GetLength(1))
                                                                .Select(x => transcriptionsMultifile[i, x])
                                                                .ToArray();
                    MessageRecognized?.Invoke(currentTranscriptions);
                }
            }
        }
    }
}
