using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DeltaListReplacer
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\data\deltalist");
            var phonetics = loadPhonetics(Path.Combine(dataFolder, "phonetic.xml"));

            var atcWords = dummyAtcWords();
            var deltalist = dummyDeltaList();

            var replacer = new DeltaReplacer(phonetics, atcWords, deltalist);

            (new List<string> {
                replacer.Replace("roger alitalia 110 climb to flight level 320"),
                replacer.Replace("butter alitalia 110 clim to fight level 320"),
                "---",
                replacer.Replace("sabena 376 contact reims on 134.4 good bye"),
                replacer.Replace("sabena 376 contract rhein on 134.4 good bye"),
            }).ForEach(s => Console.WriteLine(s));



        }
        private static List<string> dummyAtcWords()
        {
            var list = new List<string>();

            list.Add("descend");
            list.Add("climb");
            list.Add("maintain");

            list.Add("flight");
            list.Add("level");

            list.Add("heading");
            list.Add("turn");

            list.Add("roger");
            list.Add("contact");

            return list;
        }

        private static Dictionary<string, string> dummyDeltaList()
        {
            var dict = new Dictionary<string, string>();

            dict.Add("roger", "butter");
            dict.Add("atc", "air traffic controller");

            return dict;
        }

        /// <summary>Loads the used phonetic substitution data from the passed xml.</summary>
        /// <param name="filePath">The path of the xml file, whiche contains the phonetic substitutions.</param>
        private static IReadOnlyDictionary<string, string> loadPhonetics(string filePath)
        {
            var phonetics = new Dictionary<string, string>();

            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(filePath);
            }
            catch (Exception e)
            {
                throw new Exception("Can't load the phonetic data", e);
            }
            XmlNodeReader reader = new XmlNodeReader(document);
            string value = null, key = null;
            while (reader.Read())
            {
                if ((reader.MoveToContent() == XmlNodeType.Element) && (reader.Name == "value"))
                {
                    value = reader.ReadString();
                }
                if ((reader.MoveToContent() == XmlNodeType.Element) && (reader.Name == "key"))
                {
                    key = reader.ReadString();
                }
                if ((key != null) && (value != null))
                {
                    phonetics.Add(key, value);
                    key = null;
                    value = null;
                }
            }

            return phonetics;
        }
    }
}
