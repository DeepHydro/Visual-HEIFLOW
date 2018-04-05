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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Applications.Views;
using System.ComponentModel.Composition;
using Heiflow.Applications.ViewModels;
using System.Waf.Applications;
using Heiflow.Controls.Tree;
using Heiflow.Core.Data.ODM;
using Heiflow.Controls.WinForm.Properties;
using System.Data.OleDb;
using Heiflow.Presentation.Controls;

namespace Heiflow.Controls.WinForm.DatabaseExplorer
{
    [Export(typeof(IDatabaseExplorerView))]
    public partial class DatabaseExplorer : UserControl, IDatabaseExplorerView
    {
        private Lazy<DatabaseExplorerViewModel> viewModel;
        private TreeModel _TreeModel;
        private ODMNodeCreator _NodeCreator;
        private SeriesCatalog _SeriesCatalog;
        private GroupMethod _GroupMethod = GroupMethod.VariableMedium;
        private ToolStripMenuItem[] _group_items;
        private TreeNodeAdv m_OldSelectNode;
        private ODMConextMenu _ODMConextMenu;

        public DatabaseExplorer()
        {
            InitializeComponent();
            this.Load += DatabaseExplorer_Load;
            _NodeCreator = new ODMNodeCreator();
            _NodeCreator.ImageList = this.imageList_odm;
            this.nodeStateIcon1.DataPropertyName = "Image";
            this.nodeTextBox1.DataPropertyName = "Text";
            _GroupMethod = GroupMethod.VariableMedium;
            _group_items = new ToolStripMenuItem[]
            {
                 btnGroupBySiteCategory,
                 btnGroupBySiteState,
                 btnGroupByVariable
            };

        }
        public object DataContext
        {
            get;
            set;
        }

        private void DatabaseExplorer_Load(object sender, EventArgs e)
        {
            _TreeModel = new TreeModel();
            treeView1.Model = _TreeModel;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            treeView1.MouseUp += treeView1_MouseUp;
            viewModel = new Lazy<DatabaseExplorerViewModel>(() => ViewHelper.GetViewModel<DatabaseExplorerViewModel>(this));
            _ODMConextMenu = new ODMConextMenu()
            {
                ShellService = viewModel.Value.ShellService
            };
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = e.Node.Tag as Node;

            var record = node.Tag as IDendritiRecord<ObservationSeries>;
            if (record != null && record.Value != null)
            {
                var dt = viewModel.Value.ODMSource.GetValues(record.Value);
                viewModel.Value.ShellService.DataGridView.Bind(dt);
            }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                var node = treeView1.GetNodeAt(p);
                if (node != null)
                {
                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    m_OldSelectNode = treeView1.SelectedNode;
                    treeView1.SelectedNode = node;
                    var pnode = node.Tag as Node;
                    if (pnode != null)
                    {
                        _ODMConextMenu.ODMSource = _SeriesCatalog.ODM;
                        _ODMConextMenu.Enable(pnode.Tag);
                        _ODMConextMenu.DataContext = pnode.Tag;
                        _ODMConextMenu.ContextMenu.Show(treeView1, p);
                        // Highlight the selected node.
                        treeView1.SelectedNode = m_OldSelectNode;
                        m_OldSelectNode = null;
                    }
                }
            }
            else
            {
                _ODMConextMenu.EnableAll(false);
            }

        }

        private void btnConectODM_Click(object sender, EventArgs e)
        {
            if (viewModel.Value.ODMSource != null)
            {
                if (_SeriesCatalog == null)
                {
                    _SeriesCatalog = new SeriesCatalog(viewModel.Value.ODMSource);
                    btnRefresh_Click(null, null);
                }
                else
                {
                    MessageBox.Show("An ODM database has been connected. If you want to connect to another database, please removec connection to current database at first"
                        , "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                OpenFileDialog form = new OpenFileDialog();
                form.Filter = "access 2003 file |*.mdb|access 2007-2013 file|*.accdb";
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ODMSource odm = new ODMSource();
                    string msg = "";
                    if (odm.Open(form.FileName, ref msg))
                    {
                        viewModel.Value.ProjectService.Project.ODMSource = odm;
                        viewModel.Value.ODMSource = odm;
                        _SeriesCatalog = new SeriesCatalog(odm);
                        btnRefresh_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect to the database. Error message: " + msg, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!CheckConnection())
                return;
            _SeriesCatalog.ODM.UpdateSiteList();
            _SeriesCatalog.ODM.UpdateVariableList();
            _SeriesCatalog.Retrieve();
            CreateNodes();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (!CheckConnection())
                return;

            ImportODMData import_odm = new ImportODMData(_SeriesCatalog.ODM);
            import_odm.ShowDialog();
        }
        private void btnRemoveDB_Click(object sender, EventArgs e)
        {
            if (CheckConnection())
            {
                viewModel.Value.ODMSource.Close();
                viewModel.Value.ODMSource = null;
                viewModel.Value.ProjectService.Project.ODMSource = null;
                _TreeModel.Nodes.Clear();
            }
        }
        private void btnGroupBySiteState_Click(object sender, EventArgs e)
        {
            if (!CheckConnection())
                return;

            foreach (var item in _group_items)
            {
                if (sender.Equals(item))
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
            if (sender.Equals(btnGroupByVariable))
            {
                _GroupMethod = GroupMethod.VariableMedium;
            }
            else if (sender.Equals(btnGroupBySiteState))
            {
                _GroupMethod = GroupMethod.SiteState;
            }
            else if (sender.Equals(btnGroupBySiteCategory))
            {
                _GroupMethod = GroupMethod.SiteCategory;
            }
            CreateNodes();
        }

        private bool CheckConnection()
        {
            if (viewModel.Value.ODMSource == null || _SeriesCatalog == null)
            {
                MessageBox.Show("You need to connect to an ODM database at first!");
                return false;
            }
            else
            {
                return true; 
            }
        }

        private void CreateNodes()
        {
            if (_SeriesCatalog == null)
                return;

            treeView1.BeginUpdate();
            _TreeModel.Nodes.Clear();
            var root_node = new Node(_SeriesCatalog.ODM.Name)
            {
                Image = Resources.DatabaseServer16,
                Tag = _SeriesCatalog.ODM
            };
            _SeriesCatalog.GroupMethod = _GroupMethod;
            var records = _SeriesCatalog.Group();
            var nodes = _NodeCreator.Creat(records);
            foreach (var node in nodes)
            {
                root_node.Nodes.Add(node);
            }
            _TreeModel.Nodes.Add(root_node);
            treeView1.EndUpdate();
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode;
            if (node != null)
            {
                var record = (node.Tag as Node).Tag as IDendritiRecord<ObservationSeries>;
                if (record != null && record.Children.Count ==0)
                {
                    var qc = new QueryCriteria()
                    {
                        SiteID = record.Value.SiteID,
                        VariableID = record.Value.VariableID,
                        AllTime = true
                    };
                    var ts = _SeriesCatalog.ODM.GetTimeSeries(qc);

                    if (ts != null)
                    {
                        var variable =_SeriesCatalog.ODM.GetVariable(qc.VariableID);
                        viewModel.Value.ShellService.SelectPanel(DockPanelNames.DataGridPanel);
                        viewModel.Value.ShellService.DataGridView.Bind(ts.ToDataTable(variable.Name));
                    }
                }
            }
        }

    }
}
