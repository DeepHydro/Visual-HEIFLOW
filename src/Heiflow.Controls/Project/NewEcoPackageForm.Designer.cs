namespace Heiflow.Controls.WinForm.Project
{
    partial class NewEcoPackageForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("GHM", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("CropSPAC", 1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("DVSim", 3);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Euphratica ", 2);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewEcoPackageForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tbModelDes = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(740, 463);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 45);
            this.btnCancel.TabIndex = 33;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(582, 463);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(127, 45);
            this.btnOK.TabIndex = 34;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // tbModelDes
            // 
            this.tbModelDes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbModelDes.Location = new System.Drawing.Point(654, 64);
            this.tbModelDes.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tbModelDes.Multiline = true;
            this.tbModelDes.Name = "tbModelDes";
            this.tbModelDes.Size = new System.Drawing.Size(212, 389);
            this.tbModelDes.TabIndex = 32;
            // 
            // listView1
            // 
            listViewItem1.Checked = true;
            listViewItem1.StateImageIndex = 1;
            listViewItem1.Tag = "GHM is developed by Dr. Sun Zan at Peking University. It provides generic modelin" +
    "g capabilities to simulate vegetation growth.";
            listViewItem2.Tag = "CropSPAC package is developed by Prof. Mao Xiaoming at China Agricultural Univers" +
    "ity. It enables to simulate crop (e.g., Maize, Wheat) growth and productivity.";
            listViewItem3.Tag = "DVSim package is developed by Prof. Huang Yongmei at Beijing Normal University It" +
    " is specially developed for simulating scatted desert vegetation.";
            listViewItem4.Tag = "Euphratica package is developed by Dr. Li Xi at Peking University It is specially" +
    " developed for simulating Euphratica.";
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(16, 64);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(630, 389);
            this.listView1.TabIndex = 36;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ghm.png");
            this.imageList1.Images.SetKeyName(1, "crop.png");
            this.imageList1.Images.SetKeyName(2, "hy.png");
            this.imageList1.Images.SetKeyName(3, "dv.png");
            // 
            // button1
            // 
            this.button1.Image = global::Heiflow.Controls.WinForm.Properties.Resources.AddContent16;
            this.button1.Location = new System.Drawing.Point(16, 21);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 34);
            this.button1.TabIndex = 37;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Image = global::Heiflow.Controls.WinForm.Properties.Resources.ViewList16;
            this.button2.Location = new System.Drawing.Point(75, 21);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(51, 34);
            this.button2.TabIndex = 37;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // NewEcoPackageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 516);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbModelDes);
            this.Font = new System.Drawing.Font("Calibri", 11F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "NewEcoPackageForm";
            this.Text = "Ecological Packages";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbModelDes;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}