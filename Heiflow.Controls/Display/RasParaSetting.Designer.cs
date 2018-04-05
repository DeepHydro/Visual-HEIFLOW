namespace Heiflow.Controls.WinForm.Display
{
    partial class RasParaSetting
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
            this.cmbLayers = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbGrid = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPeriods = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnApply = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbLayers
            // 
            this.cmbLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayers.FormattingEnabled = true;
            this.cmbLayers.Location = new System.Drawing.Point(328, 12);
            this.cmbLayers.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(433, 27);
            this.cmbLayers.TabIndex = 2;
            this.cmbLayers.SelectedIndexChanged += new System.EventHandler(this.cmbLayers_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cmbGrid);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cmbLayers);
            this.panel1.Controls.Add(this.cmbPeriods);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(7, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 87);
            this.panel1.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(232, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 19);
            this.label4.TabIndex = 5;
            this.label4.Text = "Grid Layer";
            // 
            // cmbGrid
            // 
            this.cmbGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGrid.FormattingEnabled = true;
            this.cmbGrid.Location = new System.Drawing.Point(328, 47);
            this.cmbGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbGrid.Name = "cmbGrid";
            this.cmbGrid.Size = new System.Drawing.Size(433, 27);
            this.cmbGrid.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(232, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Raster Layer";
            // 
            // cmbPeriods
            // 
            this.cmbPeriods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPeriods.FormattingEnabled = true;
            this.cmbPeriods.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cmbPeriods.Location = new System.Drawing.Point(99, 12);
            this.cmbPeriods.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbPeriods.Name = "cmbPeriods";
            this.cmbPeriods.Size = new System.Drawing.Size(121, 27);
            this.cmbPeriods.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "Time Period";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(7, 135);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(761, 342);
            this.dataGridView1.TabIndex = 5;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.btnApply.Location = new System.Drawing.Point(568, 495);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(95, 30);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Parameter Mapping Table";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.btnCancel.Location = new System.Drawing.Point(678, 495);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImport.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.btnImport.Location = new System.Drawing.Point(7, 495);
            this.btnImport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(90, 35);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Import...";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.btnExport.Location = new System.Drawing.Point(115, 495);
            this.btnExport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 35);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export...";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // RasParaSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(780, 543);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RasParaSetting";
            this.Text = "Setting Parameter";
            this.Load += new System.EventHandler(this.RasParaSetting_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbLayers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbPeriods;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbGrid;
    }
}
