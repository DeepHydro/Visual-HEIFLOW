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
    public partial class SedGradationForm : Form
    {
        private SedGradationPackage _Package;
        public SedGradationForm(SedGradationPackage pck)
        {
            InitializeComponent();
            _Package = pck;
        }

        private void FormSedGradation_Load(object sender, EventArgs e)
        {
            if (_Package.Load(null) == LoadingState.Normal)
            {
                string str = string.Join("  ", _Package.Gradations);
                lbState.Text = "级配: " + str;
                toolStripComboBox1.SelectedIndex = 0;
            }
            else
            {
                string msg = string.Format("读取{0}失败。错误信息：", _Package.FileName);
                MessageBox.Show(msg, "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dt = _Package.ToDataTable(toolStripComboBox1.SelectedIndex);
            bindingSource1.DataSource = dt;
            dataGridView1.DataSource = bindingSource1;
        }
    }
}
