namespace Heiflow.Models.Hydrodynamics.Susbed
{
    partial class OverlandFlowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverlandFlowForm));
            this.olvTributaryPara = new System.Windows.Forms.DataGridView();
            this.olvColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PervN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.olvColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            this.olvColumn8,
            this.PervN,
            this.olvColumn9,
            this.olvColumn11,
            this.olvColumn6});
            this.olvTributaryPara.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvTributaryPara.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvTributaryPara.Location = new System.Drawing.Point(9, 39);
            this.olvTributaryPara.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvTributaryPara.Name = "olvTributaryPara";
            this.olvTributaryPara.AllowUserToAddRows = false;
            this.olvTributaryPara.AllowUserToDeleteRows = false;
            this.olvTributaryPara.AutoGenerateColumns = false;
            this.olvTributaryPara.RowHeadersVisible = false;
            this.olvTributaryPara.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.olvTributaryPara.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.olvTributaryPara.Size = new System.Drawing.Size(1085, 485);
            this.olvTributaryPara.TabIndex = 32;
            // 
            // olvColumn1
            // 
            this.olvColumn1.DataPropertyName = "Name";
            this.olvColumn1.HeaderText = "子流域名称";
            this.olvColumn1.Name = "olvColumn1";
            this.olvColumn1.Width = 81;
            // 
            // olvColumn2
            // 
            this.olvColumn2.DataPropertyName = "RainGage";
            this.olvColumn2.HeaderText = "雨量站编号";
            this.olvColumn2.Name = "olvColumn2";
            this.olvColumn2.Width = 79;
            // 
            // olvColumn3
            // 
            this.olvColumn3.DataPropertyName = "Area";
            this.olvColumn3.HeaderText = "面积";
            this.olvColumn3.Name = "olvColumn3";
            // 
            // olvColumn4
            // 
            this.olvColumn4.DataPropertyName = "Slope";
            this.olvColumn4.HeaderText = "坡度(%)";
            this.olvColumn4.Name = "olvColumn4";
            this.olvColumn4.Width = 69;
            // 
            // olvColumn5
            // 
            this.olvColumn5.DataPropertyName = "ImpervPercentage";
            this.olvColumn5.HeaderText = "不透水面积比例(%)";
            this.olvColumn5.Name = "olvColumn5";
            this.olvColumn5.Width = 117;
            // 
            // olvColumn7
            // 
            this.olvColumn7.DataPropertyName = "N_Imperv";
            this.olvColumn7.HeaderText = "不透水曼宁系数";
            this.olvColumn7.Name = "olvColumn7";
            this.olvColumn7.Width = 106;
            // 
            // olvColumn8
            // 
            this.olvColumn8.DataPropertyName = "S_Imperv";
            this.olvColumn8.HeaderText = "不透水滞水深";
            this.olvColumn8.Name = "olvColumn8";
            this.olvColumn8.Width = 101;
            // 
            // PervN
            // 
            this.PervN.DataPropertyName = "N_Perv";
            this.PervN.HeaderText = "透水曼宁系数";
            this.PervN.Name = "PervN";
            this.PervN.Width = 90;
            // 
            // olvColumn9
            // 
            this.olvColumn9.DataPropertyName = "S_Perv";
            this.olvColumn9.HeaderText = "透水滞水深";
            this.olvColumn9.Name = "olvColumn9";
            this.olvColumn9.Width = 75;
            // 
            // olvColumn11
            // 
            this.olvColumn11.DataPropertyName = "PctZero";
            this.olvColumn11.HeaderText = "无滞留不透水面积比例";
            this.olvColumn11.Name = "olvColumn11";
            this.olvColumn11.Width = 141;
            // 
            // olvColumn6
            // 
            this.olvColumn6.DataPropertyName = "SectionID";
            this.olvColumn6.HeaderText = "汇入断面编号";
            this.olvColumn6.Name = "olvColumn6";
            this.olvColumn6.Width = 96;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(1022, 534);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 30);
            this.btnClose.TabIndex = 33;
            this.btnClose.Text = "关  闭";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(102, 534);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 30);
            this.btnRemove.TabIndex = 34;
            this.btnRemove.Text = "删  除";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(12, 534);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 30);
            this.btnAdd.TabIndex = 35;
            this.btnAdd.Text = "添  加";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(931, 534);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 36;
            this.btnSave.Text = "保  存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 12);
            this.label3.TabIndex = 37;
            this.label3.Text = "请在下表中输入子流域信息";
            // 
            // OverlandFlowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1107, 575);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.olvTributaryPara);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OverlandFlowForm";
            this.Text = "产流过程";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView olvTributaryPara;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn PervN;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn11;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn olvColumn6;
        private System.Windows.Forms.Label label3;
    }
}
