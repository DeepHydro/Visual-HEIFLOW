using Heiflow.Controls.WinForm.Controls;
namespace Heiflow.Controls.WinForm.SFRExplorer
{
    partial class SFRExplorer
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.chk_datasource = new System.Windows.Forms.ToolStripDropDownButton();
            this.mi_flow = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_nps = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_sediment = new System.Windows.Forms.ToolStripMenuItem();
            this.mi_month_npc = new System.Windows.Forms.ToolStripMenuItem();
            this.btnScan = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripLabel();
            this.cmbLoadVar = new System.Windows.Forms.ToolStripComboBox();
            this.chbReadComplData = new System.Windows.Forms.ToolStripButton();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmbSFRVars = new System.Windows.Forms.ToolStripComboBox();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tbnSlctDataSource = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRiversToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportReachesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsShpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.segmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reachesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.riverJunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToSWMMInpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.labelStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlLeft = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.grpSimulated = new System.Windows.Forms.GroupBox();
            this.grpObservation = new System.Windows.Forms.GroupBox();
            this.btnAddSfrMat2Toolbox = new System.Windows.Forms.Button();
            this.cmbObsVars = new System.Windows.Forms.ComboBox();
            this.cmbSite = new System.Windows.Forms.ComboBox();
            this.cmbRchID = new System.Windows.Forms.ComboBox();
            this.cmbSegsID = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbCurDate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd2Toolbox = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chbUnifiedByLength = new System.Windows.Forms.CheckBox();
            this.cmbEndID = new System.Windows.Forms.ComboBox();
            this.cmbStartID = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabControl_Chart = new System.Windows.Forms.TabControl();
            this.tabPageTimeSeries = new System.Windows.Forms.TabPage();
            this.winChart_timeseries = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.tabPageProfile = new System.Windows.Forms.TabPage();
            this.winChart_proflie = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.colorSlider1 = new Heiflow.Controls.WinForm.ColorSlider();
            this.tabPageFeatureLayer = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbDates = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbReachFields = new System.Windows.Forms.ComboBox();
            this.cmbSegFields = new System.Windows.Forms.ComboBox();
            this.cmbLayers = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxSaveLayer = new System.Windows.Forms.CheckBox();
            this.btnRefreshLayer = new System.Windows.Forms.Button();
            this.btnShowLayer = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlLeft.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.grpSimulated.SuspendLayout();
            this.grpObservation.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl_Chart.SuspendLayout();
            this.tabPageTimeSeries.SuspendLayout();
            this.tabPageProfile.SuspendLayout();
            this.tabPageFeatureLayer.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chk_datasource,
            this.btnScan,
            this.toolStripSeparator3,
            this.toolStripButton2,
            this.cmbLoadVar,
            this.chbReadComplData,
            this.btnLoad,
            this.toolStripButton1,
            this.toolStripLabel1,
            this.cmbSFRVars,
            this.btnClear,
            this.toolStripSeparator2,
            this.btnRefresh,
            this.tbnSlctDataSource,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.labelStatus,
            this.toolStripProgressBar1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1132, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // chk_datasource
            // 
            this.chk_datasource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.chk_datasource.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mi_flow,
            this.mi_nps,
            this.mi_sediment,
            this.mi_month_npc});
            this.chk_datasource.Image = global::Heiflow.Controls.WinForm.Properties.Resources.SfrDataSource24;
            this.chk_datasource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chk_datasource.Name = "chk_datasource";
            this.chk_datasource.Size = new System.Drawing.Size(86, 25);
            this.chk_datasource.Text = "Output";
            this.chk_datasource.ToolTipText = "Select the SFR output file to explore: flow, non-point source, sediment or monthly NPC";
            // 
            // mi_flow
            // 
            this.mi_flow.Checked = true;
            this.mi_flow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mi_flow.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiProfile16;
            this.mi_flow.Name = "mi_flow";
            this.mi_flow.Size = new System.Drawing.Size(248, 26);
            this.mi_flow.Text = "Flow Module";
            this.mi_flow.Click += new System.EventHandler(this.mi_flow_Click);
            // 
            // mi_nps
            // 
            this.mi_nps.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiChart16;
            this.mi_nps.Name = "mi_nps";
            this.mi_nps.Size = new System.Drawing.Size(248, 26);
            this.mi_nps.Text = "Non-Point Source Module";
            this.mi_nps.Click += new System.EventHandler(this.mi_flow_Click);
            // 
            // mi_sediment
            // 
            this.mi_sediment.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiLayer16;
            this.mi_sediment.Name = "mi_sediment";
            this.mi_sediment.Size = new System.Drawing.Size(248, 26);
            this.mi_sediment.Text = "Sediment";
            this.mi_sediment.Click += new System.EventHandler(this.mi_flow_Click);
            // 
            // mi_month_npc
            // 
            this.mi_month_npc.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiCsv16;
            this.mi_month_npc.Name = "mi_month_npc";
            this.mi_month_npc.Size = new System.Drawing.Size(248, 26);
            this.mi_month_npc.Text = " Monthly NPC";
            this.mi_month_npc.Click += new System.EventHandler(this.mi_flow_Click);
            // 
            // btnScan
            // 
            this.btnScan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScan.Image = global::Heiflow.Controls.WinForm.Properties.Resources.SfrScan24;
            this.btnScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(24, 25);
            this.btnScan.Text = "Scan";
            this.btnScan.ToolTipText = "Scan the selected output file and read its variables and time steps";
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(46, 25);
            this.toolStripButton2.Text = "Scope:";
            // 
            // cmbLoadVar
            // 
            this.cmbLoadVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLoadVar.Items.AddRange(new object[] {
            "All Variables",
            "Single Variable"});
            this.cmbLoadVar.Name = "cmbLoadVar";
            this.cmbLoadVar.Size = new System.Drawing.Size(121, 28);
            this.cmbLoadVar.SelectedIndexChanged += new System.EventHandler(this.cmbLoadVar_SelectedIndexChanged);
            // 
            // chbReadComplData
            // 
            this.chbReadComplData.CheckOnClick = true;
            this.chbReadComplData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.chbReadComplData.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiDefault24;
            this.chbReadComplData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chbReadComplData.Name = "chbReadComplData";
            this.chbReadComplData.Size = new System.Drawing.Size(112, 25);
            this.chbReadComplData.Text = "All Reaches";
            this.chbReadComplData.ToolTipText = "When checked, every reach of every segment is loaded; otherwise only the last reach of each segment is loaded";
            this.chbReadComplData.CheckedChanged += new System.EventHandler(this.chbReadComplData_CheckedChanged_1);
            // 
            // btnLoad
            // 
            this.btnLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnLoad.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiLoad24;
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(64, 25);
            this.btnLoad.Text = "Load";
            this.btnLoad.ToolTipText = "Load the SFR output according to the selected scope";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(56, 25);
            this.toolStripLabel1.Text = "Variable";
            // 
            // cmbSFRVars
            // 
            this.cmbSFRVars.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSFRVars.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cmbSFRVars.Name = "cmbSFRVars";
            this.cmbSFRVars.Size = new System.Drawing.Size(180, 28);
            this.cmbSFRVars.SelectedIndexChanged += new System.EventHandler(this.cmbSFRVars_SelectedIndexChanged);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiClear24;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 25);
            this.btnClear.Text = "Clear";
            this.btnClear.ToolTipText = "Unload the data currently held in memory";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 28);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiRefresh24;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 25);
            this.btnRefresh.Text = "Refresh observation sites";
            this.btnRefresh.ToolTipText = "Refresh the observation sites of the current ODM database";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // tbnSlctDataSource
            // 
            this.tbnSlctDataSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbnSlctDataSource.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiDatabase24;
            this.tbnSlctDataSource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbnSlctDataSource.Name = "tbnSlctDataSource";
            this.tbnSlctDataSource.Size = new System.Drawing.Size(24, 25);
            this.tbnSlctDataSource.Text = "Observation data source";
            this.tbnSlctDataSource.ToolTipText = "Select the observation data source (ODM database)";
            this.tbnSlctDataSource.Click += new System.EventHandler(this.tbnSlctDataSource_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem1,
            this.saveAsShpToolStripMenuItem,
            this.exportToSWMMInpToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiExport24;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(78, 25);
            this.toolStripDropDownButton1.Text = "Export";
            this.toolStripDropDownButton1.ToolTipText = "Export the profile, the river network or the SWMM input file";
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportRiversToolStripMenuItem,
            this.exportReachesToolStripMenuItem});
            this.exportToolStripMenuItem1.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiExport16;
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(219, 26);
            this.exportToolStripMenuItem1.Text = "Export Profile As CSV";
            // 
            // exportRiversToolStripMenuItem
            // 
            this.exportRiversToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiExport16;
            this.exportRiversToolStripMenuItem.Name = "exportRiversToolStripMenuItem";
            this.exportRiversToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.exportRiversToolStripMenuItem.Text = "Segments";
            this.exportRiversToolStripMenuItem.Click += new System.EventHandler(this.exportRiversToolStripMenuItem_Click);
            // 
            // exportReachesToolStripMenuItem
            // 
            this.exportReachesToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiExport16;
            this.exportReachesToolStripMenuItem.Name = "exportReachesToolStripMenuItem";
            this.exportReachesToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.exportReachesToolStripMenuItem.Text = "Reaches";
            this.exportReachesToolStripMenuItem.Click += new System.EventHandler(this.exportReachesToolStripMenuItem_Click);
            // 
            // saveAsShpToolStripMenuItem
            // 
            this.saveAsShpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.segmentsToolStripMenuItem,
            this.reachesToolStripMenuItem,
            this.riverJunctionsToolStripMenuItem});
            this.saveAsShpToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiLayer16;
            this.saveAsShpToolStripMenuItem.Name = "saveAsShpToolStripMenuItem";
            this.saveAsShpToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.saveAsShpToolStripMenuItem.Text = "Save As Shp";
            // 
            // segmentsToolStripMenuItem
            // 
            this.segmentsToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiLayer16;
            this.segmentsToolStripMenuItem.Name = "segmentsToolStripMenuItem";
            this.segmentsToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.segmentsToolStripMenuItem.Text = "Segments";
            this.segmentsToolStripMenuItem.Click += new System.EventHandler(this.segmentsToolStripMenuItem_Click);
            // 
            // reachesToolStripMenuItem
            // 
            this.reachesToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiMap16;
            this.reachesToolStripMenuItem.Name = "reachesToolStripMenuItem";
            this.reachesToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.reachesToolStripMenuItem.Text = "Reaches";
            this.reachesToolStripMenuItem.Click += new System.EventHandler(this.reachesToolStripMenuItem_Click);
            // 
            // riverJunctionsToolStripMenuItem
            // 
            this.riverJunctionsToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiMap16;
            this.riverJunctionsToolStripMenuItem.Name = "riverJunctionsToolStripMenuItem";
            this.riverJunctionsToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.riverJunctionsToolStripMenuItem.Text = "River Junctions";
            this.riverJunctionsToolStripMenuItem.Click += new System.EventHandler(this.riverJunctionsToolStripMenuItem_Click);
            // 
            // exportToSWMMInpToolStripMenuItem
            // 
            this.exportToSWMMInpToolStripMenuItem.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiExport16;
            this.exportToSWMMInpToolStripMenuItem.Name = "exportToSWMMInpToolStripMenuItem";
            this.exportToSWMMInpToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.exportToSWMMInpToolStripMenuItem.Text = "Export to SWMM Inp";
            this.exportToSWMMInpToolStripMenuItem.Click += new System.EventHandler(this.exportToSWMMInpToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(160, 25);
            // 
            // labelStatus
            // 
            this.labelStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(49, 25);
            this.labelStatus.Text = "Ready";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControlLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl_Chart);
            this.splitContainer1.Size = new System.Drawing.Size(1132, 625);
            this.splitContainer1.SplitterDistance = 268;
            this.splitContainer1.SplitterWidth = 4;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControlLeft
            // 
            this.tabControlLeft.Controls.Add(this.tabPage3);
            this.tabControlLeft.Controls.Add(this.tabPage4);
            this.tabControlLeft.Controls.Add(this.tabPage1);
            this.tabControlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLeft.Location = new System.Drawing.Point(0, 0);
            this.tabControlLeft.Name = "tabControlLeft";
            this.tabControlLeft.SelectedIndex = 0;
            this.tabControlLeft.Size = new System.Drawing.Size(268, 625);
            this.tabControlLeft.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnAddSfrMat2Toolbox);
            this.tabPage3.Controls.Add(this.grpObservation);
            this.tabPage3.Controls.Add(this.grpSimulated);
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(257, 593);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Time Series";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnAddSfrMat2Toolbox
            // 
            this.btnAddSfrMat2Toolbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSfrMat2Toolbox.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiAdd24;
            this.btnAddSfrMat2Toolbox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddSfrMat2Toolbox.Location = new System.Drawing.Point(8, 288);
            this.btnAddSfrMat2Toolbox.Name = "btnAddSfrMat2Toolbox";
            this.btnAddSfrMat2Toolbox.Size = new System.Drawing.Size(241, 34);
            this.btnAddSfrMat2Toolbox.TabIndex = 13;
            this.btnAddSfrMat2Toolbox.Text = "Add Time Series To Toolbox";
            this.btnAddSfrMat2Toolbox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddSfrMat2Toolbox.UseVisualStyleBackColor = true;
            this.btnAddSfrMat2Toolbox.Click += new System.EventHandler(this.btnAddSfrMat2Toolbox_Click);
            // 
            // grpSimulated
            // 
            this.grpSimulated.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSimulated.Controls.Add(this.cmbRchID);
            this.grpSimulated.Controls.Add(this.label3);
            this.grpSimulated.Controls.Add(this.cmbSegsID);
            this.grpSimulated.Controls.Add(this.label1);
            this.grpSimulated.Location = new System.Drawing.Point(8, 8);
            this.grpSimulated.Name = "grpSimulated";
            this.grpSimulated.Size = new System.Drawing.Size(241, 132);
            this.grpSimulated.TabIndex = 14;
            this.grpSimulated.TabStop = false;
            this.grpSimulated.Text = "Simulated Series";
            // 
            // grpObservation
            // 
            this.grpObservation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpObservation.Controls.Add(this.cmbObsVars);
            this.grpObservation.Controls.Add(this.label5);
            this.grpObservation.Controls.Add(this.cmbSite);
            this.grpObservation.Controls.Add(this.label4);
            this.grpObservation.Location = new System.Drawing.Point(8, 148);
            this.grpObservation.Name = "grpObservation";
            this.grpObservation.Size = new System.Drawing.Size(241, 132);
            this.grpObservation.TabIndex = 15;
            this.grpObservation.TabStop = false;
            this.grpObservation.Text = "Observation";
            // 
            // cmbObsVars
            // 
            this.cmbObsVars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbObsVars.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbObsVars.FormattingEnabled = true;
            this.cmbObsVars.Location = new System.Drawing.Point(10, 96);
            this.cmbObsVars.Name = "cmbObsVars";
            this.cmbObsVars.Size = new System.Drawing.Size(221, 27);
            this.cmbObsVars.TabIndex = 6;
            this.cmbObsVars.SelectedIndexChanged += new System.EventHandler(this.cmbObsVars_SelectedIndexChanged);
            // 
            // cmbSite
            // 
            this.cmbSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSite.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSite.FormattingEnabled = true;
            this.cmbSite.Location = new System.Drawing.Point(10, 46);
            this.cmbSite.Name = "cmbSite";
            this.cmbSite.Size = new System.Drawing.Size(221, 27);
            this.cmbSite.TabIndex = 7;
            this.cmbSite.SelectedIndexChanged += new System.EventHandler(this.cmbSite_SelectedIndexChanged);
            // 
            // cmbRchID
            // 
            this.cmbRchID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRchID.Enabled = false;
            this.cmbRchID.FormattingEnabled = true;
            this.cmbRchID.Location = new System.Drawing.Point(10, 96);
            this.cmbRchID.Name = "cmbRchID";
            this.cmbRchID.Size = new System.Drawing.Size(221, 27);
            this.cmbRchID.TabIndex = 8;
            this.cmbRchID.SelectedIndexChanged += new System.EventHandler(this.cmbRchID_SelectedIndexChanged);
            // 
            // cmbSegsID
            // 
            this.cmbSegsID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSegsID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSegsID.FormattingEnabled = true;
            this.cmbSegsID.Location = new System.Drawing.Point(10, 46);
            this.cmbSegsID.Name = "cmbSegsID";
            this.cmbSegsID.Size = new System.Drawing.Size(221, 27);
            this.cmbSegsID.TabIndex = 9;
            this.cmbSegsID.SelectedIndexChanged += new System.EventHandler(this.cmbSegsID_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 19);
            this.label5.TabIndex = 2;
            this.label5.Text = "Observed Variable";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "Observation Site";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "Reach ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Segment ID";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Controls.Add(this.btnAdd2Toolbox);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(257, 593);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Profile";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.tbCurDate);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(6, 215);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(245, 104);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Information";
            // 
            // tbCurDate
            // 
            this.tbCurDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCurDate.BackColor = System.Drawing.SystemColors.Info;
            this.tbCurDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCurDate.Location = new System.Drawing.Point(10, 58);
            this.tbCurDate.Name = "tbCurDate";
            this.tbCurDate.ReadOnly = true;
            this.tbCurDate.Size = new System.Drawing.Size(229, 27);
            this.tbCurDate.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Current time:";
            // 
            // btnAdd2Toolbox
            // 
            this.btnAdd2Toolbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd2Toolbox.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiAdd24;
            this.btnAdd2Toolbox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd2Toolbox.Location = new System.Drawing.Point(6, 327);
            this.btnAdd2Toolbox.Name = "btnAdd2Toolbox";
            this.btnAdd2Toolbox.Size = new System.Drawing.Size(245, 34);
            this.btnAdd2Toolbox.TabIndex = 12;
            this.btnAdd2Toolbox.Text = "Add Profile To Toolbox";
            this.btnAdd2Toolbox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAdd2Toolbox.UseVisualStyleBackColor = true;
            this.btnAdd2Toolbox.Click += new System.EventHandler(this.btnAdd2Toolbox_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chbUnifiedByLength);
            this.groupBox2.Controls.Add(this.cmbEndID);
            this.groupBox2.Controls.Add(this.cmbStartID);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(6, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 196);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Profile Settings";
            // 
            // chbUnifiedByLength
            // 
            this.chbUnifiedByLength.AutoSize = true;
            this.chbUnifiedByLength.Location = new System.Drawing.Point(10, 158);
            this.chbUnifiedByLength.Name = "chbUnifiedByLength";
            this.chbUnifiedByLength.Size = new System.Drawing.Size(141, 23);
            this.chbUnifiedByLength.TabIndex = 11;
            this.chbUnifiedByLength.Text = "Unified by length";
            this.chbUnifiedByLength.UseVisualStyleBackColor = true;
            // 
            // cmbEndID
            // 
            this.cmbEndID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEndID.FormattingEnabled = true;
            this.cmbEndID.Location = new System.Drawing.Point(10, 110);
            this.cmbEndID.Name = "cmbEndID";
            this.cmbEndID.Size = new System.Drawing.Size(229, 27);
            this.cmbEndID.TabIndex = 4;
            this.cmbEndID.SelectedIndexChanged += new System.EventHandler(this.cmbEndID_SelectedIndexChanged);
            // 
            // cmbStartID
            // 
            this.cmbStartID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbStartID.FormattingEnabled = true;
            this.cmbStartID.Location = new System.Drawing.Point(10, 50);
            this.cmbStartID.Name = "cmbStartID";
            this.cmbStartID.Size = new System.Drawing.Size(229, 27);
            this.cmbStartID.TabIndex = 5;
            this.cmbStartID.SelectedIndexChanged += new System.EventHandler(this.cmbStartID_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 19);
            this.label6.TabIndex = 2;
            this.label6.Text = "End Segment ID";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 19);
            this.label7.TabIndex = 3;
            this.label7.Text = "Start Segment ID";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propertyGrid1);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(257, 593);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Config";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(251, 587);
            this.propertyGrid1.TabIndex = 1;
            // 
            // tabControl_Chart
            // 
            this.tabControl_Chart.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl_Chart.Controls.Add(this.tabPageTimeSeries);
            this.tabControl_Chart.Controls.Add(this.tabPageProfile);
            this.tabControl_Chart.Controls.Add(this.tabPageFeatureLayer);
            this.tabControl_Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Chart.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Chart.Name = "tabControl_Chart";
            this.tabControl_Chart.SelectedIndex = 0;
            this.tabControl_Chart.Size = new System.Drawing.Size(864, 625);
            this.tabControl_Chart.TabIndex = 6;
            // 
            // tabPageTimeSeries
            // 
            this.tabPageTimeSeries.Controls.Add(this.winChart_timeseries);
            this.tabPageTimeSeries.Location = new System.Drawing.Point(4, 4);
            this.tabPageTimeSeries.Name = "tabPageTimeSeries";
            this.tabPageTimeSeries.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTimeSeries.Size = new System.Drawing.Size(856, 593);
            this.tabPageTimeSeries.TabIndex = 0;
            this.tabPageTimeSeries.Text = "Time Series View";
            this.tabPageTimeSeries.UseVisualStyleBackColor = true;
            // 
            // winChart_timeseries
            // 
            this.winChart_timeseries.BackColor = System.Drawing.SystemColors.Control;
            this.winChart_timeseries.ClearExistesSeries = true;
            this.winChart_timeseries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_timeseries.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.winChart_timeseries.Location = new System.Drawing.Point(3, 3);
            this.winChart_timeseries.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChart_timeseries.Name = "winChart_timeseries";
            this.winChart_timeseries.ShowStatPanel = true;
            this.winChart_timeseries.Size = new System.Drawing.Size(850, 587);
            this.winChart_timeseries.TabIndex = 7;
            // 
            // tabPageProfile
            // 
            this.tabPageProfile.Controls.Add(this.winChart_proflie);
            this.tabPageProfile.Controls.Add(this.colorSlider1);
            this.tabPageProfile.Location = new System.Drawing.Point(4, 4);
            this.tabPageProfile.Name = "tabPageProfile";
            this.tabPageProfile.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile.Size = new System.Drawing.Size(856, 593);
            this.tabPageProfile.TabIndex = 1;
            this.tabPageProfile.Text = "Profile View";
            this.tabPageProfile.UseVisualStyleBackColor = true;
            // 
            // winChart_proflie
            // 
            this.winChart_proflie.BackColor = System.Drawing.SystemColors.Control;
            this.winChart_proflie.ClearExistesSeries = true;
            this.winChart_proflie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_proflie.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.winChart_proflie.Location = new System.Drawing.Point(3, 3);
            this.winChart_proflie.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChart_proflie.Name = "winChart_proflie";
            this.winChart_proflie.ShowStatPanel = true;
            this.winChart_proflie.Size = new System.Drawing.Size(850, 564);
            this.winChart_proflie.TabIndex = 8;
            // 
            // colorSlider1
            // 
            this.colorSlider1.BackColor = System.Drawing.Color.Transparent;
            this.colorSlider1.BarInnerColor = System.Drawing.Color.GhostWhite;
            this.colorSlider1.BarOuterColor = System.Drawing.Color.White;
            this.colorSlider1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.colorSlider1.ElapsedInnerColor = System.Drawing.Color.DeepSkyBlue;
            this.colorSlider1.ElapsedOuterColor = System.Drawing.Color.White;
            this.colorSlider1.LargeChange = ((uint)(5u));
            this.colorSlider1.Location = new System.Drawing.Point(3, 567);
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.Size = new System.Drawing.Size(850, 23);
            this.colorSlider1.SmallChange = ((uint)(1u));
            this.colorSlider1.TabIndex = 6;
            this.colorSlider1.Text = "50";
            this.colorSlider1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.ThumbSize = 30;
            this.colorSlider1.Value = 0;
            this.colorSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider1_Scroll);
            // 
            // tabPageFeatureLayer
            // 
            this.tabPageFeatureLayer.Controls.Add(this.groupBox3);
            this.tabPageFeatureLayer.Controls.Add(this.groupBox1);
            this.tabPageFeatureLayer.Controls.Add(this.btnShowLayer);
            this.tabPageFeatureLayer.Location = new System.Drawing.Point(4, 4);
            this.tabPageFeatureLayer.Name = "tabPageFeatureLayer";
            this.tabPageFeatureLayer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFeatureLayer.Size = new System.Drawing.Size(856, 593);
            this.tabPageFeatureLayer.TabIndex = 2;
            this.tabPageFeatureLayer.Text = "Feature Layer View";
            this.tabPageFeatureLayer.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cmbDates);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(10, 246);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(827, 70);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Date";
            // 
            // cmbDates
            // 
            this.cmbDates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDates.FormattingEnabled = true;
            this.cmbDates.Location = new System.Drawing.Point(170, 26);
            this.cmbDates.Name = "cmbDates";
            this.cmbDates.Size = new System.Drawing.Size(302, 27);
            this.cmbDates.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(79, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 19);
            this.label9.TabIndex = 3;
            this.label9.Text = "Select date:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbReachFields);
            this.groupBox1.Controls.Add(this.cmbSegFields);
            this.groupBox1.Controls.Add(this.cmbLayers);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.checkBoxSaveLayer);
            this.groupBox1.Controls.Add(this.btnRefreshLayer);
            this.groupBox1.Location = new System.Drawing.Point(10, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(827, 210);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Feature Layer";
            // 
            // cmbReachFields
            // 
            this.cmbReachFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbReachFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReachFields.FormattingEnabled = true;
            this.cmbReachFields.Location = new System.Drawing.Point(170, 114);
            this.cmbReachFields.Name = "cmbReachFields";
            this.cmbReachFields.Size = new System.Drawing.Size(302, 27);
            this.cmbReachFields.TabIndex = 0;
            // 
            // cmbSegFields
            // 
            this.cmbSegFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSegFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSegFields.FormattingEnabled = true;
            this.cmbSegFields.Location = new System.Drawing.Point(170, 69);
            this.cmbSegFields.Name = "cmbSegFields";
            this.cmbSegFields.Size = new System.Drawing.Size(302, 27);
            this.cmbSegFields.TabIndex = 0;
            // 
            // cmbLayers
            // 
            this.cmbLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayers.FormattingEnabled = true;
            this.cmbLayers.Location = new System.Drawing.Point(170, 24);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(302, 27);
            this.cmbLayers.TabIndex = 0;
            this.cmbLayers.SelectedIndexChanged += new System.EventHandler(this.cmbLayers_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(61, 114);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 19);
            this.label11.TabIndex = 1;
            this.label11.Text = "Reach ID field:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(45, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 19);
            this.label10.TabIndex = 1;
            this.label10.Text = "Segment ID field:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(139, 19);
            this.label8.TabIndex = 1;
            this.label8.Text = "Select feature layer:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBoxSaveLayer
            // 
            this.checkBoxSaveLayer.AutoSize = true;
            this.checkBoxSaveLayer.Location = new System.Drawing.Point(29, 166);
            this.checkBoxSaveLayer.Name = "checkBoxSaveLayer";
            this.checkBoxSaveLayer.Size = new System.Drawing.Size(282, 23);
            this.checkBoxSaveLayer.TabIndex = 6;
            this.checkBoxSaveLayer.Text = "Save data to the selected feature layer";
            this.checkBoxSaveLayer.UseVisualStyleBackColor = true;
            // 
            // btnRefreshLayer
            // 
            this.btnRefreshLayer.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiLayer24;
            this.btnRefreshLayer.Location = new System.Drawing.Point(490, 22);
            this.btnRefreshLayer.Name = "btnRefreshLayer";
            this.btnRefreshLayer.Size = new System.Drawing.Size(171, 30);
            this.btnRefreshLayer.TabIndex = 5;
            this.btnRefreshLayer.Text = "Refresh layer list";
            this.btnRefreshLayer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefreshLayer.UseVisualStyleBackColor = true;
            this.btnRefreshLayer.Click += new System.EventHandler(this.btnRefreshLayer_Click);
            // 
            // btnShowLayer
            // 
            this.btnShowLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowLayer.Image = global::Heiflow.Controls.WinForm.Properties.Resources.UiLayer24;
            this.btnShowLayer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnShowLayer.Location = new System.Drawing.Point(39, 337);
            this.btnShowLayer.Name = "btnShowLayer";
            this.btnShowLayer.Size = new System.Drawing.Size(302, 38);
            this.btnShowLayer.TabIndex = 4;
            this.btnShowLayer.Text = "Show on GIS Layer";
            this.btnShowLayer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnShowLayer.UseVisualStyleBackColor = true;
            this.btnShowLayer.Click += new System.EventHandler(this.btnShowLayer_Click);
            // 
            // SFRExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "SFRExplorer";
            this.Size = new System.Drawing.Size(1132, 653);
            this.Load += new System.EventHandler(this.SFRExplorer_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlLeft.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.grpSimulated.ResumeLayout(false);
            this.grpSimulated.PerformLayout();
            this.grpObservation.ResumeLayout(false);
            this.grpObservation.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabControl_Chart.ResumeLayout(false);
            this.tabPageTimeSeries.ResumeLayout(false);
            this.tabPageProfile.ResumeLayout(false);
            this.tabPageFeatureLayer.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripComboBox cmbSFRVars;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TabControl tabControlLeft;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox grpSimulated;
        private System.Windows.Forms.GroupBox grpObservation;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripLabel labelStatus;
        private System.Windows.Forms.ComboBox cmbObsVars;
        private System.Windows.Forms.ComboBox cmbSite;
        private System.Windows.Forms.ComboBox cmbRchID;
        private System.Windows.Forms.ComboBox cmbSegsID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbEndID;
        private System.Windows.Forms.ComboBox cmbStartID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabControl tabControl_Chart;
        private System.Windows.Forms.TabPage tabPageTimeSeries;
        private WinChart winChart_timeseries;
        private System.Windows.Forms.TabPage tabPageProfile;
        private WinChart winChart_proflie;
        private ColorSlider colorSlider1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.CheckBox chbUnifiedByLength;
        private System.Windows.Forms.ToolStripButton tbnSlctDataSource;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportRiversToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportReachesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsShpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem segmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reachesToolStripMenuItem;
        private System.Windows.Forms.Button btnAdd2Toolbox;
        private System.Windows.Forms.Button btnAddSfrMat2Toolbox;
        private System.Windows.Forms.ToolStripButton btnScan;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripComboBox cmbLoadVar;
        private System.Windows.Forms.ToolStripLabel toolStripButton2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnShowLayer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbDates;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbLayers;
        private System.Windows.Forms.Button btnRefreshLayer;
        private System.Windows.Forms.ToolStripMenuItem exportToSWMMInpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem riverJunctionsToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageFeatureLayer;
        private System.Windows.Forms.ToolStripButton chbReadComplData;
        private System.Windows.Forms.CheckBox checkBoxSaveLayer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbReachFields;
        private System.Windows.Forms.ComboBox cmbSegFields;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbCurDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripDropDownButton chk_datasource;
        private System.Windows.Forms.ToolStripMenuItem mi_nps;
        private System.Windows.Forms.ToolStripMenuItem mi_month_npc;
        private System.Windows.Forms.ToolStripMenuItem mi_flow;
        private System.Windows.Forms.ToolStripMenuItem mi_sediment;
    }
}
