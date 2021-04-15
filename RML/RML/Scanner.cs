/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      Scanner.cs
 * Version:   1.0
 * Date:      13.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System;
using log4net;

namespace Iib.RegexMarkupLanguage {

  /// <summary>Diese Klasse stellt Methoden zur Verfügung um Tokens eines Quellcodes zu erhalten.</summary>
  /// <remarks>Bevor die Scanner-Klasse gebraucht werden kann, muss der Scanner mittels der <c>Init()</c>
  /// Methode initialisiert werden. Danach kann mit <c>nextToken()</c> ein Token gelesen werden. Wenn
  /// der Scanner nicht mehr gebraucht wird, sollte die Methode <c>Clear()</c> aufgerufen werden, damit
  /// alle Streams geschlossen werden.</remarks>
  /// <seealso cref="Iib.TextAnalyzer.Compiler.Scanner.Init(TextReader, string)"/>
  /// <seealso cref="Iib.TextAnalyzer.Compiler.Scanner.Clear"/>
  internal static class Scanner {
    private static readonly ILog log = LogManager.GetLogger(typeof(Scanner));
    private const char EOF = '\u0080';
    private static IDictionary<string, TokenCode> keyWords = null;
    private static TextReader source = null;
    private static string fileName;
    private static char ch;
    private static int col;
    private static int line;
    
    /// <summary>Initialisiert die Scanner Klasse. Es werden diverse Zustände zurück gesetzt und eine
    /// Hashtable mit den Schlüsselwörtern aufgebaut. Der Quellcode wird über den Stream <c>source</c>
    /// fortlaufend eingelesen.</summary>
    /// <param name="source">Der Quellcode als Stream.</param>
    /// <param name="scriptFile">Dateiname des Quellcodes. Wird für die Fehlerbehandlung gebraucht.</param>
    /// <seealso cref="Iib.TextAnalyzer.Compiler.Scanner.Clear"/>
    public static void Init(TextReader source, string scriptFile) {
      log.Info("Initialize the scanner with sourcefile '" + scriptFile + "'...");
      Clear();
      
      // Hashtable mit Schlüsselwörter initialisieren
      if(keyWords == null) {
        keyWords = new Dictionary<string, TokenCode>();
        keyWords.Add("imports", TokenCode.IMPORTS);
        keyWords.Add("patterns", TokenCode.PATTERNS);
        keyWords.Add("output-structure", TokenCode.OUTPUTSTRUCTURE);
        keyWords.Add("analyser-rules", TokenCode.ANALYSERRULES);
        keyWords.Add("pre-process", TokenCode.PREPROCESS);
        keyWords.Add("post-process", TokenCode.POSTPROCESS);
        keyWords.Add("start", TokenCode.START);
        keyWords.Add("element", TokenCode.ELEMENT);
        keyWords.Add("text", TokenCode.TEXT);
        keyWords.Add("atom", TokenCode.ATOM);
        keyWords.Add("check", TokenCode.CHECK);
      }
      
      Scanner.source = source;
      Scanner.fileName = scriptFile;
      nextChar();
    }
        
    /// <summary>Falls ein Stream offen ist wird dieser geschlossen. Die Zustände <c>col</c> und <c>line</c>
    /// werden zurück gesetzt.</summary>
    public static void Clear() {
      if(source != null) {
        source.Close();
      }
      ch = EOF;
      col = 0;
      line = 1;
      fileName = "";
    }

    /// <summary>Liest das nächste Zeichen vom Stream <c>source</c> ein und speichert es in <c>ch</c> ab. Die
    /// Spaltenzahl wird bei jedem Zeichen erhöht. Wenn das Zeichen ein Zeilenumbruch ist wird die Zeilenzahl
    /// erhöht und die Spaltenzahl auf 0 zurück gesetzt. Wenn keine weiteren Zeichen vorhanden sind, wird als
    /// Zeichen <c>EOF</c> in <c>ch</c> gespeichert.</summary>
    private static void nextChar() {
      try {
        ch = (char)source.Read();
        col++;
        if(ch == '\n') {
          line++;
          col = 0;
        } else if(ch == '\uFFFF') {
          ch = EOF;
        }
      } catch(IOException e) {
        log.Error("Error when reading from sourcefile...", e);
        throw e;
      } catch(ObjectDisposedException e) {
        log.Error("Error when reading from sourcefile. File is closed...", e);
        throw e;
      }
    }

