// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
