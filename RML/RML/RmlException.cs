/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      RmlException.cs
 * Version:   1.0
 * Date:      08.08.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System;

namespace Iib.RegexMarkupLanguage {

  /// <summary>Allgemeine Exception von Rml.</summary>
  public class RmlException : ApplicationException {
        
    /// <summary>Erzeugt eine <c>RmlException</c>.<summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="e">Original Exception, welche aufgetreten ist.</param>
    public RmlException(string message, Exception e) : base(message, e) {} 
    
    /// <summary>Erzeugt eine <c>RmlException</c>.</summary>
    /// <param name="message">Fehlermeldung</param>
    public RmlException(string message) : base(message) {}
  }
}
