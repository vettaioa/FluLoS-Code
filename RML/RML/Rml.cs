/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      Regex2Xml.cs
 * Version:   1.0
 * Date:      14.07.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */

using System.Xml;
using Iib.RegexMarkupLanguage.Collections;
using Iib.RegexMarkupLanguage.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using log4net;
using System.Threading;
using Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls;

[assembly: log4net.Config.XmlConfigurator()]
namespace Iib.RegexMarkupLanguage
{

  /// <summary>Dieser Klasse kann ein Script und Input-Daten übergeben werden. Daraus wir ein XML-Dokument
  /// mit den entsprechenden Daten generiert.</summary>
  /// <remarks> Die Klasse ist Thread-Safe.</remarks>
  public class Rml
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(Rml));
    private BinaryTree<Token> outputTree;
    private Regex regex;
    private int timeout;

    /// <summary>Erstellt ein Rml Objekt. Kompilliert das Script und den daraus entstehenden Regex.</summary>
    /// <param name="scriptName">Dateiname des Scripts, das kompilliert werden soll (Vollständiger Pfad).</param>
    /// <param name="exceptions">Out-Parameter in den Warnungen und Fehler gespeichert werden.</param>
    public Rml(string scriptName, out IEnumerable<CompilerException> exceptions) : this(scriptName, out exceptions, 0, "", "", @"[ \t\n\r]*") { }

    /// <summary>Erstellt ein Rml Objekt. Kompilliert das Script und den daraus entstehenden Regex.</summary>
    /// <param name="scriptName">Dateiname des Scripts, das kompilliert werden soll (Vollständiger Pfad).</param>
    /// <param name="exceptions">Out-Parameter in den Warnungen und Fehler gespeichert werden.</param>
    /// <param name="timeout">Timeout in Sekunden nachdem das Matchen des Regex auf die Daten abgebrochen wird. 0 bedeutet, dass es kein Timeout gibt.</param>
    public Rml(string scriptName, out IEnumerable<CompilerException> exceptions, int timeout) : this(scriptName, out exceptions, timeout, "", "", @"[ \t\n\r]*") { }

    /// <summary>Erstellt ein Rml Objekt. Kompilliert das Script und den daraus entstehenden Regex.</summary>
    /// <param name="scriptName">Dateiname des Scripts, das kompilliert werden soll (Vollständiger Pfad).</param>
    /// <param name="exceptions">Out-Parameter in den Warnungen und Fehler gespeichert werden.param>
    /// <param name="timeout">Timeout in Sekunden nachdem das Matchen des Regex auf die Daten abgebrochen wird. 0 bedeutet, dass es kein Timeout gibt.</param>
    /// <param name="prefix">Regex, welcher an den Anfang des generierten Regex gestellt wird.</param>
    /// <param name="suffix">Regex, welcher an das Ende des generierten Regex gestellt wird.</param>
    /// <param name="delimiter">Regex, welcher zwischen jeden Teilregex eingesetzt wird.</param>
    /// <exception cref="CompilerException">Fehler beim Compilieren.</exception>
    public Rml(string scriptName, out IEnumerable<CompilerException> exceptions, int timeout, string prefix, string suffix, string delimiter)
    {
      log.Info("Initialize the Regex Markup Language (RML)...");
      this.timeout = timeout;
      string analyserRegex;
      IDictionary<string, string> extCallMethods;
      Compiler.Compile(scriptName, prefix, suffix, delimiter, out exceptions, out outputTree, out analyserRegex, out extCallMethods);
      regex = new Regex(analyserRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
      ExternalCallBuilder.createInstance().loadMethods(extCallMethods);
    }

    /// <summary>Matched den Regex auf die <c>inputData</c> und erzeugt die Output-XML-Datei.</summary>
    /// <param name="inputData">String auf welchen der Regex angewendet werden soll.</param>
    /// <returns>XML-Dokument mit den Output-Daten.</returns>
    /// <exception cref="RmlException">Wenn ein Timeout auftritt oder es keinen Match gibt.</exception>
    public XmlDocument Execute(string inputData)
    {
      lock(this)
      {
        // Der Match des Regex ist Timeout überwacht
        Match matchResult = null;
        Thread matchThread = new Thread(delegate () {
          try
          {
            matchResult = regex.Match(inputData);
          } catch(Exception e)
          {
            log.Error(e.Message);
          }
        });
        matchThread.Start();
        if(timeout > 0)
        {
          if(!matchThread.Join(timeout * 1000))
          {
            matchThread.Abort();
            matchThread.Join();
            log.Warn("Timeout has reached. Please check the script and the input data...");
            throw new RmlException("Timeout has reached. Please check the script and the input data...");
          }
        } else
        {
          matchThread.Join();
        }

        XmlDocument outputDoc = new XmlDocument();
        if((matchResult != null) && matchResult.Success)
        {
          foreach(Group g in matchResult.Groups)
          {
            g.ExpandCaptureCollection();
          }
          outputTree.PreOrderTraverse(new ResetCountVisitor(false));
          AppendChildsToXmlNode(outputDoc, EvalOutputTree(outputTree.Root, outputDoc, matchResult));
        } else
        {
          log.Warn("No match for input: '" + inputData + "'...");
          throw new RmlException("No match for input: '" + inputData + "'...");
        }
        log.Info("Successful executed RML...");
        return outputDoc;
      }
    }

    /// <summary>Wertet rekursiv den OutputTree aus. Geht alle Nodes durch und erstellt eine Liste mit XML-Nodes mit den
    /// Daten aus dem Regex.</summary>
    /// <param name="node">TreeNode welcher ausgewertet werden soll.</param>
    /// <param name="xmlDoc">Wird gebraucht um XML-Elemente zu erstellen.</param>
    /// <param name="matched">Daten welche in die XML-Elemente als Werte eingefügt werden.</param>
    /// <returns>Liste mit allen XML-Nodes die es für den TreeNode <c>node</c> gibt.</returns>
    private IList<XmlNode> EvalOutputTree(TreeNode<Token> node, XmlDocument xmlDoc, Match data, string parentName = "")
    {
      List<XmlNode> childs = new List<XmlNode>();
      XmlNode element;
      switch(node.Item.Kind)
      {
        case TokenCode.IDENT:                                                            // ident = element ident
          element = xmlDoc.CreateElement(node.Left.Item.Value);
          AppendChildsToXmlNode(element, EvalOutputTree(node.Left.Left, xmlDoc, data, parentName = node.Left.Item.Value));
          childs.Add(element);
          break;
        case TokenCode.TEXT:                                                             // { text }
          string value = GetValueFromMatch(node.Parent.Parent, data);

          if((value != null) && (value != String.Empty))
          {
            value = ExternalCallBuilder.getInstance().adaptValue(parentName, value);
            childs.Add(xmlDoc.CreateTextNode(value.Trim()));
          }
          break;
        case TokenCode.COMMA:                                                            // { ident, ident, ...}  
          childs.AddRange(EvalOutputTree(node.Left, xmlDoc, data));
          childs.AddRange(EvalOutputTree(node.Right, xmlDoc, data));
          break;
        // KARL
        case TokenCode.OR:                                                              // { ident | ident | ...}  
          IList<XmlNode> elementList = EvalOutputTree(node.Left, xmlDoc, data);
          if(elementList.Count > 0 && HasTextNode(elementList[0]))
          {
            childs.AddRange(elementList);
          } else
          {
            elementList = EvalOutputTree(node.Right, xmlDoc, data);
            if(elementList.Count > 0)
            {
              childs.AddRange(elementList);
            }
          }
          break;
        case TokenCode.ANY:                                                              // { ident* }
          while(HasTextNode((element = EvalOutputTree(node.Left, xmlDoc, data)[0])))
          {
            childs.Add(element);
          }
          break;
        case TokenCode.PLUS:                                                             // { ident+ }
          element = EvalOutputTree(node.Left, xmlDoc, data)[0];
          do
          {
            childs.Add(element);
          } while(HasTextNode((element = EvalOutputTree(node.Left, xmlDoc, data)[0])));
          break;
        case TokenCode.OPTIONAL:                                                         // { ident? }
          element = EvalOutputTree(node.Left, xmlDoc, data)[0];
          if(HasTextNode(element))
          {
            childs.Add(element);
          }
          break;
      }
      return childs;
    }

    /// <summary>Fügt alle Nodes von <c>childs</c> dem XML-Node <c>parent</c> als Kind-Element hinzu.</summary>
    /// <param name="parent">XML-Node dem Nodes hinzugefügt werden.</param>
    /// <param name="childs">Liste mit Nodes, die dem XML-Node hinzugefügt werden.</param>
    private void AppendChildsToXmlNode(XmlNode parent, IList<XmlNode> childs)
    {
      foreach(XmlNode node in childs)
      {
        parent.AppendChild(node);
      }
    }

    /// <summary>Gibt den Wert zurück, welcher durch den Regex unter dem Namen <c>node.Item.Value</c> gespeichert ist.
    /// Es wird der aktuelle Wert <c>node.Count</c> zurück gegeben. Falls es keine Werte mehr gibt, wird <c>null</c>
    /// zurück geliefert.</summary>
    /// <param name="node">Name des Wertes, welcher gesucht wird.</param>
    /// <param name="data">Daten welche durch den Regex gematched wurden.</param>
    /// <returns>Wert oder <c>null</c> wenn es keinen Wert mehr gibt.</returns>
    private string GetValueFromMatch(TreeNode<Token> node, Match data)
    {
      if(node.Count < data.Groups[node.Item.Value].Captures.Count)
      {
        return data.Groups[node.Item.Value].Captures[node.Count++].Value;
      }
      return null;
    }

    /// <summary>Gibt <code>true</code> zurück wenn das Element mindestens einen Text-Node besitzt.</summary>
    /// <param name="element">XmlNode welcher nach einem Text-Node durchsucht wird.</param>
    /// <returns><code>True</code> wenn das Element mindestens einen Text-Node besitzt.</returns>
    private bool HasTextNode(XmlNode element)
    {
      foreach(XmlNode n in element.ChildNodes)
      {
        if(n.HasChildNodes)
        {
          return HasTextNode(n);
        } else if(n.NodeType == XmlNodeType.Text)
        {
          return true;
        }
      }
      return false;
    }
  }
}
