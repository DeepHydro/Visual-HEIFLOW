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

using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Project;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.GHM;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.MenuItems
{
    [Export(typeof(IPEContextMenu))]
    public class VariablesFolderContextMenu : PEContextMenu
    {

        public const string _LA = "Load Variables";
        public const string _LAD = "Load All Data";
        public const string _AT = "Table View...";
        public const string _OP = "Advanced...";

        private Node _SelectedNode;

        public VariablesFolderContextMenu()
        {

        }

        public override Type ItemType
        {
            get
            {
                return typeof(VariablesFolderItem);
            }
        }

        public override void AddMenuItems()
        {
            ContextMenuItems.Add(new ExplorerMenuItem(_LA, null, LoadVariables_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem(_LAD, null, LoaddAllData_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem(_AT, Resources.AttributesWindow16, AttributeTable_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem(_OP, Resources.GenericWindowLightBlue16, Optional_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem(PEContextMenu.MenuSeparator, null, null));
        }

        public override void Initialize()
        {
            base.Initialize();
            Enable(_LAD, false);
        }

        protected virtual void LoadVariables_Clicked(object sender, EventArgs e)
        {
            if (Package is IDataPackage)
            {
                _SelectedNode = (sender as ToolStripMenuItem).Tag as Node;
                IDataPackage dp = null;
                if (Package is GHMPackage)
                {
                    dp = (from vv in (Package as GHMPackage).DynamicVariables where vv.Name == _SelectedNode.Text select vv).First();
                }
                else
                {
                    dp = Package as IDataPackage;
                }
                dp.PropertyChanged += dp_PropertyChanged;
                dp.ScanFailed += dp_ScanFailed;
                //if (Package.TimeService != null)
                //    dp.Layer = Package.TimeService.CurrentGridLayer;
                Cursor.Current = Cursors.WaitCursor;
                dp.Scan();
                dp.PropertyChanged -= dp_PropertyChanged;
                dp.ScanFailed -= dp_ScanFailed;
                Cursor.Current = Cursors.Default;
            }

        }
        protected virtual void LoaddAllData_Clicked(object sender, EventArgs e)
        {

        }
        protected virtual void AttributeTable_Clicked(object sender, EventArgs e)
        {
            var item = _Item as VariablesFolderItem;
            if (Package != null && item != null)
            {
                _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
                var buf = _Package.GetType().GetProperty(item.PropertyInfo.Name);
                if (buf != null)
                {
                    var vv = buf.GetValue(_Package);
                    var dc = vv as IDataCubeObject;
                    if (dc != null)
                    {
                        dc.SelectedVariableIndex = -1;
                        dc.SelectedTimeIndex = 0;
                        _ShellService.DataGridView.Bind( dc);
                        _ShellService.DataGridView.ShowView();
                    }
                }
            }
            else
            {
                if(this.Tag != null)
                {
                    var paras = this.Tag as IParameter[];
                    _ShellService.DataGridView.Bind(paras);
                    _ShellService.DataGridView.ShowView();
                }
            }
        }

        protected virtual void Optional_Clicked(object sender, EventArgs e)
        {
            if (Package != null && Package.OptionalView != null)
                Package.OptionalView.ShowView(_ShellService.MainForm);
        }

        private void dp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {  
            if (e.PropertyName == "Variables" && _SelectedNode != null)
            {
                var dp = sender as IDataPackage;
                if (dp.Variables == null)
                {
                    _ShellService.MessageService.ShowWarning(null, "The file is empty. No variables are loaded");
                    return;
                }
                _SelectedNode.Nodes.Clear();

                for (int i = 0; i < dp.Variables.Length; i++)
                {
                    DynamicVariableItem item = new DynamicVariableItem(i)
                    {
                        PropertyInfo = new SimplePropertyInfo(dp.Variables[i], Type.GetType("System.String")),
                    };

                    var creator = _ProjectService.NodeFactory.Select(item);
                    var node = creator.Creat(Package, item) as Node;
                    _SelectedNode.Nodes.Add(node);
                }
            }
        }

        protected void dp_ScanFailed(object sender, string e)
        {
            MessageBox.Show("Failed to load. Error message: " + e, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}