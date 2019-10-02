using Heiflow.Controls.WinForm.Controls;
namespace Heiflow.Controls.WinForm.Display
{
    partial class WinChartView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinChartView));
            this.winChart1 = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.SuspendLayout();
            // 
            // winChart1
            // 
            this.winChart1.BackColor = System.Drawing.SystemColors.Control;
            this.winChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart1.Location = new System.Drawing.Point(0, 0);
            this.winChart1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.winChart1.Name = "winChart1";
            this.winChart1.ShowStatPanel = false;
            this.winChart1.Size = new System.Drawing.Size(868, 570);
            this.winChart1.TabIndex = 0;
            // 
            // WinChartView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 570);
            this.Controls.Add(this.winChart1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WinChartView";
            this.Text = "Figure";
            this.ResumeLayout(false);

        }

        #endregion

        private WinChart winChart1;
    }
}