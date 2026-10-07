namespace Heiflow.Controls.WinForm.MT3DMS
{
    partial class SetSpeciesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetSpeciesForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.olvMobileSpeciesList = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSelected = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.olvExchangeSpeciesList = new System.Windows.Forms.DataGridView();
            this.olvColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.olvMineralSpeciesList = new System.Windows.Forms.DataGridView();
            this.olvColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(568, 12);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOk.Location = new System.Drawing.Point(454, 12);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 30);
            this.btnOk.TabIndex = 31;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 631);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 59);
            this.panel1.TabIndex = 34;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(685, 631);
            this.tabControl1.TabIndex = 35;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.olvMobileSpeciesList);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(677, 598);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mobile Species";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // olvMobileSpeciesList
            // 
            this.olvMobileSpeciesList.AllowUserToOrderColumns = true;
            this.olvMobileSpeciesList.AllowDrop = true;
            this.olvMobileSpeciesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colSelected,
            this.olvColumn2});
            this.olvMobileSpeciesList.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMobileSpeciesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMobileSpeciesList.Location = new System.Drawing.Point(3, 4);
            this.olvMobileSpeciesList.Margin = new System.Windows.Forms.Padding(4);
            this.olvMobileSpeciesList.Name = "olvMobileSpeciesList";
            this.olvMobileSpeciesList.AllowUserToAddRows = false;
            this.olvMobileSpeciesList.AllowUserToDeleteRows = false;
            this.olvMobileSpeciesList.AutoGenerateColumns = false;
            this.olvMobileSpeciesList.RowHeadersVisible = false;
            this.olvMobileSpeciesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvMobileSpeciesList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvMobileSpeciesList.Size = new System.Drawing.Size(671, 590);
            this.olvMobileSpeciesList.TabIndex = 21;
            // 
            // colName
            // 
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Species Name";
            this.colName.Name = "colName";
            this.colName.Width = 115;
            // 
            // colSelected
            // 
            this.colSelected.DataPropertyName = "Selected";
            this.colSelected.HeaderText = "Selected";
            this.colSelected.Name = "colSelected";
            this.colSelected.Width = 128;
            // 
            // olvColumn2
            // 
            this.olvColumn2.DataPropertyName = "InitialConcentration";
            this.olvColumn2.HeaderText = "Initial Concentration (mol/L)";
            this.olvColumn2.Name = "olvColumn2";
            this.olvColumn2.Width = 208;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.olvExchangeSpeciesList);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(677, 598);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Exchange Species";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // olvExchangeSpeciesList
            // 
            this.olvExchangeSpeciesList.AllowUserToOrderColumns = true;
            this.olvExchangeSpeciesList.AllowDrop = true;
            this.olvExchangeSpeciesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn8,
            this.olvColumn7});
            this.olvExchangeSpeciesList.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvExchangeSpeciesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvExchangeSpeciesList.Location = new System.Drawing.Point(3, 3);
            this.olvExchangeSpeciesList.Margin = new System.Windows.Forms.Padding(4);
            this.olvExchangeSpeciesList.Name = "olvExchangeSpeciesList";
            this.olvExchangeSpeciesList.AllowUserToAddRows = false;
            this.olvExchangeSpeciesList.AllowUserToDeleteRows = false;
            this.olvExchangeSpeciesList.AutoGenerateColumns = false;
            this.olvExchangeSpeciesList.RowHeadersVisible = false;
            this.olvExchangeSpeciesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvExchangeSpeciesList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvExchangeSpeciesList.Size = new System.Drawing.Size(671, 592);
            this.olvExchangeSpeciesList.TabIndex = 22;
            // 
            // olvColumn5
            // 
            this.olvColumn5.DataPropertyName = "Name";
            this.olvColumn5.HeaderText = "Species Name";
            this.olvColumn5.Name = "olvColumn5";
            this.olvColumn5.Width = 115;
            // 
            // olvColumn6
            // 
            this.olvColumn6.DataPropertyName = "Selected";
            this.olvColumn6.HeaderText = "Selected";
            this.olvColumn6.Name = "olvColumn6";
            this.olvColumn6.Width = 128;
            // 
            // olvColumn8
            // 
            this.olvColumn8.DataPropertyName = "LonNum";
            this.olvColumn8.HeaderText = "Ion Number";
            this.olvColumn8.Name = "olvColumn8";
            this.olvColumn8.Width = 104;
            // 
            // olvColumn7
            // 
            this.olvColumn7.DataPropertyName = "InitialConcentration";
            this.olvColumn7.HeaderText = "Initial Concentration (mol/L)";
            this.olvColumn7.Name = "olvColumn7";
            this.olvColumn7.Width = 400;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.olvMineralSpeciesList);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(677, 598);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mineral Species";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // olvMineralSpeciesList
            // 
            this.olvMineralSpeciesList.AllowUserToOrderColumns = true;
            this.olvMineralSpeciesList.AllowDrop = true;
            this.olvMineralSpeciesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.olvColumn1,
            this.olvColumn3,
            this.olvColumn4});
            this.olvMineralSpeciesList.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMineralSpeciesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMineralSpeciesList.Location = new System.Drawing.Point(3, 3);
            this.olvMineralSpeciesList.Margin = new System.Windows.Forms.Padding(4);
            this.olvMineralSpeciesList.Name = "olvMineralSpeciesList";
            this.olvMineralSpeciesList.AllowUserToAddRows = false;
            this.olvMineralSpeciesList.AllowUserToDeleteRows = false;
            this.olvMineralSpeciesList.AutoGenerateColumns = false;
            this.olvMineralSpeciesList.RowHeadersVisible = false;
            this.olvMineralSpeciesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvMineralSpeciesList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvMineralSpeciesList.Size = new System.Drawing.Size(671, 592);
            this.olvMineralSpeciesList.TabIndex = 22;
            // 
            // olvColumn1
            // 
            this.olvColumn1.DataPropertyName = "Name";
            this.olvColumn1.HeaderText = "Species Name";
            this.olvColumn1.Name = "olvColumn1";
            this.olvColumn1.Width = 115;
            // 
            // olvColumn3
            // 
            this.olvColumn3.DataPropertyName = "Selected";
            this.olvColumn3.HeaderText = "Selected";
            this.olvColumn3.Name = "olvColumn3";
            this.olvColumn3.Width = 128;
            // 
            // olvColumn4
            // 
            this.olvColumn4.DataPropertyName = "InitialConcentration";
            this.olvColumn4.HeaderText = "Initial Concentration (mol/L)";
            this.olvColumn4.Name = "olvColumn4";
            this.olvColumn4.Width = 236;
            // 
            // SetSpeciesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 690);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SetSpeciesForm";
            this.Text = "Set Species";
            this.Load += new System.EventHandler(this.SetSpeciesForm_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView olvMobileSpeciesList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView olvMineralSpeciesList;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView olvExchangeSpeciesList;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn8;
    }
}
