/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      Token.cs
 * Version:   1.0
 * Date:      13.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */
 
namespace Iib.RegexMarkupLanguage {

  /// <summary>Enumeration mit allen Token-Type welche es gibt.</summary>
  public enum TokenCode {
    NONE,             // Fehler-Token
    IDENT,            // Example_0
    VARIDENT,         // $Example_1
    STRINGCONST,      // "example"
    EXTCALL,          // Class.Method
    ANNOTATION,       // @RelaxNG
    ASSIGN,           // =
    SEMICOLON,        // ;
    COMMA,            // ,
    OR,               // |
    AND,              // &
    ANY,              // *
    PLUS,             // +
    OPTIONAL,         // ?
    LPAR,             // (
    RPAR,             // )
    LBRACK,           // [
    RBRACK,           // ]
    LBRACE,           // {
    RBRACE,           // }
    IMPORTS,          // imports
    PATTERNS,         // patterns
    OUTPUTSTRUCTURE,  // output-structure
    ANALYSERRULES,    // analyser-rules
    PREPROCESS,       // pre-process
    POSTPROCESS,      // post-process
    START,            // start
    ELEMENT,          // element
    TEXT,             // text
    ATOM,             // atom
    CHECK,            // check
    EOF               // Ende des Zeichenstroms
  };
  
  /// <summary>Diese Klasse beinhaltet die Eigenschaften und Werte eines Tokens.</summary>
  public class Token {
    public static string[] MapToName = {"NONE", "identifier", "variable identifier", "string constant", "external call", "annotation",
                                       "assign", "semicolon", "comma", "|", "&", "*", "+", "?", "(", ")", "[", "]", "{", "}",
                                       "imports block", "patterns block", "output-structure block", "analyser-rules block",
                                       "pre-process block", "post-process block", "label start", "label element",
                                       "label text", "label atom", "label check", "end of file"};
    public TokenCode Kind;
    public int Line;
    public int Column;
    public string Value;
    public string FileName;
    
    /// <summary>Initialisiert ein Token mit den Werten, welche übergeben werden.</summary>
    /// <param name="kind">Typ des Tokens</param>
    /// <param name="line">Zeilenzahl auf der das Token auftritt</param>
    /// <param name="column">Spaltenzahl an der das Token auftritt</param>
    /// <param name="value">Wert des Tokens</param>
    /// <param name="fileName">Name der Datei, in der das Token auftritt</param>
    public Token(TokenCode kind, int line, int column, string value, string fileName) {
      this.Kind = kind;
      this.Line = line;
      this.Column = column;
      this.Value = value;
      this.FileName = fileName;
    }
    
    /// <summary>Initialisiert ein Token mit den Werten, welche übergeben werden.</summary>
    /// <param name="line">Zeilenzahl auf der das Token auftritt</param>
    /// <param name="column">Spaltenzahl an der das Token auftritt</param>
    /// <param name="fileName">Name der Datei, in der das Token auftritt</param>
    public Token(int line, int column, string fileName) {
      this.Kind = TokenCode.NONE;
      this.Line = line;
      this.Column = column;
      this.Value = "";
      this.FileName = fileName;
    }
  }
}