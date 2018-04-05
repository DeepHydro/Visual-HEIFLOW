// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Plugins.Default.Properties;
using Heiflow.Presentation;
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Heiflow.Plugins.View3DPanel
{
    public class View3DPlugin: Extension
    {
        private  Win3DView _Win3DView;
        public View3DPlugin()
        {
            DeactivationAllowed = false;
            _Win3DView = new Win3DView();
        }
     

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            var showView3D = new SimpleActionItem("kView", "3D View", delegate(object sender, EventArgs e)
            { ProjectManager.ShellService.SurfacePlot.ShowView(ProjectManager.ShellService.MainForm); })
            {
                Key = "kShowView3D",
                ToolTipText = "Show 3D View",
                GroupCaption = "Data",
                LargeImage = Resources._3d_plot_printing_printer
            };
            App.HeaderControl.Add(showView3D);
        
            base.Activate();
            ProjectManager.ShellService.SurfacePlot = _Win3DView;
            ProjectManager.ShellService.AddChild(_Win3DView);

        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowView3D");
            base.Deactivate();
        }
    }
}
