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
            this.olvMatName = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextMenuStrip_matname = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menu_remove = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.olvVariableName = new BrightIdeasSoftware.DataListView();
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvBehavior = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.arrayGrid = new SourceGrid.ArrayGrid();
            this.toolStripArray = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tsSelectionMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsDataViewMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsLabel = new System.Windows.Forms.ToolStripLabel();
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
            ((System.ComponentModel.ISupportInitialize)(this.olvMatName)).BeginInit();
            this.contextMenuStrip_matname.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvVariableName)).BeginInit();
            this.toolStripArray.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.tabControlLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.arrayGrid);
            this.splitContainer1.Panel2.Controls.Add(this.toolStripArray);
            this.splitContainer1.Size = new System.Drawing.Size(1090, 550);
            this.splitContainer1.SplitterDistance = 368;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControlLeft
            // 
            this.tabControlLeft.Controls.Add(this.tabPage5);
            this.tabControlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLeft.Location = new System.Drawing.Point(0, 0);
            this.tabControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControlLeft.Name = "tabControlLeft";
            this.tabControlLeft.SelectedIndex = 0;
            this.tabControlLeft.Size = new System.Drawing.Size(368, 550);
            this.tabControlLeft.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer3);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(360, 524);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Data Cube";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.olvMatName);
            this.splitContainer3.Panel1.Controls.Add(this.toolStrip2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.olvVariableName);
            this.splitContainer3.Size = new System.Drawing.Size(354, 518);
            this.splitContainer3.SplitterDistance = 259;
            this.splitContainer3.TabIndex = 1;
            // 
            // olvMatName
            // 
            this.olvMatName.AllColumns.Add(this.olvColumn1);
            this.olvMatName.AllColumns.Add(this.olvColumn2);
            this.olvMatName.AllColumns.Add(this.olvColumn3);
            this.olvMatName.AllowColumnReorder = true;
            this.olvMatName.AllowDrop = true;
            this.olvMatName.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMatName.CellEditUseWholeCell = false;
            this.olvMatName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.olvMatName.ContextMenuStrip = this.contextMenuStrip_matname;
            this.olvMatName.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMatName.DataSource = null;
            this.olvMatName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMatName.EmptyListMsg = "";
            this.olvMatName.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMatName.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.olvMatName.FullRowSelect = true;
            this.olvMatName.GridLines = true;
            this.olvMatName.GroupWithItemCountFormat = "";
            this.olvMatName.GroupWithItemCountSingularFormat = "";
            this.olvMatName.HideSelection = false;
            this.olvMatName.Location = new System.Drawing.Point(0, 27);
            this.olvMatName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvMatName.Name = "olvMatName";
            this.olvMatName.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMatName.SelectedBackColor = System.Drawing.Color.LimeGreen;
            this.olvMatName.SelectedForeColor = System.Drawing.Color.White;
            this.olvMatName.ShowCommandMenuOnRightClick = true;
            this.olvMatName.ShowGroups = false;
            this.olvMatName.ShowImagesOnSubItems = true;
            this.olvMatName.ShowItemToolTips = true;
            this.olvMatName.Size = new System.Drawing.Size(354, 232);
            this.olvMatName.TabIndex = 3;
            this.olvMatName.UnfocusedSelectedBackColor = System.Drawing.Color.LimeGreen;
            this.olvMatName.UnfocusedSelectedForeColor = System.Drawing.Color.White;
            this.olvMatName.UseCellFormatEvents = true;
            this.olvMatName.UseCompatibleStateImageBehavior = false;
            this.olvMatName.UseFilterIndicator = true;
            this.olvMatName.UseFiltering = true;
            this.olvMatName.UseHotItem = true;
            this.olvMatName.UseTranslucentHotItem = true;
            this.olvMatName.View = System.Windows.Forms.View.Details;
            this.olvMatName.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.olvMatName_ItemSelectionChanged);
            this.olvMatName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.olvMatName_MouseUp);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn1.CellEditUseWholeCell = false;
            this.olvColumn1.IsTileViewColumn = true;
            this.olvColumn1.Text = "Name";
            this.olvColumn1.UseInitialLetterForGroup = true;
            this.olvColumn1.Width = 100;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Size";
            this.olvColumn2.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn2.CellEditUseWholeCell = false;
            this.olvColumn2.IsTileViewColumn = true;
            this.olvColumn2.Text = "Size";
            this.olvColumn2.Width = 115;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Owner";
            this.olvColumn3.Text = "Owner";
            this.olvColumn3.Width = 83;
            // 
            // contextMenuStrip_matname
            // 
            this.contextMenuStrip_matname.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_matname.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_remove,
            this.menu_Clear,
            this.toolStripSeparator2});
            this.contextMenuStrip_matname.Name = "contextMenuStrip_matname";
            this.contextMenuStrip_matname.Size = new System.Drawing.Size(181, 62);
            // 
            // menu_remove
            // 
            this.menu_remove.Name = "menu_remove";
            this.menu_remove.Size = new System.Drawing.Size(180, 26);
            this.menu_remove.Text = "Remove";
            this.menu_remove.Click += new System.EventHandler(this.menu_remove_Click);
            // 
            // menu_Clear
            // 
            this.menu_Clear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_edit_clear_15273;
            this.menu_Clear.Name = "menu_Clear";
            this.menu_Clear.Size = new System.Drawing.Size(180, 26);
            this.menu_Clear.Text = "Clear Workspace";
            this.menu_Clear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRemove,
            this.btnClear});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(354, 27);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnRemove
            // 
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemove.Enabled = false;
            this.btnRemove.Image = global::Heiflow.Controls.WinForm.Properties.Resources.SpatialAnalystTrainingSampleClear16;
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(24, 24);
            this.btnRemove.Text = "Remove selected data cube";
            this.btnRemove.Click += new System.EventHandler(this.menu_remove_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_edit_clear_15273;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.Text = "Clear work space";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // olvVariableName
            // 
            this.olvVariableName.AllColumns.Add(this.olvColumn4);
            this.olvVariableName.AllColumns.Add(this.olvBehavior);
            this.olvVariableName.AllColumns.Add(this.olvColumn6);
            this.olvVariableName.AllColumns.Add(this.olvColumn7);
            this.olvVariableName.AllowColumnReorder = true;
            this.olvVariableName.AllowDrop = true;
            this.olvVariableName.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvVariableName.CellEditUseWholeCell = false;
            this.olvVariableName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn4,
            this.olvBehavior,
            this.olvColumn6,
            this.olvColumn7});
            this.olvVariableName.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvVariableName.DataSource = null;
            this.olvVariableName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvVariableName.EmptyListMsg = "";
            this.olvVariableName.EmptyListMsgFont = new System.Drawing.Font("Calibri", 9.5F);
            this.olvVariableName.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.olvVariableName.FullRowSelect = true;
            this.olvVariableName.GridLines = true;
            this.olvVariableName.GroupWithItemCountFormat = "";
            this.olvVariableName.GroupWithItemCountSingularFormat = "";
            this.olvVariableName.HideSelection = false;
            this.olvVariableName.Location = new System.Drawing.Point(0, 0);
            this.olvVariableName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvVariableName.Name = "olvVariableName";
            this.olvVariableName.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvVariableName.SelectedBackColor = System.Drawing.Color.LimeGreen;
            this.olvVariableName.SelectedForeColor = System.Drawing.Color.White;
            this.olvVariableName.ShowCommandMenuOnRightClick = true;
            this.olvVariableName.ShowGroups = false;
            this.olvVariableName.ShowImagesOnSubItems = true;
            this.olvVariableName.ShowItemToolTips = true;
            this.olvVariableName.Size = new System.Drawing.Size(354, 255);
            this.olvVariableName.TabIndex = 2;
            this.olvVariableName.UnfocusedSelectedBackColor = System.Drawing.Color.LimeGreen;
            this.olvVariableName.UnfocusedSelectedForeColor = System.Drawing.Color.White;
            this.olvVariableName.UseCellFormatEvents = true;
            this.olvVariableName.UseCompatibleStateImageBehavior = false;
            this.olvVariableName.UseFilterIndicator = true;
            this.olvVariableName.UseFiltering = true;
            this.olvVariableName.UseHotItem = true;
            this.olvVariableName.UseTranslucentHotItem = true;
            this.olvVariableName.View = System.Windows.Forms.View.Details;
            this.olvVariableName.CellEditFinished += new BrightIdeasSoftware.CellEditEventHandler(this.olvVariableName_CellEditFinished);
            this.olvVariableName.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.olvVariableName_CellEditFinishing);
            this.olvVariableName.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.olvVariableName_ItemSelectionChanged);
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "TimeIndex";
            this.olvColumn4.IsEditable = false;
            this.olvColumn4.Text = "Index";
            this.olvColumn4.Width = 78;
            // 
            // olvBehavior
            // 
            this.olvBehavior.AspectName = "Behavior";
            this.olvBehavior.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvBehavior.CellEditUseWholeCell = true;
            this.olvBehavior.IsTileViewColumn = true;
            this.olvBehavior.Text = "Behavior";
            this.olvBehavior.UseInitialLetterForGroup = true;
            this.olvBehavior.Width = 89;
            this.olvBehavior.WordWrap = true;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Constant";
            this.olvColumn6.Text = "Constant";
            this.olvColumn6.Width = 74;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "Multiplier";
            this.olvColumn7.Text = "Multiplier";
            this.olvColumn7.Width = 73;
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
            this.arrayGrid.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.arrayGrid.Location = new System.Drawing.Point(0, 25);
            this.arrayGrid.Name = "arrayGrid";
            this.arrayGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.arrayGrid.Size = new System.Drawing.Size(718, 525);
            this.arrayGrid.TabIndex = 26;
            this.arrayGrid.TabStop = true;
            this.arrayGrid.ToolTipText = "";
            // 
            // toolStripArray
            // 
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
            this.toolStripArray.Size = new System.Drawing.Size(718, 25);
            this.toolStripArray.TabIndex = 1;
            this.toolStripArray.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericSave_B_16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save temporarily. This will not save to source file";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(60, 22);
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
            this.tsSelectionMode.Size = new System.Drawing.Size(121, 25);
            this.tsSelectionMode.SelectedIndexChanged += new System.EventHandler(this.tsSelectionMode_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(46, 22);
            this.toolStripLabel1.Text = "Layout";
            // 
            // tsDataViewMode
            // 
            this.tsDataViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsDataViewMode.Items.AddRange(new object[] {
            "Serial",
            "Regular"});
            this.tsDataViewMode.Name = "tsDataViewMode";
            this.tsDataViewMode.Size = new System.Drawing.Size(121, 25);
            this.tsDataViewMode.SelectedIndexChanged += new System.EventHandler(this.tsDataViewMode_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsLabel
            // 
            this.tsLabel.Name = "tsLabel";
            this.tsLabel.Size = new System.Drawing.Size(44, 22);
            this.tsLabel.Text = "Empty";
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.excel_32;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(23, 22);
            this.btnExport.Text = "Export to a csv file";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GeodatabaseXMLRecordSetImport32;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(23, 22);
            this.btnImport.Text = "Import from an exsiting file";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // DataCubeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DataCubeEditor";
            this.Size = new System.Drawing.Size(1090, 550);
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
            ((System.ComponentModel.ISupportInitialize)(this.olvMatName)).EndInit();
            this.contextMenuStrip_matname.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvVariableName)).EndInit();
            this.toolStripArray.ResumeLayout(false);
            this.toolStripArray.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlLeft;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private BrightIdeasSoftware.DataListView olvMatName;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnClear;
        private BrightIdeasSoftware.DataListView olvVariableName;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvBehavior;
        private System.Windows.Forms.ToolStrip toolStripArray;
        private System.Windows.Forms.ToolStripButton btnSave;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
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
