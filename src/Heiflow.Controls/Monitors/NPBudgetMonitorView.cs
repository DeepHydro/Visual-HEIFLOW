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

using Heiflow.Applications.ViewModels;
using Heiflow.Applications.Views;
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Monitors;
using Heiflow.Models.Running;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Display
{
    /// <summary>
    /// N / P 质量平衡分析用户控件。
    /// 输入文件为工作目录下的 sz_N_budget.csv 与 sz_P_budget.csv。
    /// </summary>
    [Export(typeof(INPBudgetMonitorView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class NPBudgetMonitorView : UserControl, INPBudgetMonitorView
    {
        private readonly Lazy<NPBudgetMonitorViewModel> viewModel;
        private TreeModel _model;
        private TreeNodeAdv m_OldSelectNode;
        private NPBudgetNodeCreators _NodeCreator;
        private int step = 0;

        public NPBudgetMonitorView()
        {
            InitializeComponent();

            this.nodeStateIcon1.DataPropertyName = "Image";
            this.nodeTextBox1.DataPropertyName = "Text";

            _model = new TreeModel();
            treeView1.Model = _model;
            viewModel = new Lazy<NPBudgetMonitorViewModel>(() => ViewHelper.GetViewModel<NPBudgetMonitorViewModel>(this));

            _NodeCreator = new NPBudgetNodeCreators();
            _NodeCreator.Chart = winChart1;
            _NodeCreator.DataGrid = this.olvMassBudget;
            _NodeCreator.ReportBox = this.textBoxReport;

            treeView1.MouseUp += treeView1_MouseUp;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            olvMassBudget.RootKeyValue = 9999;
            this.Load += NPBudgetMonitorView_Load;
        }

        public object DataContext
        {
            get;
            set;
        }

        #region IView 实现

        public void Show()
        {
        }

        public void Close()
        {
        }

        public void UpdateView()
        {
            step++;
        }

        public void Reset()
        {
            step = 0;
        }

        public void RefreshTree()
        {
            _model.Nodes.Clear();
            treeView1.BeginUpdate();
            foreach (var monitor in viewModel.Value.Monitors)
            {
                var nodes = _NodeCreator.Creat(monitor);
                foreach (var node in nodes)
                    _model.Nodes.Add(node);
            }
            treeView1.EndUpdate();
            _NodeCreator.Initialize();
            treeView1.ExpandAll();
        }

        #endregion

        #region 事件处理

        private void NPBudgetMonitorView_Load(object sender, EventArgs e)
        {
            if (viewModel.Value.Monitor != null)
                propertyGrid1.SelectedObject = viewModel.Value.Monitor;
            RefreshTree();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var vm = viewModel.Value;
            if (vm.LoadCommand != null && vm.LoadCommand.CanExecute(null))
                vm.LoadCommand.Execute(null);
            RefreshTree();
            Cursor.Current = Cursors.Default;
        }

        private void btnMassBudget_Click(object sender, EventArgs e)
        {
            var monitor = SelectedMonitor();
            string message = "";
            if (!_NodeCreator.MassBudget(monitor, TitleOf(monitor), out message))
            {
                if (!string.IsNullOrEmpty(message))
                    MessageBox.Show(message, "N/P Mass Budget", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            tabControlMain.SelectedTab = tabPageReport;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (var monitor in viewModel.Value.Monitors)
            {
                monitor.Clear();
            }
            RefreshTree();
            textBoxReport.Clear();
            ClearBudgetTable();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = e.Node.Tag as Node;
            var item = node.Tag as MonitorItem;
            if (item == null)
            {
                var collection = node.Tag as MonitorItemCollection;
                if (collection != null)
                    _NodeCreator.PlotGroup(collection);
                return;
            }
            _NodeCreator.Plot(item);
            tabControlMain.SelectedTab = tabPageGraph;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = e.Node.Tag as Node;
            if (node.Tag is IMonitorItem)
            {
                var monitor = (node.Tag as IMonitorItem).Monitor;
                propertyGrid1.SelectedObject = monitor;
                viewModel.Value.Monitor = monitor;
            }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            var p = new Point(e.X, e.Y);
            var node_av = treeView1.GetNodeAt(p);
            if (node_av == null)
                return;

            var node = node_av.Tag as Node;
            var menu = node.ContextMenu;
            if (menu == null)
                return;

            m_OldSelectNode = treeView1.SelectedNode;
            treeView1.SelectedNode = node_av;
            menu.Tag = node.Tag;
            menu.Show(treeView1, p);
            treeView1.SelectedNode = m_OldSelectNode;
            m_OldSelectNode = null;
        }

        #endregion

        #region 私有方法

        private IFileMonitor SelectedMonitor()
        {
            if (treeView1.SelectedNode != null)
            {
                var node = treeView1.SelectedNode.Tag as Node;
                if (node != null)
                {
                    var item = node.Tag as IMonitorItem;
                    if (item != null && item.Monitor != null)
                        return item.Monitor;
                }
            }
            return viewModel.Value.Monitor;
        }

        private string TitleOf(IFileMonitor monitor)
        {
            var itemRoot = monitor != null && monitor.Root.Count > 0 ? monitor.Root[0].Name : "Mass Budget";
            return itemRoot;
        }

        private void ClearBudgetTable()
        {
            olvMassBudget.DataSource = null;
        }

        #endregion
    }
}
