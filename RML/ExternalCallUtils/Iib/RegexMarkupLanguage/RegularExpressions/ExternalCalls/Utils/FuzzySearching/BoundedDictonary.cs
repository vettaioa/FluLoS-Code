/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils.FuzzySearching
 * File:      BoundedDictonary.cs
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
  public class BoundedDictionary : DataBoundControl, IDictionary {
    private Dictionary<object, object> dictionary = null;
    private string valueField = null, keyField = null;
    private IDictionary<string, string> phonetics = null;
    private IDictionary<string, Trigram> trigram = null;
    private float ratio = 1;

    public BoundedDictionary() {
      dictionary = new Dictionary<object, object>();
    }

    public BoundedDictionary(object datasource, string valueField, string keyField) {
      dictionary = new Dictionary<object, object>();
      base.DataSource = datasource;
      this.valueField = valueField;
      this.keyField = keyField;
    }

    public virtual string ValueField {
      get { return valueField; }
      set { valueField = value; }
    }

    public virtual string KeyField {
      get { return keyField; }
      set { keyField = value; }
    }

    public float Ratio {
      get { return ratio; }
      set { ratio = value; }
    }

    protected override void PerformDataBinding(IEnumerable dataSource) {
      base.PerformDataBinding(dataSource);
      DoDataBinding(dataSource);
    }

    void DoDataBinding(IEnumerable dataSource) {
      if(dataSource != null) {
        foreach(object container in dataSource) {
          string key;
          string val;
          key = val = null;
          if(keyField != null) {
            key = DataBinder.GetPropertyValue(container, keyField).ToString();
            object con = DataBinder.GetDataItem(container);

            if((key != null) && (valueField != null)) {
              val = DataBinder.GetPropertyValue(container, valueField).ToString();
              dictionary.Add(key, val);
            }
          }
        }
      }

      List<string> keys = new List<string>();
      foreach(object key in dictionary.Keys) {
        keys.Add((string)key);
      }

      phonetics = FuzzySearch.getInstance().getPhoneticSubstitutions(keys);
      trigram = FuzzySearch.getInstance().generateTrigramms(this.phonetics);
    }

    public string fuzzySearching(string search) {
      return FuzzySearch.getInstance().search(phonetics, trigram, search, ratio);
    }

    #region IDictionary Members
    public void Add(object key, object value) {
      throw new Exception("The method or operation is not implemented.");
    }

    public void Clear() {
      throw new Exception("The method or operation is not implemented.");
    }

    public bool Contains(object key) {
      return dictionary.ContainsKey(key);
    }

    public IDictionaryEnumerator GetEnumerator() {
      return dictionary.GetEnumerator();
    }

    public bool IsFixedSize {
      get { return true; }
    }

    public bool IsReadOnly {
      get { return true; }
    }

    public ICollection Keys {
     get { return dictionary.Keys; }
    }

    public void Remove(object key) {
      throw new Exception("The method or operation is not implemented.");
    }

    public ICollection Values {
      get { return dictionary.Values; }
    }

    public object this[object key] {
      get { return dictionary[key]; }
      set { dictionary[key] = value; }
    }
    #endregion

    #region ICollection Members
    public void CopyTo(Array array, int index) {
      throw new Exception("The method or operation is not implemented.");
    }

    public int Count {
      get { return dictionary.Count; }
    }

    public bool IsSynchronized {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public object SyncRoot {
      get { throw new Exception("The method or operation is not implemented."); }
    }
    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator() {
      return dictionary.GetEnumerator();
    }
    #endregion
  }
}
