namespace Heiflow.Controls.WinForm.Controls
{
    partial class GridProfileAnimator
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridProfileAnimator));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxVariable = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.cmbSpeed = new System.Windows.Forms.ComboBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.tbCurDate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxCols = new System.Windows.Forms.ComboBox();
            this.comboBoxRows = new System.Windows.Forms.ComboBox();
            this.radioButtonRow = new System.Windows.Forms.RadioButton();
            this.radioButtonCol = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.radioButtonSingleVar = new System.Windows.Forms.RadioButton();
            this.radioButtonMultiVar = new System.Windows.Forms.RadioButton();
            this.cmbDate = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnExportData = new System.Windows.Forms.Button();
            this.view3DControl1 = new Heiflow.Controls.WinForm.Controls.View3DControl();
            this.colorSlider1 = new Heiflow.Controls.WinForm.ColorSlider();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.view3DControl1);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1505, 1030);
            this.splitContainer1.SplitterDistance = 412;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.comboBoxVariable);
            this.groupBox3.Controls.Add(this.radioButtonMultiVar);
            this.groupBox3.Controls.Add(this.radioButtonSingleVar);
            this.groupBox3.Location = new System.Drawing.Point(9, 288);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(382, 178);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Variable";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(285, 31);
            this.label4.TabIndex = 2;
            this.label4.Text = "Select the variable to view:";
            // 
            // comboBoxVariable
            // 
            this.comboBoxVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxVariable.Enabled = false;
            this.comboBoxVariable.FormattingEnabled = true;
            this.comboBoxVariable.Location = new System.Drawing.Point(14, 119);
            this.comboBoxVariable.Name = "comboBoxVariable";
            this.comboBoxVariable.Size = new System.Drawing.Size(362, 38);
            this.comboBoxVariable.TabIndex = 1;
            this.comboBoxVariable.SelectedIndexChanged += new System.EventHandler(this.comboBoxCols_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cmbSpeed);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Controls.Add(this.tbCurDate);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnPlay);
            this.groupBox2.Location = new System.Drawing.Point(9, 675);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(382, 273);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(167, 31);
            this.label5.TabIndex = 2;
            this.label5.Text = "Anation speed:";
            // 
            // btnStop
            // 
            this.btnStop.BackgroundImage = global::Heiflow.Controls.WinForm.Properties.Resources.stop64;
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStop.Location = new System.Drawing.Point(68, 196);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(48, 48);
            this.btnStop.TabIndex = 4;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // cmbSpeed
            // 
            this.cmbSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpeed.FormattingEnabled = true;
            this.cmbSpeed.Items.AddRange(new object[] {
            "Very Slow",
            "Slow",
            "Normal",
            "Fast",
            "Very Fast"});
            this.cmbSpeed.Location = new System.Drawing.Point(11, 65);
            this.cmbSpeed.Name = "cmbSpeed";
            this.cmbSpeed.Size = new System.Drawing.Size(360, 38);
            this.cmbSpeed.TabIndex = 1;
            this.cmbSpeed.SelectedIndexChanged += new System.EventHandler(this.cmbSpeed_SelectedIndexChanged);
            // 
            // btnPlay
            // 
            this.btnPlay.BackgroundImage = global::Heiflow.Controls.WinForm.Properties.Resources.play64;
            this.btnPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPlay.Location = new System.Drawing.Point(14, 196);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(48, 48);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // tbCurDate
            // 
            this.tbCurDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCurDate.BackColor = System.Drawing.SystemColors.Info;
            this.tbCurDate.Location = new System.Drawing.Point(10, 142);
            this.tbCurDate.Name = "tbCurDate";
            this.tbCurDate.ReadOnly = true;
            this.tbCurDate.Size = new System.Drawing.Size(360, 37);
            this.tbCurDate.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 31);
            this.label3.TabIndex = 2;
            this.label3.Text = "Current time:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBoxCols);
            this.groupBox1.Controls.Add(this.radioButtonCol);
            this.groupBox1.Controls.Add(this.radioButtonRow);
            this.groupBox1.Controls.Add(this.comboBoxRows);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(383, 258);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grid";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(287, 31);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select the column number:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 31);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select the row number:";
            // 
            // comboBoxCols
            // 
            this.comboBoxCols.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCols.Enabled = false;
            this.comboBoxCols.FormattingEnabled = true;
            this.comboBoxCols.Location = new System.Drawing.Point(7, 197);
            this.comboBoxCols.Name = "comboBoxCols";
            this.comboBoxCols.Size = new System.Drawing.Size(362, 38);
            this.comboBoxCols.TabIndex = 1;
            this.comboBoxCols.SelectedIndexChanged += new System.EventHandler(this.comboBoxCols_SelectedIndexChanged);
            // 
            // comboBoxRows
            // 
            this.comboBoxRows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRows.FormattingEnabled = true;
            this.comboBoxRows.Location = new System.Drawing.Point(7, 122);
            this.comboBoxRows.Name = "comboBoxRows";
            this.comboBoxRows.Size = new System.Drawing.Size(362, 38);
            this.comboBoxRows.TabIndex = 1;
            this.comboBoxRows.SelectedIndexChanged += new System.EventHandler(this.comboBoxRows_SelectedIndexChanged);
            // 
            // radioButtonRow
            // 
            this.radioButtonRow.AutoSize = true;
            this.radioButtonRow.Location = new System.Drawing.Point(10, 37);
            this.radioButtonRow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonRow.Name = "radioButtonRow";
            this.radioButtonRow.Size = new System.Drawing.Size(137, 35);
            this.radioButtonRow.TabIndex = 0;
            this.radioButtonRow.Text = "Row View";
            this.radioButtonRow.UseVisualStyleBackColor = true;
            this.radioButtonRow.CheckedChanged += new System.EventHandler(this.radioButtonRow_CheckedChanged);
            // 
            // radioButtonCol
            // 
            this.radioButtonCol.AutoSize = true;
            this.radioButtonCol.Checked = true;
            this.radioButtonCol.Location = new System.Drawing.Point(168, 37);
            this.radioButtonCol.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonCol.Name = "radioButtonCol";
            this.radioButtonCol.Size = new System.Drawing.Size(173, 35);
            this.radioButtonCol.TabIndex = 0;
            this.radioButtonCol.TabStop = true;
            this.radioButtonCol.Text = "Column View";
            this.radioButtonCol.UseVisualStyleBackColor = true;
            this.radioButtonCol.CheckedChanged += new System.EventHandler(this.radioButtonCol_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.colorSlider1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 1000);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1089, 30);
            this.panel1.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // radioButtonSingleVar
            // 
            this.radioButtonSingleVar.AutoSize = true;
            this.radioButtonSingleVar.Location = new System.Drawing.Point(13, 37);
            this.radioButtonSingleVar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonSingleVar.Name = "radioButtonSingleVar";
            this.radioButtonSingleVar.Size = new System.Drawing.Size(102, 35);
            this.radioButtonSingleVar.TabIndex = 0;
            this.radioButtonSingleVar.Text = "Single";
            this.radioButtonSingleVar.UseVisualStyleBackColor = true;
            this.radioButtonSingleVar.CheckedChanged += new System.EventHandler(this.radioButtonSingleVar_CheckedChanged);
            // 
            // radioButtonMultiVar
            // 
            this.radioButtonMultiVar.AutoSize = true;
            this.radioButtonMultiVar.Checked = true;
            this.radioButtonMultiVar.Location = new System.Drawing.Point(171, 37);
            this.radioButtonMultiVar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonMultiVar.Name = "radioButtonMultiVar";
            this.radioButtonMultiVar.Size = new System.Drawing.Size(125, 35);
            this.radioButtonMultiVar.TabIndex = 0;
            this.radioButtonMultiVar.TabStop = true;
            this.radioButtonMultiVar.Text = "Multiple";
            this.radioButtonMultiVar.UseVisualStyleBackColor = true;
            this.radioButtonMultiVar.CheckedChanged += new System.EventHandler(this.radioButtonSingleVar_CheckedChanged);
            // 
            // cmbDate
            // 
            this.cmbDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDate.FormattingEnabled = true;
            this.cmbDate.Items.AddRange(new object[] {
            "Very Slow",
            "Slow",
            "Normal",
            "Fast",
            "Very Fast"});
            this.cmbDate.Location = new System.Drawing.Point(11, 68);
            this.cmbDate.Name = "cmbDate";
            this.cmbDate.Size = new System.Drawing.Size(360, 38);
            this.cmbDate.TabIndex = 1;
            this.cmbDate.SelectedIndexChanged += new System.EventHandler(this.cmbDate_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 31);
            this.label6.TabIndex = 2;
            this.label6.Text = "Select date:";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.cmbDate);
            this.groupBox4.Controls.Add(this.btnExportData);
            this.groupBox4.Location = new System.Drawing.Point(9, 479);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(382, 195);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Date";
            // 
            // btnExportData
            // 
            this.btnExportData.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnExportData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExportData.Location = new System.Drawing.Point(9, 130);
            this.btnExportData.Name = "btnExportData";
            this.btnExportData.Size = new System.Drawing.Size(187, 50);
            this.btnExportData.TabIndex = 4;
            this.btnExportData.Text = "Export Data";
            this.btnExportData.UseVisualStyleBackColor = false;
            this.btnExportData.Click += new System.EventHandler(this.btnExportData_Click);
            // 
            // view3DControl1
            // 
            this.view3DControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view3DControl1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.view3DControl1.Location = new System.Drawing.Point(0, 0);
            this.view3DControl1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.view3DControl1.Name = "view3DControl1";
            this.view3DControl1.Size = new System.Drawing.Size(1089, 1000);
            this.view3DControl1.TabIndex = 1;
            // 
            // colorSlider1
            // 
            this.colorSlider1.BackColor = System.Drawing.Color.Transparent;
            this.colorSlider1.BarInnerColor = System.Drawing.Color.GhostWhite;
            this.colorSlider1.BarOuterColor = System.Drawing.Color.White;
            this.colorSlider1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorSlider1.ElapsedInnerColor = System.Drawing.Color.DeepSkyBlue;
            this.colorSlider1.ElapsedOuterColor = System.Drawing.Color.White;
            this.colorSlider1.LargeChange = ((uint)(5u));
            this.colorSlider1.Location = new System.Drawing.Point(0, 0);
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.Size = new System.Drawing.Size(1089, 30);
            this.colorSlider1.SmallChange = ((uint)(1u));
            this.colorSlider1.TabIndex = 7;
            this.colorSlider1.Text = "50";
            this.colorSlider1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.ThumbSize = 30;
            this.colorSlider1.Value = 0;
            this.colorSlider1.ValueChanged += new System.EventHandler(this.colorSlider1_ValueChanged);
            this.colorSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider1_Scroll);
            // 
            // GridProfileAnimator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1505, 1030);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "GridProfileAnimator";
            this.Text = "Grid Profile Animator";
            this.Load += new System.EventHandler(this.GridProfileViewer_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RadioButton radioButtonRow;
        private System.Windows.Forms.RadioButton radioButtonCol;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxRows;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxCols;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbCurDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private View3DControl view3DControl1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBoxVariable;
        private ColorSlider colorSlider1;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ComboBox cmbSpeed;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RadioButton radioButtonMultiVar;
        private System.Windows.Forms.RadioButton radioButtonSingleVar;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbDate;
        private System.Windows.Forms.Button btnExportData;
    }
}