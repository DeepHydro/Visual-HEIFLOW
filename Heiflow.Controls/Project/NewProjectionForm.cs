// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Heiflow.Applications;
using Heiflow.Presentation.Services;

namespace Heiflow.Presentation.Controls.Project
{
    [Export(typeof(INewProject))]
    public partial class NewProjectionForm : Form, INewProject
    {
        private string _ProjectName;
        private string _ProjectPath;
        private int gListView1LostFocusItem;

        public NewProjectionForm()
        {
            InitializeComponent();
            lstPrjTemplate.DrawItem += lstPrjTemplate_DrawItem;
            lstPrjTemplate.Leave += lstPrjTemplate_Leave;
            lstPrjTemplate.HideSelection = false;
        }


        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
        }

        public string ProjectPath
        {
            get
            {
                return _ProjectPath;
            }
        }

        private void NewPrjForm_Load(object sender, EventArgs e)
        {
            lstPrjTemplate.Items.Clear();
            var imgs = new ImageList();
            imgs.ImageSize = new System.Drawing.Size(32, 32);
            lstPrjTemplate.LargeImageList = imgs;
            var project = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            foreach (var model in project.Serializer.SurpportedProjects)
            {
                if (model.LargeIcon != null)
                    imgs.Images.Add(model.LargeIcon);
                else
                    imgs.Images.Add(model.Icon);
            }

            int i = 0;
            foreach (var model in project.Serializer.SurpportedProjects)
            {
                lstPrjTemplate.Items.Add(new ListViewItem(model.NameToShown, i) { Tag = model });
                i++;
            }
            lstPrjTemplate.Items[0].Selected = true;
            txtPrjName.Text = "Heiflow Project";
            txtPrjDir.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void lstPrjTemplate_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // If this item is the selected item
            if (e.Item.Selected)
            {
                // If the selected item just lost the focus
                if (gListView1LostFocusItem == e.Item.Index)
                {
                    // Set the colors to whatever you want (I would suggest
                    // something less intense than the colors used for the
                    // selected item when it has focus)
                    e.Item.ForeColor = Color.Black;
                    e.Item.BackColor = Color.LightBlue;

                    // Indicate that this action does not need to be performed
                    // again (until the next time the selected item loses focus)
                    gListView1LostFocusItem = -1;
                }
                else if (lstPrjTemplate.Focused)  // If the selected item has focus
                {
                    // Set the colors to the normal colors for a selected item
                    e.Item.ForeColor = SystemColors.HighlightText;
                    e.Item.BackColor = SystemColors.Highlight;
                }
            }
            else
            {
                // Set the normal colors for items that are not selected
                e.Item.ForeColor = lstPrjTemplate.ForeColor;
                e.Item.BackColor = lstPrjTemplate.BackColor;
            }

            e.DrawBackground();
            e.DrawText();
        }

        private void lstPrjTemplate_Leave(object sender, EventArgs e)
        {
            gListView1LostFocusItem = lstPrjTemplate.FocusedItem.Index;
        }

        private void lstPrjTemplate_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            tbModelDes.Text = (e.Item.Tag as IProject).Description;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var project_service = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
               var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            if (lstPrjTemplate.SelectedItems.Count != 1)
                return;
            if(txtPrjName.Text == ""|| !Directory.Exists(txtPrjDir.Text))
            {                
                return;
            }
            _ProjectName = txtPrjName.Text;
            _ProjectPath = Path.Combine(txtPrjDir.Text, _ProjectName);

            try
            {
                ModelService.WorkDirectory = Path.GetFullPath(_ProjectPath);
                project_service.Serializer.New(_ProjectName, _ProjectPath, lstPrjTemplate.SelectedItems[0].Tag as IProject, shell.ProgressWindow, chbImprot.Checked);               
                project_service.Project = project_service.Serializer.CurrentProject;
             
            }
            catch(Exception ex)
            {
                string msg = string.Format("Failed to creat project. Error message: {0}" + ex.Message);
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPrjDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void txtPrjName_TextChanged(object sender, EventArgs e)
        {
            txtPrjName.TextChanged -= txtPrjName_TextChanged;
            txtPrjName.Text = txtPrjName.Text.Replace(' ', '_');
            txtPrjName.TextChanged += txtPrjName_TextChanged;
        }
    }
}
