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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetSpeciesForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.olvMobileSpeciesList = new BrightIdeasSoftware.DataListView();
            this.colName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colSelected = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.olvExchangeSpeciesList = new BrightIdeasSoftware.DataListView();
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.olvMineralSpeciesList = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvMobileSpeciesList)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvExchangeSpeciesList)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvMineralSpeciesList)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(568, 12);
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
            this.btnOk.Location = new System.Drawing.Point(454, 12);
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
            this.panel1.Location = new System.Drawing.Point(0, 631);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 59);
            this.panel1.TabIndex = 34;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(685, 631);
            this.tabControl1.TabIndex = 35;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.olvMobileSpeciesList);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(677, 598);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mobile Species";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // olvMobileSpeciesList
            // 
            this.olvMobileSpeciesList.AllColumns.Add(this.colName);
            this.olvMobileSpeciesList.AllColumns.Add(this.colSelected);
            this.olvMobileSpeciesList.AllColumns.Add(this.olvColumn2);
            this.olvMobileSpeciesList.AllowColumnReorder = true;
            this.olvMobileSpeciesList.AllowDrop = true;
            this.olvMobileSpeciesList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMobileSpeciesList.CellEditUseWholeCell = false;
            this.olvMobileSpeciesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSelected,
            this.olvColumn2});
            this.olvMobileSpeciesList.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMobileSpeciesList.DataSource = null;
            this.olvMobileSpeciesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMobileSpeciesList.EmptyListMsg = "";
            this.olvMobileSpeciesList.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMobileSpeciesList.FullRowSelect = true;
            this.olvMobileSpeciesList.GridLines = true;
            this.olvMobileSpeciesList.GroupWithItemCountFormat = "";
            this.olvMobileSpeciesList.GroupWithItemCountSingularFormat = "";
            this.olvMobileSpeciesList.HideSelection = false;
            this.olvMobileSpeciesList.Location = new System.Drawing.Point(3, 4);
            this.olvMobileSpeciesList.Margin = new System.Windows.Forms.Padding(4);
            this.olvMobileSpeciesList.Name = "olvMobileSpeciesList";
            this.olvMobileSpeciesList.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMobileSpeciesList.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvMobileSpeciesList.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvMobileSpeciesList.ShowCommandMenuOnRightClick = true;
            this.olvMobileSpeciesList.ShowGroups = false;
            this.olvMobileSpeciesList.ShowImagesOnSubItems = true;
            this.olvMobileSpeciesList.ShowItemToolTips = true;
            this.olvMobileSpeciesList.Size = new System.Drawing.Size(671, 590);
            this.olvMobileSpeciesList.TabIndex = 21;
            this.olvMobileSpeciesList.UseCellFormatEvents = true;
            this.olvMobileSpeciesList.UseCompatibleStateImageBehavior = false;
            this.olvMobileSpeciesList.UseFilterIndicator = true;
            this.olvMobileSpeciesList.UseFiltering = true;
            this.olvMobileSpeciesList.UseHotItem = true;
            this.olvMobileSpeciesList.UseTranslucentHotItem = true;
            this.olvMobileSpeciesList.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.AspectName = "Name";
            this.colName.ButtonPadding = new System.Drawing.Size(10, 10);
            this.colName.CellEditUseWholeCell = true;
            this.colName.IsTileViewColumn = true;
            this.colName.Text = "Species Name";
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
            this.olvColumn2.Width = 208;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.olvExchangeSpeciesList);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(677, 598);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Exchange Species";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // olvExchangeSpeciesList
            // 
            this.olvExchangeSpeciesList.AllColumns.Add(this.olvColumn5);
            this.olvExchangeSpeciesList.AllColumns.Add(this.olvColumn6);
            this.olvExchangeSpeciesList.AllColumns.Add(this.olvColumn8);
            this.olvExchangeSpeciesList.AllColumns.Add(this.olvColumn7);
            this.olvExchangeSpeciesList.AllowColumnReorder = true;
            this.olvExchangeSpeciesList.AllowDrop = true;
            this.olvExchangeSpeciesList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvExchangeSpeciesList.CellEditUseWholeCell = false;
            this.olvExchangeSpeciesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn8,
            this.olvColumn7});
            this.olvExchangeSpeciesList.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvExchangeSpeciesList.DataSource = null;
            this.olvExchangeSpeciesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvExchangeSpeciesList.EmptyListMsg = "";
            this.olvExchangeSpeciesList.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvExchangeSpeciesList.FullRowSelect = true;
            this.olvExchangeSpeciesList.GridLines = true;
            this.olvExchangeSpeciesList.GroupWithItemCountFormat = "";
            this.olvExchangeSpeciesList.GroupWithItemCountSingularFormat = "";
            this.olvExchangeSpeciesList.HideSelection = false;
            this.olvExchangeSpeciesList.Location = new System.Drawing.Point(3, 3);
            this.olvExchangeSpeciesList.Margin = new System.Windows.Forms.Padding(4);
            this.olvExchangeSpeciesList.Name = "olvExchangeSpeciesList";
            this.olvExchangeSpeciesList.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvExchangeSpeciesList.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvExchangeSpeciesList.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvExchangeSpeciesList.ShowCommandMenuOnRightClick = true;
            this.olvExchangeSpeciesList.ShowGroups = false;
            this.olvExchangeSpeciesList.ShowImagesOnSubItems = true;
            this.olvExchangeSpeciesList.ShowItemToolTips = true;
            this.olvExchangeSpeciesList.Size = new System.Drawing.Size(671, 592);
            this.olvExchangeSpeciesList.TabIndex = 22;
            this.olvExchangeSpeciesList.UseCellFormatEvents = true;
            this.olvExchangeSpeciesList.UseCompatibleStateImageBehavior = false;
            this.olvExchangeSpeciesList.UseFilterIndicator = true;
            this.olvExchangeSpeciesList.UseFiltering = true;
            this.olvExchangeSpeciesList.UseHotItem = true;
            this.olvExchangeSpeciesList.UseTranslucentHotItem = true;
            this.olvExchangeSpeciesList.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Name";
            this.olvColumn5.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn5.CellEditUseWholeCell = true;
            this.olvColumn5.IsTileViewColumn = true;
            this.olvColumn5.Text = "Species Name";
            this.olvColumn5.UseInitialLetterForGroup = true;
            this.olvColumn5.Width = 115;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Selected";
            this.olvColumn6.CellEditUseWholeCell = true;
            this.olvColumn6.CheckBoxes = true;
            this.olvColumn6.Text = "Selected";
            this.olvColumn6.ToolTipText = "Selected";
            this.olvColumn6.Width = 128;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "LonNum";
            this.olvColumn8.Text = "Ion Number";
            this.olvColumn8.Width = 104;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "InitialConcentration";
            this.olvColumn7.Text = "Initial Concentration (mol/L)";
            this.olvColumn7.Width = 400;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.olvMineralSpeciesList);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(677, 598);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mineral Species";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // olvMineralSpeciesList
            // 
            this.olvMineralSpeciesList.AllColumns.Add(this.olvColumn1);
            this.olvMineralSpeciesList.AllColumns.Add(this.olvColumn3);
            this.olvMineralSpeciesList.AllColumns.Add(this.olvColumn4);
            this.olvMineralSpeciesList.AllowColumnReorder = true;
            this.olvMineralSpeciesList.AllowDrop = true;
            this.olvMineralSpeciesList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMineralSpeciesList.CellEditUseWholeCell = false;
            this.olvMineralSpeciesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn3,
            this.olvColumn4});
            this.olvMineralSpeciesList.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMineralSpeciesList.DataSource = null;
            this.olvMineralSpeciesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMineralSpeciesList.EmptyListMsg = "";
            this.olvMineralSpeciesList.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMineralSpeciesList.FullRowSelect = true;
            this.olvMineralSpeciesList.GridLines = true;
            this.olvMineralSpeciesList.GroupWithItemCountFormat = "";
            this.olvMineralSpeciesList.GroupWithItemCountSingularFormat = "";
            this.olvMineralSpeciesList.HideSelection = false;
            this.olvMineralSpeciesList.Location = new System.Drawing.Point(3, 3);
            this.olvMineralSpeciesList.Margin = new System.Windows.Forms.Padding(4);
            this.olvMineralSpeciesList.Name = "olvMineralSpeciesList";
            this.olvMineralSpeciesList.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMineralSpeciesList.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvMineralSpeciesList.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvMineralSpeciesList.ShowCommandMenuOnRightClick = true;
            this.olvMineralSpeciesList.ShowGroups = false;
            this.olvMineralSpeciesList.ShowImagesOnSubItems = true;
            this.olvMineralSpeciesList.ShowItemToolTips = true;
            this.olvMineralSpeciesList.Size = new System.Drawing.Size(671, 592);
            this.olvMineralSpeciesList.TabIndex = 22;
            this.olvMineralSpeciesList.UseCellFormatEvents = true;
            this.olvMineralSpeciesList.UseCompatibleStateImageBehavior = false;
            this.olvMineralSpeciesList.UseFilterIndicator = true;
            this.olvMineralSpeciesList.UseFiltering = true;
            this.olvMineralSpeciesList.UseHotItem = true;
            this.olvMineralSpeciesList.UseTranslucentHotItem = true;
            this.olvMineralSpeciesList.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn1.CellEditUseWholeCell = true;
            this.olvColumn1.IsTileViewColumn = true;
            this.olvColumn1.Text = "Species Name";
            this.olvColumn1.UseInitialLetterForGroup = true;
            this.olvColumn1.Width = 115;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Selected";
            this.olvColumn3.CellEditUseWholeCell = true;
            this.olvColumn3.CheckBoxes = true;
            this.olvColumn3.Text = "Selected";
            this.olvColumn3.ToolTipText = "Selected";
            this.olvColumn3.Width = 128;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "InitialConcentration";
            this.olvColumn4.Text = "Initial Concentration (mol/L)";
            this.olvColumn4.Width = 236;
            // 
            // SetSpeciesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 690);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SetSpeciesForm";
            this.Text = "Set Species";
            this.Load += new System.EventHandler(this.SetSpeciesForm_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvMobileSpeciesList)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvExchangeSpeciesList)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvMineralSpeciesList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private BrightIdeasSoftware.DataListView olvMobileSpeciesList;
        private BrightIdeasSoftware.OLVColumn colName;
        private BrightIdeasSoftware.OLVColumn colSelected;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private System.Windows.Forms.TabPage tabPage2;
        private BrightIdeasSoftware.DataListView olvMineralSpeciesList;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private System.Windows.Forms.TabPage tabPage3;
        private BrightIdeasSoftware.DataListView olvExchangeSpeciesList;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
    }
}