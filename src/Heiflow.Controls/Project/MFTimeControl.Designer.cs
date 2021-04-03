namespace Heiflow.Controls.WinForm.Project
{
    partial class MFTimeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFTimeControl));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMF = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnMFNum = new System.Windows.Forms.RadioButton();
            this.numericUpDownMF = new System.Windows.Forms.NumericUpDown();
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
            this.label3 = new System.Windows.Forms.Label();
            this.cmbTimeUnit = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPageMF.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvMF)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageMF);
            this.tabControl1.Location = new System.Drawing.Point(4, 83);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(742, 461);
            this.tabControl1.TabIndex = 23;
            // 
            // tabPageMF
            // 
            this.tabPageMF.Controls.Add(this.panel1);
            this.tabPageMF.Controls.Add(this.olvMF);
            this.tabPageMF.Location = new System.Drawing.Point(4, 29);
            this.tabPageMF.Name = "tabPageMF";
            this.tabPageMF.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMF.Size = new System.Drawing.Size(734, 428);
            this.tabPageMF.TabIndex = 1;
            this.tabPageMF.Text = "Modflow Stress Periods   ";
            this.tabPageMF.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.rbtnMFNum);
            this.panel1.Controls.Add(this.numericUpDownMF);
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
            this.olvMF.Size = new System.Drawing.Size(723, 372);
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
            this.olvColMFNumTime.AspectName = "NSTP";
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
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbTimeUnit);
            this.groupBox1.Controls.Add(this.dateTimePickerStart);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(740, 71);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Time";
            // 
            // btnRefreshGlobalTime
            // 
            this.btnRefreshGlobalTime.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.btnRefreshGlobalTime.Location = new System.Drawing.Point(622, 24);
            this.btnRefreshGlobalTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefreshGlobalTime.Name = "btnRefreshGlobalTime";
            this.btnRefreshGlobalTime.Size = new System.Drawing.Size(92, 29);
            this.btnRefreshGlobalTime.TabIndex = 17;
            this.btnRefreshGlobalTime.Text = "Generate";
            this.btnRefreshGlobalTime.UseVisualStyleBackColor = true;
            this.btnRefreshGlobalTime.Click += new System.EventHandler(this.btnCreateGlobalTime_Click);
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
            "Seconds",
            "Minutes",
            "Hours",
            "Days",
            "Years"});
            this.cmbTimeUnit.Location = new System.Drawing.Point(390, 26);
            this.cmbTimeUnit.Name = "cmbTimeUnit";
            this.cmbTimeUnit.Size = new System.Drawing.Size(137, 28);
            this.cmbTimeUnit.TabIndex = 2;
            this.cmbTimeUnit.SelectedIndexChanged += new System.EventHandler(this.cmbTimeUnit_SelectedIndexChanged);
            // 
            // MFTimeControl
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
            this.Name = "MFTimeControl";
            this.ShowInTaskbar = false;
            this.Text = "Model Time";
            this.Load += new System.EventHandler(this.HeiflowTimeControl_Load);
            this.tabControl1.ResumeLayout(false);
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

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMF;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private BrightIdeasSoftware.DataListView olvMF;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.NumericUpDown numericUpDownMF;
        private BrightIdeasSoftware.OLVColumn olvColMFNumTime;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private System.Windows.Forms.Button btnRefreshGlobalTime;
        private System.Windows.Forms.RadioButton rbtnMFNum;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbTimeUnit;
    }
}
