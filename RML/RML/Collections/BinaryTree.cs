/*
 * Namespace: Iib.RegexMarkupLanguage.Collections
 * File:      BinaryTree.cs
 * Version:   1.0
 * Date:      30.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System;
using System.Collections.Generic;

namespace Iib.RegexMarkupLanguage.Collections {

  /// <summary><c>BinaryTree</c> ist eine Klasse welche einen Root-Node vom Typ <c>TreeNode</c> besitzt. Die
  /// Klasse stellt Traversierungs-Methoden für den Baum zur Verfügung.</summary>
  /// <remarks>Die Traversierungs-Methoden können überprüfen ob im Baum einen Zyklus vorhanden ist und brechen
  /// bei einem Zyklus mit einer <c>BinaryTreeException</c> ab. Das Flag <c>CheckCycles</c> im Visitor muss dazu
  /// auf true gesetzt werden.</remarks>
  /// <typeparam name="NodeT"></typeparam>
  public class BinaryTree <NodeT> {
    private TreeNode<NodeT> rootNode;
    private int order;
    private IList<bool> processed;
    
    /// <summary>Erstellt einen <c>BinaryTree</c>. Der Baum wird mit dem <c>rootNode</c> initialisiert.<summary>
    /// <param name="rootNode">Root-Node des Baums</param>
    public BinaryTree(TreeNode<NodeT> rootNode) {
      this.rootNode = rootNode;
      this.rootNode.Parent = null;
      processed = new List<bool>();
    }
    
    /// <summary>Gibt den Root-Node zurück.</summary>
    public TreeNode<NodeT> Root {
      get { return rootNode; }
    }
    
    /// <summary>Traversiert den Baum vom Root-Node aus in PreOrder-Reihenfolge durch.</summary>
    /// <param name="visitor">Visitor welcher den Node verarbeitet.</param>
    public void PreOrderTraverse(IVisitor<NodeT> visitor) {
      order = 0;
      processed.Clear();
      if(rootNode != null) {
        if(visitor.CheckCycles) {
          PreOrderTraverseSafe(rootNode, visitor);
        } else {
          PreOrderTraverseUnSafe(rootNode, visitor);
        }
      }
    }
    
    /// <summary>Traversiert den Baum vom <c>node</c> aus in PreOrder-Reihenfolge durch. Diese Methode ruft sich selber
    /// auf und bewirkt das der ganze Baum durch traversiert wird.</summary>
    /// <remarks>Der Name "UnSafe" der Methode bezieht sich darauf, dass keine Überprüfung auf Zyklen beim Traversieren
    /// des Baums gemacht werden.</remarks>
    /// <param name="node">Node von dem weiter traversiert wird.</param>
    /// <param name="visitor">Visitor welcher den Node verarbeitet.</param>
    private void PreOrderTraverseUnSafe(TreeNode<NodeT> node, IVisitor<NodeT> visitor) {
      visitor.Visit(node);
      if(node.Left != null) {
        PreOrderTraverseUnSafe(node.Left, visitor);
      }
      if(node.Right != null) {
        PreOrderTraverseUnSafe(node.Right, visitor);
      }
    }
    
    /// <summary>Traversiert den Baum vom <c>node</c> aus in PreOrder-Reihenfolge durch. Diese Methode ruft sich selber
    /// auf und bewirkt das der ganze Baum durch traversiert wird. Es wird überprüft ob der Baum einen Zyklus hat. Bei
    /// einem Zyklus wird die Traversierung abgebrochen.</summary>
    /// <remarks>Der Name "Safe" der Methode bezieht sich darauf, dass auf Zyklen überprüft wird beim Traversieren des
    /// Baums.</remarks>
    /// <param name="node">Node von dem weiter traversiert wird.</param>
    /// <param name="visitor">Visitor welcher den Node verarbeitet.</param>
    /// <exception cref="BinaryTreeException">Im Baum ist ein Zyklus enthalten.</exception>
    private void PreOrderTraverseSafe(TreeNode<NodeT> node, IVisitor<NodeT> visitor) {
      node.Order = order++;
      processed.Add(false);
      visitor.Visit(node);
      if(node.Left != null) {
        if((node.Left.Order == -1) || (node.Order < node.Left.Order) || processed[node.Left.Order]) {
          PreOrderTraverseSafe(node.Left, visitor);
        } else {
          throw new BinaryTreeException<NodeT>("An element caused a cycle...", node);
        }
      }
      if(node.Right != null) {
        if((node.Right.Order == -1) || (node.Order < node.Right.Order) || processed[node.Right.Order]) {
          PreOrderTraverseSafe(node.Right, visitor);
        } else {
          throw new BinaryTreeException<NodeT>("An element caused a cycle...", node);
        }
      }
      processed[node.Order] = true;
    }
  }
}
