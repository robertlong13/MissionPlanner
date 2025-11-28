namespace MissionPlanner.Controls
{
    partial class MissionStyleEditor
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
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.myButton2 = new MissionPlanner.Controls.MyButton();
            this.myButton3 = new MissionPlanner.Controls.MyButton();
            this.myButton4 = new MissionPlanner.Controls.MyButton();
            this.myButton5 = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.myButton6 = new MissionPlanner.Controls.MyButton();
            this.myButton7 = new MissionPlanner.Controls.MyButton();
            this.myButton8 = new MissionPlanner.Controls.MyButton();
            this.myButton9 = new MissionPlanner.Controls.MyButton();
            this.myButton10 = new MissionPlanner.Controls.MyButton();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.StylePresetTable = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.myButton11 = new MissionPlanner.Controls.MyButton();
            this.myButton12 = new MissionPlanner.Controls.MyButton();
            this.myButton13 = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.myButton14 = new MissionPlanner.Controls.MyButton();
            this.myButton15 = new MissionPlanner.Controls.MyButton();
            this.MainTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.StylePresetTable.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTable
            // 
            this.MainTable.ColumnCount = 1;
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTable.Controls.Add(this.StylePresetTable, 0, 0);
            this.MainTable.Controls.Add(this.splitContainer1, 0, 1);
            this.MainTable.Controls.Add(this.tableLayoutPanel5, 0, 2);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 0);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 3;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.Size = new System.Drawing.Size(800, 450);
            this.MainTable.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 38);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(794, 374);
            this.splitContainer1.SplitterDistance = 284;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(284, 374);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listBox1);
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(276, 348);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBox2);
            this.tabPage2.Controls.Add(this.tableLayoutPanel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(276, 348);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(506, 374);
            this.propertyGrid1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Controls.Add(this.myButton1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.myButton2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.myButton3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.myButton4, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.myButton5, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 315);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(270, 30);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(270, 312);
            this.listBox1.TabIndex = 1;
            // 
            // myButton1
            // 
            this.myButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton1.Location = new System.Drawing.Point(3, 3);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new System.Drawing.Size(48, 23);
            this.myButton1.TabIndex = 0;
            this.myButton1.Text = "myButton1";
            this.myButton1.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton1.UseVisualStyleBackColor = true;
            // 
            // myButton2
            // 
            this.myButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton2.Location = new System.Drawing.Point(57, 3);
            this.myButton2.Name = "myButton2";
            this.myButton2.Size = new System.Drawing.Size(48, 23);
            this.myButton2.TabIndex = 1;
            this.myButton2.Text = "myButton2";
            this.myButton2.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton2.UseVisualStyleBackColor = true;
            // 
            // myButton3
            // 
            this.myButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton3.Location = new System.Drawing.Point(111, 3);
            this.myButton3.Name = "myButton3";
            this.myButton3.Size = new System.Drawing.Size(48, 23);
            this.myButton3.TabIndex = 2;
            this.myButton3.Text = "myButton3";
            this.myButton3.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton3.UseVisualStyleBackColor = true;
            // 
            // myButton4
            // 
            this.myButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton4.Location = new System.Drawing.Point(165, 3);
            this.myButton4.Name = "myButton4";
            this.myButton4.Size = new System.Drawing.Size(48, 23);
            this.myButton4.TabIndex = 3;
            this.myButton4.Text = "myButton4";
            this.myButton4.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton4.UseVisualStyleBackColor = true;
            // 
            // myButton5
            // 
            this.myButton5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton5.Location = new System.Drawing.Point(219, 3);
            this.myButton5.Name = "myButton5";
            this.myButton5.Size = new System.Drawing.Size(48, 23);
            this.myButton5.TabIndex = 4;
            this.myButton5.Text = "myButton5";
            this.myButton5.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton5.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.Controls.Add(this.myButton6, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.myButton7, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.myButton8, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.myButton9, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.myButton10, 4, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 315);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(270, 30);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // myButton6
            // 
            this.myButton6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton6.Location = new System.Drawing.Point(3, 3);
            this.myButton6.Name = "myButton6";
            this.myButton6.Size = new System.Drawing.Size(48, 23);
            this.myButton6.TabIndex = 0;
            this.myButton6.Text = "myButton6";
            this.myButton6.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton6.UseVisualStyleBackColor = true;
            // 
            // myButton7
            // 
            this.myButton7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton7.Location = new System.Drawing.Point(57, 3);
            this.myButton7.Name = "myButton7";
            this.myButton7.Size = new System.Drawing.Size(48, 23);
            this.myButton7.TabIndex = 1;
            this.myButton7.Text = "myButton7";
            this.myButton7.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton7.UseVisualStyleBackColor = true;
            // 
            // myButton8
            // 
            this.myButton8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton8.Location = new System.Drawing.Point(111, 3);
            this.myButton8.Name = "myButton8";
            this.myButton8.Size = new System.Drawing.Size(48, 23);
            this.myButton8.TabIndex = 2;
            this.myButton8.Text = "myButton8";
            this.myButton8.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton8.UseVisualStyleBackColor = true;
            // 
            // myButton9
            // 
            this.myButton9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton9.Location = new System.Drawing.Point(165, 3);
            this.myButton9.Name = "myButton9";
            this.myButton9.Size = new System.Drawing.Size(48, 23);
            this.myButton9.TabIndex = 3;
            this.myButton9.Text = "myButton9";
            this.myButton9.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton9.UseVisualStyleBackColor = true;
            // 
            // myButton10
            // 
            this.myButton10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.myButton10.Location = new System.Drawing.Point(219, 3);
            this.myButton10.Name = "myButton10";
            this.myButton10.Size = new System.Drawing.Size(48, 23);
            this.myButton10.TabIndex = 4;
            this.myButton10.Text = "myButton10";
            this.myButton10.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton10.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(3, 3);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(270, 312);
            this.listBox2.TabIndex = 2;
            // 
            // StylePresetTable
            // 
            this.StylePresetTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StylePresetTable.AutoSize = true;
            this.StylePresetTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.StylePresetTable.ColumnCount = 5;
            this.StylePresetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.StylePresetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.StylePresetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.StylePresetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.StylePresetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.StylePresetTable.Controls.Add(this.myButton13, 4, 0);
            this.StylePresetTable.Controls.Add(this.label1, 0, 0);
            this.StylePresetTable.Controls.Add(this.myButton12, 3, 0);
            this.StylePresetTable.Controls.Add(this.comboBox1, 1, 0);
            this.StylePresetTable.Controls.Add(this.myButton11, 2, 0);
            this.StylePresetTable.Location = new System.Drawing.Point(3, 3);
            this.StylePresetTable.Name = "StylePresetTable";
            this.StylePresetTable.RowCount = 1;
            this.StylePresetTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.StylePresetTable.Size = new System.Drawing.Size(794, 29);
            this.StylePresetTable.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Style: ";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(45, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(151, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // myButton11
            // 
            this.myButton11.Location = new System.Drawing.Point(554, 3);
            this.myButton11.Name = "myButton11";
            this.myButton11.Size = new System.Drawing.Size(75, 23);
            this.myButton11.TabIndex = 0;
            this.myButton11.Text = "myButton11";
            this.myButton11.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton11.UseVisualStyleBackColor = true;
            // 
            // myButton12
            // 
            this.myButton12.Location = new System.Drawing.Point(635, 3);
            this.myButton12.Name = "myButton12";
            this.myButton12.Size = new System.Drawing.Size(75, 23);
            this.myButton12.TabIndex = 1;
            this.myButton12.Text = "myButton12";
            this.myButton12.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton12.UseVisualStyleBackColor = true;
            // 
            // myButton13
            // 
            this.myButton13.Location = new System.Drawing.Point(716, 3);
            this.myButton13.Name = "myButton13";
            this.myButton13.Size = new System.Drawing.Size(75, 23);
            this.myButton13.TabIndex = 2;
            this.myButton13.Text = "myButton13";
            this.myButton13.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton13.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.myButton14, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.myButton15, 2, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 418);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(794, 29);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // myButton14
            // 
            this.myButton14.Location = new System.Drawing.Point(635, 3);
            this.myButton14.Name = "myButton14";
            this.myButton14.Size = new System.Drawing.Size(75, 23);
            this.myButton14.TabIndex = 1;
            this.myButton14.Text = "myButton14";
            this.myButton14.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton14.UseVisualStyleBackColor = true;
            // 
            // myButton15
            // 
            this.myButton15.Location = new System.Drawing.Point(716, 3);
            this.myButton15.Name = "myButton15";
            this.myButton15.Size = new System.Drawing.Size(75, 23);
            this.myButton15.TabIndex = 2;
            this.myButton15.Text = "myButton15";
            this.myButton15.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton15.UseVisualStyleBackColor = true;
            // 
            // MissionStyleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainTable);
            this.Name = "MissionStyleEditor";
            this.Text = "MissionStyleEditor";
            this.MainTable.ResumeLayout(false);
            this.MainTable.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.StylePresetTable.ResumeLayout(false);
            this.StylePresetTable.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private MyButton myButton1;
        private MyButton myButton2;
        private MyButton myButton3;
        private MyButton myButton4;
        private MyButton myButton5;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private MyButton myButton6;
        private MyButton myButton7;
        private MyButton myButton8;
        private MyButton myButton9;
        private MyButton myButton10;
        private System.Windows.Forms.TableLayoutPanel StylePresetTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private MyButton myButton13;
        private MyButton myButton12;
        private MyButton myButton11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private MyButton myButton14;
        private MyButton myButton15;
    }
}