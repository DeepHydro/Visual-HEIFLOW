namespace Heiflow.Presentation.Controls.Project
{
    partial class ProjectExplorerControl
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
            this._nodeStateIcon = new Heiflow.Controls.Tree.NodeControls.NodeStateIcon();
            this._nodeTextBox = new Heiflow.Controls.Tree.NodeControls.NodeTextBox();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.WindowText;
            this.treeView1.BackColor2 = System.Drawing.SystemColors.WindowFrame;
            this.treeView1.BackgroundPaintMode = Heiflow.Controls.Tree.BackgroundPaintMode.Default;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.DefaultToolTipProvider = null;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeView1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.treeView1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.treeView1.HighlightColorActive = System.Drawing.SystemColors.Highlight;
            this.treeView1.HighlightColorInactive = System.Drawing.SystemColors.InactiveBorder;
            this.treeView1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Model = null;
            this.treeView1.Name = "treeView1";
            this.treeView1.NodeControls.Add(this._nodeStateIcon);
            this.treeView1.NodeControls.Add(this._nodeTextBox);
            this.treeView1.OnVisibleOverride = null;
            this.treeView1.SelectedNode = null;
            this.treeView1.Size = new System.Drawing.Size(187, 428);
            this.treeView1.TabIndex = 0;
            this.treeView1.Text = "treeViewAdv1";
            // 
            // _nodeStateIcon
            // 
            this._nodeStateIcon.LeftMargin = 1;
            this._nodeStateIcon.ParentColumn = null;
            this._nodeStateIcon.ScaleMode = Heiflow.Controls.Tree.ImageScaleMode.Clip;
            // 
            // _nodeTextBox
            // 
            this._nodeTextBox.DataPropertyName = "Text";
            this._nodeTextBox.EditEnabled = true;
            this._nodeTextBox.Font = new System.Drawing.Font("Calibri", 10.5F);
            this._nodeTextBox.IncrementalSearchEnabled = true;
            this._nodeTextBox.LeftMargin = 3;
            this._nodeTextBox.ParentColumn = null;
            // 
            // ProjectExplorerControl
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.treeView1);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Name = "ProjectExplorerControl";
            this.ResumeLayout(false);

        }

        #endregion

        private   Heiflow.Controls.Tree.TreeViewAdv treeView1;
        private Heiflow.Controls.Tree.NodeControls.NodeTextBox _nodeTextBox;
        private Heiflow.Controls.Tree.NodeControls.NodeStateIcon _nodeStateIcon;



    }
}
