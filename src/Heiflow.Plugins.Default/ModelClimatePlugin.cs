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