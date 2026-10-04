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
    public partial class JunctionParaForm : Form
    {
        private ParameterPackage _ParaPackage;
        private SourceInfo _SelectedSourceInfo;

        public JunctionParaForm(ParameterPackage pck)
        {
            InitializeComponent();
            _ParaPackage = pck;
            btnRemove.Enabled = false;
        }

        private void JunctionParaForm_Load(object sender, EventArgs e)
        {
            olvTributaryPara.DataSource = _ParaPackage.Tributaries;
            olvPointSource.DataSource = _ParaPackage.PointSources;
            olvSections.DataSource = _ParaPackage.LL;
            numericUpDownSection.ValueChanged -= this.numericUpDownSection_ValueChanged;
            numericUpDownSection.Value = _ParaPackage.MD;
            numericUpDownSection.ValueChanged += this.numericUpDownSection_ValueChanged;
        }

        private void olvTributaryPara_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (olvTributaryPara.SelectedObject != null)
            {
                btnRemove.Enabled = true;
                _SelectedSourceInfo = olvTributaryPara.SelectedObject as SourceInfo;
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }
        private void olvPointSource_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (olvPointSource.SelectedObject != null)
            {
                btnRemove.Enabled = true;
                _SelectedSourceInfo = olvPointSource.SelectedObject as SourceInfo;
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {

            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                _ParaPackage.AddSourceInfoTo(_ParaPackage.Tributaries);
                olvTributaryPara.DataSource = _ParaPackage.Tributaries;
                _ParaPackage.NUMBR = _ParaPackage.Tributaries.Count;
            }
            else if (tabControl1.SelectedTab == tabPage3)
            {
                _ParaPackage.AddSourceInfoTo(_ParaPackage.PointSources);
                olvPointSource.DataSource = _ParaPackage.PointSources;
                _ParaPackage.INODE = _ParaPackage.PointSources.Count;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (_SelectedSourceInfo == null)
                return;
            if (tabControl1.SelectedTab == tabPage1)
            {

            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                _ParaPackage.RemoveSourceInfoFrom(_SelectedSourceInfo, _ParaPackage.Tributaries);
                olvTributaryPara.DataSource = _ParaPackage.Tributaries;
                _ParaPackage.NUMBR = _ParaPackage.Tributaries.Count;
            }
            else if (tabControl1.SelectedTab == tabPage3)
            {
                _ParaPackage.RemoveSourceInfoFrom(_SelectedSourceInfo, _ParaPackage.PointSources);
                olvPointSource.DataSource = _ParaPackage.PointSources;
                _ParaPackage.INODE = _ParaPackage.PointSources.Count;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedSourceInfo = null;
            if (tabControl1.SelectedTab == tabPage1)
            {
                btnRemove.Enabled = false;
                btnAdd.Enabled = false;
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                btnRemove.Enabled = false;
                btnAdd.Enabled = true;
            }
            else if (tabControl1.SelectedTab == tabPage3)
            {
                btnRemove.Enabled = false;
                btnAdd.Enabled = true;
            }
        }

        private void numericUpDownSection_ValueChanged(object sender, EventArgs e)
        {
            _ParaPackage.LL.Clear();
            for (int i = 0; i < numericUpDownSection.Value + 1; i++)
                _ParaPackage.AddSourceInfoTo(_ParaPackage.LL);
            olvSections.DataSource = _ParaPackage.LL;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _ParaPackage.IsDirty = true;
          
            _ParaPackage.Save(null);
        }
    }
}
