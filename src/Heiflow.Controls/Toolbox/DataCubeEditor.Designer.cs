namespace Heiflow.Controls.WinForm.Toolbox
{
    partial class DataCubeEditor
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlLeft = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lvMatName = new System.Windows.Forms.ListView();
            this.colMatName = new System.Windows.Forms.ColumnHeader();
            this.colMatSize = new System.Windows.Forms.ColumnHeader();
            this.colMatOwner = new System.Windows.Forms.ColumnHeader();
            this.colMatRepeat = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip_matname = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menu_remove = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.dgvVariables = new System.Windows.Forms.DataGridView();
            this.colVariableIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBehavior = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colConstant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMultiplier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arrayGrid = new SourceGrid.ArrayGrid();
            this.toolStripArray = new System.Windows.Forms.ToolStrip();
            this.tsLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tsSelectionMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsDataViewMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlLeft.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.contextMenuStrip_matname.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).BeginInit();
            this.toolStripArray.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControlLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.arrayGrid);
            this.splitContainer1.Panel2.Controls.Add(this.toolStripArray);
            this.splitContainer1.Size = new System.Drawing.Size(915, 645);
            this.splitContainer1.SplitterDistance = 308;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControlLeft
            // 
            this.tabControlLeft.Controls.Add(this.tabPage5);
            this.tabControlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLeft.Location = new System.Drawing.Point(0, 0);
            this.tabControlLeft.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            this.tabControlLeft.Name = "tabControlLeft";
            this.tabControlLeft.SelectedIndex = 0;
            this.tabControlLeft.Size = new System.Drawing.Size(308, 645);
            this.tabControlLeft.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer3);
            this.tabPage5.Location = new System.Drawing.Point(4, 29);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage5.Size = new System.Drawing.Size(300, 612);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Data Cube";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(4, 5);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lvMatName);
            this.splitContainer3.Panel1.Controls.Add(this.toolStrip2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.dgvVariables);
            this.splitContainer3.Size = new System.Drawing.Size(292, 602);
            this.splitContainer3.SplitterDistance = 301;
            this.splitContainer3.SplitterWidth = 7;
            this.splitContainer3.TabIndex = 1;
            // 
            // lvMatName
            // 
            this.lvMatName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMatName,
            this.colMatSize,
            this.colMatOwner,
            this.colMatRepeat});
            this.lvMatName.ContextMenuStrip = this.contextMenuStrip_matname;
            this.lvMatName.Cursor = System.Windows.Forms.Cursors.Default;
            this.lvMatName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMatName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lvMatName.FullRowSelect = true;
            this.lvMatName.GridLines = true;
            this.lvMatName.HideSelection = false;
            this.lvMatName.Location = new System.Drawing.Point(0, 27);
            this.lvMatName.Margin = new System.Windows.Forms.Padding(5);
            this.lvMatName.Name = "lvMatName";
            this.lvMatName.ShowGroups = false;
            this.lvMatName.ShowItemToolTips = true;
            this.lvMatName.Size = new System.Drawing.Size(292, 274);
            this.lvMatName.TabIndex = 3;
            this.lvMatName.UseCompatibleStateImageBehavior = false;
            this.lvMatName.View = System.Windows.Forms.View.Details;
            this.lvMatName.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvMatName_ItemSelectionChanged);
            this.lvMatName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvMatName_MouseUp);
            // 
            // colMatName
            // 
            this.colMatName.Text = "Name";
            this.colMatName.Width = 100;
            // 
            // colMatSize
            // 
            this.colMatSize.Text = "Size";
            this.colMatSize.Width = 115;
            // 
            // colMatOwner
            // 
            this.colMatOwner.Text = "Owner";
            this.colMatOwner.Width = 83;
            // 
            // colMatRepeat
            // 
            this.colMatRepeat.Text = "Repeat Allowed";
            this.colMatRepeat.Width = 90;
            // 
            // contextMenuStrip_matname
            // 
            this.contextMenuStrip_matname.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuStrip_matname.ImageScalingSize = new System.Drawing.Size(16, 16);
            this.contextMenuStrip_matname.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_remove,
            this.menu_Clear,
            this.toolStripSeparator2});
            this.contextMenuStrip_matname.Name = "contextMenuStrip_matname";
            this.contextMenuStrip_matname.Size = new System.Drawing.Size(193, 62);
            // 
            // menu_remove
            // 
            this.menu_remove.Name = "menu_remove";
            this.menu_remove.Size = new System.Drawing.Size(192, 26);
            this.menu_remove.Text = "Remove";
            this.menu_remove.Click += new System.EventHandler(this.menu_remove_Click);
            // 
            // menu_Clear
            // 
            this.menu_Clear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiClear16;
            this.menu_Clear.Name = "menu_Clear";
            this.menu_Clear.Size = new System.Drawing.Size(192, 26);
            this.menu_Clear.Text = "Clear Workspace";
            this.menu_Clear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(189, 6);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRemove,
            this.btnClear});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(292, 27);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnRemove
            // 
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemove.Enabled = false;
            this.btnRemove.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiDelete24;
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(24, 24);
            this.btnRemove.Text = "Remove selected data cube";
            this.btnRemove.Click += new System.EventHandler(this.menu_remove_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiClear24;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.Text = "Clear work space";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dgvVariables
            // 
            this.dgvVariables.AllowUserToAddRows = false;
            this.dgvVariables.AllowUserToDeleteRows = false;
            this.dgvVariables.AllowUserToOrderColumns = true;
            this.dgvVariables.AutoGenerateColumns = false;
            this.dgvVariables.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVariableIndex,
            this.colBehavior,
            this.colConstant,
            this.colMultiplier});
            this.dgvVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVariables.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvVariables.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvVariables.Location = new System.Drawing.Point(0, 0);
            this.dgvVariables.Margin = new System.Windows.Forms.Padding(5);
            this.dgvVariables.MultiSelect = false;
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.RowHeadersVisible = false;
            this.dgvVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVariables.Size = new System.Drawing.Size(292, 294);
            this.dgvVariables.TabIndex = 2;
            this.dgvVariables.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvVariables_CellValidating);
            this.dgvVariables.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVariables_CellValueChanged);
            this.dgvVariables.SelectionChanged += new System.EventHandler(this.dgvVariables_SelectionChanged);
            // 
            // colVariableIndex
            // 
            this.colVariableIndex.DataPropertyName = "VariableIndex";
            this.colVariableIndex.HeaderText = "Index";
            this.colVariableIndex.Name = "colVariableIndex";
            this.colVariableIndex.ReadOnly = true;
            // 
            // colBehavior
            // 
            this.colBehavior.DataPropertyName = "Behavior";
            this.colBehavior.HeaderText = "Behavior";
            this.colBehavior.Name = "colBehavior";
            // 
            // colConstant
            // 
            this.colConstant.DataPropertyName = "Constant";
            this.colConstant.HeaderText = "Constant";
            this.colConstant.Name = "colConstant";
            // 
            // colMultiplier
            // 
            this.colMultiplier.DataPropertyName = "Multiplier";
            this.colMultiplier.HeaderText = "Multiplier";
            this.colMultiplier.Name = "colMultiplier";
            // 
            // arrayGrid
            // 
            this.arrayGrid.BackColor = System.Drawing.SystemColors.Control;
            this.arrayGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.arrayGrid.ClipboardMode = ((SourceGrid.ClipboardMode)((((SourceGrid.ClipboardMode.Copy | SourceGrid.ClipboardMode.Cut) 
            | SourceGrid.ClipboardMode.Paste) 
            | SourceGrid.ClipboardMode.Delete)));
            this.arrayGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrayGrid.EnableSort = true;
            this.arrayGrid.FixedColumns = 1;
            this.arrayGrid.FixedRows = 1;
            this.arrayGrid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.arrayGrid.Location = new System.Drawing.Point(0, 28);
            this.arrayGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.arrayGrid.Name = "arrayGrid";
            this.arrayGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.arrayGrid.Size = new System.Drawing.Size(602, 617);
            this.arrayGrid.TabIndex = 26;
            this.arrayGrid.TabStop = true;
            this.arrayGrid.ToolTipText = "";
            // 
            // toolStripArray
            // 
            this.toolStripArray.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripArray.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripArray.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripArray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLabel,
            this.toolStripLabel2,
            this.tsSelectionMode,
            this.toolStripLabel1,
            this.tsDataViewMode,
            this.toolStripSeparator1,
            this.btnSave,
            this.btnExport,
            this.btnImport});
            this.toolStripArray.Location = new System.Drawing.Point(0, 0);
            this.toolStripArray.Name = "toolStripArray";
            this.toolStripArray.Size = new System.Drawing.Size(602, 28);
            this.toolStripArray.TabIndex = 1;
            this.toolStripArray.Text = "toolStrip1";
            // 
            // tsLabel
            // 
            this.tsLabel.Name = "tsLabel";
            this.tsLabel.Size = new System.Drawing.Size(51, 25);
            this.tsLabel.Text = "Empty";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(70, 25);
            this.toolStripLabel2.Text = "Selection";
            // 
            // tsSelectionMode
            // 
            this.tsSelectionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsSelectionMode.Items.AddRange(new object[] {
            "Cell",
            "Row",
            "Column"});
            this.tsSelectionMode.Name = "tsSelectionMode";
            this.tsSelectionMode.Size = new System.Drawing.Size(160, 28);
            this.tsSelectionMode.SelectedIndexChanged += new System.EventHandler(this.tsSelectionMode_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(53, 25);
            this.toolStripLabel1.Text = "Layout";
            // 
            // tsDataViewMode
            // 
            this.tsDataViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsDataViewMode.Items.AddRange(new object[] {
            "Serial",
            "Regular"});
            this.tsDataViewMode.Name = "tsDataViewMode";
            this.tsDataViewMode.Size = new System.Drawing.Size(160, 28);
            this.tsDataViewMode.SelectedIndexChanged += new System.EventHandler(this.tsDataViewMode_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiSave24;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 25);
            this.btnSave.Text = "Save temporarily. This will not save to source file";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiExcel24;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(24, 25);
            this.btnExport.Text = "Export to a csv file";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiImport24;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(24, 25);
            this.btnImport.Text = "Import from an exsiting file";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // DataCubeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "DataCubeEditor";
            this.Size = new System.Drawing.Size(915, 645);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlLeft.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.contextMenuStrip_matname.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).EndInit();
            this.toolStripArray.ResumeLayout(false);
            this.toolStripArray.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlLeft;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ListView lvMatName;
        private System.Windows.Forms.ColumnHeader colMatName;
        private System.Windows.Forms.ColumnHeader colMatSize;
        private System.Windows.Forms.ColumnHeader colMatOwner;
        private System.Windows.Forms.ColumnHeader colMatRepeat;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.DataGridView dgvVariables;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVariableIndex;
        private System.Windows.Forms.DataGridViewComboBoxColumn colBehavior;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConstant;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMultiplier;
        private System.Windows.Forms.ToolStrip toolStripArray;
        private System.Windows.Forms.ToolStripButton btnSave;
        private SourceGrid.ArrayGrid arrayGrid;
        private System.Windows.Forms.ToolStripComboBox tsDataViewMode;
        private System.Windows.Forms.ToolStripComboBox tsSelectionMode;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel tsLabel;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_matname;
        private System.Windows.Forms.ToolStripMenuItem menu_remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menu_Clear;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripButton btnImport;
    }
}
