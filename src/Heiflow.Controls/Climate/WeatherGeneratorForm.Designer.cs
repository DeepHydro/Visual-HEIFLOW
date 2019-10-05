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
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLanduse = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tbPetval = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbMaxTval = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMinTval = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPptvalue = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbPet = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbTMax = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTMin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPpt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.tbWind = new System.Windows.Forms.TextBox();
            this.tbHumidity = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tbPressure = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.dateTimePickerEnd);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(6, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(632, 60);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Time";
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Location = new System.Drawing.Point(66, 23);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(187, 27);
            this.dateTimePickerStart.TabIndex = 15;
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Location = new System.Drawing.Point(320, 23);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(187, 27);
            this.dateTimePickerEnd.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(280, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "End";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Start";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageLanduse);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(6, 334);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(637, 279);
            this.tabControl1.TabIndex = 28;
            // 
            // tabPageLanduse
            // 
            this.tabPageLanduse.Location = new System.Drawing.Point(4, 29);
            this.tabPageLanduse.Name = "tabPageLanduse";
            this.tabPageLanduse.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLanduse.Size = new System.Drawing.Size(629, 246);
            this.tabPageLanduse.TabIndex = 0;
            this.tabPageLanduse.Text = "Random Method";
            this.tabPageLanduse.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.label21);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.textBox5);
            this.tabPage1.Controls.Add(this.textBox4);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.tbPetval);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.tbMaxTval);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.tbMinTval);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.tbPptvalue);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(629, 246);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Uniform";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(353, 115);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 20);
            this.label11.TabIndex = 26;
            this.label11.Text = "Inch";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(353, 82);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(130, 20);
            this.label12.TabIndex = 27;
            this.label12.Text = "Fahrenheit Degree";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(353, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(130, 20);
            this.label13.TabIndex = 28;
            this.label13.Text = "Fahrenheit Degree";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(353, 19);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 20);
            this.label14.TabIndex = 29;
            this.label14.Text = "Inch";
            // 
            // tbPetval
            // 
            this.tbPetval.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbPetval.Location = new System.Drawing.Point(185, 110);
            this.tbPetval.Name = "tbPetval";
            this.tbPetval.ReadOnly = true;
            this.tbPetval.Size = new System.Drawing.Size(162, 27);
            this.tbPetval.TabIndex = 22;
            this.tbPetval.Text = "0.1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 20);
            this.label7.TabIndex = 18;
            this.label7.Text = "Potential ET";
            // 
            // tbMaxTval
            // 
            this.tbMaxTval.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbMaxTval.Location = new System.Drawing.Point(185, 79);
            this.tbMaxTval.Name = "tbMaxTval";
            this.tbMaxTval.ReadOnly = true;
            this.tbMaxTval.Size = new System.Drawing.Size(162, 27);
            this.tbMaxTval.TabIndex = 23;
            this.tbMaxTval.Text = "60";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(163, 20);
            this.label8.TabIndex = 19;
            this.label8.Text = "Maximum Temperature";
            // 
            // tbMinTval
            // 
            this.tbMinTval.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbMinTval.Location = new System.Drawing.Point(185, 48);
            this.tbMinTval.Name = "tbMinTval";
            this.tbMinTval.ReadOnly = true;
            this.tbMinTval.Size = new System.Drawing.Size(162, 27);
            this.tbMinTval.TabIndex = 24;
            this.tbMinTval.Text = "50";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 51);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 20);
            this.label9.TabIndex = 20;
            this.label9.Text = "Minum Temperature";
            // 
            // tbPptvalue
            // 
            this.tbPptvalue.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbPptvalue.Location = new System.Drawing.Point(185, 16);
            this.tbPptvalue.Name = "tbPptvalue";
            this.tbPptvalue.ReadOnly = true;
            this.tbPptvalue.Size = new System.Drawing.Size(162, 27);
            this.tbPptvalue.TabIndex = 25;
            this.tbPptvalue.Text = "0.15";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 20);
            this.label10.TabIndex = 21;
            this.label10.Text = "Precipitation";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerate.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnGenerate.Location = new System.Drawing.Point(10, 621);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(0);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(130, 30);
            this.btnGenerate.TabIndex = 31;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbPressure);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.tbHumidity);
            this.groupBox2.Controls.Add(this.tbWind);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.tbPet);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbTMax);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tbTMin);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbPpt);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(6, 63);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(632, 264);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Files";
            // 
            // tbPet
            // 
            this.tbPet.Location = new System.Drawing.Point(189, 123);
            this.tbPet.Name = "tbPet";
            this.tbPet.ReadOnly = true;
            this.tbPet.Size = new System.Drawing.Size(432, 27);
            this.tbPet.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Potential ET";
            // 
            // tbTMax
            // 
            this.tbTMax.Location = new System.Drawing.Point(189, 90);
            this.tbTMax.Name = "tbTMax";
            this.tbTMax.ReadOnly = true;
            this.tbTMax.Size = new System.Drawing.Size(432, 27);
            this.tbTMax.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Maximum Temperature";
            // 
            // tbTMin
            // 
            this.tbTMin.Location = new System.Drawing.Point(189, 57);
            this.tbTMin.Name = "tbTMin";
            this.tbTMin.ReadOnly = true;
            this.tbTMin.Size = new System.Drawing.Size(432, 27);
            this.tbTMin.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minum Temperature";
            // 
            // tbPpt
            // 
            this.tbPpt.Location = new System.Drawing.Point(189, 24);
            this.tbPpt.Name = "tbPpt";
            this.tbPpt.ReadOnly = true;
            this.tbPpt.Size = new System.Drawing.Size(432, 27);
            this.tbPpt.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Precipitation";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOK.Location = new System.Drawing.Point(539, 622);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(20, 159);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(90, 20);
            this.label15.TabIndex = 0;
            this.label15.Text = "Wind Speed";
            // 
            // tbWind
            // 
            this.tbWind.Location = new System.Drawing.Point(189, 156);
            this.tbWind.Name = "tbWind";
            this.tbWind.ReadOnly = true;
            this.tbWind.Size = new System.Drawing.Size(432, 27);
            this.tbWind.TabIndex = 17;
            // 
            // tbHumidity
            // 
            this.tbHumidity.AutoSize = true;
            this.tbHumidity.Location = new System.Drawing.Point(20, 192);
            this.tbHumidity.Name = "tbHumidity";
            this.tbHumidity.Size = new System.Drawing.Size(70, 20);
            this.tbHumidity.TabIndex = 0;
            this.tbHumidity.Text = "Humidity";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(189, 222);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(432, 27);
            this.textBox2.TabIndex = 17;
            // 
            // tbPressure
            // 
            this.tbPressure.AutoSize = true;
            this.tbPressure.Location = new System.Drawing.Point(20, 225);
            this.tbPressure.Name = "tbPressure";
            this.tbPressure.Size = new System.Drawing.Size(148, 20);
            this.tbPressure.TabIndex = 0;
            this.tbPressure.Text = "Atmoshpere Pressure";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(189, 189);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(432, 27);
            this.textBox3.TabIndex = 17;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox1.Location = new System.Drawing.Point(185, 143);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(162, 27);
            this.textBox1.TabIndex = 22;
            this.textBox1.Text = "4.0";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(353, 148);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(34, 20);
            this.label17.TabIndex = 26;
            this.label17.Text = "m/s";
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox4.Location = new System.Drawing.Point(185, 175);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(162, 27);
            this.textBox4.TabIndex = 22;
            this.textBox4.Text = "0.7";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(353, 180);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 20);
            this.label19.TabIndex = 26;
            this.label19.Text = "Percentage";
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox5.Location = new System.Drawing.Point(185, 208);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(162, 27);
            this.textBox5.TabIndex = 22;
            this.textBox5.Text = "101";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(353, 213);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(31, 20);
            this.label21.TabIndex = 26;
            this.label21.Text = "kPa";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 211);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(148, 20);
            this.label16.TabIndex = 30;
            this.label16.Text = "Atmoshpere Pressure";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(16, 178);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(70, 20);
            this.label18.TabIndex = 31;
            this.label18.Text = "Humidity";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(16, 146);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(90, 20);
            this.label20.TabIndex = 32;
            this.label20.Text = "Wind Speed";
            // 
            // WeatherGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 665);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WeatherGeneratorForm";
            this.Text = "Weather Generator";
            this.Load += new System.EventHandler(this.WeatherGeneratorForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLanduse;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.GroupBox groupBox2;
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
        private System.Windows.Forms.TabPage tabPage1;
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
        private System.Windows.Forms.Label tbPressure;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label tbHumidity;
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
    }
}