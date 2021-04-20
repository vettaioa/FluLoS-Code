using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    delegate void MessageHandler(string message); // beside recognized literal message add metadata (e.g. word alternatives)

    class SpeechToTextRunner
    {
        public event MessageHandler MessageRecognized;

        public SpeechToTextRunner()
        {
            // TODO: SetUp
        }

        public void Run()
        {
            new Thread(() =>
            {
                while (true)
                {
                    // TODO: Replace with actual recognition code :D
                    MessageRecognized?.Invoke("lufthansa four seven two three resume own navigation to willisau");
                    Thread.Sleep(2000);
                    MessageRecognized?.Invoke("berlin five zero seven three bonjour maintain flight level three hundred via ST prex willisau");
                    Thread.Sleep(9000);
                }
            }).Start();
        }
    }
}
