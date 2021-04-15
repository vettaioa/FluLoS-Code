/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      Compiler.cs
 * Version:   1.0
 * Date:      26.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System.Collections.Generic;
using System;
using System.IO;
using Iib.RegexMarkupLanguage.Collections;
using System.Text;
using System.Xml;
using log4net;

namespace Iib.RegexMarkupLanguage {

  /// <summary>Diese Klasse kompilliert eine Script-Datei. Dabei wird der Parser aufgerufen und danach der eigentliche
  /// Regex generiert.</summary>
  /// <remarks>Die Klasse ist Thread-Safe.</remarks>
  internal static class Compiler {
    private static readonly ILog log = LogManager.GetLogger(typeof(Compiler));
    private static string DELIMITER = @"[ \t\n\r]*";
    
    /// <summary>Kompilliert die Script-Datei <c>scriptName</c>. Dabei wird die Script-Datei dem <c>Parser</c>
    /// übergeben, der die Syntaxbäume und allfällige Fehlermeldungen erzeugt. Danach wird aus dem AnalyserTree
    /// einen Regex generiert.</summary>
    /// <param name="scriptName">Name der Datei, welche das Script enthält.</param>
    /// <param name="exceptions">Out-Parameter in den Warnungen und Fehler gespeichert werden.</param>
    /// <param name="outputTree">Out-Parameter in den der Output-Tree gespeichert wird.</param>
    /// <param name="analyserRegex">Out-Parameter in den der generierte Regex gespeichert wird.</param>
    /// <exception cref="CompilerException">Fehler beim Compilieren.</exception>
    public static void Compile(string scriptName, string prefix, string suffix, string delimiter,
                               out IEnumerable<CompilerException> exceptions, out BinaryTree<Token> outputTree, out string analyserRegex,
                               out IDictionary<string, string> extCallMethods) {
      lock(typeof(Compiler)) {
        log.Info("Initialize the compiler...");
        DELIMITER = delimiter;
        string oldDir = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.GetDirectoryName(scriptName);
        try {
          Parser.Parse(scriptName);
          extCallMethods = Parser.ExtCallMethods;
          outputTree = Parser.OutputTree;
          analyserRegex = prefix + DELIMITER + EvalAnalyserTree(Parser.AnalyserTree.Root) + DELIMITER + suffix;
          log.Debug("Builded Analyser regex: '" + analyserRegex + "'...");
          log.Info("Compiler succeeded...");
        } catch(CompilerException e) {
          log.Info("Compiler failed with '" + e.Message + "'...");
          throw e;         
        } finally {
          exceptions = Parser.Exceptions;
          Environment.CurrentDirectory = oldDir;
        }
      }
    }
   
    /// <summary>Wertet rekursiv den AnalyserTree aus. Geht durch alle Nodes durch und erstellt einen Regex für
    /// den AnalyserTree.</summary>
    /// <param name="node">TreeNode welcher ausgewertet werden soll.</param>
    /// <returns>Regex welcher dem <c>node</c> entspricht.</returns>
    private static string EvalAnalyserTree(TreeNode<Token> node) {
      StringBuilder regex = new StringBuilder();
      switch(node.Item.Kind) {
        case TokenCode.IDENT:                              // (?<Ident-Name> expression)
          regex.Append("(?<");
          regex.Append(node.Item.Value);
          regex.Append(">");
          regex.Append(EvalAnalyserTree(node.Left) + ")");
        break;
        case TokenCode.VARIDENT:                           // ($Patter)
        case TokenCode.STRINGCONST:
          regex.Append("(");
          regex.Append(node.Item.Value);
          regex.Append(")");
        break;
        case TokenCode.ANY:                                // (expression \s*)*
          regex.Append("(");
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append(DELIMITER);
          regex.Append(")*");
        break;
        case TokenCode.PLUS:                               // (expression \s*)+
          regex.Append("(");
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append(DELIMITER);
          regex.Append(")+");
        break;
        case TokenCode.OPTIONAL:                           // (expression)?
          regex.Append("(");
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append(")?");
        break;
        case TokenCode.COMMA:                              // (expression \s* expression)
          regex.Append("(");
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append(DELIMITER);
          regex.Append(EvalAnalyserTree(node.Right));
          regex.Append(")");
        break;
        case TokenCode.AND:                                // (\s* expression & \s* expression)
          regex.Append("(");
          regex.Append(DELIMITER);
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append("&");
          regex.Append(DELIMITER);
          regex.Append(EvalAnalyserTree(node.Right));
          regex.Append(")");
        break;
        case TokenCode.OR:                                 // (expression | expression)
          regex.Append("(");
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append("|");
          regex.Append(EvalAnalyserTree(node.Right));
          regex.Append(")");
        break;
        case TokenCode.ATOM:                               // (?>expression)
          regex.Append("(?>");
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append(")");
        break;
        case TokenCode.CHECK:                              // (expression \C{Class.Method})
          regex.Append("(");
          regex.Append(EvalAnalyserTree(node.Left));
          regex.Append(@"\C{" + node.Right.Item.Value + "}");
          regex.Append(")");
         
        break;
      }
      return regex.ToString();
    }
  }
}
