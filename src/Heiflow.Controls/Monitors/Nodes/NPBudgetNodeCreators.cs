//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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
    /// <summary>
    /// N / P 质量平衡收支项树的构建与交互助手
    /// </summary>
    public class NPBudgetNodeCreators
    {
        private ContextMenuStrip _RootMenu;
        private ContextMenuStrip _GroupMenu;
        private ContextMenuStrip _ItemMenu;
        protected IShellService _ShellService;

        public NPBudgetNodeCreators()
        {
            _RootMenu = new ContextMenuStrip();
            _GroupMenu = new ContextMenuStrip();
            _ItemMenu = new ContextMenuStrip();

            _RootMenu.Items.Add(VariablesFolderContextMenu._AT, Resources.AttributesWindow16, TableView_Click);
            _RootMenu.Items.Add("Mass Budget", Resources.GraphHistogram32, MassBudget_Click);
            _RootMenu.Items.Add("Plot All Terms", Resources._3dplot16, PlotGroup_Click);

            _GroupMenu.Items.Add(VariablesFolderContextMenu._AT, Resources.AttributesWindow16, TableView_Click);
            _GroupMenu.Items.Add("Mass Budget", Resources.GraphHistogram32, MassBudget_Click);
            _GroupMenu.Items.Add("Plot All Terms", Resources._3dplot16, PlotGroup_Click);

            _ItemMenu.Items.Add(VariablesFolderContextMenu._AT, Resources.AttributesWindow16, TableView_Click);
            _ItemMenu.Items.Add("Plot", Resources._3dplot16, PlotItem_Click);
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
            if (_ShellService == null)
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
        }

        /// <summary>
        /// 按收支项分组构建树节点
        /// </summary>
        public List<Node> Creat(IFileMonitor monitor)
        {
            var nodes = new List<Node>();
            foreach (var root in monitor.Root)
            {
                root.Monitor = monitor;
                var root_node = new Node(root.Name)
                {
                    Tag = root,
                    ContextMenu = _RootMenu,
                    Image = Resources.AnimationCreateGroup16
                };

                var categories = from item in root.Children
                                 group item by item.Group into cat
                                 select new { Category = cat.Key, Items = cat.ToArray() };

                foreach (var cat in categories)
                {
                    var cat_item = new MonitorItemCollection(cat.Category);
                    cat_item.Monitor = monitor;
                    cat_item.Children.AddRange(cat.Items);

                    var group_node = new Node(cat.Category)
                    {
                        ContextMenu = _GroupMenu,
                        Tag = cat_item,
                        Image = Resources.KML_GroundOverlay16
                    };

                    foreach (var item in cat.Items)
                    {
                        var leaf = new Node(item.Name)
                        {
                            ContextMenu = _ItemMenu,
                            Tag = item,
                            Image = Resources.ItemInformation16
                        };
                        group_node.Nodes.Add(leaf);
                    }
                    root_node.Nodes.Add(group_node);
                }

                if (root.IsDisplay)
                    nodes.Add(root_node);
            }
            return nodes;
        }

        #region 绘图

        public void Plot(MonitorItem item)
        {
            if (item == null || item.Monitor == null || item.Monitor.DataSource == null)
                return;

            var dates = item.Monitor.DataSource.Dates.ToArray();
            if (dates.Length == 0)
                return;

            var values = Resolve(item);
            if (values == null)
                return;

            Chart.Plot<double>(dates, values, item.Name, SeriesChartType.FastLine, "Date", MassTitle(item.Monitor));
        }

        /// <summary>
        /// 绘制一个分组下的所有收支项
        /// </summary>
        public void PlotGroup(MonitorItemCollection group)
        {
            if (group == null || group.Children.Count == 0)
                return;
            if (Chart != null)
                Chart.Clear();
            foreach (var item in group.Children)
            {
                PlotCore(item, !ReferenceEquals(item, group.Children[0]));
            }
        }

        private void PlotCore(MonitorItem item, bool keepExisting)
        {
            if (item == null || item.Monitor == null || item.Monitor.DataSource == null)
                return;

            var dates = item.Monitor.DataSource.Dates.ToArray();
            if (dates.Length == 0)
                return;

            var values = Resolve(item);
            if (values == null)
                return;

            bool old = true;
            if (Chart != null)
            {
                old = Chart.ClearExistesSeries;
                Chart.ClearExistesSeries = !keepExisting;
            }
            Chart.Plot<double>(dates, values, item.Name, SeriesChartType.FastLine, "Date", MassTitle(item.Monitor));
            if (Chart != null)
                Chart.ClearExistesSeries = old;
        }

        private double[] Resolve(MonitorItem item)
        {
            if (item.Derivable)
            {
                item.DerivedValues = item.Derive(item.Monitor.DataSource);
                return item.DerivedValues;
            }
            if (item.VariableIndex < 0 || item.VariableIndex >= item.Monitor.DataSource.Values.Length)
                return null;
            return item.Monitor.DataSource.Values[item.VariableIndex].ToArray();
        }

        private string MassTitle(IFileMonitor monitor)
        {
            var np = monitor as NPBudgetMonitor;
            if (np == null)
                return "Mass";
            return string.Format("Mass ({0})", np.MassUnit);
        }

        #endregion

        #region 质量平衡

        /// <summary>
        /// 计算质量平衡并填充数据表与报表
        /// </summary>
        public bool MassBudget(IFileMonitor monitor, string title, out string message)
        {
            message = "";
            if (monitor == null)
            {
                message = "未选择有效的监视器。";
                return false;
            }
            if (monitor.DataSource == null || monitor.DataSource.Dates.Count == 0)
            {
                message = "尚未加载数据，请先执行 Load。";
                return false;
            }

            string report = "";
            var dt = monitor.Balance(title, ref report);
            if (dt == null)
            {
                message = "无法生成质量平衡表，请检查数据。";
                return false;
            }

            if (ReportBox != null)
                ReportBox.Text = report;

            if (DataGrid != null)
            {
                var ds = new DataSet();
                dt.TableName = "MassBudget";
                ds.Tables.Add(dt);
                DataGrid.DataMember = "MassBudget";
                DataGrid.DataSource = new DataViewManager(ds);
                DataGrid.ExpandAll();
            }
            return true;
        }

        #endregion

        #region 上下文菜单

        private void TableView_Click(object sender, EventArgs e)
        {
            var item = (sender as ToolStripItem).Owner.Tag as MonitorItem;
            if (item == null || item.Monitor == null || item.Monitor.DataSource == null)
                return;

            Initialize();
            var dt = item.ToDataTable(item.Monitor.DataSource);
            if (_ShellService == null)
                return;
            _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
            _ShellService.DataGridView.Bind(dt);
        }

        private void MassBudget_Click(object sender, EventArgs e)
        {
            var menu = (sender as ToolStripItem).Owner;
            var collection = menu.Tag as MonitorItemCollection;
            if (collection == null)
                return;

            string msg;
            MassBudget(collection.Monitor, collection.Name, out msg);
            ShowMessage(msg);
        }

        private void PlotGroup_Click(object sender, EventArgs e)
        {
            var menu = (sender as ToolStripItem).Owner;
            var collection = menu.Tag as MonitorItemCollection;
            if (collection == null)
                return;
            PlotGroup(collection);
        }

        private void PlotItem_Click(object sender, EventArgs e)
        {
            var menu = (sender as ToolStripItem).Owner;
            Plot(menu.Tag as MonitorItem);
        }

        private void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            MessageBox.Show(message, "N/P Mass Budget", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}
