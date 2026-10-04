namespace Heiflow.Models.Hydrodynamics.Susbed
{
    partial class JunctionParaForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JunctionParaForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.olvSections = new BrightIdeasSoftware.DataListView();
            this.olvColumn12 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownSection = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.olvTributaryPara = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.olvPointSource = new BrightIdeasSoftware.DataListView();
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn10 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn11 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn13 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn14 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvSections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSection)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvTributaryPara)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 503);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(655, 48);
            this.panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(567, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 30);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "关  闭";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(104, 6);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 30);
            this.btnRemove.TabIndex = 0;
            this.btnRemove.Text = "删  除";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(14, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 30);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "添  加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(476, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保  存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(655, 503);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.olvSections);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.numericUpDownSection);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(647, 477);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "河段  ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // olvSections
            // 
            this.olvSections.AllColumns.Add(this.olvColumn12);
            this.olvSections.AllowColumnReorder = true;
            this.olvSections.AllowDrop = true;
            this.olvSections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvSections.AutoGenerateColumns = false;
            this.olvSections.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvSections.CellEditUseWholeCell = false;
            this.olvSections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn12});
            this.olvSections.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvSections.DataSource = null;
            this.olvSections.EmptyListMsg = "";
            this.olvSections.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvSections.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvSections.FullRowSelect = true;
            this.olvSections.GridLines = true;
            this.olvSections.GroupWithItemCountFormat = "";
            this.olvSections.GroupWithItemCountSingularFormat = "";
            this.olvSections.HideSelection = false;
            this.olvSections.Location = new System.Drawing.Point(8, 45);
            this.olvSections.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvSections.Name = "olvSections";
            this.olvSections.RowHeight = 20;
            this.olvSections.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvSections.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvSections.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvSections.ShowCommandMenuOnRightClick = true;
            this.olvSections.ShowGroups = false;
            this.olvSections.ShowImagesOnSubItems = true;
            this.olvSections.ShowItemToolTips = true;
            this.olvSections.Size = new System.Drawing.Size(629, 426);
            this.olvSections.TabIndex = 32;
            this.olvSections.UseCellFormatEvents = true;
            this.olvSections.UseCompatibleStateImageBehavior = false;
            this.olvSections.UseFilterIndicator = true;
            this.olvSections.UseFiltering = true;
            this.olvSections.UseHotItem = true;
            this.olvSections.UseTranslucentHotItem = true;
            this.olvSections.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn12
            // 
            this.olvColumn12.AspectName = "NO";
            this.olvColumn12.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn12.CellEditUseWholeCell = false;
            this.olvColumn12.IsTileViewColumn = true;
            this.olvColumn12.Text = "断面编号";
            this.olvColumn12.UseInitialLetterForGroup = true;
            this.olvColumn12.Width = 556;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "请在下表中输入河段分界面序号:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(441, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "河段数目:";
            // 
            // numericUpDownSection
            // 
            this.numericUpDownSection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSection.Location = new System.Drawing.Point(506, 16);
            this.numericUpDownSection.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSection.Name = "numericUpDownSection";
            this.numericUpDownSection.Size = new System.Drawing.Size(132, 21);
            this.numericUpDownSection.TabIndex = 0;
            this.numericUpDownSection.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSection.ValueChanged += new System.EventHandler(this.numericUpDownSection_ValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.olvTributaryPara);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(647, 477);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "支流  ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // olvTributaryPara
            // 
            this.olvTributaryPara.AllColumns.Add(this.olvColumn1);
            this.olvTributaryPara.AllColumns.Add(this.olvColumn2);
            this.olvTributaryPara.AllColumns.Add(this.olvColumn3);
            this.olvTributaryPara.AllColumns.Add(this.olvColumn4);
            this.olvTributaryPara.AllColumns.Add(this.olvColumn5);
            this.olvTributaryPara.AllColumns.Add(this.olvColumn7);
            this.olvTributaryPara.AllColumns.Add(this.olvColumn6);
            this.olvTributaryPara.AllowColumnReorder = true;
            this.olvTributaryPara.AllowDrop = true;
            this.olvTributaryPara.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvTributaryPara.AutoGenerateColumns = false;
            this.olvTributaryPara.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvTributaryPara.CellEditUseWholeCell = false;
            this.olvTributaryPara.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5,
            this.olvColumn7,
            this.olvColumn6});
            this.olvTributaryPara.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvTributaryPara.DataSource = null;
            this.olvTributaryPara.EmptyListMsg = "";
            this.olvTributaryPara.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvTributaryPara.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvTributaryPara.FullRowSelect = true;
            this.olvTributaryPara.GridLines = true;
            this.olvTributaryPara.GroupWithItemCountFormat = "";
            this.olvTributaryPara.GroupWithItemCountSingularFormat = "";
            this.olvTributaryPara.HideSelection = false;
            this.olvTributaryPara.Location = new System.Drawing.Point(8, 45);
            this.olvTributaryPara.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvTributaryPara.Name = "olvTributaryPara";
            this.olvTributaryPara.RowHeight = 20;
            this.olvTributaryPara.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvTributaryPara.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvTributaryPara.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvTributaryPara.ShowCommandMenuOnRightClick = true;
            this.olvTributaryPara.ShowGroups = false;
            this.olvTributaryPara.ShowImagesOnSubItems = true;
            this.olvTributaryPara.ShowItemToolTips = true;
            this.olvTributaryPara.Size = new System.Drawing.Size(686, 433);
            this.olvTributaryPara.TabIndex = 31;
            this.olvTributaryPara.UseCellFormatEvents = true;
            this.olvTributaryPara.UseCompatibleStateImageBehavior = false;
            this.olvTributaryPara.UseFilterIndicator = true;
            this.olvTributaryPara.UseFiltering = true;
            this.olvTributaryPara.UseHotItem = true;
            this.olvTributaryPara.UseTranslucentHotItem = true;
            this.olvTributaryPara.View = System.Windows.Forms.View.Details;
            this.olvTributaryPara.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.olvTributaryPara_ItemSelectionChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "NO";
            this.olvColumn1.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn1.CellEditUseWholeCell = false;
            this.olvColumn1.IsTileViewColumn = true;
            this.olvColumn1.Text = "支流编号";
            this.olvColumn1.UseInitialLetterForGroup = true;
            this.olvColumn1.Width = 81;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "MFH";
            this.olvColumn2.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn2.CellEditUseWholeCell = false;
            this.olvColumn2.IsTileViewColumn = true;
            this.olvColumn2.Text = "分汇类型";
            this.olvColumn2.Width = 91;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "MQ";
            this.olvColumn3.Text = "Q形式";
            this.olvColumn3.Width = 70;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "MS";
            this.olvColumn4.Text = "S形式";
            this.olvColumn4.Width = 75;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "UpperSectionID";
            this.olvColumn5.Text = "进口断面编号";
            this.olvColumn5.Width = 92;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "LowerSectionID";
            this.olvColumn7.Text = "出口断面编号";
            this.olvColumn7.Width = 98;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Name";
            this.olvColumn6.Text = "入汇点名称";
            this.olvColumn6.Width = 85;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 12);
            this.label3.TabIndex = 29;
            this.label3.Text = "请在下表中输入汇入支流信息";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.olvPointSource);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(647, 477);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "节点  ";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // olvPointSource
            // 
            this.olvPointSource.AllColumns.Add(this.olvColumn8);
            this.olvPointSource.AllColumns.Add(this.olvColumn9);
            this.olvPointSource.AllColumns.Add(this.olvColumn10);
            this.olvPointSource.AllColumns.Add(this.olvColumn11);
            this.olvPointSource.AllColumns.Add(this.olvColumn13);
            this.olvPointSource.AllColumns.Add(this.olvColumn14);
            this.olvPointSource.AllowColumnReorder = true;
            this.olvPointSource.AllowDrop = true;
            this.olvPointSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvPointSource.AutoGenerateColumns = false;
            this.olvPointSource.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvPointSource.CellEditUseWholeCell = false;
            this.olvPointSource.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn8,
            this.olvColumn9,
            this.olvColumn10,
            this.olvColumn11,
            this.olvColumn13,
            this.olvColumn14});
            this.olvPointSource.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPointSource.DataSource = null;
            this.olvPointSource.EmptyListMsg = "";
            this.olvPointSource.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvPointSource.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvPointSource.FullRowSelect = true;
            this.olvPointSource.GridLines = true;
            this.olvPointSource.GroupWithItemCountFormat = "";
            this.olvPointSource.GroupWithItemCountSingularFormat = "";
            this.olvPointSource.HideSelection = false;
            this.olvPointSource.Location = new System.Drawing.Point(8, 45);
            this.olvPointSource.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvPointSource.Name = "olvPointSource";
            this.olvPointSource.RowHeight = 20;
            this.olvPointSource.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvPointSource.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvPointSource.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvPointSource.ShowCommandMenuOnRightClick = true;
            this.olvPointSource.ShowGroups = false;
            this.olvPointSource.ShowImagesOnSubItems = true;
            this.olvPointSource.ShowItemToolTips = true;
            this.olvPointSource.Size = new System.Drawing.Size(686, 433);
            this.olvPointSource.TabIndex = 33;
            this.olvPointSource.UseCellFormatEvents = true;
            this.olvPointSource.UseCompatibleStateImageBehavior = false;
            this.olvPointSource.UseFilterIndicator = true;
            this.olvPointSource.UseFiltering = true;
            this.olvPointSource.UseHotItem = true;
            this.olvPointSource.UseTranslucentHotItem = true;
            this.olvPointSource.View = System.Windows.Forms.View.Details;
            this.olvPointSource.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.olvPointSource_ItemSelectionChanged);
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "NO";
            this.olvColumn8.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn8.CellEditUseWholeCell = false;
            this.olvColumn8.IsTileViewColumn = true;
            this.olvColumn8.Text = "支流编号";
            this.olvColumn8.UseInitialLetterForGroup = true;
            this.olvColumn8.Width = 81;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "MFH";
            this.olvColumn9.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn9.CellEditUseWholeCell = false;
            this.olvColumn9.IsTileViewColumn = true;
            this.olvColumn9.Text = "分汇类型";
            this.olvColumn9.Width = 91;
            // 
            // olvColumn10
            // 
            this.olvColumn10.AspectName = "MQ";
            this.olvColumn10.Text = "Q形式";
            this.olvColumn10.Width = 70;
            // 
            // olvColumn11
            // 
            this.olvColumn11.AspectName = "MS";
            this.olvColumn11.Text = "S形式";
            this.olvColumn11.Width = 75;
            // 
            // olvColumn13
            // 
            this.olvColumn13.AspectName = "LowerSectionID";
            this.olvColumn13.Text = "下游断面编号";
            this.olvColumn13.Width = 98;
            // 
            // olvColumn14
            // 
            this.olvColumn14.AspectName = "Name";
            this.olvColumn14.Text = "入汇点名称";
            this.olvColumn14.Width = 85;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 12);
            this.label4.TabIndex = 32;
            this.label4.Text = "请在下表中输入节点汇流信息";
            // 
            // JunctionParaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(655, 551);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "JunctionParaForm";
            this.Text = " 汇流过程设置";
            this.Load += new System.EventHandler(this.JunctionParaForm_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvSections)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSection)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvTributaryPara)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPointSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownSection;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private BrightIdeasSoftware.DataListView olvTributaryPara;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private BrightIdeasSoftware.DataListView olvPointSource;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private BrightIdeasSoftware.OLVColumn olvColumn10;
        private BrightIdeasSoftware.OLVColumn olvColumn11;
        private BrightIdeasSoftware.OLVColumn olvColumn13;
        private BrightIdeasSoftware.OLVColumn olvColumn14;
        private System.Windows.Forms.Label label4;
        private BrightIdeasSoftware.DataListView olvSections;
        private BrightIdeasSoftware.OLVColumn olvColumn12;

    }
}