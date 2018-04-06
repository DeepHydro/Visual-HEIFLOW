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
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel.Composition;
using Heiflow.Presentation.Controls;
using Heiflow.Applications;
using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using Heiflow.Presentation.Services;

namespace Heiflow.Controls.WinForm.Controls
{
    public partial class WinChart : UserControl
    {

        private bool _ClearExist = true;
        private List<double> _MaxList = new List<double>();
        private List<double> _MinList = new List<double>();
        private MessageService _MessageService = new MessageService();
        private string _selectedMenuItem = "";
        private IShellService _ShellService;
        private bool _ShowStatPanel = true;

        public WinChart()
        {
            InitializeComponent();
            this.Load += WinChart_Load;
        }

        public ToolStrip ToolStrip
        {
            get
            {
                return toolStrip1;
            }
        }

        public bool ShowStatPanel
        {
            get
            {
                return _ShowStatPanel;
            }
            set
            {
                _ShowStatPanel = value;
            }
        }

        private void WinChart_Load(object sender, EventArgs e)
        {
            checkedListBox1.DisplayMember = "Name";
            btnLengend_Click(null, null);
            Plot(new DateTime[] { DateTime.Now }, new double[] { 10 });
            Clear();
            if (!ShowStatPanel)
            {
                btnStatPanel.Visible = false;
                btnStat.Visible = false;
                splitContainerChart.Panel2Collapsed = true;
            }
        }
        private void btnLengend_Click(object sender, EventArgs e)
        {
            splitContainerMain.Panel2Collapsed = !splitContainerMain.Panel2Collapsed;
            btnLengend.Checked = !splitContainerMain.Panel1Collapsed;
        }

        private void btnStatPanel_Click(object sender, EventArgs e)
        {
            splitContainerChart.Panel2Collapsed = !splitContainerChart.Panel2Collapsed;
            btnStatPanel.Checked = !splitContainerChart.Panel1Collapsed;
        }

        private void btnClearExist_Click(object sender, EventArgs e)
        {
            _ClearExist = !_ClearExist;
            btnClearExist.Checked = _ClearExist;
        }



        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnZoomFull_Click(object sender, EventArgs e)
        {
            SetScale();
            chart1.ChartAreas["Default"].AxisX.ScaleView.ZoomReset(0);
            chart1.ChartAreas["Default"].AxisY.ScaleView.ZoomReset(0);
        }

        private void btnStat_Click(object sender, EventArgs e)
        {
            if (chart1.Series.Count != 2)
            {
                _MessageService.ShowWarning(this, "Two series are required!");
            }
            else
            {
                var xx = (from pt in chart1.Series[0].Points select pt.YValues[0]).ToArray();
                var yy = (from pt in chart1.Series[1].Points select pt.YValues[0]).ToArray();
                if (xx.Length != yy.Length)
                    return;
                lvStatistics.Items[0].SubItems[3].Text = MyStatisticsMath.Mse(xx, yy).ToString("F3");
                lvStatistics.Items[1].SubItems[3].Text = MyStatisticsMath.RMse(xx, yy).ToString("F3");
                lvStatistics.Items[2].SubItems[3].Text = MyStatisticsMath.Correlation(xx, yy).ToString("F3");
                lvStatistics.Items[3].SubItems[3].Text = MyStatisticsMath.NashStucliffeR(xx, yy).ToString("F3");
                // lvStatistics.Items[4].SubItems[3].Text = MyStatisticsMath.GetSimilarityScore(xx, yy).ToString("F3");
            }
        }

        private void menu_scatter_Click(object sender, EventArgs e)
        {
            if (chart1.Series.Count != 2)
            {
                _MessageService.ShowWarning(this, "Two series are required!");
            }
            else
            {
                var xx = from pt in chart1.Series[0].Points select pt.YValues[0];
                var yy = from pt in chart1.Series[1].Points select pt.YValues[0];
                string series = string.Format("{0} VS. {1}", chart1.Series[0].Name, chart1.Series[1].Name);
                Clear();
                Plot<double>(xx.ToArray(), yy.ToArray(), "", SeriesChartType.FastPoint);
            }
        }
        private void menu_line_Click(object sender, EventArgs e)
        {

        }



