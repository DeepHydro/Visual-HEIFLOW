using Heiflow.Controls.WinForm.Controls;
namespace Heiflow.Presentation.Controls.Project
{
    partial class NewProjectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectionForm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtPrjName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPrjDir = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.lstPrjTemplate = new Heiflow.Controls.WinForm.Controls.MyListView();
            this.chbImprot = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbVersion = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 368);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name:";
            // 
            // txtPrjName
            // 
            this.txtPrjName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrjName.Location = new System.Drawing.Point(101, 368);
            this.txtPrjName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPrjName.Name = "txtPrjName";
            this.txtPrjName.Size = new System.Drawing.Size(435, 27);
            this.txtPrjName.TabIndex = 1;
            this.txtPrjName.TextChanged += new System.EventHandler(this.txtPrjName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 401);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Location:";
            // 
            // txtPrjDir
            // 
            this.txtPrjDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrjDir.Location = new System.Drawing.Point(101, 401);
            this.txtPrjDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPrjDir.Name = "txtPrjDir";
            this.txtPrjDir.Size = new System.Drawing.Size(435, 27);
            this.txtPrjDir.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(546, 396);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(100, 30);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(434, 441);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(546, 441);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.propertyGrid1);
            this.panel1.Controls.Add(this.lstPrjTemplate);
            this.panel1.Location = new System.Drawing.Point(-2, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(659, 327);
            this.panel1.TabIndex = 4;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(446, 8);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(210, 310);
            this.propertyGrid1.TabIndex = 1;
            // 
            // lstPrjTemplate
            // 
            this.lstPrjTemplate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstPrjTemplate.HideSelection = false;
            this.lstPrjTemplate.Location = new System.Drawing.Point(12, 8);
            this.lstPrjTemplate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstPrjTemplate.Name = "lstPrjTemplate";
            this.lstPrjTemplate.Size = new System.Drawing.Size(428, 310);
            this.lstPrjTemplate.TabIndex = 0;
            this.lstPrjTemplate.UseCompatibleStateImageBehavior = false;
            this.lstPrjTemplate.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstPrjTemplate_ItemSelectionChanged);
            // 
            // chbImprot
            // 
            this.chbImprot.AutoSize = true;
            this.chbImprot.Location = new System.Drawing.Point(18, 438);
            this.chbImprot.Name = "chbImprot";
            this.chbImprot.Size = new System.Drawing.Size(234, 24);
            this.chbImprot.TabIndex = 5;
            this.chbImprot.Text = "Import from an exsiting model";
            this.chbImprot.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 336);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Version:";
            // 
            // cmbVersion
            // 
            this.cmbVersion.FormattingEnabled = true;
            this.cmbVersion.Location = new System.Drawing.Point(101, 333);
            this.cmbVersion.Name = "cmbVersion";
            this.cmbVersion.Size = new System.Drawing.Size(163, 28);
            this.cmbVersion.TabIndex = 6;
            // 
            // NewProjectionForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(658, 479);
            this.Controls.Add(this.cmbVersion);
            this.Controls.Add(this.chbImprot);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPrjDir);
            this.Controls.Add(this.txtPrjName);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectionForm";
            this.Text = "New Project";
            this.Load += new System.EventHandler(this.NewPrjForm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyListView lstPrjTemplate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPrjName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPrjDir;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chbImprot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbVersion;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}