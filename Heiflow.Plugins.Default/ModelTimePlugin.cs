// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Modflow;
using Heiflow.Controls.WinForm.Project;
using Heiflow.Models.Generic;
using Heiflow.Presentation;
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class ModelTimePlugin : Extension
    {
        private SimpleActionItem _heiflow_time;

        public ModelTimePlugin()
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
            _heiflow_time = new SimpleActionItem("kModel", "HEIFLOW Time", HeiflowTime_Clicked)
            {
                Key = "kHeiflowTime",
                ToolTipText = "Set HEIFLOW Time",
                GroupCaption = "Time",
                LargeImage = Properties.Resources.calendar_32,
                SortOrder = 1
            };
            App.HeaderControl.Add(_heiflow_time);
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kHeiflowTime");
            App.HeaderControl.Remove("_mf_time");
            base.Deactivate();
        }

        private void HeiflowTime_Clicked(object sender, EventArgs e)
        {
            if (ProjectManager.Project != null)
            {
                HeiflowTimeControl form = new HeiflowTimeControl(ProjectManager.Project.Model.TimeServiceList["Base Timeline"] as TimeService,
                    ProjectManager.Project.Model.TimeServiceList["Subsurface Timeline"] as TimeService);
                form.ShowDialog();
            }
            else
            {
                ProjectManager.ShellService.MessageService.ShowError(null, "You need to open or creat a project at first");
            }
        }
    }
}
