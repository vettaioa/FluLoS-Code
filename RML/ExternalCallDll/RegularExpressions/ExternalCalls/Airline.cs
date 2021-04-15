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
using System.Web.UI.WebControls;
using System.Configuration;
using Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching;
using Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils;
using System.IO;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls {

  public class Airline {
    private BoundedCollection marksAirline;
    private BoundedCollection marksFlightNumber;

    // anderpas & schuejen
    public Airline() {
      string dataDir = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["dataDir"]);

      marksAirline = new BoundedCollection();
      XmlDataSource sourceAirline = new XmlDataSource();
      sourceAirline.DataFile = dataDir + @"\airlines.xml";
      sourceAirline.XPath = "airlines/airline";
      marksAirline.DataSource = sourceAirline;
      marksAirline.DataField = "@mark";
      marksAirline.DataBind();
      marksAirline.Ratio = 0.5F;

      marksFlightNumber = new BoundedCollection();
      XmlDataSource sourceFlightNumber = new XmlDataSource();
      sourceFlightNumber.DataFile = dataDir + @"\flightnumbers.xml";
      sourceFlightNumber.XPath = "flightNumbers/flightNumber";
      marksFlightNumber.DataSource = sourceFlightNumber;
      marksFlightNumber.DataField = "@mark";
      marksFlightNumber.DataBind();
      marksFlightNumber.Ratio = 0.5F;
    }

    [ExternalCallMethod()]
    public string checkAirline(string value) {
      marksAirline.Ratio = 0.5F;

      string result = marksAirline.fuzzySearching(value);
      if (result != null) {
        return result;
      }
      return "";
    }

    [ExternalCallMethod()]
    public string checkFlightNumber(string value) {
      marksFlightNumber.Ratio = 0.5F;

      string result = marksFlightNumber.fuzzySearching(value);
      if(result != null) {
        return result;
      }
      return "";
    }
  }
}
