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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Climate;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Modflow;
using Heiflow.Controls.WinForm.Processing;
using Heiflow.Controls.WinForm.Project;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Presentation;
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class ModelPlugin : Extension
    {
        private SimpleActionItem _fd_grid;
        private SimpleActionItem _layer_group;
        private SimpleActionItem _runCascade;
        private SimpleActionItem _GlobalSet;
        private SimpleActionItem _ParaViewer;

        public ModelPlugin()
        {

        }
        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {

            _layer_group = new SimpleActionItem("kModel", "Layer Group", LayerGroup_Clicked)
            {
                Key = "kLayerGroup",
                ToolTipText = "Set Layer Group",
                GroupCaption = "Grid",
                LargeImage = Properties.Resources.stack,
                SortOrder = 0
            };
            App.HeaderControl.Add(_layer_group);

            _fd_grid = new SimpleActionItem("kModel", "Finite Difference Grid", FDGrid_Clicked)
            {
                Key = "kFDGrid",
                ToolTipText = "Create Finite Difference Grid",
                GroupCaption = "Grid",
                LargeImage = Properties.Resources.convert_to_mesh,
                SortOrder = 1
            };
            App.HeaderControl.Add(_fd_grid);


            _runCascade = new SimpleActionItem("kModel", "Cascade", RunCascade_Clicked)
            {
                Key = "kRunCascade",
                ToolTipText = "Calculate Cascade",
                GroupCaption = "Grid",
                LargeImage = Properties.Resources.FileNetworkDataset32,
                Enabled = true,
                SortOrder = 2
            };
            App.HeaderControl.Add(_runCascade);

            _GlobalSet = new SimpleActionItem("kModel", "Global", GlobalSet_Clicked)
            {
                Key = "kGlobalSet",
                ToolTipText = "Global Setting",
                GroupCaption = "Setting",
                LargeImage = Properties.Resources.equilizer_48,
                Enabled = true,
                SortOrder = 3
            };
            App.HeaderControl.Add(_GlobalSet);

            _ParaViewer = new SimpleActionItem("kModel", "Surface Parameter", ParaViewer_Clicked)
            {
                Key = "kParaViewer",
                ToolTipText = "Surface Parameter Viewer",
                GroupCaption = "Setting",
                LargeImage = Properties.Resources.TableFields32,
                Enabled = true,
                SortOrder = 4
            };
            App.HeaderControl.Add(_ParaViewer);

            ProjectManager.ShellService.ParameterExplorerView = new ParameterExplorer();
        }

        private void FDGrid_Clicked(object sender, EventArgs e)
        {
            if (ProjectManager.Project != null)
            {
                if (ProjectManager.Project.Model.TimeService.Initialized)
                {
                    RegularGridGenForm form = new RegularGridGenForm(ProjectManager);
                    form.ShowDialog();
                }
                else
                {
                    ProjectManager.ShellService.MessageService.ShowError(null, "The model time has not been set!");
                }
            }
            else
            {
                ProjectManager.ShellService.MessageService.ShowError(null, "You need to open or creat a project at first");
            }
        }

        private void LayerGroup_Clicked(object sender, EventArgs e)
        {
            if (ProjectManager.Project != null && ProjectManager.Project.Model != null)
            {
                var model = ProjectManager.Project.Model as Heiflow.Models.Integration.HeiflowModel;
                if (model != null)
                {
                    LayerGroupForm form = new LayerGroupForm(model.ModflowModel.LayerGroupManager);
                    form.ShowDialog();
                }
                else
                {
                    var mf = ProjectManager.Project.Model as Heiflow.Models.Subsurface.Modflow;
                    LayerGroupForm form = new LayerGroupForm(mf.LayerGroupManager);
                    form.ShowDialog();
                }
            }
            else
            {
                ProjectManager.ShellService.MessageService.ShowError(null, "There is no project");
            }
        }

        private void RunCascade_Clicked(object sender, EventArgs e)
        {
            CascadeForm form = new CascadeForm();
            form.ShowDialog();
        }
        private void GlobalSet_Clicked(object sender, EventArgs e)
        {
            if (ProjectManager.Project != null && ProjectManager.Project.Model != null)
            {
                ProjectManager.ShellService.PropertyView.SelectedObject = ProjectManager.Project.Model.GetType().GetProperty("MasterPackage").GetValue(ProjectManager.Project.Model);
                GlobalOptionForm form = new GlobalOptionForm();
                form.ShowDialog();
            }
        }
        private void ParaViewer_Clicked(object sender, EventArgs e)
        {
            if (ProjectManager.Project != null && ProjectManager.Project.Model != null)
            {
                ProjectManager.ShellService.ParameterExplorerView.ShowView(ProjectManager.ShellService.MainForm);
            }
        }
    }
}
