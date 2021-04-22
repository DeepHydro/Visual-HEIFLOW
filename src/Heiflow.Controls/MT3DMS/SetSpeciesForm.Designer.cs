namespace Heiflow.Controls.WinForm.MT3DMS
{
    partial class SetSpeciesForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.olvLayerGroup = new BrightIdeasSoftware.DataListView();
            this.colName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colSelected = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvLayerGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(689, 12);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOk.Location = new System.Drawing.Point(575, 12);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 30);
            this.btnOk.TabIndex = 31;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 376);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(806, 59);
            this.panel1.TabIndex = 34;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(806, 376);
            this.tabControl1.TabIndex = 35;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.olvLayerGroup);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(798, 343);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Species List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // olvLayerGroup
            // 
            this.olvLayerGroup.AllColumns.Add(this.colName);
            this.olvLayerGroup.AllColumns.Add(this.colSelected);
            this.olvLayerGroup.AllColumns.Add(this.olvColumn2);
            this.olvLayerGroup.AllowColumnReorder = true;
            this.olvLayerGroup.AllowDrop = true;
            this.olvLayerGroup.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvLayerGroup.CellEditUseWholeCell = false;
            this.olvLayerGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSelected,
            this.olvColumn2});
            this.olvLayerGroup.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvLayerGroup.DataSource = null;
            this.olvLayerGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvLayerGroup.EmptyListMsg = "";
            this.olvLayerGroup.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvLayerGroup.FullRowSelect = true;
            this.olvLayerGroup.GridLines = true;
            this.olvLayerGroup.GroupWithItemCountFormat = "";
            this.olvLayerGroup.GroupWithItemCountSingularFormat = "";
            this.olvLayerGroup.HideSelection = false;
            this.olvLayerGroup.Location = new System.Drawing.Point(3, 4);
            this.olvLayerGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.olvLayerGroup.Name = "olvLayerGroup";
            this.olvLayerGroup.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvLayerGroup.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvLayerGroup.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvLayerGroup.ShowCommandMenuOnRightClick = true;
            this.olvLayerGroup.ShowGroups = false;
            this.olvLayerGroup.ShowImagesOnSubItems = true;
            this.olvLayerGroup.ShowItemToolTips = true;
            this.olvLayerGroup.Size = new System.Drawing.Size(792, 335);
            this.olvLayerGroup.TabIndex = 21;
            this.olvLayerGroup.UseCellFormatEvents = true;
            this.olvLayerGroup.UseCompatibleStateImageBehavior = false;
            this.olvLayerGroup.UseFilterIndicator = true;
            this.olvLayerGroup.UseFiltering = true;
            this.olvLayerGroup.UseHotItem = true;
            this.olvLayerGroup.UseTranslucentHotItem = true;
            this.olvLayerGroup.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.AspectName = "Name";
            this.colName.ButtonPadding = new System.Drawing.Size(10, 10);
            this.colName.CellEditUseWholeCell = true;
            this.colName.IsTileViewColumn = true;
            this.colName.Text = "Layer Name";
            this.colName.UseInitialLetterForGroup = true;
            this.colName.Width = 115;
            // 
            // colSelected
            // 
            this.colSelected.AspectName = "Selected";
            this.colSelected.CellEditUseWholeCell = true;
            this.colSelected.CheckBoxes = true;
            this.colSelected.Text = "Selected";
            this.colSelected.ToolTipText = "Selected";
            this.colSelected.Width = 128;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "InitialConcentration";
            this.olvColumn2.Text = "Initial Concentration (mol/L)";
            this.olvColumn2.Width = 400;
            // 
            // SetSpeciesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 435);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SetSpeciesForm";
            this.Text = "Set Species";
            this.Load += new System.EventHandler(this.SetSpeciesForm_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvLayerGroup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private BrightIdeasSoftware.DataListView olvLayerGroup;
        private BrightIdeasSoftware.OLVColumn colName;
        private BrightIdeasSoftware.OLVColumn colSelected;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
    }
}