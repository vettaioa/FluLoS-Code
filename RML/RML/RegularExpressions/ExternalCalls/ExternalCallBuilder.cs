/*
 * Namespace: Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls
 * File:      ExternalCallBuilder.cs
 * Version:   1.0
 * Date:      15.01.2007
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2007 ZHAW-InIT. All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using log4net;
using Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls.Utils;
using System.Configuration;
using System.Collections.Specialized;

namespace Iib.RegexMarkupLanguage.RegularExpressions.ExternalCalls {

  ///<summary>Die <c>ExternalCallBuilder</c> Klasse ist für das laden der "external call" Klassen aus den im angegebenen
  /// Verzeichnis enthaltenden Dll's zuständig. Sie stellt die benötigten Delegates zur Verfügung um die "external call"
  /// Methoden aus der Regexmaschinerie ausführen zu können.</summary>
  /// <remarks>Die DLL's werden beim Initialisieren des Programmes auf "external call" Methoden abgesucht und registriert. 
  /// Später ist es möglich die benötigten Methoden mittels der loadMethods() zu laden. Es werden nur die benötigten Methoden
  /// geladen, alle weiteren Methoden sind nicht ausführbar.</remarks>
  internal class ExternalCallBuilder {
    private static readonly ILog log = LogManager.GetLogger(typeof(ExternalCallBuilder));
    private static ExternalCallBuilder instance = null;
    public delegate object ExtCall(string toCheck);
    private IDictionary<String, ExternalCallMethodInfo> externalCallMethods;
    private IDictionary<String, Object> externalCallClasses;
    private IDictionary<String, ExtCall> externalCallDelegates;
    private IDictionary<String, String> externalCallMapping;
    private string dllDir;

    /// <summary>Erstellt ein <c>ExternalCallBuilder</c> Objekt.</summary>
    /// <param name="dllDir">Der Pfad zum Verzeichnis, welches die DLL's enthält.</param>
    private ExternalCallBuilder(string dllDir) {
      this.dllDir = dllDir;
      externalCallMethods = new Dictionary<String, ExternalCallMethodInfo>();
      externalCallClasses = new Dictionary<String, Object>();
      externalCallDelegates = new Dictionary<String, ExtCall>();
      analyseDll();
    }

    /// <summary>Initialisiert den <c>ExternalCallBuilder</c>. Diese Methode muss vor dem ersten Aufruf von <c>getInstance()</c>
    /// ausgeführt werden.</summary>
    /// <returns>Die Instanz des <c>ExternalCallBuilder</c>.</returns>
    public static ExternalCallBuilder createInstance() {
      if(instance == null) {
        string dllDir = ConfigurationManager.AppSettings["dllDir"];
        if(dllDir == null) {
          log.Fatal("Can't read application setting argument 'dllDir'...");
          throw new Exception("Can't read application setting argument 'dllDir'...");
        }
        instance = new ExternalCallBuilder(ConfigurationManager.AppSettings["dllDir"]);
      }
      return instance;
    }

    /// <summary>Gibt die Instanz des <c>ExternalCallBuilder</c> zurück. Vor dem ersten Aufruf dieser Methode muss zuerst
    /// die Methode <c>createInstance()</c> aufgerufen werden. Sonst gibt <c>getInstance()</c> null zurück.</summary>
    /// <returns>Die Instanz des <c>ExternalCallBuilder</c>.</returns>
    public static ExternalCallBuilder getInstance() {
      return instance;
    }

    /// <summary>Gibt das Delegate der "external call" Methoden mit dem übergebenen Namen zurück.</summary>
    /// <param name="name">Der name der gewünschten Methode. ([Klasse].[Methodenname]).</param>
    /// <returns>Das Delegate der gewünschten Methode.</returns>
    public ExtCall this[string name] {
      get { return externalCallDelegates[name]; }
    }
    
    /// <summary>Lädt alle vorhandenen "external call" Methoden.</summary>
    public void loadMethods() {
      loadMethods(null);
    }
    
    /// <summary>Lädt die übergebenen Methoden, welche dann ausführbar sind. Alle anderen Methoden sind zu diesem Zeitpunkt
    /// nicht ausführbar. Wird diese Methode mehrmals ausgeführt, werden jeweils die alten Methoden entladen und die neuen geladen.</summary>
    /// <param name="methods">Eine liste mit den zu ladenden "external call" Methoden. ([Klasse].[Methodenname])</param>
    public void loadMethods(IDictionary<string, string> methods) {
      // anderpas & schuejen
      externalCallMapping = methods;
      externalCallClasses.Clear();
      externalCallDelegates.Clear();
      if(methods != null) {
        foreach(string m in methods.Values) {
          ExternalCallMethodInfo ecmi = externalCallMethods[m];
          if(ecmi != null) {
            generateDelegate(ecmi);   
          }
        }
      } else {
        foreach(ExternalCallMethodInfo ecmi in externalCallMethods.Values) {
          generateDelegate(ecmi);
        }
      }
    }

    /// <summary>Instanziert die benötigte Klasse und erstellt das Delegate der ensprechenden Methode.</summary>
    /// <param name="ecmi">Die Daten der zu ladenden Methode.</param>
    private void generateDelegate(ExternalCallMethodInfo ecmi) {
      Object callClass;
      if(externalCallClasses.ContainsKey(ecmi.FullClassName)) {
        callClass = externalCallClasses[ecmi.FullClassName];
      } else {
        callClass = ecmi.Dll.CreateInstance(ecmi.FullClassName);
        externalCallClasses.Add(ecmi.FullClassName, callClass);
      }
      InvokeMethodWrapper imw = new InvokeMethodWrapper(callClass, ecmi.MethodName);
      externalCallDelegates.Add(ecmi.ClassName + "." + ecmi.MethodCallName, new ExtCall(imw.invoke));
    }

    /// <summary>Durchsucht mittels Reflection die vorhandenen DLL's nach "external call" Methoden und registriert diese,
    /// mit den benötigten Daten.</summary>
    private void analyseDll() {
      DirectoryInfo dir = new DirectoryInfo(dllDir);
      if (!dir.Exists){
          log.Error("The 'dll' directory doesn't exits...");
          throw new Exception("The 'dll' directory doesn't exits...");
      }
      foreach(FileInfo file in dir.GetFiles()) {
        if(file.Extension.ToLower().Equals(".dll")) {
          Assembly dll;
          try {
            dll = Assembly.LoadFile(file.FullName);
          } catch(Exception e) {
            log.Warn("DLL '" + file.FullName + "' could not be loaded...", e);
            continue;
          }
               
          foreach(Type type in dll.GetTypes()) {
            if(type.IsClass) {
              foreach(MethodInfo method in type.GetMethods()) {
                object[] attributes = method.GetCustomAttributes(false);
                foreach(Object attr in attributes) {
                  if(attr.GetType().Name.Equals("ExternalCallMethod")) {
                    ExternalCallMethod ecm = (ExternalCallMethod)attr;
                    // anderpas & schuejen
                    if(method.ReturnType.Name.Equals("Boolean") || method.ReturnType.Name.Equals("bool") || 
                      method.ReturnType.Name.Equals("string") || method.ReturnType.Name.Equals("String"))
                                        {
                      if(ecm.name == null) {
                        ecm.name = method.Name;
                      }
                      ExternalCallMethodInfo ecmi = new ExternalCallMethodInfo(dll, ecm.name, method.Name, type.Name, type.Namespace);
                      externalCallMethods.Add(ecmi.ClassName + "." + ecmi.MethodCallName, ecmi);
                    }
                  }
                }
              }
            }        
          }
        }
      }
    }

    // anderpas & schuejen
    public string adaptValue(string parentName, string value)
    {
      if(!externalCallMapping.ContainsKey(parentName))
      {
        return value;
      }

      var methodName = externalCallMapping[parentName];
      object newValue = externalCallDelegates[methodName](value);

      if(newValue.GetType() == typeof(string) || newValue.GetType() == typeof(String))
      {
        return (string)newValue;
      }
      return value;
    }
  }
}