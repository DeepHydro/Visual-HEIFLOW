namespace Heiflow.Controls.WinForm.SFRExplorer
{
    partial class SFRCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SFRCreator));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlLeft = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupPlotProp = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPropertyName = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbEndID = new System.Windows.Forms.ComboBox();
            this.cmbStartID = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPageProfile = new System.Windows.Forms.TabPage();
            this.tabControl_Chart = new System.Windows.Forms.TabControl();
            this.winChart_proflie = new Heiflow.Controls.WinForm.Controls.WinChart();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlLeft.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupPlotProp.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPageProfile.SuspendLayout();
            this.tabControl_Chart.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControlLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl_Chart);
            this.splitContainer1.Size = new System.Drawing.Size(1313, 606);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControlLeft
            // 
            this.tabControlLeft.Controls.Add(this.tabPage4);
            this.tabControlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLeft.Location = new System.Drawing.Point(0, 0);
            this.tabControlLeft.Name = "tabControlLeft";
            this.tabControlLeft.SelectedIndex = 0;
            this.tabControlLeft.Size = new System.Drawing.Size(271, 606);
            this.tabControlLeft.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupPlotProp);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(263, 574);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Profile";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupPlotProp
            // 
            this.groupPlotProp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPlotProp.Controls.Add(this.label2);
            this.groupPlotProp.Controls.Add(this.cmbPropertyName);
            this.groupPlotProp.Location = new System.Drawing.Point(9, 172);
            this.groupPlotProp.Name = "groupPlotProp";
            this.groupPlotProp.Size = new System.Drawing.Size(248, 103);
            this.groupPlotProp.TabIndex = 15;
            this.groupPlotProp.TabStop = false;
            this.groupPlotProp.Text = "Plot Reach Properties";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Select property name";
            // 
            // cmbPropertyName
            // 
            this.cmbPropertyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPropertyName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPropertyName.FormattingEnabled = true;
            this.cmbPropertyName.Items.AddRange(new object[] {
            "TopElevation",
            "Slope",
            "Length",
            "Width"});
            this.cmbPropertyName.Location = new System.Drawing.Point(6, 55);
            this.cmbPropertyName.Name = "cmbPropertyName";
            this.cmbPropertyName.Size = new System.Drawing.Size(221, 27);
            this.cmbPropertyName.TabIndex = 0;
            this.cmbPropertyName.SelectedIndexChanged += new System.EventHandler(this.cmbPropertyName_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cmbEndID);
            this.groupBox2.Controls.Add(this.cmbStartID);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(6, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 163);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Setting Profile ";
            // 
            // cmbEndID
            // 
            this.cmbEndID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEndID.FormattingEnabled = true;
            this.cmbEndID.Location = new System.Drawing.Point(10, 110);
            this.cmbEndID.Name = "cmbEndID";
            this.cmbEndID.Size = new System.Drawing.Size(220, 27);
            this.cmbEndID.TabIndex = 4;
            // 
            // cmbStartID
            // 
            this.cmbStartID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbStartID.FormattingEnabled = true;
            this.cmbStartID.Location = new System.Drawing.Point(10, 50);
            this.cmbStartID.Name = "cmbStartID";
            this.cmbStartID.Size = new System.Drawing.Size(220, 27);
            this.cmbStartID.TabIndex = 5;
            this.cmbStartID.SelectedIndexChanged += new System.EventHandler(this.cmbStartID_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 19);
            this.label6.TabIndex = 2;
            this.label6.Text = "End Segment ID";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 19);
            this.label7.TabIndex = 3;
            this.label7.Text = "Start Segment ID";
            // 
            // tabPageProfile
            // 
            this.tabPageProfile.Controls.Add(this.winChart_proflie);
            this.tabPageProfile.Location = new System.Drawing.Point(4, 4);
            this.tabPageProfile.Name = "tabPageProfile";
            this.tabPageProfile.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile.Size = new System.Drawing.Size(1031, 574);
            this.tabPageProfile.TabIndex = 0;
            this.tabPageProfile.Text = "Profile View";
            this.tabPageProfile.UseVisualStyleBackColor = true;
            // 
            // tabControl_Chart
            // 
            this.tabControl_Chart.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl_Chart.Controls.Add(this.tabPageProfile);
            this.tabControl_Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Chart.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Chart.Name = "tabControl_Chart";
            this.tabControl_Chart.SelectedIndex = 0;
            this.tabControl_Chart.Size = new System.Drawing.Size(1039, 606);
            this.tabControl_Chart.TabIndex = 6;
            // 
            // winChart_proflie
            // 
            this.winChart_proflie.BackColor = System.Drawing.SystemColors.Control;
            this.winChart_proflie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_proflie.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart_proflie.Location = new System.Drawing.Point(3, 3);
            this.winChart_proflie.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChart_proflie.Name = "winChart_proflie";
            this.winChart_proflie.ShowStatPanel = true;
            this.winChart_proflie.Size = new System.Drawing.Size(1025, 568);
            this.winChart_proflie.TabIndex = 7;
            // 
            // SFRCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 606);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SFRCreator";
            this.Text = "SFR Viewer";
            this.Load += new System.EventHandler(this.SFRCreator_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlLeft.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupPlotProp.ResumeLayout(false);
            this.groupPlotProp.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPageProfile.ResumeLayout(false);
            this.tabControl_Chart.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlLeft;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupPlotProp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbPropertyName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbEndID;
        private System.Windows.Forms.ComboBox cmbStartID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabControl tabControl_Chart;
        private System.Windows.Forms.TabPage tabPageProfile;
        private Controls.WinChart winChart_proflie;


    }
}