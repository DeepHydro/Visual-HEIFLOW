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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Applications.Views;
using System.ComponentModel.Composition;
using Heiflow.Applications.ViewModels;
using System.Waf.Applications;
using System.Windows.Forms.DataVisualization.Charting;
using Heiflow.Models.Running;
using Heiflow.Models.UI;
using Heiflow.Applications;

namespace Heiflow.Controls.WinForm.Display
{
    [Export(typeof(IRunningMonitorView))]
    public partial class RunningMonitor : Form, IRunningMonitorView, IChildView
    {
        private  RunningMonitorViewModel viewModel;
        private int _line_count;
        private Series _Series;
        public static string _ChildName = "RunningMonitorView";
        private string[] filtedstrs;

        public RunningMonitor()
        {
            InitializeComponent();
            richTextBox1.HideSelection = false;
            this.Load += RunningMonitor_Load;
            this.FormClosing += this.RunningMonitor_FormClosing;
            progressBar1.Maximum = 365;
            _line_count = 0;
            progressBar1.Visible = false;
            filtedstrs = new string[]
            {
                "SEAWAT",                "U.S. GEOLOGICAL",                                "Version",                                "This program is",
                "condition",  "the United","damages resulting","use.", "PHREEQC "
            };
        }

        public object DataContext
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return _ChildName; }
        }


        private void RunningMonitor_Load(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Title = "Date";
            chart1.ChartAreas[0].AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Days;
            chart1.ChartAreas[0].AxisY.Title = "Percent Discrepancy (%)";
            _Series = chart1.Series[0];
            _Series.ChartType = SeriesChartType.FastLine;
        }



        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
            {
                this.Show(pararent);
       
            }
        }
        private void RunningMonitor_FormClosing(object sender, FormClosingEventArgs e)
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

        private void viewModel_ProgressChanged(object sender, int e)
        {
            this.Invoke((MethodInvoker)delegate
          {
              progressBar1.Value = e;
              UpdateView();
          });
        }

        private void viewModel_MessageReported(object sender, string e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                bool found = false;
                foreach(var str in filtedstrs)
                {
                    if(e.Contains(str))
                    {
                        e = "";
                        found = true;
                        break;
                    }
                }
                if (found) return;
                _line_count++;
                if (_line_count > 500)
                {
                    richTextBox1.Text = e;
                    _line_count = 0;
                }
                else
                {
                    richTextBox1.AppendText(e + "\n");
                }
            });
        }
        private void Monitor_RunningStarted(object sender, EventArgs e)
        {
            Reset();
        }
        private void Monitor_RunningFinished(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
        }

        public void Reset()
        {
            if (viewModel.ProjectService.Project != null && viewModel.ProjectService.Project.Model != null)
            {
                var ts = viewModel.ProjectService.Project.Model.TimeService;
                progressBar1.Visible = true;
                progressBar1.Maximum = ts.NumTimeStep;
                progressBar1.Value = 0;
                //var datasource = viewModel.StateMonitor.Monitor.DataSource;
                //var dates = datasource.Dates;
                if (_Series != null)
                    _Series.Points.Clear();
                _line_count = 0;
                this.labelInfo.Text = string.Format("Current date: {0};   Total elapsed time: {1} (day)",ts.Start, 0);
                richTextBox1.Clear();
                chart1.Update();
            }
        }

        public void UpdateView()
        {
            this.Invoke((MethodInvoker)delegate
            {
                var datasource = viewModel.StateMonitor.Monitor.DataSource;
                var dates = datasource.Dates;
                if (dates.Count == 0)
                    return;
                var step = dates.Count - 1;
                int var_index = datasource.Values.Length - 1;
                double y = datasource.Values[var_index][step];
                _Series.Points.AddXY(dates[step], y);
                this.chart1.Update();
                this.labelInfo.Text = string.Format("Current date: {0};   Total elapsed time: {1} (day)", dates[step], step + 1);
            });
        }

        public void ClearContent()
        {
            Reset();
        }
        public void InitService()
        {
            viewModel = MyAppManager.Instance.CompositionContainer.GetExportedValue<RunningMonitorViewModel>();
            viewModel.RunningStarted += Monitor_RunningStarted;
            viewModel.RunningFinished += Monitor_RunningFinished;
            viewModel.MessageReported += viewModel_MessageReported;
            viewModel.ProgressChanged += viewModel_ProgressChanged;
        }
    }
}
