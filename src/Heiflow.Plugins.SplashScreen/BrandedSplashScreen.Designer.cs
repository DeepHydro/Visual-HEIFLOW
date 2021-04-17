namespace DotSpatial.Plugins.SplashScreenManager
{
    partial class BrandedSplashScreen
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
            this.marqueeProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.uxMessage = new DevExpress.XtraEditors.LabelControl();
            this.uxSplashImage = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxSplashImage.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // marqueeProgressBarControl1
            // 
            this.marqueeProgressBarControl1.EditValue = 0;
            this.marqueeProgressBarControl1.Location = new System.Drawing.Point(23, 231);
            this.marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
            this.marqueeProgressBarControl1.Size = new System.Drawing.Size(404, 12);
            this.marqueeProgressBarControl1.TabIndex = 5;
            // 
            // labelControl1
            // 
            this.labelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.labelControl1.Location = new System.Drawing.Point(23, 276);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(109, 13);
            this.labelControl1.TabIndex = 6;
            this.labelControl1.Text = "Copyright ©2016-2020 SUSTech. All Rights Reserved. Version 1.2.0";
            // 
            // uxMessage
            // 
            this.uxMessage.Location = new System.Drawing.Point(23, 206);
            this.uxMessage.Name = "uxMessage";
            this.uxMessage.Size = new System.Drawing.Size(50, 13);
            this.uxMessage.TabIndex = 7;
            this.uxMessage.Text = "Starting...";
            // 
            // uxSplashImage
            // 
            this.uxSplashImage.EditValue = global::Heiflow.Plugins.SplashScreen.Properties.Resources.splash_vhf;
            this.uxSplashImage.Location = new System.Drawing.Point(12, 12);
            this.uxSplashImage.Name = "uxSplashImage";
            this.uxSplashImage.Properties.AllowFocused = false;
            this.uxSplashImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.uxSplashImage.Properties.Appearance.Options.UseBackColor = true;
            this.uxSplashImage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.uxSplashImage.Properties.ShowMenu = false;
            this.uxSplashImage.Size = new System.Drawing.Size(426, 180);
            this.uxSplashImage.TabIndex = 9;
            // 
            // BrandedSplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 320);
            this.Controls.Add(this.uxSplashImage);
            this.Controls.Add(this.uxMessage);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.marqueeProgressBarControl1);
            this.Name = "BrandedSplashScreen";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.BrandedSplashScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uxSplashImage.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.MarqueeProgressBarControl marqueeProgressBarControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl uxMessage;
        private DevExpress.XtraEditors.PictureEdit uxSplashImage;
    }
}
