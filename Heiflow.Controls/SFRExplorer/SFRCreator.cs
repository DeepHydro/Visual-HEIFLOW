// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.SFRExplorer
{
    [Export(typeof(IPackageOptionalView))]
    public partial class SFRCreator : Form, IPackageOptionalView
    {
        private SFRPackage _SFRPackage;

        public SFRCreator()
        {
            InitializeComponent();
            this.FormClosing += SFRCreator_FormClosing;
            
        }
        public string ChildName
        {
            get { return "SFRCreator"; }
        }
        public string PackageName
        {
            get 
            {
                return SFRPackage.PackageName;
            }
        }

        public Models.Generic.IPackage Package
        {
            get
            {
                return _SFRPackage ;
            }
            set
            {
                _SFRPackage = value as SFRPackage;
            }
        }
        public object DataContext
        {
            get;
            set;
        }

        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
                this.Show(pararent);
        }

        private void btnApplyEleOffset_Click(object sender, EventArgs e)
        {
            float offset = 0;
            float.TryParse(tbEleOffset.Text, out offset);

            _SFRPackage.CorrectElevation(offset);
            lbState.Text = "Streambed elevations are modified successfully";
        }

        private void SFRCreator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
        public void ClearContent()
        {
            
        }
        public void InitService()
        {

        }
    }
}
