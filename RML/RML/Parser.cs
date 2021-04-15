/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      Parser.cs
 * Version:   1.0
 * Date:      20.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System.Collections.Generic;
using System.IO;
using System.Text;
using Iib.RegexMarkupLanguage.Collections;
using Iib.RegexMarkupLanguage.RegularExpressions;
using System;
using log4net;

namespace Iib.RegexMarkupLanguage {

  /// <summary>Diese Klasse parst eine Scriptdatei und liefert als Resultat die beiden Syntaxbäume OuputTree und 
  /// AnalyserTree zurück. Ausserdem wird noch eine Liste mit Warnungen und Fehlern erstellt.</summary>
  internal static class Parser {
    private static readonly ILog log = LogManager.GetLogger(typeof(Parser));
    private static IList<CompilerException> exceptions = null;
    private static IList<TokenCode> blocks;
    private static IList<string> importFiles;
    private static IDictionary<string, TreeNode<Token>> patterns;
    private static IDictionary<string, BinaryTree<Token>> outputHelperStruct;
    private static IDictionary<string, BinaryTree<Token>> analyserHelperStruct;
    private static IDictionary<string, string> extCallMethodList;
    private static BinaryTree<Token> outputTree;
    private static BinaryTree<Token> analyserTree;
    private static TokenCode la;
    private static TokenCode l2a;
    private static Token token;
    private static Token laToken;
    private static Token l2aToken;
        
    /// <summary>Parst das Sourcefile <c>scriptFile</c>. Für jeden Import im Sourcefile wird der Scanner erneut
    /// initialisiert und der Parsevorgang vortgesetzt. Nach dem Parsevorgang kann mittels den Properties
    /// <c>Exceptions</c>, <c>OutputTree</c> und <c>AnalyserTree</c> auf das Resultat zugegriffen werden.</summary>
    /// <param name="scriptFile">Name der Datei, welche das Script enthält.</param>
    /// <seealso cref="Iib.TextAnalyzer.Compiler.Parser.Exceptions"/>
    /// <seealso cref="Iib.TextAnalyzer.Compiler.Parser.OutputTree"/>
    /// <seealso cref="Iib.TextAnalyzer.Compiler.Parser.AnalyserTree"/>
    public static void Parse(string scriptFile) {
      log.Info("Initialize the parser...");
      
      // Dummy Tokens erzeugen
      token = new Token(0, 0, "");
      laToken = new Token(0, 0, "");
      l2aToken = new Token(0, 0, "");
      
      // Initialisiert die Datenstrukturen
      exceptions = new List<CompilerException>();
      blocks = new List<TokenCode>();
      importFiles = new List<string>();
      patterns = new Dictionary<string, TreeNode<Token>>();
      outputHelperStruct = new Dictionary<string, BinaryTree<Token>>();
      analyserHelperStruct = new Dictionary<string, BinaryTree<Token>>();
      extCallMethodList = new Dictionary<string, string>();
      outputTree = null;
      analyserTree = null;
      
      // Beginnt mit dem eigentlichen Parsevorgang
      try {
        importFiles.Add(Path.GetFullPath(scriptFile));
        for(int i=0; i<importFiles.Count; i++) {
          try {
            Scanner.Init(new StreamReader(importFiles[i]), importFiles[i]);
          } catch(IOException e) {
            Error(e.Message, new Token(0, 0, ""));
          }
          Scan(2);
          Program();
          Check(TokenCode.EOF);
        }
      } finally {
        Scanner.Clear();
      }
      
      // Überprüft ob alle zwingenden Blöcke definiert sind
      if(outputTree == null) {
        Error("The block 'output-structure' is not defined...", new Token(0, 0, ""));
      }
      if(analyserTree == null) {
        Error("The block 'analyser-rules' is not defined...", new Token(0, 0, ""));
      }
      
      // Erstellt die beiden Trees Output- und Analyser-Tree
      GenerateOutputTree();
      ExpandOutputTree();
      GenerateAnalyserTree();
      
      // Debug-Output der Bäume als String
      if(log.IsDebugEnabled) {
        DebugVisitor dv = new DebugVisitor(false);
        outputTree.PreOrderTraverse(dv);
        log.Debug("OutputTree in preorder traverse; '" + dv.TreeString + "'...");
        dv = new DebugVisitor(false);
        analyserTree.PreOrderTraverse(dv);
        log.Debug("AnalyserTree in preorder traverse: '" + dv.TreeString + "'...");
      }
    }
    
