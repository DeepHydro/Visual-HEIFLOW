// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using Heiflow.Applications.Controllers;
using Heiflow.Controls;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class ProgressPanelPlugin : Extension
    {
        private ProgressForm _ProgressForm;

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            _ProgressForm = new ProgressForm();
            ProjectManager.ShellService.ProgressWindow = _ProgressForm;
            ProjectManager.ShellService.ProgressWindow.MainForm = ProjectManager.ShellService.MainForm;
            ProjectManager.ShellService.AddChild(_ProgressForm);
        }

        public override void Deactivate()
        {
            _ProgressForm.Close();
        }

    }
}
