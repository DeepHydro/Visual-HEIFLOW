namespace Heiflow.Controls.WinForm.Controls
{
    partial class WinChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "MSE",
            "Mean Square Error",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "RMSE",
            "Root Mear Square Error",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Correlation",
            "Correlation Efficient",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "NSC",
            "Nash-Stucliffe Cofficient",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "",
            "Regression",
            "Linear Regression",
            ""}, -1);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStatPanel = new System.Windows.Forms.ToolStripButton();
            this.btnLengend = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClearExist = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.btnZoomFull = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.menu_line = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_scatter = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStat = new System.Windows.Forms.ToolStripButton();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerChart = new System.Windows.Forms.SplitContainer();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStripChart = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zoomFullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.chartPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvStatistics = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControlRight = new System.Windows.Forms.TabControl();
            this.tabPageSeries = new System.Windows.Forms.TabPage();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.contextMenuStripItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zoomToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_trend = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.styleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageProperty = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChart)).BeginInit();
            this.splitContainerChart.Panel1.SuspendLayout();
            this.splitContainerChart.Panel2.SuspendLayout();
            this.splitContainerChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.contextMenuStripChart.SuspendLayout();
            this.tabControlRight.SuspendLayout();
            this.tabPageSeries.SuspendLayout();
            this.contextMenuStripItems.SuspendLayout();
            this.tabPageProperty.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Calibri", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStatPanel,
            this.btnLengend,
            this.toolStripSeparator1,
            this.btnClearExist,
            this.btnClear,
            this.btnZoomFull,
            this.toolStripSeparator2,
            this.toolStripDropDownButton1,
            this.btnStat});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1024, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnStatPanel
            // 
            this.btnStatPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStatPanel.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_business_work_12_2377635;
            this.btnStatPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStatPanel.Name = "btnStatPanel";
            this.btnStatPanel.Size = new System.Drawing.Size(24, 24);
            this.btnStatPanel.Text = "Show Statistics";
            this.btnStatPanel.Click += new System.EventHandler(this.btnStatPanel_Click);
            // 
            // btnLengend
            // 
            this.btnLengend.Checked = true;
            this.btnLengend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnLengend.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLengend.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_stock_chart_toggle_legend_93841;
            this.btnLengend.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLengend.Name = "btnLengend";
            this.btnLengend.Size = new System.Drawing.Size(24, 24);
            this.btnLengend.Text = "Show series list";
            this.btnLengend.Click += new System.EventHandler(this.btnLengend_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnClearExist
            // 
            this.btnClearExist.Checked = true;
            this.btnClearExist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnClearExist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClearExist.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_selected_delete_37293;
            this.btnClearExist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearExist.Name = "btnClearExist";
            this.btnClearExist.Size = new System.Drawing.Size(24, 24);
            this.btnClearExist.Text = "Clear existed series";
            this.btnClearExist.Click += new System.EventHandler(this.btnClearExist_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_edit_clear_15273;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.Text = "Clear the chart";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnZoomFull
            // 
            this.btnZoomFull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomFull.Image = global::Heiflow.Controls.WinForm.Properties.Resources.ZoomFixedZoomOut32;
            this.btnZoomFull.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomFull.Name = "btnZoomFull";
            this.btnZoomFull.Size = new System.Drawing.Size(24, 24);
            this.btnZoomFull.Text = "Zoom to full extent";
            this.btnZoomFull.Click += new System.EventHandler(this.btnZoomFull_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_line,
            this.menu_scatter});
            this.toolStripDropDownButton1.Image = global::Heiflow.Controls.WinForm.Properties.Resources.Chart;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 24);
            this.toolStripDropDownButton1.Text = "Select Figure Type";
            // 
            // menu_line
            // 
            this.menu_line.Checked = true;
            this.menu_line.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menu_line.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GraphVerticalLine16;
            this.menu_line.Name = "menu_line";
            this.menu_line.Size = new System.Drawing.Size(125, 26);
            this.menu_line.Text = "Line";
            this.menu_line.Click += new System.EventHandler(this.menu_line_Click);
            // 
            // menu_scatter
            // 
            this.menu_scatter.Image = global::Heiflow.Controls.WinForm.Properties.Resources.SpatialAnalystTrainingSampleScatterplots16;
            this.menu_scatter.Name = "menu_scatter";
            this.menu_scatter.Size = new System.Drawing.Size(125, 26);
            this.menu_scatter.Text = "Scatter";
            this.menu_scatter.Click += new System.EventHandler(this.menu_scatter_Click);
            // 
            // btnStat
            // 
            this.btnStat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStat.Image = global::Heiflow.Controls.WinForm.Properties.Resources.SelectionStatistics_B_32;
            this.btnStat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStat.Name = "btnStat";
            this.btnStat.Size = new System.Drawing.Size(24, 24);
            this.btnStat.Text = "Calculate statistics";
            this.btnStat.ToolTipText = "Calculate statistics";
            this.btnStat.Click += new System.EventHandler(this.btnStat_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.BackColor = System.Drawing.Color.White;
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 27);
            this.splitContainerMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerChart);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tabControlRight);
            this.splitContainerMain.Size = new System.Drawing.Size(1024, 574);
            this.splitContainerMain.SplitterDistance = 744;
            this.splitContainerMain.TabIndex = 0;
            // 
            // splitContainerChart
            // 
            this.splitContainerChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerChart.Location = new System.Drawing.Point(0, 0);
            this.splitContainerChart.Name = "splitContainerChart";
            this.splitContainerChart.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerChart.Panel1
            // 
            this.splitContainerChart.Panel1.Controls.Add(this.chart1);
            // 
            // splitContainerChart.Panel2
            // 
            this.splitContainerChart.Panel2.Controls.Add(this.lvStatistics);
            this.splitContainerChart.Size = new System.Drawing.Size(744, 574);
            this.splitContainerChart.SplitterDistance = 432;
            this.splitContainerChart.TabIndex = 2;
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.Gainsboro;
            this.chart1.BackSecondaryColor = System.Drawing.Color.White;
            this.chart1.BorderlineColor = System.Drawing.Color.DarkGray;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Area3DStyle.Inclination = 15;
            chartArea1.Area3DStyle.IsClustered = true;
            chartArea1.Area3DStyle.IsRightAngleAxes = false;
            chartArea1.Area3DStyle.Perspective = 10;
            chartArea1.Area3DStyle.Rotation = 10;
            chartArea1.Area3DStyle.WallWidth = 0;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.ScaleView.Position = 10D;
            chartArea1.AxisX.ScaleView.Size = 20D;
            chartArea1.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.ScrollBar.LineColor = System.Drawing.Color.Black;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.ScaleView.Position = 10D;
            chartArea1.AxisY.ScaleView.Size = 70D;
            chartArea1.AxisY.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.ScrollBar.LineColor = System.Drawing.Color.Black;
            chartArea1.BackColor = System.Drawing.Color.White;
            chartArea1.BackSecondaryColor = System.Drawing.Color.White;
            chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorX.SelectionColor = System.Drawing.Color.Gray;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.CursorY.SelectionColor = System.Drawing.Color.Gray;
            chartArea1.InnerPlotPosition.Auto = false;
            chartArea1.InnerPlotPosition.Height = 88F;
            chartArea1.InnerPlotPosition.Width = 88F;
            chartArea1.InnerPlotPosition.X = 5F;
            chartArea1.Name = "Default";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 95F;
            chartArea1.Position.Width = 95F;
            chartArea1.Position.X = 5F;
            chartArea1.Position.Y = 5F;
            chartArea1.ShadowColor = System.Drawing.Color.Transparent;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.ContextMenuStrip = this.contextMenuStripChart;
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Font = new System.Drawing.Font("Calibri", 8.5F);
            legend1.IsTextAutoFit = false;
            legend1.Name = "Default";
            legend1.Position.Auto = false;
            legend1.Position.Height = 5.346535F;
            legend1.Position.Width = 13.25301F;
            legend1.Position.X = 75F;
            legend1.Position.Y = 5F;
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 7);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            series1.BorderWidth = 3;
            series1.ChartArea = "Default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series1.Legend = "Default";
            series1.Name = "Default";
            series1.ShadowColor = System.Drawing.Color.Black;
            series1.ShadowOffset = 1;
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(744, 432);
            this.chart1.TabIndex = 1;
            // 
            // contextMenuStripChart
            // 
            this.contextMenuStripChart.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripChart.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomFullToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripSeparator3,
            this.chartPropertyToolStripMenuItem});
            this.contextMenuStripChart.Name = "contextMenuStripChart";
            this.contextMenuStripChart.Size = new System.Drawing.Size(155, 88);
            this.contextMenuStripChart.Click += new System.EventHandler(this.btnZoomFull_Click);
            // 
            // zoomFullToolStripMenuItem
            // 
            this.zoomFullToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.ZoomFixedZoomOut32;
            this.zoomFullToolStripMenuItem.Name = "zoomFullToolStripMenuItem";
            this.zoomFullToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.zoomFullToolStripMenuItem.Text = "Zoom Full";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericDeleteRed32;
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(151, 6);
            // 
            // chartPropertyToolStripMenuItem
            // 
            this.chartPropertyToolStripMenuItem.Name = "chartPropertyToolStripMenuItem";
            this.chartPropertyToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.chartPropertyToolStripMenuItem.Text = "Property";
            this.chartPropertyToolStripMenuItem.Click += new System.EventHandler(this.chartPropertyToolStripMenuItem_Click);
            // 
            // lvStatistics
            // 
            this.lvStatistics.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.lvStatistics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.lvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvStatistics.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lvStatistics.GridLines = true;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.Checked = true;
            listViewItem2.StateImageIndex = 2;
            listViewItem3.Checked = true;
            listViewItem3.StateImageIndex = 1;
            listViewItem4.Checked = true;
            listViewItem4.StateImageIndex = 10;
            this.lvStatistics.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.lvStatistics.LabelEdit = true;
            this.lvStatistics.Location = new System.Drawing.Point(0, 0);
            this.lvStatistics.Name = "lvStatistics";
            this.lvStatistics.Size = new System.Drawing.Size(744, 138);
            this.lvStatistics.TabIndex = 4;
            this.lvStatistics.UseCompatibleStateImageBehavior = false;
            this.lvStatistics.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "!";
            this.columnHeader5.Width = 19;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Item";
            this.columnHeader6.Width = 96;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Description";
            this.columnHeader7.Width = 203;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Value";
            this.columnHeader8.Width = 389;
            // 
            // tabControlRight
            // 
            this.tabControlRight.Controls.Add(this.tabPageSeries);
            this.tabControlRight.Controls.Add(this.tabPageProperty);
            this.tabControlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlRight.Location = new System.Drawing.Point(0, 0);
            this.tabControlRight.Multiline = true;
            this.tabControlRight.Name = "tabControlRight";
            this.tabControlRight.SelectedIndex = 0;
            this.tabControlRight.Size = new System.Drawing.Size(276, 574);
            this.tabControlRight.TabIndex = 1;
            // 
            // tabPageSeries
            // 
            this.tabPageSeries.Controls.Add(this.checkedListBox1);
            this.tabPageSeries.Location = new System.Drawing.Point(4, 29);
            this.tabPageSeries.Name = "tabPageSeries";
            this.tabPageSeries.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSeries.Size = new System.Drawing.Size(268, 541);
            this.tabPageSeries.TabIndex = 0;
            this.tabPageSeries.Text = "Series";
            this.tabPageSeries.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.ColumnWidth = 400;
            this.checkedListBox1.ContextMenuStrip = this.contextMenuStripItems;
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.HorizontalScrollbar = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "series1"});
            this.checkedListBox1.Location = new System.Drawing.Point(3, 3);
            this.checkedListBox1.MultiColumn = true;
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(262, 535);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            this.checkedListBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkedListBox1_MouseUp);
            // 
            // contextMenuStripItems
            // 
            this.contextMenuStripItems.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToToolStripMenuItem,
            this.menu_trend,
            this.viewDataToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeToolStripMenuItem,
            this.toolStripSeparator4,
            this.styleToolStripMenuItem});
            this.contextMenuStripItems.Name = "contextMenuStripItems";
            this.contextMenuStripItems.Size = new System.Drawing.Size(167, 146);
            // 
            // zoomToToolStripMenuItem
            // 
            this.zoomToToolStripMenuItem.Name = "zoomToToolStripMenuItem";
            this.zoomToToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.zoomToToolStripMenuItem.Text = "Zoom to";
            this.zoomToToolStripMenuItem.Click += new System.EventHandler(this.zoomToToolStripMenuItem_Click);
            // 
            // menu_trend
            // 
            this.menu_trend.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GeostatisticalGraphGeneralQQPlot16;
            this.menu_trend.Name = "menu_trend";
            this.menu_trend.Size = new System.Drawing.Size(166, 26);
            this.menu_trend.Text = "Trend";
            this.menu_trend.Click += new System.EventHandler(this.menu_trend_Click);
            // 
            // viewDataToolStripMenuItem
            // 
            this.viewDataToolStripMenuItem.Name = "viewDataToolStripMenuItem";
            this.viewDataToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.viewDataToolStripMenuItem.Text = "View Data...";
            this.viewDataToolStripMenuItem.Click += new System.EventHandler(this.viewDataToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(163, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(163, 6);
            // 
            // styleToolStripMenuItem
            // 
            this.styleToolStripMenuItem.Name = "styleToolStripMenuItem";
            this.styleToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.styleToolStripMenuItem.Text = "Property";
            this.styleToolStripMenuItem.Click += new System.EventHandler(this.styleToolStripMenuItem_Click);
            // 
            // tabPageProperty
            // 
            this.tabPageProperty.Controls.Add(this.propertyGrid1);
            this.tabPageProperty.Location = new System.Drawing.Point(4, 30);
            this.tabPageProperty.Name = "tabPageProperty";
            this.tabPageProperty.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProperty.Size = new System.Drawing.Size(268, 541);
            this.tabPageProperty.TabIndex = 1;
            this.tabPageProperty.Text = "Property";
            this.tabPageProperty.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(262, 535);
            this.propertyGrid1.TabIndex = 0;
            // 
            // WinChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WinChart";
            this.Size = new System.Drawing.Size(1024, 601);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerChart.Panel1.ResumeLayout(false);
            this.splitContainerChart.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChart)).EndInit();
            this.splitContainerChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.contextMenuStripChart.ResumeLayout(false);
            this.tabControlRight.ResumeLayout(false);
            this.tabPageSeries.ResumeLayout(false);
            this.contextMenuStripItems.ResumeLayout(false);
            this.tabPageProperty.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLengend;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripButton btnClearExist;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ToolStripButton btnZoomFull;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem menu_line;
        private System.Windows.Forms.ToolStripMenuItem menu_scatter;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripItems;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnStat;
        private System.Windows.Forms.TabControl tabControlRight;
        private System.Windows.Forms.TabPage tabPageSeries;
        private System.Windows.Forms.TabPage tabPageProperty;
        private System.Windows.Forms.ListView lvStatistics;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerChart;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripButton btnStatPanel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripChart;
        private System.Windows.Forms.ToolStripMenuItem zoomFullToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem chartPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menu_trend;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}
