namespace Heiflow.Controls.WinForm.Controls
{
    partial class View3DControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(View3DControl));
            this.ilPanel1 = new ILNumerics.Drawing.ILPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLengend = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveExisted = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnShowSeries = new System.Windows.Forms.ToolStripButton();
            this.cmbColorMap = new System.Windows.Forms.ToolStripComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlRight = new System.Windows.Forms.TabControl();
            this.tabPageSeries = new System.Windows.Forms.TabPage();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cmbZScale = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlRight.SuspendLayout();
            this.tabPageSeries.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilPanel1
            // 
            this.ilPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ilPanel1.Driver = ILNumerics.Drawing.RendererTypes.OpenGL;
            this.ilPanel1.Editor = null;
            this.ilPanel1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.ilPanel1.Location = new System.Drawing.Point(0, 0);
            this.ilPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ilPanel1.Name = "ilPanel1";
            this.ilPanel1.Rectangle = ((System.Drawing.RectangleF)(resources.GetObject("ilPanel1.Rectangle")));
            this.ilPanel1.ShowUIControls = false;
            this.ilPanel1.Size = new System.Drawing.Size(656, 525);
            this.ilPanel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLengend,
            this.btnRemoveExisted,
            this.btnClear,
            this.toolStripSeparator1,
            this.btnShowSeries,
            this.cmbColorMap,
            this.cmbZScale});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(834, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLengend
            // 
            this.btnLengend.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLengend.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_stock_chart_toggle_legend_93841;
            this.btnLengend.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLengend.Name = "btnLengend";
            this.btnLengend.Size = new System.Drawing.Size(24, 24);
            this.btnLengend.Text = "Show legend";
            // 
            // btnRemoveExisted
            // 
            this.btnRemoveExisted.Checked = true;
            this.btnRemoveExisted.CheckOnClick = true;
            this.btnRemoveExisted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnRemoveExisted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemoveExisted.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_selected_delete_37293;
            this.btnRemoveExisted.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveExisted.Name = "btnRemoveExisted";
            this.btnRemoveExisted.Size = new System.Drawing.Size(24, 24);
            this.btnRemoveExisted.Text = "Remove existed graphy";
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_edit_clear_15273;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnShowSeries
            // 
            this.btnShowSeries.Checked = true;
            this.btnShowSeries.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnShowSeries.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShowSeries.Image = global::Heiflow.Controls.WinForm.Properties.Resources.if_Side_Panel;
            this.btnShowSeries.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowSeries.Name = "btnShowSeries";
            this.btnShowSeries.Size = new System.Drawing.Size(24, 24);
            this.btnShowSeries.Text = "Series Panel";
            this.btnShowSeries.Click += new System.EventHandler(this.btnShowSeries_Click);
            // 
            // cmbColorMap
            // 
            this.cmbColorMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColorMap.Items.AddRange(new object[] {
            "Autumn",
            "Bone",
            "Colorcube",
            "Cool",
            "Copper",
            "Flag",
            "Gray",
            "Hot",
            "Hsv",
            "ILNumerics",
            "Jet",
            "Lines",
            "Pink",
            "Prism",
            "Spring",
            "Summer",
            "White",
            "Winter"});
            this.cmbColorMap.Name = "cmbColorMap";
            this.cmbColorMap.Size = new System.Drawing.Size(121, 27);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ilPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlRight);
            this.splitContainer1.Size = new System.Drawing.Size(834, 525);
            this.splitContainer1.SplitterDistance = 656;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControlRight
            // 
            this.tabControlRight.Controls.Add(this.tabPageSeries);
            this.tabControlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlRight.Location = new System.Drawing.Point(0, 0);
            this.tabControlRight.Multiline = true;
            this.tabControlRight.Name = "tabControlRight";
            this.tabControlRight.SelectedIndex = 0;
            this.tabControlRight.Size = new System.Drawing.Size(174, 525);
            this.tabControlRight.TabIndex = 2;
            // 
            // tabPageSeries
            // 
            this.tabPageSeries.Controls.Add(this.checkedListBox1);
            this.tabPageSeries.Location = new System.Drawing.Point(4, 22);
            this.tabPageSeries.Name = "tabPageSeries";
            this.tabPageSeries.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSeries.Size = new System.Drawing.Size(166, 499);
            this.tabPageSeries.TabIndex = 0;
            this.tabPageSeries.Text = "Series";
            this.tabPageSeries.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.ColumnWidth = 400;
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.HorizontalScrollbar = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "series1"});
            this.checkedListBox1.Location = new System.Drawing.Point(3, 3);
            this.checkedListBox1.MultiColumn = true;
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(160, 493);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // cmbZScale
            // 
            this.cmbZScale.Items.AddRange(new object[] {
            "0.1",
            "0.2",
            "0.5",
            "1.0",
            "1.5",
            "2.0",
            "5.0",
            "10.0"});
            this.cmbZScale.Name = "cmbZScale";
            this.cmbZScale.Size = new System.Drawing.Size(121, 27);
            this.cmbZScale.Text = "1.0";
            // 
            // View3DControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "View3DControl";
            this.Size = new System.Drawing.Size(834, 552);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlRight.ResumeLayout(false);
            this.tabPageSeries.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ILNumerics.Drawing.ILPanel ilPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlRight;
        private System.Windows.Forms.TabPage tabPageSeries;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.ToolStripButton btnLengend;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripButton btnShowSeries;
        private System.Windows.Forms.ToolStripButton btnRemoveExisted;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox cmbColorMap;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripComboBox cmbZScale;
    }
}
