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
            this.olvSections = new System.Windows.Forms.DataGridView();
            this.olvColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownSection = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.olvTributaryPara = new System.Windows.Forms.DataGridView();
            this.olvColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.olvPointSource = new System.Windows.Forms.DataGridView();
            this.olvColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSection)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
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
            this.olvSections.AllowUserToOrderColumns = true;
            this.olvSections.AllowDrop = true;
            this.olvSections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvSections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.olvColumn12});
            this.olvSections.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvSections.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvSections.Location = new System.Drawing.Point(8, 45);
            this.olvSections.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvSections.Name = "olvSections";
            this.olvSections.AllowUserToAddRows = false;
            this.olvSections.AllowUserToDeleteRows = false;
            this.olvSections.AutoGenerateColumns = false;
            this.olvSections.RowHeadersVisible = false;
            this.olvSections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvSections.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvSections.Size = new System.Drawing.Size(629, 426);
            this.olvSections.TabIndex = 32;
            // 
            // olvColumn12
            // 
            this.olvColumn12.DataPropertyName = "NO";
            this.olvColumn12.HeaderText = "断面编号";
            this.olvColumn12.Name = "olvColumn12";
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
            this.olvTributaryPara.AllowUserToOrderColumns = true;
            this.olvTributaryPara.AllowDrop = true;
            this.olvTributaryPara.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvTributaryPara.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5,
            this.olvColumn7,
            this.olvColumn6});
            this.olvTributaryPara.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvTributaryPara.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvTributaryPara.Location = new System.Drawing.Point(8, 45);
            this.olvTributaryPara.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvTributaryPara.Name = "olvTributaryPara";
            this.olvTributaryPara.AllowUserToAddRows = false;
            this.olvTributaryPara.AllowUserToDeleteRows = false;
            this.olvTributaryPara.AutoGenerateColumns = false;
            this.olvTributaryPara.RowHeadersVisible = false;
            this.olvTributaryPara.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvTributaryPara.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvTributaryPara.Size = new System.Drawing.Size(686, 433);
            this.olvTributaryPara.TabIndex = 31;
            this.olvTributaryPara.SelectionChanged += new System.EventHandler(this.olvTributaryPara_ItemSelectionChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.DataPropertyName = "NO";
            this.olvColumn1.HeaderText = "支流编号";
            this.olvColumn1.Name = "olvColumn1";
            this.olvColumn1.Width = 81;
            // 
            // olvColumn2
            // 
            this.olvColumn2.DataPropertyName = "MFH";
            this.olvColumn2.HeaderText = "分汇类型";
            this.olvColumn2.Name = "olvColumn2";
            this.olvColumn2.Width = 91;
            // 
            // olvColumn3
            // 
            this.olvColumn3.DataPropertyName = "MQ";
            this.olvColumn3.HeaderText = "Q形式";
            this.olvColumn3.Name = "olvColumn3";
            this.olvColumn3.Width = 70;
            // 
            // olvColumn4
            // 
            this.olvColumn4.DataPropertyName = "MS";
            this.olvColumn4.HeaderText = "S形式";
            this.olvColumn4.Name = "olvColumn4";
            this.olvColumn4.Width = 75;
            // 
            // olvColumn5
            // 
            this.olvColumn5.DataPropertyName = "UpperSectionID";
            this.olvColumn5.HeaderText = "进口断面编号";
            this.olvColumn5.Name = "olvColumn5";
            this.olvColumn5.Width = 92;
            // 
            // olvColumn7
            // 
            this.olvColumn7.DataPropertyName = "LowerSectionID";
            this.olvColumn7.HeaderText = "出口断面编号";
            this.olvColumn7.Name = "olvColumn7";
            this.olvColumn7.Width = 98;
            // 
            // olvColumn6
            // 
            this.olvColumn6.DataPropertyName = "Name";
            this.olvColumn6.HeaderText = "入汇点名称";
            this.olvColumn6.Name = "olvColumn6";
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
            this.olvPointSource.AllowUserToOrderColumns = true;
            this.olvPointSource.AllowDrop = true;
            this.olvPointSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvPointSource.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.olvColumn8,
            this.olvColumn9,
            this.olvColumn10,
            this.olvColumn11,
            this.olvColumn13,
            this.olvColumn14});
            this.olvPointSource.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPointSource.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvPointSource.Location = new System.Drawing.Point(8, 45);
            this.olvPointSource.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvPointSource.Name = "olvPointSource";
            this.olvPointSource.AllowUserToAddRows = false;
            this.olvPointSource.AllowUserToDeleteRows = false;
            this.olvPointSource.AutoGenerateColumns = false;
            this.olvPointSource.RowHeadersVisible = false;
            this.olvPointSource.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvPointSource.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvPointSource.Size = new System.Drawing.Size(686, 433);
            this.olvPointSource.TabIndex = 33;
            this.olvPointSource.SelectionChanged += new System.EventHandler(this.olvPointSource_ItemSelectionChanged);
            // 
            // olvColumn8
            // 
            this.olvColumn8.DataPropertyName = "NO";
            this.olvColumn8.HeaderText = "支流编号";
            this.olvColumn8.Name = "olvColumn8";
            this.olvColumn8.Width = 81;
            // 
            // olvColumn9
            // 
            this.olvColumn9.DataPropertyName = "MFH";
            this.olvColumn9.HeaderText = "分汇类型";
            this.olvColumn9.Name = "olvColumn9";
            this.olvColumn9.Width = 91;
            // 
            // olvColumn10
            // 
            this.olvColumn10.DataPropertyName = "MQ";
            this.olvColumn10.HeaderText = "Q形式";
            this.olvColumn10.Name = "olvColumn10";
            this.olvColumn10.Width = 70;
            // 
            // olvColumn11
            // 
            this.olvColumn11.DataPropertyName = "MS";
            this.olvColumn11.HeaderText = "S形式";
            this.olvColumn11.Name = "olvColumn11";
            this.olvColumn11.Width = 75;
            // 
            // olvColumn13
            // 
            this.olvColumn13.DataPropertyName = "LowerSectionID";
            this.olvColumn13.HeaderText = "下游断面编号";
            this.olvColumn13.Name = "olvColumn13";
            this.olvColumn13.Width = 98;
            // 
            // olvColumn14
            // 
            this.olvColumn14.DataPropertyName = "Name";
            this.olvColumn14.HeaderText = "入汇点名称";
            this.olvColumn14.Name = "olvColumn14";
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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSection)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
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
        private System.Windows.Forms.DataGridView olvTributaryPara;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn7;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView olvPointSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn14;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView olvSections;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn12;

    }
}
