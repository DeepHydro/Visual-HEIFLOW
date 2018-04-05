namespace Heiflow.Image
{
    partial class WebMapForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxMapType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(267, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxMapType
            // 
            this.comboBoxMapType.FormattingEnabled = true;
            this.comboBoxMapType.Location = new System.Drawing.Point(29, 58);
            this.comboBoxMapType.Name = "comboBoxMapType";
            this.comboBoxMapType.Size = new System.Drawing.Size(215, 20);
            this.comboBoxMapType.TabIndex = 1;
            this.comboBoxMapType.DropDownClosed += new System.EventHandler(this.comboBoxMapType_DropDownClosed);
            // 
            // WebMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 535);
            this.Controls.Add(this.comboBoxMapType);
            this.Controls.Add(this.button1);
            this.Name = "WebMapForm";
            this.Text = "Web Map";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBoxMapType;
    }
}