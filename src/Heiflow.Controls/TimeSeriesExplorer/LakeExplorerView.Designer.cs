namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    partial class LakeExplorerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LakeExplorerView));
            this.timeSeriesExplorer1 = new Heiflow.Controls.WinForm.TimeSeriesExplorer.SiteTimeSeriesExplorer();
            this.SuspendLayout();
            // 
            // timeSeriesExplorer1
            // 
            this.timeSeriesExplorer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeSeriesExplorer1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.timeSeriesExplorer1.Location = new System.Drawing.Point(0, 0);
            this.timeSeriesExplorer1.Name = "timeSeriesExplorer1";
            this.timeSeriesExplorer1.ODM = null;
            this.timeSeriesExplorer1.Package = null;
            this.timeSeriesExplorer1.Size = new System.Drawing.Size(981, 580);
            this.timeSeriesExplorer1.SQLSelection = null;
            this.timeSeriesExplorer1.TabIndex = 0;
            // 
            // LakeExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(981, 580);
            this.Controls.Add(this.timeSeriesExplorer1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LakeExplorerView";
            this.Text = "Lake Explorer";
            this.ResumeLayout(false);

        }

        #endregion

        private SiteTimeSeriesExplorer timeSeriesExplorer1;
    }
}