    // Karl
    #region Expandiere alle Knoten des Output Baumes
    private static void ExpandOutputTree() {
    	BinaryTree<Token> newOutputTree = new BinaryTree<Token>(ExpandOutputTree(outputTree.Root));
    	outputTree = newOutputTree;
    }
    
    private static TreeNode<Token> ExpandOutputTree(TreeNode<Token> node) {
    	if (node != null) {
    		TreeNode<Token> newNode = node.Clone();
    		if (node.Left != null) {
    			newNode.Left = ExpandOutputTree(node.Left);
    			newNode.Left.Parent = newNode;
    		}
    		if (node.Right != null) {
         		newNode.Right = ExpandOutputTree(node.Right);
    			newNode.Right.Parent = newNode;  			
    		}
    	    return newNode;
    	} else {
    		return null;
    	}
    }
    #endregion

    #region Semantischer-Teil des Parsers
    /// <summary>Erzeugt den vollständigen Output-Baum. Dies wird mittels der PreOrderTraverse-Methode und dem
    /// <c>GenerateOutputTreeVisitor</c> gemacht. Es wird überprüft ob alle Elemente definiert sind und bei den
    /// Elementen die definiert sind aber nicht gebraucht werden wird eine Warnung ausgegeben.</summary>
    private static void GenerateOutputTree() {
      try {
        outputTree.PreOrderTraverse(new GenerateOutputTreeVisitor(outputHelperStruct, true));
      } catch(BinaryTreeException<Token> e) {
        Error(e.Message, e.Node.Item);
      } 
       
      // Überprüfen ob es definierte Elemente gibt die nicht gebraucht werden
      foreach(KeyValuePair<string, BinaryTree<Token>> kvp in outputHelperStruct) {
        if(kvp.Value.Root.Order == -1) {
          Warn("Output element '" + kvp.Value.Root.Item.Value + "' defined but not used...", kvp.Value.Root.Item);
        }
      }
    }
    
    /// <summary>Erzeugt den vollständigen Analyser-Baum. Dies wird mittels der PreOrderTraverse-Methode und dem
    /// <c>GenerateAnalyserTreeVisitor</c> gemacht. Es wird überprüft ob alle Elemente und Patterns definiert sind
    /// und bei den Elementen und Patterns die definiert sind aber nicht gebraucht werden wird eine Warnung
    /// ausgegeben.</summary>
    private static void GenerateAnalyserTree() {
      try {
        analyserTree.PreOrderTraverse(new GenerateAnalyserTreeVisitor(analyserHelperStruct, patterns, true));
      } catch(BinaryTreeException<Token> e) {
        Error(e.Message, e.Node.Item);
      } 
       
      // Überprüfen ob es definierte Elemente gibt die nicht gebraucht werden
      foreach(KeyValuePair<string, BinaryTree<Token>> kvp in analyserHelperStruct) {
        if(kvp.Value.Root.Order == -1) {
          Warn("Analyser element '" + kvp.Key + "' defined but not used...", kvp.Value.Root.Item);
        }
      }
      
      // Überprüfen ob es definierte Pattern gibt die nicht gebraucht werden
      foreach(KeyValuePair<string, TreeNode<Token>> kvp in patterns) {
        if(kvp.Value.Order == -1) {
          Warn("Pattern '" + kvp.Key + "' defined but not used...", kvp.Value.Item);
        }
      }
    }
    #endregion
    
    #region Properties wie Exceptions, OutputTree, AnalyserTree
    /// <summary>Gibt einen <c>IEnumerable</c> von den Meldungen zurück.</summary>
    public static IEnumerable<CompilerException> Exceptions {
      get { return exceptions; }
    }
    
    /// <summary>Gibt den <c>OutputTree</c> zurück.</summary>
    public static BinaryTree<Token> OutputTree {
      get { return outputTree; }
    }
    
    /// <summary>Gibt den <c>AnalyserTree</c> zurück.</summary>
    public static BinaryTree<Token> AnalyserTree {
      get { return analyserTree; }
    }
    
    /// <summary>Gibt die <c>ExtCallMethodList</c> zurück.</summary>
    public static IDictionary<string, string> ExtCallMethods {
      get { return extCallMethodList; }
    }
    #endregion
    
