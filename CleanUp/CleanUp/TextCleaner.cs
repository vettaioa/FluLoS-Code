using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cleanup
{
    /// <summary>
    /// Wrapper for the text_cleanup Python script
    /// </summary>
    public static class TextCleaner
    {

        public static string Clean(string text)
        {
            return Clean(new string[] { text })[0];
        }

        public static string[] Clean(params string[] texts)
        {
            string text = string.Join("||", texts);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python.exe";
            start.Arguments = $"text_cleanup_main.py \"{text}\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            try
            {
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        text = reader.ReadToEnd();
                        text = text.Replace(System.Environment.NewLine, "");
                    }
                }

                texts = text.Split("||")[..^1]; // last element is empty
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Execution of python script failed. Is Python 3+ installed and available in path?");
                Console.WriteLine(e);
            }
            

            return texts;
        }
    }
}
