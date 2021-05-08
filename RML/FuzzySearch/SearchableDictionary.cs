/*
 * File:      SearchableDictionary.cs
 * Version:   1.0
 * Date:      31.03.2021
 * Authors:   Ioannis Vettas and Pascal Haupt
 * 
 * Copyright 2021. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FuzzySearching
{
    public class SearchableDictionary<TValue> : IReadOnlyDictionary<string, TValue>
    {
        private IReadOnlyDictionary<string, TValue> dict;
        private float ratio = 1;

        private IDictionary<string, string> phonetics = new Dictionary<string, string>();
        private IDictionary<string, Trigram> trigram = new Dictionary<string, Trigram>();

        private FuzzySearch fuzzySearch;

        public IReadOnlyDictionary<string, TValue> Dict
        {
            get { return dict; }
            set
            {
                dict = value;
                phonetics = fuzzySearch.getPhoneticSubstitutions(value.Keys);
                trigram = fuzzySearch.generateTrigramms(phonetics);
            }
        }

        /// <summary>Get-/Setter Methode für die Ratio, welche für die 'fuzzy' Suche benötigt wird. Zulässige werte: 1...0,
        /// wobei 1 einer 100% übereinstimmung entspricht und 0 keiner.</summary>
        public float Ratio
        {
            get { return ratio; }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("Invalid ratio!");
                }
                ratio = value;
            }
        }

        public SearchableDictionary(FuzzySearch fuzzySearch)
        {
            this.fuzzySearch = fuzzySearch;
        }
        public SearchableDictionary(FuzzySearch fuzzySearch, IReadOnlyDictionary<string, TValue> dict)
        {
            this.fuzzySearch = fuzzySearch;
            Dict = dict;
        }

        /// <summary>Führt eine 'fuzzy' Suche für die übergebene Zeichenkette aus. Gibt das Resultat für die am besten übereinstimmende Zeichenkette zurück. 
        /// Wurde keine übereinstimmung gefunden, welche der geforderten Ratio entspricht, wird der Type default zurück gegeben.</summary>
        /// <param name="search">Die zu suchende Zeichenkette.</param>
        /// <param name="ratio">Ratio für die fuzzy suche. Überschreibt den <c>Ratio</c> Property Wert, falls gesetzt und valid</param>
        /// <returns>Die Value der besten Übereinstimmung, oder der Type default falls keine genügend gute gefunden worden ist.</returns>
        public TValue fuzzySearching(string search, float? ratio = null)
        {
            if (dict == null)
            {
                throw new Exception("Dictionary is 'null'");
            }

            float neededRatio = Ratio;
            if (ratio != null && ratio > 0 && ratio < 1)
            {
                neededRatio = ratio.Value;
            }

            var key = fuzzySearch.search(phonetics, trigram, search, neededRatio);
            if(key != null)
            {
                TryGetValue(key, out var value);
                return value;
            }
            return default(TValue);
        }

        // IReadOnlyDictionary interface
        public int Count => dict.Count;

        public bool IsReadOnly => true;

        public IEnumerable<string> Keys => dict.Keys;

        public IEnumerable<TValue> Values => dict.Values;

        public TValue this[string key] => dict[key];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        IEnumerator<KeyValuePair<string, TValue>> IEnumerable<KeyValuePair<string, TValue>>.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }
}
