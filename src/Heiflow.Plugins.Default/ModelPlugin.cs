﻿//
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
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Modflow;
using Heiflow.Controls.WinForm.Processing;
using Heiflow.Controls.WinForm.Project;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Plugins.Default.Properties;
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
        private SimpleActionItem _ParaMapping;
        private SimpleActionItem _Create_GridFromMF;

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

            _layer_group = new SimpleActionItem("kModel", Resources.Layer_Group, LayerGroup_Clicked)
            {
                Key = "kLayerGroup",
                ToolTipText = Resources.Layer_Group,
                GroupCaption = Resources.Grid_group,
                LargeImage = Resources.stack,
                SortOrder = 0
            };
            App.HeaderControl.Add(_layer_group);

            _fd_grid = new SimpleActionItem("kModel", Resources.Finite_Difference_Grid, FDGrid_Clicked)
            {
                Key = "kFDGrid",
                ToolTipText = Resources.Finite_Difference_Grid,
                GroupCaption = Resources.Grid_group,
                LargeImage = Properties.Resources.convert_to_mesh,
                SortOrder = 1
            };
            App.HeaderControl.Add(_fd_grid);


            _runCascade = new SimpleActionItem("kModel", Resources.Cascade, RunCascade_Clicked)
            {
                Key = "kRunCascade",
                ToolTipText = Resources.Cascade,
                GroupCaption = Resources.Grid_group,
                LargeImage = Properties.Resources.FileNetworkDataset32,
                Enabled = true,
                SortOrder = 2
            };
            App.HeaderControl.Add(_runCascade);

            _GlobalSet = new SimpleActionItem("kModel", Resources.Global, GlobalSet_Clicked)
            {
                Key = "kGlobalSet",
                ToolTipText = Resources.Global,
                GroupCaption = Resources.Setting_group,
                LargeImage = Properties.Resources.equilizer_48,
                Enabled = true,
                SortOrder = 3
            };
            App.HeaderControl.Add(_GlobalSet);

            _ParaViewer = new SimpleActionItem("kModel", Resources.Surface_Parameter, ParaViewer_Clicked)
            {
                Key = "kParaViewer",
                ToolTipText = Resources.Surface_Parameter,
                GroupCaption = Resources.Setting_group,
                LargeImage = Properties.Resources.TableFields32,
                Enabled = true,
                SortOrder = 4
            };
            App.HeaderControl.Add(_ParaViewer);

            _ParaMapping = new SimpleActionItem("kModel", Resources.Parameter_Mapping_Tool, ParaMapping_Clicked)
            {
                Key = "kParaMapping",
                ToolTipText = Resources.Parameter_Mapping_Tool,
                GroupCaption = Resources.Tool_group,
                LargeImage = Properties.Resources.hyper_link,
                Enabled = true,
                SortOrder = 0
            };
            App.HeaderControl.Add(_ParaMapping);

            _Create_GridFromMF = new SimpleActionItem("kModel", Resources.CreateGridFromMF, CreateGridFromMF_Clicked)
            {
                Key = "kCreateGridFromMF",
                ToolTipText = Resources.CreateGridFromMF,
                GroupCaption = Resources.Tool_group,
                LargeImage = Properties.Resources.grid128,
                Enabled = true,
                SortOrder = 0
            };
            App.HeaderControl.Add(_Create_GridFromMF);

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

        private void ParaMapping_Clicked(object sender, EventArgs e)
        {
            if (ProjectManager.Project != null && ProjectManager.Project.Model != null)
            {
                ParaMapping dlg = new ParaMapping();
                dlg.ShowDialog();
            }
        }

        private void CreateGridFromMF_Clicked(object sender, EventArgs e)
        {
            if (ProjectManager.Project != null)
            {
                if (ProjectManager.Project.Model.TimeService.Initialized)
                {
                    CreatHeiflowFromMFForm form = new CreatHeiflowFromMFForm(ProjectManager);
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

    }
}