/*
 * Namespace: Iib.RegexMarkupLanguage.Gui
 * File:      Test.Designer.cs
 * Version:   1.0
 * Date:      14.06.2006
 * Authors:   Marco Vergari (marco@vergari.ch)
 *
 * Copyright 2006-2007 ZHW-InIT. All rights reserved.
 */
 
namespace Iib.RegexMarkupLanguage.Gui {

  partial class Test {
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TextBox tbSource;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button bBuild;
    private System.Windows.Forms.TextBox tbSourceFile;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button bSourceOpen;
    private System.Windows.Forms.Button bSourceSave;
    private System.Windows.Forms.ListView lvMessages;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ColumnHeader chDescription;
    private System.Windows.Forms.ColumnHeader chLine;
    private System.Windows.Forms.ColumnHeader chColumn;
    private System.Windows.Forms.ColumnHeader chFile;
    private System.Windows.Forms.ColumnHeader chType;

    /// <summary>Clean up any resources being used.</summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code
    /// <summary>Required method for Designer support - do not modify
    /// the contents of this method with the code editor.</summary>
    private void InitializeComponent() {
            this.tbSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bBuild = new System.Windows.Forms.Button();
            this.tbSourceFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bSourceOpen = new System.Windows.Forms.Button();
            this.bSourceSave = new System.Windows.Forms.Button();
            this.lvMessages = new System.Windows.Forms.ListView();
            this.chType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbInputFile = new System.Windows.Forms.TextBox();
            this.bExecute = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.tbOutputData = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lMatched = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lTotal = new System.Windows.Forms.Label();
            this.lTime = new System.Windows.Forms.Label();
            this.pbStatus = new System.Windows.Forms.ProgressBar();
            this.lPercent = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lAverage = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.bInputOpen = new System.Windows.Forms.Button();
            this.cbMode = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.showWarnings = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tbSource
            // 
            this.tbSource.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSource.Location = new System.Drawing.Point(18, 117);
            this.tbSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSource.Multiline = true;
            this.tbSource.Name = "tbSource";
            this.tbSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbSource.Size = new System.Drawing.Size(954, 1093);
            this.tbSource.TabIndex = 8;
            this.tbSource.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 92);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sourcecode:";
            // 
            // bBuild
            // 
            this.bBuild.Location = new System.Drawing.Point(852, 28);
            this.bBuild.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bBuild.Name = "bBuild";
            this.bBuild.Size = new System.Drawing.Size(112, 35);
            this.bBuild.TabIndex = 4;
            this.bBuild.Text = "Build";
            this.bBuild.UseVisualStyleBackColor = true;
            this.bBuild.Click += new System.EventHandler(this.bBuild_Click);
            // 
            // tbSourceFile
            // 
            this.tbSourceFile.Location = new System.Drawing.Point(112, 32);
            this.tbSourceFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSourceFile.Name = "tbSourceFile";
            this.tbSourceFile.Size = new System.Drawing.Size(486, 26);
            this.tbSourceFile.TabIndex = 1;
            this.tbSourceFile.Text = "../../../atc.rml";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Source-File:";
            // 
            // bSourceOpen
            // 
            this.bSourceOpen.Location = new System.Drawing.Point(609, 29);
            this.bSourceOpen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bSourceOpen.Name = "bSourceOpen";
            this.bSourceOpen.Size = new System.Drawing.Size(112, 35);
            this.bSourceOpen.TabIndex = 2;
            this.bSourceOpen.Text = "Open";
            this.bSourceOpen.UseVisualStyleBackColor = true;
            this.bSourceOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // bSourceSave
            // 
            this.bSourceSave.Location = new System.Drawing.Point(730, 28);
            this.bSourceSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bSourceSave.Name = "bSourceSave";
            this.bSourceSave.Size = new System.Drawing.Size(112, 35);
            this.bSourceSave.TabIndex = 3;
            this.bSourceSave.Text = "Save";
            this.bSourceSave.UseVisualStyleBackColor = true;
            this.bSourceSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // lvMessages
            // 
            this.lvMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chType,
            this.chDescription,
            this.chLine,
            this.chColumn,
            this.chFile});
            this.lvMessages.FullRowSelect = true;
            this.lvMessages.GridLines = true;
            this.lvMessages.HideSelection = false;
            this.lvMessages.Location = new System.Drawing.Point(1004, 515);
            this.lvMessages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvMessages.MultiSelect = false;
            this.lvMessages.Name = "lvMessages";
            this.lvMessages.Size = new System.Drawing.Size(826, 218);
            this.lvMessages.TabIndex = 9;
            this.lvMessages.UseCompatibleStateImageBehavior = false;
            this.lvMessages.View = System.Windows.Forms.View.Details;
            this.lvMessages.ItemActivate += new System.EventHandler(this.lvMessages_ItemActivate);
            // 
            // chType
            // 
            this.chType.Text = "";
            this.chType.Width = 22;
            // 
            // chDescription
            // 
            this.chDescription.Text = "Description";
            this.chDescription.Width = 333;
            // 
            // chLine
            // 
            this.chLine.Text = "Line";
            this.chLine.Width = 48;
            // 
            // chColumn
            // 
            this.chColumn.Text = "Column";
            this.chColumn.Width = 48;
            // 
            // chFile
            // 
            this.chFile.Text = "File";
            this.chFile.Width = 121;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1004, 489);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Messages:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1002, 37);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 20);
            this.label8.TabIndex = 21;
            this.label8.Text = "Input-Data:";
            // 
            // tbInputFile
            // 
            this.tbInputFile.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.tbInputFile.Location = new System.Drawing.Point(1108, 32);
            this.tbInputFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbInputFile.Name = "tbInputFile";
            this.tbInputFile.Size = new System.Drawing.Size(474, 26);
            this.tbInputFile.TabIndex = 5;
            this.tbInputFile.Text = "../../../../data/rml/test_input.txt";
            // 
            // bExecute
            // 
            this.bExecute.Location = new System.Drawing.Point(1718, 28);
            this.bExecute.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bExecute.Name = "bExecute";
            this.bExecute.Size = new System.Drawing.Size(112, 35);
            this.bExecute.TabIndex = 24;
            this.bExecute.Text = "Regex2XML";
            this.bExecute.UseVisualStyleBackColor = true;
            this.bExecute.Click += new System.EventHandler(this.bExecute_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(999, 754);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 20);
            this.label9.TabIndex = 22;
            this.label9.Text = "Output:";
            // 
            // tbOutputData
            // 
            this.tbOutputData.Location = new System.Drawing.Point(1004, 778);
            this.tbOutputData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbOutputData.Multiline = true;
            this.tbOutputData.Name = "tbOutputData";
            this.tbOutputData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutputData.Size = new System.Drawing.Size(826, 324);
            this.tbOutputData.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(999, 1175);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 20);
            this.label4.TabIndex = 25;
            this.label4.Text = "Lines matched:";
            // 
            // lMatched
            // 
            this.lMatched.AutoSize = true;
            this.lMatched.Location = new System.Drawing.Point(1126, 1175);
            this.lMatched.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lMatched.Name = "lMatched";
            this.lMatched.Size = new System.Drawing.Size(18, 20);
            this.lMatched.TabIndex = 26;
            this.lMatched.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(999, 1155);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 20);
            this.label5.TabIndex = 27;
            this.label5.Text = "Lines total:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1438, 1155);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 20);
            this.label6.TabIndex = 28;
            this.label6.Text = "Time total:";
            // 
            // lTotal
            // 
            this.lTotal.AutoSize = true;
            this.lTotal.Location = new System.Drawing.Point(1126, 1155);
            this.lTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lTotal.Name = "lTotal";
            this.lTotal.Size = new System.Drawing.Size(18, 20);
            this.lTotal.TabIndex = 29;
            this.lTotal.Text = "0";
            // 
            // lTime
            // 
            this.lTime.AutoSize = true;
            this.lTime.Location = new System.Drawing.Point(1578, 1155);
            this.lTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(18, 20);
            this.lTime.TabIndex = 30;
            this.lTime.Text = "0";
            // 
            // pbStatus
            // 
            this.pbStatus.Location = new System.Drawing.Point(1004, 1114);
            this.pbStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(828, 23);
            this.pbStatus.Step = 1;
            this.pbStatus.TabIndex = 31;
            this.pbStatus.UseWaitCursor = true;
            // 
            // lPercent
            // 
            this.lPercent.AutoSize = true;
            this.lPercent.Location = new System.Drawing.Point(1227, 1166);
            this.lPercent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lPercent.Name = "lPercent";
            this.lPercent.Size = new System.Drawing.Size(18, 20);
            this.lPercent.TabIndex = 33;
            this.lPercent.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1188, 1165);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 20);
            this.label10.TabIndex = 32;
            this.label10.Text = "==>";
            // 
            // lAverage
            // 
            this.lAverage.AutoSize = true;
            this.lAverage.Location = new System.Drawing.Point(1578, 1175);
            this.lAverage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lAverage.Name = "lAverage";
            this.lAverage.Size = new System.Drawing.Size(18, 20);
            this.lAverage.TabIndex = 35;
            this.lAverage.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(1438, 1175);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(127, 20);
            this.label11.TabIndex = 34;
            this.label11.Text = "Average per line:";
            // 
            // tbInput
            // 
            this.tbInput.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInput.Location = new System.Drawing.Point(1004, 117);
            this.tbInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbInput.Multiline = true;
            this.tbInput.Name = "tbInput";
            this.tbInput.ReadOnly = true;
            this.tbInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbInput.Size = new System.Drawing.Size(826, 350);
            this.tbInput.TabIndex = 36;
            this.tbInput.WordWrap = false;
            // 
            // bInputOpen
            // 
            this.bInputOpen.Location = new System.Drawing.Point(1596, 28);
            this.bInputOpen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bInputOpen.Name = "bInputOpen";
            this.bInputOpen.Size = new System.Drawing.Size(112, 35);
            this.bInputOpen.TabIndex = 37;
            this.bInputOpen.Text = "Open";
            this.bInputOpen.UseVisualStyleBackColor = true;
            this.bInputOpen.Click += new System.EventHandler(this.bInputOpen_Click);
            // 
            // cbMode
            // 
            this.cbMode.AutoSize = true;
            this.cbMode.Location = new System.Drawing.Point(1108, 68);
            this.cbMode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(166, 24);
            this.cbMode.TabIndex = 38;
            this.cbMode.Text = "Jede Zeile als Input";
            this.cbMode.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1004, 92);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 20);
            this.label7.TabIndex = 39;
            this.label7.Text = "Input (read only):";
            // 
            // showWarnings
            // 
            this.showWarnings.Location = new System.Drawing.Point(1108, 480);
            this.showWarnings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.showWarnings.Name = "showWarnings";
            this.showWarnings.Size = new System.Drawing.Size(260, 29);
            this.showWarnings.TabIndex = 40;
            this.showWarnings.Text = "Warnungen anzeigen";
            this.showWarnings.UseVisualStyleBackColor = true;
            // 
            // Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1854, 1231);
            this.Controls.Add(this.showWarnings);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbMode);
            this.Controls.Add(this.bInputOpen);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.lAverage);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lPercent);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.pbStatus);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.lTotal);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lMatched);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bExecute);
            this.Controls.Add(this.tbOutputData);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tbInputFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lvMessages);
            this.Controls.Add(this.bSourceSave);
            this.Controls.Add(this.bSourceOpen);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbSourceFile);
            this.Controls.Add(this.bBuild);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Test";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test";
            this.Load += new System.EventHandler(this.TestLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    private System.Windows.Forms.CheckBox showWarnings;
    #endregion

    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox tbInputFile;
    private System.Windows.Forms.Button bExecute;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox tbOutputData;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lMatched;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label lTotal;
    private System.Windows.Forms.Label lTime;
    private System.Windows.Forms.ProgressBar pbStatus;
    private System.Windows.Forms.Label lPercent;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label lAverage;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.TextBox tbInput;
    private System.Windows.Forms.Button bInputOpen;
    private System.Windows.Forms.CheckBox cbMode;
    private System.Windows.Forms.Label label7;
    
    void TestLoad(object sender, System.EventArgs e)
    {
    	
    }
  }
}