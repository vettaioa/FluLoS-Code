using DeltaListReplacer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Pipeline
{
    class DeltaListReplaceCaller
    {
        private DeltaReplacer replacer;

        public DeltaListReplaceCaller()
        {
            var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\data\deltalist");

            var phonetics = loadFromJsonFile<Dictionary<string, string>>(Path.Combine(dataFolder, "phonetics.json"));

            var atcWords = loadFromJsonFile<List<string>>(Path.Combine(dataFolder, "atcwords.json"));
            var deltalist = loadFromJsonFile<Dictionary<string, string>>(Path.Combine(dataFolder, "deltalist.json"));

            replacer = new DeltaReplacer(phonetics, atcWords, deltalist);
        }

        private static T loadFromJsonFile<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
        }

        public string Call(string input)
        {
            return replacer.Replace(input);
        }
    }
}
