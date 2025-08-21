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
            this.tabModule = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cmbmxsziter = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioPETPM = new System.Windows.Forms.RadioButton();
            this.radioPETClimate = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioSRClimate = new System.Windows.Forms.RadioButton();
            this.radioSRTemp = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioRunoffNonLinear = new System.Windows.Forms.RadioButton();
            this.radioRunoffLinear = new System.Windows.Forms.RadioButton();
            this.tabFile = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.checkPringDebug = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkSaveSMBudget = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkSM = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbClimateFormat = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbMapFilename = new System.Windows.Forms.TextBox();
            this.checkMappedClimate = new System.Windows.Forms.CheckBox();
            this.btnCreateMapFile = new System.Windows.Forms.Button();
            this.tabOutVars = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listVars = new System.Windows.Forms.CheckedListBox();
            this.tabExtension = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.btnUpdateSfrunoff = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabModule.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabFile.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabOutVars.SuspendLayout();
            this.tabExtension.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(855, 751);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 42);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(690, 751);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(140, 42);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabModule);
            this.tabControl1.Controls.Add(this.tabFile);
            this.tabControl1.Controls.Add(this.tabOutVars);
            this.tabControl1.Controls.Add(this.tabExtension);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1023, 743);
            this.tabControl1.TabIndex = 6;
            // 
            // tabModule
            // 
            this.tabModule.Controls.Add(this.groupBox6);
            this.tabModule.Controls.Add(this.groupBox5);
            this.tabModule.Controls.Add(this.groupBox4);
            this.tabModule.Controls.Add(this.groupBox3);
            this.tabModule.Location = new System.Drawing.Point(4, 39);
            this.tabModule.Margin = new System.Windows.Forms.Padding(4);
            this.tabModule.Name = "tabModule";
            this.tabModule.Padding = new System.Windows.Forms.Padding(4);
            this.tabModule.Size = new System.Drawing.Size(1015, 700);
            this.tabModule.TabIndex = 0;
            this.tabModule.Text = "Modules";
            this.tabModule.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBox1);
            this.groupBox6.Controls.Add(this.comboBox1);
            this.groupBox6.Controls.Add(this.cmbmxsziter);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(17, 533);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox6.Size = new System.Drawing.Size(974, 141);
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
            this.checkBox1.Location = new System.Drawing.Point(18, 88);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(732, 34);
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
            this.comboBox1.Location = new System.Drawing.Point(760, 87);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(129, 38);
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
            this.cmbmxsziter.Location = new System.Drawing.Point(760, 36);
            this.cmbmxsziter.Margin = new System.Windows.Forms.Padding(4);
            this.cmbmxsziter.Name = "cmbmxsziter";
            this.cmbmxsziter.Size = new System.Drawing.Size(129, 38);
            this.cmbmxsziter.TabIndex = 10;
            this.cmbmxsziter.Text = "15";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.Location = new System.Drawing.Point(13, 39);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(742, 30);
            this.label10.TabIndex = 9;
            this.label10.Text = "Maximum number of iterations soil-zone flow to MODFLOW at each time step:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioPETPM);
            this.groupBox5.Controls.Add(this.radioPETClimate);
            this.groupBox5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(17, 379);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(974, 140);
            this.groupBox5.TabIndex = 23;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Potential ET";
            // 
            // radioPETPM
            // 
            this.radioPETPM.AutoSize = true;
            this.radioPETPM.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioPETPM.Location = new System.Drawing.Point(18, 88);
            this.radioPETPM.Margin = new System.Windows.Forms.Padding(4);
            this.radioPETPM.Name = "radioPETPM";
            this.radioPETPM.Size = new System.Drawing.Size(335, 34);
            this.radioPETPM.TabIndex = 18;
            this.radioPETPM.Text = "Caculating ET using PM method";
            this.radioPETPM.UseVisualStyleBackColor = true;
            // 
            // radioPETClimate
            // 
            this.radioPETClimate.AutoSize = true;
            this.radioPETClimate.Checked = true;
            this.radioPETClimate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioPETClimate.Location = new System.Drawing.Point(18, 41);
            this.radioPETClimate.Margin = new System.Windows.Forms.Padding(4);
            this.radioPETClimate.Name = "radioPETClimate";
            this.radioPETClimate.Size = new System.Drawing.Size(313, 34);
            this.radioPETClimate.TabIndex = 18;
            this.radioPETClimate.TabStop = true;
            this.radioPETClimate.Text = "Using an external file as input";
            this.radioPETClimate.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioSRClimate);
            this.groupBox4.Controls.Add(this.radioSRTemp);
            this.groupBox4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(17, 225);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(974, 140);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Solar Radiation Distribution";
            // 
            // radioSRClimate
            // 
            this.radioSRClimate.AutoSize = true;
            this.radioSRClimate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioSRClimate.Location = new System.Drawing.Point(18, 84);
            this.radioSRClimate.Margin = new System.Windows.Forms.Padding(4);
            this.radioSRClimate.Name = "radioSRClimate";
            this.radioSRClimate.Size = new System.Drawing.Size(223, 34);
            this.radioSRClimate.TabIndex = 18;
            this.radioSRClimate.Text = "Using climate input ";
            this.radioSRClimate.UseVisualStyleBackColor = true;
            // 
            // radioSRTemp
            // 
            this.radioSRTemp.AutoSize = true;
            this.radioSRTemp.Checked = true;
            this.radioSRTemp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioSRTemp.Location = new System.Drawing.Point(18, 36);
            this.radioSRTemp.Margin = new System.Windows.Forms.Padding(4);
            this.radioSRTemp.Name = "radioSRTemp";
            this.radioSRTemp.Size = new System.Drawing.Size(548, 34);
            this.radioSRTemp.TabIndex = 18;
            this.radioSRTemp.TabStop = true;
            this.radioSRTemp.Text = "Using a maximum temperature per degree-day relation";
            this.radioSRTemp.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioRunoffNonLinear);
            this.groupBox3.Controls.Add(this.radioRunoffLinear);
            this.groupBox3.Controls.Add(this.btnUpdateSfrunoff);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(17, 17);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(974, 194);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Surface runoff and infiltration";
            // 
            // radioRunoffNonLinear
            // 
            this.radioRunoffNonLinear.AutoSize = true;
            this.radioRunoffNonLinear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioRunoffNonLinear.Location = new System.Drawing.Point(18, 84);
            this.radioRunoffNonLinear.Margin = new System.Windows.Forms.Padding(4);
            this.radioRunoffNonLinear.Name = "radioRunoffNonLinear";
            this.radioRunoffNonLinear.Size = new System.Drawing.Size(935, 34);
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
            this.radioRunoffLinear.Location = new System.Drawing.Point(18, 36);
            this.radioRunoffLinear.Margin = new System.Windows.Forms.Padding(4);
            this.radioRunoffLinear.Name = "radioRunoffLinear";
            this.radioRunoffLinear.Size = new System.Drawing.Size(887, 34);
            this.radioRunoffLinear.TabIndex = 18;
            this.radioRunoffLinear.TabStop = true;
            this.radioRunoffLinear.Text = "Using a linear variable-source-area method allowing for cascading flow (srunoff_c" +
    "area_casc)";
            this.radioRunoffLinear.UseVisualStyleBackColor = true;
            // 
            // tabFile
            // 
            this.tabFile.Controls.Add(this.groupBox7);
            this.tabFile.Controls.Add(this.groupBox2);
            this.tabFile.Controls.Add(this.groupBox1);
            this.tabFile.Location = new System.Drawing.Point(4, 39);
            this.tabFile.Margin = new System.Windows.Forms.Padding(4);
            this.tabFile.Name = "tabFile";
            this.tabFile.Padding = new System.Windows.Forms.Padding(4);
            this.tabFile.Size = new System.Drawing.Size(1015, 700);
            this.tabFile.TabIndex = 3;
            this.tabFile.Text = "Input/Output Files";
            this.tabFile.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.checkPringDebug);
            this.groupBox7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.Location = new System.Drawing.Point(11, 403);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox7.Size = new System.Drawing.Size(988, 129);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Debug File";
            // 
            // checkPringDebug
            // 
            this.checkPringDebug.AutoSize = true;
            this.checkPringDebug.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkPringDebug.Location = new System.Drawing.Point(25, 62);
            this.checkPringDebug.Margin = new System.Windows.Forms.Padding(4);
            this.checkPringDebug.Name = "checkPringDebug";
            this.checkPringDebug.Size = new System.Drawing.Size(265, 34);
            this.checkPringDebug.TabIndex = 1;
            this.checkPringDebug.Text = "Print surface flow debug";
            this.checkPringDebug.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkSaveSMBudget);
            this.groupBox2.Controls.Add(this.linkLabel1);
            this.groupBox2.Controls.Add(this.checkSM);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(11, 217);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(988, 178);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output File";
            // 
            // checkSaveSMBudget
            // 
            this.checkSaveSMBudget.AutoSize = true;
            this.checkSaveSMBudget.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkSaveSMBudget.Location = new System.Drawing.Point(357, 109);
            this.checkSaveSMBudget.Margin = new System.Windows.Forms.Padding(4);
            this.checkSaveSMBudget.Name = "checkSaveSMBudget";
            this.checkSaveSMBudget.Size = new System.Drawing.Size(242, 34);
            this.checkSaveSMBudget.TabIndex = 1;
            this.checkSaveSMBudget.Text = "Save soil zone budget";
            this.checkSaveSMBudget.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(20, 48);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(343, 30);
            this.linkLabel1.TabIndex = 7;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Select animation output variables";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // checkSM
            // 
            this.checkSM.AutoSize = true;
            this.checkSM.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkSM.Location = new System.Drawing.Point(25, 109);
            this.checkSM.Margin = new System.Windows.Forms.Padding(4);
            this.checkSM.Name = "checkSM";
            this.checkSM.Size = new System.Drawing.Size(240, 34);
            this.checkSM.TabIndex = 0;
            this.checkSM.Text = "Save soil moisture file";
            this.checkSM.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbClimateFormat);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbMapFilename);
            this.groupBox1.Controls.Add(this.checkMappedClimate);
            this.groupBox1.Controls.Add(this.btnCreateMapFile);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(11, 8);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(988, 200);
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
            "Binary",
            "Text"});
            this.cmbClimateFormat.Location = new System.Drawing.Point(270, 38);
            this.cmbClimateFormat.Margin = new System.Windows.Forms.Padding(4);
            this.cmbClimateFormat.Name = "cmbClimateFormat";
            this.cmbClimateFormat.Size = new System.Drawing.Size(255, 38);
            this.cmbClimateFormat.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.Location = new System.Drawing.Point(21, 42);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(239, 30);
            this.label7.TabIndex = 6;
            this.label7.Text = "Climate input file format";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.Location = new System.Drawing.Point(59, 144);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(202, 30);
            this.label6.TabIndex = 6;
            this.label6.Text = "Spatial mapping file:";
            // 
            // tbMapFilename
            // 
            this.tbMapFilename.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbMapFilename.Location = new System.Drawing.Point(270, 139);
            this.tbMapFilename.Margin = new System.Windows.Forms.Padding(4);
            this.tbMapFilename.Name = "tbMapFilename";
            this.tbMapFilename.Size = new System.Drawing.Size(542, 35);
            this.tbMapFilename.TabIndex = 1;
            // 
            // checkMappedClimate
            // 
            this.checkMappedClimate.AutoSize = true;
            this.checkMappedClimate.Checked = true;
            this.checkMappedClimate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkMappedClimate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkMappedClimate.Location = new System.Drawing.Point(25, 95);
            this.checkMappedClimate.Margin = new System.Windows.Forms.Padding(4);
            this.checkMappedClimate.Name = "checkMappedClimate";
            this.checkMappedClimate.Size = new System.Drawing.Size(350, 34);
            this.checkMappedClimate.TabIndex = 0;
            this.checkMappedClimate.Text = "Use mapped climate driving force";
            this.checkMappedClimate.UseVisualStyleBackColor = true;
            this.checkMappedClimate.CheckedChanged += new System.EventHandler(this.checkMappedClimate_CheckedChanged);
            // 
            // btnCreateMapFile
            // 
            this.btnCreateMapFile.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCreateMapFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCreateMapFile.Location = new System.Drawing.Point(839, 136);
            this.btnCreateMapFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateMapFile.Name = "btnCreateMapFile";
            this.btnCreateMapFile.Size = new System.Drawing.Size(140, 42);
            this.btnCreateMapFile.TabIndex = 5;
            this.btnCreateMapFile.Text = "Create";
            this.btnCreateMapFile.UseVisualStyleBackColor = true;
            this.btnCreateMapFile.Click += new System.EventHandler(this.btnCreateMapFilename_Click);
            // 
            // tabOutVars
            // 
            this.tabOutVars.Controls.Add(this.textBox1);
            this.tabOutVars.Controls.Add(this.label4);
            this.tabOutVars.Controls.Add(this.label2);
            this.tabOutVars.Controls.Add(this.listVars);
            this.tabOutVars.Location = new System.Drawing.Point(4, 39);
            this.tabOutVars.Margin = new System.Windows.Forms.Padding(4);
            this.tabOutVars.Name = "tabOutVars";
            this.tabOutVars.Padding = new System.Windows.Forms.Padding(4);
            this.tabOutVars.Size = new System.Drawing.Size(1015, 700);
            this.tabOutVars.TabIndex = 1;
            this.tabOutVars.Text = "Output Variables";
            this.tabOutVars.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(360, 49);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(638, 651);
            this.textBox1.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(353, 13);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(209, 30);
            this.label4.TabIndex = 16;
            this.label4.Text = "Variable Description";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(11, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 30);
            this.label2.TabIndex = 16;
            this.label2.Text = "Variable Name";
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
            this.listVars.Location = new System.Drawing.Point(11, 49);
            this.listVars.Margin = new System.Windows.Forms.Padding(4);
            this.listVars.Name = "listVars";
            this.listVars.Size = new System.Drawing.Size(337, 604);
            this.listVars.TabIndex = 0;
            // 
            // tabExtension
            // 
            this.tabExtension.Controls.Add(this.propertyGrid1);
            this.tabExtension.Location = new System.Drawing.Point(4, 39);
            this.tabExtension.Margin = new System.Windows.Forms.Padding(4);
            this.tabExtension.Name = "tabExtension";
            this.tabExtension.Padding = new System.Windows.Forms.Padding(4);
            this.tabExtension.Size = new System.Drawing.Size(1015, 700);
            this.tabExtension.TabIndex = 4;
            this.tabExtension.Text = "Extensions";
            this.tabExtension.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(4, 4);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(4);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(1007, 692);
            this.propertyGrid1.TabIndex = 0;
            // 
            // btnUpdateSfrunoff
            // 
            this.btnUpdateSfrunoff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateSfrunoff.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUpdateSfrunoff.Location = new System.Drawing.Point(43, 135);
            this.btnUpdateSfrunoff.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdateSfrunoff.Name = "btnUpdateSfrunoff";
            this.btnUpdateSfrunoff.Size = new System.Drawing.Size(339, 42);
            this.btnUpdateSfrunoff.TabIndex = 5;
            this.btnUpdateSfrunoff.Text = "Update sfrunoff_module";
            this.btnUpdateSfrunoff.UseVisualStyleBackColor = true;
            this.btnUpdateSfrunoff.Click += new System.EventHandler(this.btnUpdateSfrunoff_Click);
            // 
            // GlobalOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1028, 804);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "GlobalOptionForm";
            this.ShowInTaskbar = false;
            this.Text = "Global Option";
            this.Load += new System.EventHandler(this.GlobalOptionForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabModule.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabFile.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabOutVars.ResumeLayout(false);
            this.tabOutVars.PerformLayout();
            this.tabExtension.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabModule;
        private System.Windows.Forms.TabPage tabOutVars;
        private System.Windows.Forms.RadioButton radioRunoffNonLinear;
        private System.Windows.Forms.RadioButton radioRunoffLinear;
        private System.Windows.Forms.RadioButton radioSRClimate;
        private System.Windows.Forms.RadioButton radioSRTemp;
        private System.Windows.Forms.CheckedListBox listVars;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioPETPM;
        private System.Windows.Forms.RadioButton radioPETClimate;
        private System.Windows.Forms.TabPage tabFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbMapFilename;
        private System.Windows.Forms.CheckBox checkMappedClimate;
        private System.Windows.Forms.Button btnCreateMapFile;
        private System.Windows.Forms.ComboBox cmbClimateFormat;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkSM;
        private System.Windows.Forms.TabPage tabExtension;
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
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnUpdateSfrunoff;
    }
}