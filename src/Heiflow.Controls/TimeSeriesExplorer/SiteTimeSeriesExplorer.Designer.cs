namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    partial class SiteTimeSeriesExplorer
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
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmbSimVars = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.tbnSlctDataSource = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCompareMode = new System.Windows.Forms.ToolStripButton();
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
            this.cmbObsVars = new System.Windows.Forms.ComboBox();
            this.cmbObsSite = new System.Windows.Forms.ComboBox();
            this.cmbSimSite = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabControl_Chart = new System.Windows.Forms.TabControl();
            this.tabPageTimeSeries = new System.Windows.Forms.TabPage();
            this.winChart_timeseries = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl_Chart.SuspendLayout();
            this.tabPageTimeSeries.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad,
            this.toolStripLabel1,
            this.cmbSimVars,
            this.toolStripButton1,
            this.btnRefresh,
            this.tbnSlctDataSource,
            this.toolStripSeparator1,
            this.btnCompareMode,
            this.toolStripProgressBar1,
            this.labelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1125, 29);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLoad
            // 
            this.btnLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoad.Image = global::Heiflow.Controls.WinForm.Properties.Resources.Load24;
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(24, 26);
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(134, 26);
            this.toolStripLabel1.Text = "Simulated Variable";
            // 
            // cmbSimVars
            // 
            this.cmbSimVars.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSimVars.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.cmbSimVars.Name = "cmbSimVars";
            this.cmbSimVars.Size = new System.Drawing.Size(200, 29);
            this.cmbSimVars.SelectedIndexChanged += new System.EventHandler(this.cmbSimVars_SelectedIndexChanged);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 29);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::Heiflow.Controls.WinForm.Properties.Resources.refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 26);
            this.btnRefresh.Text = "Refresh data source";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // tbnSlctDataSource
            // 
            this.tbnSlctDataSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbnSlctDataSource.Image = global::Heiflow.Controls.WinForm.Properties.Resources.DatabaseServer16;
            this.tbnSlctDataSource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbnSlctDataSource.Name = "tbnSlctDataSource";
            this.tbnSlctDataSource.Size = new System.Drawing.Size(24, 26);
            this.tbnSlctDataSource.Text = "Select Data Source";
            this.tbnSlctDataSource.Click += new System.EventHandler(this.tbnSlctDataSource_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // btnCompareMode
            // 
            this.btnCompareMode.Checked = true;
            this.btnCompareMode.CheckOnClick = true;
            this.btnCompareMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnCompareMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompareMode.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GeostatisticalGraphGeneralQQPlot16;
            this.btnCompareMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompareMode.Name = "btnCompareMode";
            this.btnCompareMode.Size = new System.Drawing.Size(24, 26);
            this.btnCompareMode.Text = "Compare Mode";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(300, 26);
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(50, 26);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 29);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl_Chart);
            this.splitContainer1.Size = new System.Drawing.Size(1125, 532);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(263, 532);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cmbObsVars);
            this.tabPage3.Controls.Add(this.cmbObsSite);
            this.tabPage3.Controls.Add(this.cmbSimSite);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(255, 499);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Time Series";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cmbObsVars
            // 
            this.cmbObsVars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbObsVars.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbObsVars.FormattingEnabled = true;
            this.cmbObsVars.Location = new System.Drawing.Point(6, 178);
            this.cmbObsVars.Name = "cmbObsVars";
            this.cmbObsVars.Size = new System.Drawing.Size(233, 28);
            this.cmbObsVars.TabIndex = 6;
            this.cmbObsVars.SelectedIndexChanged += new System.EventHandler(this.cmbObsVars_SelectedIndexChanged);
            // 
            // cmbObsSite
            // 
            this.cmbObsSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbObsSite.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbObsSite.FormattingEnabled = true;
            this.cmbObsSite.Location = new System.Drawing.Point(6, 115);
            this.cmbObsSite.Name = "cmbObsSite";
            this.cmbObsSite.Size = new System.Drawing.Size(233, 28);
            this.cmbObsSite.TabIndex = 7;
            this.cmbObsSite.SelectedIndexChanged += new System.EventHandler(this.cmbObsSite_SelectedIndexChanged);
            // 
            // cmbSimSite
            // 
            this.cmbSimSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSimSite.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSimSite.FormattingEnabled = true;
            this.cmbSimSite.Location = new System.Drawing.Point(6, 44);
            this.cmbSimSite.Name = "cmbSimSite";
            this.cmbSimSite.Size = new System.Drawing.Size(233, 28);
            this.cmbSimSite.TabIndex = 9;
            this.cmbSimSite.SelectedIndexChanged += new System.EventHandler(this.cmbSimSite_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 20);
            this.label5.TabIndex = 2;
            this.label5.Text = "Observed Variable";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Observation Site";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Simulated Site";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propertyGrid1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(255, 499);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Config";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(249, 493);
            this.propertyGrid1.TabIndex = 1;
            // 
            // tabControl_Chart
            // 
            this.tabControl_Chart.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl_Chart.Controls.Add(this.tabPageTimeSeries);
            this.tabControl_Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Chart.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Chart.Name = "tabControl_Chart";
            this.tabControl_Chart.SelectedIndex = 0;
            this.tabControl_Chart.Size = new System.Drawing.Size(859, 532);
            this.tabControl_Chart.TabIndex = 6;
            // 
            // tabPageTimeSeries
            // 
            this.tabPageTimeSeries.Controls.Add(this.winChart_timeseries);
            this.tabPageTimeSeries.Location = new System.Drawing.Point(4, 4);
            this.tabPageTimeSeries.Name = "tabPageTimeSeries";
            this.tabPageTimeSeries.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTimeSeries.Size = new System.Drawing.Size(851, 499);
            this.tabPageTimeSeries.TabIndex = 0;
            this.tabPageTimeSeries.Text = "Time Series View";
            this.tabPageTimeSeries.UseVisualStyleBackColor = true;
            // 
            // winChart_timeseries
            // 
            this.winChart_timeseries.BackColor = System.Drawing.SystemColors.Control;
            this.winChart_timeseries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_timeseries.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.winChart_timeseries.Location = new System.Drawing.Point(3, 3);
            this.winChart_timeseries.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChart_timeseries.Name = "winChart_timeseries";
            this.winChart_timeseries.ShowStatPanel = true;
            this.winChart_timeseries.Size = new System.Drawing.Size(845, 493);
            this.winChart_timeseries.TabIndex = 7;
            // 
            // SiteTimeSeriesExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "SiteTimeSeriesExplorer";
            this.Size = new System.Drawing.Size(1125, 561);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabControl_Chart.ResumeLayout(false);
            this.tabPageTimeSeries.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripComboBox cmbSimVars;
        private System.Windows.Forms.SplitContainer splitContainer1;
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
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripLabel labelStatus;
        private System.Windows.Forms.ComboBox cmbObsVars;
        private System.Windows.Forms.ComboBox cmbObsSite;
        private System.Windows.Forms.ComboBox cmbSimSite;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabControl tabControl_Chart;
        private System.Windows.Forms.TabPage tabPageTimeSeries;
        private Controls.WinChart winChart_timeseries;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton tbnSlctDataSource;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnCompareMode;
    }
}
