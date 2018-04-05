// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Project;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
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
                var dp = Package as IDataPackage;
                dp.PropertyChanged += dp_PropertyChanged;
                dp.Layer = Package.TimeService.CurrentGridLayer;
                Cursor.Current = Cursors.WaitCursor;
                dp.Scan(); 
                dp.PropertyChanged -= dp_PropertyChanged;
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
                        PropertyInfo = new SimplePropertyInfo(dp.Variables[i], Type.GetType("System.String"))
                    };

                    var creator = _ProjectService.NodeFactory.Select(item);
                    var node = creator.Creat(Package, item) as Node;
                    _SelectedNode.Nodes.Add(node);
                }
            }
        }
    }
}