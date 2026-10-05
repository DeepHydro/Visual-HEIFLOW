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
    public partial class FlowOutViewForm : Form
    {
        private FlowOutPackage _FlowOutPackage;
        private CrossSectionPackage _CrossSectionPackage;
        private CrossSection _SelectedCrossSection;
        private ParameterPackage _ParameterPackage;
        private bool _profile_init = false;
        public FlowOutViewForm(FlowOutPackage pck, CrossSectionPackage cs_pck,ParameterPackage para_pck)
        {
            InitializeComponent();
            _FlowOutPackage = pck;
            _CrossSectionPackage = cs_pck;
            _ParameterPackage = para_pck;
        }
        private void FlowOutViewForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (_FlowOutPackage.Load(null) == LoadingState.Normal)
            {
                olvMatName.DataSource = _CrossSectionPackage.CrossSections;
                colorSlider1.Minimum = 0;
                colorSlider1.Maximum = _FlowOutPackage.NumTimes - 1;
            }
            else
            {
                string msg = string.Format("读取{0}失败。错误信息：", _FlowOutPackage.FileName);
                MessageBox.Show(msg, "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                colorSlider1.Enabled = false;
            }
            Cursor.Current = Cursors.Default;
        }
        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            winChart_proflie.Clear();
            var yy = _FlowOutPackage.Values[colorSlider1.Value];
            var ids = _ParameterPackage.MainStream.SectionIDs;
            var main_yy = new double[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                main_yy[i] = yy[ids[i] - 1];
            }
            string ytitle = "";
            if (this.Text == "水位过程")
            {
                ytitle = "水位 (米)";
            }
            else
            {
                ytitle = "流量 (立方米/秒)";
            }
            winChart_proflie.Plot<double>(main_yy, "第" + _FlowOutPackage.Times[colorSlider1.Value]+ "天洪水演进过程", 
                System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine, "断面", ytitle);
        }

        private void olvMatName_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            _SelectedCrossSection = olvMatName.SelectedObject as CrossSection;
            if (_SelectedCrossSection != null)
            {

                string ytitle = "";
                if (this.Text == "水位过程")
                {
                    ytitle = "水位 (米)";
                }
                else
                {
                    ytitle = "流量 (立方米/秒)";
                }
                var yy= _FlowOutPackage.GetValueAt(_SelectedCrossSection.ID - 1);
                winChartSection.Plot<double>(_FlowOutPackage.Times.ToArray(), yy, _SelectedCrossSection.Name,
                    System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine, "时间 (天)", ytitle);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_profile_init)
                return;
            if(tabControl1.SelectedTab== tabPageProfile)
            {
                colorSlider1.Value = 1;
                _profile_init = true;
            }
        }
    }
}