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

using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.IO;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using Heiflow.Controls.WinForm.Controls;

namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    public delegate bool BoolEventHandler(object sender, EventArgs e);
    public partial class SiteTimeSeriesExplorer : UserControl
    {
        private BackgroundWorker worker;
        private IPackage _Package;
        public event BoolEventHandler LoadPackage_Clicking;
        public event EventHandler LoadPackage_Finished;
        public event EventHandler SlctDataSource_Clicked;

        public SiteTimeSeriesExplorer()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            toolStripProgressBar1.Visible = false;
            cmbSimSite.DisplayMember = "Name";
            cmbSimVars.ComboBox.DisplayMember = "Name";
            cmbObsSite.DisplayMember = "Name";
            cmbObsVars.DisplayMember = "Name";
            this.Load += Control_Load;
        }

        [Browsable(false)]
        public IPackage Package
        {
            get
            {
                return _Package;
            }
            set
            {
                _Package = value;
                propertyGrid1.SelectedObject = _Package;
            }
        }

        [Browsable(false)]
        public ODMSource ODM
        {
            get;
            set;
        }

        public ToolStrip MainBar
        {
            get
            {
                return toolStrip1;
            }
        }

        public WinChart Chart
        {
            get
            {
                return winChart_timeseries;
            }
        }

        public PropertyGrid PropertyGrid
        {
            get
            {
                return propertyGrid1;
            }
        }

        public string SQLSelection
        {
            get;
            set;
        }

        public int SelectedVariableIndex
        {
            get
            {
                return cmbSimVars.SelectedIndex;
            }
        }

        public void ClearContent()
        {

        }

        private void Control_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            BindSites();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (LoadPackage_Clicking != null)
            {
               if(!LoadPackage_Clicking(sender, e))
               {
                   return;
               }
            }

            if (Package != null)
            {
                toolStripProgressBar1.Visible = true;
                toolStrip1.Enabled = false;
                tabControl2.Enabled = false;

                Package.Loading += Package_Loading;
                Package.Loaded += Package_Loaded;
                worker.RunWorkerAsync();
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Package.Load();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //make sure the new value is valid for the progress bar and update it 

            if (e.ProgressPercentage >= toolStripProgressBar1.Minimum && e.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            //do not update the text if a cancellation request is pending
            labelStatus.Text = string.Format("Loading  {0}%", e.ProgressPercentage);
        }

        private void Package_Loading(object sender, int e)
        {
            worker.ReportProgress(e);
        }

        private void Package_Loaded(object sender, object e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                toolStripProgressBar1.Visible = false;
                labelStatus.Text = "";
                toolStrip1.Enabled = true;
                tabControl2.Enabled = true;

                var provider = Package as ISitesProvider;
                cmbSimSite.DataSource = provider.Sites;

                Package.Loading -= Package_Loading;
                Package.Loaded -= Package_Loaded;

                if (LoadPackage_Finished != null)
                    LoadPackage_Finished(sender, EventArgs.Empty);
            });            
        }


        private void cmbSimSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSimSite.SelectedItem != null)
            {
                var site = cmbSimSite.SelectedItem as Site;
                cmbSimVars.ComboBox.DataSource = site.Variables;
                if(ODM != null)
                {
                    cmbObsSite.DataSource = new Site[] { site };
                }
            }
        }

        private void cmbSimVars_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSimSite.SelectedItem != null && cmbSimVars.SelectedItem != null)
            {
                var sim_site = cmbSimSite.SelectedItem as Site;
                var pck = Package as IDataPackage;
                var ts = pck.Values.GetDoubleSeriesBetween(SelectedVariableIndex, cmbSimSite.SelectedIndex, pck.StartOfLoading, pck.EndOfLoading);
                var series_anme = sim_site.Name;
                if (ts != null)
                {
                    if (btnCompareMode.Checked)
                        winChart_timeseries.Clear();
                    var derieved_ts = TimeSeriesAnalyzer.Derieve(ts, pck.NumericalDataType, pck.TimeUnits);
                    string sereis = string.Format("{1} at {0}", sim_site.Name, cmbSimSite.SelectedItem.ToString());
                    sim_site.TimeSeries = derieved_ts;
                    winChart_timeseries.Plot<double>(derieved_ts.DateTimes, derieved_ts.Value, sereis);

                    var sites = cmbObsSite.DataSource as Site[];
                    if(sites != null)
                    {
                        var selected_site = from ss in sites where ss.ID == sim_site.ID select ss;
                        if (selected_site.Count() > 0)
                            cmbObsSite.SelectedItem = selected_site.First();
                    }
                }
            }
        }

        private void cmbObsSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ODM != null)
            {
                var site = cmbObsSite.SelectedItem as Site;
                var varbs = ODM.GetVariables(site.ID);
                 var pck = Package as IDataPackage;
                 if (varbs != null)
                 {
                     var selected_var = from vv in varbs where vv.ID == pck.ODMVariableID select vv;
                     if (selected_var != null)
                         cmbObsVars.DataSource = selected_var.ToArray();
                     else
                         cmbObsVars.DataSource = null;
                 }
            }
        }

        private void cmbObsVars_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ODM != null)
            {
                var pck = Package as IDataPackage;
                var site = cmbObsSite.SelectedItem as Site;
                var varb = cmbObsVars.SelectedItem as Variable;
                var ts = ODM.GetTimeSeries(new QueryCriteria()
                {
                    Start = pck.StartOfLoading,
                    End = pck.EndOfLoading,
                    SiteID = site.ID,
                    VariableID = varb.ID,
                    VariableName = varb.Name
                });
                if (ts != null)
                {
                    var derieved_ts = TimeSeriesAnalyzer.Derieve(ts, pck.NumericalDataType, pck.TimeUnits);
                    string sereis = string.Format("{1} at {0}", site.Name, varb.Name);
                    if (btnCompareMode.Checked)
                    {
                        var sim_site = cmbSimSite.SelectedItem as Site;
                        if (sim_site != null && sim_site.TimeSeries != null)
                        {
                           TimeSeriesAnalyzer.Compensate(derieved_ts, sim_site.TimeSeries as IVectorTimeSeries<double>);
                           winChart_timeseries.Plot<double>(derieved_ts.DateTimes, derieved_ts.Value, sereis);
                        }
                    }
                    else
                    {
                        winChart_timeseries.Plot<double>(derieved_ts.DateTimes, derieved_ts.Value, sereis);
                    }
                }
            }
        }

        private void tbnSlctDataSource_Click(object sender, EventArgs e)
        {
            if (SlctDataSource_Clicked != null)
                SlctDataSource_Clicked(sender, e);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BindSites();
        }

        private void BindSites()
        {
            if (ODM != null)
            {
                var sites = ODM.GetSites(SQLSelection);
                if (sites != null)
                    cmbObsSite.DataSource = sites;
            }
        }
    }
}