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
using Heiflow.Applications.Controllers;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.MT3DMS;
using Heiflow.Controls.WinForm.Processing;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Subsurface.MT3DMS;
using Heiflow.Models.Subsurface.VFT3D;
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
    public class MT3DMSPlugin : Extension
    {
        private SimpleActionItem setSpecies;
        //private SimpleActionItem stopmodel;

        [Import("VHFManager", typeof(VHFAppManager))]
        public VHFAppManager Manager
        {
            get;
            set;
        }

        public MT3DMSPlugin()
        {

        }

        public override void Activate()
        {
            setSpecies = new SimpleActionItem("kModel", "Set Species", RunModel_Clicked)
            {
                Key = "kSetSpecies",
                ToolTipText = "Set Species",
                GroupCaption = "MT3DMS",
                LargeImage = Properties.Resources.species,
                SortOrder = 1
            };
            App.HeaderControl.Add(setSpecies);
            base.Activate();
        }


        private void RunModel_Clicked(object sender, EventArgs e)
        {
            var vm = Manager.ProjectController.Project.Model;
            if (vm is  MT3DMSModel || vm is VFT3DModel)
            {
                SetSpeciesForm form = new SetSpeciesForm(vm as Modflow);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Current model is not MT3DMS or VFT3D", "Model", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public override void Deactivate()
        {
            App.HeaderControl.Remove("kSetSpecies");
            base.Deactivate();
        }
    }
}
