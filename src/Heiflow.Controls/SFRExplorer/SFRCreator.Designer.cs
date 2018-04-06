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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbEleOffset = new System.Windows.Forms.TextBox();
            this.btnApplyEleOffset = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbState = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnApplyEleOffset);
            this.groupBox1.Controls.Add(this.tbEleOffset);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(583, 79);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stream Bed Elevation";
            // 
            // tbEleOffset
            // 
            this.tbEleOffset.Location = new System.Drawing.Point(274, 34);
            this.tbEleOffset.Name = "tbEleOffset";
            this.tbEleOffset.Size = new System.Drawing.Size(195, 23);
            this.tbEleOffset.TabIndex = 0;
            this.tbEleOffset.Text = "0.0";
            // 
            // btnApplyEleOffset
            // 
            this.btnApplyEleOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyEleOffset.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.btnApplyEleOffset.Location = new System.Drawing.Point(482, 29);
            this.btnApplyEleOffset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnApplyEleOffset.Name = "btnApplyEleOffset";
            this.btnApplyEleOffset.Size = new System.Drawing.Size(95, 30);
            this.btnApplyEleOffset.TabIndex = 14;
            this.btnApplyEleOffset.Text = "OK";
            this.btnApplyEleOffset.UseVisualStyleBackColor = true;
            this.btnApplyEleOffset.Click += new System.EventHandler(this.btnApplyEleOffset_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 15);
            this.label1.TabIndex = 15;
            this.label1.Text = "Elevation offset compared to grid top elevation";
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(18, 128);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(0, 15);
            this.lbState.TabIndex = 15;
            // 
            // SFRCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 152);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SFRCreator";
            this.Text = "SFR Creator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApplyEleOffset;
        private System.Windows.Forms.TextBox tbEleOffset;
        private System.Windows.Forms.Label lbState;

    }
}