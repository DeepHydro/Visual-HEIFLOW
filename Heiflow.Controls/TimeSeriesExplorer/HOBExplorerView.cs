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

using DotSpatial.Symbology;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using Heiflow.Presentation;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    [Export(typeof(IPackageOptionalView))]
    public partial class HOBExplorerView : Form, IPackageOptionalView
    {
        private IProjectService _ProjectService;
        private IPackage _Package;
        private ToolStripComboBox cmbGwLayers;
        private FeatureMapLayer _SelectedFeatureMapLayer;
        private FHDPackage _fhd;
        private HOBPackage _hob;
        private HOBOutputPackage _hob_out;
        public HOBExplorerView()
        {
            InitializeComponent();
            tbn_compare_tr.Click += tbn_compare_tr_Click;
            tbn_compare_ss.Click += tbn_compare_ss_Click;
            this.timeSeriesExplorer1.SQLSelection = Settings.Default.HOBSQL;
            cmbMapLayers.ComboBox.SelectedIndexChanged += cmbMapLayers_SelectedIndexChanged;
            cmbMapLayers.ComboBox.DisplayMember = "LegendText";
            cmbMapLayers.ComboBox.ValueMember = "DataSet";
            this.timeSeriesExplorer1.MainBar.Items.Add(tbn_compare_tr);
            this.timeSeriesExplorer1.MainBar.Items.Add(tbn_compare_ss);
            this.timeSeriesExplorer1.MainBar.Items.Add(cmbMapLayers);
            this.FormClosing += HOBExplorerView_FormClosing;
            this.timeSeriesExplorer1.LoadPackage_Clicking += timeSeriesExplorer1_Load_Clicking;
            this.timeSeriesExplorer1.SlctDataSource_Clicked += timeSeriesExplorer1_SlctDataSource_Clicked;

            cmbGwLayers = new ToolStripComboBox();
            this.cmbGwLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGwLayers.Name = "cmbGWLayers";
            this.cmbGwLayers.Size = new System.Drawing.Size(121, 28);
            this.cmbGwLayers.ToolTipText = "Select a aquifer layer from which the simulated groudwater head will be loaded";
            cmbGwLayers.SelectedIndexChanged += cmbGwLayers_SelectedIndexChanged;
            timeSeriesExplorer1.MainBar.Items.Add(cmbGwLayers);
        }

        public object DataContext
        {
            get;
            set;
        }

        public string PackageName
        {
            get { return HOBOutputPackage.PackageName; }
        }
      
        public IPackage Package
        {
            get
            {
                return _Package;
            }
            set
            {
                _Package = value;
                if(_Package != null)
                {
                     _hob_out = _Package as HOBOutputPackage;
                     _hob = Package.Parent as HOBPackage;
                }
            }
        }
        public string ChildName
        {
            get { return "HOBExplorerView"; }
        }
        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
            {
                this.Show(pararent);
                _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
                var control = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectController>();
                timeSeriesExplorer1.ODM = _ProjectService.Project.ODMSource;
                this.timeSeriesExplorer1.Package = Package;
                var map_layers = from layer in control.MapAppManager.Map.Layers where layer is IFeatureLayer select new FeatureMapLayer { LegendText = layer.LegendText, DataSet = (layer as IFeatureLayer).DataSet };
                cmbMapLayers.ComboBox.DataSource = map_layers.ToArray();

                cmbGwLayers.Items.Clear();
                for (int i = 0; i < _ProjectService.Project.Model.Grid.LayerCount; i++)
                {
                    cmbGwLayers.Items.Add(i);
                }
                cmbGwLayers.SelectedIndex = 0;
            }
        }
        private void HOBExplorerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
            else
                e.Cancel = false;
        }
        private bool timeSeriesExplorer1_Load_Clicking(object sender, EventArgs e)
        {
            bool can_load = false;
            if (Package.Owner.Packages.Keys.Contains(MFOutputPackage.PackageName) && cmbGwLayers.SelectedIndex >= 0)
            {
                if (_fhd == null)
                    GetFHD();
                if (_fhd.Values == null || _fhd.Values.Value[cmbGwLayers.SelectedIndex] == null)
                {
                    MessageBox.Show("You need to load groundwater head from FHD pacakage at first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    can_load = true;
                }
            }

            return can_load;
        }
        private void cmbMapLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var msg = MyAppManager.Instance.CompositionContainer.GetExportedValue<IMessageService>();
            _SelectedFeatureMapLayer = cmbMapLayers.ComboBox.SelectedItem as FeatureMapLayer;
            if (_SelectedFeatureMapLayer == null)
                return;
            try
            {
                HOBPackage hob = new HOBPackage();
                hob.Owner = Package.Owner;
                var sites = hob.ExtractFrom(_SelectedFeatureMapLayer.DataSet.DataTable);
                if (sites != null)
                {
                    (Package as HOBOutputPackage).Sites = sites;
                }
                else
                {
                    var  _hob = Package.Parent as HOBPackage;
                   // msg.ShowError(null, "No sites are found in the selected layer. Default sites will be used, but you can not compare simulated values with observed values" );
                    (Package as HOBOutputPackage).Sites = _hob.Observations;
                }
                 if (_fhd == null)
                    GetFHD();
              
            }
            catch (Exception ex)
            {   
                msg.ShowError(null, "Failed to extract observation sites from the selected layer. Error message: " + ex.Message);
                return;
            }
        }
        private void cmbGwLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var hob_out = Package as HOBOutputPackage;
            hob_out.SelectedLayerIndex = cmbGwLayers.SelectedIndex;
        }
        private void tbn_compare_tr_Click(object sender, EventArgs e)
        {
            var hob_out = Package as HOBOutputPackage;
            var hob = hob_out.Parent as HOBPackage;
            if (hob_out.Values == null || hob_out.Sites == null)
                return;

            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            if (hob_out.ComparingValues == null)
            {
                int nsites = hob_out.Sites.Count;

                var heads = hob_out.GetHeads(timeSeriesExplorer1.SelectedVariableIndex, 0);
                List<float> obs_list = new List<float>();
                List<float> sim_list = new List<float>();
                for (int i = 0; i < nsites; i++)
                {
                    if (timeSeriesExplorer1.ODM != null)
                    {
                        var ts = timeSeriesExplorer1.ODM.GetTimeSeries(new QueryCriteria()
                        {
                            Start = hob_out.StartOfLoading,
                            End = hob_out.EndOfLoading,
                            SiteID = hob_out.Sites[i].ID,
                            VariableID = hob_out.ODMVariableID
                        });
                        if (ts != null)
                        {
                            if (ts.Value[0] > 0)
                            {
                                obs_list.Add((float)ts.Value[0]);
                                sim_list.Add(heads[i]);
                            }
                        }
                    }
                }

                hob_out.ComparingValues = new MyLazy3DMat<float>(2, 1, obs_list.Count);
                hob_out.ComparingValues.Allocate(0);
                hob_out.ComparingValues.Allocate(1);

                hob_out.ComparingValues.Value[0][0] = obs_list.ToArray();
                hob_out.ComparingValues.Value[1][0] = sim_list.ToArray();
            }


            var obs = hob_out.ComparingValues.Value[0][0];
            var sim = hob_out.ComparingValues.Value[1][0];

            if (obs.Length > 0)
            {
                var min = Math.Min(obs.Min(), sim.Min());
                var max = Math.Max(obs.Max(), sim.Max());

                var xx = new float[] { min, max };
                var yy = new float[] { min, max };

                timeSeriesExplorer1.Chart.Plot<float>(xx, yy, "45 Degree Line", SeriesChartType.FastLine);
                timeSeriesExplorer1.Chart.Plot<float>(obs, sim, 0, "Observed VS. Simulated Head", SeriesChartType.FastPoint);
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }
        private void tbn_compare_ss_Click(object sender, EventArgs e)
        {
            var hob_out = Package as HOBOutputPackage;
            var hob = hob_out.Parent as HOBPackage;
            if (hob_out.Values == null || hob_out.Sites == null || _fhd == null)
                return;
            if (_SelectedFeatureMapLayer == null || hob_out.Values == null)
                return;

            var dt = _SelectedFeatureMapLayer.DataSet.DataTable;

            int nsites = hob_out.Sites.Count;
            var var_index = timeSeriesExplorer1.SelectedVariableIndex;
            string[] mandatory_fields = new string[] { "HOBS", "HSIM", "DepOBS", "DepSIM", "DepDIF", "HDIF" };
            foreach (string field in mandatory_fields)
            {
                if (!dt.Columns.Contains(field))
                {
                    DataColumn dc = new DataColumn(field, Type.GetType("System.Double"));
                    dt.Columns.Add(dc);
                }
            }
            var obs = new double[nsites];
            var sim = new double[nsites];
            for (int i = 0; i < nsites; i++)
            {
                var cell_elev = float.Parse(dt.Rows[i]["Elevation"].ToString());    
                var hobs = float.Parse(dt.Rows[i]["HOBS"].ToString());
                var hsim = hob_out.Values.Value[var_index][0][i];
                dt.Rows[i]["HSIM"] = Math.Round(hsim, 2);
                dt.Rows[i]["HDIF"] = Math.Round((hobs - hob_out.Values.Value[var_index][0][i]), 2);

                var dep_sim = cell_elev - hsim;
                var dep_obs = float.Parse(dt.Rows[i]["DepOBS"].ToString());
                dt.Rows[i]["DepSIM"] = Math.Round(dep_sim);
                dt.Rows[i]["DepDIF"] = Math.Round((dep_obs - dep_sim), 2);
                obs[i] = dep_obs;
                sim[i] = dep_sim;
            }


            var min = Math.Min(obs.Min(), sim.Min());
            var max = Math.Max(obs.Max(), sim.Max());

            var xx = new double[] { min, max };
            var yy = new double[] { min, max };

            timeSeriesExplorer1.Chart.Plot<double>(xx, yy, "45 Degree Line", SeriesChartType.FastLine);
            timeSeriesExplorer1.Chart.Plot<double>(obs, sim, 0, "Observed VS. Simulated Depth to GW", SeriesChartType.FastPoint);

            _SelectedFeatureMapLayer.DataSet.Save();

        }
        private void timeSeriesExplorer1_SlctDataSource_Clicked(object sender, EventArgs e)
        {
            SQLSelection sql = new SQLSelection(Settings.Default.HOBSQL)
            {
                ODM = this._ProjectService.Project.ODMSource
            };
            if (sql.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Default.HOBSQL = sql.SQLScript;
                Settings.Default.Save();
            }
        }

        private void GetFHD()
        {
            var buf = from pp in Package.Owner.Packages[MFOutputPackage.PackageName].Children where pp.Name == FHDPackage.PackageName select pp;
            if (buf.Count() == 1)
            {
                _fhd = buf.First() as FHDPackage;
                //_hob_out.NumTimeStep = _fhd.NumTimeStep;
                //_hob_out.StartOfLoading = _fhd.StartOfLoading;
                //_hob_out.EndOfLoading = _fhd.EndOfLoading;                
                //_hob_out.MaxTimeStep = _hob_out.MaxTimeStep;
            }
        }

        public void ClearContent()
        {
            this.timeSeriesExplorer1.ClearContent();
        }
        public void InitService()
        {

        }
    }
}