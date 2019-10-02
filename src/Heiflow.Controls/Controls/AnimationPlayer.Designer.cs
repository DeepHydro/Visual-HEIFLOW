namespace Heiflow.Controls.WinForm.Controls
{
    partial class AnimationPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationPlayer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cmbAnimators = new System.Windows.Forms.ToolStripComboBox();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.listBox_timeline = new System.Windows.Forms.ListBox();
            this.olvDataCubeTree = new BrightIdeasSoftware.DataTreeListView();
            this.olvColumnState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDataCubeTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbAnimators,
            this.btnPlay});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(432, 28);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // cmbAnimators
            // 
            this.cmbAnimators.Name = "cmbAnimators";
            this.cmbAnimators.Size = new System.Drawing.Size(146, 28);
            this.cmbAnimators.SelectedIndexChanged += new System.EventHandler(this.cmbAnimators_SelectedIndexChanged);
            // 
            // btnPlay
            // 
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericBlueRightArrowNoTail32;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(24, 25);
            this.btnPlay.Text = "Play";
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // listBox_timeline
            // 
            this.listBox_timeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_timeline.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox_timeline.FormattingEnabled = true;
            this.listBox_timeline.ItemHeight = 21;
            this.listBox_timeline.Location = new System.Drawing.Point(0, 0);
            this.listBox_timeline.Name = "listBox_timeline";
            this.listBox_timeline.Size = new System.Drawing.Size(432, 294);
            this.listBox_timeline.TabIndex = 12;
            this.listBox_timeline.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxTimeLine_DrawItem);
            this.listBox_timeline.SelectedIndexChanged += new System.EventHandler(this.listBoxTimeLine_SelectedIndexChanged);
            // 
            // olvDataCubeTree
            // 
            this.olvDataCubeTree.AllColumns.Add(this.olvColumnState);
            this.olvDataCubeTree.AllColumns.Add(this.olvColumn1);
            this.olvDataCubeTree.AllColumns.Add(this.olvColumn2);
            this.olvDataCubeTree.AllColumns.Add(this.olvColumn3);
            this.olvDataCubeTree.AutoGenerateColumns = false;
            this.olvDataCubeTree.CellEditUseWholeCell = false;
            this.olvDataCubeTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnState,
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.olvDataCubeTree.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvDataCubeTree.DataSource = null;
            this.olvDataCubeTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvDataCubeTree.FullRowSelect = true;
            this.olvDataCubeTree.GridLines = true;
            this.olvDataCubeTree.HideSelection = false;
            this.olvDataCubeTree.KeyAspectName = "ID";
            this.olvDataCubeTree.Location = new System.Drawing.Point(0, 0);
            this.olvDataCubeTree.Margin = new System.Windows.Forms.Padding(4);
            this.olvDataCubeTree.MultiSelect = false;
            this.olvDataCubeTree.Name = "olvDataCubeTree";
            this.olvDataCubeTree.ParentKeyAspectName = "ParentID";
            this.olvDataCubeTree.RootKeyValueString = "";
            this.olvDataCubeTree.SelectedBackColor = System.Drawing.Color.LimeGreen;
            this.olvDataCubeTree.SelectedForeColor = System.Drawing.Color.White;
            this.olvDataCubeTree.ShowGroups = false;
            this.olvDataCubeTree.ShowKeyColumns = false;
            this.olvDataCubeTree.Size = new System.Drawing.Size(432, 298);
            this.olvDataCubeTree.TabIndex = 13;
            this.olvDataCubeTree.UnfocusedSelectedBackColor = System.Drawing.Color.LimeGreen;
            this.olvDataCubeTree.UnfocusedSelectedForeColor = System.Drawing.Color.White;
            this.olvDataCubeTree.UseCompatibleStateImageBehavior = false;
            this.olvDataCubeTree.UseFilterIndicator = true;
            this.olvDataCubeTree.UseFiltering = true;
            this.olvDataCubeTree.UseHotItem = true;
            this.olvDataCubeTree.View = System.Windows.Forms.View.Details;
            this.olvDataCubeTree.VirtualMode = true;
            this.olvDataCubeTree.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.olvDataCubeTree_ItemSelectionChanged);
            // 
            // olvColumnState
            // 
            this.olvColumnState.AspectName = "";
            this.olvColumnState.Text = "State";
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 104;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Size";
            this.olvColumn2.AspectToStringFormat = "";
            this.olvColumn2.Text = "Size";
            this.olvColumn2.Width = 117;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Owner";
            this.olvColumn3.AspectToStringFormat = "";
            this.olvColumn3.Text = "Owner";
            this.olvColumn3.Width = 210;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.olvDataCubeTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox_timeline);
            this.splitContainer1.Size = new System.Drawing.Size(432, 596);
            this.splitContainer1.SplitterDistance = 298;
            this.splitContainer1.TabIndex = 14;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ready");
            this.imageList1.Images.SetKeyName(1, "standby");
            // 
            // AnimationPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "AnimationPlayer";
            this.Size = new System.Drawing.Size(432, 624);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDataCubeTree)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ListBox listBox_timeline;
        private System.Windows.Forms.ToolStripComboBox cmbAnimators;
        private BrightIdeasSoftware.DataTreeListView olvDataCubeTree;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BrightIdeasSoftware.OLVColumn olvColumnState;
        private System.Windows.Forms.ImageList imageList1;

    }
}
