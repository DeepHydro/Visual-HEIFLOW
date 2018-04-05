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
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Presentation;

namespace DotSpatial.Plugins.ToolManager
{
    public class MatEditorPlugin : Extension
    {
        private DataCubeEditor _TV3DMatEditor;

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }
        public override void Activate()
        {
            ShowEditorPanel();
            var showMatEditor = new SimpleActionItem("kView", "Data Cube Editor",
            delegate(object sender, EventArgs e)
            { App.DockManager.ShowPanel("kMatEditor"); })
            {
                Key = "kShowMatEditor",
                ToolTipText = "Show Data Cube Editor",
                GroupCaption = "Model Tool",
                LargeImage = Resources.matrix
            };
            App.HeaderControl.Add(showMatEditor);
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove("kMatEditor");
            _TV3DMatEditor = null;
            base.Deactivate();
        }


        private void ShowEditorPanel()
        {
            _TV3DMatEditor = new DataCubeEditor()
            {
                Location = new Point(208, 12),
                Name = "dcEditor",
                Size = new Size(1300, 800),
            };
            ProjectManager.ShellService.TV3DMatEditor = _TV3DMatEditor;
            ProjectManager.ShellService.AddChild(_TV3DMatEditor);
            App.DockManager.Add(new DockablePanel("kDCEditor", "Data Cube Editor", _TV3DMatEditor, DockStyle.None) { SmallImage = Resources.matrix16 });
           
        }
    }
}