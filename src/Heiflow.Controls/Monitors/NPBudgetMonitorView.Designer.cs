using Heiflow.Controls.WinForm.Controls;
namespace Heiflow.Controls.WinForm.Display
{
    partial class NPBudgetMonitorView
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
            this.tabControlExplorer = new System.Windows.Forms.TabControl();
            this.tabPageTree = new System.Windows.Forms.TabPage();
            this.treeView1 = new Heiflow.Controls.Tree.TreeViewAdv();
            this.nodeStateIcon1 = new Heiflow.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBox1 = new Heiflow.Controls.Tree.NodeControls.NodeTextBox();
            this.tabPageConfig = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.btnMassBudget = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageGraph = new System.Windows.Forms.TabPage();
            this.winChart1 = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.tabPageTable = new System.Windows.Forms.TabPage();
            this.olvMassBudget = new BrightIdeasSoftware.DataTreeListView();
            this.olvColumnItem = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnMass = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnPercent = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPageReport = new System.Windows.Forms.TabPage();
            this.textBoxReport = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlExplorer.SuspendLayout();
            this.tabPageTree.SuspendLayout();
            this.tabPageConfig.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPageGraph.SuspendLayout();
            this.tabPageTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvMassBudget)).BeginInit();
            this.tabPageReport.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.tabControlExplorer);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlMain);
            this.splitContainer1.Size = new System.Drawing.Size(1100, 640);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControlExplorer
            // 
            this.tabControlExplorer.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControlExplorer.Controls.Add(this.tabPageTree);
            this.tabControlExplorer.Controls.Add(this.tabPageConfig);
            this.tabControlExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlExplorer.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.tabControlExplorer.Location = new System.Drawing.Point(0, 27);
            this.tabControlExplorer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControlExplorer.Name = "tabControlExplorer";
            this.tabControlExplorer.SelectedIndex = 0;
            this.tabControlExplorer.Size = new System.Drawing.Size(260, 613);
            this.tabControlExplorer.TabIndex = 5;
            // 
            // tabPageTree
            // 
            this.tabPageTree.Controls.Add(this.treeView1);
            this.tabPageTree.Location = new System.Drawing.Point(4, 4);
            this.tabPageTree.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageTree.Name = "tabPageTree";
            this.tabPageTree.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageTree.Size = new System.Drawing.Size(252, 573);
            this.tabPageTree.TabIndex = 0;
            this.tabPageTree.Text = "Explorer";
            this.tabPageTree.UseVisualStyleBackColor = true;
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
            this.treeView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeView1.Model = null;
            this.treeView1.Name = "treeView1";
            this.treeView1.NodeControls.Add(this.nodeStateIcon1);
            this.treeView1.NodeControls.Add(this.nodeTextBox1);
            this.treeView1.OnVisibleOverride = null;
            this.treeView1.SelectedNode = null;
            this.treeView1.Size = new System.Drawing.Size(246, 565);
            this.treeView1.TabIndex = 2;
            this.treeView1.Text = "treeViewAdv1";
            // 
            // nodeStateIcon1
            // 
            this.nodeStateIcon1.LeftMargin = 1;
            this.nodeStateIcon1.ParentColumn = null;
            this.nodeStateIcon1.ScaleMode = Heiflow.Controls.Tree.ImageScaleMode.Clip;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Text";
            this.nodeTextBox1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = null;
            // 
            // tabPageConfig
            // 
            this.tabPageConfig.Controls.Add(this.propertyGrid1);
            this.tabPageConfig.Location = new System.Drawing.Point(4, 4);
            this.tabPageConfig.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageConfig.Name = "tabPageConfig";
            this.tabPageConfig.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageConfig.Size = new System.Drawing.Size(252, 573);
            this.tabPageConfig.TabIndex = 1;
            this.tabPageConfig.Text = "Config";
            this.tabPageConfig.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 4);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(246, 565);
            this.propertyGrid1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad,
            this.btnMassBudget,
            this.btnClear});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(260, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLoad
            // 
            this.btnLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoad.Image = global::Heiflow.Controls.WinForm.Properties.Resources.Load24;
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(24, 24);
            this.btnLoad.Text = "Load sz_N_budget.csv / sz_P_budget.csv";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnMassBudget
            // 
            this.btnMassBudget.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMassBudget.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GraphHistogram32;
            this.btnMassBudget.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMassBudget.Name = "btnMassBudget";
            this.btnMassBudget.Size = new System.Drawing.Size(24, 24);
            this.btnMassBudget.Text = "Mass Budget";
            this.btnMassBudget.Click += new System.EventHandler(this.btnMassBudget_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_history_clear_9334;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.Text = "Clear cache";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageGraph);
            this.tabControlMain.Controls.Add(this.tabPageTable);
            this.tabControlMain.Controls.Add(this.tabPageReport);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(836, 640);
            this.tabControlMain.TabIndex = 1;
            // 
            // tabPageGraph
            // 
            this.tabPageGraph.Controls.Add(this.winChart1);
            this.tabPageGraph.Location = new System.Drawing.Point(4, 30);
            this.tabPageGraph.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageGraph.Name = "tabPageGraph";
            this.tabPageGraph.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageGraph.Size = new System.Drawing.Size(828, 606);
            this.tabPageGraph.TabIndex = 0;
            this.tabPageGraph.Text = "Time Series";
            this.tabPageGraph.UseVisualStyleBackColor = true;
            // 
            // winChart1
            // 
            this.winChart1.BackColor = System.Drawing.SystemColors.Control;
            this.winChart1.ClearExistesSeries = true;
            this.winChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart1.Location = new System.Drawing.Point(3, 4);
            this.winChart1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.winChart1.Name = "winChart1";
            this.winChart1.ShowStatPanel = true;
            this.winChart1.Size = new System.Drawing.Size(822, 598);
            this.winChart1.TabIndex = 0;
            // 
            // tabPageTable
            // 
            this.tabPageTable.Controls.Add(this.olvMassBudget);
            this.tabPageTable.Location = new System.Drawing.Point(4, 30);
            this.tabPageTable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageTable.Name = "tabPageTable";
            this.tabPageTable.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageTable.Size = new System.Drawing.Size(828, 606);
            this.tabPageTable.TabIndex = 1;
            this.tabPageTable.Text = "Mass Budget Table";
            this.tabPageTable.UseVisualStyleBackColor = true;
            // 
            // olvMassBudget
            // 
            this.olvMassBudget.AllColumns.Add(this.olvColumnItem);
            this.olvMassBudget.AllColumns.Add(this.olvColumnMass);
            this.olvMassBudget.AllColumns.Add(this.olvColumnPercent);
            this.olvMassBudget.AutoGenerateColumns = false;
            this.olvMassBudget.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMassBudget.CellEditUseWholeCell = false;
            this.olvMassBudget.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnItem,
            this.olvColumnMass,
            this.olvColumnPercent});
            this.olvMassBudget.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMassBudget.DataSource = null;
            this.olvMassBudget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMassBudget.HideSelection = false;
            this.olvMassBudget.KeyAspectName = "ID";
            this.olvMassBudget.Location = new System.Drawing.Point(3, 4);
            this.olvMassBudget.Margin = new System.Windows.Forms.Padding(4);
            this.olvMassBudget.Name = "olvMassBudget";
            this.olvMassBudget.ParentKeyAspectName = "ParentID";
            this.olvMassBudget.RootKeyValueString = "";
            this.olvMassBudget.ShowGroups = false;
            this.olvMassBudget.ShowKeyColumns = false;
            this.olvMassBudget.Size = new System.Drawing.Size(822, 598);
            this.olvMassBudget.TabIndex = 4;
            this.olvMassBudget.UseCompatibleStateImageBehavior = false;
            this.olvMassBudget.UseFilterIndicator = true;
            this.olvMassBudget.UseFiltering = true;
            this.olvMassBudget.View = System.Windows.Forms.View.Details;
            this.olvMassBudget.VirtualMode = true;
            // 
            // olvColumnItem
            // 
            this.olvColumnItem.AspectName = "Item";
            this.olvColumnItem.Text = "Item";
            this.olvColumnItem.Width = 260;
            // 
            // olvColumnMass
            // 
            this.olvColumnMass.AspectName = "Mass";
            this.olvColumnMass.AspectToStringFormat = "{0:E4}";
            this.olvColumnMass.Text = "Mass";
            this.olvColumnMass.Width = 200;
            // 
            // olvColumnPercent
            // 
            this.olvColumnPercent.AspectName = "Percentage";
            this.olvColumnPercent.AspectToStringFormat = "{0:0.00}";
            this.olvColumnPercent.Text = "Percent of Total In (%)";
            this.olvColumnPercent.Width = 200;
            // 
            // tabPageReport
            // 
            this.tabPageReport.Controls.Add(this.textBoxReport);
            this.tabPageReport.Location = new System.Drawing.Point(4, 30);
            this.tabPageReport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageReport.Name = "tabPageReport";
            this.tabPageReport.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageReport.Size = new System.Drawing.Size(828, 606);
            this.tabPageReport.TabIndex = 2;
            this.tabPageReport.Text = "Mass Budget Report";
            this.tabPageReport.UseVisualStyleBackColor = true;
            // 
            // textBoxReport
            // 
            this.textBoxReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxReport.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBoxReport.Location = new System.Drawing.Point(3, 4);
            this.textBoxReport.Multiline = true;
            this.textBoxReport.Name = "textBoxReport";
            this.textBoxReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxReport.Size = new System.Drawing.Size(822, 598);
            this.textBoxReport.TabIndex = 0;
            this.textBoxReport.WordWrap = false;
            // 
            // NPBudgetMonitorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "NPBudgetMonitorView";
            this.Size = new System.Drawing.Size(1100, 640);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlExplorer.ResumeLayout(false);
            this.tabPageTree.ResumeLayout(false);
            this.tabPageConfig.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabPageGraph.ResumeLayout(false);
            this.tabPageTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvMassBudget)).EndInit();
            this.tabPageReport.ResumeLayout(false);
            this.tabPageReport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlExplorer;
        private System.Windows.Forms.TabPage tabPageTree;
        private System.Windows.Forms.TabPage tabPageConfig;
        private Tree.TreeViewAdv treeView1;
        private Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private Tree.NodeControls.NodeTextBox nodeTextBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripButton btnMassBudget;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageGraph;
        private System.Windows.Forms.TabPage tabPageTable;
        private System.Windows.Forms.TabPage tabPageReport;
        private WinChart winChart1;
        private BrightIdeasSoftware.DataTreeListView olvMassBudget;
        private BrightIdeasSoftware.OLVColumn olvColumnItem;
        private BrightIdeasSoftware.OLVColumn olvColumnMass;
        private BrightIdeasSoftware.OLVColumn olvColumnPercent;
        private System.Windows.Forms.TextBox textBoxReport;
    }
}
