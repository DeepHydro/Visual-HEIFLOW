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