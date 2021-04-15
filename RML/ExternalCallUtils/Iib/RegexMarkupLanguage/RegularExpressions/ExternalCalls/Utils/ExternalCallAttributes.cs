/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils
 * File:      ExternalCallAttributes.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */
 
namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils {

  /// <summary>Das <c>ExternalCallMethod</c> Attribut wird verwendet, um eine Methode als "external call" Methode 
  /// zu definieren. Es ist möglich der Methode einen Namen zu geben, andernfalls wird der Methodennamen verwendet.
  /// </summary>
  [System.AttributeUsage(System.AttributeTargets.Method)]
  public class ExternalCallMethod : System.Attribute {
    public string name;

    public ExternalCallMethod() {
      this.name = null;
    }
  }
}