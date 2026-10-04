// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public partial class FlowSedForm : Form
    {
        private Susbed _model; 
        public FlowSedForm(Susbed model)
        {
            InitializeComponent();
            _model = model;
        }

        private void FlowSedForm_Load(object sender, EventArgs e)
        {
            var mainRoot = this.treeView1.Nodes["mainRoot"];
            var tubRoot = this.treeView1.Nodes["tributaryRoot"];
            var nodeRoot = this.treeView1.Nodes["nodeRoot"];

            var mainpck = _model.Packages["MainStreamFlowSed"] as FlowSedPackage;
            var tubpck = _model.Packages["TributraryFlowSed"] as FlowSedPackage;
            var nodepck = _model.Packages["NodeFlowSed"] as FlowSedPackage;

            TreeNode node = new TreeNode(mainpck.TableNames[0]);
            node.Tag = mainpck;
            mainRoot.Nodes.Add(node);

            for(int i=0;i<tubpck.TableNames.Length;i++)
            {
                node = new TreeNode(tubpck.TableNames[i]);
                node.Tag = tubpck;
                tubRoot.Nodes.Add(node);
            }

            for (int i = 0; i < nodepck.TableNames.Length; i++)
            {
                node = new TreeNode(nodepck.TableNames[i]);
                node.Tag = nodepck;
                nodeRoot.Nodes.Add(node);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if( e.Node.Tag != null && e.Node.Tag is FlowSedPackage)
            {
                tabControl1.SelectedTab = tabPageChart;
                var pck = e.Node.Tag as FlowSedPackage;
                var dt = pck.LoadTo(e.Node.Index);
                dt.TableName = e.Node.Text;
                bindingSource1.DataSource = dt;
                dataGridView1.DataSource = bindingSource1;
                
                var buf = from DataColumn dc in dt.Columns select dc.ColumnName;
                toolStripComboBox1.ComboBox.DataSource = buf.ToArray();
                tabPageData.Text = e.Node.Text;
                toolStripComboBox1.ComboBox.SelectedIndex = 1;
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // tabControl1.SelectedTab = tabPageChart;
            var dt = bindingSource1.DataSource as DataTable;
            if(dt != null)
            {
                var yy = from dr in dt.AsEnumerable() select dr.Field<double>(toolStripComboBox1.SelectedIndex);
                winChart1.Plot<double>(yy.ToArray(), dt.TableName, System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine,"时间 (天)","");
            }
        }
    }
}
