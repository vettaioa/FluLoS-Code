/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls
 * File:      ExternalCallMethodInfo.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls {

  /// <summary>Die <c>ExternalCallMethodInfo</c> Klasse ist ein Kontäner, welcher die benötigten Information
  /// einer "external call" Methode enthält.</summary>
  internal class ExternalCallMethodInfo {
    private Assembly dll;
    private string methodCallName;
    private string methodName;
    private string className;
    private string namespaceName;
    
    /// <summary>Erstellt ein <c>ExternalCallMethodInfo</c> Objekt.</summary>
    /// 
    /// <param name="dll">DLL welche die Methode enthält.</param>
    /// <param name="methodCallName">Der Name, welcher für den Aufruf verwendet wird.</param>
    /// <param name="methodName">Der effektive Methodenname.</param>
    /// <param name="className">Der Klassenname, in welchem die Methode definiert ist.</param>
    /// <param name="namespaceName">Der Namespacename, in welchem die Klasse enthalten ist.</param>
    public ExternalCallMethodInfo(Assembly dll, string methodCallName, string methodName, string className, string namespaceName) {
      this.dll = dll;
      this.methodCallName = methodCallName;
      this.methodName = methodName;
      this.className = className;
      this.namespaceName = namespaceName;
    }

    /// <summary>Die DLL welche die Methode enthält.</summary>
    public Assembly Dll {
      get { return dll; }
    }
    
    /// <summary>Der Name, welcher für den Aufruf verwendet wird.</summary>
    public string MethodCallName {
      get { return methodCallName; }
    }
    
    /// <summary>Der effektive Methodenname.</summary>
    public string MethodName {
      get { return methodName; }
    }

    /// <summary>Der Klassenname, in welcher die Methode definiert ist.</summary>
    public string ClassName {
      get { return className; }
    }

    /// <summary>Der Namespacename, in welchem die Klasse enthalten ist.</summary>
    public string NamespaceName {
      get { return namespaceName; }
    }

    /// <summary>Der volle Klassenname. Namespacename plus Klassenname.</summary>
    public string FullClassName {
      get { return namespaceName + "." + className; }
    }
  }
}
