using Pipeline.Model;
using SharedModel;
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
        private string labelDataDir;

        public SpeechToTextRunner(SpeechToTextConfig config, string labelDataDir = null)
        {
            this.config = config;
            this.labelDataDir = labelDataDir;
            transcriber = new SpeechTranscriber(config);
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
                    fileResults = await transcriber.TranscribeAudioDirectory(config.InputAudioDirectory);
                    break;
                case SpeechToTextMode.LabelledData:
                    string[] filesToTranscribe = GetAudioFilesForLabelData(labelDataDir);
                    fileResults = await transcriber.TranscribeAudioFiles(filesToTranscribe);
                    break;
                case SpeechToTextMode.ExistingTranscriptions:
                    fileResults = ReadTranscriptionFiles(config.InputTranscriptionDirectory);
                    break;
            }

            if (micResults != null)
            {
                TranscriptionResult result = new TranscriptionResult { Transcriptions = micResults };
                SpeechTranscribed?.Invoke(result);
            }
            else if (fileResults != null && fileResults.Length > 0)
            {
                foreach (FileResult fileResult in fileResults)
                {
                    if(fileResult != null)
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

        private string[] GetAudioFilesForLabelData(string labelDataDir)
        {
            string[] audioFiles = null;         // audio files for each label file
            string[] labelDataFiles = null;     // label files

            try
            {
                labelDataFiles = Directory.GetFiles(labelDataDir);
            }
            catch (Exception ex) when (ex is IOException || ex is SystemException)
            {
                Console.WriteLine($"Error getting files from directory {labelDataDir}");
            }

            if (labelDataFiles != null && labelDataFiles.Length > 0)
            {
                audioFiles = new string[labelDataFiles.Length];
                for (int i = 0; i < labelDataFiles.Length; i++)
                {
                    string fileNameOnly = Path.GetFileNameWithoutExtension(labelDataFiles[i]);
                    audioFiles[i] = config.InputAudioDirectory + fileNameOnly + ".wav";
                }
            }

            return audioFiles;
        }

        private FileResult[] ReadTranscriptionFiles(string directory)
        {
            FileResult[] results = null;
            string[] files = null;
            try
            {
                files = Directory.GetFiles(directory);
            }
            catch { }

            if (files != null && files.Length > 0)
            {
                results = new FileResult[files.Length];

                for(int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        string currentTranscription = File.ReadAllText(files[i]);
                        FileResult currentResult = new FileResult()
                        {
                            FilePath = files[i],
                            Transcriptions = new string[] { currentTranscription }
                        };
                        results[i] = currentResult;
                    }
                    catch { }
                }
            }

            return results;
        }
    }
}
