// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Hydrodynamics.Susbed.SWMM;
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
    public partial class RunoffDataForm : Form
    {
        private SWMMPackage _SMWWPackage;
        public RunoffDataForm(SWMMPackage pck)
        {
            InitializeComponent();
            _SMWWPackage = pck;   
            for (int i = 1; i <= _SMWWPackage.NumOfSubCatVar; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }
            this.Load += RunoffDataForm_load;
        }

        private void RunoffDataForm_load(object sender, EventArgs e)
        {
            _SMWWPackage.LoadRunoff();
            foreach (var cat in _SMWWPackage.SubCachements)
            {
                TreeNode node = new TreeNode(cat.Name);
                node.Tag = cat;
                treeView1.Nodes[0].Nodes.Add(node);

            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var cat = e.Node.Tag as SubCachement;
            if (cat != null && cat.TimeSeries != null)
            {
                var ts = cat.TimeSeries;
               
                bindingSource1.DataSource = ts.ToDataTable("产流量");
                dataGridView1.DataSource = bindingSource1;

                winChartSection.Plot<double>(ts.DateTimes, ts.Values,
                    cat.Name + "产流过程", System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine, "时间 (天)", "产流量 (立方米/秒)"); 
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SMWWPackage.RunoffIndex = comboBox1.SelectedIndex;
            _SMWWPackage.LoadRunoff();
        }
    }
}
