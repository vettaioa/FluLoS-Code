/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls
 * File:      InvokeMethodWrapper.cs
 * Version:   1.0
 * Date:      18.01.2008
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007-2008 ZHAW-InIT. All rights reserved.
 */
 
using System;
using System.Collections.Generic;
using System.Text;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls {
  
  /// <summary>Die <c>InvokeMethodWrapper</c> Klasse kümmert sich um den Aufruf der "external call" Methoden
  /// mittels Reflection. Dieser Wrapper ist notwendig um eine Delegation der per Reflection aufzurufenden 
  /// Methode zu erstellen.</summary>
  internal class InvokeMethodWrapper {
    private Object classObj;
    private string methodName;

    /// <summary>Erstellt ein <c>InvokeMethodWrapper</c> Objekt.</summary>
    /// 
    /// <param name="classObj">Instanz der Klasse, welche die aufzurufende Methode enthält.</param>
    /// <param name="methodName">Der Name der aufzurufenden Methode.</param>
    public InvokeMethodWrapper(Object classObj, string methodName) {
      this.classObj = classObj;
      this.methodName = methodName;
    }

    /// <summary>Führt die "external call" Methode aus.</summary>
    /// 
    /// <param name="param">Der Stringparameter, welcher der "external call" Methode übergeben wird</param>
    /// <returns>Das Resultat der "external call" Methode. <c>True</c> oder <c>False</c>.</returns>
    public object invoke(string param) {
      return classObj.GetType().GetMethod(methodName).Invoke(classObj, new Object[]{param});
    }
  }
}
