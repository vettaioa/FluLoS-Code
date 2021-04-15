/*
 * Namespace: Iib.RegexMarkupLanguage.Collections
 * File:      Visitor.cs
 * Version:   1.0
 * Date:      03.07.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System.Text;
using System.Collections.Generic;
using System;
using log4net;

namespace Iib.RegexMarkupLanguage.Collections {
 
  /// <summary>Interface, welches alle Visitoren implementieren müssen wenn sie im <c>BinaryTree</c>
  /// verwendet werden.</summary>
  /// <typeparam name="NodeT"></typeparam>
  public interface IVisitor<NodeT> {
    bool CheckCycles { get;}
    void Visit(TreeNode<NodeT> toVisit);
  }
  
  /// <summary>Visitor der einzelne Teilbäume zu einem ganzen zusammen setzt.</summary>
  /// <remarks>Damit die Teilbäume ebenfalls traversiert werden, sollte dieser Visitor nur mit der Traversier-Methode
  /// <c>BinaryTree.PreOrderTraverse()</c> verwendet werden.</remarks>
  /// <seealso cref="Iib.TextAnalyzer.Collections.BinaryTree.PreOrderTraverse"/>
  public class GenerateOutputTreeVisitor : IVisitor<Token> {
    private IDictionary<string, BinaryTree<Token>> trees;
    private bool checkCycles;    
  
    /// <summary>Erzeugt einen <c>GenerateOutputTreeVisitor</c>.</summary>
    /// <param name="trees">Hashtable mit den einzelnen Teilbäumen.</param>
    /// <param name="checkCycles">Wenn true wird beim Traversieren auf Zyklen überprüft.</param>
    public GenerateOutputTreeVisitor(IDictionary<string, BinaryTree<Token>> trees, bool checkCycles) {
      this.trees = trees;
      this.checkCycles = checkCycles;
    }
    
    /// <summary>Wenn <c>CheckCycles</c> true zurück gibt, muss beim Traversieren auf Zyklen überprüft werden.</summary>
    public bool CheckCycles {
      get { return checkCycles; }
    }
    
    /// <summary>Wenn der besuchte TreeNode <c>toVisit</c> keine weiteren Nodes hat und vom Typ <c>TokenCode.IDENT</c>
    /// ist, wird in der Hashtable <c>trees</c> nach dem Baum mit dem Root-Node <c>toVisit.Item.Value</c> gesucht.
    /// Wird ein solcher Teilbaum gefunden wird er mit dem bestehenden Baum verknüpft.</summary>
    /// <param name="toVisit">TreeNode welcher gerade verarbeitet wird.</param>
    /// <exception cref="Iib.TextAnalyzer.Collections.BinaryTreeException">Wenn ein Teilbaum nicht in der Hashtable
    /// gefunden wird.</exception>
    public void Visit(TreeNode<Token> toVisit) {
      if((toVisit.Left == null) && (toVisit.Item.Kind == TokenCode.IDENT)) {
        try {
          BinaryTree<Token> partOfTree = trees[toVisit.Item.Value];
          partOfTree.Root.Order = 0;
          toVisit.Left = partOfTree.Root.Left;
          toVisit.Right = partOfTree.Root.Right;
        } catch(KeyNotFoundException) {
          throw new BinaryTreeException<Token>("Output element '" + toVisit.Item.Value + "' used but not defined...", toVisit);
        }
      }
    }
  }
  
  /// <summary>Visitor der einzelne Teilbäume zu einem ganzen zusammen setzt und Patterns auflöst.</summary>
  /// <remarks>Damit die Teilbäume ebenfalls traversiert werden, sollte dieser Visitor nur mit der Traversier-Methode
  /// <c>BinaryTree.PreOrderTraverse()</c> verwendet werden.</remarks>
  /// <seealso cref="Iib.TextAnalyzer.Collections.BinaryTree.PreOrderTraverse"/>
  public class GenerateAnalyserTreeVisitor : IVisitor<Token> {
    private IDictionary<string, BinaryTree<Token>> trees;  
    private IDictionary<string, TreeNode<Token>> patterns;
    private static readonly ILog log = LogManager.GetLogger(typeof(GenerateAnalyserTreeVisitor));
    private bool checkCycles;
  
    /// <summary>Erzeugt einen <c>GenerateAnalyserTreeVisitor</c>.</summary>
    /// <param name="trees">Hashtable mit den einzelnen Teilbäumen.</param>
    /// <param name="patterns">Hashtable mit Patterns.</param>
    /// <param name="checkCycles">Wenn true wird beim Traversieren auf Zyklen überprüft.</param>
    public GenerateAnalyserTreeVisitor(IDictionary<string, BinaryTree<Token>> trees, IDictionary<string,
                                       TreeNode<Token>> patterns, bool checkCycles) {
      this.trees = trees;
      this.patterns = patterns;
      this.checkCycles = checkCycles;
    }
    
    /// <summary>Wenn <c>CheckCycles</c> true zurück gibt, muss beim Traversieren auf Zyklen überprüft werden.</summary>
    public bool CheckCycles {
      get { return checkCycles; }
    }
    
    /// <summary>Wenn der besuchte TreeNode <c>toVisit</c> keine weiteren Nodes hat und vom Typ <c>TokenCode.IDENT</c>
    /// ist, wird in der Hashtable <c>trees</c> nach dem Baum mit dem Root-Node <c>toVisit.Item.Value</c> gesucht.
    /// Wird ein solcher Teilbaum gefunden wird er mit dem bestehenden Baum verknüpft. Ist der Node vom Typ
    /// <c>TokenCode.VARIDENT</c> wird in der Hashtable <c>patterns</c> nach dem Pattern gesucht und der Wert im Baum
    /// eingefügt.</summary>
    /// <param name="toVisit">TreeNode welcher gerade verarbeitet wird.</param>
    /// <exception cref="Iib.TextAnalyzer.Collections.BinaryTreeException">Wenn ein Teilbaum oder ein Pattern nicht in
    /// der Hashtable gefunden wird.</exception>
    public void Visit(TreeNode<Token> toVisit) {
      if((toVisit.Left == null) && (toVisit.Item.Kind == TokenCode.IDENT)) {
        try {
          BinaryTree<Token> partOfTree = trees[toVisit.Item.Value];
          partOfTree.Root.Order = 0;
          toVisit.Left = partOfTree.Root.Left;
          toVisit.Right = partOfTree.Root.Right;
        } catch(KeyNotFoundException) {
          throw new BinaryTreeException<Token>("Analyser element '" + toVisit.Item.Value + "' used but not defined...", toVisit);
        }
      } else if(toVisit.Item.Kind == TokenCode.VARIDENT && !toVisit.IsVisited) {
        try {
    	  
    	   // KARL
    	   
    	  log.Debug("visit:"+toVisit.Item.Value+" "+toVisit.Item.Kind+" "+toVisit.IsVisited);
          TreeNode<Token> p = patterns[toVisit.Item.Value];
          
          p.Order = 0;
          toVisit.Item.Value = p.Item.Value;
          toVisit.IsVisited = true;
        } catch(KeyNotFoundException) {
         throw new BinaryTreeException<Token>("Pattern '" + toVisit.Item.Value + "' used but not defined...", toVisit);
        }
      }
    }
  }
  
  /// <summary>Dieser Visitor wird gebraucht um das Count-Property jedes Nodes auf 0 zu setzen. Dies ist nötig wenn man
  /// den selben Baum mehrmals nacheinander verwenden will.</summary>
  public class ResetCountVisitor : IVisitor<Token> {
    private bool checkCycles;
    
    /// <summary>Erzeugt einen <c>ResetCountVisitor</c>.</summary>
    /// <param name="checkCycles">Wenn true wird beim Traversieren auf Zyklen überprüft.</param>
    public ResetCountVisitor(bool checkCycles) {
      this.checkCycles = checkCycles;
    }
    
    /// <summary>Wenn <c>CheckCycles</c> true zurück gibt, muss beim Traversieren auf Zyklen überprüft werden.</summary>
    public bool CheckCycles {
      get { return checkCycles; }
    }
  
    /// <summary>Diese Methode verarbeitet einen Node.</summary>
    /// <param name="toVisit">Node der Verarbeit wird.</param>
    public void Visit(TreeNode<Token> toVisit) {
      toVisit.Count = 0;
    }
  }
  
  /// <summary>Dieser Visitor wird für das Debuging der beiden Bäume Analyser- und OutputTree gebraucht. Der Visitor
  /// fügt die Werte der Nodes, die der <c>Visit</c>-Methode übergeben werden, zu einem String zusammen. Auf den
  /// vollständige String kann nach dem Traversieren eines Baums mittels <c>TreeString</c><c> zugegriffen werden.
  /// </summary>
  public class DebugVisitor : IVisitor<Token> {
    private StringBuilder treeString;
    private bool checkCycles;
    
    /// <summary>Erzeugt einen <c>DebugVisitor</c>.</summary>
    /// <param name="checkCycles">Wenn true wird beim Traversieren auf Zyklen überprüft.</param>
    public DebugVisitor(bool checkCycles) {
      this.treeString = new StringBuilder();
      this.checkCycles = checkCycles;
    }    
    
    /// <summary>Wenn <c>CheckCycles</c> true zurück gibt, muss beim Traversieren auf Zyklen überprüft werden.</summary>
    public bool CheckCycles {
      get { return checkCycles; }
    }
    
    public void Visit(TreeNode<Token> toVisit) {
      if(toVisit.Item.Kind == TokenCode.COMMA) {
        treeString.Append(",");
      } else if(toVisit.Item.Kind == TokenCode.TEXT) {
        treeString.Append("text");
      } else if(toVisit.Item.Kind == TokenCode.ATOM) {
        treeString.Append("atom");
      } else if(toVisit.Item.Kind == TokenCode.CHECK) {
        treeString.Append("check");
      } else if(toVisit.Item.Kind == TokenCode.ANY) {
        treeString.Append("*");
      } else if(toVisit.Item.Kind == TokenCode.PLUS) {
        treeString.Append("+");
      } else if(toVisit.Item.Kind == TokenCode.OPTIONAL) {
        treeString.Append("?");
      } else if(toVisit.Item.Kind == TokenCode.OR) {
        treeString.Append("|");
      } else {       
        treeString.Append(toVisit.Item.Value);
      }
    }
    
    public string TreeString {
      get { return treeString.ToString(); }
    }
  }
}
