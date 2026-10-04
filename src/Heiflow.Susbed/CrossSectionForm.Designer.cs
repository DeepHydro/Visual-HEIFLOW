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
            this.olvMatName = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
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
            ((System.ComponentModel.ISupportInitialize)(this.olvMatName)).BeginInit();
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
            this.olvMatName.AllColumns.Add(this.olvColumn1);
            this.olvMatName.AllColumns.Add(this.olvColumn2);
            this.olvMatName.AllColumns.Add(this.olvColumn3);
            this.olvMatName.AllowColumnReorder = true;
            this.olvMatName.AllowDrop = true;
            this.olvMatName.AutoGenerateColumns = false;
            this.olvMatName.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMatName.CellEditUseWholeCell = false;
            this.olvMatName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.olvMatName.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMatName.DataSource = null;
            this.olvMatName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMatName.EmptyListMsg = "";
            this.olvMatName.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMatName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMatName.FullRowSelect = true;
            this.olvMatName.GridLines = true;
            this.olvMatName.GroupWithItemCountFormat = "";
            this.olvMatName.GroupWithItemCountSingularFormat = "";
            this.olvMatName.HideSelection = false;
            this.olvMatName.Location = new System.Drawing.Point(0, 0);
            this.olvMatName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvMatName.Name = "olvMatName";
            this.olvMatName.RowHeight = 20;
            this.olvMatName.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMatName.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvMatName.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvMatName.ShowCommandMenuOnRightClick = true;
            this.olvMatName.ShowGroups = false;
            this.olvMatName.ShowImagesOnSubItems = true;
            this.olvMatName.ShowItemToolTips = true;
            this.olvMatName.Size = new System.Drawing.Size(326, 583);
            this.olvMatName.TabIndex = 4;
            this.olvMatName.UseCellFormatEvents = true;
            this.olvMatName.UseCompatibleStateImageBehavior = false;
            this.olvMatName.UseFilterIndicator = true;
            this.olvMatName.UseFiltering = true;
            this.olvMatName.UseHotItem = true;
            this.olvMatName.UseTranslucentHotItem = true;
            this.olvMatName.View = System.Windows.Forms.View.Details;
            this.olvMatName.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.olvMatName_ItemSelectionChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "ID";
            this.olvColumn1.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn1.CellEditUseWholeCell = false;
            this.olvColumn1.IsTileViewColumn = true;
            this.olvColumn1.Text = "断面序号";
            this.olvColumn1.UseInitialLetterForGroup = true;
            this.olvColumn1.Width = 81;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Name";
            this.olvColumn2.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn2.CellEditUseWholeCell = false;
            this.olvColumn2.IsTileViewColumn = true;
            this.olvColumn2.Text = "断面编号";
            this.olvColumn2.Width = 95;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "NumNodes";
            this.olvColumn3.Text = "断面节点数";
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
            ((System.ComponentModel.ISupportInitialize)(this.olvMatName)).EndInit();
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
        private BrightIdeasSoftware.DataListView olvMatName;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}