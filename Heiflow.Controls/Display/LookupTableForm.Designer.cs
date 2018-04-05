using Heiflow.Controls.WinForm.Controls;
namespace Heiflow.Controls.WinForm.Display
{
    partial class LookupTableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LookupTableForm));
            this.labelSP = new System.Windows.Forms.ToolStripLabel();
            this.cmbStressPeriod = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.dataGridEx1 = new Heiflow.Controls.WinForm.Controls.DataGridEx();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSP
            // 
            this.labelSP.Name = "labelSP";
            this.labelSP.Size = new System.Drawing.Size(105, 25);
            this.labelSP.Text = "Stress Period";
            // 
            // cmbStressPeriod
            // 
            this.cmbStressPeriod.Name = "cmbStressPeriod";
            this.cmbStressPeriod.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericSave_B_16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 25);
            this.btnSave.Text = "Save to local file";
            this.btnSave.ToolTipText = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.excel_32;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(24, 25);
            this.btnExport.Text = "Export lookup table to csv file";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.TmImportFeatures16;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(24, 25);
            this.btnImport.Text = "Import from an existing file";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericDeleteRed32;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 25);
            this.btnClear.Text = "Clear cached lookup file";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 356);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(734, 59);
            this.panel1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnCancel.Location = new System.Drawing.Point(623, 10);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 34);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnOK.Location = new System.Drawing.Point(522, 10);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(83, 34);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "Run";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dataGridEx1
            // 
            this.dataGridEx1.DataObjectName = "matrix";
            this.dataGridEx1.DataTable = null;
            this.dataGridEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridEx1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.dataGridEx1.Location = new System.Drawing.Point(0, 0);
            this.dataGridEx1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridEx1.Name = "dataGridEx1";
            this.dataGridEx1.Size = new System.Drawing.Size(734, 356);
            this.dataGridEx1.TabIndex = 2;
            // 
            // LookupTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 415);
            this.Controls.Add(this.dataGridEx1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "LookupTableForm";
            this.Text = "Lookup Table";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripComboBox cmbStressPeriod;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ToolStripLabel labelSP;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripButton btnClear;
        private DataGridEx dataGridEx1;
    }
}