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
            this.toolStripButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.menu_LoadAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_LoadSingle = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmbSFRVars = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tbnSlctDataSource = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRiversToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportReachesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsShpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.segmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reachesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.labelStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.cmbVariables = new System.Windows.Forms.ToolStripComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnAddSfrMat2Toolbox = new System.Windows.Forms.Button();
            this.chbReadComplData = new System.Windows.Forms.CheckBox();
            this.cmbObsVars = new System.Windows.Forms.ComboBox();
            this.cmbSite = new System.Windows.Forms.ComboBox();
            this.cmbRchID = new System.Windows.Forms.ComboBox();
            this.cmbSegsID = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAdd2Toolbox = new System.Windows.Forms.Button();
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
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl_Chart.SuspendLayout();
            this.tabPageTimeSeries.SuspendLayout();
            this.tabPageProfile.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.btnLoad,
            this.toolStripLabel1,
            this.cmbSFRVars,
            this.toolStripButton1,
            this.btnRefresh,
            this.tbnSlctDataSource,
            this.toolStripSeparator2,
            this.toolStripDropDownButton1,
            this.toolStripSeparator1,
            this.toolStripProgressBar1,
            this.labelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1132, 28);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_LoadAll,
            this.menu_LoadSingle,
            this.menu_Clear});
            this.toolStripButton2.Image = global::Heiflow.Controls.WinForm.Properties.Resources.setting32;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(34, 25);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // menu_LoadAll
            // 
            this.menu_LoadAll.Checked = true;
            this.menu_LoadAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menu_LoadAll.Name = "menu_LoadAll";
            this.menu_LoadAll.Size = new System.Drawing.Size(216, 26);
            this.menu_LoadAll.Text = "Load all variables";
            this.menu_LoadAll.Click += new System.EventHandler(this.menu_LoadAll_Click);
            // 
            // menu_LoadSingle
            // 
            this.menu_LoadSingle.Name = "menu_LoadSingle";
            this.menu_LoadSingle.Size = new System.Drawing.Size(216, 26);
            this.menu_LoadSingle.Text = "Load singel variable";
            this.menu_LoadSingle.Click += new System.EventHandler(this.menu_LoadSingle_Click);
            // 
            // menu_Clear
            // 
            this.menu_Clear.Name = "menu_Clear";
            this.menu_Clear.Size = new System.Drawing.Size(216, 26);
            this.menu_Clear.Text = "Clear loaded data";
            this.menu_Clear.Click += new System.EventHandler(this.menu_Clear_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoad.Image = global::Heiflow.Controls.WinForm.Properties.Resources.Load24;
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(24, 25);
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(130, 25);
            this.toolStripLabel1.Text = "Simulated Variable";
            // 
            // cmbSFRVars
            // 
            this.cmbSFRVars.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSFRVars.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cmbSFRVars.Name = "cmbSFRVars";
            this.cmbSFRVars.Size = new System.Drawing.Size(200, 28);
            this.cmbSFRVars.SelectedIndexChanged += new System.EventHandler(this.cmbSFRVars_SelectedIndexChanged);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 28);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::Heiflow.Controls.WinForm.Properties.Resources.refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 25);
            this.btnRefresh.Text = "Refresh data source";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // tbnSlctDataSource
            // 
            this.tbnSlctDataSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbnSlctDataSource.Image = global::Heiflow.Controls.WinForm.Properties.Resources.DatabaseServer16;
            this.tbnSlctDataSource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbnSlctDataSource.Name = "tbnSlctDataSource";
            this.tbnSlctDataSource.Size = new System.Drawing.Size(24, 25);
            this.tbnSlctDataSource.Text = "Select Data Source";
            this.tbnSlctDataSource.Click += new System.EventHandler(this.tbnSlctDataSource_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem1,
            this.saveAsShpToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericSave_B_16;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 25);
            this.toolStripDropDownButton1.Text = "Export";
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportRiversToolStripMenuItem,
            this.exportReachesToolStripMenuItem,
            this.exportProfileToolStripMenuItem});
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(176, 26);
            this.exportToolStripMenuItem1.Text = "Export As Text";
            // 
            // exportRiversToolStripMenuItem
            // 
            this.exportRiversToolStripMenuItem.Name = "exportRiversToolStripMenuItem";
            this.exportRiversToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.exportRiversToolStripMenuItem.Text = "Segments";
            this.exportRiversToolStripMenuItem.Click += new System.EventHandler(this.exportRiversToolStripMenuItem_Click);
            // 
            // exportReachesToolStripMenuItem
            // 
            this.exportReachesToolStripMenuItem.Name = "exportReachesToolStripMenuItem";
            this.exportReachesToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.exportReachesToolStripMenuItem.Text = "Reaches";
            // 
            // exportProfileToolStripMenuItem
            // 
            this.exportProfileToolStripMenuItem.Name = "exportProfileToolStripMenuItem";
            this.exportProfileToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.exportProfileToolStripMenuItem.Text = "Profile";
            this.exportProfileToolStripMenuItem.Click += new System.EventHandler(this.exportProfileToolStripMenuItem_Click);
            // 
            // saveAsShpToolStripMenuItem
            // 
            this.saveAsShpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.segmentsToolStripMenuItem,
            this.reachesToolStripMenuItem});
            this.saveAsShpToolStripMenuItem.Name = "saveAsShpToolStripMenuItem";
            this.saveAsShpToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.saveAsShpToolStripMenuItem.Text = "Save As Shp";
            // 
            // segmentsToolStripMenuItem
            // 
            this.segmentsToolStripMenuItem.Name = "segmentsToolStripMenuItem";
            this.segmentsToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.segmentsToolStripMenuItem.Text = "Segments";
            this.segmentsToolStripMenuItem.Click += new System.EventHandler(this.segmentsToolStripMenuItem_Click);
            // 
            // reachesToolStripMenuItem
            // 
            this.reachesToolStripMenuItem.Name = "reachesToolStripMenuItem";
            this.reachesToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.reachesToolStripMenuItem.Text = "Reaches";
            this.reachesToolStripMenuItem.Click += new System.EventHandler(this.reachesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(300, 25);
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(49, 25);
            this.labelStatus.Text = "Ready";
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(83, 25);
            this.toolStripLabel4.Text = "Segment ID";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(67, 25);
            this.toolStripLabel5.Text = "Reach ID";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(33, 25);
            this.toolStripLabel6.Text = "Site";
            // 
            // cmbVariables
            // 
            this.cmbVariables.Name = "cmbVariables";
            this.cmbVariables.Size = new System.Drawing.Size(121, 28);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl_Chart);
            this.splitContainer1.Size = new System.Drawing.Size(1132, 625);
            this.splitContainer1.SplitterDistance = 265;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(265, 625);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnAddSfrMat2Toolbox);
            this.tabPage3.Controls.Add(this.chbReadComplData);
            this.tabPage3.Controls.Add(this.cmbObsVars);
            this.tabPage3.Controls.Add(this.cmbSite);
            this.tabPage3.Controls.Add(this.cmbRchID);
            this.tabPage3.Controls.Add(this.cmbSegsID);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.label1);
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
            this.btnAddSfrMat2Toolbox.Location = new System.Drawing.Point(9, 307);
            this.btnAddSfrMat2Toolbox.Name = "btnAddSfrMat2Toolbox";
            this.btnAddSfrMat2Toolbox.Size = new System.Drawing.Size(169, 31);
            this.btnAddSfrMat2Toolbox.TabIndex = 13;
            this.btnAddSfrMat2Toolbox.Text = "Add to Toolbox";
            this.btnAddSfrMat2Toolbox.UseVisualStyleBackColor = true;
            this.btnAddSfrMat2Toolbox.Click += new System.EventHandler(this.btnAddSfrMat2Toolbox_Click);
            // 
            // chbReadComplData
            // 
            this.chbReadComplData.AutoSize = true;
            this.chbReadComplData.Location = new System.Drawing.Point(10, 264);
            this.chbReadComplData.Name = "chbReadComplData";
            this.chbReadComplData.Size = new System.Drawing.Size(159, 23);
            this.chbReadComplData.TabIndex = 10;
            this.chbReadComplData.Text = "Load complete data";
            this.chbReadComplData.UseVisualStyleBackColor = true;
            this.chbReadComplData.CheckedChanged += new System.EventHandler(this.chbReadComplData_CheckedChanged);
            // 
            // cmbObsVars
            // 
            this.cmbObsVars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbObsVars.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbObsVars.FormattingEnabled = true;
            this.cmbObsVars.Location = new System.Drawing.Point(6, 222);
            this.cmbObsVars.Name = "cmbObsVars";
            this.cmbObsVars.Size = new System.Drawing.Size(235, 27);
            this.cmbObsVars.TabIndex = 6;
            this.cmbObsVars.SelectedIndexChanged += new System.EventHandler(this.cmbObsVars_SelectedIndexChanged);
            // 
            // cmbSite
            // 
            this.cmbSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSite.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSite.FormattingEnabled = true;
            this.cmbSite.Location = new System.Drawing.Point(6, 162);
            this.cmbSite.Name = "cmbSite";
            this.cmbSite.Size = new System.Drawing.Size(235, 27);
            this.cmbSite.TabIndex = 7;
            this.cmbSite.SelectedIndexChanged += new System.EventHandler(this.cmbSite_SelectedIndexChanged);
            // 
            // cmbRchID
            // 
            this.cmbRchID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRchID.Enabled = false;
            this.cmbRchID.FormattingEnabled = true;
            this.cmbRchID.Location = new System.Drawing.Point(6, 102);
            this.cmbRchID.Name = "cmbRchID";
            this.cmbRchID.Size = new System.Drawing.Size(235, 27);
            this.cmbRchID.TabIndex = 8;
            this.cmbRchID.SelectedIndexChanged += new System.EventHandler(this.cmbRchID_SelectedIndexChanged);
            // 
            // cmbSegsID
            // 
            this.cmbSegsID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSegsID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSegsID.FormattingEnabled = true;
            this.cmbSegsID.Location = new System.Drawing.Point(6, 42);
            this.cmbSegsID.Name = "cmbSegsID";
            this.cmbSegsID.Size = new System.Drawing.Size(235, 27);
            this.cmbSegsID.TabIndex = 9;
            this.cmbSegsID.SelectedIndexChanged += new System.EventHandler(this.cmbSegsID_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 19);
            this.label5.TabIndex = 2;
            this.label5.Text = "Observed Variable";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "Observation Site";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "Reach ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Segment ID";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(257, 593);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Profile";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAdd2Toolbox);
            this.groupBox2.Controls.Add(this.chbUnifiedByLength);
            this.groupBox2.Controls.Add(this.cmbEndID);
            this.groupBox2.Controls.Add(this.cmbStartID);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 587);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // btnAdd2Toolbox
            // 
            this.btnAdd2Toolbox.Location = new System.Drawing.Point(10, 206);
            this.btnAdd2Toolbox.Name = "btnAdd2Toolbox";
            this.btnAdd2Toolbox.Size = new System.Drawing.Size(169, 31);
            this.btnAdd2Toolbox.TabIndex = 12;
            this.btnAdd2Toolbox.Text = "Add to Toolbox";
            this.btnAdd2Toolbox.UseVisualStyleBackColor = true;
            this.btnAdd2Toolbox.Click += new System.EventHandler(this.btnAdd2Toolbox_Click);
            // 
            // chbUnifiedByLength
            // 
            this.chbUnifiedByLength.AutoSize = true;
            this.chbUnifiedByLength.Location = new System.Drawing.Point(10, 157);
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
            this.cmbEndID.Size = new System.Drawing.Size(235, 27);
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
            this.cmbStartID.Size = new System.Drawing.Size(235, 27);
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
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(257, 596);
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
            this.propertyGrid1.Size = new System.Drawing.Size(251, 590);
            this.propertyGrid1.TabIndex = 1;
            // 
            // tabControl_Chart
            // 
            this.tabControl_Chart.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl_Chart.Controls.Add(this.tabPageTimeSeries);
            this.tabControl_Chart.Controls.Add(this.tabPageProfile);
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
            this.winChart_timeseries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_timeseries.Font = new System.Drawing.Font("Calibri", 9.5F);
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
            this.tabPageProfile.Size = new System.Drawing.Size(856, 596);
            this.tabPageProfile.TabIndex = 1;
            this.tabPageProfile.Text = "Profile View";
            this.tabPageProfile.UseVisualStyleBackColor = true;
            // 
            // winChart_proflie
            // 
            this.winChart_proflie.BackColor = System.Drawing.SystemColors.Control;
            this.winChart_proflie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_proflie.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart_proflie.Location = new System.Drawing.Point(3, 3);
            this.winChart_proflie.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChart_proflie.Name = "winChart_proflie";
            this.winChart_proflie.ShowStatPanel = true;
            this.winChart_proflie.Size = new System.Drawing.Size(850, 567);
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
            this.colorSlider1.Location = new System.Drawing.Point(3, 570);
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
            // SFRExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Name = "SFRExplorer";
            this.Size = new System.Drawing.Size(1132, 653);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabControl_Chart.ResumeLayout(false);
            this.tabPageTimeSeries.ResumeLayout(false);
            this.tabPageProfile.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripComboBox cmbSFRVars;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripComboBox cmbVariables;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
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
        private System.Windows.Forms.GroupBox groupBox2;
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
        private System.Windows.Forms.CheckBox chbReadComplData;
        private System.Windows.Forms.CheckBox chbUnifiedByLength;
        private System.Windows.Forms.ToolStripButton tbnSlctDataSource;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportRiversToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportReachesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsShpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem segmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reachesToolStripMenuItem;
        private System.Windows.Forms.Button btnAdd2Toolbox;
        private System.Windows.Forms.Button btnAddSfrMat2Toolbox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton2;
        private System.Windows.Forms.ToolStripMenuItem menu_LoadAll;
        private System.Windows.Forms.ToolStripMenuItem menu_LoadSingle;
        private System.Windows.Forms.ToolStripMenuItem menu_Clear;
    }
}
