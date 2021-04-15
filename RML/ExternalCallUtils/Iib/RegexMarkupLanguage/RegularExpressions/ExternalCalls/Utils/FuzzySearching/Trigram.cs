/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching
 * File:      Trigram.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching {
    
  /// <summary>The trigram class correlate to a string of three 3 chars for the trigram algo. expansion
  /// of the ArrayList: The position of the string data, which contains this trigram are saved in the
  /// ArrayList container.</summary>
  public class Trigram : ArrayList, IComparable {
    private String name = null;

    /// <summary>Constructor</summary>
    /// <param name="name">trigram string</param>
    public Trigram(String name) {
      if(name.Length != 3) {
        throw new Exception("The given string can't used as a Trigramm because the size of it isn't three!");
      }
      this.name = name;
    }
     
    /// <summary>Get-/Setter of the name.</summary>
    public String Name {
      get { return this.name; }
      set { this.name = value; }
    }

    /// <summary>Compares the current instance with another object of the same type.
    /// 
    /// Returns:
    //     A 32-bit signed integer that indicates the relative order of the objects
    //     being compared. The return value has these meanings: Value Meaning Less than
    //     zero This instance is less than obj. Zero This instance is equal to obj.
    //     Greater than zero This instance is greater than obj.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    int IComparable.CompareTo(object obj) {
      Trigram tri = (Trigram)obj;
      return this.name.CompareTo(tri.Name);
    }

    /// <summary>Determines whether the specified System.Object is equal to the current instance.
    /// 
    /// Returns:
    //     true if the specified System.Object is equal to the current System.Object;
    //     otherwise, false.
    /// </summary>
    /// <param name="obj">The System.Object to compare with the current instance.</param>
    public override bool Equals(object obj) {
      if(obj == null) return false;
      if(this.GetType() != obj.GetType()) {
        if(obj.GetType() != this.name.GetType()) return false;
        return this.name.Equals((String)obj);
      } else {
        Trigram tri = (Trigram) obj;
        return this.name.Equals(tri.name);
      }
    }

    /// <summary>For debugging, write the content of the trigram object to the console.</summary>
    /// <returns></returns>
    public override string ToString() {
      string output = this.name;
      foreach(int i in this){
        output += "-" + i;
      }
      return output;
    }
  }
}
