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
using Heiflow.Applications.Controllers;
using Heiflow.Plugins.Default.Properties;
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
    public class PropertyGridPlugin : Extension
    {
        private PropertyGridEx _PropertyGrid;

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            this._PropertyGrid = new PropertyGridEx();
            this._PropertyGrid.Name = "propGrid1";
            App.DockManager.Add
                (new DockablePanel("kPropGrid", 
                    "Property", _PropertyGrid, DockStyle.Right) { SmallImage = Properties.Resources.MetadataProperties16 });

            var showPropertyGrid = new SimpleActionItem("kView", Resources.Property,
               delegate(object sender, EventArgs e)
               { App.DockManager.ShowPanel("kPropGrid"); }
          )
            {
                Key = "kShowProperty",
                ToolTipText = Resources.Property,
                GroupCaption = Resources.Common_Group,
                LargeImage = Resources.MetadataProperties32
            };
            App.HeaderControl.Add(showPropertyGrid);

            base.Activate();
            ProjectManager.ShellService.PropertyView = _PropertyGrid;
            ProjectManager.ShellService.AddChild(_PropertyGrid);
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowProperty");
            this.App.DockManager.Remove("kPropGrid");
            base.Deactivate();
        }

        private void ShowPropGrid()
        {
      
        }
    }
}