    #region Allgemeine Parsermethoden wie Scan, Check, Error, Warn
    /// <summary>Speichert das look ahead Token im aktuellen Token und liest das nächste Token
    /// als look ahead Token ein.</summary>
    /// <param name="count">Anzal Token die gelesen werden sollen.</param>
    private static void Scan(int count) {
      try {
        for(int i=0; i<count; ++i) {
          token = laToken;
          laToken = l2aToken;
          l2aToken = Scanner.nextToken();
          la = laToken.Kind;
          l2a = l2aToken.Kind;
        }
      } catch(IOException e) {
        Error(e.Message, new Token(0, 0, ""));
      }
    }
    
    /// <summary>Überprüft ob das look ahead Token dem erwarteten <c>expected</c> Token entspricht. Wenn ja
    /// wird mittels <c>Scan()</c> ein Token weiter gelesen. Andernfalls wird eine Fehlermeldung mittels
    /// <c>Error()</c> erzeugt.</summary>
    /// <param name="expected"></param>
    private static void Check(params TokenCode[] expected) {
      foreach(TokenCode exp in expected) {
        if(la == exp) {
          Scan(1);
        } else {
          Error(Token.MapToName[(int)exp] + " expected...", laToken);
        }
      }
    }
    
    /// <summary>Erzeugt eine Fehlermeldung, welche sich auf das look ahead Token bezieht. Der Parsevorgang wird
    /// nach dem ersten Fehler unterbrochen. Die Meldungen können mittels <c>Exceptions</c> abgefragt werden.</summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="token">Token das einen Error ausgelöst hat.</param>
    /// <exception cref="CompilerException">Fehler beim Compilieren.</exception>
    /// <seealso cref="Iib.TextAnalyzer.Compiler.Parser.Exceptions"/>
    private static void Error(string message, Token token) {
      CompilerException error = new CompilerException(message, CompilerExceptionType.Error, token);
      exceptions.Add(error);
      throw error;
    }
    
    /// <summary>Erzeugt eine Warnung. Der Parsevorgang wird nicht unterbrochen. Die Meldungen können nach dem
    /// Parsevorgang mittels <c>Exceptions</c> abgefragt werden.</summary>
    /// <param name="message">Warnmeldung</param>
    /// <param name="token">Token das eine Warnung ausgelöst hat.</param>
    /// <seealso cref="Iib.TextAnalyzer.Compiler.Parser.Exceptions"/>
    private static void Warn(string message, Token token) {
      exceptions.Add(new CompilerException(message, CompilerExceptionType.Warn, token));
    }
    #endregion
    
    #region Allgemeine Nonterminalsymbole wie StringConcat und Quant
    /// <summary>Parst Strings und setzt die Strings zu einem zusammen. Am Ende ist im aktuellen Token
    /// <c>token.Value</c> der gesammte String gespeichert.</summary>
    /// <example>
    ///   Bsp 1: "string1" + "string2" + "string3"
    ///   Bsp 2: "string1"
    /// </example>
    private static void StringConcat() {
      StringBuilder str = new StringBuilder();
      while(true) {
        Check(TokenCode.STRINGCONST);
        str.Append(token.Value);
      
        // Weiterer String folgt
        if(la == TokenCode.PLUS) {
          Scan(1);
        } else {
          break;
        }
      }
      token.Value = str.ToString();
    }
    
    /// <summary>Parst Quantifiers.</summary>
    /// <remarks>Diese Methode geht immer davon aus, dass ein Quantifier optional ist.</remarks>
    /// <example>
    ///   Bsp 1: ? 
    ///   Bsp 2: *
    ///   Bsp 3: +
    /// </example>
    /// <returns>Gibt ein TreeNode zurück wenn ein Quantifier vorhanden ist, sonst null.</returns>
    private static TreeNode<Token> Quant() {
      if((la == TokenCode.OPTIONAL) || (la == TokenCode.ANY) || (la == TokenCode.PLUS)) {
        Scan(1);
        return new TreeNode<Token>(token);
      }   
      return null;
    }
    #endregion
    
