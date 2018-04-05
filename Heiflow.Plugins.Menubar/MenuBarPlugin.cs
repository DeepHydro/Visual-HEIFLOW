//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using Heiflow.Plugins.Menubar.Properties;
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

            header.Add(new SimpleActionItem(FileMenuKey, "Open Project", OpenProject_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 0,
                SmallImage = Resources.ReportLoad16,
                LargeImage = Resources.ReportLoad32,
                ToolTipText = "Open project"
            });

            header.Add(new SimpleActionItem(FileMenuKey, "New Project", NewProject_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 1,
                SmallImage = Resources.MapWindowNew16,
                LargeImage = Resources.MapWindowNew32,
                ToolTipText = "Create a new project"
            });

            header.Add(new SimpleActionItem(FileMenuKey, "Save Project", SaveProject_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 2,
                SmallImage = Resources.GenericSave_B_16,
                LargeImage = Resources.GenericSave_B_32,
                ToolTipText = "Save current project"
            });

            header.Add(new SimpleActionItem(FileMenuKey, "Import Model", ImportModel_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 3,
                SmallImage = Resources.MapServiceDefinitionSave16,
                LargeImage = Resources.MapServiceDefinitionSave32,
                ToolTipText = "Import a model"
            });

            header.Add(new SimpleActionItem(FileMenuKey, "About", About_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 4,
                SmallImage = Resources.information32,
                LargeImage = Resources.information32,
                ToolTipText = "About this system"
            });

            header.Add(new SimpleActionItem(FileMenuKey, "Exit", Exit_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 1000,
                SmallImage = Resources.exit32,
                LargeImage = Resources.exit32,
                ToolTipText = "Exit"
            });

        }

        private void About_Click(object sender, EventArgs e)
        {
            //if (ProjectManager.Project != null)
            //{
            //    var mf = (ProjectManager.Project.Model as HeiflowModel).ModflowModel;
            //    string mfn = @"E:\PKU\Models\ZB_HRB\zb.mfn";
            //    string fsfile = @"G:\E\Heihe\HRB\GeoData\Surface\Grid_HRU.shp";
            //    mf.Extract(mfn, fsfile, 347, 108, 496, 279);
            //}
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void OpenProject_Click(object sender, EventArgs e)
        {
            if (ProjectManager.ProjectService.Project != null)
            {
                var msg = string.Format("The project has  been opened. Do you want to save the project?", ProjectManager.ProjectService.Project.Name);
                if (MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    ProjectManager.Save.Execute(null);
                }
                ProjectManager.ShellService.ClearContents();
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

        private void NewProject_Click(object sender, EventArgs e)
        {
            if (ProjectManager.ProjectService.Project != null)
            {
                var msg = string.Format("The project has been opened. Do you want to save the project?", ProjectManager.ProjectService.Project.Name);
                var result = MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result== DialogResult.Yes)
                {
                    ProjectManager.Save.Execute(null);
                }
                else if (result== DialogResult.Cancel)
                {
                    return;
                }
                ProjectManager.ShellService.ClearContents();
                ProjectManager.ProjectService.Clear();
            }

            if (ProjectManager.New.CanExecute(null))
            {
                ProjectManager.New.Execute(null);
            }
        }

        private void SaveProject_Click(object sender, EventArgs e)
        {
            if (ProjectManager.Save.CanExecute(null))
            {
                Cursor.Current = Cursors.WaitCursor;
                    ProjectManager.Save.Execute(null);
                Cursor.Current = Cursors.Default;
            }
            else
            {
                MessageBox.Show("You cann't save since no project has been created!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }

        private void ImportModel_Click(object sender, EventArgs e)
        {
            if (ProjectManager.Project == null)
            {
                MessageBox.Show("Please creat a new project at first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ImportModelForm form = new ImportModelForm(ProjectManager.Project);
            form.ShowInTaskbar = false;
            if (form.ShowDialog() == DialogResult.OK)
            {
                ProjectManager.ShellService.ProjectExplorer.AddProject(ProjectManager.Project);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
