namespace Heiflow.Presentation.Controls.Project
{
    partial class NewProjectionItemForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectionItemForm));
            this.tbModelDes = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.olvSimple = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.Mandatory = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.treeView1 = new Heiflow.Controls.Tree.TreeViewAdv();
            this.nodeStateIcon1 = new Heiflow.Controls.Tree.NodeControls.NodeStateIcon();
            this._nodeTextBox = new Heiflow.Controls.Tree.NodeControls.NodeTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.olvSimple)).BeginInit();
            this.SuspendLayout();
            // 
            // tbModelDes
            // 
            this.tbModelDes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbModelDes.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbModelDes.Location = new System.Drawing.Point(226, 289);
            this.tbModelDes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbModelDes.Multiline = true;
            this.tbModelDes.Name = "tbModelDes";
            this.tbModelDes.ReadOnly = true;
            this.tbModelDes.Size = new System.Drawing.Size(628, 166);
            this.tbModelDes.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(631, 471);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(750, 471);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // olvSimple
            // 
            this.olvSimple.AllColumns.Add(this.olvColumn2);
            this.olvSimple.AllColumns.Add(this.olvColumn1);
            this.olvSimple.AllColumns.Add(this.olvColumn3);
            this.olvSimple.AllColumns.Add(this.Mandatory);
            this.olvSimple.AllColumns.Add(this.olvColumn4);
            this.olvSimple.AllowColumnReorder = true;
            this.olvSimple.AllowDrop = true;
            this.olvSimple.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvSimple.CellEditUseWholeCell = false;
            this.olvSimple.CheckBoxes = true;
            this.olvSimple.CheckedAspectName = "IsUsed";
            this.olvSimple.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn2,
            this.olvColumn1,
            this.olvColumn3,
            this.Mandatory,
            this.olvColumn4});
            this.olvSimple.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvSimple.FullRowSelect = true;
            this.olvSimple.HeaderWordWrap = true;
            this.olvSimple.HideSelection = false;
            this.olvSimple.IncludeColumnHeadersInCopy = true;
            this.olvSimple.Location = new System.Drawing.Point(226, 11);
            this.olvSimple.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvSimple.Name = "olvSimple";
            this.olvSimple.OverlayText.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            this.olvSimple.OverlayText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.olvSimple.OverlayText.BorderWidth = 2F;
            this.olvSimple.OverlayText.Rotation = -20;
            this.olvSimple.OverlayText.Text = "";
            this.olvSimple.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvSimple.ShowCommandMenuOnRightClick = true;
            this.olvSimple.ShowGroups = false;
            this.olvSimple.ShowHeaderInAllViews = false;
            this.olvSimple.ShowItemToolTips = true;
            this.olvSimple.Size = new System.Drawing.Size(628, 271);
            this.olvSimple.SortGroupItemsByPrimaryColumn = false;
            this.olvSimple.TabIndex = 31;
            this.olvSimple.TriStateCheckBoxes = true;
            this.olvSimple.UseAlternatingBackColors = true;
            this.olvSimple.UseCellFormatEvents = true;
            this.olvSimple.UseCompatibleStateImageBehavior = false;
            this.olvSimple.UseFilterIndicator = true;
            this.olvSimple.UseFiltering = true;
            this.olvSimple.UseHotItem = true;
            this.olvSimple.View = System.Windows.Forms.View.Details;
            this.olvSimple.SubItemChecking += new System.EventHandler<BrightIdeasSoftware.SubItemCheckingEventArgs>(this.olvSimple_SubItemChecking);
            this.olvSimple.SelectionChanged += new System.EventHandler(this.olvSimple_SelectionChanged);
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Name";
            this.olvColumn2.Text = "Name";
            this.olvColumn2.Width = 132;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "FullName";
            this.olvColumn1.Text = "Full Name";
            this.olvColumn1.Width = 169;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Version";
            this.olvColumn3.Text = "Version";
            this.olvColumn3.Width = 106;
            // 
            // Mandatory
            // 
            this.Mandatory.AspectName = "IsMandatory";
            this.Mandatory.Text = "Mandatory";
            this.Mandatory.Width = 115;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "IsUsed";
            this.olvColumn4.CheckBoxes = true;
            this.olvColumn4.Text = "Using";
            this.olvColumn4.Width = 125;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.BackColor2 = System.Drawing.SystemColors.Window;
            this.treeView1.BackgroundPaintMode = Heiflow.Controls.Tree.BackgroundPaintMode.Default;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.DefaultToolTipProvider = null;
            this.treeView1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeView1.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.treeView1.HighlightColorActive = System.Drawing.SystemColors.Highlight;
            this.treeView1.HighlightColorInactive = System.Drawing.SystemColors.InactiveBorder;
            this.treeView1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeView1.Location = new System.Drawing.Point(12, 11);
            this.treeView1.Model = null;
            this.treeView1.Name = "treeView1";
            this.treeView1.NodeControls.Add(this.nodeStateIcon1);
            this.treeView1.NodeControls.Add(this._nodeTextBox);
            this.treeView1.OnVisibleOverride = null;
            this.treeView1.SelectedNode = null;
            this.treeView1.Size = new System.Drawing.Size(207, 445);
            this.treeView1.TabIndex = 4;
            this.treeView1.Text = "treeViewAdv1";
            // 
            // nodeStateIcon1
            // 
            this.nodeStateIcon1.LeftMargin = 1;
            this.nodeStateIcon1.ParentColumn = null;
            this.nodeStateIcon1.ScaleMode = Heiflow.Controls.Tree.ImageScaleMode.Clip;
            // 
            // _nodeTextBox
            // 
            this._nodeTextBox.IncrementalSearchEnabled = true;
            this._nodeTextBox.LeftMargin = 3;
            this._nodeTextBox.ParentColumn = null;
            // 
            // NewProjectionItemForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(862, 513);
            this.Controls.Add(this.olvSimple);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbModelDes);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectionItemForm";
            this.Text = "New Package";
            this.Load += new System.EventHandler(this.NewPrjForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.olvSimple)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbModelDes;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private Heiflow.Controls.Tree.TreeViewAdv treeView1;
        private Heiflow.Controls.Tree.NodeControls.NodeTextBox _nodeTextBox;
        private Heiflow.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon1;
        private BrightIdeasSoftware.ObjectListView olvSimple;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn Mandatory;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
    }
}