    #region Nonterminalsymbol Program
    /// <summary>Parst die verschiedenen Blöcke.</summary>
    /// <example>
    ///   Bsp 1: imports {}
    ///   Bsp 2: patterns {}
    ///   Bsp 3: @RelaxNG output-structure {}
    ///   Bsp 4: analyser-rules {}
    ///   Bsp 5: post-process {}
    ///   Bsp 6: pre-process {}
    /// </example>
    private static void Program() {
      while(la != TokenCode.EOF) {
        switch(la) {
          case TokenCode.IMPORTS:
            Scan(1);
            Check(TokenCode.LBRACE);
            ImportBlock();
            Check(TokenCode.RBRACE);
          break;
          case TokenCode.PATTERNS:
            Scan(1);
            Check(TokenCode.LBRACE);
            PatternBlock();
            Check(TokenCode.RBRACE);
          break;
          case TokenCode.ANNOTATION:
            Scan(1);
            if(token.Value == "@RelaxNG") {
              Check(TokenCode.OUTPUTSTRUCTURE);
              if(!blocks.Contains(token.Kind)) {
                blocks.Add(token.Kind);
                Check(TokenCode.LBRACE);
                RelaxngOutputBlock();
                Check(TokenCode.RBRACE);
              } else {
                Error("Block 'output-structure' multiple defined...", token);
              }
            } else {
              Error("Invalid annotation. '@RelaxNG' expected...", token);
            }
          break;
          case TokenCode.OUTPUTSTRUCTURE:
            Scan(1);
            if(!blocks.Contains(token.Kind)) {
              blocks.Add(token.Kind);
              Check(TokenCode.LBRACE);
              RelaxngOutputBlock();
              Check(TokenCode.RBRACE);
            } else {
              Error("Block 'output-structure' multiple defined...", token);
            }
          break;
          case TokenCode.ANALYSERRULES:
            Scan(1);
            if(!blocks.Contains(token.Kind)) {
              blocks.Add(token.Kind);
              Check(TokenCode.LBRACE);
              AnalyserRuleBlock();
              Check(TokenCode.RBRACE);
            } else {
              Error("Block 'analyser-rules' multiple defined...", token);
            }
          break;
          case TokenCode.POSTPROCESS:
            Scan(1);
            if(!blocks.Contains(token.Kind)) {
              blocks.Add(token.Kind);
              Check(TokenCode.LBRACE);
              PostProcessBlock();
              Check(TokenCode.RBRACE);
            } else {
              Error("Block 'post-process' multiple defined...", token);
            }
          break;
          case TokenCode.PREPROCESS:
            Scan(1);
            if(!blocks.Contains(token.Kind)) {
              blocks.Add(token.Kind);
              Check(TokenCode.LBRACE);
              PreProcessBlock();
              Check(TokenCode.RBRACE);
            } else {
              Error("Block 'pre-process' multiple defined...", token);
            }
          break;
          default: 
            Error("Block expected...", laToken);
          break;
        } 
      }
    }   
    #endregion 
    
    #region Nonterminalsymbol ImportBlock
    /// <summary>Parst den Import-Block. Überprüft ob nicht mehrmals die gleiche Datei importiert wird.</summary>
    /// <example>
    ///   Bsp 1: "file1.itas", "file2.itas";
    ///   Bsp 2: "file1" + ".itas", "file2.itas";
    /// </example>
    private static void ImportBlock() {
      while(true) {
        StringConcat();
        
        // Überprüfen ob die Datei nicht schon einmal importiert wurde
        if(importFiles.Contains(Path.GetFullPath(token.Value))) {
          Warn("Multiple defined import '" + token.Value + "'...", token);
        } else {
          importFiles.Add(Path.GetFullPath(token.Value));  
        }
        
        // Falls es noch weitere Imports hat
        if(la == TokenCode.COMMA) {
          Scan(1);
        } else {
          break;
        }
      }
      Check(TokenCode.SEMICOLON);
    }
    #endregion

    #region Nonterminalsymbol PatternBlock
    /// <summary>Parst den Pattern-Block.</summary>
    private static void PatternBlock() {
      VarDecl();
      while(la == TokenCode.VARIDENT) {
        VarDecl();  
      }
    }
    
