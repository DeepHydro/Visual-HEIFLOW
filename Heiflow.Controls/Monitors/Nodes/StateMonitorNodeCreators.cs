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

using Heiflow.Applications;
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Controls.WinForm.MenuItems;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Running;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Controls.WinForm.Monitors
{
    public class StateMonitorNodeCreators
    {
        private ContextMenuStrip _RootMenu;
        private ContextMenuStrip _GroupMenu;
        private ContextMenuStrip _ItemMenu;
        protected IShellService _ShellService;
        public event EventHandler<Dictionary<string,double>> ZonalBudgetClicked;

        public StateMonitorNodeCreators()
        {
            _RootMenu = new ContextMenuStrip();
            _GroupMenu = new ContextMenuStrip();
            _ItemMenu = new ContextMenuStrip();

            ToolStripItem item = null;

            _RootMenu.Items.Add("Load", Resources.TmImportFeatures16, Load_Click);
            _RootMenu.Items.Add(VariablesFolderContextMenu._AT, Resources.AttributesWindow16, Attributes_Click);
            _RootMenu.Items.Add("Flow Budget",null, FlowBudget_Click);
           // _RootMenu.Items.Add("Zonal Budget", null, ZonalBudget_Click);
            item = _RootMenu.Items.Add("Plot", Resources._3dplot16, Plot_Click);
            item.Enabled = false;

            item = _GroupMenu.Items.Add("Load", Resources.TmImportFeatures16, Load_Click);
            item.Enabled = false;
            _GroupMenu.Items.Add(VariablesFolderContextMenu._AT, Resources.AttributesWindow16, Attributes_Click);
            item = _GroupMenu.Items.Add("Plot", Resources._3dplot16, Plot_Click);
            item.Enabled = false;

            item = _ItemMenu.Items.Add("Load", Resources.TmImportFeatures16, Load_Click);
            item.Enabled = false;
            _ItemMenu.Items.Add(VariablesFolderContextMenu._AT, Resources.AttributesWindow16, Attributes_Click);
            _ItemMenu.Items.Add("Plot", Resources._3dplot16, ItemPlot_Click);   
        }

        public WinChart Chart
        {
            get;
            set;
        }

        public BrightIdeasSoftware.DataTreeListView DataGrid
        {
            get;
            set;
        }

        public TextBox ReportBox
        {
            get;
            set;
        }

        public void Initialize()
        {
            _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
        }

        public List<Node> Creat(IFileMonitor monitor)
        {
            List<Node> nodes = new List<Node>();
            foreach (var root in monitor.Root)
            {
                root.Monitor = monitor;
                Node root_node = new Node(root.Name)
                {
                    Tag = root,
                    ContextMenu = _RootMenu,
                    Image = Resources.AnimationCreateGroup16,               
                };

                var categories = from item in root.Children
                                 group item by item.Group into cat
                                 select new { Category = cat.Key, Items = cat.ToArray() };

                foreach (var cat in categories)
                {
                    MonitorItemCollection cat_item = new MonitorItemCollection(cat.Category);
                    cat_item.Monitor = monitor;
                    cat_item.Children.AddRange(cat.Items);

                    Node tn = new Node(cat.Category)
                    {
                        ContextMenu = _GroupMenu,
                        Tag = cat_item,
                        Image = Resources.KML_GroundOverlay16
                    };

                    foreach (var item in cat.Items)
                    {
                        Node optn = new Node(item.Name)
                        {
                            ContextMenu = _ItemMenu,
                            Tag = item,
                            Image = Resources.ItemInformation16
                        };
                        tn.Nodes.Add(optn);
                    }
                    root_node.Nodes.Add(tn);
                }
                nodes.Add(root_node);
            }
            return nodes;
        }

        private void Attributes_Click(object sender, EventArgs e)
        {
            var item = (sender as ToolStripItem).Owner.Tag as MonitorItem;
            if (item == null || item.Monitor == null)
                return;
            if (item.Monitor.DataSource == null)
                return;

            var dt = item.ToDataTable(item.Monitor.DataSource);
            _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
            _ShellService.DataGridView.Bind(dt);
        }

        private void Load_Click(object sender, EventArgs e)
        {

        }

        private void Plot_Click(object sender, EventArgs e)
        {

        }
        private void ItemPlot_Click(object sender, EventArgs e)
        {           
            var item = (sender as ToolStripItem).Owner.Tag as MonitorItem;
            Plot(item);
        }

        private void FlowBudget_Click(object sender, EventArgs e)
        {
            var item = (sender as ToolStripItem).Owner.Tag as MonitorItem;
            if (item == null || item.Monitor == null)
                return;
            if (item.Monitor.DataSource == null)
                return;

            string report = "";
            var dt = item.Monitor.Balance(ref report);
            if(ReportBox !=null)
                ReportBox.Text = report;

            DataSet ds = new DataSet();
            dt.TableName = "Budget";
            ds.Tables.Add(dt);
            this.DataGrid.DataMember = "Budget";
            this.DataGrid.DataSource = new DataViewManager(ds);
            DataGrid.ExpandAll();

            var dic = item.Monitor.ZonalBudgets();
            if (ZonalBudgetClicked != null)
                ZonalBudgetClicked(this, dic);
        }

        //private void ZonalBudget_Click(object sender, EventArgs e)
        //{
        
        //}

        public void Plot(MonitorItem item)
        {
            if (item == null)
                return;
            if (item.Monitor.DataSource == null)
                return;
            var dates = item.Monitor.DataSource.Dates.ToArray();
            int step = dates.Length;

            if (step > 0)
            {
                if (item.Derivable)
                {
                    item.DerivedValues = item.Derive(item.Monitor.DataSource);
                    Chart.Plot<double>(dates, item.DerivedValues, item.Name, SeriesChartType.FastLine);
                }
                else
                {
                    Chart.Plot<double>(dates, item.Monitor.DataSource.Values[item.VariableIndex].ToArray(), item.Name, SeriesChartType.FastLine);
                }
            }
        }
    }
}