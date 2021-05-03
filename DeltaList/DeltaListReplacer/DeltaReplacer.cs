using FuzzySearching;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaListReplacer
{
    public class DeltaReplacer
    {
        private FuzzySearch fuzzySearch;
        private SearchableCollection atcWords;
        private SearchableDictionary<string> deltaList;

        public DeltaReplacer(
            IReadOnlyDictionary<string, string> phonetics,
            IReadOnlyList<string> atcWords,
            IReadOnlyDictionary<string, string> deltaList
        )
        {
            fuzzySearch = new FuzzySearch(phonetics);

            this.atcWords = new SearchableCollection(fuzzySearch, atcWords);
            this.atcWords.Ratio = 0.7f;

            this.deltaList = new SearchableDictionary<string>(fuzzySearch, deltaList);
            this.deltaList.Ratio = 0.8f;
        }

        public string Replace(string inputStr)
        {
            var sb = new StringBuilder();
            foreach(var word in inputStr?.Split(' '))
            {
                sb.Append(ReplaceWord(word)).Append(' ');
            }
            return sb.ToString();
        }

        private string ReplaceWord(string word)
        {
            string atcWord = atcWords.fuzzySearching(word);
            if (atcWord != null)
            {
#if DEBUG
                if(word != atcWord)
                {
                    Console.WriteLine($"[DEBUG] Replacing '{word}' with '{atcWord}' [atcWord]");
                }
#endif
                return atcWord;
            }

            string deltaWord = deltaList.fuzzySearching(word);
            if(deltaWord != null)
            {
#if DEBUG
                if (word != atcWord)
                {
                    Console.WriteLine($"[DEBUG] Replacing '{word}' with '{deltaWord}' [deltaWord]");
                }
#endif
                return deltaWord;
            }

            return word; // no word alternative found => use original
        }
    }
}
