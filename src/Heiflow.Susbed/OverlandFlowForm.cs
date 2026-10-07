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
using Heiflow.Controls.WinForm.Controls;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public partial class OverlandFlowForm : Form
    {
        private SWMMPackage _SMWWPackage;
        public OverlandFlowForm(SWMMPackage pck)
        {
            InitializeComponent();
            _SMWWPackage = pck;
            this.Load += OverlandFlowForm_Load;
        }

      private   void OverlandFlowForm_Load(object sender, EventArgs e)
        {
            NativeList.Bind(olvTributaryPara, _SMWWPackage.SubCachements);
        }

      private void btnSave_Click(object sender, EventArgs e)
      {

      }
    }
}
