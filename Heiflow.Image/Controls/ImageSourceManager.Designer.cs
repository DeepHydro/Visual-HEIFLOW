namespace Heiflow.Image.Controls
{
    partial class ImageSourceManager
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
            this.treeView1 = new Heiflow.Controls.Tree.TreeViewAdv();
            this.nodeTextBox1 = new Heiflow.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeStateIcon1 = new Heiflow.Controls.Tree.NodeControls.NodeStateIcon();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.BackColor2 = System.Drawing.SystemColors.Window;
            this.treeView1.BackgroundPaintMode = Heiflow.Controls.Tree.BackgroundPaintMode.Default;
            this.treeView1.DefaultToolTipProvider = null;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeView1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.treeView1.HighlightColorActive = System.Drawing.SystemColors.Highlight;
            this.treeView1.HighlightColorInactive = System.Drawing.SystemColors.InactiveBorder;
            this.treeView1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(2);
            this.treeView1.Model = null;
            this.treeView1.Name = "treeView1";
            this.treeView1.NodeControls.Add(this.nodeStateIcon1);
            this.treeView1.NodeControls.Add(this.nodeTextBox1);
            this.treeView1.OnVisibleOverride = null;
            this.treeView1.SelectedNode = null;
            this.treeView1.Size = new System.Drawing.Size(341, 734);
            this.treeView1.TabIndex = 3;
            this.treeView1.Text = "treeViewAdv1";
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = null;
            // 
            // nodeStateIcon1
            // 
            this.nodeStateIcon1.LeftMargin = 1;
            this.nodeStateIcon1.ParentColumn = null;
            this.nodeStateIcon1.ScaleMode = Heiflow.Controls.Tree.ImageScaleMode.Clip;
            // 
            // ImageSourceManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ImageSourceManager";
            this.Size = new System.Drawing.Size(341, 734);
            this.ResumeLayout(false);

        }

        #endregion

        private Heiflow.Controls.Tree.TreeViewAdv treeView1;
        private Heiflow.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private Heiflow.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
    }
}
