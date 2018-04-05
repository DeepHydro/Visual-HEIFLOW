// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
using Heiflow.Applications.ViewModels;
using System.Waf.Applications;
using System.ComponentModel.Composition;
using Heiflow.Controls.Tree;
using Heiflow.Models.IO;
using Heiflow.Models.Running;
using Heiflow.Controls.WinForm.Monitors;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Controls.WinForm.Display
{
    [Export(typeof(IStateMonitorView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StateMonitor : UserControl, IStateMonitorView
    {
        private readonly Lazy<StateMonitorViewModel> viewModel;
        private TreeModel _model;
        private TreeNodeAdv m_OldSelectNode;
        private StateMonitorNodeCreators _NodeCreator;
        private int step = 0;

        public StateMonitor()
        {
            InitializeComponent();
            this.nodeStateIcon1.DataPropertyName = "Image";
            this.nodeTextBox1.DataPropertyName = "Text";
            _model = new TreeModel();
            treeView1.Model = _model;
            viewModel = new Lazy<StateMonitorViewModel>(() => ViewHelper.GetViewModel<StateMonitorViewModel>(this));
            _NodeCreator = new StateMonitorNodeCreators();
            _NodeCreator.Chart = winChart1;
            _NodeCreator.DataGrid = this.olvDataTree;
            //_NodeCreator.ReportBox = this.textBox1;
            _NodeCreator.ZonalBudgetClicked += NodeCreator_ZonalBudgetClicked;
            this.treeView1.MouseUp += treeView1_MouseUp;
            olvDataTree.RootKeyValue = 9999;
            this.treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            this.Load += StateMonitor_Load;
        }



        public object DataContext
        {
            get;
            set;
        }

        private void StateMonitor_Load(object sender, EventArgs e)
        {
            RefreshTree();
        }
        public void Close()
        {

        }

        private void RefreshTree()
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
        }

        public void UpdateView()
        {
            step++;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = e.Node.Tag as Node;
            var item = node.Tag as MonitorItem;
            _NodeCreator.Plot(item);
            tabControl_Main.SelectedTab = tabPage_Graph;
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = e.Node.Tag as Node;
            if (node.Tag is IMonitorItem)
            {
                var monitor = (node.Tag as IMonitorItem).Monitor;
                propertyGrid1.SelectedObject = monitor;
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
                var node_av = treeView1.GetNodeAt(p);
                if (node_av != null)
                {
                    var node = node_av.Tag as Node;
                    var _contextMenu = node.ContextMenu;
                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    m_OldSelectNode = treeView1.SelectedNode;
                    treeView1.SelectedNode = node_av;
                    // Find the appropriate ContextMenu depending on the selected node.
                    _contextMenu.Tag = node.Tag;
                    _contextMenu.Show(treeView1, p);

                    // Highlight the selected node.
                    treeView1.SelectedNode = m_OldSelectNode;
                    m_OldSelectNode = null;
                }
                else
                {
                    //for (int i = 0; i < _contextMenu.Items.Count; i++)
                    //{
                    //    _contextMenu.Items[i].Enabled = false;
                    //}
                }
            }
        }

        public void Reset()
        {
            step = 0;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            var vm = viewModel.Value;
            if (vm.LoadCommand.CanExecute(null))
                vm.LoadCommand.Execute(null);
            RefreshTree();
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void NodeCreator_ZonalBudgetClicked(object sender, Dictionary<string, double> e)
        {
            var totalin = e[FileMonitor.PPT] + e[FileMonitor.Streams_Inflow] + e[FileMonitor.Groundwater_Inflow];
            var totalout = e[FileMonitor.Evapotranspiration] + e[FileMonitor.Evaporation] + e[FileMonitor.Streams_Outflow] + e[FileMonitor.Groundwater_Outflow];
            var totalds = e[FileMonitor.Total_Storage_Change];
            var totalerror = totalin - totalout - totalds;
            var totaldisp = Math.Round((totalerror) / (totalin + totalout + Math.Abs(totalds)) * 2 * 100, 2);

            var surface_et= e[FileMonitor.BASINIMPERVEVAP_HRU] + e[FileMonitor.BASININTCPEVAP_HRU]
                + e[FileMonitor.BASINSNOWEVAP_HRU] + e[FileMonitor.IR_Industry];

            var soilin =  e[FileMonitor.Streams_Inflow]
                + e[FileMonitor.IR_PUMP] + e[FileMonitor.SURFACE_LEAKAGE_OUT] + e[FileMonitor.PPT];
            var soilout = e[FileMonitor.BASININTERFLOW] + e[FileMonitor.BASINSROFF]
                + e[FileMonitor.BASINLAKEINSZ] + e[FileMonitor.BASINHORTONIANLAKES]
                 + e[FileMonitor.BASINPERVET_HRU] + e[FileMonitor.Streams_Outflow]
                 + e[FileMonitor.BASINIMPERVEVAP_HRU] + e[FileMonitor.BASININTCPEVAP_HRU] + e[FileMonitor.BASINSNOWEVAP_HRU]
                 + e[FileMonitor.UZF_INFIL]
                 + e[FileMonitor.CANAL_ET] + e[FileMonitor.SFRET] + e[FileMonitor.LAKET];
            var soilds = e[FileMonitor.Soil_Zone_DS] + e[FileMonitor.LAND_SURFACE_Zone_DS];
            var soilerror = soilin - soilout - soilds;
            var soildisp = Math.Round((soilerror) / (soilin + soilout + Math.Abs(soilds)) * 2 * 100, 2);

            var uzfin = e[FileMonitor.UZF_INFIL];
            var uzfout = e[FileMonitor.UZF_ET] + e[FileMonitor.UZF_RECHARGE];
            var uzfds = e[FileMonitor.Unsaturated_Zone_DS];
            var uzferror = uzfin - uzfout - uzfds;
            var uzf_discrepancy = Math.Round((uzferror) / (uzfin + uzfout + Math.Abs(uzfds)) * 2 * 100, 2);

            var satin = e[FileMonitor.UZF_RECHARGE] + e[FileMonitor.Groundwater_Inflow] + e[FileMonitor.STREAM_LEAKAGE_IN];
            var satout = e[FileMonitor.GW_ET_OUT] + e[FileMonitor.STREAM_LEAKAGE_OUT]
                + e[FileMonitor.IR_PUMP] + e[FileMonitor.SURFACE_LEAKAGE_OUT] + e[FileMonitor.Groundwater_Outflow];
            var satds = e[FileMonitor.Saturated_Zone_DS];
            var saterror = satin - satout - satds;
            var sat_discrepancy = Math.Round((saterror) / (satin + satout + Math.Abs(satds)) * 2 * 100, 2);
   
            ppt.Text = e[FileMonitor.PPT].ToString();
            ds.Text = e[FileMonitor.Total_Storage_Change].ToString();
            et.Text = (e[FileMonitor.Evapotranspiration] + e[FileMonitor.Evaporation]).ToString();
            sw_in.Text = e[FileMonitor.Streams_Inflow].ToString();
            sw_out.Text = e[FileMonitor.Streams_Outflow].ToString();
            sat_in.Text = e[FileMonitor.Groundwater_Inflow].ToString();
            sat_out.Text = e[FileMonitor.Groundwater_Outflow].ToString();

            sf_et.Text = surface_et.ToString("0.0");
            sw_ds.Text = (e[FileMonitor.LAND_SURFACE_Zone_DS]).ToString("0.00");

            sfr_ds.Text = "0.00";
            sfr_et.Text = (e[FileMonitor.SFRET]).ToString("0.00");
            sfr_slow.Text = (e[FileMonitor.BASININTERFLOW]).ToString("0.0");
            sfr_dun.Text = (e[FileMonitor.BASINSROFF]).ToString("0.0");

            lak_et.Text = e[FileMonitor.LAKET].ToString("0.00");
            lak_ds.Text = e[FileMonitor.Lakes_Zone_DS].ToString("0.00");
            lak_slow.Text = (e[FileMonitor.BASINLAKEINSZ]).ToString("0.0");
            lak_dun.Text = (e[FileMonitor.BASINHORTONIANLAKES]).ToString("0.0");

            canal_et.Text = e[FileMonitor.CANAL_ET].ToString("0.00");
            canal_ds.Text = (e[FileMonitor.Canal_DS]).ToString("0.00");

            div.Text = e[FileMonitor.IR_DIV].ToString("0.00");
            sat_pr.Text = e[FileMonitor.IR_PUMP].ToString("0.00");

            sz_Percolation.Text = e[FileMonitor.UZF_INFIL].ToString("0.00");
            sz_et.Text = e[FileMonitor.BASINPERVET_HRU].ToString("0.00");
            sz_ds.Text = e[FileMonitor.Soil_Zone_DS].ToString("0.00");           

            uzf_ds.Text = e[FileMonitor.Unsaturated_Zone_DS].ToString("0.00");
            uzf_et.Text = e[FileMonitor.UZF_ET].ToString("0.00");
            uzf_recharge.Text = e[FileMonitor.UZF_RECHARGE].ToString("0.00");

            sat_gw2sz.Text = e[FileMonitor.SURFACE_LEAKAGE_OUT].ToString("0.00");
            sat_s2g.Text = e[FileMonitor.STREAM_LEAKAGE_IN].ToString("0.00");
            sat_et.Text = e[FileMonitor.GW_ET_OUT].ToString("0.00");
            sat_g2s.Text = e[FileMonitor.STREAM_LEAKAGE_OUT].ToString("0.00");
            sat_ds.Text = e[FileMonitor.Saturated_Zone_DS].ToString("0.00");

            total_error.Text = totaldisp.ToString("0.00") + "%";
            soil_error.Text = soildisp.ToString("0.00") + "%";
            sat_error.Text = sat_discrepancy.ToString("0.00") + "%";
            uz_error.Text = uzf_discrepancy.ToString("0.00") + "%";
        }
    }
}