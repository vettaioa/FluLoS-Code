/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      CompilerException.cs
 * Version:   1.0
 * Date:      26.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System;

namespace Iib.RegexMarkupLanguage {

  /// <summary>Definiert den Typ einer <c>CompilerException</c>.</summary>
  public enum CompilerExceptionType {
    /// <summary>Die Exception ist eine Warnung.</summary>
    Warn,
    /// <summary>Die Exception ist ein Fehler.</summary>
    Error,
  };

  /// <summary>Allgemeine Exception vom Compiler.</summary>
  public class CompilerException : ApplicationException {
    private CompilerExceptionType compilerExceptionType;
    private Token token;
    
    /// <summary>Erzeugt eine <c>CompilerException</c>.<summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="e">Original Exception, welche aufgetreten ist.</param>
    /// <param name="type">Definiert von welchem Typ die Exception ist.</param>
    /// <param name="token">Token das einen Error oder eine Warnung ausgelöst hat. Kann auch <c>null</c> sein.</param>
    public CompilerException(string message, Exception e, CompilerExceptionType type, Token token) : base(message, e) {
      compilerExceptionType = type;
      this.token = token;
    }
    
    /// <summary>Erzeugt eine <c>CompilerException</c>.</summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="type">Definiert von welchem Typ die Exception ist.</param>
    /// <param name="token">Token das einen Error oder eine Warnung ausgelöst hat. Kann auch <c>null</c> sein.</param>
    public CompilerException(string message, CompilerExceptionType type, Token token) : base(message) {
      compilerExceptionType = type;
      this.token = token;
    }
    
    /// <summary>Gibt den Typ der Exception zurück.</summary>
    public CompilerExceptionType Type {
      get { return compilerExceptionType; }  
    }
    
    /// <summary>Gibt das Token der Exception zurück.</summary>
    public Token Token {
      get { return token; }
    }
  }
}
