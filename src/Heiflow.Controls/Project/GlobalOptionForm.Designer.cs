namespace Heiflow.Controls.WinForm.Project
{
    partial class GlobalOptionForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cmbmxsziter = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioPETPM = new System.Windows.Forms.RadioButton();
            this.radioPETClimate = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioSRCloud = new System.Windows.Forms.RadioButton();
            this.radioSRTemp = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioRunoffNonLinear = new System.Windows.Forms.RadioButton();
            this.radioRunoffLinear = new System.Windows.Forms.RadioButton();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.checkSaveSMBudget = new System.Windows.Forms.CheckBox();
            this.checkPringDebug = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkSM = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbSM = new System.Windows.Forms.TextBox();
            this.btnSM = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbClimateFormat = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbMapFilename = new System.Windows.Forms.TextBox();
            this.checkMappedClimate = new System.Windows.Forms.CheckBox();
            this.btnMapFilename = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listVarDescriptions = new System.Windows.Forms.ListBox();
            this.listVars = new System.Windows.Forms.CheckedListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(618, 564);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(500, 564);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(731, 557);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(723, 524);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Modules";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBox1);
            this.groupBox6.Controls.Add(this.comboBox1);
            this.groupBox6.Controls.Add(this.cmbmxsziter);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(11, 334);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(696, 101);
            this.groupBox6.TabIndex = 24;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Solver";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkBox1.Location = new System.Drawing.Point(13, 63);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(523, 24);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Use maximum iteration number for MODFLOW during transit stress period:";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "10",
            "15",
            "20",
            "30",
            "40",
            "50",
            "100"});
            this.comboBox1.Location = new System.Drawing.Point(543, 62);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(93, 28);
            this.comboBox1.TabIndex = 10;
            this.comboBox1.Text = "10";
            // 
            // cmbmxsziter
            // 
            this.cmbmxsziter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbmxsziter.FormattingEnabled = true;
            this.cmbmxsziter.Items.AddRange(new object[] {
            "10",
            "15",
            "20",
            "30",
            "40",
            "50",
            "100"});
            this.cmbmxsziter.Location = new System.Drawing.Point(543, 26);
            this.cmbmxsziter.Name = "cmbmxsziter";
            this.cmbmxsziter.Size = new System.Drawing.Size(93, 28);
            this.cmbmxsziter.TabIndex = 10;
            this.cmbmxsziter.Text = "15";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.Location = new System.Drawing.Point(9, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(528, 20);
            this.label10.TabIndex = 9;
            this.label10.Text = "Maximum number of iterations soil-zone flow to MODFLOW at each time step:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioPETPM);
            this.groupBox5.Controls.Add(this.radioPETClimate);
            this.groupBox5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(11, 228);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(696, 100);
            this.groupBox5.TabIndex = 23;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Potential ET";
            // 
            // radioPETPM
            // 
            this.radioPETPM.AutoSize = true;
            this.radioPETPM.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioPETPM.Location = new System.Drawing.Point(13, 63);
            this.radioPETPM.Name = "radioPETPM";
            this.radioPETPM.Size = new System.Drawing.Size(240, 24);
            this.radioPETPM.TabIndex = 18;
            this.radioPETPM.Text = "Caculating ET using PM method";
            this.radioPETPM.UseVisualStyleBackColor = true;
            // 
            // radioPETClimate
            // 
            this.radioPETClimate.AutoSize = true;
            this.radioPETClimate.Checked = true;
            this.radioPETClimate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioPETClimate.Location = new System.Drawing.Point(13, 29);
            this.radioPETClimate.Name = "radioPETClimate";
            this.radioPETClimate.Size = new System.Drawing.Size(225, 24);
            this.radioPETClimate.TabIndex = 18;
            this.radioPETClimate.TabStop = true;
            this.radioPETClimate.Text = "Using an external file as input";
            this.radioPETClimate.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioSRCloud);
            this.groupBox4.Controls.Add(this.radioSRTemp);
            this.groupBox4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(11, 121);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(696, 100);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Solar Radiation Distribution";
            // 
            // radioSRCloud
            // 
            this.radioSRCloud.AutoSize = true;
            this.radioSRCloud.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioSRCloud.Location = new System.Drawing.Point(13, 60);
            this.radioSRCloud.Name = "radioSRCloud";
            this.radioSRCloud.Size = new System.Drawing.Size(513, 24);
            this.radioSRCloud.TabIndex = 18;
            this.radioSRCloud.Text = "Using a relation between solar radiation and cloud cover (ccsolrad_prms)";
            this.radioSRCloud.UseVisualStyleBackColor = true;
            // 
            // radioSRTemp
            // 
            this.radioSRTemp.AutoSize = true;
            this.radioSRTemp.Checked = true;
            this.radioSRTemp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioSRTemp.Location = new System.Drawing.Point(13, 26);
            this.radioSRTemp.Name = "radioSRTemp";
            this.radioSRTemp.Size = new System.Drawing.Size(536, 24);
            this.radioSRTemp.TabIndex = 18;
            this.radioSRTemp.TabStop = true;
            this.radioSRTemp.Text = "Using a maximum temperature per degree-day relation (ddsolrad_hru_prms)";
            this.radioSRTemp.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioRunoffNonLinear);
            this.groupBox3.Controls.Add(this.radioRunoffLinear);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(11, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(696, 100);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Surface runoff and infiltration";
            // 
            // radioRunoffNonLinear
            // 
            this.radioRunoffNonLinear.AutoSize = true;
            this.radioRunoffNonLinear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioRunoffNonLinear.Location = new System.Drawing.Point(13, 60);
            this.radioRunoffNonLinear.Name = "radioRunoffNonLinear";
            this.radioRunoffNonLinear.Size = new System.Drawing.Size(674, 24);
            this.radioRunoffNonLinear.TabIndex = 18;
            this.radioRunoffNonLinear.Text = "Using a non-linear variable-source-area method allowing for cascading flow (sruno" +
    "ff_smidx_casc)";
            this.radioRunoffNonLinear.UseVisualStyleBackColor = true;
            // 
            // radioRunoffLinear
            // 
            this.radioRunoffLinear.AutoSize = true;
            this.radioRunoffLinear.Checked = true;
            this.radioRunoffLinear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioRunoffLinear.Location = new System.Drawing.Point(13, 26);
            this.radioRunoffLinear.Name = "radioRunoffLinear";
            this.radioRunoffLinear.Size = new System.Drawing.Size(640, 24);
            this.radioRunoffLinear.TabIndex = 18;
            this.radioRunoffLinear.TabStop = true;
            this.radioRunoffLinear.Text = "Using a linear variable-source-area method allowing for cascading flow (srunoff_c" +
    "area_casc)";
            this.radioRunoffLinear.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 29);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(723, 524);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Extensions";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox7);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(723, 524);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Input/Output Files";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.checkSaveSMBudget);
            this.groupBox7.Controls.Add(this.checkPringDebug);
            this.groupBox7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.Location = new System.Drawing.Point(8, 315);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(706, 92);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Debug/Report File";
            // 
            // checkSaveSMBudget
            // 
            this.checkSaveSMBudget.AutoSize = true;
            this.checkSaveSMBudget.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkSaveSMBudget.Location = new System.Drawing.Point(247, 44);
            this.checkSaveSMBudget.Name = "checkSaveSMBudget";
            this.checkSaveSMBudget.Size = new System.Drawing.Size(177, 24);
            this.checkSaveSMBudget.TabIndex = 1;
            this.checkSaveSMBudget.Text = "Save soil zone budget";
            this.checkSaveSMBudget.UseVisualStyleBackColor = true;
            // 
            // checkPringDebug
            // 
            this.checkPringDebug.AutoSize = true;
            this.checkPringDebug.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkPringDebug.Location = new System.Drawing.Point(18, 44);
            this.checkPringDebug.Name = "checkPringDebug";
            this.checkPringDebug.Size = new System.Drawing.Size(192, 24);
            this.checkPringDebug.TabIndex = 1;
            this.checkPringDebug.Text = "Print surface flow debug";
            this.checkPringDebug.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.linkLabel1);
            this.groupBox2.Controls.Add(this.checkSM);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbSM);
            this.groupBox2.Controls.Add(this.btnSM);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(8, 155);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(706, 145);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output File";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(14, 34);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(243, 20);
            this.linkLabel1.TabIndex = 7;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Select animation output variables";
            // 
            // checkSM
            // 
            this.checkSM.AutoSize = true;
            this.checkSM.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkSM.Location = new System.Drawing.Point(18, 67);
            this.checkSM.Name = "checkSM";
            this.checkSM.Size = new System.Drawing.Size(191, 24);
            this.checkSM.TabIndex = 0;
            this.checkSM.Text = "Output soil moisture file";
            this.checkSM.UseVisualStyleBackColor = true;
            this.checkSM.CheckedChanged += new System.EventHandler(this.checkSM_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label8.Location = new System.Drawing.Point(42, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 20);
            this.label8.TabIndex = 6;
            this.label8.Text = "Soil moisture file:";
            // 
            // tbSM
            // 
            this.tbSM.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbSM.Location = new System.Drawing.Point(193, 100);
            this.tbSM.Name = "tbSM";
            this.tbSM.Size = new System.Drawing.Size(388, 27);
            this.tbSM.TabIndex = 1;
            // 
            // btnSM
            // 
            this.btnSM.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSM.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSM.Location = new System.Drawing.Point(599, 98);
            this.btnSM.Name = "btnSM";
            this.btnSM.Size = new System.Drawing.Size(100, 30);
            this.btnSM.TabIndex = 5;
            this.btnSM.Text = "Browse...";
            this.btnSM.UseVisualStyleBackColor = true;
            this.btnSM.Click += new System.EventHandler(this.btnSM_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbClimateFormat);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbMapFilename);
            this.groupBox1.Controls.Add(this.checkMappedClimate);
            this.groupBox1.Controls.Add(this.btnMapFilename);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(706, 143);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input Driving Force";
            // 
            // cmbClimateFormat
            // 
            this.cmbClimateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClimateFormat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbClimateFormat.FormattingEnabled = true;
            this.cmbClimateFormat.Items.AddRange(new object[] {
            "DCX",
            "TXT"});
            this.cmbClimateFormat.Location = new System.Drawing.Point(193, 27);
            this.cmbClimateFormat.Name = "cmbClimateFormat";
            this.cmbClimateFormat.Size = new System.Drawing.Size(183, 28);
            this.cmbClimateFormat.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.Location = new System.Drawing.Point(15, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(172, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Climate input file format";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.Location = new System.Drawing.Point(42, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(147, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "Spatial mapping file:";
            // 
            // tbMapFilename
            // 
            this.tbMapFilename.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbMapFilename.Location = new System.Drawing.Point(193, 99);
            this.tbMapFilename.Name = "tbMapFilename";
            this.tbMapFilename.Size = new System.Drawing.Size(388, 27);
            this.tbMapFilename.TabIndex = 1;
            // 
            // checkMappedClimate
            // 
            this.checkMappedClimate.AutoSize = true;
            this.checkMappedClimate.Checked = true;
            this.checkMappedClimate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkMappedClimate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkMappedClimate.Location = new System.Drawing.Point(18, 68);
            this.checkMappedClimate.Name = "checkMappedClimate";
            this.checkMappedClimate.Size = new System.Drawing.Size(256, 24);
            this.checkMappedClimate.TabIndex = 0;
            this.checkMappedClimate.Text = "Use mapped climate driving force";
            this.checkMappedClimate.UseVisualStyleBackColor = true;
            this.checkMappedClimate.CheckedChanged += new System.EventHandler(this.checkMappedClimate_CheckedChanged);
            // 
            // btnMapFilename
            // 
            this.btnMapFilename.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnMapFilename.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnMapFilename.Location = new System.Drawing.Point(599, 97);
            this.btnMapFilename.Name = "btnMapFilename";
            this.btnMapFilename.Size = new System.Drawing.Size(100, 30);
            this.btnMapFilename.TabIndex = 5;
            this.btnMapFilename.Text = "Browse...";
            this.btnMapFilename.UseVisualStyleBackColor = true;
            this.btnMapFilename.Click += new System.EventHandler(this.btnMapFilename_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.listVarDescriptions);
            this.tabPage2.Controls.Add(this.listVars);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(723, 524);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Output Variables";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(252, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 20);
            this.label4.TabIndex = 16;
            this.label4.Text = "Variable Description";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(8, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "Variable Name";
            // 
            // listVarDescriptions
            // 
            this.listVarDescriptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listVarDescriptions.FormattingEnabled = true;
            this.listVarDescriptions.ItemHeight = 20;
            this.listVarDescriptions.Items.AddRange(new object[] {
            "hru_actet"});
            this.listVarDescriptions.Location = new System.Drawing.Point(252, 35);
            this.listVarDescriptions.Name = "listVarDescriptions";
            this.listVarDescriptions.Size = new System.Drawing.Size(462, 464);
            this.listVarDescriptions.TabIndex = 1;
            // 
            // listVars
            // 
            this.listVars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listVars.CheckOnClick = true;
            this.listVars.FormattingEnabled = true;
            this.listVars.Items.AddRange(new object[] {
            "hru_actet",
            "soil_moist_frac",
            "infil_tot",
            "sroff"});
            this.listVars.Location = new System.Drawing.Point(8, 35);
            this.listVars.Name = "listVars";
            this.listVars.Size = new System.Drawing.Size(242, 466);
            this.listVars.TabIndex = 0;
            // 
            // GlobalOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(734, 606);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "GlobalOptionForm";
            this.Text = "Global Option";
            this.Load += new System.EventHandler(this.GlobalOptionForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RadioButton radioRunoffNonLinear;
        private System.Windows.Forms.RadioButton radioRunoffLinear;
        private System.Windows.Forms.RadioButton radioSRCloud;
        private System.Windows.Forms.RadioButton radioSRTemp;
        private System.Windows.Forms.CheckedListBox listVars;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listVarDescriptions;
        private System.Windows.Forms.RadioButton radioPETPM;
        private System.Windows.Forms.RadioButton radioPETClimate;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbMapFilename;
        private System.Windows.Forms.CheckBox checkMappedClimate;
        private System.Windows.Forms.Button btnMapFilename;
        private System.Windows.Forms.ComboBox cmbClimateFormat;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkSM;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbSM;
        private System.Windows.Forms.Button btnSM;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ComboBox cmbmxsziter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox checkSaveSMBudget;
        private System.Windows.Forms.CheckBox checkPringDebug;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}