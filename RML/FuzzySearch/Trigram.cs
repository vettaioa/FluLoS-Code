/*
 * File:      Trigram.cs
 * Version:   2.0
 * Date:      31.03.2021
 * Authors:   Marco Vergari (marco@vergari.ch)
 * 
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 * 
 * Updated to .NET 5 and Improved by Ioannis Vettas and Pascal Haupt
 *
 */

using System;
using System.Collections;

namespace FuzzySearching
{
    /// <summary>The trigram class correlate to a string of three 3 chars for the trigram algo. expansion
    /// of the ArrayList: The position of the string data, which contains this trigram are saved in the
    /// ArrayList container.</summary>
    public class Trigram : ArrayList, IComparable<Trigram>
    {
        public string Name { get; }

        /// <summary>Constructor</summary>
        /// <param name="name">trigram string (length of 3)</param>
        public Trigram(string name)
        {
            if (name.Length != 3)
            {
                throw new ArgumentException("The given string can't be used as a Trigramm because the size of it isn't three!");
            }
            Name = name;
        }

        /// <summary>Compares the current instance with another object of the same type.
        /// 
        /// Returns:
        //     A 32-bit signed integer that indicates the relative order of the objects
        //     being compared. The return value has these meanings: Value Meaning Less than
        //     zero This instance is less than obj. Zero This instance is equal to obj.
        //     Greater than zero This instance is greater than obj.
        /// </summary>
        /// <param name="other">Another Trigram to compare with this instance.</param>
        public int CompareTo(Trigram other)
        {
            return Name.CompareTo(other.Name);
        }

        /// <summary>Determines whether the specified System.Object is equal to the current instance.
        /// 
        /// Returns:
        //     true if the specified System.Object is equal to the current System.Object;
        //     otherwise, false.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current instance.</param>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType())
            {
                if (obj.GetType() != Name.GetType())
                {
                    return false;
                }
                return Name.Equals((string) obj);
            }
            else
            {
                Trigram other = (Trigram) obj;
                return Name.Equals(other.Name);
            }
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        /// <summary>For debugging, write the content of the trigram object to the console.</summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = Name;
            foreach (int i in this)
            {
                output += "-" + i;
            }
            return output;
        }
    }
}
