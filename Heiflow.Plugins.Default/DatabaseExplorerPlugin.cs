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
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class DatabaseExplorerPlugin : Extension
    {
        private UserControl _Explorer;

        [Import("VHFManager", typeof(VHFAppManager))]
        public VHFAppManager Manager
        {
            get;
            set;
        }

        public DatabaseExplorerPlugin()
        {

        }

        public override void Activate()
        {
            this._Explorer = Manager.DatabaseExplorerController.ViewModel.View as UserControl;
            this._Explorer.Name = "dbexplorer";

            var dock = new DockablePanel("kDatabaseExplorer", "Database",
                _Explorer, DockStyle.Right) { SmallImage = Properties.Resources.DatabaseServer16 };
            App.DockManager.Add(dock);

            var showDatabase = new SimpleActionItem("kView", "ODM Database",
           delegate(object sender, EventArgs e)
           { App.DockManager.ShowPanel("kDatabaseExplorer"); })
            {
                Key = "kShowDatabaseExplorer",
                ToolTipText = "Show ODM Database Explorer",
                GroupCaption = "Data",
                LargeImage = Heiflow.Plugins.Default.Properties.Resources.DatabaseServer32
            };

            App.HeaderControl.Add(showDatabase);
            this.App.DockManager.HidePanel("kDatabaseExplorer");
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowDatabaseExplorer");
            this.App.DockManager.Remove("kDatabaseExplorer");
            base.Deactivate();
        }
    }
}
