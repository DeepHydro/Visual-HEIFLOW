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
            this.chkSlopeAspect = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbAvMethod = new System.Windows.Forms.ComboBox();
            this.cmbRasterLayer = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.olvSoilLayers = new BrightIdeasSoftware.DataListView();
            this.colLayerName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colCHANI = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLAYWET = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.Origin.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvSoilLayers)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(402, 666);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 29);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOk.Location = new System.Drawing.Point(299, 666);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(89, 29);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "Create";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbLayers
            // 
            this.cmbLayers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbLayers.FormattingEnabled = true;
            this.cmbLayers.Location = new System.Drawing.Point(6, 54);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(240, 28);
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
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(6, 262);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 153);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spatial Extent";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.panel1.Location = new System.Drawing.Point(140, 59);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(178, 41);
            this.panel1.TabIndex = 15;
            // 
            // tbMaxX
            // 
            this.tbMaxX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbMaxX.Location = new System.Drawing.Point(335, 68);
            this.tbMaxX.Name = "tbMaxX";
            this.tbMaxX.Size = new System.Drawing.Size(116, 27);
            this.tbMaxX.TabIndex = 14;
            this.tbMaxX.Text = "0";
            // 
            // tbMinY
            // 
            this.tbMinY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbMinY.Location = new System.Drawing.Point(171, 110);
            this.tbMinY.Name = "tbMinY";
            this.tbMinY.Size = new System.Drawing.Size(116, 27);
            this.tbMinY.TabIndex = 14;
            this.tbMinY.Text = "0";
            // 
            // tbMaxY
            // 
            this.tbMaxY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbMaxY.Location = new System.Drawing.Point(171, 23);
            this.tbMaxY.Name = "tbMaxY";
            this.tbMaxY.Size = new System.Drawing.Size(116, 27);
            this.tbMaxY.TabIndex = 14;
            this.tbMaxY.Text = "0";
            // 
            // tbMinX
            // 
            this.tbMinX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbMinX.Location = new System.Drawing.Point(6, 68);
            this.tbMinX.Name = "tbMinX";
            this.tbMinX.Size = new System.Drawing.Size(116, 27);
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
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 484);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(470, 129);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cell Size";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.Location = new System.Drawing.Point(268, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 20);
            this.label5.TabIndex = 21;
            this.label5.Text = "Y-Dim size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(24, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 20);
            this.label3.TabIndex = 21;
            this.label3.Text = "Y-Dim cell number:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.Location = new System.Drawing.Point(267, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 20);
            this.label4.TabIndex = 21;
            this.label4.Text = "X-Dim size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(24, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "X-Dim cell number:";
            // 
            // rbtnByCellSize
            // 
            this.rbtnByCellSize.AutoSize = true;
            this.rbtnByCellSize.Checked = true;
            this.rbtnByCellSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbtnByCellSize.Location = new System.Drawing.Point(269, 23);
            this.rbtnByCellSize.Name = "rbtnByCellSize";
            this.rbtnByCellSize.Size = new System.Drawing.Size(102, 24);
            this.rbtnByCellSize.TabIndex = 0;
            this.rbtnByCellSize.TabStop = true;
            this.rbtnByCellSize.Text = "By cell size";
            this.rbtnByCellSize.UseVisualStyleBackColor = true;
            this.rbtnByCellSize.CheckedChanged += new System.EventHandler(this.rbtnByCellSize_CheckedChanged);
            // 
            // rbtnByCellNumber
            // 
            this.rbtnByCellNumber.AutoSize = true;
            this.rbtnByCellNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbtnByCellNumber.Location = new System.Drawing.Point(29, 23);
            this.rbtnByCellNumber.Name = "rbtnByCellNumber";
            this.rbtnByCellNumber.Size = new System.Drawing.Size(132, 24);
            this.rbtnByCellNumber.TabIndex = 0;
            this.rbtnByCellNumber.Text = " By cell number";
            this.rbtnByCellNumber.UseVisualStyleBackColor = true;
            this.rbtnByCellNumber.CheckedChanged += new System.EventHandler(this.rbtnByCellNumber_CheckedChanged);
            // 
            // tbYSize
            // 
            this.tbYSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbYSize.Location = new System.Drawing.Point(352, 89);
            this.tbYSize.Name = "tbYSize";
            this.tbYSize.Size = new System.Drawing.Size(89, 27);
            this.tbYSize.TabIndex = 14;
            this.tbYSize.Text = "1000";
            this.tbYSize.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // tbXSize
            // 
            this.tbXSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbXSize.Location = new System.Drawing.Point(351, 52);
            this.tbXSize.Name = "tbXSize";
            this.tbXSize.Size = new System.Drawing.Size(89, 27);
            this.tbXSize.TabIndex = 14;
            this.tbXSize.Text = "1000";
            this.tbXSize.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // tbYNum
            // 
            this.tbYNum.Enabled = false;
            this.tbYNum.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbYNum.Location = new System.Drawing.Point(165, 90);
            this.tbYNum.Name = "tbYNum";
            this.tbYNum.Size = new System.Drawing.Size(89, 27);
            this.tbYNum.TabIndex = 14;
            this.tbYNum.Text = "50";
            this.tbYNum.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // tbXNum
            // 
            this.tbXNum.Enabled = false;
            this.tbXNum.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbXNum.Location = new System.Drawing.Point(165, 57);
            this.tbXNum.Name = "tbXNum";
            this.tbXNum.Size = new System.Drawing.Size(89, 27);
            this.tbXNum.TabIndex = 14;
            this.tbXNum.Text = "50";
            this.tbXNum.TextChanged += new System.EventHandler(this.tbCellSize_TextChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(5, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(326, 20);
            this.label1.TabIndex = 21;
            this.label1.Text = "Select the model boundary (must be a polygon)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.Location = new System.Drawing.Point(7, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(175, 20);
            this.label6.TabIndex = 22;
            this.label6.Text = "Number of vertical layers";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericUpDown1.Location = new System.Drawing.Point(185, 25);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 27);
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
            this.Origin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Origin.Location = new System.Drawing.Point(6, 417);
            this.Origin.Name = "Origin";
            this.Origin.Size = new System.Drawing.Size(470, 65);
            this.Origin.TabIndex = 24;
            this.Origin.TabStop = false;
            this.Origin.Text = "Origin";
            // 
            // tbOriginY
            // 
            this.tbOriginY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbOriginY.Location = new System.Drawing.Point(333, 28);
            this.tbOriginY.Name = "tbOriginY";
            this.tbOriginY.Size = new System.Drawing.Size(89, 27);
            this.tbOriginY.TabIndex = 14;
            this.tbOriginY.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label8.Location = new System.Drawing.Point(232, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 20);
            this.label8.TabIndex = 21;
            this.label8.Text = "Y coordinate:";
            // 
            // tbOriginX
            // 
            this.tbOriginX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbOriginX.Location = new System.Drawing.Point(125, 27);
            this.tbOriginX.Name = "tbOriginX";
            this.tbOriginX.Size = new System.Drawing.Size(75, 27);
            this.tbOriginX.TabIndex = 14;
            this.tbOriginX.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.Location = new System.Drawing.Point(24, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 20);
            this.label7.TabIndex = 21;
            this.label7.Text = "X coordinate:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDown1);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.btnLayerGroup);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(6, 193);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(470, 67);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Aquifer Layers";
            // 
            // btnLayerGroup
            // 
            this.btnLayerGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLayerGroup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLayerGroup.Location = new System.Drawing.Point(331, 24);
            this.btnLayerGroup.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnLayerGroup.Name = "btnLayerGroup";
            this.btnLayerGroup.Size = new System.Drawing.Size(127, 29);
            this.btnLayerGroup.TabIndex = 17;
            this.btnLayerGroup.Text = "Layer Group...";
            this.btnLayerGroup.UseVisualStyleBackColor = true;
            this.btnLayerGroup.Click += new System.EventHandler(this.btnLayerGroup_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkSlopeAspect);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.cmbAvMethod);
            this.groupBox4.Controls.Add(this.cmbRasterLayer);
            this.groupBox4.Controls.Add(this.cmbLayers);
            this.groupBox4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(470, 185);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Spatial Data";
            // 
            // chkSlopeAspect
            // 
            this.chkSlopeAspect.AutoSize = true;
            this.chkSlopeAspect.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkSlopeAspect.Location = new System.Drawing.Point(6, 153);
            this.chkSlopeAspect.Name = "chkSlopeAspect";
            this.chkSlopeAspect.Size = new System.Drawing.Size(208, 24);
            this.chkSlopeAspect.TabIndex = 22;
            this.chkSlopeAspect.Text = "Calculate slope and aspect";
            this.chkSlopeAspect.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.Location = new System.Drawing.Point(261, 85);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(175, 20);
            this.label10.TabIndex = 21;
            this.label10.Text = "Select averaging method";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label9.Location = new System.Drawing.Point(7, 85);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 20);
            this.label9.TabIndex = 21;
            this.label9.Text = "Select the DEM";
            // 
            // cmbAvMethod
            // 
            this.cmbAvMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAvMethod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbAvMethod.FormattingEnabled = true;
            this.cmbAvMethod.Items.AddRange(new object[] {
            "Mean",
            "Median"});
            this.cmbAvMethod.Location = new System.Drawing.Point(265, 114);
            this.cmbAvMethod.Name = "cmbAvMethod";
            this.cmbAvMethod.Size = new System.Drawing.Size(190, 28);
            this.cmbAvMethod.TabIndex = 18;
            this.cmbAvMethod.SelectedIndexChanged += new System.EventHandler(this.cmbRasterLayer_SelectedIndexChanged);
            // 
            // cmbRasterLayer
            // 
            this.cmbRasterLayer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbRasterLayer.FormattingEnabled = true;
            this.cmbRasterLayer.Location = new System.Drawing.Point(6, 114);
            this.cmbRasterLayer.Name = "cmbRasterLayer";
            this.cmbRasterLayer.Size = new System.Drawing.Size(240, 28);
            this.cmbRasterLayer.TabIndex = 18;
            this.cmbRasterLayer.SelectedIndexChanged += new System.EventHandler(this.cmbRasterLayer_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(495, 660);
            this.tabControl1.TabIndex = 27;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.numericUpDown2);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.olvSoilLayers);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(487, 627);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Soil";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericUpDown2.Location = new System.Drawing.Point(183, 15);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(120, 27);
            this.numericUpDown2.TabIndex = 23;
            this.numericUpDown2.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label11.Location = new System.Drawing.Point(5, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(150, 20);
            this.label11.TabIndex = 22;
            this.label11.Text = "Number of soil layers";
            // 
            // olvSoilLayers
            // 
            this.olvSoilLayers.AllColumns.Add(this.colLayerName);
            this.olvSoilLayers.AllColumns.Add(this.colCHANI);
            this.olvSoilLayers.AllColumns.Add(this.colLAYWET);
            this.olvSoilLayers.AllColumns.Add(this.olvColumn1);
            this.olvSoilLayers.AllColumns.Add(this.olvColumn2);
            this.olvSoilLayers.AllowColumnReorder = true;
            this.olvSoilLayers.AllowDrop = true;
            this.olvSoilLayers.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvSoilLayers.CellEditUseWholeCell = false;
            this.olvSoilLayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLayerName,
            this.colCHANI,
            this.colLAYWET,
            this.olvColumn1,
            this.olvColumn2});
            this.olvSoilLayers.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvSoilLayers.DataSource = null;
            this.olvSoilLayers.EmptyListMsg = "";
            this.olvSoilLayers.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvSoilLayers.FullRowSelect = true;
            this.olvSoilLayers.GridLines = true;
            this.olvSoilLayers.GroupWithItemCountFormat = "";
            this.olvSoilLayers.GroupWithItemCountSingularFormat = "";
            this.olvSoilLayers.HideSelection = false;
            this.olvSoilLayers.Location = new System.Drawing.Point(6, 58);
            this.olvSoilLayers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvSoilLayers.Name = "olvSoilLayers";
            this.olvSoilLayers.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvSoilLayers.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvSoilLayers.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvSoilLayers.ShowCommandMenuOnRightClick = true;
            this.olvSoilLayers.ShowGroups = false;
            this.olvSoilLayers.ShowImagesOnSubItems = true;
            this.olvSoilLayers.ShowItemToolTips = true;
            this.olvSoilLayers.Size = new System.Drawing.Size(474, 563);
            this.olvSoilLayers.TabIndex = 27;
            this.olvSoilLayers.UseCellFormatEvents = true;
            this.olvSoilLayers.UseCompatibleStateImageBehavior = false;
            this.olvSoilLayers.UseFilterIndicator = true;
            this.olvSoilLayers.UseFiltering = true;
            this.olvSoilLayers.UseHotItem = true;
            this.olvSoilLayers.UseTranslucentHotItem = true;
            this.olvSoilLayers.View = System.Windows.Forms.View.Details;
            // 
            // colLayerName
            // 
            this.colLayerName.AspectName = "Name";
            this.colLayerName.ButtonPadding = new System.Drawing.Size(10, 10);
            this.colLayerName.CellEditUseWholeCell = true;
            this.colLayerName.IsTileViewColumn = true;
            this.colLayerName.Text = "Layer Name";
            this.colLayerName.UseInitialLetterForGroup = true;
            this.colLayerName.Width = 100;
            // 
            // colCHANI
            // 
            this.colCHANI.AspectName = "SoilDepth";
            this.colCHANI.CellEditUseWholeCell = true;
            this.colCHANI.Text = "Soil Depth";
            this.colCHANI.ToolTipText = "a value for each layer that is a flag or the horizontal anisotropy";
            this.colCHANI.Width = 90;
            // 
            // colLAYWET
            // 
            this.colLAYWET.AspectName = "InitGVR";
            this.colLAYWET.CellEditUseWholeCell = true;
            this.colLAYWET.Text = "Init GVR";
            this.colLAYWET.ToolTipText = "Indicates if wetting is active.";
            this.colLAYWET.Width = 90;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "InitCPR";
            this.olvColumn1.Text = "Init CPR";
            this.olvColumn1.Width = 90;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "InitPFR";
            this.olvColumn2.Text = "Init PFR";
            this.olvColumn2.Width = 90;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.Origin);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(487, 627);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Aquifer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // RegularGridGenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(501, 702);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "RegularGridGenForm";
            this.ShowInTaskbar = false;
            this.Text = "Create Finite Difference Grid";
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvSoilLayers)).EndInit();
            this.tabPage2.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox chkSlopeAspect;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label11;
        private BrightIdeasSoftware.DataListView olvSoilLayers;
        private BrightIdeasSoftware.OLVColumn colLayerName;
        private BrightIdeasSoftware.OLVColumn colCHANI;
        private BrightIdeasSoftware.OLVColumn colLAYWET;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
    }
}