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
using Heiflow.Plugins.Default.Properties;
using Heiflow.Presentation;
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Heiflow.Plugins.Animation
{
    public class ModelAnalysisPlugin : Extension
    {
        AnimationPlayer _Player;
        public ModelAnalysisPlugin()
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
            _Player = new AnimationPlayer(); 

            var panel = new DockablePanel("kAnimation", "Animation",  _Player, DockStyle.Right) 
                {
                    SmallImage = Resources.TrackingDataAnimationTool16
                };
            App.DockManager.Add(panel);


            var showAnimation = new SimpleActionItem("kModel", "Animation",
                   ShowPanel)
            {
                Key = "kShowAnimation",
                ToolTipText = "Show Animation Player",
                GroupCaption = "Analysis",
                LargeImage = Resources.TrackingDataAnimationTool32,
                SortOrder = 5
            };
            App.HeaderControl.Add(showAnimation);
            App.DockManager.HidePanel("kAnimation");
            ProjectManager.ShellService.AnimationPlayer = _Player;
            ProjectManager.ShellService.AddChild(_Player);
            base.Activate();
           
        }


        private void ShowPanel(object sender, EventArgs e)
        {
            App.DockManager.ShowPanel("kAnimation");
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowAnimation");
            this.App.DockManager.Remove("kAnimation");
            base.Deactivate();
        }
  
    }
}
