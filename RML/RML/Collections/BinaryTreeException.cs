/*
 * Namespace: Iib.RegexMarkupLanguage.Collections
 * File:      BinaryTreeException.cs
 * Version:   1.0
 * Date:      07.07.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System;

namespace Iib.RegexMarkupLanguage.Collections {

  /// <summary>Allgemeine Exception vom BinaryTree.</summary>
  public class BinaryTreeException<NodeT> : ApplicationException {
    private TreeNode<NodeT> node;
    
    /// <summary>Erzeugt eine <c>BinaryTreeException</c>.<summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="e">Original Exception, welche aufgetreten ist.</param>
    /// <param name="node">TreeNode welcher einen Fehler verursacht hat.</param>
    public BinaryTreeException(string message, Exception e, TreeNode<NodeT> node) : base(message, e) {
      this.node = node;
    }
    
    /// <summary>Erzeugt eine <c>BinaryTreeException</c>.<summary>
    /// <param name="message">Fehlermeldung</param>
    /// <param name="node">TreeNode welcher einen Fehler verursacht hat.</param>
    public BinaryTreeException(string message, TreeNode<NodeT> node) : base(message) {
      this.node = node;
    }
   
    /// <summary>Gibt den TreeNode zurück der einen Fehler verursacht hat.</summary>
    public TreeNode<NodeT> Node {
      get { return node; }    
    } 
  }
}
