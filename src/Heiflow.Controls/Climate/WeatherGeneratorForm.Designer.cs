namespace Heiflow.Controls.WinForm.Climate
{
    partial class WeatherGeneratorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeatherGeneratorForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbClimateFormat = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabFile = new System.Windows.Forms.TabPage();
            this.labelPressure = new System.Windows.Forms.Label();
            this.tbHumidity = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPressure = new System.Windows.Forms.TextBox();
            this.tbPpt = new System.Windows.Forms.TextBox();
            this.labelhum = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbWind = new System.Windows.Forms.TextBox();
            this.tbTMin = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPet = new System.Windows.Forms.TextBox();
            this.tbTMax = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabUniform = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tbPetval = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbMaxTval = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMinTval = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPptvalue = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabRandom = new System.Windows.Forms.TabPage();
            this.label24 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabFile.SuspendLayout();
            this.tabUniform.SuspendLayout();
            this.tabRandom.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbClimateFormat);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.cmbMethod);
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.dateTimePickerEnd);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(7, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(600, 104);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time";
            // 
            // cmbClimateFormat
            // 
            this.cmbClimateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClimateFormat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbClimateFormat.FormattingEnabled = true;
            this.cmbClimateFormat.Items.AddRange(new object[] {
            "Binary",
            "Text"});
            this.cmbClimateFormat.Location = new System.Drawing.Point(425, 66);
            this.cmbClimateFormat.Name = "cmbClimateFormat";
            this.cmbClimateFormat.Size = new System.Drawing.Size(160, 28);
            this.cmbClimateFormat.TabIndex = 17;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label22.Location = new System.Drawing.Point(330, 67);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(84, 20);
            this.label22.TabIndex = 16;
            this.label22.Text = "File format:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label23.Location = new System.Drawing.Point(350, 27);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(64, 20);
            this.label23.TabIndex = 16;
            this.label23.Text = "Method:";
            // 
            // cmbMethod
            // 
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Items.AddRange(new object[] {
            "Unifrom",
            "Random"});
            this.cmbMethod.Location = new System.Drawing.Point(425, 24);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(160, 28);
            this.cmbMethod.TabIndex = 17;
            this.cmbMethod.SelectedIndexChanged += new System.EventHandler(this.cmbMethod_SelectedIndexChanged);
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dateTimePickerStart.Location = new System.Drawing.Point(74, 24);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(210, 27);
            this.dateTimePickerStart.TabIndex = 15;
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dateTimePickerEnd.Location = new System.Drawing.Point(74, 66);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(210, 27);
            this.dateTimePickerEnd.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.Location = new System.Drawing.Point(22, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "End:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.Location = new System.Drawing.Point(22, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Start:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabFile);
            this.tabControl1.Controls.Add(this.tabUniform);
            this.tabControl1.Controls.Add(this.tabRandom);
            this.tabControl1.Location = new System.Drawing.Point(7, 112);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(600, 352);
            this.tabControl1.TabIndex = 28;
            // 
            // tabFile
            // 
            this.tabFile.Controls.Add(this.labelPressure);
            this.tabFile.Controls.Add(this.tbHumidity);
            this.tabFile.Controls.Add(this.label2);
            this.tabFile.Controls.Add(this.tbPressure);
            this.tabFile.Controls.Add(this.tbPpt);
            this.tabFile.Controls.Add(this.labelhum);
            this.tabFile.Controls.Add(this.label1);
            this.tabFile.Controls.Add(this.tbWind);
            this.tabFile.Controls.Add(this.tbTMin);
            this.tabFile.Controls.Add(this.label15);
            this.tabFile.Controls.Add(this.label3);
            this.tabFile.Controls.Add(this.tbPet);
            this.tabFile.Controls.Add(this.tbTMax);
            this.tabFile.Controls.Add(this.label6);
            this.tabFile.Location = new System.Drawing.Point(4, 30);
            this.tabFile.Name = "tabFile";
            this.tabFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabFile.Size = new System.Drawing.Size(592, 318);
            this.tabFile.TabIndex = 2;
            this.tabFile.Text = "Files";
            this.tabFile.UseVisualStyleBackColor = true;
            // 
            // labelPressure
            // 
            this.labelPressure.AutoSize = true;
            this.labelPressure.Location = new System.Drawing.Point(18, 235);
            this.labelPressure.Name = "labelPressure";
            this.labelPressure.Size = new System.Drawing.Size(159, 21);
            this.labelPressure.TabIndex = 0;
            this.labelPressure.Text = "Atmoshpere Pressure";
            // 
            // tbHumidity
            // 
            this.tbHumidity.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbHumidity.Location = new System.Drawing.Point(218, 197);
            this.tbHumidity.Name = "tbHumidity";
            this.tbHumidity.ReadOnly = true;
            this.tbHumidity.Size = new System.Drawing.Size(363, 29);
            this.tbHumidity.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "Precipitation";
            // 
            // tbPressure
            // 
            this.tbPressure.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbPressure.Location = new System.Drawing.Point(218, 232);
            this.tbPressure.Name = "tbPressure";
            this.tbPressure.ReadOnly = true;
            this.tbPressure.Size = new System.Drawing.Size(363, 29);
            this.tbPressure.TabIndex = 17;
            // 
            // tbPpt
            // 
            this.tbPpt.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbPpt.Location = new System.Drawing.Point(218, 24);
            this.tbPpt.Name = "tbPpt";
            this.tbPpt.ReadOnly = true;
            this.tbPpt.Size = new System.Drawing.Size(363, 29);
            this.tbPpt.TabIndex = 17;
            // 
            // labelhum
            // 
            this.labelhum.AutoSize = true;
            this.labelhum.Location = new System.Drawing.Point(18, 201);
            this.labelhum.Name = "labelhum";
            this.labelhum.Size = new System.Drawing.Size(74, 21);
            this.labelhum.TabIndex = 0;
            this.labelhum.Text = "Humidity";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minum Temperature";
            // 
            // tbWind
            // 
            this.tbWind.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbWind.Location = new System.Drawing.Point(218, 163);
            this.tbWind.Name = "tbWind";
            this.tbWind.ReadOnly = true;
            this.tbWind.Size = new System.Drawing.Size(363, 29);
            this.tbWind.TabIndex = 17;
            // 
            // tbTMin
            // 
            this.tbTMin.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbTMin.Location = new System.Drawing.Point(218, 59);
            this.tbTMin.Name = "tbTMin";
            this.tbTMin.ReadOnly = true;
            this.tbTMin.Size = new System.Drawing.Size(363, 29);
            this.tbTMin.TabIndex = 17;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 166);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(94, 21);
            this.label15.TabIndex = 0;
            this.label15.Text = "Wind Speed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Maximum Temperature";
            // 
            // tbPet
            // 
            this.tbPet.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbPet.Location = new System.Drawing.Point(218, 128);
            this.tbPet.Name = "tbPet";
            this.tbPet.ReadOnly = true;
            this.tbPet.Size = new System.Drawing.Size(363, 29);
            this.tbPet.TabIndex = 17;
            // 
            // tbTMax
            // 
            this.tbTMax.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbTMax.Location = new System.Drawing.Point(218, 93);
            this.tbTMax.Name = "tbTMax";
            this.tbTMax.ReadOnly = true;
            this.tbTMax.Size = new System.Drawing.Size(363, 29);
            this.tbTMax.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 21);
            this.label6.TabIndex = 0;
            this.label6.Text = "Potential ET";
            // 
            // tabUniform
            // 
            this.tabUniform.Controls.Add(this.label16);
            this.tabUniform.Controls.Add(this.label18);
            this.tabUniform.Controls.Add(this.label20);
            this.tabUniform.Controls.Add(this.label21);
            this.tabUniform.Controls.Add(this.label19);
            this.tabUniform.Controls.Add(this.label17);
            this.tabUniform.Controls.Add(this.label11);
            this.tabUniform.Controls.Add(this.label12);
            this.tabUniform.Controls.Add(this.label13);
            this.tabUniform.Controls.Add(this.label14);
            this.tabUniform.Controls.Add(this.textBox5);
            this.tabUniform.Controls.Add(this.textBox4);
            this.tabUniform.Controls.Add(this.textBox1);
            this.tabUniform.Controls.Add(this.tbPetval);
            this.tabUniform.Controls.Add(this.label7);
            this.tabUniform.Controls.Add(this.tbMaxTval);
            this.tabUniform.Controls.Add(this.label8);
            this.tabUniform.Controls.Add(this.tbMinTval);
            this.tabUniform.Controls.Add(this.label9);
            this.tabUniform.Controls.Add(this.tbPptvalue);
            this.tabUniform.Controls.Add(this.label10);
            this.tabUniform.Location = new System.Drawing.Point(4, 30);
            this.tabUniform.Name = "tabUniform";
            this.tabUniform.Padding = new System.Windows.Forms.Padding(3);
            this.tabUniform.Size = new System.Drawing.Size(592, 294);
            this.tabUniform.TabIndex = 1;
            this.tabUniform.Text = "Uniform";
            this.tabUniform.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(18, 222);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(159, 21);
            this.label16.TabIndex = 30;
            this.label16.Text = "Atmoshpere Pressure";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(18, 187);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(74, 21);
            this.label18.TabIndex = 31;
            this.label18.Text = "Humidity";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(18, 153);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(94, 21);
            this.label20.TabIndex = 32;
            this.label20.Text = "Wind Speed";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(397, 224);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(34, 21);
            this.label21.TabIndex = 26;
            this.label21.Text = "kPa";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(397, 189);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(86, 21);
            this.label19.TabIndex = 26;
            this.label19.Text = "Percentage";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(397, 155);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(37, 21);
            this.label17.TabIndex = 26;
            this.label17.Text = "m/s";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(397, 121);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 21);
            this.label11.TabIndex = 26;
            this.label11.Text = "Inch";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(397, 86);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(137, 21);
            this.label12.TabIndex = 27;
            this.label12.Text = "Fahrenheit Degree";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(397, 52);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(137, 21);
            this.label13.TabIndex = 28;
            this.label13.Text = "Fahrenheit Degree";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(397, 20);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 21);
            this.label14.TabIndex = 29;
            this.label14.Text = "Inch";
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox5.Location = new System.Drawing.Point(208, 218);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(182, 29);
            this.textBox5.TabIndex = 22;
            this.textBox5.Text = "101";
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox4.Location = new System.Drawing.Point(208, 184);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(182, 29);
            this.textBox4.TabIndex = 22;
            this.textBox4.Text = "0.7";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox1.Location = new System.Drawing.Point(208, 150);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(182, 29);
            this.textBox1.TabIndex = 22;
            this.textBox1.Text = "4.0";
            // 
            // tbPetval
            // 
            this.tbPetval.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbPetval.Location = new System.Drawing.Point(208, 115);
            this.tbPetval.Name = "tbPetval";
            this.tbPetval.Size = new System.Drawing.Size(182, 29);
            this.tbPetval.TabIndex = 22;
            this.tbPetval.Text = "0.1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 121);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 21);
            this.label7.TabIndex = 18;
            this.label7.Text = "Potential ET";
            // 
            // tbMaxTval
            // 
            this.tbMaxTval.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbMaxTval.Location = new System.Drawing.Point(208, 83);
            this.tbMaxTval.Name = "tbMaxTval";
            this.tbMaxTval.Size = new System.Drawing.Size(182, 29);
            this.tbMaxTval.TabIndex = 23;
            this.tbMaxTval.Text = "70";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(171, 21);
            this.label8.TabIndex = 19;
            this.label8.Text = "Maximum Temperature";
            // 
            // tbMinTval
            // 
            this.tbMinTval.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbMinTval.Location = new System.Drawing.Point(208, 50);
            this.tbMinTval.Name = "tbMinTval";
            this.tbMinTval.Size = new System.Drawing.Size(182, 29);
            this.tbMinTval.TabIndex = 24;
            this.tbMinTval.Text = "60";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(151, 21);
            this.label9.TabIndex = 20;
            this.label9.Text = "Minum Temperature";
            // 
            // tbPptvalue
            // 
            this.tbPptvalue.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbPptvalue.Location = new System.Drawing.Point(208, 17);
            this.tbPptvalue.Name = "tbPptvalue";
            this.tbPptvalue.Size = new System.Drawing.Size(182, 29);
            this.tbPptvalue.TabIndex = 25;
            this.tbPptvalue.Text = "0.15";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 21);
            this.label10.TabIndex = 21;
            this.label10.Text = "Precipitation";
            // 
            // tabRandom
            // 
            this.tabRandom.Controls.Add(this.label24);
            this.tabRandom.Location = new System.Drawing.Point(4, 30);
            this.tabRandom.Name = "tabRandom";
            this.tabRandom.Padding = new System.Windows.Forms.Padding(3);
            this.tabRandom.Size = new System.Drawing.Size(592, 294);
            this.tabRandom.TabIndex = 0;
            this.tabRandom.Text = "Random Method";
            this.tabRandom.UseVisualStyleBackColor = true;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.label24.Location = new System.Drawing.Point(22, 31);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(194, 37);
            this.label24.TabIndex = 0;
            this.label24.Text = "Coming soon...";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerate.BackColor = System.Drawing.SystemColors.Control;
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnGenerate.Location = new System.Drawing.Point(11, 471);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(0);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(146, 31);
            this.btnGenerate.TabIndex = 31;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOK.Location = new System.Drawing.Point(488, 471);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 31);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // WeatherGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 515);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WeatherGeneratorForm";
            this.Text = "Weather Generator";
            this.Load += new System.EventHandler(this.WeatherGeneratorForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabFile.ResumeLayout(false);
            this.tabFile.PerformLayout();
            this.tabUniform.ResumeLayout(false);
            this.tabUniform.PerformLayout();
            this.tabRandom.ResumeLayout(false);
            this.tabRandom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRandom;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.TextBox tbPet;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbTMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPpt;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabPage tabUniform;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbPetval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbMaxTval;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbMinTval;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbPptvalue;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelPressure;
        private System.Windows.Forms.TextBox tbHumidity;
        private System.Windows.Forms.TextBox tbPressure;
        private System.Windows.Forms.Label labelhum;
        private System.Windows.Forms.TextBox tbWind;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cmbClimateFormat;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TabPage tabFile;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox cmbMethod;
        private System.Windows.Forms.Label label24;
    }
}