    /// <summary>Parst Variablen-Deklarationen. Füllt <c>patterns</c> mit den deklarierten Variablen. Überprüft das
    /// keine Variable doppelt deklariert ist. Das Token das in <c>patterns</c> gespeichert wird, hat den
    /// <c>TokenCode</c> <c>VARIDENT</c>. In <c>Token.Value</c> ist jedoch der Inhalt der Variable gespeichert und
    /// nicht der Name der Variable.</summary>
    /// <example>
    ///   Bsp 1: $Pattern1 = "regex1";
    ///   Bsp 2: $Pattern2 = "regex2" + "regex3";
    /// </example>
    private static void VarDecl() {
      Check(TokenCode.VARIDENT);
      if(patterns.ContainsKey(token.Value)) {
        Error("Multiple defined pattern...", token);
      } else {
        Token varIdent = token;
        Check(TokenCode.ASSIGN);
        StringConcat();
        string varIdentName = varIdent.Value;
        varIdent.Value = ExpandPattern(token.Value);
        // KARL DEBUG
        log.Debug(varIdentName+" "+varIdent.Value + " added to patterns");
        patterns.Add(varIdentName, new TreeNode<Token>(varIdent));
        CheckRegex(varIdent.Value);
      }
      Check(TokenCode.SEMICOLON);
    }
    
    /// <summary>Ersetzt Variabeln welche in einem Pattern vorkommen durch ihren Wert.</summary>
    /// <remarks>Alle Variabeln müssen vor der ersten Verwendung bereits definiert sein.</remarks>
    /// <example>$test2 = "\w+{$test1}";</example>
    /// <param name="toExpand">Pattern welches auf Variabeln untersucht wird.</param>
    /// <returns>Neues Pattern mit aufgelösten Variabeln.</returns>
    private static string ExpandPattern(string toExpand) {
      StringBuilder regex = new StringBuilder(toExpand);
      string toReplace = "";
      try {
        int begin = 0, end = 0;
        while(true) {        
          begin = toExpand.IndexOf("{$", end);
          if(begin != -1) {
            end = toExpand.IndexOf("}", begin);
            if(end != -1) {
              toReplace = toExpand.Substring(begin+1, end-begin-1);  
              TreeNode<Token> p = patterns[toReplace];
              p.Order = 0;  // Bedeutet, dass das Pattern gebraucht wurde
              regex.Replace("{" + toReplace + "}", p.Item.Value);
            } else {
              break;
            }
          } else {
            break;
          }
        }
      } catch(KeyNotFoundException){
        Error("No valid regex. Pattern '" + toReplace + "' not defined...", token);
      }
      return regex.ToString();     
    }

    /// <summary>Überprüft ob es ein gültiger Regex ist.</summary>
    /// <param name="toValidate">String welcher den Regex enthält.</param>
    private static void CheckRegex(string toValidate) {
      try {
        new Regex(toValidate);
      } catch(ArgumentException e) {
        Error("No valid regex: " + e.Message + "...", token);
      }
    }
    #endregion
    
    #region Nonterminalsymbol RelaxngOutputBlock
    /// <summary>Parst den Relaxng-Output-Block. Fügt das Start-Element in den <c>outputTree</c> ein.</summary>
    /// <example>
    ///   Bsp 1: start = Address {RelaxngStatement}
    /// </example>
    private static void RelaxngOutputBlock() {
      Check(TokenCode.START, TokenCode.ASSIGN, TokenCode.IDENT);
      outputTree = new BinaryTree<Token>(new TreeNode<Token>(token));
      while(la == TokenCode.IDENT) {
        RelaxngStatement();          
      }
    }
    
    /// <summary>Parst ein Relaxng-Statement. Füllt die <c>outputHelperStruct</c> mit den deklarierten
    /// Relaxng-Statements. Überprüft das kein Statement doppelt deklariert ist.</summary>
    /// <example>
    ///   Bsp 1: Address = RelaxngExpr
    /// </example>
    private static void RelaxngStatement() {
      Check(TokenCode.IDENT);
      if(outputHelperStruct.ContainsKey(token.Value)) {
        Error("Multiple defined output element...", token);
      } else {
        TreeNode<Token> statement = new TreeNode<Token>(token);
        outputHelperStruct.Add(token.Value, new BinaryTree<Token>(statement));
        Check(TokenCode.ASSIGN);
        statement.Left = RelaxngExpr();
      }
    }

