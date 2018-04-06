namespace Heiflow.Controls.WinForm.Display
{
    partial class Win3DView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Win3DView));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.view3DControl1 = new Heiflow.Controls.WinForm.Controls.View3DControl();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // view3DControl1
            // 
            this.view3DControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view3DControl1.Location = new System.Drawing.Point(0, 0);
            this.view3DControl1.Name = "view3DControl1";
            this.view3DControl1.Size = new System.Drawing.Size(1085, 526);
            this.view3DControl1.TabIndex = 0;
            // 
            // Win3DView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1085, 526);
            this.Controls.Add(this.view3DControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Win3DView";
            this.Text = "3D View";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private Controls.View3DControl view3DControl1;


    }
}