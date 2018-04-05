// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.Modeling.Forms;
using Heiflow.Plugins.Default.Properties;

namespace DotSpatial.Plugins.ToolManager
{
    public class ToolManagerPlugin : Extension, IPartImportsSatisfiedNotification
    {
        private Controls.ToolManager toolManager;

        [Import("Shell", typeof(ContainerControl))]
        private ContainerControl Shell { get; set; }

        /// <summary>
        /// Gets the list tools available.
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<ITool> Tools { get; set; }

        #region IPartImportsSatisfiedNotification Members

        public void OnImportsSatisfied()
        {
            if (IsActive)
            {
                // This method may be called on another thread after recomposition.
                if (Shell.InvokeRequired)
                {
                    Shell.Invoke((MethodInvoker)ShowToolsPanel);
                }
                else
                {
                    ShowToolsPanel();
                }
            }
        }

        #endregion

        public override void Activate()
        {
            ShowToolsPanel();
            var showTools = new SimpleActionItem("kView", "Spatial Tools",
              delegate(object sender, EventArgs e)
              { App.DockManager.ShowPanel("kTools"); })
            {
                Key = "kShowPackageTools",
                ToolTipText = "Show Spatial Tools",
                GroupCaption = "Map",
                LargeImage = toolManager.ImageList.Images["Hammer"]
            };
            App.HeaderControl.Add(showTools);


            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove("kTools");
            toolManager = null;
            base.Deactivate();
        }


        private void ShowToolsPanel()
        {
            if (Tools != null && Tools.Any())
            {
                if (toolManager != null) return;
                toolManager = new Controls.ToolManager
                {
                    App = App,
                    Legend = App.Legend,
                    Location = new Point(208, 12),
                    Name = "toolManager",
                    Size = new Size(192, 308),
                    TabIndex = 1
                };
                App.CompositionContainer.ComposeParts(toolManager);
                Shell.Controls.Add(toolManager);
                App.DockManager.Add(new DockablePanel("kTools", "Spatial Tools", toolManager, DockStyle.Left) 
                { SmallImage = toolManager.ImageList.Images["Hammer"] });


            }
            else
            {
                toolManager = null;
                App.DockManager.Remove("kTools");
            }
        }
    }
}