    /// <summary>List vom Stream das nächste Token. Dabei werden Whitespaces überlesen.</summary>
    /// <returns>Gibt das aktuelle Token zurück. Bei einem Scan-Fehler wird ein Fehler-Token zurück gegeben,</returns>
    public static Token nextToken() {
      // Überlist Leerschläge, Tabs und Zeilenende
      while(ch <= ' ') {
        nextChar();
      }

      // Überprüft was es für ein Typ von Token ist
      Token tok = new Token(line, col, fileName);
      if((ch == '$') || (ch == '@') || ((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z'))) {
        ReadName(tok);
      } else {
        switch(ch) {
          case '"': ReadString(tok);                            break;
          case '/': tok = ReadComment();                        break;
          case '=': tok.Kind = TokenCode.ASSIGN;    nextChar(); break;
          case ';': tok.Kind = TokenCode.SEMICOLON; nextChar(); break;
          case ',': tok.Kind = TokenCode.COMMA;     nextChar(); break;
          case '|': tok.Kind = TokenCode.OR;        nextChar(); break;
          case '&': tok.Kind = TokenCode.AND;       nextChar(); break;
          case '*': tok.Kind = TokenCode.ANY;       nextChar(); break;
          case '+': tok.Kind = TokenCode.PLUS;      nextChar(); break;
          case '?': tok.Kind = TokenCode.OPTIONAL;  nextChar(); break;
          case '(': tok.Kind = TokenCode.LPAR;      nextChar(); break;
          case ')': tok.Kind = TokenCode.RPAR;      nextChar(); break;
          case '[': tok.Kind = TokenCode.LBRACK;    nextChar(); break;
          case ']': tok.Kind = TokenCode.RBRACK;    nextChar(); break;
          case '{': tok.Kind = TokenCode.LBRACE;    nextChar(); break;
          case '}': tok.Kind = TokenCode.RBRACE;    nextChar(); break;
          case EOF: tok.Kind = TokenCode.EOF;                   break;
          default: tok.Kind = TokenCode.NONE;       nextChar(); break;
        }
      }
      return tok;
    }
    
    /// <summary>Diese Methode erkennt Idents, VarIdents, ExtCalls, Annotations und Schlüsselwörter. List sollange
    /// Zeichen ein bis ein Whitespace, das Ende des Streams oder ein ungültiges Zeichen erreicht wird.</summary>
    /// <param name="tok">Token in das die Eigenschaften und der Wert des Namens gespeichert wird.</param>
    private static void ReadName(Token tok) {
      StringBuilder value = new StringBuilder();
      
      // List den Namen ein
      do {
        value.Append(ch);
        nextChar();
      } while(((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z')) || ((ch >= '0') && (ch <= '9')) || (ch == '_') || (ch == '-') || (ch == '.'));

      // Überprüft was für ein Typ Name es ist
      switch(value[0]) {
        case '$': 
          tok.Value = value.ToString();
          if(tok.Value.IndexOf('.') == -1) {
            tok.Kind = TokenCode.VARIDENT;
          } else {
            tok.Kind = TokenCode.NONE;
          }
        break;  
        case '@': 
          tok.Value = value.ToString();
          if(tok.Value.IndexOf('.') == -1) {
            tok.Kind = TokenCode.ANNOTATION;
          } else {
            tok.Kind = TokenCode.NONE;
          } 
        break;
        default: 
          if(keyWords.ContainsKey(value.ToString())) {
            tok.Kind = keyWords[value.ToString()];
          } else {
            tok.Value = value.ToString();
            int first = tok.Value.IndexOf('.'); 
            int last = tok.Value.LastIndexOf('.');
            if( first == -1) {
              tok.Kind = TokenCode.IDENT;
            } else if((first == last) && (first != (tok.Value.Length - 1))) {
              tok.Kind = TokenCode.EXTCALL;
            } else {
              tok.Kind = TokenCode.NONE;
            }
          }
        break;
      }
    }
    
    /// <summary>Diese Methode erkennt String-Konstanten. List sollange Zeichen für Zeichen bis der String mit dem
    /// "-Zeichen beendet wird. Die Escape-Sequenz \" wird dabei nicht als Abschluss des Strings angesehen. Wenn der
    /// String nicht mit dem "-Zeichen beendet wird, wird ein Fehler-Token zurück gegeben.</summary>
    /// <param name="tok">Token in das der Wert des Strings gespeichert werden soll</param>
    private static void ReadString(Token tok) {
 	    StringBuilder value = new StringBuilder();
      
      // List den String ein
      nextChar();
 	    while((ch != '"') && (ch >= ' ') && (ch != EOF))  {
        value.Append(ch);
        	    
 	      // Auf die Escape-Sequenz \" überprüfen
 	      if(ch == '\\') {
          nextChar();
 	        if(ch == '"') {
 	          value.Replace('\\', ch, value.Length-1 , 1);
            nextChar();
          }
 	      } else {
          nextChar();
        }
 	    }

      // Überprüfen ob der String abgeschlossen ist
      if(ch == '"') {
        tok.Kind = TokenCode.STRINGCONST;
        tok.Value = value.ToString();
        nextChar();
      } else {
        tok.Kind = TokenCode.NONE;
      }
    }

    /// <summary>Überlist Kommentare vom Typ "//comment" oder "/*comment*/". Nach dem Kommentar wird die Methode
    /// <c>nextToken()</c> erneut aufgerufen um das nächste Token nach dem Kommentar zu ermitteln.</summary>
    /// <returns>Gibt das nächste Token nach dem Kommentar zurück</returns>
    private static Token ReadComment() {
      nextChar();
      
      // Kommentar welcher mit // beginnt überlesen
      if(ch == '/') {
        do {
          nextChar();
        } while((ch != '\n') && (ch != EOF));
        return nextToken();
      }
      
      // Kommentar welcher mit /* beginnt überlesen
      if(ch == '*') {
        char before;
        int nested = 1;
        nextChar();
        do {
          before = ch;
          nextChar();
          if((before == '/') && (ch == '*')) {
            nested++;
          }
          if((before == '*') && (ch == '/')) {
            nested--;
          }
        } while((nested != 0) && (ch != EOF));

        // Überprüfen ob der Kommentar mit */ geschlossen wurde
        if(ch == '/') {
          nextChar();
          return nextToken();
        }
      }
      return new Token(TokenCode.NONE, line, col, "", fileName);
    }
  }
}