    /// <summary>Parst eine Relaxng-Expression.</summary>
    /// <example>
    ///   Bsp 1: element address { Name, Surname+, ... }
    ///   Bsp 2: element name { text }
    /// </example>
    /// <returns>Gibt einen TreeNode zurück der eine ganze RelaxngExpr enthält.</returns>
    private static TreeNode<Token> RelaxngExpr() {
      Check(TokenCode.ELEMENT, TokenCode.IDENT);
      TreeNode<Token> expr = new TreeNode<Token>(token);
      Check(TokenCode.LBRACE);
      switch(la) {
        case TokenCode.TEXT:
          Scan(1);
          expr.Left = new TreeNode<Token>(token);
        break;
        case TokenCode.IDENT:
          expr.Left = RelaxngElement();
          
          // Falls es noch weitere RelaxngFactors gibt
          // KARL TODO: es wird hier keine Präzedenz unterstützt -> Faktor einführen
          while(la == TokenCode.COMMA || la == TokenCode.OR) {
            Scan(1);
            TreeNode<Token> comma = new TreeNode<Token>(token);
            comma.Left = expr.Left;
            comma.Right = RelaxngElement();
            expr.Left = comma;
          }
        break;
        default:
          Check(TokenCode.TEXT);
        break;
      }
      Check(TokenCode.RBRACE);  
      return expr;
    }
    
    /// <summary>Parst ein RelaxngElement.</summary>
    /// <example>
    ///   Bsp 1: Name
    ///   Bsp 2: Surname*
    /// </example>
    /// <returns>Gibt einen TreeNode mit dem RelaxngElement mit allenfalls voran gehängten Quantifier zurück.</returns>
    private static TreeNode<Token> RelaxngElement() {
      Check(TokenCode.IDENT);
      TreeNode<Token> element = new TreeNode<Token>(token);
      TreeNode<Token> quant = Quant();
      if(quant != null) {
        quant.Left = element;
        return quant;
      }
      return element;
    }
    #endregion
    
    #region Nonterminalsymbol AnalyserRuleBlock
    /// <summary>Parst den Analyser-Rule-Block.</summary>
    /// <example>
    ///   Bsp 1:  start = Address {AnalyserRuleStatement}
    /// </example>
    private static void AnalyserRuleBlock() {
      Check(TokenCode.START, TokenCode.ASSIGN, TokenCode.IDENT);
      analyserTree = new BinaryTree<Token>(new TreeNode<Token>(token));
      while(la == TokenCode.IDENT) {
        AnalyserRuleStatement();          
      }
    }

    /// <summary>Parst ein Analyser-Rule-Statement. Füllt die <c>analyserHelperStruct</c> mit den deklarierten
    /// AnalyserRule-Statements. Überprüft das kein Statement doppelt deklariert ist.</summary>
    /// <example>
    ///   Bsp 1: Address = AnalyserRuleExpr
    /// </example>
    private static void AnalyserRuleStatement() {
      Check(TokenCode.IDENT);
      if(analyserHelperStruct.ContainsKey(token.Value)) {
        Error("Multiple defined analyser element...", token);
      } else {
        TreeNode<Token> statement = new TreeNode<Token>(token);
        analyserHelperStruct.Add(token.Value, new BinaryTree<Token>(statement));
        Check(TokenCode.ASSIGN);
        statement.Left = AnalyserRuleExpr();
      }
    }

    /// <summary>Parst eine Analyser-Rule-Expression.</summary>
    /// <example>
    ///   Bsp 1: AnalyserRuleTerm { "|" AnalyserRuleTerm }
    /// </example>
    /// <returns>Gibt einen TreeNode zurück der eine ganze AnalyserRuleExpr enthält.</returns>
    private static TreeNode<Token> AnalyserRuleExpr() {
      TreeNode<Token> expr = AnalyserRuleTerm();
      
      // Falls es noch weitere AnalyserRuleTerms gibt
      while(la == TokenCode.OR) {
        Scan(1);
        
        TreeNode<Token> or = new TreeNode<Token>(token);
        or.Left = expr;
        or.Right = AnalyserRuleTerm();
        expr = or;
      }
      return expr;
    }

    /// <summary>Parst ein Analyser-Rule-Term.</summary>
    /// <example>
    ///   Bsp 1: AnalyserRuleFactor { ("," | "&" AnalyserRuleFactor }
    /// </example>
    /// <returns>Gibt einen TreeNode zurück der einen ganzen AnalyserRuleTerm enthält.</returns>
    private static TreeNode<Token> AnalyserRuleTerm() {
      TreeNode<Token> term = AnalyserRuleFactor();
      
      // Falls es noch weitere AnalyserRuleFactors gibt
      while(((la == TokenCode.COMMA) && (l2a != TokenCode.EXTCALL)) || (la == TokenCode.AND)) {
        Scan(1);
        
        TreeNode<Token> op = new TreeNode<Token>(token);
        op.Left = term;
        op.Right = AnalyserRuleFactor();
        term = op;
      }
      return term;
    }

