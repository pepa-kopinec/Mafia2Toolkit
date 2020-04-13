﻿namespace Forms.Docking
{
    partial class DockPropertyGrid
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.QuickEditTab = new System.Windows.Forms.TabPage();
            this.ScaleZNumeric = new System.Windows.Forms.NumericUpDown();
            this.ScaleYNumeric = new System.Windows.Forms.NumericUpDown();
            this.ScaleXNumeric = new System.Windows.Forms.NumericUpDown();
            this.ScaleZLabel = new System.Windows.Forms.Label();
            this.ScaleYLabel = new System.Windows.Forms.Label();
            this.ScaleXLabel = new System.Windows.Forms.Label();
            this.RotationZNumeric = new System.Windows.Forms.NumericUpDown();
            this.RotationYNumeric = new System.Windows.Forms.NumericUpDown();
            this.RotationXNumeric = new System.Windows.Forms.NumericUpDown();
            this.PositionZNumeric = new System.Windows.Forms.NumericUpDown();
            this.PositionYNumeric = new System.Windows.Forms.NumericUpDown();
            this.PositionXNumeric = new System.Windows.Forms.NumericUpDown();
            this.CurrentEntry = new System.Windows.Forms.Label();
            this.RotationZLabel = new System.Windows.Forms.Label();
            this.RotationYLabel = new System.Windows.Forms.Label();
            this.RotationXLabel = new System.Windows.Forms.Label();
            this.PositionZLabel = new System.Windows.Forms.Label();
            this.PositionYLabel = new System.Windows.Forms.Label();
            this.PositionXLabel = new System.Windows.Forms.Label();
            this.PropertyTab = new System.Windows.Forms.TabPage();
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.MaterialPage = new System.Windows.Forms.TabPage();
            this.MatViewPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.LODComboBox = new System.Windows.Forms.ComboBox();
            this.QuickEditTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationXNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionZNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionYNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionXNumeric)).BeginInit();
            this.PropertyTab.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.MaterialPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // QuickEditTab
            // 
            this.QuickEditTab.Controls.Add(this.ScaleZNumeric);
            this.QuickEditTab.Controls.Add(this.ScaleYNumeric);
            this.QuickEditTab.Controls.Add(this.ScaleXNumeric);
            this.QuickEditTab.Controls.Add(this.ScaleZLabel);
            this.QuickEditTab.Controls.Add(this.ScaleYLabel);
            this.QuickEditTab.Controls.Add(this.ScaleXLabel);
            this.QuickEditTab.Controls.Add(this.RotationZNumeric);
            this.QuickEditTab.Controls.Add(this.RotationYNumeric);
            this.QuickEditTab.Controls.Add(this.RotationXNumeric);
            this.QuickEditTab.Controls.Add(this.PositionZNumeric);
            this.QuickEditTab.Controls.Add(this.PositionYNumeric);
            this.QuickEditTab.Controls.Add(this.PositionXNumeric);
            this.QuickEditTab.Controls.Add(this.CurrentEntry);
            this.QuickEditTab.Controls.Add(this.RotationZLabel);
            this.QuickEditTab.Controls.Add(this.RotationYLabel);
            this.QuickEditTab.Controls.Add(this.RotationXLabel);
            this.QuickEditTab.Controls.Add(this.PositionZLabel);
            this.QuickEditTab.Controls.Add(this.PositionYLabel);
            this.QuickEditTab.Controls.Add(this.PositionXLabel);
            this.QuickEditTab.Location = new System.Drawing.Point(4, 25);
            this.QuickEditTab.Margin = new System.Windows.Forms.Padding(4);
            this.QuickEditTab.Name = "QuickEditTab";
            this.QuickEditTab.Padding = new System.Windows.Forms.Padding(4);
            this.QuickEditTab.Size = new System.Drawing.Size(351, 538);
            this.QuickEditTab.TabIndex = 1;
            this.QuickEditTab.Text = "Edit Transform";
            this.QuickEditTab.UseVisualStyleBackColor = true;
            // 
            // ScaleZNumeric
            // 
            this.ScaleZNumeric.DecimalPlaces = 5;
            this.ScaleZNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleZNumeric.Location = new System.Drawing.Point(89, 295);
            this.ScaleZNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.ScaleZNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ScaleZNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.ScaleZNumeric.Name = "ScaleZNumeric";
            this.ScaleZNumeric.Size = new System.Drawing.Size(247, 22);
            this.ScaleZNumeric.TabIndex = 29;
            this.ScaleZNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.ScaleZNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.ScaleZNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // ScaleYNumeric
            // 
            this.ScaleYNumeric.DecimalPlaces = 5;
            this.ScaleYNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleYNumeric.Location = new System.Drawing.Point(89, 263);
            this.ScaleYNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.ScaleYNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ScaleYNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.ScaleYNumeric.Name = "ScaleYNumeric";
            this.ScaleYNumeric.Size = new System.Drawing.Size(247, 22);
            this.ScaleYNumeric.TabIndex = 28;
            this.ScaleYNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.ScaleYNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.ScaleYNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // ScaleXNumeric
            // 
            this.ScaleXNumeric.DecimalPlaces = 5;
            this.ScaleXNumeric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ScaleXNumeric.Location = new System.Drawing.Point(89, 231);
            this.ScaleXNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.ScaleXNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ScaleXNumeric.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.ScaleXNumeric.Name = "ScaleXNumeric";
            this.ScaleXNumeric.Size = new System.Drawing.Size(247, 22);
            this.ScaleXNumeric.TabIndex = 27;
            this.ScaleXNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.ScaleXNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.ScaleXNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // ScaleZLabel
            // 
            this.ScaleZLabel.AutoSize = true;
            this.ScaleZLabel.Location = new System.Drawing.Point(13, 298);
            this.ScaleZLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ScaleZLabel.Name = "ScaleZLabel";
            this.ScaleZLabel.Size = new System.Drawing.Size(56, 17);
            this.ScaleZLabel.TabIndex = 26;
            this.ScaleZLabel.Text = "Scale Z";
            // 
            // ScaleYLabel
            // 
            this.ScaleYLabel.AutoSize = true;
            this.ScaleYLabel.Location = new System.Drawing.Point(13, 266);
            this.ScaleYLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ScaleYLabel.Name = "ScaleYLabel";
            this.ScaleYLabel.Size = new System.Drawing.Size(56, 17);
            this.ScaleYLabel.TabIndex = 25;
            this.ScaleYLabel.Text = "Scale Y";
            // 
            // ScaleXLabel
            // 
            this.ScaleXLabel.AutoSize = true;
            this.ScaleXLabel.Location = new System.Drawing.Point(13, 234);
            this.ScaleXLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ScaleXLabel.Name = "ScaleXLabel";
            this.ScaleXLabel.Size = new System.Drawing.Size(56, 17);
            this.ScaleXLabel.TabIndex = 24;
            this.ScaleXLabel.Text = "Scale X";
            // 
            // RotationZNumeric
            // 
            this.RotationZNumeric.DecimalPlaces = 5;
            this.RotationZNumeric.Location = new System.Drawing.Point(89, 199);
            this.RotationZNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.RotationZNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationZNumeric.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationZNumeric.Name = "RotationZNumeric";
            this.RotationZNumeric.Size = new System.Drawing.Size(247, 22);
            this.RotationZNumeric.TabIndex = 23;
            this.RotationZNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.RotationZNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.RotationZNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // RotationYNumeric
            // 
            this.RotationYNumeric.DecimalPlaces = 5;
            this.RotationYNumeric.Location = new System.Drawing.Point(89, 167);
            this.RotationYNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.RotationYNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationYNumeric.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationYNumeric.Name = "RotationYNumeric";
            this.RotationYNumeric.Size = new System.Drawing.Size(247, 22);
            this.RotationYNumeric.TabIndex = 22;
            this.RotationYNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.RotationYNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.RotationYNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // RotationXNumeric
            // 
            this.RotationXNumeric.DecimalPlaces = 5;
            this.RotationXNumeric.Location = new System.Drawing.Point(89, 135);
            this.RotationXNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.RotationXNumeric.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.RotationXNumeric.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.RotationXNumeric.Name = "RotationXNumeric";
            this.RotationXNumeric.Size = new System.Drawing.Size(247, 22);
            this.RotationXNumeric.TabIndex = 21;
            this.RotationXNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.RotationXNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.RotationXNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // PositionZNumeric
            // 
            this.PositionZNumeric.DecimalPlaces = 5;
            this.PositionZNumeric.Location = new System.Drawing.Point(89, 103);
            this.PositionZNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.PositionZNumeric.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.PositionZNumeric.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.PositionZNumeric.Name = "PositionZNumeric";
            this.PositionZNumeric.Size = new System.Drawing.Size(247, 22);
            this.PositionZNumeric.TabIndex = 20;
            this.PositionZNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.PositionZNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.PositionZNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // PositionYNumeric
            // 
            this.PositionYNumeric.DecimalPlaces = 5;
            this.PositionYNumeric.Location = new System.Drawing.Point(89, 71);
            this.PositionYNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.PositionYNumeric.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.PositionYNumeric.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.PositionYNumeric.Name = "PositionYNumeric";
            this.PositionYNumeric.Size = new System.Drawing.Size(247, 22);
            this.PositionYNumeric.TabIndex = 19;
            this.PositionYNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.PositionYNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.PositionYNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // PositionXNumeric
            // 
            this.PositionXNumeric.DecimalPlaces = 5;
            this.PositionXNumeric.Location = new System.Drawing.Point(89, 39);
            this.PositionXNumeric.Margin = new System.Windows.Forms.Padding(4);
            this.PositionXNumeric.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.PositionXNumeric.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.PositionXNumeric.Name = "PositionXNumeric";
            this.PositionXNumeric.Size = new System.Drawing.Size(247, 22);
            this.PositionXNumeric.TabIndex = 18;
            this.PositionXNumeric.DoubleClick += new System.EventHandler(this.Numeric_DoubleClick);
            this.PositionXNumeric.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.PositionXNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // CurrentEntry
            // 
            this.CurrentEntry.AutoSize = true;
            this.CurrentEntry.Location = new System.Drawing.Point(13, 9);
            this.CurrentEntry.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CurrentEntry.Name = "CurrentEntry";
            this.CurrentEntry.Size = new System.Drawing.Size(0, 17);
            this.CurrentEntry.TabIndex = 12;
            // 
            // RotationZLabel
            // 
            this.RotationZLabel.AutoSize = true;
            this.RotationZLabel.Location = new System.Drawing.Point(9, 202);
            this.RotationZLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.RotationZLabel.Name = "RotationZLabel";
            this.RotationZLabel.Size = new System.Drawing.Size(74, 17);
            this.RotationZLabel.TabIndex = 10;
            this.RotationZLabel.Text = "Rotation Z";
            // 
            // RotationYLabel
            // 
            this.RotationYLabel.AutoSize = true;
            this.RotationYLabel.Location = new System.Drawing.Point(9, 170);
            this.RotationYLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.RotationYLabel.Name = "RotationYLabel";
            this.RotationYLabel.Size = new System.Drawing.Size(74, 17);
            this.RotationYLabel.TabIndex = 8;
            this.RotationYLabel.Text = "Rotation Y";
            // 
            // RotationXLabel
            // 
            this.RotationXLabel.AutoSize = true;
            this.RotationXLabel.Location = new System.Drawing.Point(9, 138);
            this.RotationXLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.RotationXLabel.Name = "RotationXLabel";
            this.RotationXLabel.Size = new System.Drawing.Size(74, 17);
            this.RotationXLabel.TabIndex = 6;
            this.RotationXLabel.Text = "Rotation X";
            // 
            // PositionZLabel
            // 
            this.PositionZLabel.AutoSize = true;
            this.PositionZLabel.Location = new System.Drawing.Point(9, 106);
            this.PositionZLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PositionZLabel.Name = "PositionZLabel";
            this.PositionZLabel.Size = new System.Drawing.Size(71, 17);
            this.PositionZLabel.TabIndex = 4;
            this.PositionZLabel.Text = "Position Z";
            // 
            // PositionYLabel
            // 
            this.PositionYLabel.AutoSize = true;
            this.PositionYLabel.Location = new System.Drawing.Point(9, 74);
            this.PositionYLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PositionYLabel.Name = "PositionYLabel";
            this.PositionYLabel.Size = new System.Drawing.Size(71, 17);
            this.PositionYLabel.TabIndex = 2;
            this.PositionYLabel.Text = "Position Y";
            // 
            // PositionXLabel
            // 
            this.PositionXLabel.AutoSize = true;
            this.PositionXLabel.Location = new System.Drawing.Point(8, 42);
            this.PositionXLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PositionXLabel.Name = "PositionXLabel";
            this.PositionXLabel.Size = new System.Drawing.Size(71, 17);
            this.PositionXLabel.TabIndex = 0;
            this.PositionXLabel.Text = "Position X";
            // 
            // PropertyTab
            // 
            this.PropertyTab.Controls.Add(this.PropertyGrid);
            this.PropertyTab.Location = new System.Drawing.Point(4, 25);
            this.PropertyTab.Margin = new System.Windows.Forms.Padding(4);
            this.PropertyTab.Name = "PropertyTab";
            this.PropertyTab.Padding = new System.Windows.Forms.Padding(4);
            this.PropertyTab.Size = new System.Drawing.Size(351, 538);
            this.PropertyTab.TabIndex = 0;
            this.PropertyTab.Text = "Property Grid";
            this.PropertyTab.UseVisualStyleBackColor = true;
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.PropertyGrid.Location = new System.Drawing.Point(4, 4);
            this.PropertyGrid.Margin = new System.Windows.Forms.Padding(4);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(343, 530);
            this.PropertyGrid.TabIndex = 2;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.PropertyTab);
            this.MainTabControl.Controls.Add(this.QuickEditTab);
            this.MainTabControl.Controls.Add(this.MaterialPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(359, 567);
            this.MainTabControl.TabIndex = 7;
            this.MainTabControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.MainTabControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            // 
            // MaterialPage
            // 
            this.MaterialPage.Controls.Add(this.MatViewPanel);
            this.MaterialPage.Controls.Add(this.label1);
            this.MaterialPage.Controls.Add(this.LODComboBox);
            this.MaterialPage.Location = new System.Drawing.Point(4, 25);
            this.MaterialPage.Margin = new System.Windows.Forms.Padding(4);
            this.MaterialPage.Name = "MaterialPage";
            this.MaterialPage.Padding = new System.Windows.Forms.Padding(4);
            this.MaterialPage.Size = new System.Drawing.Size(351, 538);
            this.MaterialPage.TabIndex = 2;
            this.MaterialPage.Text = "Model Materials";
            this.MaterialPage.UseVisualStyleBackColor = true;
            // 
            // MatViewPanel
            // 
            this.MatViewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MatViewPanel.AutoScroll = true;
            this.MatViewPanel.Location = new System.Drawing.Point(16, 63);
            this.MatViewPanel.Margin = new System.Windows.Forms.Padding(4);
            this.MatViewPanel.Name = "MatViewPanel";
            this.MatViewPanel.Size = new System.Drawing.Size(319, 463);
            this.MatViewPanel.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select LOD";
            // 
            // LODComboBox
            // 
            this.LODComboBox.FormattingEnabled = true;
            this.LODComboBox.Location = new System.Drawing.Point(11, 30);
            this.LODComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.LODComboBox.Name = "LODComboBox";
            this.LODComboBox.Size = new System.Drawing.Size(323, 24);
            this.LODComboBox.TabIndex = 0;
            this.LODComboBox.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // DockPropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(359, 567);
            this.Controls.Add(this.MainTabControl);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DockPropertyGrid";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.TabText = "Frame Properties";
            this.Text = "PropertyGrid";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransformEditKeyUp);
            this.QuickEditTab.ResumeLayout(false);
            this.QuickEditTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RotationXNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionZNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionYNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionXNumeric)).EndInit();
            this.PropertyTab.ResumeLayout(false);
            this.MainTabControl.ResumeLayout(false);
            this.MaterialPage.ResumeLayout(false);
            this.MaterialPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage QuickEditTab;
        public System.Windows.Forms.NumericUpDown RotationZNumeric;
        public System.Windows.Forms.NumericUpDown RotationYNumeric;
        public System.Windows.Forms.NumericUpDown RotationXNumeric;
        public System.Windows.Forms.NumericUpDown PositionZNumeric;
        public System.Windows.Forms.NumericUpDown PositionYNumeric;
        public System.Windows.Forms.NumericUpDown PositionXNumeric;
        private System.Windows.Forms.Label CurrentEntry;
        private System.Windows.Forms.Label RotationZLabel;
        private System.Windows.Forms.Label RotationYLabel;
        private System.Windows.Forms.Label RotationXLabel;
        private System.Windows.Forms.Label PositionZLabel;
        private System.Windows.Forms.Label PositionYLabel;
        private System.Windows.Forms.Label PositionXLabel;
        private System.Windows.Forms.TabPage PropertyTab;
        public System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.TabControl MainTabControl;
        public System.Windows.Forms.NumericUpDown ScaleZNumeric;
        public System.Windows.Forms.NumericUpDown ScaleYNumeric;
        public System.Windows.Forms.NumericUpDown ScaleXNumeric;
        private System.Windows.Forms.Label ScaleZLabel;
        private System.Windows.Forms.Label ScaleYLabel;
        private System.Windows.Forms.Label ScaleXLabel;
        private System.Windows.Forms.TabPage MaterialPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox LODComboBox;
        private System.Windows.Forms.FlowLayoutPanel MatViewPanel;
    }
}