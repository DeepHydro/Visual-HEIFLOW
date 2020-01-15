using Heiflow.Controls.WinForm.Controls;
namespace Heiflow.Controls.WinForm.Toolbox
{
    partial class ModelToolManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelToolManager));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlCmd = new System.Windows.Forms.TabControl();
            this.tabPageCommand = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRun = new System.Windows.Forms.ToolStripButton();
            this.tabPageFigure = new System.Windows.Forms.TabPage();
            this.winChart_timeseries = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.tabPageTable = new System.Windows.Forms.TabPage();
            this.dataGridEx1 = new Heiflow.Controls.WinForm.Controls.DataCubeGrid();
            this.tabPageOutput = new System.Windows.Forms.TabPage();
            this.txt_msg = new System.Windows.Forms.RichTextBox();
            this.tabControlLeft = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.olvMatName = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextMenuStrip_matname = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menu_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_remove = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btn_open_dcx = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.olvVariableName = new BrightIdeasSoftware.DataListView();
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvVariable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeView1 = new Heiflow.Controls.Tree.TreeViewAdv();
            this._nodeStateIcon = new Heiflow.Controls.Tree.NodeControls.NodeStateIcon();
            this._nodeTextBox = new Heiflow.Controls.Tree.NodeControls.NodeTextBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlCmd.SuspendLayout();
            this.tabPageCommand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPageFigure.SuspendLayout();
            this.tabPageTable.SuspendLayout();
            this.tabPageOutput.SuspendLayout();
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
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControlCmd);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlLeft);
            this.splitContainer1.Size = new System.Drawing.Size(1065, 669);
            this.splitContainer1.SplitterDistance = 707;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControlCmd
            // 
            this.tabControlCmd.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControlCmd.Controls.Add(this.tabPageCommand);
            this.tabControlCmd.Controls.Add(this.tabPageFigure);
            this.tabControlCmd.Controls.Add(this.tabPageTable);
            this.tabControlCmd.Controls.Add(this.tabPageOutput);
            this.tabControlCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCmd.Location = new System.Drawing.Point(0, 0);
            this.tabControlCmd.Name = "tabControlCmd";
            this.tabControlCmd.SelectedIndex = 0;
            this.tabControlCmd.Size = new System.Drawing.Size(707, 669);
            this.tabControlCmd.TabIndex = 0;
            // 
            // tabPageCommand
            // 
            this.tabPageCommand.Controls.Add(this.splitContainer2);
            this.tabPageCommand.Controls.Add(this.toolStrip1);
            this.tabPageCommand.Location = new System.Drawing.Point(4, 4);
            this.tabPageCommand.Name = "tabPageCommand";
            this.tabPageCommand.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCommand.Size = new System.Drawing.Size(699, 636);
            this.tabPageCommand.TabIndex = 2;
            this.tabPageCommand.Text = "Command";
            this.tabPageCommand.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 30);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer2.Size = new System.Drawing.Size(693, 603);
            this.splitContainer2.SplitterDistance = 448;
            this.splitContainer2.TabIndex = 2;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(693, 448);
            this.propertyGrid1.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(693, 151);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRun});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(693, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRun
            // 
            this.btnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRun.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericBlueRightArrowNoTail32;
            this.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(24, 24);
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // tabPageFigure
            // 
            this.tabPageFigure.Controls.Add(this.winChart_timeseries);
            this.tabPageFigure.Location = new System.Drawing.Point(4, 4);
            this.tabPageFigure.Name = "tabPageFigure";
            this.tabPageFigure.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFigure.Size = new System.Drawing.Size(699, 636);
            this.tabPageFigure.TabIndex = 0;
            this.tabPageFigure.Text = "Figure";
            this.tabPageFigure.UseVisualStyleBackColor = true;
            // 
            // winChart_timeseries
            // 
            this.winChart_timeseries.BackColor = System.Drawing.SystemColors.Control;
            this.winChart_timeseries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_timeseries.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart_timeseries.Location = new System.Drawing.Point(3, 3);
            this.winChart_timeseries.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChart_timeseries.Name = "winChart_timeseries";
            this.winChart_timeseries.ShowStatPanel = false;
            this.winChart_timeseries.Size = new System.Drawing.Size(693, 630);
            this.winChart_timeseries.TabIndex = 8;
            // 
            // tabPageTable
            // 
            this.tabPageTable.Controls.Add(this.dataGridEx1);
            this.tabPageTable.Location = new System.Drawing.Point(4, 4);
            this.tabPageTable.Name = "tabPageTable";
            this.tabPageTable.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTable.Size = new System.Drawing.Size(699, 640);
            this.tabPageTable.TabIndex = 1;
            this.tabPageTable.Text = "Table";
            this.tabPageTable.UseVisualStyleBackColor = true;
            // 
            // dataGridEx1
            // 
            this.dataGridEx1.DataObjectName = "";
            this.dataGridEx1.DataTable = null;
            this.dataGridEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridEx1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.dataGridEx1.Location = new System.Drawing.Point(3, 3);
            this.dataGridEx1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridEx1.Name = "dataGridEx1";
            this.dataGridEx1.ShowImport = false;
            this.dataGridEx1.ShowSave2Excel = false;
            this.dataGridEx1.ShowSaveButton = false;
            this.dataGridEx1.Size = new System.Drawing.Size(693, 634);
            this.dataGridEx1.TabIndex = 0;
            // 
            // tabPageOutput
            // 
            this.tabPageOutput.Controls.Add(this.txt_msg);
            this.tabPageOutput.Location = new System.Drawing.Point(4, 4);
            this.tabPageOutput.Name = "tabPageOutput";
            this.tabPageOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOutput.Size = new System.Drawing.Size(699, 640);
            this.tabPageOutput.TabIndex = 3;
            this.tabPageOutput.Text = "Output";
            this.tabPageOutput.UseVisualStyleBackColor = true;
            // 
            // txt_msg
            // 
            this.txt_msg.BackColor = System.Drawing.SystemColors.Control;
            this.txt_msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_msg.Location = new System.Drawing.Point(3, 3);
            this.txt_msg.Name = "txt_msg";
            this.txt_msg.Size = new System.Drawing.Size(693, 634);
            this.txt_msg.TabIndex = 1;
            this.txt_msg.Text = "";
            // 
            // tabControlLeft
            // 
            this.tabControlLeft.Controls.Add(this.tabPage5);
            this.tabControlLeft.Controls.Add(this.tabPage1);
            this.tabControlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLeft.Location = new System.Drawing.Point(0, 0);
            this.tabControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControlLeft.Name = "tabControlLeft";
            this.tabControlLeft.SelectedIndex = 0;
            this.tabControlLeft.Size = new System.Drawing.Size(354, 669);
            this.tabControlLeft.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer3);
            this.tabPage5.Location = new System.Drawing.Point(4, 29);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(346, 636);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Workspace";
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
            this.splitContainer3.Size = new System.Drawing.Size(340, 630);
            this.splitContainer3.SplitterDistance = 410;
            this.splitContainer3.TabIndex = 1;
            // 
            // olvMatName
            // 
            this.olvMatName.AllColumns.Add(this.olvColumn1);
            this.olvMatName.AllColumns.Add(this.olvColumn2);
            this.olvMatName.AllowColumnReorder = true;
            this.olvMatName.AllowDrop = true;
            this.olvMatName.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMatName.CellEditUseWholeCell = false;
            this.olvMatName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2});
            this.olvMatName.ContextMenuStrip = this.contextMenuStrip_matname;
            this.olvMatName.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMatName.DataSource = null;
            this.olvMatName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMatName.EmptyListMsg = "";
            this.olvMatName.EmptyListMsgFont = new System.Drawing.Font("Segoe UI", 9F);
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
            this.olvMatName.Size = new System.Drawing.Size(340, 383);
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
            this.olvMatName.CellEditFinished += new BrightIdeasSoftware.CellEditEventHandler(this.olvMatName_CellEditFinished);
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
            this.olvColumn2.Width = 100;
            // 
            // contextMenuStrip_matname
            // 
            this.contextMenuStrip_matname.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_matname.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Open,
            this.menu_remove,
            this.menu_SaveAs,
            this.toolStripSeparator1,
            this.menu_Clear});
            this.contextMenuStrip_matname.Name = "contextMenuStrip_matname";
            this.contextMenuStrip_matname.Size = new System.Drawing.Size(205, 114);
            // 
            // menu_Open
            // 
            this.menu_Open.Name = "menu_Open";
            this.menu_Open.Size = new System.Drawing.Size(204, 26);
            this.menu_Open.Text = "View Values...";
            this.menu_Open.Click += new System.EventHandler(this.menu_Open_Click);
            // 
            // menu_remove
            // 
            this.menu_remove.Name = "menu_remove";
            this.menu_remove.Size = new System.Drawing.Size(204, 26);
            this.menu_remove.Text = "Remove";
            this.menu_remove.Click += new System.EventHandler(this.menu_Remove_Click);
            // 
            // menu_SaveAs
            // 
            this.menu_SaveAs.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericSave_B_16;
            this.menu_SaveAs.Name = "menu_SaveAs";
            this.menu_SaveAs.Size = new System.Drawing.Size(204, 26);
            this.menu_SaveAs.Text = "Save As...";
            this.menu_SaveAs.Click += new System.EventHandler(this.menu_SaveAs_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
            // 
            // menu_Clear
            // 
            this.menu_Clear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_edit_clear_15273;
            this.menu_Clear.Name = "menu_Clear";
            this.menu_Clear.Size = new System.Drawing.Size(204, 26);
            this.menu_Clear.Text = "Clear Workspace";
            this.menu_Clear.Click += new System.EventHandler(this.menu_Clear_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_open_dcx,
            this.btnClear});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(340, 27);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btn_open_dcx
            // 
            this.btn_open_dcx.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_open_dcx.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericOpen_B_32;
            this.btn_open_dcx.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_open_dcx.Name = "btn_open_dcx";
            this.btn_open_dcx.Size = new System.Drawing.Size(24, 24);
            this.btn_open_dcx.Text = "Open a dcx file";
            this.btn_open_dcx.Click += new System.EventHandler(this.btn_open_dcx_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_edit_clear_15273;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.Text = "Clear workspace";
            this.btnClear.Click += new System.EventHandler(this.menu_Clear_Click);
            // 
            // olvVariableName
            // 
            this.olvVariableName.AllColumns.Add(this.olvColumn4);
            this.olvVariableName.AllColumns.Add(this.olvVariable);
            this.olvVariableName.AllColumns.Add(this.olvColumn5);
            this.olvVariableName.AllColumns.Add(this.olvColumn6);
            this.olvVariableName.AllColumns.Add(this.olvColumn3);
            this.olvVariableName.AllowColumnReorder = true;
            this.olvVariableName.AllowDrop = true;
            this.olvVariableName.CellEditUseWholeCell = false;
            this.olvVariableName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn4,
            this.olvVariable,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn3});
            this.olvVariableName.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvVariableName.DataSource = null;
            this.olvVariableName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvVariableName.EmptyListMsg = "";
            this.olvVariableName.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvVariableName.FullRowSelect = true;
            this.olvVariableName.GridLines = true;
            this.olvVariableName.GroupWithItemCountFormat = "";
            this.olvVariableName.GroupWithItemCountSingularFormat = "";
            this.olvVariableName.HideSelection = false;
            this.olvVariableName.Location = new System.Drawing.Point(0, 0);
            this.olvVariableName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvVariableName.Name = "olvVariableName";
            this.olvVariableName.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvVariableName.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvVariableName.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvVariableName.ShowCommandMenuOnRightClick = true;
            this.olvVariableName.ShowGroups = false;
            this.olvVariableName.ShowImagesOnSubItems = true;
            this.olvVariableName.ShowItemToolTips = true;
            this.olvVariableName.Size = new System.Drawing.Size(340, 216);
            this.olvVariableName.TabIndex = 2;
            this.olvVariableName.UseCellFormatEvents = true;
            this.olvVariableName.UseCompatibleStateImageBehavior = false;
            this.olvVariableName.UseFilterIndicator = true;
            this.olvVariableName.UseFiltering = true;
            this.olvVariableName.UseHotItem = true;
            this.olvVariableName.UseTranslucentHotItem = true;
            this.olvVariableName.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "Index";
            this.olvColumn4.Text = "Index";
            this.olvColumn4.Width = 97;
            // 
            // olvVariable
            // 
            this.olvVariable.AspectName = "Variable";
            this.olvVariable.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvVariable.IsTileViewColumn = true;
            this.olvVariable.Text = "Variable";
            this.olvVariable.UseInitialLetterForGroup = true;
            this.olvVariable.Width = 100;
            this.olvVariable.WordWrap = true;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Size";
            this.olvColumn5.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn5.IsTileViewColumn = true;
            this.olvColumn5.Text = "Size";
            this.olvColumn5.Width = 100;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Max";
            this.olvColumn6.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn6.IsTileViewColumn = true;
            this.olvColumn6.Text = "Max";
            this.olvColumn6.Width = 100;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Min";
            this.olvColumn3.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn3.Text = "Min";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(346, 640);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tools";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.BackColor2 = System.Drawing.SystemColors.Window;
            this.treeView1.BackgroundPaintMode = Heiflow.Controls.Tree.BackgroundPaintMode.Default;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.DefaultToolTipProvider = null;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeView1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.treeView1.HighlightColorActive = System.Drawing.SystemColors.Highlight;
            this.treeView1.HighlightColorInactive = System.Drawing.SystemColors.InactiveBorder;
            this.treeView1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeView1.Location = new System.Drawing.Point(3, 4);
            this.treeView1.Model = null;
            this.treeView1.Name = "treeView1";
            this.treeView1.NodeControls.Add(this._nodeStateIcon);
            this.treeView1.NodeControls.Add(this._nodeTextBox);
            this.treeView1.OnVisibleOverride = null;
            this.treeView1.SelectedNode = null;
            this.treeView1.Size = new System.Drawing.Size(340, 632);
            this.treeView1.TabIndex = 1;
            this.treeView1.Text = "treeViewAdv1";
            // 
            // _nodeStateIcon
            // 
            this._nodeStateIcon.LeftMargin = 1;
            this._nodeStateIcon.ParentColumn = null;
            this._nodeStateIcon.ScaleMode = Heiflow.Controls.Tree.ImageScaleMode.Clip;
            // 
            // _nodeTextBox
            // 
            this._nodeTextBox.IncrementalSearchEnabled = true;
            this._nodeTextBox.LeftMargin = 3;
            this._nodeTextBox.ParentColumn = null;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "toolbox32.png");
            this.imageList1.Images.SetKeyName(1, "toolbox16.png");
            // 
            // ModelToolManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ModelToolManager";
            this.Size = new System.Drawing.Size(1065, 669);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlCmd.ResumeLayout(false);
            this.tabPageCommand.ResumeLayout(false);
            this.tabPageCommand.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPageFigure.ResumeLayout(false);
            this.tabPageTable.ResumeLayout(false);
            this.tabPageOutput.ResumeLayout(false);
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
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlLeft;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControlCmd;
        private System.Windows.Forms.TabPage tabPageFigure;
        private System.Windows.Forms.TabPage tabPageTable;
        private Controls.WinChart winChart_timeseries;
        private Tree.TreeViewAdv treeView1;
        private Tree.NodeControls.NodeStateIcon _nodeStateIcon;
        private Tree.NodeControls.NodeTextBox _nodeTextBox;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPageCommand;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRun;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TabPage tabPageOutput;
        private System.Windows.Forms.RichTextBox txt_msg;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private BrightIdeasSoftware.DataListView olvVariableName;
        private BrightIdeasSoftware.OLVColumn olvVariable;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_matname;
        private System.Windows.Forms.ToolStripMenuItem menu_SaveAs;
        private System.Windows.Forms.ToolStripMenuItem menu_Clear;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private System.Windows.Forms.ToolStripMenuItem menu_Open;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menu_remove;
        private BrightIdeasSoftware.DataListView olvMatName;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btn_open_dcx;
        private DataCubeGrid dataGridEx1;
        private System.Windows.Forms.ToolStripButton btnClear;
    }
}
