namespace Heiflow.Controls.WinForm.Project
{
    partial class CreatHeiflowFromMFForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbBASfile = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnBrowseBAS = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPrjName = new System.Windows.Forms.TextBox();
            this.btnSelecProjection = new System.Windows.Forms.Button();
            this.tbX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbDIS = new System.Windows.Forms.TextBox();
            this.btnBrowseDIS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Modflow BAS File";
            // 
            // tbBASfile
            // 
            this.tbBASfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBASfile.Location = new System.Drawing.Point(171, 7);
            this.tbBASfile.Name = "tbBASfile";
            this.tbBASfile.Size = new System.Drawing.Size(485, 27);
            this.tbBASfile.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(670, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(556, 167);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnBrowseBAS
            // 
            this.btnBrowseBAS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseBAS.Location = new System.Drawing.Point(670, 4);
            this.btnBrowseBAS.Name = "btnBrowseBAS";
            this.btnBrowseBAS.Size = new System.Drawing.Size(100, 30);
            this.btnBrowseBAS.TabIndex = 6;
            this.btnBrowseBAS.Text = "Browse...";
            this.btnBrowseBAS.UseVisualStyleBackColor = true;
            this.btnBrowseBAS.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Projection";
            // 
            // tbPrjName
            // 
            this.tbPrjName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPrjName.Location = new System.Drawing.Point(171, 79);
            this.tbPrjName.Name = "tbPrjName";
            this.tbPrjName.Size = new System.Drawing.Size(485, 27);
            this.tbPrjName.TabIndex = 1;
            // 
            // btnSelecProjection
            // 
            this.btnSelecProjection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelecProjection.Location = new System.Drawing.Point(670, 77);
            this.btnSelecProjection.Name = "btnSelecProjection";
            this.btnSelecProjection.Size = new System.Drawing.Size(100, 30);
            this.btnSelecProjection.TabIndex = 6;
            this.btnSelecProjection.Text = "Select...";
            this.btnSelecProjection.UseVisualStyleBackColor = true;
            this.btnSelecProjection.Click += new System.EventHandler(this.btnSelecProjection_Click);
            // 
            // tbX
            // 
            this.tbX.Location = new System.Drawing.Point(188, 122);
            this.tbX.Name = "tbX";
            this.tbX.Size = new System.Drawing.Size(182, 27);
            this.tbX.TabIndex = 1;
            this.tbX.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Upper left coordinate";
            // 
            // tbY
            // 
            this.tbY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbY.Location = new System.Drawing.Point(403, 122);
            this.tbY.Name = "tbY";
            this.tbY.Size = new System.Drawing.Size(253, 27);
            this.tbY.TabIndex = 1;
            this.tbY.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(166, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "X:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(376, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Y:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Modflow DIS File";
            // 
            // tbDIS
            // 
            this.tbDIS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDIS.Location = new System.Drawing.Point(171, 43);
            this.tbDIS.Name = "tbDIS";
            this.tbDIS.Size = new System.Drawing.Size(485, 27);
            this.tbDIS.TabIndex = 1;
            // 
            // btnBrowseDIS
            // 
            this.btnBrowseDIS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDIS.Location = new System.Drawing.Point(670, 38);
            this.btnBrowseDIS.Name = "btnBrowseDIS";
            this.btnBrowseDIS.Size = new System.Drawing.Size(100, 30);
            this.btnBrowseDIS.TabIndex = 6;
            this.btnBrowseDIS.Text = "Browse...";
            this.btnBrowseDIS.UseVisualStyleBackColor = true;
            this.btnBrowseDIS.Click += new System.EventHandler(this.btnBrowseDIS_Click);
            // 
            // CreatHeiflowFromMFForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 211);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelecProjection);
            this.Controls.Add(this.btnBrowseDIS);
            this.Controls.Add(this.btnBrowseBAS);
            this.Controls.Add(this.tbY);
            this.Controls.Add(this.tbX);
            this.Controls.Add(this.tbPrjName);
            this.Controls.Add(this.tbDIS);
            this.Controls.Add(this.tbBASfile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CreatHeiflowFromMFForm";
            this.ShowInTaskbar = false;
            this.Text = "Creat Heiflow From Modflow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbBASfile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnBrowseBAS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPrjName;
        private System.Windows.Forms.Button btnSelecProjection;
        private System.Windows.Forms.TextBox tbX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbDIS;
        private System.Windows.Forms.Button btnBrowseDIS;
    }
}