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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
