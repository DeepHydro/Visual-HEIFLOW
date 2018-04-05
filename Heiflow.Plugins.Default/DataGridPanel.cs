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

namespace Heiflow.Plugins.DataGridPanel
{
    public class DataGridPanel : Extension
    {
        DataGridEx _DataGridEx;
        public DataGridPanel()
        {
            DeactivationAllowed = false;

            _DataGridEx = new DataGridEx();
        }
    

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            App.DockManager.Add(new DockablePanel("kDataGridPanel", "Table View",
                _DataGridEx, DockStyle.None) { SmallImage = Resources.table_green_48 });

            var showDataGrid = new SimpleActionItem("kView", "Table View", 
                     delegate(object sender, EventArgs e)
           { App.DockManager.ShowPanel("kDataGridPanel"); }
                )
            {
                Key = "kShowDataGridPanel",
                ToolTipText = "Show Table View",
                GroupCaption = "Data",
                LargeImage = Resources.table_green_48
            };
            App.HeaderControl.Add(showDataGrid);
            App.DockManager.HidePanel("kDataGridPanel");

            base.Activate();
            ProjectManager.ShellService.DataGridView = _DataGridEx;
            ProjectManager.ShellService.AddChild(_DataGridEx);
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowDataGridPanel");
            App.DockManager.Remove("kDataGridPanel");
            base.Deactivate();
        }

        private void Option_Click(object sender, EventArgs e)
        {
          
        }
    }
}
