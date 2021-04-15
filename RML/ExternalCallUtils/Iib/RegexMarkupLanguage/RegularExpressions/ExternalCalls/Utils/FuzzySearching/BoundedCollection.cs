/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching
 * File:      BoundedCollection.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching {
    
  /// <summary>Diese Klasse wird benötigt wenn man in einer Liste mittels Fuzzysearch nach Werten suchen möchte. Die Liste kann
  /// mittels Databinding gefüllt werden.</summary>
  public class BoundedCollection : DataBoundControl, ICollection {
    private string dataField = null;
    private List<string> list = null;
    private IDictionary<string, string> phonetics = null;
    private IDictionary<string, Trigram> trigram = null;
    private float ratio = 1;

    /// <summary>Erstellt ein <c>BoundedCollection</c> Objekt.</summary>
    /// 
    /// <param name="datasource">Die zu bindende DataSource.</param>
    /// <param name="dataField">Das zu bindende Datenelement (Xml: Element, Table: Spalte,...)</param>
    /// <param name="ratio">Die für die 'fuzzy' Suche benützte Ratio.</param>
    public BoundedCollection(object datasource, string dataField, float ratio) {
      list = new List<string>();
      base.DataSource = datasource;
      this.dataField = dataField;
      this.DataBind();
    }
    
    /// <summary>Default Konstruktor der <c>BoundedCollection</c> Klasse.</summary>
    public BoundedCollection() {
      list = new List<string>();
      phonetics = new Dictionary<string, string>();
      trigram = new Dictionary<string, Trigram>();
    }

    /// <summary>Get-/Setter Methode für das DataField. Dieses Feld wird für das binden der Datenquelle benutzt.</summary>
    /// <remarks>Bei einer hierarchischen Datenquelle wie XmlDataSource, ist hier ein XPath ausdruck anzugeben.</remarks>
    public virtual string DataField {
      get{ return dataField; }
      set{ dataField = value; }
    }

    /// <summary>Get-/Setter Methode für die Ratio, welche für die 'fuzzy' Suche benötigt wird. Zulässige werte: 1...0,
    /// wobei 1 einer 100% übereinstimmung entspricht und 0 keiner.</summary>
    public float Ratio {
      get { return ratio; }
      set {
        if((value < 0) || (value > 1)) {
          throw new Exception("Invalid ratio!");
        }
        ratio = value; 
      }
    }

    /// <summary>Wird von der Basis Klasse DataBoundControl aufgerufen, um die Daten der DataSource zu binden.
    /// In diesem Falle werden die Daten nur übernommen.</summary>
    /// <param name="dataSource">Enumeration der internen Daten der DataSource.</param>
    protected override void PerformDataBinding(IEnumerable dataSource) {
      base.PerformDataBinding(dataSource);
      DoDataBinding(dataSource);
    }

    /// <summary>Bindet die zugeordnete DataSource an die Collection. Alle möglichen Daten werden in die Collection übernommen.</summary>
    /// <param name="dataSource">Enumeration der internen Daten der DataSource.</param>
    void DoDataBinding(IEnumerable dataSource) {
      string value_field = "";

      if((dataSource != null) && (dataField != null) && !value_field.Equals(dataField)) {
        value_field = dataField;
        foreach(object container in dataSource) {
          string val = null;
          if(DataSource.GetType().BaseType == typeof(HierarchicalDataSourceControl)) {
            object obj = XPathBinder.Eval(container, value_field);
            if(obj != null) {
              val = obj.ToString();
            }
          } else {
            object obj = DataBinder.GetPropertyValue(container, value_field);
            if(obj != null) {
              val = obj.ToString();
            }
          }
          
          if(val != null) {
            list.Add(val);
          }
        }
      } else {
        throw new Exception("Invalid datasource or datafield!");
      }

      phonetics = FuzzySearch.getInstance().getPhoneticSubstitutions(list);
      trigram = FuzzySearch.getInstance().generateTrigramms(phonetics);
    }

    /// <summary>Führt eine 'fuzzy' Suche für die übergebene Zeichenkette aus. Gibt die am Besten übereinstimmende Zeichenkette zurück. 
    /// Wurde keine übereinstimmung gefunden, welche der geforderten Ratio entspricht, wird <c>null</c> zurück gegeben.</summary>
    /// <param name="search">Die zu suchende Zeichenkette.</param>
    /// <returns>Die beste Übereinstimmung, oder <c>null</c> falls keine genügend gute gefunden worden ist.</returns>
    public string fuzzySearching(string search) {
      return FuzzySearch.getInstance().search(phonetics, trigram, search, ratio);
    }
    
    public string search(string search) {
      return list.Find(delegate(string match){return match == search;});
    }
    
    #region ICollection Members
    public void CopyTo(Array array, int index) {
      int i = index;
      foreach(string s in list) {
        array.SetValue(s,i);
        i++;
      }
    }

    public int Count {
      get{ return list.Count; }
    }

    public bool IsSynchronized {
      get { return false; }
    }

    public object SyncRoot {
      get { return null; }
    }
    #endregion

    #region IEnumerable Members
    public IEnumerator GetEnumerator() {
      return list.GetEnumerator();
    }
    #endregion
  }
}
