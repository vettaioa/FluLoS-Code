/*
 * File:      SearchableCollection.cs
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

namespace FuzzySearching
{
    public class SearchableCollection : IReadOnlyCollection<string>
    {
        private IReadOnlyCollection<string> list;
        private float ratio = 1;

        private IDictionary<string, string> phonetics = new Dictionary<string, string>();
        private IDictionary<string, Trigram> trigram = new Dictionary<string, Trigram>();

        private FuzzySearch fuzzySearch;

        public IReadOnlyCollection<string> List
        {
            get { return list; }
            set
            {
                list = value;
                phonetics = fuzzySearch.getPhoneticSubstitutions(value);
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

        public SearchableCollection(FuzzySearch fuzzySearch)
        {
            this.fuzzySearch = fuzzySearch;
        }
        public SearchableCollection(FuzzySearch fuzzySearch, IReadOnlyCollection<string> list)
        {
            this.fuzzySearch = fuzzySearch;
            List = list;
        }

        /// <summary>Führt eine 'fuzzy' Suche für die übergebene Zeichenkette aus. Gibt die am besten übereinstimmende Zeichenkette zurück.
        /// Wurde keine übereinstimmung gefunden, welche der geforderten Ratio entspricht, wird <c>null</c> zurück gegeben.</summary>
        /// <param name="search">Die zu suchende Zeichenkette.</param>
        /// <returns>Die beste Übereinstimmung, oder <c>null</c> falls keine genügend gute gefunden worden ist.</returns>
        public string fuzzySearching(string search)
        {
            if(list == null)
            {
                throw new Exception("Collection is 'null'");
            }

            return fuzzySearch.search(phonetics, trigram, search, Ratio);
        }

        // IReadOnlyCollection interface
        public int Count => list.Count;

        public bool IsReadOnly => true;

        public IEnumerator<string> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
