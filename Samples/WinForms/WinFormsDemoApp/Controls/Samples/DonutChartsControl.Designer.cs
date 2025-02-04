﻿/* Copyright 1998-2023 by Northwoods Software Corporation. */

namespace Demo.Samples.DonutCharts {
  partial class DonutChartsControl {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.diagramControl1 = new Northwoods.Go.WinForms.DiagramControl();
            this.btnChangeSelectedValue = new System.Windows.Forms.Button();
            this.goWebBrowser1 = new WinFormsDemoApp.GoWebBrowser();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.goWebBrowser1)).BeginInit();
            this.SuspendLayout();
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.diagramControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnChangeSelectedValue, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.goWebBrowser1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1000, 659);
            this.tableLayoutPanel1.TabIndex = 0;
            //
            // diagramControl1
            //
            this.diagramControl1.AllowDrop = true;
            this.diagramControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.diagramControl1.Location = new System.Drawing.Point(3, 3);
            this.diagramControl1.Name = "diagramControl1";
            this.diagramControl1.Size = new System.Drawing.Size(994, 444);
            this.diagramControl1.TabIndex = 0;
            this.diagramControl1.Text = "diagramControl1";
            //
            // btnChangeSelectedValue
            //
            this.btnChangeSelectedValue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnChangeSelectedValue.Location = new System.Drawing.Point(3, 453);
            this.btnChangeSelectedValue.Name = "btnChangeSelectedValue";
            this.btnChangeSelectedValue.Size = new System.Drawing.Size(200, 38);
            this.btnChangeSelectedValue.TabIndex = 1;
            this.btnChangeSelectedValue.Text = "Change Selected Value";
            this.btnChangeSelectedValue.UseVisualStyleBackColor = true;
            //
            // goWebBrowser1
            //
            this.goWebBrowser1.CreationProperties = null;
            this.goWebBrowser1.DefaultBackgroundColor = System.Drawing.Color.White;
            this.goWebBrowser1.Dock = System.Windows.Forms.DockStyle.Top;
            this.goWebBrowser1.Location = new System.Drawing.Point(3, 497);
            this.goWebBrowser1.Name = "goWebBrowser1";
            this.goWebBrowser1.Size = new System.Drawing.Size(994, 131);
            this.goWebBrowser1.TabIndex = 2;
            this.goWebBrowser1.ZoomFactor = 1D;
            //
            // DonutChartsControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DonutChartsControl";
            this.Size = new System.Drawing.Size(1000, 659);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.goWebBrowser1)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Northwoods.Go.WinForms.DiagramControl diagramControl1;
    private System.Windows.Forms.Button btnChangeSelectedValue;
    private WinFormsDemoApp.GoWebBrowser goWebBrowser1;
  }
}
