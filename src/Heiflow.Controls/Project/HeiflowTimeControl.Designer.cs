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
            this.olvLanduse = new System.Windows.Forms.DataGridView();
            this.colStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimeLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.olvMF = new System.Windows.Forms.DataGridView();
            this.olvColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColMFNumTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRefreshGlobalTime = new System.Windows.Forms.Button();
            this.tbTimeNums = new System.Windows.Forms.TextBox();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageLanduse.SuspendLayout();
            this.panelLU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLU)).BeginInit();
            this.tabPageMF.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMF)).BeginInit();
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
            this.olvLanduse.AllowUserToOrderColumns = true;
            this.olvLanduse.AllowDrop = true;
            this.olvLanduse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvLanduse.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStart,
            this.colEnd,
            this.colTimeLength,
            this.olvColumn8});
            this.olvLanduse.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvLanduse.Location = new System.Drawing.Point(6, 88);
            this.olvLanduse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvLanduse.Name = "olvLanduse";
            this.olvLanduse.AllowUserToAddRows = false;
            this.olvLanduse.AllowUserToDeleteRows = false;
            this.olvLanduse.AutoGenerateColumns = false;
            this.olvLanduse.RowHeadersVisible = false;
            this.olvLanduse.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvLanduse.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvLanduse.Size = new System.Drawing.Size(723, 308);
            this.olvLanduse.TabIndex = 22;
            // 
            // colStart
            // 
            this.colStart.DataPropertyName = "Start";
            this.colStart.HeaderText = "Start";
            this.colStart.Name = "colStart";
            this.colStart.Width = 141;
            // 
            // colEnd
            // 
            this.colEnd.DataPropertyName = "End";
            this.colEnd.HeaderText = "End";
            this.colEnd.Name = "colEnd";
            this.colEnd.Width = 155;
            // 
            // colTimeLength
            // 
            this.colTimeLength.DataPropertyName = "NumTimeSteps";
            this.colTimeLength.HeaderText = "Num Time Steps";
            this.colTimeLength.Name = "colTimeLength";
            this.colTimeLength.Width = 161;
            // 
            // olvColumn8
            // 
            this.olvColumn8.DataPropertyName = "ParameterFile";
            this.olvColumn8.HeaderText = "Parameter File";
            this.olvColumn8.Name = "olvColumn8";
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
            this.btnRefreshLU.Font = new System.Drawing.Font("Segoe UI", 10F);
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
            this.btnRefreshMF.Font = new System.Drawing.Font("Segoe UI", 10F);
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
            this.olvMF.AllowUserToOrderColumns = true;
            this.olvMF.AllowDrop = true;
            this.olvMF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvMF.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColMFNumTime,
            this.olvColumn3,
            this.olvColumn5,
            this.olvColumn6});
            this.olvMF.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMF.Location = new System.Drawing.Point(6, 57);
            this.olvMF.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvMF.Name = "olvMF";
            this.olvMF.AllowUserToAddRows = false;
            this.olvMF.AllowUserToDeleteRows = false;
            this.olvMF.AutoGenerateColumns = false;
            this.olvMF.RowHeadersVisible = false;
            this.olvMF.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvMF.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvMF.Size = new System.Drawing.Size(723, 338);
            this.olvMF.TabIndex = 25;
            // 
            // olvColumn1
            // 
            this.olvColumn1.DataPropertyName = "Start";
            this.olvColumn1.HeaderText = "Start";
            this.olvColumn1.Name = "olvColumn1";
            this.olvColumn1.Width = 121;
            // 
            // olvColumn2
            // 
            this.olvColumn2.DataPropertyName = "End";
            this.olvColumn2.HeaderText = "End";
            this.olvColumn2.Name = "olvColumn2";
            this.olvColumn2.Width = 135;
            // 
            // olvColMFNumTime
            // 
            this.olvColMFNumTime.DataPropertyName = "NSTP";
            this.olvColMFNumTime.HeaderText = "Num Time Steps";
            this.olvColMFNumTime.Name = "olvColMFNumTime";
            this.olvColMFNumTime.Width = 124;
            // 
            // olvColumn3
            // 
            this.olvColumn3.DataPropertyName = "Length";
            this.olvColumn3.HeaderText = "Time Length";
            this.olvColumn3.Name = "olvColumn3";
            this.olvColumn3.Width = 137;
            // 
            // olvColumn5
            // 
            this.olvColumn5.DataPropertyName = "Multiplier";
            this.olvColumn5.HeaderText = "Multiplier";
            this.olvColumn5.Name = "olvColumn5";
            this.olvColumn5.Width = 107;
            // 
            // olvColumn6
            // 
            this.olvColumn6.DataPropertyName = "IsSteadyState";
            this.olvColumn6.HeaderText = "Steady State";
            this.olvColumn6.Name = "olvColumn6";
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
            this.btnRefreshGlobalTime.Font = new System.Drawing.Font("Segoe UI", 10F);
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
        private System.Windows.Forms.DataGridView olvLanduse;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeLength;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLanduse;
        private System.Windows.Forms.TabPage tabPageMF;
        private System.Windows.Forms.CheckBox chbLanduse;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView olvMF;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn3;
        private System.Windows.Forms.NumericUpDown numericUpDownMF;
        private System.Windows.Forms.TextBox tbTimeNums;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColMFNumTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn8;
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
