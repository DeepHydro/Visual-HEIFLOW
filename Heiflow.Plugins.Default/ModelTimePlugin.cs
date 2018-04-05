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
