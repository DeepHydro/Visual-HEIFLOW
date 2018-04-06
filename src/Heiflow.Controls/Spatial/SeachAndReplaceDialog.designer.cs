using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Spatial
{
    /// <summary>
    /// The Designer for a dialog to replace a field.
    /// </summary>
    partial class SeachAndReplaceDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(SeachAndReplaceDialog));
            this.label1 = new Label();
            this.label2 = new Label();
            this.txtReplace = new TextBox();
            this.btnCancel = new Button();
            this.btnOK = new Button();
            this.txtFind = new TextBox();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            //
            // label2
            //
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            //
            // txtReplace
            //
            this.txtReplace.AccessibleDescription = null;
            this.txtReplace.AccessibleName = null;
            resources.ApplyResources(this.txtReplace, "txtReplace");
            this.txtReplace.BackgroundImage = null;
            this.txtReplace.Font = null;
            this.txtReplace.Name = "txtReplace";
            //
            // btnCancel
            //
            this.btnCancel.AccessibleDescription = null;
            this.btnCancel.AccessibleName = null;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackgroundImage = null;
            this.btnCancel.Font = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.BtnCancelClick);
            //
            // btnOK
            //
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackgroundImage = null;
            this.btnOK.Font = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.BtnOkClick);
            //
            // txtFind
            //
            this.txtFind.AccessibleDescription = null;
            this.txtFind.AccessibleName = null;
            resources.ApplyResources(this.txtFind, "txtFind");
            this.txtFind.BackgroundImage = null;
            this.txtFind.Font = null;
            this.txtFind.Name = "txtFind";
            //
            // Replace
            //
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = ((ContainerControl)this).AutoScaleMode;
            this.BackgroundImage = null;
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = null;
            this.Name = "SeachAndReplaceDialog";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtReplace;
        private Button btnCancel;
        private Button btnOK;
        private TextBox txtFind;
    }
}