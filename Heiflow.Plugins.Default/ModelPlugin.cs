// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Climate;
using Heiflow.Controls.WinForm.Modflow;
using Heiflow.Controls.WinForm.Processing;
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
            _fd_grid = new SimpleActionItem("kModel", "Finite Difference Grid", FDGrid_Clicked)
            {
                Key = "kFDGrid",
                ToolTipText = "Create Finite Difference Grid",
                GroupCaption = "Grid",
                LargeImage = Properties.Resources.convert_to_mesh,
                SortOrder = 2
            };
            App.HeaderControl.Add(_fd_grid);

            _layer_group = new SimpleActionItem("kModel", "Layer Group", LayerGroup_Clicked)
            {
                Key = "kLayerGroup",
                ToolTipText = "Set Layer Group",
                GroupCaption = "Grid",
                LargeImage = Properties.Resources.stack,
                SortOrder = 2
            };
            App.HeaderControl.Add(_layer_group);

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
                SortOrder = 2
            };
            App.HeaderControl.Add(_GlobalSet);
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
            if(ProjectManager.Project != null && ProjectManager.Project.Model != null)
                ProjectManager.ShellService.PropertyView.SelectedObject = ProjectManager.Project.Model.GetType().GetProperty("MasterPackage").GetValue(ProjectManager.Project.Model);
        }
    }
}
