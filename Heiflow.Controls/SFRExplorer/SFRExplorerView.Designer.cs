namespace Heiflow.Controls.WinForm.SFRExplorer
{
    partial class SFRExplorerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SFRExplorerView));
            this.sfrExplorer1 = new Heiflow.Controls.WinForm.SFRExplorer.SFRExplorer();
            this.SuspendLayout();
            // 
            // sfrExplorer1
            // 
            this.sfrExplorer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sfrExplorer1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.sfrExplorer1.Location = new System.Drawing.Point(0, 0);
            this.sfrExplorer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.sfrExplorer1.Name = "sfrExplorer1";
            this.sfrExplorer1.ODM = null;
            this.sfrExplorer1.SFROutput = null;
            this.sfrExplorer1.Size = new System.Drawing.Size(1052, 654);
            this.sfrExplorer1.TabIndex = 0;
            // 
            // SFRExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 654);
            this.Controls.Add(this.sfrExplorer1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SFRExplorerView";
            this.Text = "SFR Package";
            this.ResumeLayout(false);

        }

        #endregion

        private SFRExplorer sfrExplorer1;
    }
}