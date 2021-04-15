/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.Interleave
 * File:      PermutationGenerator.cs
 * Version:   1.0
 * Date:      23.03.2007
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System;

namespace Iib.RegexMarkupLanguage.RegularExpressions.Interleave {

  /// <summary>Diese Klasse generiert alle Permutationen einer bestimmten Anzahl Elemente.
  /// Der Algorithmus ist von Kenneth H. Rosen, Discret Mathematics and its Applications.</summary>
  public class PermutationGenerator {
    private int[] state;
    private long left;
    private long total;

    /// <summary>Initialisiert den Permutation-Generator.</summary>
    /// <remarks>ACHTUNG: <c>n</c> darf nicht zu gross gewählt werden. Die Anzahl permutationen ist n! was
    /// bedeutet das die Anzahl sehr schnell sehr gross wird.</remarks>
    /// <example>20! = 2'432'902'008'176'640'000</example>
    /// <param name="n">Anzahl welche permutiert werden soll.</param>
    public PermutationGenerator(int n) {
      if(n < 1) {
        throw new Exception("The permutation generator must be initialze with n > 0.");
      }
      state = new int[n];
      total = getFactorial(n);
      Reset();
    }

    /// <summary>Gibt die verbleibenden Permutationen zurück.</summary>
    public long Left {
      get {
        return left;
      }
    }

    /// <summary>Gibt die Anzahl Permutation, die es gibt zurück.</summary>
    public long Total {
      get {
        return total;
      }
    }

    /// <summary>Gibt an ob es noch mehr Permutationen gibt.</summary>
    public bool HasMore {
      get {
        return (left != 0);
      }
    }

    /// <summary>Berechnet die Fakultät von <c>n</c>.</summary>
    /// <param name="n">Fakultät, welche berechnet wird</param>
    private long getFactorial(int n) {
      long fact = 1;
      for(int i=n; i>1; i--) {
        fact *= i;
      }
      return fact;
    }
    
    /// <summary>Setzt den Permutation-Generator in den Anfangszustand zurück.</summary>
    public void Reset() {
      for(int i=0; i<state.Length; i++) {
        state[i] = i;
      }
      left = total;
    }

    /// <summary>Generiert die nächste Permutation.</summary>
    /// <returns>Gibt die Reihenfolge der Elemente zurück.</returns>
    public int[] GetNext() {
      if(left == total) {
        left -= 1;
        return state;
      }

      int tmp;

      // Grösster Index j finden mit state[j] < state[j+1]
      int j = state.Length - 2;
      while(state[j] > state[j+1]) {
        j--;
      }

      // Index k finden, so dass state[k] die kleinste Zahl ist
      // wo grösser als state[j] ist und rechts von a[j] ist
      int k = state.Length - 1;
      while(state[j] > state[k]) {
        k--;
      }

      // state[j] und state[k] tauschen
      tmp = state[k];
      state[k] = state[j];
      state[j] = tmp;

      // Sortiert die Elemente nach der j-ten Position aufsteigend
      int r = state.Length - 1;
      int s = j + 1;
      while(r > s) {
        tmp = state[s];
        state[s] = state[r];
        state[r] = tmp;
        r--;
        s++;
      }
      left -= 1;
      return state;
    }
  }
}
