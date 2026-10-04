namespace Heiflow.Visualization.Studio.Controls
{
    partial class FormSymbol
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
            this.rasterCategoryControl1 = new DotSpatial.Symbology.Forms.RasterCategoryControl();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rasterCategoryControl1
            // 
            this.rasterCategoryControl1.Location = new System.Drawing.Point(12, 12);
            this.rasterCategoryControl1.Maximum = 0D;
            this.rasterCategoryControl1.Minimum = 0D;
            this.rasterCategoryControl1.Name = "rasterCategoryControl1";
            this.rasterCategoryControl1.Size = new System.Drawing.Size(807, 432);
            this.rasterCategoryControl1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(573, 459);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormSymbol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 494);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rasterCategoryControl1);
            this.Name = "FormSymbol";
            this.Text = "FormSymbol";
            this.ResumeLayout(false);

        }

        #endregion

        private DotSpatial.Symbology.Forms.RasterCategoryControl rasterCategoryControl1;
        private System.Windows.Forms.Button button1;
    }
}