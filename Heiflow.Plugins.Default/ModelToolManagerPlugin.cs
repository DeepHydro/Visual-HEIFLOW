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
using Heiflow.Models.Tools;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Presentation;

namespace DotSpatial.Plugins.ToolManager
{
    public class ModelToolManagerPlugin : Extension, IPartImportsSatisfiedNotification
    {
        private ModelToolManager toolManager;

        [Import("Shell", typeof(ContainerControl))]
        private ContainerControl Shell { get; set; }

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

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
            var showTools = new SimpleActionItem("kView", "Model Toolbox",
              delegate(object sender, EventArgs e)
              { App.DockManager.ShowPanel("kModelToolbox"); })
            {
                Key = "kShowModelToolbox",
                ToolTipText = "Show Model Toolbox",
                GroupCaption = "Model Tool",
                LargeImage = toolManager.ImageList.Images[1]
            };
            App.HeaderControl.Add(showTools);
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove("kModelToolbox");
            toolManager = null;
            base.Deactivate();
        }

        private void ShowToolsPanel()
        {
            if (ProjectManager.Tools != null && ProjectManager.Tools.Any())
            {
                if (toolManager != null) return;
                toolManager = new ModelToolManager
                {
                    Location = new Point(208, 12),
                    Name = "pcktoolManager",
                    Size = new Size(192, 308),
                    TabIndex = 1
                };

                ProjectManager.ShellService.PackageToolManager = toolManager;
                ProjectManager.ShellService.AddChild(toolManager);
                App.CompositionContainer.ComposeParts(toolManager);
                App.DockManager.Add(new DockablePanel("kModelToolbox", "Model Toolbox", toolManager, DockStyle.Fill)
                {
                    SmallImage = toolManager.ImageList.Images[0]
                });
            }
        }
    }
}