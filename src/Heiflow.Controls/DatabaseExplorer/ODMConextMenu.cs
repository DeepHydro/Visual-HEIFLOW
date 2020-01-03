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

using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.IO;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.DatabaseExplorer
{
    public class ODMConextMenu
    {
        private ContextMenuStrip _ContextMenuStrip;
        private Dictionary<string, ToolStripItem> _Dict_Items;
        private IShellService _Shell;
    //    public static string _DeleteString = "Delete";
        public static string _ExportShpString = "Export Site List...";
        public static string _ExportExcelString = "Export Time Series...";
        public static string _PlotTSString = "Plot Time Series...";
        public static string _PropertyString = "Property";

        public ODMConextMenu()
        {
            _ContextMenuStrip = new ContextMenuStrip();
            _Dict_Items = new Dictionary<string, ToolStripItem>();
            var item = _ContextMenuStrip.Items.Add(_PlotTSString, Resources.Chart, Plot);
            _Dict_Items.Add(_PlotTSString, item);
            item = _ContextMenuStrip.Items.Add(_ExportExcelString, Resources.excel_32, Export2Excel_Clicked);
            _Dict_Items.Add(_ExportExcelString, item);
            item = _ContextMenuStrip.Items.Add(_ExportShpString, null, Export2Shp_Clicked);
            _Dict_Items.Add(_ExportShpString, item);
            //item = _ContextMenuStrip.Items.Add(_DeleteString, null, Delete_Clicked);
            //_Dict_Items.Add(_DeleteString, item);
            ToolStripSeparator sepa = new ToolStripSeparator();
            _ContextMenuStrip.Items.Add(sepa);
            item = _ContextMenuStrip.Items.Add(_PropertyString, Resources.MetadataProperties16, Property_Clicked);
            _Dict_Items.Add(_PropertyString, item);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return _ContextMenuStrip;
            }
        }

        public object DataContext
        {
            get;
            set;
        }

        public ODMSource ODMSource
        {
            get;
            set;
        }

        public IShellService ShellService
        {
            get
            {
                return _Shell;
            }
            set
            {
                _Shell = value;
            }
        }

        public void Enable(object sender)
        {
            EnableAll(true);
            if (sender is ODMSource)
            {
               // Enable(_DeleteString, false);
            }
            else
            {
                var re = sender as IDendritiRecord<ObservationSeries>;
                if (re == null)
                    return;
                if (re.Children.Count == 0)
                {   
                    Enable(_ExportExcelString, true);
                    Enable(_PlotTSString, true);
                    Enable(_ExportShpString, false);
                }
                else
                {
                    Enable(_ExportExcelString, false);
                    Enable(_PlotTSString, false);
                    Enable(_ExportShpString, true);
                }
            }
        }

        public void Enable(string key, bool enabled)
        {
            _Dict_Items[key].Enabled = enabled;
        }

        public void EnableAll(bool enabled)
        {
            foreach (var key in _Dict_Items.Keys)
                _Dict_Items[key].Enabled = enabled;
        }

        private void Refresh_Clicked(object sender, EventArgs e)
        {

        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            var dc = DataContext as IDendritiRecord<ObservationSeries>;
            if (dc != null)
            {
                if (dc.Tag is Site)
                {

                }
            }
        }
        private void Property_Clicked(object sender, EventArgs e)
        {
            _Shell.PropertyView.SelectedObject = this.DataContext;
            _Shell.SelectPanel("kPropGrid");
        }
        //TODO

        private void Export2Excel_Clicked(object sender, EventArgs e)
        {
            var dc = DataContext as IDendritiRecord<ObservationSeries>;
            if (dc != null && dc.Children.Count ==0)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "csv file|*.csv";
                dlg.FileName = dc.Name + ".csv";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var qc = new QueryCriteria()
                    {
                        SiteID = dc.Value.SiteID,
                        VariableID = dc.Value.VariableID,
                        AllTime = true
                    };
                    var ts = ODMSource.GetTimeSeries(qc);
                    if (ts != null)
                    {
                        CSVFileStream csvf = new CSVFileStream(dlg.FileName);
                        csvf.Save(ts);
                    }
                }

            }
        }

        private void Plot(object sender, EventArgs e)
        {
            var dc = DataContext as IDendritiRecord<ObservationSeries>;
            if (dc != null && dc.Children.Count == 0)
            {
                var qc = new QueryCriteria()
                {
                    SiteID = dc.Value.SiteID,
                    VariableID = dc.Value.VariableID,
                    AllTime = true
                };
                var ts = ODMSource.GetTimeSeries(qc);
                if (ts != null)
                {
                    ShellService.ShowChildWindow(ChildWindowNames.WinChartView);
                    ShellService.WinChart.Plot(ts.DateTimes, ts[0, ":", "0"], qc.VariableName,
                        System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine);
                }
            }
        }

        //TODO

        private void Export2Shp_Clicked(object sender, EventArgs e)
        {
            var dc = DataContext as IDendritiRecord<ObservationSeries>;
            if (dc != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "csv file|*.csv";
                dlg.FileName = dc.Name + ".csv";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dc.Tag is Heiflow.Core.Data.ODM.Variable)
                    {
                        CSVFileStream csv = new CSVFileStream(dlg.FileName);
                        string content = "SiteName,SiteID,Longitude,Latitude,Max,Min,Average,NoDataValue,StandardDeviation,Count\n";
                        foreach (var record in dc.Children)
                        {
                            var site = record.Value.Site;
                            var statinfo = ODMSource.GetValueStatistics(record.Value);
                            content += string.Format("{0},{1},{2},{3},{4}\n", site.Name, site.ID, site.Longitude, site.Latitude, statinfo.ToString());
                        }
                        content.Trim(TypeConverterEx.Enter);
                        csv.Save(content);
                    }
                }
            }
        }
    }
}
