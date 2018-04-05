using Heiflow.Applications;
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Core.Data;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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

namespace Heiflow.Controls.WinForm.Display
{
      [Export(typeof(ISurfacePlotView))]
    public partial class Win3DView : Form, ISurfacePlotView
    {

        public Win3DView()
        {
            InitializeComponent();
            this.FormClosing += Win3DView_FormClosing;
        }
        public View3DControl View3DControl
        {
            get
            {
                return view3DControl1;
            }
        }
        public string ChildName
        {
            get { return ChildWindowNames.Win3DView; }
        }

        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
                this.Show(pararent);
        }
       private void Win3DView_FormClosing(object sender, FormClosingEventArgs e)
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

       public void PlotSurface(ILArray<float> array)
       {
           view3DControl1.PlotSurface(array);
           this.Text = string.Format("3D View - {0}", array.Name);
       }

       public void ClearContent()
       {
           view3DControl1.ClearContent();
       }
       public void InitService()
       {

       }
    }
}
