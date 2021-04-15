/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching
 * File:      FuzzySearch.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */
 
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Configuration;
using log4net;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching {
    
  /// <summary>The <c>FuzzySearch</c> class, contains a collection of function for the fuzzy searching. It's a singelton
  /// and should be initialized by the createInstance() method.</summary>
  public class FuzzySearch {
    private static readonly ILog log = LogManager.GetLogger(typeof(FuzzySearch));
    private static FuzzySearch instance = null;
    IDictionary<string, string> phoneticSubs;
    
    /// <summary>The constructor of the <c>FuzzySearch</c> class.</summary>
    private FuzzySearch(string phoneticsFilePath) {
      phoneticSubs = new Dictionary<string, string>();
      loadPhonetics(phoneticsFilePath);
    }

    /// <summary>Gets the singelton instance of the <c>FuzzySearch</c> class.</summary>
    public static FuzzySearch getInstance() {
      if(instance == null) {
        string phoneticsFilePath = ConfigurationManager.AppSettings["phonetics"];
        if(phoneticsFilePath == null) {
          log.Fatal("Can't read application setting argument 'phonetics'...");
          throw new Exception("Can't read application setting argument 'phonetics'...");
        }
        instance = new FuzzySearch(phoneticsFilePath);
      }
      return instance;
    }

    /// <summary>Gets a dictionary with the orginal string as key and the phonetic substitution as value.</summary>
    /// <param name="data">A list with the orginal strings.</param>
    public IDictionary<string, string> getPhoneticSubstitutions(IList<string> data) {
      IDictionary<string, string> retVal = new Dictionary<string, string>();
      foreach(string s in data) {
        string org = s.ToLower();
        if(!retVal.ContainsKey(s)) {
          retVal.Add(s, getPhoneticSubstitution(org));
        }
      }
      return retVal;
    }

    /// <summary>Process the phonetic substitution on the passed string.</summary>
    /// <param name="data">The orginal string.</param>
    /// <returns>Returns the string with the phonetic substitutions.</returns>
    public string getPhoneticSubstitution(string data) {
      string sub = (string)data.Clone();
      foreach(string key in this.phoneticSubs.Keys) {
        sub = sub.Replace(key, phoneticSubs[key]);
      }
      return sub;
    }

    /// <summary>Gets a dictionary with all <c>Trigram</c>, which the passed phonetic data contains. The keys are the
    /// related string of the <c>Trigram</c> and the values are the <c>Trigram</c> self.</summary>
    /// <param name="phoneticData">The dictionary with the phonetic substitutions.</param>
    public IDictionary<string,Trigram> generateTrigramms(IDictionary<string,string> phoneticData) {
      string data = null;
      int size = 0;

      IDictionary<string,Trigram> retVal = new Dictionary<string,Trigram>();
      foreach(string key in phoneticData.Keys) {
        data = phoneticData[key];
        // add a "_" char if the word is to short for one Trigramm, but words of one letter will be ignored
        if(data.Length < 3) {
          data += "_";
        }
        size = data.Length;

        string triString = null;
        Trigram trigramm;
        for(int i=0; i+3< size+1; i++) {
          triString = data.Substring(i, 3);
          if(retVal.ContainsKey(triString)) {
            retVal[triString].Add(key);
          } else {
            trigramm = new Trigram(triString);
            trigramm.Add(key);
            retVal.Add(trigramm.Name,trigramm);
          }
        }
      }
      return retVal;
    }

    /// <summary>Fuzzy search the passed string in the passed data. If related string is founded in the data, this one would be
    /// returned else <c>null</c>.</summary>
    /// <remarks>The ratio of the result can be a value between 1 and 0. 1: best ratio / 0: worst ratio</remarks>
    ///
    /// <param name="phoneticData">The dictionary with the phonetic substitutions.</param>
    /// <param name="trigramms">The dictionary with the <c>Trigram</c> data.</param>
    /// <param name="txt">The string to search</param>
    /// <param name="neededRatio">The ratio,whiche the result must have.</param>
    public string search(IDictionary<string, string> phoneticData, IDictionary<string, Trigram> trigramms, string txt, float neededRatio) {
      string trim;
      int size = 0;
      IList<float> ratio = new List<float>();
      IList<string> entry = new List<string>();

      txt = txt.ToLower().Trim();
      txt = getPhoneticSubstitution(txt);
      // add a "_" char if the word is to short for one Trigramm, but words of one letter will be ignored
      if(txt.Length == 2) {
        txt += "_";
      }
      size = txt.Length;
        
      //search related trigramms and count the hits on the diffrent possible results
      int h = 3;
      for(int p=0; p+3 < size+1; p++) {
        trim = txt.Substring(p, 3);
        if(trigramms.ContainsKey(trim)) {
          foreach(string foundedIn in trigramms[trim]) { 
            int index = entry.IndexOf(foundedIn);
            if( index != -1) {
              ratio[index] = ratio[index]+h;
            } else {
              entry.Add(foundedIn);
              ratio.Add(h);
            }
          }
        }
        h = 1;
      }

      //return null if no possible result is founded
      if(entry.Count < 1) {
        return null;
      }

      //extract the result or the results with the best ratio
      IList<string> best = new List<string>();
      float bestRatio = 0;

      int i = 0;
      foreach(float acRat in ratio) {
        if(acRat > bestRatio) {
          best.Clear();
          best.Add(entry[i]);
          bestRatio = acRat;
        } else if(acRat == bestRatio) {
          best.Add(entry[i]);
        }
        i++;
      }
      ratio.Clear();


      //incorporate the the levenshtein distance in the ratio
      i = 0;
      string bestEntry = null;
      float endRatio = 0;
      foreach(string org in best) {
        float tmpRatio = 1 -(bestRatio/(txt.Length));
        float dist = Levenshtein.LevenshteinDistance(txt, phoneticData[org]);
        dist = 1-((txt.Length - dist) / txt.Length);
        tmpRatio = 1-((tmpRatio + dist) / 2);

        if(tmpRatio > endRatio) {
          endRatio = tmpRatio;
          bestEntry = org;
        }
        i++;
      }

      //check if the final result better then the passed ratio
      if(endRatio >= neededRatio){
        return bestEntry;
      }
      return null;
    }
     
    /// <summary>Loads the used phonetic substitution data from the passed xml.</summary>
    /// <param name="filePath">The path of the xml file, whiche contains the phonetic substitutions.</param>
    private void loadPhonetics(string filePath) {
      XmlDocument document = new XmlDocument();
      try{
        document.Load(filePath);
      }catch (Exception e){
        log.Fatal("Can't load the phonetic data ....", e);
        throw new Exception("Can't load the phonetic data ....", e);
      }
      XmlNodeReader reader = new XmlNodeReader(document);
      string value = null, key = null;
      while(reader.Read()) {
        if((reader.MoveToContent() == XmlNodeType.Element) && (reader.Name == "value")) {
          value = reader.ReadString();
        }
        if((reader.MoveToContent() == XmlNodeType.Element) && (reader.Name == "key")) {
          key = reader.ReadString();
        }
        if((key != null) && (value != null)) {
          phoneticSubs.Add(key, value);
          key = null;
          value = null;
        }
      }
    }
  }
}
