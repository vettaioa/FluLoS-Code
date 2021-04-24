using Iib.RegexMarkupLanguage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RML
{
  class TestProgram
  {
    public static void Main(string[] agrs)
    {
      var root = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\");
      var rmlDataFolder = Path.Combine(root, @"data\rml\");
      var rmlDefinition = Path.Combine(root, @"RML\atc.rml");

      ConfigurationManager.AppSettings["dataDir"] = rmlDataFolder;
      ConfigurationManager.AppSettings["dllDir"] = Path.Combine(root, @"RML\ExternalCallDll\bin\Debug\net5.0\");
      ConfigurationManager.AppSettings["phonetics"] = Path.Combine(rmlDataFolder, "phonetic.xml");

      IEnumerable<CompilerException> errors;
      Rml rmlRegex = new Rml(rmlDefinition, out errors);

      if (errors.Count() > 0)
      {
        foreach (var error in errors)
        {
          Console.WriteLine($"[ERROR] RML error: {error}");
        }
      }

      Console.WriteLine("Testing RML:");

      try
      {
        string[] inputs = {
          "luxair 551 too short on your way fly heading 180",
          "klm 376 climb to flight level 330",
          "iberia 4476 contact zurich on 125.85 good bye",
          "lufthansa 3556 turn left 99 degrees",
          "lufthansa 3556 rhein radar identified turn right by 15 degrees and climb flight level 290",
        };

        foreach (var input in inputs) {
          Console.WriteLine(rmlRegex.Execute(input).OuterXml);
          Console.WriteLine();
        }

      }
      catch (Exception e)
      {
        Console.WriteLine("Exception thrown:");
        Console.WriteLine(e);
      }
    }
  }
}
