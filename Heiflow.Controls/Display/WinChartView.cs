//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using Heiflow.Applications.Controllers;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation.Controls;
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
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Controls.WinForm.Display
{
    [Export(typeof(IWinChartView))]
    public partial class WinChartView : Form, IWinChartView
    {
        public WinChartView()
        {
            InitializeComponent();
            this.FormClosing += WinChartView_FormClosing;
        }

        public Heiflow.Controls.WinForm.Controls.WinChart Chart
        {
            get
            {
                return winChart1;
            }
        }
        public string ChildName
        {
            get { return ChildWindowNames.WinChartView; }
        }
        public void ShowView(IWin32Window parent)
        {
            if (!this.Visible)
            {
                this.Show(parent);
            }
        }

        private void WinChartView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                ProjectController.Instance.MapAppManager.Map.FunctionMode = DotSpatial.Controls.FunctionMode.Pan;
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
        public void CloseView()
        {
            this.Hide();
        }

        public void Plot<T>(DateTime[] xx, T[] yy, string series_name, SeriesChartType chartType)
        {
            winChart1.Plot<T>(xx, yy, series_name, chartType);
        }

        public void Plot<T>(T[] yy, string series_name, SeriesChartType chartType)
        {
            winChart1.Plot<T>(yy, series_name, chartType);
        }


        public void Plot<T>(Core.Data.ITimeSeries<T> source, SeriesChartType chartType)
        {
            winChart1.Plot<T>(source, chartType);
        }

        public void ClearContent()
        {
            winChart1.Clear();
        }
        public void InitService()
        {

        }
    }
}
