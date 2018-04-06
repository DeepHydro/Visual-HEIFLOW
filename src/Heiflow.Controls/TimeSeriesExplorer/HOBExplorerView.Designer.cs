using Heiflow.Controls.WinForm.Properties;
namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    partial class HOBExplorerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HOBExplorerView));
            this.timeSeriesExplorer1 = new Heiflow.Controls.WinForm.TimeSeriesExplorer.SiteTimeSeriesExplorer();
            this.tbn_compare_tr = new System.Windows.Forms.ToolStripButton();
            this.tbn_compare_ss = new System.Windows.Forms.ToolStripButton();
            this.cmbMapLayers = new System.Windows.Forms.ToolStripComboBox();
            this.SuspendLayout();
            // 
            // timeSeriesExplorer1
            // 
            this.timeSeriesExplorer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeSeriesExplorer1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.timeSeriesExplorer1.Location = new System.Drawing.Point(0, 0);
            this.timeSeriesExplorer1.Margin = new System.Windows.Forms.Padding(2);
            this.timeSeriesExplorer1.Name = "timeSeriesExplorer1";
            this.timeSeriesExplorer1.ODM = null;
            this.timeSeriesExplorer1.Package = null;
            this.timeSeriesExplorer1.Size = new System.Drawing.Size(1044, 580);
            this.timeSeriesExplorer1.SQLSelection = null;
            this.timeSeriesExplorer1.TabIndex = 0;
            // 
            // tbn_compare_tr
            // 
            this.tbn_compare_tr.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbn_compare_tr.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GraphScatterplotCreate16;
            this.tbn_compare_tr.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbn_compare_tr.Name = "tbn_compare_tr";
            this.tbn_compare_tr.Size = new System.Drawing.Size(24, 25);
            this.tbn_compare_tr.Text = "Compare Transit State";
            this.tbn_compare_tr.ToolTipText = "Compare Transit State";
            // 
            // tbn_compare_ss
            // 
            this.tbn_compare_ss.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbn_compare_ss.Image = global::Heiflow.Controls.WinForm.Properties.Resources.ViewCompact16;
            this.tbn_compare_ss.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbn_compare_ss.Name = "tbn_compare_ss";
            this.tbn_compare_ss.Size = new System.Drawing.Size(24, 25);
            this.tbn_compare_ss.Text = "Compare";
            this.tbn_compare_ss.ToolTipText = "Compare Steady State";
            // 
            // cmbMapLayers
            // 
            this.cmbMapLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapLayers.Name = "cmbMapLayers";
            this.cmbMapLayers.Size = new System.Drawing.Size(121, 25);
            this.cmbMapLayers.ToolTipText = "Select a point feature layer";
            // 
            // HOBExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 580);
            this.Controls.Add(this.timeSeriesExplorer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "HOBExplorerView";
            this.Text = "Head Observation Explorer";
            this.ResumeLayout(false);

        }

        #endregion

        private SiteTimeSeriesExplorer timeSeriesExplorer1;
        private System.Windows.Forms.ToolStripButton tbn_compare_tr;
        private System.Windows.Forms.ToolStripButton tbn_compare_ss;
        private System.Windows.Forms.ToolStripComboBox cmbMapLayers;
    }
}