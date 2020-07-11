using Heiflow.Controls.WinForm.Controls;
namespace Heiflow.Controls.WinForm.Display
{
    partial class StateMonitor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StateMonitor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.treeView1 = new Heiflow.Controls.Tree.TreeViewAdv();
            this.nodeStateIcon1 = new Heiflow.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBox1 = new Heiflow.Controls.Tree.NodeControls.NodeTextBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnClearCache = new System.Windows.Forms.ToolStripButton();
            this.tabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage_Graph = new System.Windows.Forms.TabPage();
            this.winChart1 = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.tabPage_Report = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.olvDataTree = new BrightIdeasSoftware.DataTreeListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lak_slow = new System.Windows.Forms.TextBox();
            this.sfr_slow = new System.Windows.Forms.TextBox();
            this.sat_s2g = new System.Windows.Forms.TextBox();
            this.uzf_recharge = new System.Windows.Forms.TextBox();
            this.uzf_et = new System.Windows.Forms.TextBox();
            this.sat_et = new System.Windows.Forms.TextBox();
            this.sat_pr = new System.Windows.Forms.TextBox();
            this.sat_g2s = new System.Windows.Forms.TextBox();
            this.sat_ds = new System.Windows.Forms.TextBox();
            this.uzf_ds = new System.Windows.Forms.TextBox();
            this.sw_in = new System.Windows.Forms.TextBox();
            this.sat_in = new System.Windows.Forms.TextBox();
            this.sat_out = new System.Windows.Forms.TextBox();
            this.sw_out = new System.Windows.Forms.TextBox();
            this.sat_gw2sz = new System.Windows.Forms.TextBox();
            this.sz_Percolation = new System.Windows.Forms.TextBox();
            this.sz_ds = new System.Windows.Forms.TextBox();
            this.lak_dun = new System.Windows.Forms.TextBox();
            this.sfr_dun = new System.Windows.Forms.TextBox();
            this.uz_error = new System.Windows.Forms.TextBox();
            this.sat_error = new System.Windows.Forms.TextBox();
            this.soil_error = new System.Windows.Forms.TextBox();
            this.sz_et = new System.Windows.Forms.TextBox();
            this.total_error = new System.Windows.Forms.TextBox();
            this.et = new System.Windows.Forms.TextBox();
            this.ds = new System.Windows.Forms.TextBox();
            this.ppt = new System.Windows.Forms.TextBox();
            this.sf_et = new System.Windows.Forms.TextBox();
            this.sfr_ds = new System.Windows.Forms.TextBox();
            this.lak_et = new System.Windows.Forms.TextBox();
            this.lak_ds = new System.Windows.Forms.TextBox();
            this.sw_ds = new System.Windows.Forms.TextBox();
            this.div = new System.Windows.Forms.TextBox();
            this.canal_ds = new System.Windows.Forms.TextBox();
            this.canal_et = new System.Windows.Forms.TextBox();
            this.sfr_et = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabControl_Main.SuspendLayout();
            this.tabPage_Graph.SuspendLayout();
            this.tabPage_Report.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDataTree)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.splitContainer1.Panel1.Controls.Add(this.tabControl3);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl_Main);
            this.splitContainer1.Size = new System.Drawing.Size(1334, 713);
            this.splitContainer1.SplitterDistance = 219;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl3
            // 
            this.tabControl3.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl3.Controls.Add(this.tabPage5);
            this.tabControl3.Controls.Add(this.tabPage6);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.tabControl3.Location = new System.Drawing.Point(0, 27);
            this.tabControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(219, 686);
            this.tabControl3.TabIndex = 5;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.treeView1);
            this.tabPage5.Location = new System.Drawing.Point(4, 4);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage5.Size = new System.Drawing.Size(211, 654);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Explorer";
            this.tabPage5.UseVisualStyleBackColor = true;
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
            this.treeView1.Size = new System.Drawing.Size(205, 646);
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
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.propertyGrid1);
            this.tabPage6.Location = new System.Drawing.Point(4, 4);
            this.tabPage6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage6.Size = new System.Drawing.Size(211, 654);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "Config";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 4);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(205, 646);
            this.propertyGrid1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad,
            this.btnOpen,
            this.btnClearCache});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(219, 27);
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
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::Heiflow.Controls.WinForm.Properties.Resources.HostedServicesFolderOpenState32;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(24, 24);
            this.btnOpen.Text = "Open...";
            // 
            // btnClearCache
            // 
            this.btnClearCache.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClearCache.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_history_clear_9334;
            this.btnClearCache.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(24, 24);
            this.btnClearCache.Text = "Clear cache";
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // tabControl_Main
            // 
            this.tabControl_Main.Controls.Add(this.tabPage_Graph);
            this.tabControl_Main.Controls.Add(this.tabPage_Report);
            this.tabControl_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Main.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.tabControl_Main.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Main.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.SelectedIndex = 0;
            this.tabControl_Main.Size = new System.Drawing.Size(1111, 713);
            this.tabControl_Main.TabIndex = 1;
            // 
            // tabPage_Graph
            // 
            this.tabPage_Graph.Controls.Add(this.winChart1);
            this.tabPage_Graph.Location = new System.Drawing.Point(4, 28);
            this.tabPage_Graph.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage_Graph.Name = "tabPage_Graph";
            this.tabPage_Graph.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage_Graph.Size = new System.Drawing.Size(1103, 681);
            this.tabPage_Graph.TabIndex = 0;
            this.tabPage_Graph.Text = "Time Series";
            this.tabPage_Graph.UseVisualStyleBackColor = true;
            // 
            // winChart1
            // 
            this.winChart1.BackColor = System.Drawing.SystemColors.Control;
            this.winChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart1.Location = new System.Drawing.Point(3, 4);
            this.winChart1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.winChart1.Name = "winChart1";
            this.winChart1.ShowStatPanel = true;
            this.winChart1.Size = new System.Drawing.Size(1097, 673);
            this.winChart1.TabIndex = 0;
            // 
            // tabPage_Report
            // 
            this.tabPage_Report.Controls.Add(this.tabControl2);
            this.tabPage_Report.Location = new System.Drawing.Point(4, 28);
            this.tabPage_Report.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage_Report.Name = "tabPage_Report";
            this.tabPage_Report.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage_Report.Size = new System.Drawing.Size(1103, 681);
            this.tabPage_Report.TabIndex = 1;
            this.tabPage_Report.Text = "Statistics";
            this.tabPage_Report.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.tabControl2.Location = new System.Drawing.Point(3, 4);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1097, 673);
            this.tabControl2.TabIndex = 3;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.olvDataTree);
            this.tabPage3.Location = new System.Drawing.Point(4, 31);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage3.Size = new System.Drawing.Size(1089, 638);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Entire Region Budgets";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // olvDataTree
            // 
            this.olvDataTree.AllColumns.Add(this.olvColumn1);
            this.olvDataTree.AllColumns.Add(this.olvColumn2);
            this.olvDataTree.AllColumns.Add(this.olvColumn3);
            this.olvDataTree.AutoGenerateColumns = false;
            this.olvDataTree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvDataTree.CellEditUseWholeCell = false;
            this.olvDataTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.olvDataTree.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvDataTree.DataSource = null;
            this.olvDataTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvDataTree.KeyAspectName = "ID";
            this.olvDataTree.Location = new System.Drawing.Point(3, 4);
            this.olvDataTree.Margin = new System.Windows.Forms.Padding(4);
            this.olvDataTree.Name = "olvDataTree";
            this.olvDataTree.ParentKeyAspectName = "ParentID";
            this.olvDataTree.RootKeyValueString = "";
            this.olvDataTree.ShowGroups = false;
            this.olvDataTree.ShowKeyColumns = false;
            this.olvDataTree.Size = new System.Drawing.Size(1083, 630);
            this.olvDataTree.TabIndex = 4;
            this.olvDataTree.UseCompatibleStateImageBehavior = false;
            this.olvDataTree.UseFilterIndicator = true;
            this.olvDataTree.UseFiltering = true;
            this.olvDataTree.View = System.Windows.Forms.View.Details;
            this.olvDataTree.VirtualMode = true;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Item";
            this.olvColumn1.Text = "Item";
            this.olvColumn1.Width = 154;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Volumetric_Flow";
            this.olvColumn2.AspectToStringFormat = "{0:E}";
            this.olvColumn2.Text = "Volumetric Flow (cubic meter)";
            this.olvColumn2.Width = 214;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Water_Depth";
            this.olvColumn3.AspectToStringFormat = "";
            this.olvColumn3.Text = "Water Depth(mm)";
            this.olvColumn3.Width = 210;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.lak_slow);
            this.tabPage1.Controls.Add(this.sfr_slow);
            this.tabPage1.Controls.Add(this.sat_s2g);
            this.tabPage1.Controls.Add(this.uzf_recharge);
            this.tabPage1.Controls.Add(this.uzf_et);
            this.tabPage1.Controls.Add(this.sat_et);
            this.tabPage1.Controls.Add(this.sat_pr);
            this.tabPage1.Controls.Add(this.sat_g2s);
            this.tabPage1.Controls.Add(this.sat_ds);
            this.tabPage1.Controls.Add(this.uzf_ds);
            this.tabPage1.Controls.Add(this.sw_in);
            this.tabPage1.Controls.Add(this.sat_in);
            this.tabPage1.Controls.Add(this.sat_out);
            this.tabPage1.Controls.Add(this.sw_out);
            this.tabPage1.Controls.Add(this.sat_gw2sz);
            this.tabPage1.Controls.Add(this.sz_Percolation);
            this.tabPage1.Controls.Add(this.sz_ds);
            this.tabPage1.Controls.Add(this.lak_dun);
            this.tabPage1.Controls.Add(this.sfr_dun);
            this.tabPage1.Controls.Add(this.uz_error);
            this.tabPage1.Controls.Add(this.sat_error);
            this.tabPage1.Controls.Add(this.soil_error);
            this.tabPage1.Controls.Add(this.sz_et);
            this.tabPage1.Controls.Add(this.total_error);
            this.tabPage1.Controls.Add(this.et);
            this.tabPage1.Controls.Add(this.ds);
            this.tabPage1.Controls.Add(this.ppt);
            this.tabPage1.Controls.Add(this.sf_et);
            this.tabPage1.Controls.Add(this.sfr_ds);
            this.tabPage1.Controls.Add(this.lak_et);
            this.tabPage1.Controls.Add(this.lak_ds);
            this.tabPage1.Controls.Add(this.sw_ds);
            this.tabPage1.Controls.Add(this.div);
            this.tabPage1.Controls.Add(this.canal_ds);
            this.tabPage1.Controls.Add(this.canal_et);
            this.tabPage1.Controls.Add(this.sfr_et);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 31);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1089, 638);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Zonal Budgets";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(912, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(173, 19);
            this.label6.TabIndex = 3;
            this.label6.Text = "Percent Discrepancy (PD)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Info;
            this.label5.Font = new System.Drawing.Font("Calibri", 10F);
            this.label5.Location = new System.Drawing.Point(378, 449);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(211, 21);
            this.label5.TabIndex = 2;
            this.label5.Text = "Unsaturated Zone Budget PD";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Info;
            this.label4.Font = new System.Drawing.Font("Calibri", 10F);
            this.label4.Location = new System.Drawing.Point(451, 617);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 21);
            this.label4.TabIndex = 2;
            this.label4.Text = "Saturated Zone PD";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Info;
            this.label3.Font = new System.Drawing.Font("Calibri", 10F);
            this.label3.Location = new System.Drawing.Point(356, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(234, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Surface and Soil Zone Budget PD";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Info;
            this.label2.Font = new System.Drawing.Font("Calibri", 10F);
            this.label2.Location = new System.Drawing.Point(471, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Total Budget PD";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1009, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Unit: mm";
            // 
            // lak_slow
            // 
            this.lak_slow.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lak_slow.Font = new System.Drawing.Font("Calibri", 10F);
            this.lak_slow.Location = new System.Drawing.Point(284, 161);
            this.lak_slow.Name = "lak_slow";
            this.lak_slow.Size = new System.Drawing.Size(45, 28);
            this.lak_slow.TabIndex = 1;
            this.lak_slow.Text = "0";
            // 
            // sfr_slow
            // 
            this.sfr_slow.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sfr_slow.Font = new System.Drawing.Font("Calibri", 10F);
            this.sfr_slow.Location = new System.Drawing.Point(453, 158);
            this.sfr_slow.Name = "sfr_slow";
            this.sfr_slow.Size = new System.Drawing.Size(51, 28);
            this.sfr_slow.TabIndex = 1;
            this.sfr_slow.Text = "0";
            // 
            // sat_s2g
            // 
            this.sat_s2g.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_s2g.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_s2g.Location = new System.Drawing.Point(491, 508);
            this.sat_s2g.Name = "sat_s2g";
            this.sat_s2g.Size = new System.Drawing.Size(53, 30);
            this.sat_s2g.TabIndex = 1;
            this.sat_s2g.Text = "0";
            // 
            // uzf_recharge
            // 
            this.uzf_recharge.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.uzf_recharge.Font = new System.Drawing.Font("Calibri", 11F);
            this.uzf_recharge.Location = new System.Drawing.Point(255, 442);
            this.uzf_recharge.Name = "uzf_recharge";
            this.uzf_recharge.Size = new System.Drawing.Size(53, 30);
            this.uzf_recharge.TabIndex = 1;
            this.uzf_recharge.Text = "0";
            // 
            // uzf_et
            // 
            this.uzf_et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.uzf_et.Font = new System.Drawing.Font("Calibri", 11F);
            this.uzf_et.Location = new System.Drawing.Point(817, 373);
            this.uzf_et.Name = "uzf_et";
            this.uzf_et.Size = new System.Drawing.Size(53, 30);
            this.uzf_et.TabIndex = 1;
            this.uzf_et.Text = "0";
            // 
            // sat_et
            // 
            this.sat_et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_et.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_et.Location = new System.Drawing.Point(765, 515);
            this.sat_et.Name = "sat_et";
            this.sat_et.Size = new System.Drawing.Size(53, 30);
            this.sat_et.TabIndex = 1;
            this.sat_et.Text = "0";
            // 
            // sat_pr
            // 
            this.sat_pr.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_pr.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_pr.Location = new System.Drawing.Point(672, 82);
            this.sat_pr.Name = "sat_pr";
            this.sat_pr.Size = new System.Drawing.Size(53, 30);
            this.sat_pr.TabIndex = 1;
            this.sat_pr.Text = "0";
            // 
            // sat_g2s
            // 
            this.sat_g2s.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_g2s.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_g2s.Location = new System.Drawing.Point(491, 536);
            this.sat_g2s.Name = "sat_g2s";
            this.sat_g2s.Size = new System.Drawing.Size(53, 30);
            this.sat_g2s.TabIndex = 1;
            this.sat_g2s.Text = "0";
            // 
            // sat_ds
            // 
            this.sat_ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_ds.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_ds.Location = new System.Drawing.Point(317, 577);
            this.sat_ds.Name = "sat_ds";
            this.sat_ds.Size = new System.Drawing.Size(58, 30);
            this.sat_ds.TabIndex = 1;
            this.sat_ds.Text = "0";
            // 
            // uzf_ds
            // 
            this.uzf_ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.uzf_ds.Font = new System.Drawing.Font("Calibri", 11F);
            this.uzf_ds.Location = new System.Drawing.Point(312, 400);
            this.uzf_ds.Name = "uzf_ds";
            this.uzf_ds.Size = new System.Drawing.Size(58, 30);
            this.uzf_ds.TabIndex = 1;
            this.uzf_ds.Text = "0";
            // 
            // sw_in
            // 
            this.sw_in.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sw_in.Font = new System.Drawing.Font("Calibri", 11F);
            this.sw_in.Location = new System.Drawing.Point(3, 158);
            this.sw_in.Name = "sw_in";
            this.sw_in.Size = new System.Drawing.Size(53, 30);
            this.sw_in.TabIndex = 1;
            this.sw_in.Text = "0";
            // 
            // sat_in
            // 
            this.sat_in.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_in.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_in.Location = new System.Drawing.Point(3, 528);
            this.sat_in.Name = "sat_in";
            this.sat_in.Size = new System.Drawing.Size(53, 30);
            this.sat_in.TabIndex = 1;
            this.sat_in.Text = "0";
            // 
            // sat_out
            // 
            this.sat_out.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_out.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_out.Location = new System.Drawing.Point(1010, 591);
            this.sat_out.Name = "sat_out";
            this.sat_out.Size = new System.Drawing.Size(53, 30);
            this.sat_out.TabIndex = 1;
            this.sat_out.Text = "0";
            // 
            // sw_out
            // 
            this.sw_out.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sw_out.Font = new System.Drawing.Font("Calibri", 11F);
            this.sw_out.Location = new System.Drawing.Point(1012, 188);
            this.sw_out.Name = "sw_out";
            this.sw_out.Size = new System.Drawing.Size(53, 30);
            this.sw_out.TabIndex = 1;
            this.sw_out.Text = "0";
            // 
            // sat_gw2sz
            // 
            this.sat_gw2sz.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_gw2sz.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_gw2sz.Location = new System.Drawing.Point(992, 336);
            this.sat_gw2sz.Name = "sat_gw2sz";
            this.sat_gw2sz.Size = new System.Drawing.Size(53, 30);
            this.sat_gw2sz.TabIndex = 1;
            this.sat_gw2sz.Text = "0";
            // 
            // sz_Percolation
            // 
            this.sz_Percolation.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sz_Percolation.Font = new System.Drawing.Font("Calibri", 11F);
            this.sz_Percolation.Location = new System.Drawing.Point(239, 278);
            this.sz_Percolation.Name = "sz_Percolation";
            this.sz_Percolation.Size = new System.Drawing.Size(58, 30);
            this.sz_Percolation.TabIndex = 1;
            this.sz_Percolation.Text = "0";
            // 
            // sz_ds
            // 
            this.sz_ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sz_ds.Font = new System.Drawing.Font("Calibri", 10F);
            this.sz_ds.Location = new System.Drawing.Point(312, 231);
            this.sz_ds.Name = "sz_ds";
            this.sz_ds.Size = new System.Drawing.Size(58, 28);
            this.sz_ds.TabIndex = 1;
            this.sz_ds.Text = "0";
            // 
            // lak_dun
            // 
            this.lak_dun.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lak_dun.Font = new System.Drawing.Font("Calibri", 10F);
            this.lak_dun.Location = new System.Drawing.Point(284, 188);
            this.lak_dun.Name = "lak_dun";
            this.lak_dun.Size = new System.Drawing.Size(45, 28);
            this.lak_dun.TabIndex = 1;
            this.lak_dun.Text = "0";
            // 
            // sfr_dun
            // 
            this.sfr_dun.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sfr_dun.Font = new System.Drawing.Font("Calibri", 10F);
            this.sfr_dun.Location = new System.Drawing.Point(453, 185);
            this.sfr_dun.Name = "sfr_dun";
            this.sfr_dun.Size = new System.Drawing.Size(51, 28);
            this.sfr_dun.TabIndex = 1;
            this.sfr_dun.Text = "0";
            // 
            // uz_error
            // 
            this.uz_error.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.uz_error.Font = new System.Drawing.Font("Calibri", 11F);
            this.uz_error.Location = new System.Drawing.Point(616, 446);
            this.uz_error.Name = "uz_error";
            this.uz_error.Size = new System.Drawing.Size(61, 30);
            this.uz_error.TabIndex = 1;
            this.uz_error.Text = "0";
            // 
            // sat_error
            // 
            this.sat_error.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sat_error.Font = new System.Drawing.Font("Calibri", 11F);
            this.sat_error.Location = new System.Drawing.Point(616, 616);
            this.sat_error.Name = "sat_error";
            this.sat_error.Size = new System.Drawing.Size(61, 30);
            this.sat_error.TabIndex = 1;
            this.sat_error.Text = "0";
            // 
            // soil_error
            // 
            this.soil_error.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.soil_error.Font = new System.Drawing.Font("Calibri", 11F);
            this.soil_error.Location = new System.Drawing.Point(607, 265);
            this.soil_error.Name = "soil_error";
            this.soil_error.Size = new System.Drawing.Size(61, 30);
            this.soil_error.TabIndex = 1;
            this.soil_error.Text = "0";
            // 
            // sz_et
            // 
            this.sz_et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sz_et.Font = new System.Drawing.Font("Calibri", 11F);
            this.sz_et.Location = new System.Drawing.Point(864, 215);
            this.sz_et.Name = "sz_et";
            this.sz_et.Size = new System.Drawing.Size(58, 30);
            this.sz_et.TabIndex = 1;
            this.sz_et.Text = "0";
            // 
            // total_error
            // 
            this.total_error.AcceptsReturn = true;
            this.total_error.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.total_error.Font = new System.Drawing.Font("Calibri", 11F);
            this.total_error.Location = new System.Drawing.Point(607, 6);
            this.total_error.Name = "total_error";
            this.total_error.Size = new System.Drawing.Size(66, 30);
            this.total_error.TabIndex = 1;
            this.total_error.Text = "0";
            // 
            // et
            // 
            this.et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.et.Font = new System.Drawing.Font("Calibri", 11F);
            this.et.Location = new System.Drawing.Point(847, 11);
            this.et.Name = "et";
            this.et.Size = new System.Drawing.Size(58, 30);
            this.et.TabIndex = 1;
            this.et.Text = "0";
            // 
            // ds
            // 
            this.ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ds.Font = new System.Drawing.Font("Calibri", 11F);
            this.ds.Location = new System.Drawing.Point(132, 11);
            this.ds.Name = "ds";
            this.ds.Size = new System.Drawing.Size(58, 30);
            this.ds.TabIndex = 1;
            this.ds.Text = "0";
            // 
            // ppt
            // 
            this.ppt.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ppt.Font = new System.Drawing.Font("Calibri", 11F);
            this.ppt.Location = new System.Drawing.Point(387, 13);
            this.ppt.Name = "ppt";
            this.ppt.Size = new System.Drawing.Size(58, 30);
            this.ppt.TabIndex = 1;
            this.ppt.Text = "0";
            // 
            // sf_et
            // 
            this.sf_et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sf_et.Font = new System.Drawing.Font("Calibri", 11F);
            this.sf_et.Location = new System.Drawing.Point(942, 73);
            this.sf_et.Name = "sf_et";
            this.sf_et.Size = new System.Drawing.Size(58, 30);
            this.sf_et.TabIndex = 1;
            this.sf_et.Text = "0";
            // 
            // sfr_ds
            // 
            this.sfr_ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sfr_ds.Location = new System.Drawing.Point(438, 127);
            this.sfr_ds.Name = "sfr_ds";
            this.sfr_ds.ReadOnly = true;
            this.sfr_ds.Size = new System.Drawing.Size(43, 27);
            this.sfr_ds.TabIndex = 1;
            this.sfr_ds.Text = "0";
            // 
            // lak_et
            // 
            this.lak_et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lak_et.Font = new System.Drawing.Font("Calibri", 11F);
            this.lak_et.Location = new System.Drawing.Point(184, 92);
            this.lak_et.Name = "lak_et";
            this.lak_et.Size = new System.Drawing.Size(43, 30);
            this.lak_et.TabIndex = 1;
            this.lak_et.Text = "0";
            // 
            // lak_ds
            // 
            this.lak_ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lak_ds.Font = new System.Drawing.Font("Calibri", 11F);
            this.lak_ds.Location = new System.Drawing.Point(239, 127);
            this.lak_ds.Name = "lak_ds";
            this.lak_ds.Size = new System.Drawing.Size(43, 30);
            this.lak_ds.TabIndex = 1;
            this.lak_ds.Text = "0";
            // 
            // sw_ds
            // 
            this.sw_ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sw_ds.Font = new System.Drawing.Font("Calibri", 11F);
            this.sw_ds.Location = new System.Drawing.Point(132, 124);
            this.sw_ds.Name = "sw_ds";
            this.sw_ds.Size = new System.Drawing.Size(43, 30);
            this.sw_ds.TabIndex = 1;
            this.sw_ds.Text = "0";
            // 
            // div
            // 
            this.div.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.div.Font = new System.Drawing.Font("Calibri", 11F);
            this.div.Location = new System.Drawing.Point(439, 58);
            this.div.Name = "div";
            this.div.Size = new System.Drawing.Size(58, 30);
            this.div.TabIndex = 1;
            this.div.Text = "0";
            // 
            // canal_ds
            // 
            this.canal_ds.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.canal_ds.Font = new System.Drawing.Font("Calibri", 10F);
            this.canal_ds.Location = new System.Drawing.Point(598, 158);
            this.canal_ds.Name = "canal_ds";
            this.canal_ds.Size = new System.Drawing.Size(43, 28);
            this.canal_ds.TabIndex = 1;
            this.canal_ds.Text = "0";
            // 
            // canal_et
            // 
            this.canal_et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.canal_et.Font = new System.Drawing.Font("Calibri", 11F);
            this.canal_et.Location = new System.Drawing.Point(537, 92);
            this.canal_et.Name = "canal_et";
            this.canal_et.Size = new System.Drawing.Size(43, 30);
            this.canal_et.TabIndex = 1;
            this.canal_et.Text = "0";
            // 
            // sfr_et
            // 
            this.sfr_et.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sfr_et.Font = new System.Drawing.Font("Calibri", 11F);
            this.sfr_et.Location = new System.Drawing.Point(337, 92);
            this.sfr_et.Name = "sfr_et";
            this.sfr_et.Size = new System.Drawing.Size(43, 30);
            this.sfr_et.TabIndex = 1;
            this.sfr_et.Text = "0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1089, 641);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // StateMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "StateMonitor";
            this.Size = new System.Drawing.Size(1334, 713);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl_Main.ResumeLayout(false);
            this.tabPage_Graph.ResumeLayout(false);
            this.tabPage_Report.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvDataTree)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl_Main;
        private System.Windows.Forms.TabPage tabPage_Graph;
        private Tree.NodeControls.NodeTextBox nodeTextBox1;
        private WinChart winChart1;
        private Tree.TreeViewAdv treeView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private System.Windows.Forms.TabPage tabPage_Report;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private BrightIdeasSoftware.DataTreeListView olvDataTree;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private System.Windows.Forms.TabPage tabPage1;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.TextBox sfr_slow;
        private System.Windows.Forms.TextBox sat_s2g;
        private System.Windows.Forms.TextBox uzf_recharge;
        private System.Windows.Forms.TextBox uzf_et;
        private System.Windows.Forms.TextBox sat_et;
        private System.Windows.Forms.TextBox sat_pr;
        private System.Windows.Forms.TextBox sat_g2s;
        private System.Windows.Forms.TextBox sat_ds;
        private System.Windows.Forms.TextBox uzf_ds;
        private System.Windows.Forms.TextBox sw_in;
        private System.Windows.Forms.TextBox sat_in;
        private System.Windows.Forms.TextBox sat_out;
        private System.Windows.Forms.TextBox sw_out;
        private System.Windows.Forms.TextBox sat_gw2sz;
        private System.Windows.Forms.TextBox sz_Percolation;
        private System.Windows.Forms.TextBox sz_ds;
        private System.Windows.Forms.TextBox sfr_dun;
        private System.Windows.Forms.TextBox sz_et;
        private System.Windows.Forms.TextBox et;
        private System.Windows.Forms.TextBox ds;
        private System.Windows.Forms.TextBox ppt;
        private System.Windows.Forms.TextBox sf_et;
        private System.Windows.Forms.TextBox sw_ds;
        private System.Windows.Forms.TextBox div;
        private System.Windows.Forms.TextBox sfr_et;
        private System.Windows.Forms.TextBox lak_ds;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sfr_ds;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox lak_et;
        private System.Windows.Forms.TextBox canal_ds;
        private System.Windows.Forms.TextBox canal_et;
        private System.Windows.Forms.TextBox lak_slow;
        private System.Windows.Forms.TextBox lak_dun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox total_error;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox uz_error;
        private System.Windows.Forms.TextBox sat_error;
        private System.Windows.Forms.TextBox soil_error;
        private System.Windows.Forms.ToolStripButton btnClearCache;
        private System.Windows.Forms.Label label6;
    }
}
