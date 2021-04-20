using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml;

namespace DeltaListReplacer
{
    class TestProgram
    {
        static void Main(string[] args)
        {
            var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\data\deltalist");

            var phonetics = loadPhonetics(Path.Combine(dataFolder, "phonetics.json"));

            var atcWords = loadAtcWords(Path.Combine(dataFolder, "atcwords.json"));
            var deltalist = loadDeltalist(Path.Combine(dataFolder, "atcwords.json"));

            var replacer = new DeltaReplacer(phonetics, atcWords, deltalist);

            (new List<string> {
                replacer.Replace("butter alitalia 110 clim to fight level 320"),
                "roger alitalia 110 climb to flight level 320  [expected]",
                "---",
                replacer.Replace("sabena 376 contract reims on 134.4 good bye"),
                "sabena 376 contact reims on 134.4 good bye  [expected]",
                "---",
                replacer.Replace("lufthansa 8221 decent flight devil 290"),
                "lufthansa 8221 descend flight level 290  [expected]",
            }).ForEach(s => Console.WriteLine(s));

        }

        private static Dictionary<string, string> loadPhonetics(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
        }

        private static List<string> loadAtcWords(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<string>>(json, new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
        }

        private static Dictionary<string, string> loadDeltalist(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
        }
    }
}
