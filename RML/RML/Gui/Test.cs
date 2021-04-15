/*
 * Namespace: Iib.RegexMarkupLanguage.Gui
 * File:      Test.cs
 * Version:   1.0
 * Date:      14.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006 ZHAW-InIT. All rights reserved.
 */
 
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using Iib.RegexMarkupLanguage;
using Iib.RegexMarkupLanguage.Collections;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Iib.RegexMarkupLanguage.Gui {

  public partial class Test : Form {
      
    public Test() {
      InitializeComponent();
    }

    private void bBuild_Click(object sender, System.EventArgs e) {
      lvMessages.Items.Clear();
 
      Rml test = null;
      IEnumerable<CompilerException> exceptions = null;
      try {
        test = new Rml(Path.GetFullPath(tbSourceFile.Text), out exceptions, 0);
      } catch(Exception ex) {
      }
      
      foreach(CompilerException cex in exceptions) {
      	if (showWarnings.Checked || cex.Type == CompilerExceptionType.Error) {
	        lvMessages.Items.Add(new ListViewItem( new String[] {Enum.GetName(cex.Type.GetType(), cex.Type).Substring(0, 1), 
	                                               cex.Message, cex.Token.Line.ToString(), cex.Token.Column.ToString(),
	                                               cex.Token.FileName}));
      	   }
	    }
    }
    
    private void bExecute_Click(object sender, EventArgs e) {
      tbOutputData.Clear();
      
      IEnumerable<CompilerException> exceptions = null;      
      Rml r2x = new Rml(Path.GetFullPath(tbSourceFile.Text), out exceptions, 0);
      StreamReader input = new StreamReader(Path.GetFullPath(tbInputFile.Text), Encoding.GetEncoding("iso-8859-1"));
      StringWriter sw = new StringWriter();
      XmlTextWriter writer = new XmlTextWriter(sw);
      writer.Formatting = Formatting.Indented;
      writer.Indentation = 2;
      XmlDocument output;
      
      int matched = 0;
      int total = 0;
      pbStatus.Maximum = (int)(new FileInfo(Path.GetFullPath(tbInputFile.Text)).Length / 80)+10;
      pbStatus.Value = 0;
      lTime.Text = "";
      Stopwatch watch = Stopwatch.StartNew();
      if(cbMode.Checked) {
        string line;
        while((line = input.ReadLine()) != null) {
          try {
            output = r2x.Execute(line);    
            output.WriteTo(writer);
            writer.Flush();
            matched++;
          } catch(RmlException) {
          } finally {
            total++;
            if (pbStatus.Value < pbStatus.Maximum) {
              pbStatus.Value++;
            }
            lTotal.Text = total.ToString();
            lMatched.Text = matched.ToString();
            lPercent.Text = (matched * 100.00 / total).ToString() + " %";
            Application.DoEvents();
          }
        }
      } else {
        try {
          output = r2x.Execute(input.ReadToEnd());
          output.WriteTo(writer);
          writer.Flush();
        } catch(RmlException) {}
      }      
      watch.Stop();
      tbOutputData.AppendText(sw.ToString());
      tbOutputData.SelectionStart = 0;
      tbOutputData.ScrollToCaret();
      input.Close();
      writer.Close();
      
      pbStatus.Value = pbStatus.Maximum;
      lTime.Text = watch.Elapsed.TotalMinutes.ToString() + " min";     
      if(cbMode.Checked) {
        lAverage.Text = (watch.Elapsed.TotalSeconds / total).ToString() + " sec";
      } else {
        lAverage.Text = "0";
        lMatched.Text = "0";
        lPercent.Text = "0";
        lTotal.Text = "0";
      }
    }    
  
    private void bOpen_Click(object sender, EventArgs e) {
      StreamReader script = new StreamReader(Path.GetFullPath(tbSourceFile.Text));
      try {
        tbSource.Clear();
        tbSource.Text = script.ReadToEnd();
      } finally {
        script.Close();
      }
    }

    private void bSave_Click(object sender, EventArgs e) {
      StreamWriter script = new StreamWriter(Path.GetFullPath(tbSourceFile.Text));
      try {
        script.Write(tbSource.Text);
      } finally {
        script.Close();
      }
    }

    private void bInputOpen_Click(object sender, EventArgs e) {
      StreamReader input = new StreamReader(Path.GetFullPath(tbInputFile.Text));
      try {
        tbInput.Clear();
        tbInput.Text = input.ReadToEnd();
      } finally {
        input.Close();
      }
    }

    private void lvMessages_ItemActivate(object sender, EventArgs e) {
      int l = Int32.Parse(lvMessages.SelectedItems[0].SubItems[2].Text);
      int c = Int32.Parse(lvMessages.SelectedItems[0].SubItems[3].Text);
      tbSource.Select(tbSource.Text.IndexOf(tbSource.Lines[l-1])+c-1, 1);
      tbSource.Focus();
    }
  }
}