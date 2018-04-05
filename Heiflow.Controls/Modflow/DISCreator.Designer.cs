namespace Heiflow.Controls.WinForm.Modflow
{
    partial class DISCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DISCreator));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageCorrectElev = new System.Windows.Forms.TabPage();
            this.dataGridEx1 = new Heiflow.Controls.WinForm.Controls.DataGridEx();
            this.tabControl1.SuspendLayout();
            this.tabPageCorrectElev.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageCorrectElev);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(789, 512);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageCorrectElev
            // 
            this.tabPageCorrectElev.Controls.Add(this.dataGridEx1);
            this.tabPageCorrectElev.Location = new System.Drawing.Point(4, 24);
            this.tabPageCorrectElev.Name = "tabPageCorrectElev";
            this.tabPageCorrectElev.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCorrectElev.Size = new System.Drawing.Size(781, 484);
            this.tabPageCorrectElev.TabIndex = 0;
            this.tabPageCorrectElev.Text = "Correct Elevation";
            this.tabPageCorrectElev.UseVisualStyleBackColor = true;
            // 
            // dataGridEx1
            // 
            this.dataGridEx1.DataTable = null;
            this.dataGridEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridEx1.Location = new System.Drawing.Point(3, 3);
            this.dataGridEx1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridEx1.Name = "dataGridEx1";
            this.dataGridEx1.Size = new System.Drawing.Size(775, 478);
            this.dataGridEx1.TabIndex = 0;
            // 
            // DISCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 512);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DISCreator";
            this.Text = "DIS Creator";
            this.tabControl1.ResumeLayout(false);
            this.tabPageCorrectElev.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageCorrectElev;
        private Controls.DataGridEx dataGridEx1;
    }
}