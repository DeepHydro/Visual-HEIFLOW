// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Heiflow.Models.Integration;
using Heiflow.Presentation.Controls.Project;
using Heiflow.Applications.Controllers;
using Heiflow.Presentation;
using Heiflow.Controls.WinForm.Project;
using Heiflow.Plugins.Susbed.Properties;

namespace Heiflow.Plugins.Menubar
{
    public class MenuBarPlugin : Extension
    {
        private const string FileMenuKey = HeaderControl.ApplicationMenuKey;
        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;
        public MenuBarPlugin()
        {
            DeactivationAllowed = false;
        }

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            AddMenuItems();
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        private void AddMenuItems()
        {
            IHeaderControl header = App.HeaderControl;

            header.Add(new SimpleActionItem(FileMenuKey, "打开工程文件...", OpenProject_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 0,
                SmallImage = Resources.ReportLoad16,
                LargeImage = Resources.ReportLoad32,
                ToolTipText = "打开工程文件"
            });

            header.Add(new SimpleActionItem(FileMenuKey, "保存工程", SaveProject_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 2,
                SmallImage = Resources.GenericSave_B_16,
                LargeImage = Resources.GenericSave_B_32,
                ToolTipText = "保存工程"
            });
            header.Add(new SimpleActionItem(FileMenuKey, "关于", About_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 4,
                SmallImage = Resources.information32,
                LargeImage = Resources.information32,
                ToolTipText = "关于本系统"
            });

            header.Add(new SimpleActionItem(FileMenuKey, "退出", Exit_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 1000,
                SmallImage = Resources.exit32,
                LargeImage = Resources.exit32,
                ToolTipText = "退出"
            });

        }

        private void OpenProject_Click(object sender, EventArgs e)
        {
            if (ProjectManager.ProjectService.Project != null)
            {
                var msg = string.Format("The project {0} has been opened. Do you want to close the project?", ProjectManager.ProjectService.Project.Name);
                var dlg = MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dlg == DialogResult.No || dlg == DialogResult.Cancel)
                {
                    return;
                }
            }
            ProjectManager.ProjectService.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            string filter = "";
            foreach (var prjp in ProjectManager.ProjectService.Serializer.OpenProjectFileProviders)
            {
                filter += prjp.FileTypeDescription + "|*" + prjp.Extension + "|";
            }
            filter = filter.TrimEnd(new char[] { '|' });
            ofd.Filter = filter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ProjectManager.Open.Execute(ofd.FileName);
            }
        }

        private void SaveProject_Click(object sender, EventArgs e)
        {
            if (ProjectManager.Save.CanExecute(null))
            {
                Cursor.Current = Cursors.WaitCursor;
                //   if (ProjectManager.Project.IsDirty)
                ProjectManager.Save.Execute(null);
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Show("You cann't save since no project has been created!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }
        private void About_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
