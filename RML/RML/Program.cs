/*
 * Namespace: Iib.RegexMarkupLanguage
 * File:      Program.cs
 * Version:   1.0
 * Date:      12.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHAW-InIT. All rights reserved.
 */
 
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Iib.RegexMarkupLanguage.Gui;

namespace Iib.RegexMarkupLanguage {

  static class Program {
    
    /// <summary></summary>
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Test());
    }
  }
}