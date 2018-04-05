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
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLanduse = new System.Windows.Forms.TabPage();
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbPetval = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbMaxTval = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMinTval = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPptvalue = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.dateTimePickerEnd);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(7, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(711, 63);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Time";
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Location = new System.Drawing.Point(64, 24);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(210, 25);
            this.dateTimePickerStart.TabIndex = 15;
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Location = new System.Drawing.Point(329, 24);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(210, 25);
            this.dateTimePickerEnd.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(294, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "End";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Start";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnCancel.Location = new System.Drawing.Point(602, 532);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 30);
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageLanduse);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(7, 241);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(711, 270);
            this.tabControl1.TabIndex = 28;
            // 
            // tabPageLanduse
            // 
            this.tabPageLanduse.Location = new System.Drawing.Point(4, 26);
            this.tabPageLanduse.Name = "tabPageLanduse";
            this.tabPageLanduse.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLanduse.Size = new System.Drawing.Size(703, 240);
            this.tabPageLanduse.TabIndex = 0;
            this.tabPageLanduse.Text = "Random Method";
            this.tabPageLanduse.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnGenerate.Location = new System.Drawing.Point(32, 532);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(103, 30);
            this.btnGenerate.TabIndex = 31;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbPet);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbTMax);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tbTMin);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbPpt);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(7, 72);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(835, 162);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Files";
            // 
            // tbPet
            // 
            this.tbPet.Location = new System.Drawing.Point(180, 124);
            this.tbPet.Name = "tbPet";
            this.tbPet.ReadOnly = true;
            this.tbPet.Size = new System.Drawing.Size(518, 25);
            this.tbPet.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 124);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Potential ET";
            // 
            // tbTMax
            // 
            this.tbTMax.Location = new System.Drawing.Point(180, 91);
            this.tbTMax.Name = "tbTMax";
            this.tbTMax.ReadOnly = true;
            this.tbTMax.Size = new System.Drawing.Size(518, 25);
            this.tbTMax.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Maximum Temperature";
            // 
            // tbTMin
            // 
            this.tbTMin.Location = new System.Drawing.Point(180, 58);
            this.tbTMin.Name = "tbTMin";
            this.tbTMin.ReadOnly = true;
            this.tbTMin.Size = new System.Drawing.Size(518, 25);
            this.tbTMin.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minum Temperature";
            // 
            // tbPpt
            // 
            this.tbPpt.Location = new System.Drawing.Point(180, 25);
            this.tbPpt.Name = "tbPpt";
            this.tbPpt.ReadOnly = true;
            this.tbPpt.Size = new System.Drawing.Size(518, 25);
            this.tbPpt.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Precipitation";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnOK.Location = new System.Drawing.Point(486, 532);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 30);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.tbPetval);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.tbMaxTval);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.tbMinTval);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.tbPptvalue);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(703, 240);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Uniform";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbPetval
            // 
            this.tbPetval.Location = new System.Drawing.Point(176, 116);
            this.tbPetval.Name = "tbPetval";
            this.tbPetval.ReadOnly = true;
            this.tbPetval.Size = new System.Drawing.Size(182, 25);
            this.tbPetval.TabIndex = 22;
            this.tbPetval.Text = "0.1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 116);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 17);
            this.label7.TabIndex = 18;
            this.label7.Text = "Potential ET";
            // 
            // tbMaxTval
            // 
            this.tbMaxTval.Location = new System.Drawing.Point(176, 83);
            this.tbMaxTval.Name = "tbMaxTval";
            this.tbMaxTval.ReadOnly = true;
            this.tbMaxTval.Size = new System.Drawing.Size(182, 25);
            this.tbMaxTval.TabIndex = 23;
            this.tbMaxTval.Text = "60";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 17);
            this.label8.TabIndex = 19;
            this.label8.Text = "Maximum Temperature";
            // 
            // tbMinTval
            // 
            this.tbMinTval.Location = new System.Drawing.Point(176, 50);
            this.tbMinTval.Name = "tbMinTval";
            this.tbMinTval.ReadOnly = true;
            this.tbMinTval.Size = new System.Drawing.Size(182, 25);
            this.tbMinTval.TabIndex = 24;
            this.tbMinTval.Text = "50";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(125, 17);
            this.label9.TabIndex = 20;
            this.label9.Text = "Minum Temperature";
            // 
            // tbPptvalue
            // 
            this.tbPptvalue.Location = new System.Drawing.Point(176, 17);
            this.tbPptvalue.Name = "tbPptvalue";
            this.tbPptvalue.ReadOnly = true;
            this.tbPptvalue.Size = new System.Drawing.Size(182, 25);
            this.tbPptvalue.TabIndex = 25;
            this.tbPptvalue.Text = "0.15";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 17);
            this.label10.TabIndex = 21;
            this.label10.Text = "Precipitation";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(379, 116);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 17);
            this.label11.TabIndex = 26;
            this.label11.Text = "Inch";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(379, 84);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(114, 17);
            this.label12.TabIndex = 27;
            this.label12.Text = "Fahrenheit Degree";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(379, 52);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(114, 17);
            this.label13.TabIndex = 28;
            this.label13.Text = "Fahrenheit Degree";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(379, 20);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 17);
            this.label14.TabIndex = 29;
            this.label14.Text = "Inch";
            // 
            // WeatherGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(723, 575);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WeatherGeneratorForm";
            this.Text = "Weather Generator";
            this.Load += new System.EventHandler(this.WeatherGeneratorForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
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
    }
}