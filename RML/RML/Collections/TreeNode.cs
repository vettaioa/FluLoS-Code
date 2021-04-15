/*
 * Namespace: Iib.RegexMarkupLanguage.Collections
 * File:      TreeNode.cs
 * Version:   1.0
 * Date:      30.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

namespace Iib.RegexMarkupLanguage.Collections {

  /// <summary><c>TreeNode</c> wird als Node im <c>BinaryTree</c> verwendet. Der Node hat einen Zeiger auf einen
  /// linken und einen rechten Node. Weiter wir im Node ein Item gespeichert.</summary>
  /// <typeparam name="NodeT">Type des Items, welches im Node gespeichert wird.</typeparam>
  /// <seealso cref="Iib.TextAnalyzer.Collections.BinaryTree"/>
  public class TreeNode<NodeT> {
    private NodeT item;
    private int order;
    private int count;
    private TreeNode<NodeT> parent;
    private TreeNode<NodeT> left;
    private TreeNode<NodeT> right;
    
    // KARL
    public TreeNode<NodeT> Clone() {
    	TreeNode<NodeT> copy = new TreeNode<NodeT>(item);
    	copy.order = order;
    	copy.count = count;
    	copy.parent = parent;
    	copy.left = left;
    	copy.right = right;
    	copy.isVisited = isVisited;
    	return copy;
    }
    
    private bool isVisited;
    public bool IsVisited {
    	get{return isVisited;}
    	set {isVisited = value;}
    }
    /// <summary>Erzeugt ein <c>TreeNode</c> Objekt mit dem Item das übergeben wird.</summary>
    /// <param name="item">Item das im Node gespeichert wird.</param>
    public TreeNode(NodeT item) {
      this.item = item;
      order = -1;
      count = 0;
      parent = null;
      left = null;
      right = null;
    }
    
    /// <summary>Gibt das im Node gespeicherte Item zurück oder speichert ein Item im Node.</summary>
    public NodeT Item {
      get { return item; }
      set { this.item = value; }
    }
    
    /// <summary>Nummer innerhalb der Reihenfolge, in der der TreeNodes rekursiv traversiert wird. Die Nummer
    /// wird gesetzt sobald die Traversierung den TreeNode erreicht und nicht erst bei der Verarbeitung</summary>
    /// <remarks>-1 bedeutet, dass der TreeNode noch nicht erreicht wurde.</remarks>
    public int Order {
      get { return order; }
      set { order = value; }
    }
    
    /// <summary>Kann frei gesetzt und gelesen werden. Wird z.B. gebraucht um beim Durchlaufen des Baums die
    /// Wiederholungen zu zählen.</summary>
    public int Count {
      get { return count; }
      set { count = value; }
    }
    
    /// <summary>Gibt den vorgänger Node dieses Nodes zurück oder speichert einen Node als vorgänger Node.</summary>
    public TreeNode<NodeT> Parent {
      get { return parent; }
      set { parent = value; }
    }
    
    /// <summary>Gibt den linken Node dieses Nodes zurück oder speichert einen Node als linken Node.</summary>
    /// <remarks>Setzt automatisch beim Node den Parent-Node.</remarks>
    public TreeNode<NodeT> Left {
      get { return left; }
      set {
        left = value;
        if(left != null) {
          left.parent = this;
        }
      }
    }
    
    /// <summary>Gibt den rechten Node dieses Nodes zurück oder speichert einen Node als rechten Node.<summary>
    /// <remarks>Setzt automatisch beim Node den Parent-Node.</remarks>
    public TreeNode<NodeT> Right {
      get { return right; }
      set {
        right = value; 
        if(right != null) {
          right.parent = this;
        }
      }
    }
  }
}
