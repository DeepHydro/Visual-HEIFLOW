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
    public partial class ClimateDataForm : Form
    {
        private SWMMPackage _SMWWPackage;
        public ClimateDataForm(SWMMPackage pck)
        {
            InitializeComponent();
            _SMWWPackage = pck;
            this.Load += ClimateDataForm_Load;
        }

        private void ClimateDataForm_Load(object sender, EventArgs e)
        {
            foreach (var rain in _SMWWPackage.RainGages)
            {
                TreeNode node = new TreeNode(rain.Name);
                node.Tag = rain;
                treeView1.Nodes[0].Nodes.Add(node);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var rain = e.Node.Tag as RainGage;
            if (rain != null)
            {
                rain.Load();
                var ts = rain.TimeSeries;
                bindingSource1.DataSource = ts.ToDataTable("降雨量");
                dataGridView1.DataSource = bindingSource1;

                winChartSection.Plot<double>(ts.DateTimes, ts.Values,
                    "降雨量", System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column,  "时间 (天)", "降雨量 (毫米)");
            }
        }
    }
}
