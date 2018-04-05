namespace Heiflow.Controls.WinForm.Modflow
{
    partial class RegularGridGenForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegularGridGenForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.cmbLayers = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbMaxX = new System.Windows.Forms.TextBox();
            this.tbMinY = new System.Windows.Forms.TextBox();
            this.tbMaxY = new System.Windows.Forms.TextBox();
            this.tbMinX = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rbtnByCellSize = new System.Windows.Forms.RadioButton();
            this.rbtnByCellNumber = new System.Windows.Forms.RadioButton();
            this.tbYSize = new System.Windows.Forms.TextBox();
            this.tbXSize = new System.Windows.Forms.TextBox();
            this.tbYNum = new System.Windows.Forms.TextBox();
            this.tbXNum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.Origin = new System.Windows.Forms.GroupBox();
            this.tbOriginY = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbOriginX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnLayerGroup = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbRasterLayer = new System.Windows.Forms.ComboBox();
            this.cmbAvMethod = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.Origin.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnCancel.Location = new System.Drawing.Point(424, 623);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 30);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnOk.Location = new System.Drawing.Point(306, 623);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(95, 30);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "Create";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbLayers
            // 
            this.cmbLayers.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.cmbLayers.FormattingEnabled = true;
            this.cmbLayers.Location = new System.Drawing.Point(7, 52);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(270, 25);
            this.cmbLayers.TabIndex = 18;
            this.cmbLayers.SelectedIndexChanged += new System.EventHandler(this.cmbLayers_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.tbMaxX);
            this.groupBox1.Controls.Add(this.tbMinY);
            this.groupBox1.Controls.Add(this.tbMaxY);
            this.groupBox1.Controls.Add(this.tbMinX);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.groupBox1.Location = new System.Drawing.Point(13, 242);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(524, 161);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spatial Extent";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.panel1.Location = new System.Drawing.Point(157, 62);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 43);
            this.panel1.TabIndex = 15;
            // 
            // tbMaxX
            // 
            this.tbMaxX.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbMaxX.Location = new System.Drawing.Point(377, 71);
            this.tbMaxX.Name = "tbMaxX";
            this.tbMaxX.Size = new System.Drawing.Size(130, 25);
            this.tbMaxX.TabIndex = 14;
            this.tbMaxX.Text = "0";
            // 
            // tbMinY
            // 
            this.tbMinY.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbMinY.Location = new System.Drawing.Point(192, 118);
            this.tbMinY.Name = "tbMinY";
            this.tbMinY.Size = new System.Drawing.Size(130, 25);
            this.tbMinY.TabIndex = 14;
            this.tbMinY.Text = "0";
            // 
            // tbMaxY
            // 
            this.tbMaxY.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbMaxY.Location = new System.Drawing.Point(192, 24);
            this.tbMaxY.Name = "tbMaxY";
            this.tbMaxY.Size = new System.Drawing.Size(130, 25);
            this.tbMaxY.TabIndex = 14;
            this.tbMaxY.Text = "0";
            // 
            // tbMinX
            // 
            this.tbMinX.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbMinX.Location = new System.Drawing.Point(7, 71);
            this.tbMinX.Name = "tbMinX";
            this.tbMinX.Size = new System.Drawing.Size(130, 25);
            this.tbMinX.TabIndex = 14;
            this.tbMinX.Text = "0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rbtnByCellSize);
            this.groupBox2.Controls.Add(this.rbtnByCellNumber);
            this.groupBox2.Controls.Add(this.tbYSize);
            this.groupBox2.Controls.Add(this.tbXSize);
            this.groupBox2.Controls.Add(this.tbYNum);
            this.groupBox2.Controls.Add(this.tbXNum);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.groupBox2.Location = new System.Drawing.Point(13, 480);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(524, 128);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cell Size";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label5.Location = new System.Drawing.Point(301, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 17);
            this.label5.TabIndex = 21;
            this.label5.Text = "Y-Dim size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label3.Location = new System.Drawing.Point(27, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 17);
            this.label3.TabIndex = 21;
            this.label3.Text = "Y-Dim cell number:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label4.Location = new System.Drawing.Point(300, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 17);
            this.label4.TabIndex = 21;
            this.label4.Text = "X-Dim size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label2.Location = new System.Drawing.Point(27, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 17);
            this.label2.TabIndex = 21;
            this.label2.Text = "X-Dim cell number:";
            // 
            // rbtnByCellSize
            // 
            this.rbtnByCellSize.AutoSize = true;
            this.rbtnByCellSize.Checked = true;
            this.rbtnByCellSize.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.rbtnByCellSize.Location = new System.Drawing.Point(303, 24);
            this.rbtnByCellSize.Name = "rbtnByCellSize";
            this.rbtnByCellSize.Size = new System.Drawing.Size(86, 21);
            this.rbtnByCellSize.TabIndex = 0;
            this.rbtnByCellSize.TabStop = true;
            this.rbtnByCellSize.Text = "By cell size";
            this.rbtnByCellSize.UseVisualStyleBackColor = true;
            this.rbtnByCellSize.CheckedChanged += new System.EventHandler(this.rbtnByCellSize_CheckedChanged);
            // 
            // rbtnByCellNumber
            // 
            this.rbtnByCellNumber.AutoSize = true;
            this.rbtnByCellNumber.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.rbtnByCellNumber.Location = new System.Drawing.Point(26, 24);
            this.rbtnByCellNumber.Name = "rbtnByCellNumber";
            this.rbtnByCellNumber.Size = new System.Drawing.Size(112, 21);
            this.rbtnByCellNumber.TabIndex = 0;
            this.rbtnByCellNumber.Text = " By cell number";
            this.rbtnByCellNumber.UseVisualStyleBackColor = true;
            this.rbtnByCellNumber.CheckedChanged += new System.EventHandler(this.rbtnByCellNumber_CheckedChanged);
            // 
            // tbYSize
            // 
            this.tbYSize.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbYSize.Location = new System.Drawing.Point(377, 92);
            this.tbYSize.Name = "tbYSize";
            this.tbYSize.Size = new System.Drawing.Size(100, 25);
            this.tbYSize.TabIndex = 14;
            this.tbYSize.Text = "1000";
            this.tbYSize.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // tbXSize
            // 
            this.tbXSize.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbXSize.Location = new System.Drawing.Point(377, 56);
            this.tbXSize.Name = "tbXSize";
            this.tbXSize.Size = new System.Drawing.Size(100, 25);
            this.tbXSize.TabIndex = 14;
            this.tbXSize.Text = "1000";
            this.tbXSize.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // tbYNum
            // 
            this.tbYNum.Enabled = false;
            this.tbYNum.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbYNum.Location = new System.Drawing.Point(161, 95);
            this.tbYNum.Name = "tbYNum";
            this.tbYNum.Size = new System.Drawing.Size(100, 25);
            this.tbYNum.TabIndex = 14;
            this.tbYNum.Text = "50";
            this.tbYNum.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // tbXNum
            // 
            this.tbXNum.Enabled = false;
            this.tbXNum.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbXNum.Location = new System.Drawing.Point(161, 59);
            this.tbXNum.Name = "tbXNum";
            this.tbXNum.Size = new System.Drawing.Size(100, 25);
            this.tbXNum.TabIndex = 14;
            this.tbXNum.Text = "50";
            this.tbXNum.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 17);
            this.label1.TabIndex = 21;
            this.label1.Text = "Select the model boundary (must be a polygon)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label6.Location = new System.Drawing.Point(8, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(149, 17);
            this.label6.TabIndex = 22;
            this.label6.Text = "Number of vertical layers";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.numericUpDown1.Location = new System.Drawing.Point(165, 24);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(178, 25);
            this.numericUpDown1.TabIndex = 23;
            this.numericUpDown1.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // Origin
            // 
            this.Origin.Controls.Add(this.tbOriginY);
            this.Origin.Controls.Add(this.label8);
            this.Origin.Controls.Add(this.tbOriginX);
            this.Origin.Controls.Add(this.label7);
            this.Origin.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.Origin.Location = new System.Drawing.Point(15, 404);
            this.Origin.Name = "Origin";
            this.Origin.Size = new System.Drawing.Size(524, 68);
            this.Origin.TabIndex = 24;
            this.Origin.TabStop = false;
            this.Origin.Text = "Origin";
            // 
            // tbOriginY
            // 
            this.tbOriginY.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbOriginY.Location = new System.Drawing.Point(375, 32);
            this.tbOriginY.Name = "tbOriginY";
            this.tbOriginY.Size = new System.Drawing.Size(100, 25);
            this.tbOriginY.TabIndex = 14;
            this.tbOriginY.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label8.Location = new System.Drawing.Point(284, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 17);
            this.label8.TabIndex = 21;
            this.label8.Text = "Y coordinate:";
            // 
            // tbOriginX
            // 
            this.tbOriginX.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.tbOriginX.Location = new System.Drawing.Point(159, 29);
            this.tbOriginX.Name = "tbOriginX";
            this.tbOriginX.Size = new System.Drawing.Size(100, 25);
            this.tbOriginX.TabIndex = 14;
            this.tbOriginX.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label7.Location = new System.Drawing.Point(61, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 17);
            this.label7.TabIndex = 21;
            this.label7.Text = "X coordinate:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDown1);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.btnLayerGroup);
            this.groupBox3.Location = new System.Drawing.Point(13, 166);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(524, 70);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Aquifer Layers";
            // 
            // btnLayerGroup
            // 
            this.btnLayerGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLayerGroup.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnLayerGroup.Location = new System.Drawing.Point(363, 22);
            this.btnLayerGroup.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnLayerGroup.Name = "btnLayerGroup";
            this.btnLayerGroup.Size = new System.Drawing.Size(143, 30);
            this.btnLayerGroup.TabIndex = 17;
            this.btnLayerGroup.Text = "Layer Group...";
            this.btnLayerGroup.UseVisualStyleBackColor = true;
            this.btnLayerGroup.Click += new System.EventHandler(this.btnLayerGroup_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.cmbAvMethod);
            this.groupBox4.Controls.Add(this.cmbRasterLayer);
            this.groupBox4.Controls.Add(this.cmbLayers);
            this.groupBox4.Location = new System.Drawing.Point(13, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(524, 157);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Spatial Data";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label9.Location = new System.Drawing.Point(8, 89);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 17);
            this.label9.TabIndex = 21;
            this.label9.Text = "Select the DEM";
            // 
            // cmbRasterLayer
            // 
            this.cmbRasterLayer.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.cmbRasterLayer.FormattingEnabled = true;
            this.cmbRasterLayer.Location = new System.Drawing.Point(7, 109);
            this.cmbRasterLayer.Name = "cmbRasterLayer";
            this.cmbRasterLayer.Size = new System.Drawing.Size(270, 25);
            this.cmbRasterLayer.TabIndex = 18;
            this.cmbRasterLayer.SelectedIndexChanged += new System.EventHandler(this.cmbRasterLayer_SelectedIndexChanged);
            // 
            // cmbAvMethod
            // 
            this.cmbAvMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAvMethod.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.cmbAvMethod.FormattingEnabled = true;
            this.cmbAvMethod.Items.AddRange(new object[] {
            "Mean",
            "Median"});
            this.cmbAvMethod.Location = new System.Drawing.Point(293, 109);
            this.cmbAvMethod.Name = "cmbAvMethod";
            this.cmbAvMethod.Size = new System.Drawing.Size(213, 25);
            this.cmbAvMethod.TabIndex = 18;
            this.cmbAvMethod.SelectedIndexChanged += new System.EventHandler(this.cmbRasterLayer_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.label10.Location = new System.Drawing.Point(294, 89);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(148, 17);
            this.label10.TabIndex = 21;
            this.label10.Text = "Select averaging method";
            // 
            // RegularGridGenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(548, 663);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Origin);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "RegularGridGenForm";
            this.ShowInTaskbar = false;
            this.Text = "Creat Finite Difference Grid";
            this.Load += new System.EventHandler(this.RegularGridGenForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.Origin.ResumeLayout(false);
            this.Origin.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ComboBox cmbLayers;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbMaxX;
        private System.Windows.Forms.TextBox tbMinY;
        private System.Windows.Forms.TextBox tbMaxY;
        private System.Windows.Forms.TextBox tbMinX;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbtnByCellSize;
        private System.Windows.Forms.RadioButton rbtnByCellNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbYSize;
        private System.Windows.Forms.TextBox tbXSize;
        private System.Windows.Forms.TextBox tbYNum;
        private System.Windows.Forms.TextBox tbXNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox Origin;
        private System.Windows.Forms.TextBox tbOriginY;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbOriginX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbRasterLayer;
        private System.Windows.Forms.Button btnLayerGroup;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbAvMethod;
    }
}