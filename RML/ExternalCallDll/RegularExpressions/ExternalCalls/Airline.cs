/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls
 * File:      Car.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using System.IO;
using Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils;
using System.Text.Json;
using FuzzySearching;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls {

  public class Airline {
    private SearchableCollection airlines;

    public Airline()
    {
      string currentDir = Directory.GetCurrentDirectory();

      var phonetics = loadFromJson<Dictionary<string, string>>(Path.Combine(currentDir, ConfigurationManager.AppSettings["phoneticsFile"]));
      var airlines = loadFromJson<List<string>>(Path.Combine(currentDir, ConfigurationManager.AppSettings["airlinesFile"]));

      var fuzzySearch = new FuzzySearch(phonetics);

      this.airlines = new SearchableCollection(fuzzySearch, airlines);
      this.airlines.Ratio = 0.5f;
    }

    [ExternalCallMethod]
    public string checkAirline(string value) {

      string result = airlines.fuzzySearching(value);
      if (result != null)
      {
        return result;
      }
      return "";
    }

    //[ExternalCallMethod()]
    //public string checkFlightNumber(string value)
    //{
    //  marksFlightNumber.Ratio = 0.5F;

    //  string result = marksFlightNumber.fuzzySearching(value);
    //  if (result != null)
    //  {
    //    return result;
    //  }
    //  return "";
    //}

    private static TValue loadFromJson<TValue>(string filePath)
    {
      var json = File.ReadAllText(filePath);
      return JsonSerializer.Deserialize<TValue>(json, new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
    }
  }
}