    /// <summary>Parst ein AnalyserRuleFactor.</summary>
    /// <example>
    ///   Bsp 1: Name "{" AnalyserRuleElement "}"
    ///   Bsp 2: PlzCity*
    ///   Bsp 3: $Pattern3
    ///   Bsp 4: (AnalyserRuleExpr)+
    /// </example>
    /// <returns>Gibt einen TreeNode zurück der einen ganzen AnalyserFactor enthält.</returns>
    private static TreeNode<Token> AnalyserRuleFactor() {
      TreeNode<Token> factor = null;
      TreeNode<Token> quant = null;
      // Karl
      TokenCode type = la;
      switch(la) {
        case TokenCode.IDENT:
          Scan(1);
          factor = new TreeNode<Token>(token);
          if(la == TokenCode.LBRACE) {
            Scan(1);
            factor.Left = AnalyserRuleExpr();
            Check(TokenCode.RBRACE);
          }
          quant = Quant();
          if(quant != null) {
            quant.Left = factor;
            factor = quant;
          }
        break;
        case TokenCode.VARIDENT:
        
        case TokenCode.STRINGCONST:
          factor = AnalyserRuleElement();
        break;
        case TokenCode.LPAR:
          Scan(1);
          factor = AnalyserRuleExpr();
          Check(TokenCode.RPAR);
          quant = Quant();    
          if(quant != null) {
            quant.Left = factor;
            factor = quant;
          }
        break;
        case TokenCode.ATOM:
          Scan(1);
          factor = new TreeNode<Token>(token);
          Check(TokenCode.LPAR);
          factor.Left = AnalyserRuleExpr();
          Check(TokenCode.RPAR);
          quant = Quant();    
          if(quant != null) {
            quant.Left = factor;
            factor = quant;
          }
        break;
        case TokenCode.CHECK:
          Scan(1);
          factor = new TreeNode<Token>(token);
          Check(TokenCode.LPAR);
          factor.Left = AnalyserRuleExpr();
          Check(TokenCode.COMMA, TokenCode.EXTCALL);
          factor.Right = new TreeNode<Token>(token);

          // anderpas & schuejen
          var outputNode = outputHelperStruct[factor.Left.Item.Value].Root;
          var outputName = outputNode.Left.Item.Value;
          if(!extCallMethodList.ContainsKey(outputName)) {
            extCallMethodList.Add(outputName, token.Value);
          }

          Check(TokenCode.RPAR);
          quant = Quant();    
          if(quant != null) {
            quant.Left = factor;
            factor = quant;
          }
        break;
        default:
          Check(TokenCode.IDENT);
        break;
      }
      log.Debug("AnalyserRuleFactor:"+type+" "+factor.Item.Value);
      return factor;
    }
    
    /// <summary>Parst ein AnalyserRuleElement.</summary>
    /// <example>
    ///   Bsp 1: $Pattern1
    ///   Bsp 2: $Pattern2*
    ///   Bsp 3: "regex"
    /// </example>
    /// <returns>Gibt einen TreeNode mit dem AnalyserRuleElement mit allenfalls voran gehängten Quantifier zurück.</returns>
    private static TreeNode<Token> AnalyserRuleElement() {
    	
      TreeNode<Token> element = null;
      switch(la) {
        case TokenCode.VARIDENT:
          Scan(1);
          element = new TreeNode<Token>(token);
          TreeNode<Token> quant = Quant();          
          if(quant != null) {
            quant.Left = element;
            return quant;
          }
        break;
        case TokenCode.STRINGCONST: 
          StringConcat();
          token.Value = ExpandPattern(token.Value);

          CheckRegex(token.Value);
          element = new TreeNode<Token>(token);
        break;
        default:
          Check(TokenCode.VARIDENT);
        break;
      }
      log.Debug("AnalyserRuleElement:"+element.Item.Value);
      return element;
    }
    #endregion
    
    #region Nonterminalsymbol PostProcessBlock    
    /// <summary>???????????????</summary>
    private static void PostProcessBlock() {
      // not yet implemented...
    }
    #endregion
    
    #region Nonterminalsymbol PreProcessBlock
    /// <summary>???????????????</summary>
    private static void PreProcessBlock() {
      // not yet implemented...
    }
    #endregion
  }
}