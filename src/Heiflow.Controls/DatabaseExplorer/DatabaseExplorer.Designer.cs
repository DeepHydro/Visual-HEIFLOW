namespace Heiflow.Controls.WinForm.DatabaseExplorer
{
    partial class DatabaseExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseExplorer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnConectODM = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnGroupBySiteState = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGroupBySiteCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGroupByVariable = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveDB = new System.Windows.Forms.ToolStripButton();
            this.imageList_odm = new System.Windows.Forms.ImageList(this.components);
            this.treeView1 = new Heiflow.Controls.Tree.TreeViewAdv();
            this.nodeStateIcon1 = new Heiflow.Controls.Tree.NodeControls.NodeStateIcon();
            this.nodeTextBox1 = new Heiflow.Controls.Tree.NodeControls.NodeTextBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnConectODM,
            this.btnRefresh,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.btnImport,
            this.btnRemoveDB});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(373, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnConectODM
            // 
            this.btnConectODM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConectODM.Image = global::Heiflow.Controls.WinForm.Properties.Resources.add_db;
            this.btnConectODM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConectODM.Name = "btnConectODM";
            this.btnConectODM.Size = new System.Drawing.Size(24, 24);
            this.btnConectODM.Text = "Connect to an ODM Database";
            this.btnConectODM.Click += new System.EventHandler(this.btnConectODM_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::Heiflow.Controls.WinForm.Properties.Resources.cat_db;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(24, 24);
            this.btnRefresh.Text = "Connect to the ODM Database";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnGroupBySiteState,
            this.btnGroupBySiteCategory,
            this.btnGroupByVariable});
            this.toolStripDropDownButton1.Image = global::Heiflow.Controls.WinForm.Properties.Resources.search_db;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 24);
            this.toolStripDropDownButton1.Text = "View";
            // 
            // btnGroupBySiteState
            // 
            this.btnGroupBySiteState.Name = "btnGroupBySiteState";
            this.btnGroupBySiteState.Size = new System.Drawing.Size(254, 26);
            this.btnGroupBySiteState.Text = "Group By Site State";
            this.btnGroupBySiteState.Click += new System.EventHandler(this.btnGroupBySiteState_Click);
            // 
            // btnGroupBySiteCategory
            // 
            this.btnGroupBySiteCategory.Name = "btnGroupBySiteCategory";
            this.btnGroupBySiteCategory.Size = new System.Drawing.Size(254, 26);
            this.btnGroupBySiteCategory.Text = "Group By Site Category";
            this.btnGroupBySiteCategory.Click += new System.EventHandler(this.btnGroupBySiteState_Click);
            // 
            // btnGroupByVariable
            // 
            this.btnGroupByVariable.Checked = true;
            this.btnGroupByVariable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnGroupByVariable.Name = "btnGroupByVariable";
            this.btnGroupByVariable.Size = new System.Drawing.Size(254, 26);
            this.btnGroupByVariable.Text = "Group By Variable";
            this.btnGroupByVariable.Click += new System.EventHandler(this.btnGroupBySiteState_Click);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.import_db;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(24, 24);
            this.btnImport.Text = "Import data to ODM Database";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnRemoveDB
            // 
            this.btnRemoveDB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemoveDB.Image = global::Heiflow.Controls.WinForm.Properties.Resources.remove_db;
            this.btnRemoveDB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveDB.Name = "btnRemoveDB";
            this.btnRemoveDB.Size = new System.Drawing.Size(24, 24);
            this.btnRemoveDB.Text = "Remove database";
            this.btnRemoveDB.Click += new System.EventHandler(this.btnRemoveDB_Click);
            // 
            // imageList_odm
            // 
            this.imageList_odm.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_odm.ImageStream")));
            this.imageList_odm.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_odm.Images.SetKeyName(0, "FolderBlue16.png");
            this.imageList_odm.Images.SetKeyName(1, "Gallery16.png");
            this.imageList_odm.Images.SetKeyName(2, "GenericOptions16.png");
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.BackColor2 = System.Drawing.SystemColors.Window;
            this.treeView1.BackgroundPaintMode = Heiflow.Controls.Tree.BackgroundPaintMode.Default;
            this.treeView1.DefaultToolTipProvider = null;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeView1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.treeView1.HighlightColorActive = System.Drawing.SystemColors.Highlight;
            this.treeView1.HighlightColorInactive = System.Drawing.SystemColors.InactiveBorder;
            this.treeView1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeView1.Location = new System.Drawing.Point(0, 27);
            this.treeView1.Model = null;
            this.treeView1.Name = "treeView1";
            this.treeView1.NodeControls.Add(this.nodeStateIcon1);
            this.treeView1.NodeControls.Add(this.nodeTextBox1);
            this.treeView1.OnVisibleOverride = null;
            this.treeView1.SelectedNode = null;
            this.treeView1.Size = new System.Drawing.Size(373, 784);
            this.treeView1.TabIndex = 2;
            this.treeView1.Text = "treeViewAdv1";
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // nodeStateIcon1
            // 
            this.nodeStateIcon1.LeftMargin = 1;
            this.nodeStateIcon1.ParentColumn = null;
            this.nodeStateIcon1.ScaleMode = Heiflow.Controls.Tree.ImageScaleMode.Fit;
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "Text";
            this.nodeTextBox1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = null;
            // 
            // DatabaseExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "DatabaseExplorer";
            this.Size = new System.Drawing.Size(373, 811);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnConectODM;
        private Tree.TreeViewAdv treeView1;
        private Tree.NodeControls.NodeTextBox nodeTextBox1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ImageList imageList_odm;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem btnGroupBySiteState;
        private System.Windows.Forms.ToolStripMenuItem btnGroupBySiteCategory;
        private System.Windows.Forms.ToolStripMenuItem btnGroupByVariable;
        private Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private System.Windows.Forms.ToolStripButton btnRemoveDB;
    }
}
