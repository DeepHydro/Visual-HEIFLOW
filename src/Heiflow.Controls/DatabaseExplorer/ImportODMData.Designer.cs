namespace Heiflow.Controls.WinForm.DatabaseExplorer
{
    partial class ImportODMData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportODMData));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.nav_top = new System.Windows.Forms.BindingNavigator(this.components);
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.cmbTables = new System.Windows.Forms.ToolStripComboBox();
            this.btnOpenExcel = new System.Windows.Forms.ToolStripButton();
            this.cmbSheet = new System.Windows.Forms.ToolStripComboBox();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_ShowScript = new System.Windows.Forms.ToolStripButton();
            this.btn_script = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripDropDownButton();
            this.defaultExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUpdateSeriesCata = new System.Windows.Forms.ToolStripButton();
            this.bindingSourceODM = new System.Windows.Forms.BindingSource(this.components);
            this.nav_bottom = new System.Windows.Forms.BindingNavigator(this.components);
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.labelStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lb_File = new System.Windows.Forms.ToolStripLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dg_odm = new System.Windows.Forms.DataGridView();
            this.tb_script = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dg_external = new System.Windows.Forms.DataGridView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.nav_top)).BeginInit();
            this.nav_top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceODM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nav_bottom)).BeginInit();
            this.nav_bottom.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_odm)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_external)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // nav_top
            // 
            this.nav_top.AddNewItem = null;
            this.nav_top.CountItem = null;
            this.nav_top.DeleteItem = null;
            this.nav_top.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.nav_top.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.cmbTables,
            this.btnOpenExcel,
            this.cmbSheet,
            this.btnImport,
            this.toolStripSeparator1,
            this.btn_ShowScript,
            this.btn_script,
            this.btnExport,
            this.btnUpdateSeriesCata});
            this.nav_top.Location = new System.Drawing.Point(0, 0);
            this.nav_top.MoveFirstItem = null;
            this.nav_top.MoveLastItem = null;
            this.nav_top.MoveNextItem = null;
            this.nav_top.MovePreviousItem = null;
            this.nav_top.Name = "nav_top";
            this.nav_top.PositionItem = null;
            this.nav_top.Size = new System.Drawing.Size(1119, 28);
            this.nav_top.TabIndex = 1;
            this.nav_top.Text = "bindingNavigator1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(83, 25);
            this.toolStripLabel2.Text = "Import to:";
            // 
            // cmbTables
            // 
            this.cmbTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTables.Name = "cmbTables";
            this.cmbTables.Size = new System.Drawing.Size(150, 28);
            this.cmbTables.SelectedIndexChanged += new System.EventHandler(this.cmbTables_SelectedIndexChanged);
            // 
            // btnOpenExcel
            // 
            this.btnOpenExcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenExcel.Image = global::Heiflow.Controls.WinForm.Properties.Resources.excel_32;
            this.btnOpenExcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenExcel.Name = "btnOpenExcel";
            this.btnOpenExcel.Size = new System.Drawing.Size(24, 25);
            this.btnOpenExcel.Text = "Open Excel File";
            this.btnOpenExcel.Click += new System.EventHandler(this.btnOpenExcel_Click);
            // 
            // cmbSheet
            // 
            this.cmbSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSheet.Name = "cmbSheet";
            this.cmbSheet.Size = new System.Drawing.Size(150, 28);
            this.cmbSheet.SelectedIndexChanged += new System.EventHandler(this.cmbSheet_SelectedIndexChanged);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.TmImportFeatures16;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(24, 25);
            this.btnImport.Text = "Import to ODM Database";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // btn_ShowScript
            // 
            this.btn_ShowScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_ShowScript.Image = global::Heiflow.Controls.WinForm.Properties.Resources.script;
            this.btn_ShowScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_ShowScript.Name = "btn_ShowScript";
            this.btn_ShowScript.Size = new System.Drawing.Size(24, 25);
            this.btn_ShowScript.Text = "Show Script Editor";
            this.btn_ShowScript.Click += new System.EventHandler(this.btn_ShowScript_Click);
            // 
            // btn_script
            // 
            this.btn_script.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_script.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GeoprocessingScriptUnfilled16;
            this.btn_script.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_script.Name = "btn_script";
            this.btn_script.Size = new System.Drawing.Size(24, 25);
            this.btn_script.Text = "Run SQL Script";
            this.btn_script.Click += new System.EventHandler(this.btn_script_Click);
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultExportToolStripMenuItem,
            this.customExportToolStripMenuItem});
            this.btnExport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.TableExport16;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(34, 25);
            this.btnExport.Text = "Export Data";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // defaultExportToolStripMenuItem
            // 
            this.defaultExportToolStripMenuItem.Name = "defaultExportToolStripMenuItem";
            this.defaultExportToolStripMenuItem.Size = new System.Drawing.Size(188, 26);
            this.defaultExportToolStripMenuItem.Text = "Default Export";
            this.defaultExportToolStripMenuItem.Click += new System.EventHandler(this.defaultExportToolStripMenuItem_Click);
            // 
            // customExportToolStripMenuItem
            // 
            this.customExportToolStripMenuItem.Name = "customExportToolStripMenuItem";
            this.customExportToolStripMenuItem.Size = new System.Drawing.Size(188, 26);
            this.customExportToolStripMenuItem.Text = "Batch Export...";
            this.customExportToolStripMenuItem.Click += new System.EventHandler(this.customExportToolStripMenuItem_Click);
            // 
            // btnUpdateSeriesCata
            // 
            this.btnUpdateSeriesCata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUpdateSeriesCata.Image = global::Heiflow.Controls.WinForm.Properties.Resources.refresh;
            this.btnUpdateSeriesCata.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpdateSeriesCata.Name = "btnUpdateSeriesCata";
            this.btnUpdateSeriesCata.Size = new System.Drawing.Size(24, 25);
            this.btnUpdateSeriesCata.Text = "toolStripButton3";
            this.btnUpdateSeriesCata.ToolTipText = "Update SeriesCatalog";
            this.btnUpdateSeriesCata.Click += new System.EventHandler(this.btnUpdateSeriesCata_Click);
            // 
            // nav_bottom
            // 
            this.nav_bottom.AddNewItem = this.toolStripButton1;
            this.nav_bottom.BindingSource = this.bindingSourceODM;
            this.nav_bottom.CountItem = this.toolStripLabel3;
            this.nav_bottom.DeleteItem = this.toolStripButton2;
            this.nav_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.nav_bottom.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.nav_bottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripSeparator5,
            this.toolStripTextBox1,
            this.toolStripLabel3,
            this.toolStripSeparator6,
            this.toolStripButton7,
            this.toolStripButton8,
            this.toolStripSeparator7,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripProgressBar1,
            this.labelStatus,
            this.toolStripSeparator2,
            this.lb_File});
            this.nav_bottom.Location = new System.Drawing.Point(0, 567);
            this.nav_bottom.MoveFirstItem = this.toolStripButton5;
            this.nav_bottom.MoveLastItem = this.toolStripButton8;
            this.nav_bottom.MoveNextItem = this.toolStripButton7;
            this.nav_bottom.MovePreviousItem = this.toolStripButton6;
            this.nav_bottom.Name = "nav_bottom";
            this.nav_bottom.PositionItem = this.toolStripTextBox1;
            this.nav_bottom.Size = new System.Drawing.Size(1119, 27);
            this.nav_bottom.TabIndex = 2;
            this.nav_bottom.Text = "bindingNavigator2";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.RightToLeftAutoMirrorImage = true;
            this.toolStripButton1.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton1.Text = "新添";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(38, 24);
            this.toolStripLabel3.Text = "/ {0}";
            this.toolStripLabel3.ToolTipText = "总项数";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.RightToLeftAutoMirrorImage = true;
            this.toolStripButton2.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton2.Text = "删除";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.RightToLeftAutoMirrorImage = true;
            this.toolStripButton5.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton5.Text = "移到第一条记录";
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.RightToLeftAutoMirrorImage = true;
            this.toolStripButton6.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton6.Text = "移到上一条记录";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.AccessibleName = "位置";
            this.toolStripTextBox1.AutoSize = false;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(50, 27);
            this.toolStripTextBox1.Text = "0";
            this.toolStripTextBox1.ToolTipText = "当前位置";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.RightToLeftAutoMirrorImage = true;
            this.toolStripButton7.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton7.Text = "移到下一条记录";
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.RightToLeftAutoMirrorImage = true;
            this.toolStripButton8.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton8.Text = "移到最后一条记录";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 24);
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(54, 24);
            this.labelStatus.Text = "Ready";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // lb_File
            // 
            this.lb_File.Name = "lb_File";
            this.lb_File.Size = new System.Drawing.Size(34, 24);
            this.lb_File.Text = "File";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(952, 539);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(944, 507);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Database";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dg_odm);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tb_script);
            this.splitContainer1.Size = new System.Drawing.Size(938, 501);
            this.splitContainer1.SplitterDistance = 417;
            this.splitContainer1.TabIndex = 5;
            // 
            // dg_odm
            // 
            this.dg_odm.AllowUserToAddRows = false;
            this.dg_odm.AllowUserToDeleteRows = false;
            this.dg_odm.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dg_odm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dg_odm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 9.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightGreen;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_odm.DefaultCellStyle = dataGridViewCellStyle1;
            this.dg_odm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_odm.Location = new System.Drawing.Point(0, 0);
            this.dg_odm.Margin = new System.Windows.Forms.Padding(4);
            this.dg_odm.Name = "dg_odm";
            this.dg_odm.RowTemplate.Height = 23;
            this.dg_odm.Size = new System.Drawing.Size(938, 417);
            this.dg_odm.TabIndex = 4;
            // 
            // tb_script
            // 
            this.tb_script.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_script.Location = new System.Drawing.Point(0, 0);
            this.tb_script.Multiline = true;
            this.tb_script.Name = "tb_script";
            this.tb_script.Size = new System.Drawing.Size(938, 80);
            this.tb_script.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dg_external);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(944, 507);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "External Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dg_external
            // 
            this.dg_external.AllowUserToAddRows = false;
            this.dg_external.AllowUserToDeleteRows = false;
            this.dg_external.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dg_external.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dg_external.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 9.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightGreen;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_external.DefaultCellStyle = dataGridViewCellStyle2;
            this.dg_external.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_external.Location = new System.Drawing.Point(3, 3);
            this.dg_external.Margin = new System.Windows.Forms.Padding(4);
            this.dg_external.Name = "dg_external";
            this.dg_external.RowTemplate.Height = 23;
            this.dg_external.Size = new System.Drawing.Size(938, 501);
            this.dg_external.TabIndex = 5;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 28);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(1119, 539);
            this.splitContainer2.SplitterDistance = 163;
            this.splitContainer2.TabIndex = 4;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(163, 539);
            this.propertyGrid1.TabIndex = 0;
            // 
            // ImportODMData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 594);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.nav_bottom);
            this.Controls.Add(this.nav_top);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportODMData";
            this.Text = "ODM Database Manager";
            ((System.ComponentModel.ISupportInitialize)(this.nav_top)).EndInit();
            this.nav_top.ResumeLayout(false);
            this.nav_top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceODM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nav_bottom)).EndInit();
            this.nav_bottom.ResumeLayout(false);
            this.nav_bottom.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_odm)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_external)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingNavigator nav_top;
        private System.Windows.Forms.ToolStripButton btnOpenExcel;
        private System.Windows.Forms.ToolStripComboBox cmbSheet;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox cmbTables;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.BindingSource bindingSourceODM;
        private System.Windows.Forms.BindingNavigator nav_bottom;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dg_odm;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dg_external;
        private System.Windows.Forms.ToolStripLabel labelStatus;
        private System.Windows.Forms.ToolStripLabel lb_File;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btn_script;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox tb_script;
        private System.Windows.Forms.ToolStripButton btn_ShowScript;
        private System.Windows.Forms.ToolStripDropDownButton btnExport;
        private System.Windows.Forms.ToolStripMenuItem defaultExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customExportToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripButton btnUpdateSeriesCata;

    }
}