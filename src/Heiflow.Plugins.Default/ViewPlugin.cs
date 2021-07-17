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
using Heiflow.Plugins.Default.Properties;
using Heiflow.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class ViewPlugin : Extension
    {

        [Import("VHFManager", typeof(VHFAppManager))]
        public VHFAppManager Manager
        {
            get;
            set;
        }
        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }
        public ViewPlugin()
        {

        }

        public override void Activate()
        {
            RootItem view = new RootItem("kView",Resources.View_root);
            App.HeaderControl.Add(view);
            RootItem model = new RootItem("kModel", Resources.Model_root);
            App.HeaderControl.Add(model);

            var showMap = new SimpleActionItem("kView", Resources.Map_control, ShowMap_Clicked)
               {
                   Key = "kShowMap",
                   ToolTipText = Resources.Map_control,
                   GroupCaption = Resources.Map_Group,
                   LargeImage = Properties.Resources.Map32
               };
            App.HeaderControl.Add(showMap);

            var showProgress = new SimpleActionItem("kView", Resources.ProgressWindow, ShowProgress_Clicked)
            {
                Key = "kShowProgress",
                ToolTipText = Resources.ProgressWindow,
                GroupCaption = Resources.Common_Group,
                LargeImage = Properties.Resources.progess64
            };
            App.HeaderControl.Add(showProgress);
            base.Activate();
        }



        private void ShowMap_Clicked(object sender, EventArgs e)
        {
            App.DockManager.ShowPanel("kMap");
        }
        private void ShowProgress_Clicked(object sender, EventArgs e)
        {
            ProjectManager.ShellService.ProgressWindow.ShowView(ProjectManager.ShellService.MainForm);
        }
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove("kMap");
            base.Deactivate();
        }
    }
}
