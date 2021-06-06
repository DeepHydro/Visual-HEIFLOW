namespace Heiflow.Controls.WinForm.Display
{
    partial class CoverageSetup
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoverageSetup));
            this.chbProp = new System.Windows.Forms.CheckedListBox();
            this.contextMenuStripAreal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tbCoverageName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCreateLookupTable = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMap = new System.Windows.Forms.ToolStripButton();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnUseDefault = new System.Windows.Forms.ToolStripButton();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.labelStatus = new System.Windows.Forms.ToolStripLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridEx1 = new Heiflow.Controls.WinForm.Controls.DataCubeGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlLeft = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbMapLayers = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbGridLayer = new System.Windows.Forms.ComboBox();
            this.cmbPackages = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveAsCsv = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStripAreal.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlLeft.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chbProp
            // 
            this.chbProp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chbProp.ContextMenuStrip = this.contextMenuStripAreal;
            this.chbProp.FormattingEnabled = true;
            this.chbProp.Location = new System.Drawing.Point(7, 263);
            this.chbProp.Name = "chbProp";
            this.chbProp.Size = new System.Drawing.Size(243, 290);
            this.chbProp.TabIndex = 6;
            // 
            // contextMenuStripAreal
            // 
            this.contextMenuStripAreal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripAreal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.contextMenuStripAreal.Name = "contextMenuStripAreal";
            this.contextMenuStripAreal.Size = new System.Drawing.Size(144, 52);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(143, 24);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 238);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "* Areal Properties";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.tbCoverageName,
            this.toolStripSeparator1,
            this.btnCreateLookupTable,
            this.btnUseDefault,
            this.btnMap,
            this.toolStripSeparator2,
            this.btnSave,
            this.btnImport,
            this.btnSaveAsCsv,
            this.toolStripProgressBar1,
            this.labelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1066, 27);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(116, 24);
            this.toolStripLabel2.Text = "Coverage Name";
            // 
            // tbCoverageName
            // 
            this.tbCoverageName.Name = "tbCoverageName";
            this.tbCoverageName.Size = new System.Drawing.Size(178, 27);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnCreateLookupTable
            // 
            this.btnCreateLookupTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCreateLookupTable.Image = global::Heiflow.Controls.WinForm.Properties.Resources.begin;
            this.btnCreateLookupTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreateLookupTable.Name = "btnCreateLookupTable";
            this.btnCreateLookupTable.Size = new System.Drawing.Size(24, 24);
            this.btnCreateLookupTable.Text = "toolStripButton1";
            this.btnCreateLookupTable.ToolTipText = "Create lookup table";
            this.btnCreateLookupTable.Click += new System.EventHandler(this.btnCreateLookupTable_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericSave_B_16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 24);
            this.btnSave.ToolTipText = "Save lookup table";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnMap
            // 
            this.btnMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMap.Image = global::Heiflow.Controls.WinForm.Properties.Resources.ReportRun16;
            this.btnMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMap.Name = "btnMap";
            this.btnMap.Size = new System.Drawing.Size(24, 24);
            this.btnMap.Text = "toolStripButton1";
            this.btnMap.ToolTipText = "Run parameterization";
            this.btnMap.Click += new System.EventHandler(this.btnMap_Click);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GeodatabaseXMLRecordSetImport32;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(24, 24);
            this.btnImport.ToolTipText = "Import a lookup table from a csv file";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnUseDefault
            // 
            this.btnUseDefault.Checked = true;
            this.btnUseDefault.CheckOnClick = true;
            this.btnUseDefault.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnUseDefault.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUseDefault.Image = global::Heiflow.Controls.WinForm.Properties.Resources.defaultv;
            this.btnUseDefault.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUseDefault.Name = "btnUseDefault";
            this.btnUseDefault.Size = new System.Drawing.Size(24, 24);
            this.btnUseDefault.Text = "Use default value";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(178, 24);
            this.toolStripProgressBar1.Visible = false;
            // 
            // labelStatus
            // 
            this.labelStatus.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(50, 24);
            this.labelStatus.Text = "Ready";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(796, 591);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridEx1);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(788, 558);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lookup Table";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridEx1
            // 
            this.dataGridEx1.DataObjectName = "";
            this.dataGridEx1.DataTable = null;
            this.dataGridEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridEx1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridEx1.Location = new System.Drawing.Point(3, 3);
            this.dataGridEx1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridEx1.Name = "dataGridEx1";
            this.dataGridEx1.ShowImport = false;
            this.dataGridEx1.ShowSave2Excel = false;
            this.dataGridEx1.ShowSaveButton = false;
            this.dataGridEx1.Size = new System.Drawing.Size(782, 552);
            this.dataGridEx1.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControlLeft);
            this.splitContainer1.Panel1.Font = new System.Drawing.Font("Calibri", 10.5F);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1066, 591);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 16;
            // 
            // tabControlLeft
            // 
            this.tabControlLeft.Controls.Add(this.tabPage1);
            this.tabControlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLeft.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControlLeft.Location = new System.Drawing.Point(0, 0);
            this.tabControlLeft.Name = "tabControlLeft";
            this.tabControlLeft.SelectedIndex = 0;
            this.tabControlLeft.Size = new System.Drawing.Size(266, 591);
            this.tabControlLeft.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chbProp);
            this.tabPage1.Controls.Add(this.cmbFields);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.cmbMapLayers);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.cmbGridLayer);
            this.tabPage1.Controls.Add(this.cmbPackages);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(258, 557);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Setting";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cmbFields
            // 
            this.cmbFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFields.FormattingEnabled = true;
            this.cmbFields.Location = new System.Drawing.Point(7, 91);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(243, 28);
            this.cmbFields.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "*Grid Layer";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "* Zone Field";
            // 
            // cmbMapLayers
            // 
            this.cmbMapLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMapLayers.FormattingEnabled = true;
            this.cmbMapLayers.Location = new System.Drawing.Point(7, 34);
            this.cmbMapLayers.Name = "cmbMapLayers";
            this.cmbMapLayers.Size = new System.Drawing.Size(243, 28);
            this.cmbMapLayers.TabIndex = 1;
            this.cmbMapLayers.SelectedIndexChanged += new System.EventHandler(this.cmbMapLayers_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "* Map Layer";
            // 
            // cmbGridLayer
            // 
            this.cmbGridLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGridLayer.FormattingEnabled = true;
            this.cmbGridLayer.Location = new System.Drawing.Point(7, 206);
            this.cmbGridLayer.Name = "cmbGridLayer";
            this.cmbGridLayer.Size = new System.Drawing.Size(243, 28);
            this.cmbGridLayer.TabIndex = 1;
            this.cmbGridLayer.SelectedIndexChanged += new System.EventHandler(this.cmbGridLayer_SelectedIndexChanged);
            // 
            // cmbPackages
            // 
            this.cmbPackages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPackages.FormattingEnabled = true;
            this.cmbPackages.Location = new System.Drawing.Point(7, 149);
            this.cmbPackages.Name = "cmbPackages";
            this.cmbPackages.Size = new System.Drawing.Size(243, 28);
            this.cmbPackages.TabIndex = 1;
            this.cmbPackages.SelectedIndexChanged += new System.EventHandler(this.cmbPackages_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "* Packages";
            // 
            // btnSaveAsCsv
            // 
            this.btnSaveAsCsv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveAsCsv.Image = global::Heiflow.Controls.WinForm.Properties.Resources.excel_32;
            this.btnSaveAsCsv.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveAsCsv.Name = "btnSaveAsCsv";
            this.btnSaveAsCsv.Size = new System.Drawing.Size(24, 24);
            this.btnSaveAsCsv.Text = "Save lookup table as CSV file";
            this.btnSaveAsCsv.Click += new System.EventHandler(this.btnSaveAsCsv_Click);
            // 
            // CoverageSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1066, 618);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "CoverageSetup";
            this.ShowInTaskbar = false;
            this.Text = "Coverage Setup";
            this.Load += new System.EventHandler(this.CoverageSetup_Load);
            this.contextMenuStripAreal.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlLeft.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chbProp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cmbPackages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripTextBox tbCoverageName;
        private Controls.DataCubeGrid dataGridEx1;
        private System.Windows.Forms.TabControl tabControlLeft;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbMapLayers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripButton btnMap;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCreateLookupTable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbGridLayer;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAreal;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel labelStatus;
        private System.Windows.Forms.ToolStripButton btnUseDefault;
        private System.Windows.Forms.ToolStripButton btnSaveAsCsv;
    }
}