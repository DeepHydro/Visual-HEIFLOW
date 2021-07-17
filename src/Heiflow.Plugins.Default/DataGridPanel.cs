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
       //DataGridEx _DataGridEx;
        DataCubeGrid _DataGridEx;
        public DataGridPanel()
        {
            DeactivationAllowed = false;
          //  _DataGridEx = new DataGridEx();          
        }
    

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            _DataGridEx = new DataCubeGrid();
            App.DockManager.Add(new DockablePanel("kDataGridPanel", Resources.Table_View,
                _DataGridEx, DockStyle.None) { SmallImage = Resources.table_green_48 });

            var showDataGrid = new SimpleActionItem("kView", Resources.Table_View, 
                     delegate(object sender, EventArgs e)
           { App.DockManager.ShowPanel("kDataGridPanel"); }
                )
            {
                Key = "kShowDataGridPanel",
                ToolTipText = Resources.Table_View_tips,
                GroupCaption = Resources.Data_Group,
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
