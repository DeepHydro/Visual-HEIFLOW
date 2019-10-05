namespace Heiflow.Controls.WinForm.Project
{
    partial class HeiflowTimeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeiflowTimeControl));
            this.label3 = new System.Windows.Forms.Label();
            this.cmbTimeUnit = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.olvLanduse = new BrightIdeasSoftware.DataListView();
            this.colStart = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colEnd = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colTimeLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLanduse = new System.Windows.Forms.TabPage();
            this.panelLU = new System.Windows.Forms.Panel();
            this.btnRefreshLU = new System.Windows.Forms.Button();
            this.numericUpDownLU = new System.Windows.Forms.NumericUpDown();
            this.rbtnLUTime = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.rbtnLUNum = new System.Windows.Forms.RadioButton();
            this.cmbLUTimeUnit = new System.Windows.Forms.ComboBox();
            this.chbLanduse = new System.Windows.Forms.CheckBox();
            this.tabPageMF = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefreshMF = new System.Windows.Forms.Button();
            this.rbtnMFNum = new System.Windows.Forms.RadioButton();
            this.rbtnMFTime = new System.Windows.Forms.RadioButton();
            this.numericUpDownMF = new System.Windows.Forms.NumericUpDown();
            this.cmbMFSPUnit = new System.Windows.Forms.ComboBox();
            this.olvMF = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColMFNumTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRefreshGlobalTime = new System.Windows.Forms.Button();
            this.tbTimeNums = new System.Windows.Forms.TextBox();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.olvLanduse)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageLanduse.SuspendLayout();
            this.panelLU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLU)).BeginInit();
            this.tabPageMF.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvMF)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(250, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "Time Unit of Model";
            // 
            // cmbTimeUnit
            // 
            this.cmbTimeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimeUnit.FormattingEnabled = true;
            this.cmbTimeUnit.Items.AddRange(new object[] {
            "Hours",
            "Days"});
            this.cmbTimeUnit.Location = new System.Drawing.Point(390, 26);
            this.cmbTimeUnit.Name = "cmbTimeUnit";
            this.cmbTimeUnit.Size = new System.Drawing.Size(137, 28);
            this.cmbTimeUnit.TabIndex = 2;
            this.cmbTimeUnit.SelectedIndexChanged += new System.EventHandler(this.cmbTimeUnit_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(295, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total time";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(644, 551);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOk.Location = new System.Drawing.Point(528, 551);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 30);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // olvLanduse
            // 
            this.olvLanduse.AllColumns.Add(this.colStart);
            this.olvLanduse.AllColumns.Add(this.colEnd);
            this.olvLanduse.AllColumns.Add(this.colTimeLength);
            this.olvLanduse.AllColumns.Add(this.olvColumn8);
            this.olvLanduse.AllowColumnReorder = true;
            this.olvLanduse.AllowDrop = true;
            this.olvLanduse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvLanduse.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvLanduse.CellEditUseWholeCell = false;
            this.olvLanduse.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colStart,
            this.colEnd,
            this.colTimeLength,
            this.olvColumn8});
            this.olvLanduse.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvLanduse.DataSource = null;
            this.olvLanduse.EmptyListMsg = "";
            this.olvLanduse.EmptyListMsgFont = new System.Drawing.Font("Segoe UI", 9F);
            this.olvLanduse.FullRowSelect = true;
            this.olvLanduse.GridLines = true;
            this.olvLanduse.GroupWithItemCountFormat = "";
            this.olvLanduse.GroupWithItemCountSingularFormat = "";
            this.olvLanduse.HideSelection = false;
            this.olvLanduse.Location = new System.Drawing.Point(6, 88);
            this.olvLanduse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvLanduse.Name = "olvLanduse";
            this.olvLanduse.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvLanduse.SelectedBackColor = System.Drawing.Color.LightSkyBlue;
            this.olvLanduse.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvLanduse.ShowCommandMenuOnRightClick = true;
            this.olvLanduse.ShowGroups = false;
            this.olvLanduse.ShowImagesOnSubItems = true;
            this.olvLanduse.ShowItemToolTips = true;
            this.olvLanduse.Size = new System.Drawing.Size(723, 308);
            this.olvLanduse.TabIndex = 22;
            this.olvLanduse.UseCellFormatEvents = true;
            this.olvLanduse.UseCompatibleStateImageBehavior = false;
            this.olvLanduse.UseFilterIndicator = true;
            this.olvLanduse.UseFiltering = true;
            this.olvLanduse.UseHotItem = true;
            this.olvLanduse.UseTranslucentHotItem = true;
            this.olvLanduse.View = System.Windows.Forms.View.Details;
            // 
            // colStart
            // 
            this.colStart.AspectName = "Start";
            this.colStart.ButtonPadding = new System.Drawing.Size(10, 10);
            this.colStart.CellEditUseWholeCell = true;
            this.colStart.IsTileViewColumn = true;
            this.colStart.Text = "Start";
            this.colStart.UseInitialLetterForGroup = true;
            this.colStart.Width = 141;
            // 
            // colEnd
            // 
            this.colEnd.AspectName = "End";
            this.colEnd.ButtonPadding = new System.Drawing.Size(10, 10);
            this.colEnd.CellEditUseWholeCell = true;
            this.colEnd.IsTileViewColumn = true;
            this.colEnd.Text = "End";
            this.colEnd.Width = 155;
            // 
            // colTimeLength
            // 
            this.colTimeLength.AspectName = "NumTimeSteps";
            this.colTimeLength.CellEditUseWholeCell = true;
            this.colTimeLength.Text = "Num Time Steps";
            this.colTimeLength.Width = 161;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "ParameterFile";
            this.olvColumn8.CellEditUseWholeCell = true;
            this.olvColumn8.Text = "Parameter File";
            this.olvColumn8.Width = 340;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageLanduse);
            this.tabControl1.Controls.Add(this.tabPageMF);
            this.tabControl1.Location = new System.Drawing.Point(4, 117);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(742, 427);
            this.tabControl1.TabIndex = 23;
            // 
            // tabPageLanduse
            // 
            this.tabPageLanduse.Controls.Add(this.panelLU);
            this.tabPageLanduse.Controls.Add(this.chbLanduse);
            this.tabPageLanduse.Controls.Add(this.olvLanduse);
            this.tabPageLanduse.Location = new System.Drawing.Point(4, 29);
            this.tabPageLanduse.Name = "tabPageLanduse";
            this.tabPageLanduse.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLanduse.Size = new System.Drawing.Size(734, 394);
            this.tabPageLanduse.TabIndex = 0;
            this.tabPageLanduse.Text = "Land Use Periods      ";
            this.tabPageLanduse.UseVisualStyleBackColor = true;
            // 
            // panelLU
            // 
            this.panelLU.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelLU.Controls.Add(this.btnRefreshLU);
            this.panelLU.Controls.Add(this.numericUpDownLU);
            this.panelLU.Controls.Add(this.rbtnLUTime);
            this.panelLU.Controls.Add(this.label7);
            this.panelLU.Controls.Add(this.rbtnLUNum);
            this.panelLU.Controls.Add(this.cmbLUTimeUnit);
            this.panelLU.Enabled = false;
            this.panelLU.Location = new System.Drawing.Point(8, 36);
            this.panelLU.Name = "panelLU";
            this.panelLU.Size = new System.Drawing.Size(721, 46);
            this.panelLU.TabIndex = 37;
            // 
            // btnRefreshLU
            // 
            this.btnRefreshLU.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshLU.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnRefreshLU.Location = new System.Drawing.Point(611, 9);
            this.btnRefreshLU.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefreshLU.Name = "btnRefreshLU";
            this.btnRefreshLU.Size = new System.Drawing.Size(92, 29);
            this.btnRefreshLU.TabIndex = 31;
            this.btnRefreshLU.Text = "Refresh";
            this.btnRefreshLU.UseVisualStyleBackColor = true;
            this.btnRefreshLU.Click += new System.EventHandler(this.btnRefreshLU_Click);
            // 
            // numericUpDownLU
            // 
            this.numericUpDownLU.Location = new System.Drawing.Point(270, 10);
            this.numericUpDownLU.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.numericUpDownLU.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownLU.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownLU.Name = "numericUpDownLU";
            this.numericUpDownLU.Size = new System.Drawing.Size(70, 27);
            this.numericUpDownLU.TabIndex = 32;
            this.numericUpDownLU.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownLU.ValueChanged += new System.EventHandler(this.numericUpDownLU_ValueChanged);
            // 
            // rbtnLUTime
            // 
            this.rbtnLUTime.AutoSize = true;
            this.rbtnLUTime.Location = new System.Drawing.Point(363, 10);
            this.rbtnLUTime.Name = "rbtnLUTime";
            this.rbtnLUTime.Size = new System.Drawing.Size(114, 24);
            this.rbtnLUTime.TabIndex = 36;
            this.rbtnLUTime.Text = "By Time Unit";
            this.rbtnLUTime.UseVisualStyleBackColor = true;
            this.rbtnLUTime.CheckedChanged += new System.EventHandler(this.rbtnLUTime_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(149, 20);
            this.label7.TabIndex = 33;
            this.label7.Text = "Create stress periods:";
            // 
            // rbtnLUNum
            // 
            this.rbtnLUNum.AutoSize = true;
            this.rbtnLUNum.Checked = true;
            this.rbtnLUNum.Location = new System.Drawing.Point(168, 10);
            this.rbtnLUNum.Name = "rbtnLUNum";
            this.rbtnLUNum.Size = new System.Drawing.Size(110, 24);
            this.rbtnLUNum.TabIndex = 35;
            this.rbtnLUNum.TabStop = true;
            this.rbtnLUNum.Text = "By Numbers";
            this.rbtnLUNum.UseVisualStyleBackColor = true;
            this.rbtnLUNum.CheckedChanged += new System.EventHandler(this.rbtnLUNum_CheckedChanged);
            // 
            // cmbLUTimeUnit
            // 
            this.cmbLUTimeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLUTimeUnit.Enabled = false;
            this.cmbLUTimeUnit.FormattingEnabled = true;
            this.cmbLUTimeUnit.Items.AddRange(new object[] {
            "Days",
            "Weeks",
            "Months",
            "Years"});
            this.cmbLUTimeUnit.Location = new System.Drawing.Point(478, 10);
            this.cmbLUTimeUnit.Name = "cmbLUTimeUnit";
            this.cmbLUTimeUnit.Size = new System.Drawing.Size(86, 28);
            this.cmbLUTimeUnit.TabIndex = 34;
            this.cmbLUTimeUnit.SelectedIndexChanged += new System.EventHandler(this.cmbLUTimeUnit_SelectedIndexChanged);
            // 
            // chbLanduse
            // 
            this.chbLanduse.AutoSize = true;
            this.chbLanduse.Location = new System.Drawing.Point(8, 11);
            this.chbLanduse.Name = "chbLanduse";
            this.chbLanduse.Size = new System.Drawing.Size(208, 24);
            this.chbLanduse.TabIndex = 23;
            this.chbLanduse.Text = "Use Time-Variant Land Use";
            this.chbLanduse.UseVisualStyleBackColor = true;
            this.chbLanduse.CheckedChanged += new System.EventHandler(this.chbLanduse_CheckedChanged);
            // 
            // tabPageMF
            // 
            this.tabPageMF.Controls.Add(this.panel1);
            this.tabPageMF.Controls.Add(this.olvMF);
            this.tabPageMF.Location = new System.Drawing.Point(4, 29);
            this.tabPageMF.Name = "tabPageMF";
            this.tabPageMF.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMF.Size = new System.Drawing.Size(734, 394);
            this.tabPageMF.TabIndex = 1;
            this.tabPageMF.Text = "Modflow Stress Periods   ";
            this.tabPageMF.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnRefreshMF);
            this.panel1.Controls.Add(this.rbtnMFNum);
            this.panel1.Controls.Add(this.rbtnMFTime);
            this.panel1.Controls.Add(this.numericUpDownMF);
            this.panel1.Controls.Add(this.cmbMFSPUnit);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(721, 46);
            this.panel1.TabIndex = 38;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 20);
            this.label1.TabIndex = 33;
            this.label1.Text = "Create stress periods:";
            // 
            // btnRefreshMF
            // 
            this.btnRefreshMF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshMF.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnRefreshMF.Location = new System.Drawing.Point(617, 9);
            this.btnRefreshMF.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefreshMF.Name = "btnRefreshMF";
            this.btnRefreshMF.Size = new System.Drawing.Size(92, 29);
            this.btnRefreshMF.TabIndex = 17;
            this.btnRefreshMF.Text = "Refresh";
            this.btnRefreshMF.UseVisualStyleBackColor = true;
            this.btnRefreshMF.Click += new System.EventHandler(this.btnRefreshMF_Click);
            // 
            // rbtnMFNum
            // 
            this.rbtnMFNum.AutoSize = true;
            this.rbtnMFNum.Checked = true;
            this.rbtnMFNum.Location = new System.Drawing.Point(160, 9);
            this.rbtnMFNum.Name = "rbtnMFNum";
            this.rbtnMFNum.Size = new System.Drawing.Size(110, 24);
            this.rbtnMFNum.TabIndex = 29;
            this.rbtnMFNum.TabStop = true;
            this.rbtnMFNum.Text = "By Numbers";
            this.rbtnMFNum.UseVisualStyleBackColor = true;
            this.rbtnMFNum.CheckedChanged += new System.EventHandler(this.rbtnMFNum_CheckedChanged);
            // 
            // rbtnMFTime
            // 
            this.rbtnMFTime.AutoSize = true;
            this.rbtnMFTime.Location = new System.Drawing.Point(379, 9);
            this.rbtnMFTime.Name = "rbtnMFTime";
            this.rbtnMFTime.Size = new System.Drawing.Size(114, 24);
            this.rbtnMFTime.TabIndex = 30;
            this.rbtnMFTime.Text = "By Time Unit";
            this.rbtnMFTime.UseVisualStyleBackColor = true;
            this.rbtnMFTime.CheckedChanged += new System.EventHandler(this.rbtnMFTime_CheckedChanged);
            // 
            // numericUpDownMF
            // 
            this.numericUpDownMF.Location = new System.Drawing.Point(287, 9);
            this.numericUpDownMF.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.numericUpDownMF.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownMF.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownMF.Name = "numericUpDownMF";
            this.numericUpDownMF.Size = new System.Drawing.Size(70, 27);
            this.numericUpDownMF.TabIndex = 24;
            this.numericUpDownMF.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownMF.ValueChanged += new System.EventHandler(this.numericUpDownMF_ValueChanged);
            // 
            // cmbMFSPUnit
            // 
            this.cmbMFSPUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMFSPUnit.Enabled = false;
            this.cmbMFSPUnit.FormattingEnabled = true;
            this.cmbMFSPUnit.Items.AddRange(new object[] {
            "Days",
            "Weeks",
            "Months",
            "Years"});
            this.cmbMFSPUnit.Location = new System.Drawing.Point(510, 9);
            this.cmbMFSPUnit.Name = "cmbMFSPUnit";
            this.cmbMFSPUnit.Size = new System.Drawing.Size(86, 28);
            this.cmbMFSPUnit.TabIndex = 27;
            this.cmbMFSPUnit.SelectedIndexChanged += new System.EventHandler(this.cmbMFSPUnit_SelectedIndexChanged);
            // 
            // olvMF
            // 
            this.olvMF.AllColumns.Add(this.olvColumn1);
            this.olvMF.AllColumns.Add(this.olvColumn2);
            this.olvMF.AllColumns.Add(this.olvColMFNumTime);
            this.olvMF.AllColumns.Add(this.olvColumn3);
            this.olvMF.AllColumns.Add(this.olvColumn5);
            this.olvMF.AllColumns.Add(this.olvColumn6);
            this.olvMF.AllowColumnReorder = true;
            this.olvMF.AllowDrop = true;
            this.olvMF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvMF.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMF.CellEditUseWholeCell = false;
            this.olvMF.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColMFNumTime,
            this.olvColumn3,
            this.olvColumn5,
            this.olvColumn6});
            this.olvMF.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMF.DataSource = null;
            this.olvMF.EmptyListMsg = "";
            this.olvMF.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMF.FullRowSelect = true;
            this.olvMF.GridLines = true;
            this.olvMF.GroupWithItemCountFormat = "";
            this.olvMF.GroupWithItemCountSingularFormat = "";
            this.olvMF.HideSelection = false;
            this.olvMF.Location = new System.Drawing.Point(6, 57);
            this.olvMF.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvMF.Name = "olvMF";
            this.olvMF.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMF.SelectedBackColor = System.Drawing.Color.LightSkyBlue;
            this.olvMF.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvMF.ShowCommandMenuOnRightClick = true;
            this.olvMF.ShowGroups = false;
            this.olvMF.ShowImagesOnSubItems = true;
            this.olvMF.ShowItemToolTips = true;
            this.olvMF.Size = new System.Drawing.Size(723, 338);
            this.olvMF.TabIndex = 25;
            this.olvMF.UseCellFormatEvents = true;
            this.olvMF.UseCompatibleStateImageBehavior = false;
            this.olvMF.UseFilterIndicator = true;
            this.olvMF.UseFiltering = true;
            this.olvMF.UseHotItem = true;
            this.olvMF.UseTranslucentHotItem = true;
            this.olvMF.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Start";
            this.olvColumn1.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn1.CellEditUseWholeCell = true;
            this.olvColumn1.IsTileViewColumn = true;
            this.olvColumn1.Text = "Start";
            this.olvColumn1.UseInitialLetterForGroup = true;
            this.olvColumn1.Width = 121;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "End";
            this.olvColumn2.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn2.CellEditUseWholeCell = true;
            this.olvColumn2.IsTileViewColumn = true;
            this.olvColumn2.Text = "End";
            this.olvColumn2.Width = 135;
            // 
            // olvColMFNumTime
            // 
            this.olvColMFNumTime.AspectName = "NumTimeSteps";
            this.olvColMFNumTime.CellEditUseWholeCell = true;
            this.olvColMFNumTime.Text = "Num Time Steps";
            this.olvColMFNumTime.Width = 124;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Length";
            this.olvColumn3.CellEditUseWholeCell = true;
            this.olvColumn3.Text = "Time Length";
            this.olvColumn3.Width = 137;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Multiplier";
            this.olvColumn5.CellEditUseWholeCell = true;
            this.olvColumn5.Text = "Multiplier";
            this.olvColumn5.Width = 107;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "IsSteadyState";
            this.olvColumn6.CellEditUseWholeCell = true;
            this.olvColumn6.Text = "Steady State";
            this.olvColumn6.Width = 107;
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Location = new System.Drawing.Point(54, 26);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(169, 27);
            this.dateTimePickerStart.TabIndex = 15;
            this.dateTimePickerStart.ValueChanged += new System.EventHandler(this.dateTimePickerStart_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Start";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnRefreshGlobalTime);
            this.groupBox1.Controls.Add(this.tbTimeNums);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dateTimePickerEnd);
            this.groupBox1.Controls.Add(this.cmbTimeUnit);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(740, 106);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Time";
            // 
            // btnRefreshGlobalTime
            // 
            this.btnRefreshGlobalTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshGlobalTime.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnRefreshGlobalTime.Location = new System.Drawing.Point(622, 25);
            this.btnRefreshGlobalTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefreshGlobalTime.Name = "btnRefreshGlobalTime";
            this.btnRefreshGlobalTime.Size = new System.Drawing.Size(92, 29);
            this.btnRefreshGlobalTime.TabIndex = 17;
            this.btnRefreshGlobalTime.Text = "Generate";
            this.btnRefreshGlobalTime.UseVisualStyleBackColor = true;
            this.btnRefreshGlobalTime.Click += new System.EventHandler(this.btnCreateGlobalTime_Click);
            // 
            // tbTimeNums
            // 
            this.tbTimeNums.Location = new System.Drawing.Point(390, 66);
            this.tbTimeNums.Name = "tbTimeNums";
            this.tbTimeNums.ReadOnly = true;
            this.tbTimeNums.Size = new System.Drawing.Size(137, 27);
            this.tbTimeNums.TabIndex = 16;
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Location = new System.Drawing.Point(54, 66);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(169, 27);
            this.dateTimePickerEnd.TabIndex = 15;
            this.dateTimePickerEnd.ValueChanged += new System.EventHandler(this.dateTimePickerStart_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "End";
            // 
            // HeiflowTimeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(753, 589);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "HeiflowTimeControl";
            this.ShowInTaskbar = false;
            this.Text = "Model Time";
            this.Load += new System.EventHandler(this.HeiflowTimeControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.olvLanduse)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageLanduse.ResumeLayout(false);
            this.tabPageLanduse.PerformLayout();
            this.panelLU.ResumeLayout(false);
            this.panelLU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLU)).EndInit();
            this.tabPageMF.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvMF)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbTimeUnit;
        private BrightIdeasSoftware.DataListView olvLanduse;
        private BrightIdeasSoftware.OLVColumn colStart;
        private BrightIdeasSoftware.OLVColumn colEnd;
        private BrightIdeasSoftware.OLVColumn colTimeLength;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLanduse;
        private System.Windows.Forms.TabPage tabPageMF;
        private System.Windows.Forms.CheckBox chbLanduse;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label5;
        private BrightIdeasSoftware.DataListView olvMF;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.NumericUpDown numericUpDownMF;
        private System.Windows.Forms.TextBox tbTimeNums;
        private BrightIdeasSoftware.OLVColumn olvColMFNumTime;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private System.Windows.Forms.ComboBox cmbMFSPUnit;
        private System.Windows.Forms.Button btnRefreshGlobalTime;
        private System.Windows.Forms.RadioButton rbtnMFTime;
        private System.Windows.Forms.RadioButton rbtnMFNum;
        private System.Windows.Forms.Button btnRefreshMF;
        private System.Windows.Forms.Button btnRefreshLU;
        private System.Windows.Forms.RadioButton rbtnLUTime;
        private System.Windows.Forms.RadioButton rbtnLUNum;
        private System.Windows.Forms.ComboBox cmbLUTimeUnit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDownLU;
        private System.Windows.Forms.Panel panelLU;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}
