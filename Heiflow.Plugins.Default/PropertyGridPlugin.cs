// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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

            var showPropertyGrid = new SimpleActionItem("kView", "Property",
               delegate(object sender, EventArgs e)
               { App.DockManager.ShowPanel("kPropGrid"); }
          )
            {
                Key = "kShowProperty",
                ToolTipText = "Show Project Explorer",
                GroupCaption = "Common",
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
