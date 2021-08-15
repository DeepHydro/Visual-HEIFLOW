﻿//
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

using DotSpatial.Symbology;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Controls.WinForm.TimeSeriesExplorer;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.SFRExplorer
{
    public partial class SFRExplorer : UserControl
    {
        private BackgroundWorker worker;
        private SFROutputPackage _SFROutputPackage;
        private DataCube<double> _ProfileMat;
        private List<River> _ProfileRivers;
        private IShellService _ShellService;
        private int _Selected_Sfr_var;
        private bool _LoadAllVars = true;
        private FeatureMapLayer[] _FeatureMapLayers;
        private FeatureMapLayer _SelectedFeatureMapLayer;

        public SFRExplorer()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            toolStripProgressBar1.Visible = false;
            cmbSegsID.DisplayMember = "ID";
            cmbRchID.DisplayMember = "SubID";
            cmbSite.DisplayMember = "Name";
            cmbObsVars.DisplayMember = "Name";
            cmbLoadVar.SelectedIndex = 0;
            tabControlLeft.Enabled = false;
            cmbLayers.DisplayMember = "LegendText";
            cmbLayers.ValueMember = "DataSet";
        }

        public SFROutputPackage SFROutput
        {
            get
            {
                return _SFROutputPackage;
            }
            set
            {
                _SFROutputPackage = value;
                if (_SFROutputPackage != null)
                {
                    _SFROutputPackage.ScaleFactor = 1.0 / 86400;
                    propertyGrid1.SelectedObject = _SFROutputPackage;
                    cmbSFRVars.ComboBox.DataSource = _SFROutputPackage.Variables;
                    cmbSFRVars.SelectedIndex = 0;
                }
            }
        }

        public ODMSource ODM
        {
            get;
            set;
        }

        public ToolStrip MainBar
        {
            get
            {
                return this.toolStrip1;
            }
        }

        public FeatureMapLayer[] FeatureLayers
        {
            set
            {
                _FeatureMapLayers = value;
                cmbLayers.DataSource = _FeatureMapLayers;
            }
            get
            {
                return _FeatureMapLayers;
            }
        }

        private void SFRExplorer_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            BindSites();
            if (MyAppManager.Instance.AppMode == Presentation.Controls.AppMode.HE)
                btnAdd2Toolbox.Visible = false;
            else
                btnAdd2Toolbox.Visible = true;
            cmbSFRVars.ComboBox.DataSource = SFROutput.DefaultAttachedVariables;
            cmbSFRVars.SelectedIndex = 0;
            _LoadAllVars = true;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (SFROutput != null)
            {
                SFROutput.IsLoadCompleteData = chbReadComplData.Checked;
                toolStripProgressBar1.Visible = true;
                toolStrip1.Enabled = false;
                tabControlLeft.Enabled = false;
                colorSlider1.Enabled = false;
                SFROutput.Loading += SFROutputPackage_Loading;
                SFROutput.Loaded += SFROutputPackage_Loaded;
                worker.RunWorkerAsync();
            }
        }

        public void ClearContent()
        {
            this.winChart_timeseries.Clear();

        }
        private void SFROutputPackage_Loaded(object sender, LoadingObjectState e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                toolStripProgressBar1.Visible = false;
                labelStatus.Text = "";
                toolStrip1.Enabled = true;
                tabControlLeft.Enabled = true;
                colorSlider1.Enabled = true;
                SFROutput.Loading -= SFROutputPackage_Loading;
                SFROutput.Loaded -= SFROutputPackage_Loaded;

                if (e.State == LoadingState.Normal)
                {
                    var riv_ids = from rv in SFROutput.RiverNetwork.Rivers select rv.ID;
                    cmbSegsID.DataSource = SFROutput.RiverNetwork.Rivers;
                    cmbStartID.DataSource = riv_ids.ToArray();
                    cmbDates.DataSource = SFROutput.DataCube.DateTimes;
                }
                else
                {
                    MessageBox.Show("Failed to load data. Error message: " + e.Message, "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });
        }

        private void SFROutputPackage_Loading(object sender, int e)
        {
            worker.ReportProgress(e);
        }

        private void SFROutput_LoadFailed(object sender, string e)
        {
            worker.CancelAsync();
            this.Invoke((MethodInvoker)delegate
       {
           toolStripProgressBar1.Visible = false;
           labelStatus.Text = "";
           toolStrip1.Enabled = true;
           tabControlLeft.Enabled = true;
           colorSlider1.Enabled = true;
           //SFROutput.LoadFailed -= SFROutput_LoadFailed;
       });
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_LoadAllVars)
                SFROutput.Load(null);
            else
                SFROutput.Load(_Selected_Sfr_var, null);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= toolStripProgressBar1.Minimum && e.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            //do not update the text if a cancellation request is pending
            labelStatus.Text = string.Format("Loading  {0}%", e.ProgressPercentage);
        }

        private void cmbSegsID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSegsID.SelectedItem != null)
            {
                var river = cmbSegsID.SelectedItem as River;
                if (chbReadComplData.Checked)
                    cmbRchID.DataSource = river.Reaches;
                else
                {
                    tabControl_Chart.SelectedTab = this.tabPageTimeSeries;
                    if (cmbSFRVars.SelectedIndex < 0)
                        return;
                    var fts = SFROutput.GetTimeSeries(river.ID - 1, cmbSFRVars.SelectedIndex);
                    if (fts != null)
                    {
                        string sereis = string.Format("{0} at Segment {1} Reach {2}", cmbSFRVars.SelectedItem.ToString(), river.ID, river.LastReach.SubID);
                        winChart_timeseries.Plot<float>(fts.DateTimes, fts[0, ":", "0"], sereis);
                    }
                    else
                    {
                        MessageBox.Show("No data retrieved.", "SFR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void cmbRchID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSFRVars.SelectedIndex < 0)
                return;

            if (cmbRchID.SelectedItem != null)
            {
                tabControl_Chart.SelectedTab = this.tabPageTimeSeries;
                var reach = cmbRchID.SelectedItem as Reach;
                var fts = SFROutput.GetTimeSeries(reach.Parent.SubIndex, reach.SubIndex, cmbSFRVars.SelectedIndex, _SFROutputPackage.StartOfLoading);
                if (fts != null)
                {
                    string sereis = string.Format("{0} at Segment {1} Reach {2}", cmbSFRVars.SelectedItem.ToString(), reach.Parent.ID, reach.SubID);
                    winChart_timeseries.Plot<float>(fts.DateTimes, fts[0, ":", "0"], sereis);
                }
                else
                {
                    MessageBox.Show("No data retrieved.", "SFR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void cmbSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSite.SelectedItem != null && ODM != null)
            {
                var site = cmbSite.SelectedItem as Site;
                var varbs = ODM.GetVariables(site.ID);
                cmbObsVars.DataSource = varbs;
            }
        }

        private void cmbObsVars_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ODM != null)
            {
                var site = cmbSite.SelectedItem as Site;
                var varb = cmbObsVars.SelectedItem as Variable;
                var ts = ODM.GetTimeSeries(new QueryCriteria()
                {
                    Start = _SFROutputPackage.StartOfLoading,
                    End = _SFROutputPackage.EndOfLoading,
                    SiteID = site.ID,
                    VariableID = varb.ID,
                    VariableName = varb.Name
                });
                if (ts != null)
                {
                    var derieved_ts = TimeSeriesAnalyzer.Derieve(ts, _SFROutputPackage.NumericalDataType, _SFROutputPackage.TimeUnits);
                    string sereis = string.Format("{1} at {0}", site.Name, varb.Name);
                    winChart_timeseries.Plot<double>(derieved_ts.DateTimes, derieved_ts[0, ":", "0"], sereis);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BindSites();
        }

        private void chbReadComplData_CheckedChanged(object sender, EventArgs e)
        {
            cmbRchID.Enabled = chbReadComplData.Checked;
        }

        private void cmbStartID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStartID.SelectedItem != null)
            {
                var river = (int)cmbStartID.SelectedItem;
                var profiles = SFROutput.RiverNetwork.BuildProfile(river);
                var riv_ids = (from rv in profiles select rv.ID).ToArray();
                cmbEndID.DataSource = riv_ids;
            }
        }

        private void cmbEndID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEndID.SelectedItem != null)
            {
                if (cmbSFRVars.SelectedIndex < 0)
                    return;

                if (!SFROutput.DataCube.IsAllocated(cmbSFRVars.SelectedIndex))
                {
                    MessageBox.Show("The selected variable is not loaded.", "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                tabControl_Chart.SelectedTab = this.tabPageProfile;
                var river_start = (int)cmbStartID.SelectedItem;
                var river_end = (int)cmbEndID.SelectedItem;

                _ProfileRivers = SFROutput.RiverNetwork.BuildProfile(river_start, river_end);
                _ProfileMat = SFROutput.ProfileTimeSeries(_ProfileRivers, cmbSFRVars.SelectedIndex, 0,
                    chbReadComplData.Checked, chbUnifiedByLength.Checked);
                colorSlider1.Maximum = SFROutput.DataCube.Size[1] - 1;
                colorSlider1.Value = 0;
                string series = string.Format("{0} from {1} to {2}", cmbSFRVars.SelectedItem.ToString(), river_start, river_end);
                winChart_proflie.Plot(_ProfileMat[0, "0", ":"], _ProfileMat[1, "0", ":"], series);
            }
        }

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            if (cmbStartID.SelectedItem == null || cmbEndID.SelectedItem == null || cmbSFRVars.SelectedIndex < 0 || colorSlider1.Value == 0)
            {
                return;
            }
            _ProfileMat = SFROutput.ProfileTimeSeries(_ProfileRivers, cmbSFRVars.SelectedIndex, colorSlider1.Value,
                chbReadComplData.Checked, chbUnifiedByLength.Checked);
            string series = string.Format("{0} from {1} to {2}", cmbSFRVars.SelectedItem.ToString(), cmbStartID.SelectedItem, cmbEndID.SelectedItem);
            winChart_proflie.Plot(_ProfileMat[0, "0", ":"], _ProfileMat[1, "0", ":"], series);
            if (colorSlider1.Value < SFROutput.TimeService.IOTimeline.Count)
                tbCurDate.Text = SFROutput.TimeService.IOTimeline[colorSlider1.Value].ToString();
        }

        private void BindSites()
        {
            var projectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            ODM = projectService.Project.ODMSource;
            if (ODM != null)
            {
                var sites = ODM.GetSites(Settings.Default.GagingStationSQL);
                if (sites != null)
                    cmbSite.DataSource = sites;
            }
        }

        private void tbnSlctDataSource_Click(object sender, EventArgs e)
        {
            SQLSelection sql = new SQLSelection(Settings.Default.GagingStationSQL)
            {
                ODM = this.ODM
            };
            if (sql.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Default.GagingStationSQL = sql.SQLScript;
                Settings.Default.Save();
            }
        }

        private void segmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "shp file|*.shp";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SFROutput.SFRPackage.SaveSegmentAsShp(dlg.FileName);
            }
        }

        private void reachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "shp file|*.shp";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SFROutput.SFRPackage.SaveReachAsShp(dlg.FileName);
            }
        }

        private void exportRiversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ProfileRivers == null)
                return;
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.FileName = "Segment Profile.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(ofd.FileName);
                sw.WriteLine("NSEG,ICALC,OUTSEG,IUPSEG,FLOW,RUNOFF,ETSW,PPTSW,ROUGHCH,WIDTH1,WIDTH2,IPRIOR");
                foreach (var river in _ProfileRivers)
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", river.ID, river.ICALC, river.OutRiverID,
                        river.UpRiverID, river.Flow, river.Runoff, river.ETSW, river.PPTSW, river.ROUGHCH, river.Width1, river.Width2, river.IPrior);
                    sw.WriteLine(line);

                }
                sw.Close();
            }
        }
        private void exportReachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ProfileRivers == null)
                return;
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.FileName = "Reach Profile.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(ofd.FileName);
                sw.WriteLine("KRCH,IRCH,JRCH,ISEG,IREACH,RCHLEN,STRTOP,SLOPE,STRTHICK,STRHC1,THTS,THTI,EPS");
                foreach (var river in _ProfileRivers)
                {
                    foreach (var re in river.Reaches)
                    {
                        string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", re.KRCH, re.IRCH, re.JRCH, re.ISEG,
                            re.IREACH, re.Length, re.TopElevation, re.Slope, re.BedThick,
                            re.STRHC1, re.THTS, re.THTI, re.EPS);
                        sw.WriteLine(line);
                    }
                }
                sw.Close();
            }
        }

        private void btnAdd2Toolbox_Click(object sender, EventArgs e)
        {
            if (_ShellService == null)
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            Cursor.Current = Cursors.WaitCursor;
            if (_ProfileRivers != null && SFROutput.DataCube != null)
            {
                var mat = SFROutput.GetProfileTimeSeries(_ProfileRivers, cmbSFRVars.SelectedIndex, cmbSFRVars.SelectedItem.ToString(), SFROutput.DataCube.Size[1],
                       chbReadComplData.Checked, chbUnifiedByLength.Checked);
                _ShellService.PackageToolManager.Workspace.Add(mat);
            }
            else
            {
                MessageBox.Show("No data loaded", "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btnAddSfrMat2Toolbox_Click(object sender, EventArgs e)
        {
            if (_ShellService == null)
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            Cursor.Current = Cursors.WaitCursor;
            _ShellService.PackageToolManager.Workspace.Add(SFROutput.DataCube);
            Cursor.Current = Cursors.Default;
        }

        private void cmbSFRVars_SelectedIndexChanged(object sender, EventArgs e)
        {
            _Selected_Sfr_var = cmbSFRVars.SelectedIndex;
            //if (_Selected_Sfr_var == 0 || _Selected_Sfr_var == 2)
            //    _SFROutputPackage.Offset = 10;
            //else
            //    _SFROutputPackage.Offset = 0;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            SFROutput.Scan();
            if (SFROutput.NumTimeStep > 0)
            {
                tabControlLeft.Enabled = true;

            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _SFROutputPackage.DataCube.Clear();
        }

        private void cmbLoadVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLoadVar.SelectedIndex == 0)
                _LoadAllVars = true;
            else
                _LoadAllVars = false;
        }

        private void btnShowLayer_Click(object sender, EventArgs e)
        {
            if (cmbLayers.SelectedItem == null)
                return;
            if (cmbDates.SelectedItem == null)
                return;
            if (cmbSFRVars.SelectedIndex < 0)
                return;
            if (cmbSegFields.SelectedIndex < 0)
            {
                MessageBox.Show("Field that represents Segment ID must be selected", "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chbReadComplData.Checked && cmbReachFields.SelectedIndex < 0)
            {
                MessageBox.Show("Field that represents Reach ID must be selected", "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!SFROutput.DataCube.IsAllocated(cmbSFRVars.SelectedIndex))
            {
                MessageBox.Show("The selected variable is not loaded.", "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _SelectedFeatureMapLayer = cmbLayers.SelectedItem as FeatureMapLayer;
            if (_SelectedFeatureMapLayer != null)
            {
                var dt = _SelectedFeatureMapLayer.DataSet.DataTable;
                Cursor.Current = Cursors.WaitCursor;
                int segid = 0;
                int reachid = 0;
                int failurecount = 0;
                var varname = SFROutput.DefaultVariablesAbbrv[cmbSFRVars.SelectedIndex];
                if (!dt.Columns.Contains(varname))
                {
                    DataColumn col = new DataColumn(varname, typeof(float));
                    dt.Columns.Add(col);
                }
                var vec = SFROutput.DataCube.GetVector(cmbSFRVars.SelectedIndex, cmbDates.SelectedIndex.ToString(), ":");

                if (chbReadComplData.Checked)
                {
                    var segfield = cmbSegFields.SelectedItem.ToString();
                    var reachfield = cmbReachFields.SelectedItem.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        segid = 0;
                        reachid = 0;
                        int.TryParse(dr[segfield].ToString(), out segid);
                        int.TryParse(dr[reachfield].ToString(), out reachid);
                        if (segid != 0 && reachid != 0)
                        {
                            var index = SFROutput.GetReachSerialIndex(segid, reachid);
                            if (index >= 0)
                                dt.Rows[i][varname] = vec[index];
                            else
                                failurecount++;
                        }
                    }
                }
                else
                {
                    var segfield = cmbSegFields.SelectedItem.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        segid = 0;
                        int.TryParse(dr[segfield].ToString(), out segid);
                        if (segid != 0)
                        {
                            dt.Rows[i][varname] = vec[segid - 1];
                        }
                        else
                            failurecount++;
                    }
                }

                if (failurecount > 0)
                {
                    MessageBox.Show("The number of failed rows is: " + failurecount, "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (checkBoxSaveLayer.Checked)
                        _SelectedFeatureMapLayer.DataSet.Save();
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnRefreshLayer_Click(object sender, EventArgs e)
        {
            var control = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectController>();
            var map_layers = from layer in control.MapAppManager.Map.Layers where layer is IFeatureLayer select new FeatureMapLayer { LegendText = layer.LegendText, DataSet = (layer as IFeatureLayer).DataSet };
            this.FeatureLayers = map_layers.ToArray();
        }

        private void exportToSWMMInpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "SWMM input file|*.inp";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SFROutput.SFRPackage.RiverNetwork.NetworkToSWMM(dlg.FileName);
            }
        }

        private void riverJunctionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "shp file|*.shp";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SFROutput.SFRPackage.SaveJunctionsAsShp(dlg.FileName);
            }
        }

        private void chbReadComplData_CheckedChanged_1(object sender, EventArgs e)
        {
            cmbRchID.Enabled = chbReadComplData.Checked;

        }

        private void cmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedFeatureMapLayer = cmbLayers.SelectedItem as FeatureMapLayer;
            if (_SelectedFeatureMapLayer != null)
            {
                var buf = from DataColumn dc in _SelectedFeatureMapLayer.DataSet.DataTable.Columns select dc.ColumnName;
                var fields = buf.ToArray();
                cmbSegFields.DataSource = fields;

                var buf1 = from DataColumn dc in _SelectedFeatureMapLayer.DataSet.DataTable.Columns select dc.ColumnName;
                var fields1 = buf1.ToArray();
                cmbReachFields.DataSource = fields1;

                if (chbReadComplData.Checked)
                {
                    cmbSegFields.Enabled = true;
                    cmbReachFields.Enabled = true;
                }
                else
                {
                    cmbSegFields.Enabled = true;
                    cmbReachFields.Enabled = false;
                }
            }
        }
    }
}