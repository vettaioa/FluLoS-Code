//using Iib.RegexMarkupLanguage;
using System.Configuration;
using System.IO;

namespace Pipeline
{
    class RmlCaller
    {
        //private Rml rmlRegex;
        public RmlCaller()
        {
            var root = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\");
            var rmlDataFolder = Path.Combine(root, @"data\rml\");
            var rmlDefinition = Path.Combine(root, @"RML\atc.rml");

            ConfigurationManager.AppSettings["dataDir"] = rmlDataFolder;
            ConfigurationManager.AppSettings["dllDir"] = Path.Combine(root, @"RML\ExternalCallDll\bin\Debug\");
            ConfigurationManager.AppSettings["phonetics"] = Path.Combine(rmlDataFolder, "phonetic.xml");

            //IEnumerable<CompilerException> errors;
            //rmlRegex = new Rml(rmlDefinition, out errors);

            //if(errors.Count() > 0)
            //{
            //    throw new ApplicationException("RML Compile Error");
            //}

        }

        public string Call(string input)
        {
            //var res = rmlRegex.Execute(input);
            //return res.ToString();
            return input;
        }
    }
}
