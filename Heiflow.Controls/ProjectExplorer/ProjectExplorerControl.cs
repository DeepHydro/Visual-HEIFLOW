// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data.Forms;
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.MenuItems;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GHM;
using Heiflow.Models.UI;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace Heiflow.Presentation.Controls.Project
{
    [Export(typeof(IProjectExplorer))]
    public partial class ProjectExplorerControl : ScrollingControl, IProjectExplorer
    {
        private TreeModel _model;
        private ContextMenuStrip _contextMenu;
        private TreeNodeAdv m_OldSelectNode;
        private IProject _currentPrj;

        public ProjectExplorerControl()
        {
            InitializeComponent();
            _model = new TreeModel();
            treeView1.Model = _model;
            this._nodeStateIcon.DataPropertyName = "Image";
            this._nodeTextBox.DataPropertyName = "Text";
            _contextMenu = new ContextMenuStrip();
            this.treeView1.ContextMenuStrip = _contextMenu;
            this.treeView1.MouseUp += treeView1_MouseUp;
            this.treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            this.BackColor = ColorTranslator.FromHtml("#FF2D2D30");
            this.treeView1.BackColor = Color.White; //ColorTranslator.FromHtml("#FF2D2D30");
            this.treeView1.BorderStyle = BorderStyle.None;
        }

        public IExplorerNodeFactory NodeFactory
        {
            get;
            set;
        }

        public IPEContextMenuFactory ContextMenuFactory
        {
            get;
            set;
        }

        public static TreeNodeAdv SelectedNode
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return "ProjectExplorerControl"; }
        }

        public void ShowView(IWin32Window pararent)
        {
            
        }
        public void Initialize()
        {
            foreach (var menu in ContextMenuFactory.Menus)
            {
                menu.Initialize();
            }
            foreach (var creator in NodeFactory.Creators)
            {
                creator.ContextMenuFactory = this.ContextMenuFactory;
                creator.NodeFactory = this.NodeFactory;
            }
        }

        public void AddProject(IProject prj)
        {
            if (prj == null || prj.Model == null)
                return;
            CreateItems(prj.Model);
            _currentPrj = prj;
            prj.Model.PackageAdded += Model_PackageAdded;
            prj.Model.PackageRemoved += Model_PackageRemoved;
            prj.Model.PackageStatechanged += Model_PackageStatechanged;
        }

        private void Model_PackageStatechanged(object sender, IPackage pck)
        {
            var node = SelectNode(pck.Name);
            if (node != null)
            {
                if(pck.State == ModelObjectState.Ready)
                {
                    node.Image = Resources.MapPackageTiledTPKFile16;
                }
                else if(pck.State == ModelObjectState.Standby)
                {
                    node.Image = Resources.PkgInfo_File16;
                }
            }
        }

        private void Model_PackageRemoved(object sender, IPackage pck)
        {
            var node = SelectNode(pck.Name);
            if (node != null)
            {
                treeView1.BeginUpdate();
                var parent = node.Parent;
                parent.Nodes.Remove(node);
                treeView1.EndUpdate();
            }
        }

        private void Model_PackageAdded(object sender, IPackage pck)
        {
            var node = SelectNode(pck.Owner.Name);
            if(node != null)
            {
                var atr_pck = pck.GetType().GetCustomAttributes(typeof(PackageItem), true);
                if (atr_pck.Length != 0)
                {
                    treeView1.BeginUpdate();
                    var atr = atr_pck[0] as PackageItem;
                    var creator = NodeFactory.Select(atr);
                    creator.ContextMenuFactory = this.ContextMenuFactory;
                    creator.NodeFactory = this.NodeFactory;
                    var pck_node = creator.Creat(pck, atr) as Node;
                    node.Nodes.Add(pck_node);
                    treeView1.EndUpdate();
                }
            }
        }

        private void CreateItems(IBasicModel model)
        {
            if (model == null)
                return;
            treeView1.BeginUpdate();
            _model.Nodes.Clear();

            var atr_model = model.GetType().GetCustomAttributes(typeof(ModelItem), true);
            if (atr_model.Length != 0)
            {
                var atr = atr_model[0] as ModelItem;
                var creator = NodeFactory.Select(atr);
                creator.ContextMenuFactory = this.ContextMenuFactory;
                creator.NodeFactory = this.NodeFactory;
                var root = creator.Creat(model, atr) as Node;
                _model.Nodes.Add(root);
            }

            var mapdata_item = new MapDataItem();
            var map_creator = NodeFactory.Select(mapdata_item);
            var root_mapdata = map_creator.Creat(model, mapdata_item) as Node;
            _model.Nodes.Add(root_mapdata);

            treeView1.EndUpdate();
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

                    _contextMenu.Items.Clear();
                    if (node.Tag != null)
                    {
                        SelectedNode = node;
                        var item = (node.Tag as Node).Tag as IPEContextMenu;
                        if (item != null)
                        {
                            foreach (var ci in item.ContextMenuItems)
                            {
                                if (ci.Name == PEContextMenu.MenuSeparator)
                                {
                                    _contextMenu.Items.Add(new ToolStripSeparator());
                                    continue;
                                }
                                if (ci.Name != "Property")
                                {
                                    ToolStripMenuItem drop = null;
                                    if (ci.ClickHandler != null)
                                    {
                                        drop = _contextMenu.Items.Add(ci.Name, ci.Image, ci.ClickHandler) as ToolStripMenuItem;
                                    }
                                    else
                                        drop = _contextMenu.Items.Add(ci.Name, ci.Image) as ToolStripMenuItem;
                                    drop.Enabled = ci.Enabled;
                                    drop.Tag = node.Tag;
                                    foreach (var sub in ci.MenuItems)
                                    {
                                        if (sub.Name == PEContextMenu.MenuSeparator)
                                        {
                                            drop.DropDown.Items.Add(new ToolStripSeparator());
                                            continue;
                                        }
                                        if (sub.ClickHandler != null)
                                        {
                                            var mi = drop.DropDown.Items.Add(sub.Name, sub.Image, sub.ClickHandler);
                                            mi.Enabled = ci.Enabled;
                                            mi.Tag = node.Tag;
                                        }
                                        else
                                        {
                                            var mi = drop.DropDown.Items.Add(sub.Name, sub.Image);
                                            mi.Enabled = ci.Enabled;
                                            mi.Tag = node.Tag;
                                        }
                                    }
                                    _contextMenu.Items.Add(drop);
                                }
                            }

                            var prop_mi = from mi in item.ContextMenuItems where mi.Name == "Property" select mi;
                            if (prop_mi.Count() == 1)
                            {
                                var mi = prop_mi.First();
                                _contextMenu.Items.Add(mi.Name, mi.Image, mi.ClickHandler);
                            }
                            // Find the appropriate ContextMenu depending on the selected node.
                            _contextMenu.Show(treeView1, p);

                            // Highlight the selected node.
                            treeView1.SelectedNode = m_OldSelectNode;
                            m_OldSelectNode = null;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _contextMenu.Items.Count; i++)
                    {
                        _contextMenu.Items[i].Enabled = false;
                    }
                }
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var item = (e.Node.Tag as Node).Tag as IPEContextMenu;
            if (item != null)
            {
                item.NodeDoubleClick();
            }
        }

        public void ClearContent()
        {
            treeView1.BeginUpdate();
            _model.Nodes.Clear();
            treeView1.EndUpdate();
        }

        private Node SelectNode(string name)
        {
            Node selected = null;
            foreach (var node in _model.Root.Nodes)
            {
                selected = SelectChildNode(node, name);
                if (selected != null)
                    break;
            }
            return selected;
        }

        private Node SelectChildNode(Node parent, string name)
        {
            if (parent.Nodes.Count > 0)
            {
                Node found = null;
                foreach (var node in parent.Nodes)
                {
                    if (node.Text == name)
                    {
                        found = node;
                        break;
                    }
                    else
                    {
                        found = SelectChildNode(node, name);
                    }
                }
                return found;
            }
            else
            {
                return null;
            }
        }

        public void InitService()
        {

        }
    }
}