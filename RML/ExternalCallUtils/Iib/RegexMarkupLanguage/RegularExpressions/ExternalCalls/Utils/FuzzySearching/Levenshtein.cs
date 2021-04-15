/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching
 * File:      Levenshtein.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */

using System;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching {
	
	public class Levenshtein {
	
		/// <summary>Berechnet die Levenshtein Distanz. http://www.merriampark.com/ld.htm </summary>
		/// <returns>Distanz zwischen zwei Strings. Distanz = Anzahl Mutationen welche nötig sind um
    /// vom String 1 auf String 2 zu kommen.</returns>
		public static int LevenshteinDistance(string sSource, string sTarget) {
			int n = sSource.Length; //length of s
			int m = sTarget.Length; //length of t
			int[,] d = new int[n + 1,m + 1]; // matrix
			int cost; // cost
			// Step 1
			if(n == 0) return m;
			if(m == 0) return n;
			// Step 2
			for(int i=0; i<=n; d[i, 0] = i++);
			for(int j=0; j<=m; d[0, j] = j++);
			// Step 3
			for(int i=1; i<=n; i++) {
				//Step 4
				for(int j = 1; j <= m; j++) {
				  // Step 5
					cost = (sTarget.Substring(j - 1, 1) == sSource.Substring(i - 1, 1) ? 0 : 1);
					// Step 6
					d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
				}
			}
			// Step 7
			return d[n, m];
		}
  }
}