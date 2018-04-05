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
using Heiflow.Models.Surface.PRMS;
using Heiflow.Presentation;
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{

    public class ModelClimatePlugin : Extension
    {
        private SimpleActionItem _Weather;

        public ModelClimatePlugin()
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

            _Weather = new SimpleActionItem("kModel", "Weather Generator", WeatherGenerator_Clicked)
            {
                Key = "kWeatherGenerator",
                ToolTipText = "Generate weather data",
                GroupCaption = "Driving Forces",
                LargeImage = Properties.Resources.ThunderD_A_32,
                SortOrder = 3
            };
            App.HeaderControl.Add(_Weather);
        }

        private void WeatherGenerator_Clicked(object sender, EventArgs e)
        {
            if( ProjectManager.Project == null)
            {
                ProjectManager.ShellService.MessageService.ShowWarning(null, "No project opened or created.");
                return;
            }

            var pck = ProjectManager.Project.Model.GetPackage(ClimateDataPackage.PackageName) as ClimateDataPackage;
            if (pck != null)
            {
                WeatherGeneratorForm form = new WeatherGeneratorForm(pck);
                form.ShowInTaskbar = false;
                form.ShowDialog();
            }
            else
            {
                ProjectManager.ShellService.MessageService.ShowWarning(null, "Can't find climate data package");
            }
        }
    }
}