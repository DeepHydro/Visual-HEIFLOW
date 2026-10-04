// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
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
    public partial class CrossSectionForm : Form
    {
        private CrossSectionPackage _CrossSectionPackage;
        private CrossSection _SelectedCrossSection;

        public CrossSectionForm(CrossSectionPackage pck)
        {
            InitializeComponent();
            _CrossSectionPackage = pck;
        }

        private void CrossSectionForm_Load(object sender, EventArgs e)
        { 
            if (_CrossSectionPackage.Load(null) == LoadingState.FatalError)
            {
                string msg = string.Format("读取{0}失败。错误信息：", _CrossSectionPackage.FileName);
                MessageBox.Show(msg, "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                olvMatName.DataSource = _CrossSectionPackage.CrossSections;
            }
        }
        private void olvMatName_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            _SelectedCrossSection = olvMatName.SelectedObject as CrossSection;
            if (_SelectedCrossSection != null)
            {
                winChartSection.Plot<double>(_SelectedCrossSection.Distance, _SelectedCrossSection.Elevation,
                    _SelectedCrossSection.Name, System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline, "距离 (米)", "高程 (米)");
                bindingSource1.DataSource = _SelectedCrossSection.ToDataTable();
                dataGridView1.DataSource = bindingSource1;
            }
        }
    }
}
