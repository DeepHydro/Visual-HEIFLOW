namespace Heiflow.Image
{
    partial class RecgnizationForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecgnizationForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recognizeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnOpenTrainingImages = new System.Windows.Forms.ToolStripDropDownButton();
            this.openTraningImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openImagesToBeRecognizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmb_Models = new System.Windows.Forms.ToolStripComboBox();
            this.btnTrain = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRecognize = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.imageSourceManager1 = new Heiflow.Image.Controls.ImageSourceManager();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pb_recg2 = new System.Windows.Forms.PictureBox();
            this.pb_recg1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pb_train2 = new System.Windows.Forms.PictureBox();
            this.pb_train1 = new System.Windows.Forms.PictureBox();
            this.webMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_recg2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_recg1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_train2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_train1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modeToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(994, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProjToolStripMenuItem,
            this.saveProjToolStripMenuItem,
            this.webMapToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.modeToolStripMenuItem.Text = "File";
            // 
            // openProjToolStripMenuItem
            // 
            this.openProjToolStripMenuItem.Name = "openProjToolStripMenuItem";
            this.openProjToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.openProjToolStripMenuItem.Text = "Open Image Project File...";
            this.openProjToolStripMenuItem.Click += new System.EventHandler(this.openProjToolStripMenuItem_Click);
            // 
            // saveProjToolStripMenuItem
            // 
            this.saveProjToolStripMenuItem.Name = "saveProjToolStripMenuItem";
            this.saveProjToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.saveProjToolStripMenuItem.Text = "Save Image Project File...";
            this.saveProjToolStripMenuItem.Click += new System.EventHandler(this.saveProjToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trainToolStripMenuItem,
            this.recognizeToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
            this.helpToolStripMenuItem.Text = "Run";
            // 
            // trainToolStripMenuItem
            // 
            this.trainToolStripMenuItem.Name = "trainToolStripMenuItem";
            this.trainToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.trainToolStripMenuItem.Text = "Train";
            // 
            // recognizeToolStripMenuItem1
            // 
            this.recognizeToolStripMenuItem1.Name = "recognizeToolStripMenuItem1";
            this.recognizeToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
            this.recognizeToolStripMenuItem1.Text = "Recognize";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpenTrainingImages,
            this.toolStripLabel1,
            this.cmb_Models,
            this.btnTrain,
            this.toolStripSeparator1,
            this.btnRecognize});
            this.toolStrip2.Location = new System.Drawing.Point(0, 25);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(994, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnOpenTrainingImages
            // 
            this.btnOpenTrainingImages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenTrainingImages.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTraningImagesToolStripMenuItem,
            this.openImagesToBeRecognizeToolStripMenuItem});
            this.btnOpenTrainingImages.Image = global::Heiflow.Image.Properties.Resources.Apps_image_viewer_24;
            this.btnOpenTrainingImages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenTrainingImages.Name = "btnOpenTrainingImages";
            this.btnOpenTrainingImages.Size = new System.Drawing.Size(29, 22);
            this.btnOpenTrainingImages.Text = "Open Training Images";
            // 
            // openTraningImagesToolStripMenuItem
            // 
            this.openTraningImagesToolStripMenuItem.Name = "openTraningImagesToolStripMenuItem";
            this.openTraningImagesToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.openTraningImagesToolStripMenuItem.Text = "Open Traning Images";
            this.openTraningImagesToolStripMenuItem.Click += new System.EventHandler(this.openTraningImagesToolStripMenuItem_Click);
            // 
            // openImagesToBeRecognizeToolStripMenuItem
            // 
            this.openImagesToBeRecognizeToolStripMenuItem.Name = "openImagesToBeRecognizeToolStripMenuItem";
            this.openImagesToBeRecognizeToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.openImagesToBeRecognizeToolStripMenuItem.Text = "Open Images to Be Recognized";
            this.openImagesToBeRecognizeToolStripMenuItem.Click += new System.EventHandler(this.openImagesToBeRecognizeToolStripMenuItem_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel1.Text = "Model";
            // 
            // cmb_Models
            // 
            this.cmb_Models.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Models.Items.AddRange(new object[] {
            "SVM",
            "ANN"});
            this.cmb_Models.Name = "cmb_Models";
            this.cmb_Models.Size = new System.Drawing.Size(121, 25);
            this.cmb_Models.ToolTipText = "Select a model";
            this.cmb_Models.SelectedIndexChanged += new System.EventHandler(this.cmb_Models_SelectedIndexChanged);
            // 
            // btnTrain
            // 
            this.btnTrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTrain.Image = global::Heiflow.Image.Properties.Resources.training_24;
            this.btnTrain.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(23, 22);
            this.btnTrain.Text = "toolStripButton2";
            this.btnTrain.ToolTipText = "Train";
            this.btnTrain.Click += new System.EventHandler(this.btnTrain_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRecognize
            // 
            this.btnRecognize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRecognize.Image = global::Heiflow.Image.Properties.Resources.Recognition_24;
            this.btnRecognize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRecognize.Name = "btnRecognize";
            this.btnRecognize.Size = new System.Drawing.Size(23, 22);
            this.btnRecognize.Text = "Recognize";
            this.btnRecognize.Click += new System.EventHandler(this.btnRecognize_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 695);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(994, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(44, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 50);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(994, 645);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(250, 645);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.imageSourceManager1);
            this.tabPage1.Controls.Add(this.treeView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(242, 617);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Images";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // imageSourceManager1
            // 
            this.imageSourceManager1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageSourceManager1.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.imageSourceManager1.ImageSource = null;
            this.imageSourceManager1.Location = new System.Drawing.Point(3, 3);
            this.imageSourceManager1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageSourceManager1.Name = "imageSourceManager1";
            this.imageSourceManager1.Size = new System.Drawing.Size(236, 611);
            this.imageSourceManager1.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(236, 611);
            this.treeView1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.propertyGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(242, 619);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Property";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(236, 613);
            this.propertyGrid1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pb_recg2);
            this.groupBox2.Controls.Add(this.pb_recg1);
            this.groupBox2.Location = new System.Drawing.Point(13, 327);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(711, 301);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Recgonization";
            // 
            // pb_recg2
            // 
            this.pb_recg2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_recg2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_recg2.Location = new System.Drawing.Point(369, 32);
            this.pb_recg2.Name = "pb_recg2";
            this.pb_recg2.Size = new System.Drawing.Size(331, 239);
            this.pb_recg2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_recg2.TabIndex = 3;
            this.pb_recg2.TabStop = false;
            // 
            // pb_recg1
            // 
            this.pb_recg1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_recg1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_recg1.Location = new System.Drawing.Point(20, 32);
            this.pb_recg1.Name = "pb_recg1";
            this.pb_recg1.Size = new System.Drawing.Size(332, 239);
            this.pb_recg1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_recg1.TabIndex = 2;
            this.pb_recg1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pb_train2);
            this.groupBox1.Controls.Add(this.pb_train1);
            this.groupBox1.Location = new System.Drawing.Point(13, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(711, 304);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Training";
            // 
            // pb_train2
            // 
            this.pb_train2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_train2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_train2.Location = new System.Drawing.Point(369, 41);
            this.pb_train2.Name = "pb_train2";
            this.pb_train2.Size = new System.Drawing.Size(331, 233);
            this.pb_train2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_train2.TabIndex = 1;
            this.pb_train2.TabStop = false;
            // 
            // pb_train1
            // 
            this.pb_train1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_train1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_train1.Location = new System.Drawing.Point(20, 41);
            this.pb_train1.Name = "pb_train1";
            this.pb_train1.Size = new System.Drawing.Size(332, 233);
            this.pb_train1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_train1.TabIndex = 0;
            this.pb_train1.TabStop = false;
            // 
            // webMapToolStripMenuItem
            // 
            this.webMapToolStripMenuItem.Name = "webMapToolStripMenuItem";
            this.webMapToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.webMapToolStripMenuItem.Text = "Web Map";
            this.webMapToolStripMenuItem.Click += new System.EventHandler(this.webMapToolStripMenuItem_Click);
            // 
            // RecgnizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 717);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Calibri", 9.5F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RecgnizationForm";
            this.Text = "Image Recognization of Desert Vegetation";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_recg2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_recg1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_train2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_train1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cmb_Models;
        private System.Windows.Forms.ToolStripButton btnTrain;
        private System.Windows.Forms.ToolStripButton btnRecognize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pb_train1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pb_train2;
        private System.Windows.Forms.ToolStripDropDownButton btnOpenTrainingImages;
        private System.Windows.Forms.ToolStripMenuItem openTraningImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openImagesToBeRecognizeToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.PictureBox pb_recg2;
        private System.Windows.Forms.PictureBox pb_recg1;
        private System.Windows.Forms.ToolStripMenuItem trainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recognizeToolStripMenuItem1;
        private Controls.ImageSourceManager imageSourceManager1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripMenuItem webMapToolStripMenuItem;
    }
}

