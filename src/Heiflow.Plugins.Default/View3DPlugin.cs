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
