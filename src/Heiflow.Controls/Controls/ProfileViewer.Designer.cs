namespace Heiflow.Controls.WinForm.Controls
{
    partial class Profile_Viewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Profile_Viewer));
            this.cmbPackages = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridViewZone = new System.Windows.Forms.DataGridView();
            this.bindingNavigatorZone = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingSourceZone = new System.Windows.Forms.BindingSource(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.btnImportZone = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExportZone = new System.Windows.Forms.ToolStripButton();
            this.btnZoneTemplateFile = new System.Windows.Forms.ToolStripButton();
            this.btnLocateZoneDic = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlLeft = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox_Props = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonCol = new System.Windows.Forms.RadioButton();
            this.radioButtonRow = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxCols = new System.Windows.Forms.ComboBox();
            this.comboBoxRows = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.winChart1 = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.contextMenuStripAreal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSourceLookup = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.labelStatus = new System.Windows.Forms.ToolStripLabel();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorZone)).BeginInit();
            this.bindingNavigatorZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlLeft.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.contextMenuStripAreal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceLookup)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbPackages
            // 
            this.cmbPackages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPackages.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cmbPackages.FormattingEnabled = true;
            this.cmbPackages.Location = new System.Drawing.Point(6, 55);
            this.cmbPackages.Name = "cmbPackages";
            this.cmbPackages.Size = new System.Drawing.Size(259, 29);
            this.cmbPackages.TabIndex = 1;
            this.cmbPackages.SelectedIndexChanged += new System.EventHandler(this.cmbPackages_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridViewZone);
            this.tabPage3.Controls.Add(this.bindingNavigatorZone);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(924, 614);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Table View";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridViewZone
            // 
            this.dataGridViewZone.AllowUserToAddRows = false;
            this.dataGridViewZone.AllowUserToDeleteRows = false;
            this.dataGridViewZone.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewZone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewZone.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightGreen;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewZone.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewZone.Location = new System.Drawing.Point(3, 30);
            this.dataGridViewZone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridViewZone.Name = "dataGridViewZone";
            this.dataGridViewZone.RowTemplate.Height = 23;
            this.dataGridViewZone.Size = new System.Drawing.Size(918, 581);
            this.dataGridViewZone.TabIndex = 3;
            // 
            // bindingNavigatorZone
            // 
            this.bindingNavigatorZone.AddNewItem = null;
            this.bindingNavigatorZone.BindingSource = this.bindingSourceZone;
            this.bindingNavigatorZone.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigatorZone.DeleteItem = null;
            this.bindingNavigatorZone.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.bindingNavigatorZone.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportZone,
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.btnExportZone,
            this.btnZoneTemplateFile,
            this.btnLocateZoneDic});
            this.bindingNavigatorZone.Location = new System.Drawing.Point(3, 3);
            this.bindingNavigatorZone.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigatorZone.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigatorZone.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigatorZone.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigatorZone.Name = "bindingNavigatorZone";
            this.bindingNavigatorZone.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigatorZone.Size = new System.Drawing.Size(918, 27);
            this.bindingNavigatorZone.TabIndex = 0;
            this.bindingNavigatorZone.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(38, 24);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "总项数";
            // 
            // btnImportZone
            // 
            this.btnImportZone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImportZone.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GeodatabaseXMLRecordSetImport32;
            this.btnImportZone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportZone.Name = "btnImportZone";
            this.btnImportZone.Size = new System.Drawing.Size(24, 24);
            this.btnImportZone.Text = "Import Zone ID table from a csv file. The column names must be HRU_ID, Layer, Zon" +
    "e_ID";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveFirstItem.Text = "移到第一条记录";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMovePreviousItem.Text = "移到上一条记录";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(56, 27);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "当前位置";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveNextItem.Text = "移到下一条记录";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveLastItem.Text = "移到最后一条记录";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnExportZone
            // 
            this.btnExportZone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExportZone.Image = global::Heiflow.Controls.WinForm.Properties.Resources.csv_3_24;
            this.btnExportZone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportZone.Name = "btnExportZone";
            this.btnExportZone.Size = new System.Drawing.Size(24, 24);
            this.btnExportZone.Text = "Export template file";
            // 
            // btnZoneTemplateFile
            // 
            this.btnZoneTemplateFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoneTemplateFile.Image = global::Heiflow.Controls.WinForm.Properties.Resources.excel_32;
            this.btnZoneTemplateFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoneTemplateFile.Name = "btnZoneTemplateFile";
            this.btnZoneTemplateFile.Size = new System.Drawing.Size(24, 24);
            this.btnZoneTemplateFile.Text = "Open template file";
            // 
            // btnLocateZoneDic
            // 
            this.btnLocateZoneDic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLocateZoneDic.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericOpen_B_32;
            this.btnLocateZoneDic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLocateZoneDic.Name = "btnLocateZoneDic";
            this.btnLocateZoneDic.Size = new System.Drawing.Size(24, 24);
            this.btnLocateZoneDic.Text = "Locate template file directory";
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
            this.splitContainer1.Panel1.Font = new System.Drawing.Font("Calibri", 10.5F);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1227, 643);
            this.splitContainer1.SplitterDistance = 291;
            this.splitContainer1.TabIndex = 18;
            // 
            // tabControlLeft
            // 
            this.tabControlLeft.Controls.Add(this.tabPage1);
            this.tabControlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLeft.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControlLeft.Location = new System.Drawing.Point(0, 0);
            this.tabControlLeft.Name = "tabControlLeft";
            this.tabControlLeft.SelectedIndex = 0;
            this.tabControlLeft.Size = new System.Drawing.Size(291, 643);
            this.tabControlLeft.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(283, 609);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Setting";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cmbPackages);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.listBox_Props);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(5, 221);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(271, 381);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Property To View";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Packages";
            // 
            // listBox_Props
            // 
            this.listBox_Props.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_Props.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox_Props.FormattingEnabled = true;
            this.listBox_Props.ItemHeight = 21;
            this.listBox_Props.Location = new System.Drawing.Point(5, 131);
            this.listBox_Props.Name = "listBox_Props";
            this.listBox_Props.Size = new System.Drawing.Size(260, 214);
            this.listBox_Props.TabIndex = 13;
            this.listBox_Props.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_Props_DrawItem);
            this.listBox_Props.SelectedIndexChanged += new System.EventHandler(this.listBox_Props_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.label2.Location = new System.Drawing.Point(2, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select a property to view";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.radioButtonCol);
            this.groupBox1.Controls.Add(this.radioButtonRow);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBoxCols);
            this.groupBox1.Controls.Add(this.comboBoxRows);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 209);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Select the column number:";
            // 
            // radioButtonCol
            // 
            this.radioButtonCol.AutoSize = true;
            this.radioButtonCol.Location = new System.Drawing.Point(125, 27);
            this.radioButtonCol.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonCol.Name = "radioButtonCol";
            this.radioButtonCol.Size = new System.Drawing.Size(117, 24);
            this.radioButtonCol.TabIndex = 15;
            this.radioButtonCol.Text = "Column View";
            this.radioButtonCol.UseVisualStyleBackColor = true;
            this.radioButtonCol.CheckedChanged += new System.EventHandler(this.radioButtonCol_CheckedChanged);
            // 
            // radioButtonRow
            // 
            this.radioButtonRow.AutoSize = true;
            this.radioButtonRow.Checked = true;
            this.radioButtonRow.Location = new System.Drawing.Point(6, 27);
            this.radioButtonRow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonRow.Name = "radioButtonRow";
            this.radioButtonRow.Size = new System.Drawing.Size(95, 24);
            this.radioButtonRow.TabIndex = 14;
            this.radioButtonRow.TabStop = true;
            this.radioButtonRow.Text = "Row View";
            this.radioButtonRow.UseVisualStyleBackColor = true;
            this.radioButtonRow.CheckedChanged += new System.EventHandler(this.radioButtonRow_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "Select the row number:";
            // 
            // comboBoxCols
            // 
            this.comboBoxCols.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCols.Enabled = false;
            this.comboBoxCols.FormattingEnabled = true;
            this.comboBoxCols.Location = new System.Drawing.Point(6, 167);
            this.comboBoxCols.Name = "comboBoxCols";
            this.comboBoxCols.Size = new System.Drawing.Size(260, 28);
            this.comboBoxCols.TabIndex = 1;
            // 
            // comboBoxRows
            // 
            this.comboBoxRows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRows.FormattingEnabled = true;
            this.comboBoxRows.Location = new System.Drawing.Point(6, 95);
            this.comboBoxRows.Name = "comboBoxRows";
            this.comboBoxRows.Size = new System.Drawing.Size(258, 28);
            this.comboBoxRows.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(932, 643);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.winChart1);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(924, 609);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Chart View";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // winChart1
            // 
            this.winChart1.BackColor = System.Drawing.SystemColors.Control;
            this.winChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart1.Location = new System.Drawing.Point(3, 3);
            this.winChart1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.winChart1.Name = "winChart1";
            this.winChart1.ShowStatPanel = false;
            this.winChart1.Size = new System.Drawing.Size(918, 603);
            this.winChart1.TabIndex = 1;
            // 
            // contextMenuStripAreal
            // 
            this.contextMenuStripAreal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripAreal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.contextMenuStripAreal.Name = "contextMenuStripAreal";
            this.contextMenuStripAreal.Size = new System.Drawing.Size(144, 52);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.selectAllToolStripMenuItem.Text = "Select all";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.clearToolStripMenuItem.Text = "Clear";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowMerge = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.labelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 643);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1227, 27);
            this.toolStrip1.TabIndex = 17;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(225, 25);
            this.toolStripProgressBar1.Visible = false;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(550, 24);
            this.labelStatus.Text = "Ready";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Profile_Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 670);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Profile_Viewer";
            this.Text = "Profile Viewer";
            this.Load += new System.EventHandler(this.Profile_Viewer_Load);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorZone)).EndInit();
            this.bindingNavigatorZone.ResumeLayout(false);
            this.bindingNavigatorZone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceZone)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlLeft.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.contextMenuStripAreal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceLookup)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPackages;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridViewZone;
        private System.Windows.Forms.BindingNavigator bindingNavigatorZone;
        private System.Windows.Forms.BindingSource bindingSourceZone;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton btnImportZone;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton btnExportZone;
        private System.Windows.Forms.ToolStripButton btnZoneTemplateFile;
        private System.Windows.Forms.ToolStripButton btnLocateZoneDic;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlLeft;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAreal;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.BindingSource bindingSourceLookup;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripLabel labelStatus;
        private WinChart winChart1;
        private System.Windows.Forms.ListBox listBox_Props;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonCol;
        private System.Windows.Forms.RadioButton radioButtonRow;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxCols;
        private System.Windows.Forms.ComboBox comboBoxRows;
    }
}