namespace Heiflow.Controls.Options
{
    partial class OptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmbVersion = new System.Windows.Forms.ToolStripComboBox();
            this.btnLoadDefaultParaMeta = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnImport = new System.Windows.Forms.ToolStripDropDownButton();
            this.importFromXML = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromParameterFile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCompare = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.cmbExportSource = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.export2Csv = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsXml = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsParameterFile = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(248, 586);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(492, 586);
            this.panel1.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cmbVersion,
            this.btnLoadDefaultParaMeta,
            this.btnSave,
            this.toolStripSeparator1,
            this.btnImport,
            this.btnCompare,
            this.toolStripLabel2,
            this.cmbExportSource,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(744, 28);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(57, 25);
            this.toolStripLabel1.Text = "Version";
            // 
            // cmbVersion
            // 
            this.cmbVersion.Name = "cmbVersion";
            this.cmbVersion.Size = new System.Drawing.Size(121, 28);
            this.cmbVersion.SelectedIndexChanged += new System.EventHandler(this.cmbVersion_SelectedIndexChanged);
            // 
            // btnLoadDefaultParaMeta
            // 
            this.btnLoadDefaultParaMeta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoadDefaultParaMeta.Image = global::Heiflow.Controls.WinForm.Properties.Resources.Load24;
            this.btnLoadDefaultParaMeta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadDefaultParaMeta.Name = "btnLoadDefaultParaMeta";
            this.btnLoadDefaultParaMeta.Size = new System.Drawing.Size(24, 25);
            this.btnLoadDefaultParaMeta.ToolTipText = "load defualt parameter meta file";
            this.btnLoadDefaultParaMeta.Click += new System.EventHandler(this.btnLoadDefaultParaMeta_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericSave_B_16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(24, 25);
            this.btnSave.Text = "toolStripButton1";
            this.btnSave.ToolTipText = "Save defualt parameter meta file";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFromXML,
            this.importFromParameterFile});
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GeodatabaseXMLRecordSetImport32;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(34, 25);
            this.btnImport.Text = "Import from a parameter file";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // importFromXML
            // 
            this.importFromXML.Name = "importFromXML";
            this.importFromXML.Size = new System.Drawing.Size(263, 26);
            this.importFromXML.Text = "Import from XML file";
            this.importFromXML.Click += new System.EventHandler(this.importFromXML_Click);
            // 
            // importFromParameterFile
            // 
            this.importFromParameterFile.Name = "importFromParameterFile";
            this.importFromParameterFile.Size = new System.Drawing.Size(263, 26);
            this.importFromParameterFile.Text = "Import from parameter file";
            this.importFromParameterFile.Click += new System.EventHandler(this.importFromParameterFile_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompare.Image = global::Heiflow.Controls.WinForm.Properties.Resources.compare_24p;
            this.btnCompare.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(24, 25);
            this.btnCompare.Text = "Compare imported parameter with default one";
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(101, 25);
            this.toolStripLabel2.Text = "Export Source";
            // 
            // cmbExportSource
            // 
            this.cmbExportSource.Items.AddRange(new object[] {
            "Default",
            "Imported"});
            this.cmbExportSource.Name = "cmbExportSource";
            this.cmbExportSource.Size = new System.Drawing.Size(121, 28);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.export2Csv,
            this.exportAsXml,
            this.exportAsParameterFile});
            this.toolStripDropDownButton1.Image = global::Heiflow.Controls.WinForm.Properties.Resources.GenericOpen_B_32;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 25);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // export2Csv
            // 
            this.export2Csv.Name = "export2Csv";
            this.export2Csv.Size = new System.Drawing.Size(243, 26);
            this.export2Csv.Text = "Export as csv file";
            this.export2Csv.Click += new System.EventHandler(this.export2Csv_Click);
            // 
            // exportAsXml
            // 
            this.exportAsXml.Name = "exportAsXml";
            this.exportAsXml.Size = new System.Drawing.Size(243, 26);
            this.exportAsXml.Text = "Export as XML file";
            this.exportAsXml.Click += new System.EventHandler(this.exportAsXml_Click);
            // 
            // exportAsParameterFile
            // 
            this.exportAsParameterFile.Name = "exportAsParameterFile";
            this.exportAsParameterFile.Size = new System.Drawing.Size(243, 26);
            this.exportAsParameterFile.Text = "Export as parameter file";
            this.exportAsParameterFile.Click += new System.EventHandler(this.exportAsParameterFile_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(744, 586);
            this.splitContainer1.SplitterDistance = 248;
            this.splitContainer1.TabIndex = 7;
            // 
            // OptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(744, 614);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "OptionForm";
            this.Text = " Options";
            this.Load += new System.EventHandler(this.OptionForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem export2Csv;
        private System.Windows.Forms.ToolStripMenuItem exportAsXml;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton btnLoadDefaultParaMeta;
        private System.Windows.Forms.ToolStripDropDownButton btnImport;
        private System.Windows.Forms.ToolStripMenuItem importFromXML;
        private System.Windows.Forms.ToolStripMenuItem importFromParameterFile;
        private System.Windows.Forms.ToolStripMenuItem exportAsParameterFile;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cmbVersion;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox cmbExportSource;
        private System.Windows.Forms.ToolStripButton btnCompare;
    }
}