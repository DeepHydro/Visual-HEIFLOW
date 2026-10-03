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

using DotSpatial.Symbology;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Controls.WinForm.TimeSeriesExplorer;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
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
        private string _sfroutfile;
        private string _curfilename;
        private bool _DataLoaded;

        public SFRExplorer()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
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
                    if (_SFROutputPackage.Variables != null && _SFROutputPackage.Variables.Length > 0)
                        cmbSFRVars.SelectedIndex = 0;
                    _sfroutfile = _SFROutputPackage.FileName;
                    _curfilename = _sfroutfile;
                }
            }
        }

        public ODMSource ODM
        {
            get;
            set;
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
            btnAdd2Toolbox.Visible = MyAppManager.Instance.AppMode != Presentation.Controls.AppMode.HE;
            _LoadAllVars = true;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as HeiflowModel;
            if(model != null)
            {
                if( model.ProcessModule == ProcessModule.Hydrology)
                {
                    mi_flow.Enabled = true;
                    mi_nps.Enabled = false;
                    mi_sediment.Enabled = false;
                    mi_month_npc.Enabled = false;
                }
                else if (model.ProcessModule == ProcessModule.NPS)
                {
                    mi_flow.Enabled = true;
                    mi_nps.Enabled = true;
                    mi_sediment.Enabled = true;
                    mi_month_npc.Enabled = false;
                }
                else if (model.ProcessModule == ProcessModule.Carbon)
                {
                    mi_flow.Enabled = true;
                    mi_nps.Enabled = true;
                    mi_sediment.Enabled = true;
                    mi_month_npc.Enabled = true;
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (SFROutput == null)
            {
                ShowWarning("The SFR output package is not initialized. Please open a model project first.");
                return;
            }
            if (worker.IsBusy)
            {
                ShowInfo("The SFR output is being loaded. Please wait until the current loading finishes.");
                return;
            }

            string reason;
            if (!CanLoad(out reason))
            {
                ShowWarning(reason);
                return;
            }

            SFROutput.IsLoadCompleteData = chbReadComplData.Checked;
            UnsubscribePackageEvents();
            SFROutput.Loading += SFROutputPackage_Loading;
            SFROutput.Loaded += SFROutputPackage_Loaded;
            SFROutput.LoadFailed += SFROutput_LoadFailed;
            SetBusyState(true, "Loading  0%");
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// 加载前的合法性检查，失败时给出可操作的原因
        /// </summary>
        private bool CanLoad(out string reason)
        {
            reason = "";
            var filename = SFROutput.LocalFileName;
            if (string.IsNullOrWhiteSpace(filename))
            {
                reason = "The SFR output file is not specified. Please open a model project first.";
                return false;
            }
            if (!File.Exists(filename))
            {
                reason = string.Format("The SFR output file does not exist:\n{0}\nPlease run the model first, or use \"Scan\" to select another output file.", filename);
                return false;
            }
            if (SFROutput.Variables == null || SFROutput.Variables.Length == 0)
            {
                reason = "No variable is available. Please click \"Scan\" to read the variable list from the output file.";
                return false;
            }
            if (cmbSFRVars.SelectedIndex < 0)
            {
                reason = "Please select a variable before loading.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 统一切换忙碌状态，保证任何退出路径下界面都能恢复
        /// </summary>
        private void SetBusyState(bool busy, string status = "")
        {
            toolStripProgressBar1.Visible = busy;
            toolStrip1.Enabled = !busy;
            tabControlLeft.Enabled = !busy && _DataLoaded;
            colorSlider1.Enabled = !busy && _DataLoaded;
            labelStatus.Text = status;
            if (busy)
                toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
        }

        private void UnsubscribePackageEvents()
        {
            if (SFROutput == null)
                return;
            SFROutput.Loading -= SFROutputPackage_Loading;
            SFROutput.Loaded -= SFROutputPackage_Loaded;
            SFROutput.LoadFailed -= SFROutput_LoadFailed;
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "SFR", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "SFR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "SFR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ClearContent()
        {
            this.winChart_timeseries.Clear();

        }
        private void SFROutputPackage_Loaded(object sender, LoadingObjectState e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { SFROutputPackage_Loaded(sender, e); });
                return;
            }

            if (e.State == LoadingState.Normal)
            {
                if (SFROutput.RiverNetwork != null && SFROutput.RiverNetwork.Rivers != null)
                {
                    cmbSegsID.DataSource = SFROutput.RiverNetwork.Rivers;
                    cmbStartID.DataSource = (from rv in SFROutput.RiverNetwork.Rivers select rv.ID).ToArray();
                }
                if (SFROutput.DataCube != null)
                    cmbDates.DataSource = SFROutput.DataCube.DateTimes;
            }
            else
            {
                var msg = string.IsNullOrEmpty(e.Message) ? "Unknown error." : e.Message;
                ShowWarning("Failed to load the SFR output.\n" + msg);
            }
        }

        private void SFROutputPackage_Loading(object sender, int e)
        {
            if (worker.IsBusy)
                worker.ReportProgress(e);
        }

        private void SFROutput_LoadFailed(object sender, string e)
        {
            if (worker.IsBusy)
                worker.CancelAsync();
            var msg = "Failed to load the SFR output.\n" + e;
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ShowWarning(msg); });
                return;
            }
            ShowWarning(msg);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                if (_LoadAllVars)
                    SFROutput.Load(null);
                else
                    SFROutput.Load(_Selected_Sfr_var, null);
            }
            catch (Exception ex)
            {
                // 交由 worker_RunWorkerCompleted 统一提示，避免异常逸出后台线程
                e.Result = ex;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UnsubscribePackageEvents();
            _DataLoaded = SFROutput != null && SFROutput.DataCube != null && SFROutput.DataCube.Size[1] > 0;
            SetBusyState(false);

            if (e.Error != null)
            {
                ShowError("Failed to load the SFR output.\n" + e.Error.Message);
                return;
            }
            if (e.Result is Exception)
            {
                ShowError("Failed to load the SFR output.\n" + ((Exception)e.Result).Message);
                return;
            }
            if (e.Cancelled)
            {
                ShowInfo("The loading was cancelled.");
                return;
            }
            if (_DataLoaded)
            {
                labelStatus.Text = string.Format("{0} time step(s), {1} variable(s) loaded.",
                    SFROutput.DataCube.Size[1], SFROutput.DataCube.Size[0]);
            }
            else
            {
                ShowWarning("No data is loaded. Please check the output file and the selected variable.");
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= toolStripProgressBar1.Minimum && e.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            //do not update the text if a cancellation request is pending
            labelStatus.Text = string.Format("Loading  {0}%", e.ProgressPercentage);
        }

        /// <summary>
        /// 检查当前所选变量的数据是否可读，不可读时给出可读的原因
        /// </summary>
        private bool TryGetSelectedVariable(out int varIndex, out string reason)
        {
            varIndex = cmbSFRVars.SelectedIndex;
            reason = "";
            if (SFROutput == null)
            {
                reason = "The SFR output package is not initialized. Please open a model project first.";
                return false;
            }
            if (varIndex < 0)
            {
                reason = "Please select a variable first.";
                return false;
            }
            return SFROutput.IsVariableReady(varIndex, out reason);
        }

        private void cmbSegsID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var river = cmbSegsID.SelectedItem as River;
            if (river == null)
                return;

            if (chbReadComplData.Checked)
            {
                cmbRchID.DataSource = river.Reaches;
                return;
            }

            int varIndex;
            string reason;
            if (!TryGetSelectedVariable(out varIndex, out reason))
            {
                ShowWarning(reason);
                return;
            }

            var fts = SFROutput.GetTimeSeries(river.ID - 1, varIndex);
            if (fts == null)
            {
                ShowWarning(string.Format("No data is retrieved for \"{0}\" at segment {1}.\nThe segment may not be contained in the loaded data.",
                    cmbSFRVars.SelectedItem, river.ID));
                return;
            }
            var reachLabel = river.LastReach != null ? river.LastReach.SubID.ToString() : "-";
            string sereis = string.Format("{0} at Segment {1} Reach {2}", cmbSFRVars.SelectedItem, river.ID, reachLabel);
            winChart_timeseries.Plot<float>(fts.DateTimes, fts[0, ":", "0"], sereis);
        }

        private void cmbRchID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var reach = cmbRchID.SelectedItem as Reach;
            if (reach == null)
                return;

            int varIndex;
            string reason;
            if (!TryGetSelectedVariable(out varIndex, out reason))
            {
                ShowWarning(reason);
                return;
            }

            var fts = SFROutput.GetTimeSeries(reach.Parent.SubIndex, reach.SubIndex, varIndex, _SFROutputPackage.StartOfLoading);
            if (fts == null)
            {
                ShowWarning(string.Format("No data is retrieved for \"{0}\" at segment {1} reach {2}.\nThe reach may not be contained in the loaded data.",
                    cmbSFRVars.SelectedItem, reach.Parent != null ? reach.Parent.ID : 0, reach.SubID));
                return;
            }
            var derieved_ts = TimeSeriesAnalyzer.Derieve(fts, _SFROutputPackage.NumericalDataType, _SFROutputPackage.TimeUnits);
            string sereis = string.Format("{0} at Segment {1} Reach {2}", cmbSFRVars.SelectedItem,
                reach.Parent != null ? reach.Parent.ID : 0, reach.SubID);
            winChart_timeseries.Plot<float>(derieved_ts.DateTimes, derieved_ts[0, ":", "0"], sereis);
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
            if (ODM == null)
                return;

            var site = cmbSite.SelectedItem as Site;
            var varb = cmbObsVars.SelectedItem as Variable;
            if (site == null || varb == null)
                return;
            if (_SFROutputPackage == null)
            {
                ShowWarning("The SFR output package is not initialized. Please open a model project first.");
                return;
            }

            try
            {
                var ts = ODM.GetTimeSeries(new QueryCriteria()
                {
                    Start = _SFROutputPackage.StartOfLoading,
                    End = _SFROutputPackage.EndOfLoading,
                    SiteID = site.ID,
                    VariableID = varb.ID,
                    VariableName = varb.Name
                });
                if (ts == null)
                {
                    ShowInfo(string.Format("No observation is available for \"{0}\" at \"{1}\" within the selected period.", varb.Name, site.Name));
                    return;
                }
                var derieved_ts = TimeSeriesAnalyzer.Derieve(ts, _SFROutputPackage.NumericalDataType, _SFROutputPackage.TimeUnits);
                string sereis = string.Format("{1} at {0}", site.Name, varb.Name);
                winChart_timeseries.Plot<double>(derieved_ts.DateTimes, derieved_ts[0, ":", "0"], sereis);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Failed to read the observation series of \"{0}\" at \"{1}\".\n{2}", varb.Name, site.Name, ex.Message));
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
            if (cmbEndID.SelectedItem == null || cmbStartID.SelectedItem == null)
                return;

            int varIndex;
            string reason;
            if (!TryGetSelectedVariable(out varIndex, out reason))
            {
                ShowWarning(reason);
                return;
            }

            var river_start = (int)cmbStartID.SelectedItem;
            var river_end = (int)cmbEndID.SelectedItem;

            _ProfileRivers = SFROutput.RiverNetwork.BuildProfile(river_start, river_end);
            _ProfileMat = SFROutput.ProfileTimeSeries(_ProfileRivers, varIndex, 0,
                chbReadComplData.Checked, chbUnifiedByLength.Checked);
            if (_ProfileMat == null)
            {
                ShowWarning(string.IsNullOrEmpty(SFROutput.Message)
                    ? string.Format("Failed to build the profile from segment {0} to segment {1}.", river_start, river_end)
                    : SFROutput.Message);
                return;
            }
            colorSlider1.Maximum = Math.Max(0, SFROutput.DataCube.Size[1] - 1);
            colorSlider1.Value = 0;
            colorSlider1.Enabled = true;
            string series = string.Format("{0} from {1} to {2}", cmbSFRVars.SelectedItem, river_start, river_end);
            winChart_proflie.Plot(_ProfileMat[0, "0", ":"], _ProfileMat[1, "0", ":"], series);
        }

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            if (_ProfileRivers == null || cmbStartID.SelectedItem == null || cmbEndID.SelectedItem == null)
                return;

            int varIndex;
            string reason;
            // 拖动过程中不弹窗打断操作
            if (!TryGetSelectedVariable(out varIndex, out reason))
                return;

            _ProfileMat = SFROutput.ProfileTimeSeries(_ProfileRivers, varIndex, colorSlider1.Value,
                chbReadComplData.Checked, chbUnifiedByLength.Checked);
            if (_ProfileMat == null)
                return;

            string series = string.Format("{0} from {1} to {2}", cmbSFRVars.SelectedItem, cmbStartID.SelectedItem, cmbEndID.SelectedItem);
            winChart_proflie.Plot(_ProfileMat[0, "0", ":"], _ProfileMat[1, "0", ":"], series);
            if (SFROutput.TimeService != null && SFROutput.TimeService.IOTimeline != null
                && colorSlider1.Value < SFROutput.TimeService.IOTimeline.Count)
                tbCurDate.Text = SFROutput.TimeService.IOTimeline[colorSlider1.Value].ToString();
        }

        private void BindSites()
        {
            try
            {
                var projectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
                ODM = projectService != null && projectService.Project != null ? projectService.Project.ODMSource : null;
                if (ODM == null)
                {
                    labelStatus.Text = "No observation database (ODM) is available in the current project.";
                    return;
                }
                var sites = ODM.GetSites(Settings.Default.GagingStationSQL);
                if (sites != null)
                    cmbSite.DataSource = sites;
            }
            catch (Exception ex)
            {
                ODM = null;
                labelStatus.Text = "Failed to bind observation sites: " + ex.Message;
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
            if (SFROutput == null)
            {
                ShowWarning("The SFR output package is not initialized. Please open a model project first.");
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "shp file|*.shp";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SFROutput.SFRPackage.SaveSegmentAsShp(dlg.FileName);
            }
        }

        private void reachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SFROutput == null)
            {
                ShowWarning("The SFR output package is not initialized. Please open a model project first.");
                return;
            }
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
            {
                ShowInfo("No river profile is available. Please select a start segment and an end segment first.");
                return;
            }
            using (SaveFileDialog ofd = new SaveFileDialog())
            {
                ofd.FileName = "Segment Profile.csv";
                ofd.Filter = "csv file|*.csv";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    using (StreamWriter sw = new StreamWriter(ofd.FileName))
                    {
                        sw.WriteLine("NSEG,ICALC,OUTSEG,IUPSEG,FLOW,RUNOFF,ETSW,PPTSW,ROUGHCH,WIDTH1,WIDTH2,IPRIOR");
                        foreach (var river in _ProfileRivers)
                        {
                            string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", river.ID, river.ICALC, river.OutRiverID,
                                river.UpRiverID, river.Flow, river.Runoff, river.ETSW, river.PPTSW, river.ROUGHCH, river.Width1, river.Width2, river.IPrior);
                            sw.WriteLine(line);
                        }
                    }
                    labelStatus.Text = string.Format("{0} segment(s) exported.", _ProfileRivers.Count);
                }
                catch (Exception ex)
                {
                    ShowError(string.Format("Failed to export the segment profile to \"{0}\".\n{1}", ofd.FileName, ex.Message));
                }
            }
        }
        private void exportReachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ProfileRivers == null)
            {
                ShowInfo("No river profile is available. Please select a start segment and an end segment first.");
                return;
            }
            using (SaveFileDialog ofd = new SaveFileDialog())
            {
                ofd.FileName = "Reach Profile.csv";
                ofd.Filter = "csv file|*.csv";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                try
                {
                    int count = 0;
                    using (StreamWriter sw = new StreamWriter(ofd.FileName))
                    {
                        sw.WriteLine("KRCH,IRCH,JRCH,ISEG,IREACH,RCHLEN,STRTOP,SLOPE,STRTHICK,STRHC1,THTS,THTI,EPS");
                        foreach (var river in _ProfileRivers)
                        {
                            foreach (var re in river.Reaches)
                            {
                                string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", re.KRCH, re.IRCH, re.JRCH, re.ISEG,
                                    re.IREACH, re.Length, re.TopElevation, re.Slope, re.BedThick,
                                    re.STRHC1, re.THTS, re.THTI, re.EPS);
                                sw.WriteLine(line);
                                count++;
                            }
                        }
                    }
                    labelStatus.Text = string.Format("{0} reach(es) exported.", count);
                }
                catch (Exception ex)
                {
                    ShowError(string.Format("Failed to export the reach profile to \"{0}\".\n{1}", ofd.FileName, ex.Message));
                }
            }
        }

        private void btnAdd2Toolbox_Click(object sender, EventArgs e)
        {
            if (_ShellService == null)
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();

            if (_ProfileRivers == null)
            {
                ShowWarning("No river profile is available. Please select a start segment and an end segment first.");
                return;
            }

            int varIndex;
            string reason;
            if (!TryGetSelectedVariable(out varIndex, out reason))
            {
                ShowWarning(reason);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var mat = SFROutput.GetProfileTimeSeries(_ProfileRivers, varIndex, cmbSFRVars.SelectedItem.ToString(),
                    SFROutput.DataCube.Size[1], chbReadComplData.Checked, chbUnifiedByLength.Checked);
                if (mat == null)
                {
                    ShowWarning(string.IsNullOrEmpty(SFROutput.Message)
                        ? "Failed to build the profile data cube."
                        : SFROutput.Message);
                    return;
                }
                _ShellService.PackageToolManager.Workspace.Add(mat);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnAddSfrMat2Toolbox_Click(object sender, EventArgs e)
        {
            if (SFROutput == null || SFROutput.DataCube == null || SFROutput.DataCube.Size[1] == 0)
            {
                ShowWarning("No SFR output is loaded. Please load the data first.");
                return;
            }
            if (_ShellService == null)
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                _ShellService.PackageToolManager.Workspace.Add(SFROutput.DataCube);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void cmbSFRVars_SelectedIndexChanged(object sender, EventArgs e)
        {
            _Selected_Sfr_var = cmbSFRVars.SelectedIndex;
            var slctvar = cmbSFRVars.SelectedItem.ToString() ;

            if ( slctvar ==  "overland flow" || slctvar == "flow out"|| slctvar == "stream loss")
            {
                _SFROutputPackage.ScaleFactor = 1.0 / 86400;
            }
            else
            {
                _SFROutputPackage.ScaleFactor = 1.0;
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (SFROutput == null)
            {
                ShowWarning("The SFR output package is not initialized. Please open a model project first.");
                return;
            }
            if (string.IsNullOrWhiteSpace(_curfilename) || !File.Exists(_curfilename))
            {
                ShowWarning(string.Format("The output file does not exist:\n{0}\nPlease run the model first.", _curfilename));
                return;
            }

            SFROutput.FileName = _curfilename;
            var isBinary = _curfilename.EndsWith(".dcx", StringComparison.OrdinalIgnoreCase);

            // Scan 负责构建河段索引；失败信息先记录，二进制文件可由 ScanVariables 补充时间步信息
            string scanMessage = null;
            if (!SFROutput.Scan())
                scanMessage = SFROutput.Message;

            if (isBinary)
            {
                if (!SFROutput.ScanVariables(_curfilename))
                {
                    ShowWarning(SFROutput.Message);
                    return;
                }
            }
            else
            {
                if (scanMessage != null)
                {
                    ShowWarning(scanMessage);
                    return;
                }
                SFROutput.ResetVariablesToDefault();
            }

            if (SFROutput.NumTimeStep <= 0)
            {
                ShowWarning(string.Format("No time step is found in \"{0}\". Please run the model first.", _curfilename));
                return;
            }
            if (SFROutput.Variables == null || SFROutput.Variables.Length == 0)
            {
                ShowWarning(string.Format("No variable is found in \"{0}\".", _curfilename));
                return;
            }

            tabControlLeft.Enabled = true;
            cmbSFRVars.ComboBox.DataSource = SFROutput.Variables;
            cmbSFRVars.SelectedIndex = 0;
            labelStatus.Text = string.Format("{0} variable(s), {1} time step(s) found.", SFROutput.Variables.Length, SFROutput.NumTimeStep);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_SFROutputPackage == null || _SFROutputPackage.DataCube == null)
            {
                ShowInfo("No data is loaded.");
                return;
            }
            _SFROutputPackage.DataCube.Clear();
            _ProfileMat = null;
            _ProfileRivers = null;
            _DataLoaded = false;
            winChart_timeseries.Clear();
            winChart_proflie.Clear();
            SetBusyState(false, "Content cleared.");
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
            {
                ShowWarning("Please select a feature layer first.");
                return;
            }
            if (cmbDates.SelectedItem == null)
            {
                ShowWarning("Please select a date first.");
                return;
            }
            if (cmbSegFields.SelectedIndex < 0)
            {
                ShowWarning("The field that represents Segment ID must be selected.");
                return;
            }
            if (chbReadComplData.Checked && cmbReachFields.SelectedIndex < 0)
            {
                ShowWarning("The field that represents Reach ID must be selected.");
                return;
            }

            int varIndex;
            string reason;
            if (!TryGetSelectedVariable(out varIndex, out reason))
            {
                ShowWarning(reason);
                return;
            }

            _SelectedFeatureMapLayer = cmbLayers.SelectedItem as FeatureMapLayer;
            if (_SelectedFeatureMapLayer == null)
                return;

            var dt = _SelectedFeatureMapLayer.DataSet.DataTable;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                int failurecount = 0;
                var varname = SFROutput.GetVarAbv(cmbSFRVars.SelectedItem.ToString());
                if (!dt.Columns.Contains(varname))
                {
                    DataColumn col = new DataColumn(varname, typeof(float));
                    dt.Columns.Add(col);
                }
                var vec = SFROutput.DataCube.GetVector(varIndex, cmbDates.SelectedIndex.ToString(), ":");
                if (vec == null || vec.Length == 0)
                {
                    ShowWarning(string.Format("No value is available for \"{0}\" at the selected date.", cmbSFRVars.SelectedItem));
                    return;
                }

                if (chbReadComplData.Checked)
                {
                    var segfield = cmbSegFields.SelectedItem.ToString();
                    var reachfield = cmbReachFields.SelectedItem.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        int segid = 0;
                        int reachid = 0;
                        int.TryParse(dr[segfield].ToString(), out segid);
                        int.TryParse(dr[reachfield].ToString(), out reachid);
                        if (segid <= 0 || reachid <= 0)
                        {
                            failurecount++;
                            continue;
                        }
                        var index = SFROutput.GetReachSerialIndex(segid, reachid);
                        if (index >= 0 && index < vec.Length)
                            dt.Rows[i][varname] = vec[index];
                        else
                            failurecount++;
                    }
                }
                else
                {
                    var segfield = cmbSegFields.SelectedItem.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        int segid = 0;
                        int.TryParse(dr[segfield].ToString(), out segid);
                        if (segid > 0 && segid <= vec.Length)
                            dt.Rows[i][varname] = vec[segid - 1];
                        else
                            failurecount++;
                    }
                }

                if (failurecount > 0)
                {
                    ShowWarning(string.Format("{0} of {1} row(s) in layer \"{2}\" are not updated.\nPlease check that the selected segment/reach ID fields match the SFR network.",
                        failurecount, dt.Rows.Count, _SelectedFeatureMapLayer.LegendText));
                    return;
                }

                if (checkBoxSaveLayer.Checked)
                    _SelectedFeatureMapLayer.DataSet.Save();
                labelStatus.Text = string.Format("{0} row(s) updated in layer \"{1}\".", dt.Rows.Count, _SelectedFeatureMapLayer.LegendText);
            }
            finally
            {
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
            if (SFROutput == null)
            {
                ShowWarning("The SFR output package is not initialized. Please open a model project first.");
                return;
            }
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
        private void mi_flow_Click(object sender, EventArgs e)
        {
            if (SFROutput == null)
            {
                ShowWarning("The SFR output package is not initialized. Please open a model project first.");
                return;
            }

            var items = new ToolStripMenuItem[] { this.mi_flow, this.mi_nps, this.mi_month_npc, this.mi_sediment };
            foreach (ToolStripMenuItem it in items)
            {
                it.CheckState = CheckState.Unchecked;
            }
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
                return;
            item.CheckState = CheckState.Checked;

            var outputDir = Path.Combine(ModelService.WorkDirectory, "output");
            if (item.Name == "mi_flow")
            {
                _curfilename = _sfroutfile;
            }
            else if (item.Name == "mi_nps")
            {
                _curfilename = Path.Combine(outputDir, "sfrwq.dcx");
            }
            else if (item.Name == "mi_sediment")
            {
                _curfilename = Path.Combine(outputDir, "sed_reach.dcx");
            }
            else if (item.Name == "mi_month_npc")
            {
                _curfilename = Path.Combine(outputDir, "reachNP.dcx");
            }
            this.btnScan_Click(this.btnScan, EventArgs.Empty);
        }
    }
}