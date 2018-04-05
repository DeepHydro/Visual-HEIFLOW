// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
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

        public ViewPlugin()
        {

        }

        public override void Activate()
        {
            RootItem view = new RootItem("kView", "View");
            App.HeaderControl.Add(view);
            RootItem model = new RootItem("kModel", "Model");
            App.HeaderControl.Add(model);

            var showMap = new SimpleActionItem("kView", "Map", ShowMap_Clicked)
               {
                   Key = "kShowMap",
                   ToolTipText = "Show Map Control",
                   GroupCaption = "Map",
                   LargeImage = Properties.Resources.Map32
               };
            App.HeaderControl.Add(showMap);
            base.Activate();
        }



        private void ShowMap_Clicked(object sender, EventArgs e)
        {
            App.DockManager.ShowPanel("kMap");
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove("kMap");
            base.Deactivate();
        }
    }
}
