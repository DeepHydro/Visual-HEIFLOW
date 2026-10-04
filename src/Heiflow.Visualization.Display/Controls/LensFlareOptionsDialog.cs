using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUST.WREIS.Dot3D;

namespace HUST.WREIS.Dot3D.Display.Controls
{
    public class LensFlareOptionsDialog : System.Windows.Forms.Form
    {
        private LensFlareEffect m_effect;
        public LensFlareOptionsDialog(LensFlareEffect effect)
        {
            InitializeComponent();
            m_effect = effect;
            tbFlareChooser.Maximum = m_effect.m_flareCount - 1;
        }

        private void tbFlareChooser_ValueChanged(object sender, EventArgs e)
        {
            tbAlpha.Value = (int)(100.0f / 255.0f * m_effect.m_flareOpacities[tbFlareChooser.Value]);
            tbPos1.Value = (int)(100.0f * (m_effect.m_flarePositions[tbFlareChooser.Value] + 1) / 3.0f);
            tbSize1.Value = (int)(100.0f * m_effect.m_flareSizes[tbFlareChooser.Value] / 5.0f);
        }

        private void tbAlpha_ValueChanged(object sender, EventArgs e)
        {
            m_effect.m_flareOpacities[tbFlareChooser.Value] = (float)tbAlpha.Value * 255.0f / 100.0f;
        }

        private void tbPos1_ValueChanged(object sender, EventArgs e)
        {
            m_effect.m_flarePositions[tbFlareChooser.Value] = -1.0f + (float)tbPos1.Value * 3.0f / 100.0f;
        }

        private void tbSize1_ValueChanged(object sender, EventArgs e)
        {
            m_effect.m_flareSizes[tbFlareChooser.Value] = 5.0f * (float)tbSize1.Value / 100.0f;
        }

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
            m_effect.SaveSettings();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbSize1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPos1 = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.tbAlpha = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.tbFlareChooser = new System.Windows.Forms.TrackBar();
            this.tbFlareNum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbSize1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPos1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFlareChooser)).BeginInit();
            this.SuspendLayout();
            // 
            // tbSize1
            // 
            this.tbSize1.Location = new System.Drawing.Point(12, 25);
            this.tbSize1.Maximum = 100;
            this.tbSize1.Minimum = 0;
            this.tbSize1.Name = "tbSize1";
            this.tbSize1.Size = new System.Drawing.Size(260, 45);
            this.tbSize1.TabIndex = 0;
            this.tbSize1.TickFrequency = 10;
            this.tbSize1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbSize1.Value = 10;
            this.tbSize1.ValueChanged += new System.EventHandler(this.tbSize1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Size";
            // 
            // tbPos1
            // 
            this.tbPos1.Location = new System.Drawing.Point(12, 89);
            this.tbPos1.Maximum = 200;
            this.tbPos1.Name = "tbPos1";
            this.tbPos1.Size = new System.Drawing.Size(260, 45);
            this.tbPos1.TabIndex = 2;
            this.tbPos1.TickFrequency = 10;
            this.tbPos1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbPos1.Value = 10;
            this.tbPos1.ValueChanged += new System.EventHandler(this.tbPos1_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Position";
            // 
            // tbAlpha
            // 
            this.tbAlpha.Location = new System.Drawing.Point(12, 153);
            this.tbAlpha.Maximum = 100;
            this.tbAlpha.Minimum = 0;
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Size = new System.Drawing.Size(260, 45);
            this.tbAlpha.TabIndex = 4;
            this.tbAlpha.TickFrequency = 10;
            this.tbAlpha.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbAlpha.Value = 10;
            this.tbAlpha.ValueChanged += new System.EventHandler(this.tbAlpha_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Opacity";
            // 
            // tbFlareChooser
            // 
            this.tbFlareChooser.LargeChange = 1;
            this.tbFlareChooser.Location = new System.Drawing.Point(12, 252);
            this.tbFlareChooser.Maximum = 4;
            this.tbFlareChooser.Name = "tbFlareChooser";
            this.tbFlareChooser.Size = new System.Drawing.Size(260, 45);
            this.tbFlareChooser.TabIndex = 6;
            this.tbFlareChooser.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.tbFlareChooser.Value = 1;
            this.tbFlareChooser.ValueChanged += new System.EventHandler(this.tbFlareChooser_ValueChanged);
            // 
            // tbFlareNum
            // 
            this.tbFlareNum.AutoSize = true;
            this.tbFlareNum.Location = new System.Drawing.Point(12, 233);
            this.tbFlareNum.Name = "tbFlareNum";
            this.tbFlareNum.Size = new System.Drawing.Size(72, 13);
            this.tbFlareNum.TabIndex = 7;
            this.tbFlareNum.Text = "Flare Chooser";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 309);
            this.Controls.Add(this.tbFlareNum);
            this.Controls.Add(this.tbFlareChooser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbAlpha);
            this.Controls.Add(this.tbSize1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPos1);
            this.Name = "Lens Flare Settings";
            this.Text = "Lens Flare Settings";
            ((System.ComponentModel.ISupportInitialize)(this.tbSize1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPos1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFlareChooser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbSize1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tbPos1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar tbAlpha;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar tbFlareChooser;
        private System.Windows.Forms.Label tbFlareNum;

    }
}