        private void checkedListBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = checkedListBox1.IndexFromPoint(e.Location);
            checkedListBox1.SelectedIndex = index;
            if (index != ListBox.NoMatches)
            {
                _selectedMenuItem = (checkedListBox1.Items[index] as Series).Name;
                contextMenuStripItems.Enabled = true;
                contextMenuStripItems.Visible = true;
            }
            else
            {
                contextMenuStripItems.Enabled = false;
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var series = checkedListBox1.Items[e.Index] as Series;
            if (series != null)
                series.Enabled = e.NewValue == CheckState.Checked;
        }

        private void zoomToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var series = SelectSeries(_selectedMenuItem);
            if (series != null && series.Tag != null)
            {
                var st = series.Tag as StatisticsInfo;
                SetScale(st.Max, st.Min);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var series = SelectSeries(_selectedMenuItem);
            if (series != null)
            {
                chart1.Series.Remove(series);
                checkedListBox1.Items.Remove(series);
                series = null;
            }
        }

        private void styleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var series = SelectSeries(_selectedMenuItem);
            if (series != null)
            {
                this.tabControlRight.SelectedTab = this.tabPageProperty;
                propertyGrid1.SelectedObject = series;
            }
        }


        private void chartPropertyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tabControlRight.SelectedTab = this.tabPageProperty;
            propertyGrid1.SelectedObject = this.chart1;
        }



        #region Plot
        public void Plot<T>(DateTime[] xx, T[] yy, string seriesName = "", SeriesChartType chartType = SeriesChartType.FastLine, string axisXTitle = "", string axisYTitle = "")
        {
            if (xx == null || yy == null || xx.Length == 0 || yy.Length == 0)
                return;

            if (!this.IsDisposed)
            {
                if (btnClearExist.Checked)
                {
                    Clear();
                }
                bool existed = false;
                chart1.ChartAreas[0].AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Days;

                var series1 = GetLegalSeries(seriesName, ref existed);
                series1.ChartType = chartType;

                var len = Math.Min(xx.Length, yy.Length);
                for (int i = 0; i < len; i++)
                {
                    series1.Points.AddXY(xx[i], yy[i]);
                }
                if (existed)
                {
                    chart1.Update();
                }
                else
                {
                    chart1.Series.Add(series1);
                    checkedListBox1.Items.Add(series1, true);
                }
                chart1.ChartAreas[0].AxisX.Title = axisXTitle;
                chart1.ChartAreas[0].AxisY.Title = axisYTitle;
                var max = double.Parse(yy.Max().ToString());
                var min = double.Parse(yy.Min().ToString());
                _MaxList.Add(max);
                _MinList.Add(min);
                series1.Tag = new StatisticsInfo() { Max = max, Min = min };
                SetScale();
                btnZoomFull_Click(null, null);
            }
        }

        public void Plot<T>(T[] yy, string seriesName = "", SeriesChartType chartType = SeriesChartType.FastLine,string axisXTitle="",string axisYTitle="")
        {
            if (yy == null || yy.Length == 0)
                return;
            if (!this.IsDisposed)
            {
                if (btnClearExist.Checked)
                {
                    Clear();
                }

                chart1.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Auto;
                bool existed = false;
                var series1 = GetLegalSeries(seriesName, ref existed);
                series1.ChartType = chartType;
                for (int i = 0; i < yy.Length; i++)
                {
                    series1.Points.AddY(yy[i]);
                }
                if (existed)
                {
                    chart1.Update();
                }
                else
                {
                    chart1.Series.Add(series1);
                    checkedListBox1.Items.Add(series1, true);
                }
                chart1.ChartAreas[0].AxisX.Title = axisXTitle;
                chart1.ChartAreas[0].AxisY.Title = axisYTitle;
                var max = double.Parse(yy.Max().ToString());
                var min = double.Parse(yy.Min().ToString());
                _MaxList.Add(max);
                _MinList.Add(min);
                series1.Tag = new StatisticsInfo() { Max = max, Min = min };
                SetScale();
                btnZoomFull_Click(null, null);
            }
        }

        public void Plot<T>(T[] xx, T[] yy, string seriesName = "", SeriesChartType chartType = SeriesChartType.FastLine, 
            string axisXTitle="",string axisYTitle="" )
        {
            if (xx == null || yy == null || xx.Length == 0 || yy.Length == 0)
                return;
            if (!this.IsDisposed)
            {
                if (btnClearExist.Checked)
                {
                    Clear();
                }
                bool existed = false;
                var series1 = GetLegalSeries(seriesName, ref existed);
                series1.ChartType = chartType;
                var len = Math.Min(xx.Length, yy.Length);
                for (int i = 0; i < len; i++)
                {
                    series1.Points.AddXY(xx[i], yy[i]);
                }
                if (existed)
                {
                    chart1.Update();
                }
                else
                {
                    chart1.Series.Add(series1);
                    checkedListBox1.Items.Add(series1, true);
                }
                chart1.ChartAreas[0].AxisX.Title = axisXTitle;
                chart1.ChartAreas[0].AxisY.Title = axisYTitle;
                var max = double.Parse(yy.Max().ToString());
                var min = double.Parse(yy.Min().ToString());
                _MaxList.Add(max);
                _MinList.Add(min);
                series1.Tag = new StatisticsInfo() { Max = max, Min = min };
                SetScale();
                btnZoomFull_Click(null, null);
            }
        }

        public void Plot<T>(T[] xx, T[] yy, T nodatavalue, string seriesName = "", SeriesChartType chartType = SeriesChartType.FastLine)
        {
            if (xx == null || yy == null || xx.Length == 0 || yy.Length == 0)
                return;
            if (!this.IsDisposed)
            {
                if (btnClearExist.Checked)
                {
                    Clear();
                }
                bool existed = false;
                var series1 = GetLegalSeries(seriesName, ref existed);
                series1.ChartType = chartType;
                var len = Math.Min(xx.Length, yy.Length);
                for (int i = 0; i < len; i++)
                {
                    int c1 = Comparer<T>.Default.Compare(xx[i], nodatavalue);
                    int c2 = Comparer<T>.Default.Compare(yy[i], nodatavalue);
                    if (c1 != 0 && c2 != 0)
                        series1.Points.AddXY(xx[i], yy[i]);
                }
                if (existed)
                {
                    chart1.Update();
                }
                else
                {
                    chart1.Series.Add(series1);
                    checkedListBox1.Items.Add(series1, true);
                }

                var max = double.Parse(yy.Max().ToString());
                var min = double.Parse(yy.Min().ToString());
                _MaxList.Add(max);
                _MinList.Add(min);
                series1.Tag = new StatisticsInfo() { Max = max, Min = min };
                SetScale();
                btnZoomFull_Click(null, null);
            }
        }

        public void Plot<T>(ITimeSeries<T> source, SeriesChartType chartType)
        {
            if (source == null)
                return;
            if (!this.IsDisposed)
            {
                if (btnClearExist.Checked)
                {
                    Clear();
                }

                chart1.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Auto;

                if (source is My3DMat<T>)
                {
                    var mat = source as My3DMat<T>;
                    int nvar = mat.Size[0];
                    int nsite = mat.Size[2];
                    for (int i = 0; i < nvar; i++)
                    {
                        for (int n = 0; n < nsite; n++)
                        {
                            var vec = mat.GetSeriesAt(i, n);
                            bool existed = false;
                            var series1 = GetLegalSeries(mat.Variables[i] + (n + 1), ref existed);
                            series1.ChartType = chartType;
                            AddDateSeries<T>(series1, existed, mat.DateTimes, vec);
                            var max = double.Parse(vec.Max().ToString());
                            var min = double.Parse(vec.Min().ToString());
                            _MaxList.Add(max);
                            _MinList.Add(min);
                            series1.Tag = new StatisticsInfo() { Max = max, Min = min };
                        }
                    }
                }
                SetScale();
                btnZoomFull_Click(null, null);
            }
        }

        public void Plot<T>(ITimeSeries<T> source, int var_index, SeriesChartType chartType)
        {
            if (source == null)
                return;
            if (!this.IsDisposed)
            {
                if (btnClearExist.Checked)
                {
                    Clear();
                }

                chart1.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Auto;

                if (source is My3DMat<T>)
                {
                    var mat = source as My3DMat<T>;
                    int nsite = mat.Size[2];

                    for (int n = 0; n < nsite; n++)
                    {
                        var vec = mat.GetSeriesAt(var_index, n);
                        bool existed = false;
                        var series1 = GetLegalSeries(mat.Variables[var_index] + (n + 1), ref existed);
                        series1.ChartType = chartType;
                        AddDateSeries<T>(series1, existed, mat.DateTimes, vec);
                        var max = double.Parse(vec.Max().ToString());
                        var min = double.Parse(vec.Min().ToString());
                        _MaxList.Add(max);
                        _MinList.Add(min);
                        series1.Tag = new StatisticsInfo() { Max = max, Min = min };
                    }

                    SetScale();
                    btnZoomFull_Click(null, null);
                }
            }
        }

        private void AddDateSeries<T>(Series series1, bool existed, DateTime[] dates, T[] values)
        {
            for (int i = 0; i < dates.Length; i++)
            {
                series1.Points.AddXY(dates[i], values[i]);
            }
            if (existed)
            {
                chart1.Update();
            }
            else
            {
                chart1.Series.Add(series1);
                checkedListBox1.Items.Add(series1, true);
            }
            _MaxList.Add(double.Parse(values.Max().ToString()));
            _MinList.Add(double.Parse(values.Min().ToString()));
        }
        #endregion

        private Series GetLegalSeries(string sname, ref bool existed)
        {
            if (TypeConverterEx.IsNull(sname))
                sname = "default";
            var names = from se in chart1.Series select se.Name;
            if (names.Contains(sname))
            {
                var buf = from se in chart1.Series where se.Name == sname select se;
                existed = true;
                var series = buf.First();
                series.Points.Clear();
                return series;
            }
            else
            {
                existed = false;
                return new Series()
                {
                    Name = sname
                };
            }
        }

        private Series SelectSeries(string name)
        {
            var buf = from se in chart1.Series where se.Name == name select se;
            if (buf.Count() > 0)
                return buf.FirstOrDefault();
            else
                return null;
        }

        public void Clear()
        {
            if (!this.IsDisposed)
            {
                _MaxList.Clear();
                _MinList.Clear();
                chart1.Series.Clear();
                checkedListBox1.Items.Clear();
            }
        }

        private void SetScale(double max, double min)
        {
            var inteval = max - min;
            double newmax = 1;
            double newmin = 0;
            int decimals = 0;
            if (inteval > 1)
            {
                decimals = 0;
            }
            else if (inteval < 1 && inteval > 0.1)
            {
                decimals = 1;
            }
            else if (inteval < 0.1 && inteval > 0.01)
            {
                decimals = 2;
            }
            else if (inteval < 0.01 && inteval > 0.001)
            {
                decimals = 3;
            }
            else
            {
                decimals = 4;
            }
            newmax = Math.Round(max + inteval * 0.15, decimals);
            newmin = Math.Round(min - inteval * 0.15, decimals);

            if (newmin == newmax)
                newmax = newmin + 1;

            if (min >= 0 && newmin < 0)
                newmin = 0;

            chart1.ChartAreas["Default"].AxisY.Maximum = newmax;
            chart1.ChartAreas["Default"].AxisY.Minimum = newmin;
        }

        private void SetScale()
        {
            if (_MaxList.Count > 0)
            {
                var max = _MaxList.Max();
                var min = _MinList.Min();
                SetScale(max, min);
            }
            else
            {
                chart1.ChartAreas["Default"].AxisY.Minimum = 100;
                chart1.ChartAreas["Default"].AxisY.Maximum = 0;
            }
        }

        private void viewDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var series = SelectSeries(_selectedMenuItem);
            if (series != null)
            {
                int numy = series.Points[0].YValues.Length;
                if (_ShellService == null)
                    _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();

                DataTable dt = new DataTable();
                bool isDate = false;
                if (series.XValueType == ChartValueType.Date || series.XValueType == ChartValueType.DateTime)
                {
                    DataColumn dc = new DataColumn("Date", typeof(DateTime));
                    dt.Columns.Add(dc);
                    isDate = true;
                }
                else
                {
                    DataColumn dc = new DataColumn("XValue", typeof(double));
                    dt.Columns.Add(dc);
                    isDate = false;
                }

                for (int i = 0; i < numy; i++)
                {
                    DataColumn dc1 = new DataColumn("YValue " + (i + 1), typeof(double));
                    dt.Columns.Add(dc1);
                }
                if (isDate)
                {
                    for (int i = 0; i < series.Points.Count; i++)
                    {
                        var dr = dt.NewRow();
                        dr[0] = DateTime.FromOADate(series.Points[i].XValue);
                        for (int j = 0; j < numy; j++)
                        {
                            dr[j + 1] = series.Points[i].YValues[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    for (int i = 0; i < series.Points.Count; i++)
                    {
                        var dr = dt.NewRow();
                        dr[0] = series.Points[i].XValue;
                        for (int j = 0; j < numy; j++)
                        {
                            dr[j + 1] = series.Points[i].YValues[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                _ShellService.DataGridView.Bind(dt);
            }
        }


        private void menu_trend_Click(object sender, EventArgs e)
        {
            var series = SelectSeries(_selectedMenuItem);
            if (series != null)
            {
            
                int npt = series.Points.Count;
                double rs, slope, yint;
                double[] xx = new double[npt];
                double[] yy = new double[npt];

                for (int i = 0; i < npt; i++)
                {
                    xx[i] = i + 1;
                    yy[i] = series.Points[i].YValues[0];
                }

                MyStatisticsMath.LinearRegression(xx, yy, 0, npt, out rs, out yint, out slope);
                string trend_name = "Trend of" + series.Name;
                lvStatistics.Items[4].SubItems[3].Text = string.Format("y = {0}x + {1}", slope.ToString("0.00"), yint.ToString("0.00"));
                double[] yy_trend = new double[2];
                if (series.XValueType == ChartValueType.Date || series.XValueType == ChartValueType.DateTime)
                {
                    DateTime[] date_trend = new DateTime[2];
                    double[] xx_trend = new double[2];
                    xx_trend[0] = 1;
                    xx_trend[1] = npt;
                    date_trend[0] = DateTime.FromOADate(series.Points[0].XValue);
                    date_trend[1] = DateTime.FromOADate(series.Points[npt - 1].XValue);
                    yy_trend[0] = slope * xx_trend[0] + yint;
                    yy_trend[1] = slope * xx_trend[1] + yint;
                    Plot<double>(date_trend, yy_trend, trend_name, SeriesChartType.Line);
                }
                else
                {
                    double[] xx_trend = new double[2];             
                    xx_trend[0] = 1;
                    xx_trend[1] = npt;
                    yy_trend[0] = slope * xx_trend[0] + yint;
                    yy_trend[1] = slope * xx_trend[1] + yint;
                    Plot<double>(xx_trend, yy_trend, trend_name, SeriesChartType.Line);               
                }
            }
        }
    }
}
