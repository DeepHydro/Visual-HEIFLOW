namespace Heiflow.Models.Hydrodynamics.Susbed
{
    partial class CrossSectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrossSectionForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.olvMatName = new System.Windows.Forms.DataGridView();
            this.olvColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.winChartSection = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.olvMatName);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(979, 583);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.TabIndex = 0;
            // 
            // olvMatName
            // 
            this.olvMatName.AllowUserToOrderColumns = true;
            this.olvMatName.AllowDrop = true;
            this.olvMatName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.olvMatName.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMatName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMatName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMatName.Location = new System.Drawing.Point(0, 0);
            this.olvMatName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvMatName.Name = "olvMatName";
            this.olvMatName.AllowUserToAddRows = false;
            this.olvMatName.AllowUserToDeleteRows = false;
            this.olvMatName.AutoGenerateColumns = false;
            this.olvMatName.RowHeadersVisible = false;
            this.olvMatName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvMatName.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvMatName.Size = new System.Drawing.Size(326, 583);
            this.olvMatName.TabIndex = 4;
            this.olvMatName.SelectionChanged += new System.EventHandler(this.olvMatName_ItemSelectionChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.DataPropertyName = "ID";
            this.olvColumn1.HeaderText = "断面序号";
            this.olvColumn1.Name = "olvColumn1";
            this.olvColumn1.Width = 81;
            // 
            // olvColumn2
            // 
            this.olvColumn2.DataPropertyName = "Name";
            this.olvColumn2.HeaderText = "断面编号";
            this.olvColumn2.Name = "olvColumn2";
            this.olvColumn2.Width = 95;
            // 
            // olvColumn3
            // 
            this.olvColumn3.DataPropertyName = "NumNodes";
            this.olvColumn3.HeaderText = "断面节点数";
            this.olvColumn3.Name = "olvColumn3";
            this.olvColumn3.Width = 113;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(649, 583);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.winChartSection);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(641, 557);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "断面形状";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // winChartSection
            // 
            this.winChartSection.BackColor = System.Drawing.SystemColors.Control;
            this.winChartSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChartSection.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChartSection.Location = new System.Drawing.Point(3, 3);
            this.winChartSection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChartSection.Name = "winChartSection";
            this.winChartSection.Size = new System.Drawing.Size(635, 551);
            this.winChartSection.TabIndex = 0;
            this.winChartSection.ShowStatPanel = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(641, 557);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "断面数据";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(635, 551);
            this.dataGridView1.TabIndex = 0;
            // 
            // CrossSectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 583);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CrossSectionForm";
            this.Text = "初始断面";
            this.Load += new System.EventHandler(this.CrossSectionForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Controls.WinForm.Controls.WinChart winChartSection;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView olvMatName;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}
