namespace Heiflow.Models.Hydrodynamics.Susbed
{
    partial class FlowOutViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowOutViewForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSection = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.olvMatName = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.winChartSection = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.tabPageProfile = new System.Windows.Forms.TabPage();
            this.winChart_proflie = new Heiflow.Controls.WinForm.Controls.WinChart();
            this.colorSlider1 = new Heiflow.Controls.WinForm.ColorSlider();
            this.tabControl1.SuspendLayout();
            this.tabPageSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvMatName)).BeginInit();
            this.tabPageProfile.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSection);
            this.tabControl1.Controls.Add(this.tabPageProfile);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1000, 576);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageSection
            // 
            this.tabPageSection.Controls.Add(this.splitContainer1);
            this.tabPageSection.Location = new System.Drawing.Point(4, 22);
            this.tabPageSection.Name = "tabPageSection";
            this.tabPageSection.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSection.Size = new System.Drawing.Size(992, 550);
            this.tabPageSection.TabIndex = 0;
            this.tabPageSection.Text = "断面洪水过程";
            this.tabPageSection.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.olvMatName);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.winChartSection);
            this.splitContainer1.Size = new System.Drawing.Size(986, 544);
            this.splitContainer1.SplitterDistance = 186;
            this.splitContainer1.TabIndex = 12;
            // 
            // olvMatName
            // 
            this.olvMatName.AllColumns.Add(this.olvColumn1);
            this.olvMatName.AllColumns.Add(this.olvColumn2);
            this.olvMatName.AllowColumnReorder = true;
            this.olvMatName.AllowDrop = true;
            this.olvMatName.AutoGenerateColumns = false;
            this.olvMatName.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvMatName.CellEditUseWholeCell = false;
            this.olvMatName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2});
            this.olvMatName.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMatName.DataSource = null;
            this.olvMatName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMatName.EmptyListMsg = "";
            this.olvMatName.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMatName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMatName.FullRowSelect = true;
            this.olvMatName.GridLines = true;
            this.olvMatName.GroupWithItemCountFormat = "";
            this.olvMatName.GroupWithItemCountSingularFormat = "";
            this.olvMatName.HideSelection = false;
            this.olvMatName.Location = new System.Drawing.Point(0, 0);
            this.olvMatName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvMatName.Name = "olvMatName";
            this.olvMatName.RowHeight = 20;
            this.olvMatName.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvMatName.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvMatName.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvMatName.ShowCommandMenuOnRightClick = true;
            this.olvMatName.ShowGroups = false;
            this.olvMatName.ShowImagesOnSubItems = true;
            this.olvMatName.ShowItemToolTips = true;
            this.olvMatName.Size = new System.Drawing.Size(186, 544);
            this.olvMatName.TabIndex = 5;
            this.olvMatName.UseCellFormatEvents = true;
            this.olvMatName.UseCompatibleStateImageBehavior = false;
            this.olvMatName.UseFilterIndicator = true;
            this.olvMatName.UseFiltering = true;
            this.olvMatName.UseHotItem = true;
            this.olvMatName.UseTranslucentHotItem = true;
            this.olvMatName.View = System.Windows.Forms.View.Details;
            this.olvMatName.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.olvMatName_ItemSelectionChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "ID";
            this.olvColumn1.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn1.CellEditUseWholeCell = false;
            this.olvColumn1.IsTileViewColumn = true;
            this.olvColumn1.Text = "断面序号";
            this.olvColumn1.UseInitialLetterForGroup = true;
            this.olvColumn1.Width = 81;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "Name";
            this.olvColumn2.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn2.CellEditUseWholeCell = false;
            this.olvColumn2.IsTileViewColumn = true;
            this.olvColumn2.Text = "断面编号";
            this.olvColumn2.Width = 91;
            // 
            // winChartSection
            // 
            this.winChartSection.BackColor = System.Drawing.SystemColors.Control;
            this.winChartSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChartSection.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChartSection.Location = new System.Drawing.Point(0, 0);
            this.winChartSection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChartSection.Name = "winChartSection";
            this.winChartSection.ShowStatPanel = false;
            this.winChartSection.Size = new System.Drawing.Size(796, 544);
            this.winChartSection.TabIndex = 11;
            // 
            // tabPageProfile
            // 
            this.tabPageProfile.Controls.Add(this.winChart_proflie);
            this.tabPageProfile.Controls.Add(this.colorSlider1);
            this.tabPageProfile.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfile.Name = "tabPageProfile";
            this.tabPageProfile.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfile.Size = new System.Drawing.Size(992, 550);
            this.tabPageProfile.TabIndex = 1;
            this.tabPageProfile.Text = "洪水演进过程";
            this.tabPageProfile.UseVisualStyleBackColor = true;
            // 
            // winChart_proflie
            // 
            this.winChart_proflie.BackColor = System.Drawing.SystemColors.Control;
            this.winChart_proflie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winChart_proflie.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.winChart_proflie.Location = new System.Drawing.Point(3, 3);
            this.winChart_proflie.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.winChart_proflie.Name = "winChart_proflie";
            this.winChart_proflie.ShowStatPanel = false;
            this.winChart_proflie.Size = new System.Drawing.Size(986, 521);
            this.winChart_proflie.TabIndex = 10;
            // 
            // colorSlider1
            // 
            this.colorSlider1.BackColor = System.Drawing.Color.Transparent;
            this.colorSlider1.BarInnerColor = System.Drawing.Color.GhostWhite;
            this.colorSlider1.BarOuterColor = System.Drawing.Color.White;
            this.colorSlider1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.colorSlider1.ElapsedInnerColor = System.Drawing.Color.DeepSkyBlue;
            this.colorSlider1.ElapsedOuterColor = System.Drawing.Color.White;
            this.colorSlider1.LargeChange = ((uint)(5u));
            this.colorSlider1.Location = new System.Drawing.Point(3, 524);
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.Size = new System.Drawing.Size(986, 23);
            this.colorSlider1.SmallChange = ((uint)(1u));
            this.colorSlider1.TabIndex = 9;
            this.colorSlider1.Text = "50";
            this.colorSlider1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.ThumbSize = 30;
            this.colorSlider1.Value = 0;
            this.colorSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider1_Scroll);
            // 
            // FlowOutViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 576);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FlowOutViewForm";
            this.Text = "Flow Out View";
            this.Load += new System.EventHandler(this.FlowOutViewForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageSection.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvMatName)).EndInit();
            this.tabPageProfile.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSection;
        private Controls.WinForm.Controls.WinChart winChartSection;
        private System.Windows.Forms.TabPage tabPageProfile;
        private Controls.WinForm.Controls.WinChart winChart_proflie;
        private Controls.WinForm.ColorSlider colorSlider1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BrightIdeasSoftware.DataListView olvMatName;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;

    }
}