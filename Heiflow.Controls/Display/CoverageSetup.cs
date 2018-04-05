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

using DotSpatial.Data;
using Heiflow.Applications.ViewModels;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Display
{
    [Export(typeof(ICoverageSetupView))]
    public partial class CoverageSetup : Form, ICoverageSetupView
    {
        private readonly Lazy<CoverageSetupViewModel> viewModel;
        private IPackage _SelectedPackage;
        private PackageCoverage _SelectedCoverage;
        private BackgroundWorker worker;

        public CoverageSetup()
        {
            InitializeComponent();
            cmbPackages.DisplayMember = "Name";
            cmbMapLayers.DisplayMember = "LegendText";
            cmbMapLayers.ValueMember = "DataSet";
            chbProp.DisplayMember = "PropertyName";
            viewModel = new Lazy<CoverageSetupViewModel>(() => ViewHelper.GetViewModel<CoverageSetupViewModel>(this));
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        }

        public object DataContext
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return "CoverageSetupView"; }
        }
        public void ShowView(IWin32Window parent)
        {
            if (!this.Visible)
                this.ShowDialog(parent);
        }
        public void ClearContent()
        {
            clearToolStripMenuItem_Click(null, null);
        }
        public void InitService()
        {

        }

        private void CoverageSetup_Load(object sender, EventArgs e)
        {
            var pc = viewModel.Value.ProjectController;
            _SelectedCoverage = viewModel.Value.Coverage;
            var pcks = pc.Project.Model.GetPackages();
            int layer_count = pc.Project.Model.Grid.ActualLayerCount;
            int[] grid_layers = new int[layer_count];
            var cov_pcks = new List<IPackage>();
            var buf = from layer in pc.MapAppManager.Map.Layers select new MapLayerDescriptor { LegendText = layer.LegendText, DataSet = layer.DataSet };
            var map_layers = buf.ToList();
            labelStatus.Text = "Ready";
            for (int i = 0; i < layer_count; i++)
            {
                grid_layers[i] = i + 1;
            }
            foreach (var pck in pcks)
            {
                var atr = pck.GetType().GetCustomAttributes(typeof(CoverageItem), false);
                if (atr.Length == 1)
                {
                    cov_pcks.Add(pck);
                }
            }
            cmbGridLayer.SelectedIndexChanged -= cmbGridLayer_SelectedIndexChanged;
            cmbMapLayers.SelectedIndexChanged -= cmbMapLayers_SelectedIndexChanged;

            cmbMapLayers.DataSource = map_layers;
            cmbGridLayer.DataSource = grid_layers;
            cmbPackages.DataSource = cov_pcks;
            chbProp.Items.Clear();

            if (_SelectedCoverage != null)
            {
                tbCoverageName.Text = _SelectedCoverage.LegendText;
                _SelectedCoverage.LoadLookTable();
                dataGridEx1.Bind(_SelectedCoverage.LookupTable);

                //restore cmbMapLayers
             
                for (int i = 0; i < map_layers.Count; i++)
                {
                    string fn = map_layers[i].DataSet.Filename;
                    if (DirectoryHelper.IsRelativePath(fn))
                    {
                        fn = Path.Combine(pc.Project.AbsolutePathToProjectFile, fn);
                    }
                    if (DirectoryHelper.Compare(fn, _SelectedCoverage.FullCoverageFileName))
                    {
                        cmbMapLayers.SelectedIndex = i;
                        var layer = map_layers[i];
                        if (layer.DataSet is FeatureSet)
                        {
                            var fs = layer.DataSet as FeatureSet;
                            var fields = (from DataColumn dc in fs.DataTable.Columns select dc.ColumnName).ToList();
                            cmbFields.DataSource = fields;
                            cmbFields.SelectedIndex = fields.IndexOf(_SelectedCoverage.FieldName);
                        }
                        else
                        {
                            cmbFields.DataSource = new string [] {"Raster Value"};
                            cmbFields.SelectedIndex = 0;
                        }
                        break;
                    }
                }
                for (int i = 0; i < cov_pcks.Count; i++)
                {
                    if (cov_pcks[i].Name == _SelectedCoverage.PackageName)
                    {
                        cmbPackages.SelectedIndex = i;
                        break;
                    }
                }
            }
         
            if (_SelectedCoverage != null)
            {
                cmbGridLayer.SelectedIndex = _SelectedCoverage.GridLayer;
            }
            else
            {
                tbCoverageName.Text = "new_coverage";
                cmbGridLayer.SelectedIndex = 0;
                dataGridEx1.ClearContent();
            }

            cmbMapLayers.SelectedIndexChanged += cmbMapLayers_SelectedIndexChanged;
            cmbGridLayer.SelectedIndexChanged += cmbGridLayer_SelectedIndexChanged;
        }
        private void cmbPackages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPackages.SelectedItem == null)
                return;

            chbProp.Items.Clear();
            _SelectedPackage = cmbPackages.SelectedItem as IPackage;

            var props = _SelectedPackage.GetType().GetProperties();
            foreach (var pr in props)
            {
                var atr = pr.GetCustomAttributes(typeof(ArealProperty), false);
                if (atr.Length == 1)
                {
                    var ap = atr[0] as ArealProperty;
                    if (ap.ElementType == typeof(IParameter))
                    {
                        var paras = pr.GetValue(_SelectedPackage) as Dictionary<string, IParameter>;
                        foreach (var para in paras.Values)
                        {
                            if (para.ValueCount == ModelService.NHRU)
                            {
                                chbProp.Items.Add(new ArealPropertyInfo()
                                {
                                    PropertyName = para.Name,
                                    TypeName = para.GetVariableType().FullName,
                                    DefaultValue = para.DefaultValue,
                                    Parameter = para,
                                    ParameterName = para.Name
                                }, true);
                            }
                        }
                    }
                    else
                    {
                        chbProp.Items.Add(new ArealPropertyInfo()
                        {
                            PropertyName = pr.Name,
                            TypeName = ap.ElementType.FullName,
                            DefaultValue = ap.DefaultValue,
                            ParameterName = "null"
                        },
                        true);
                    }
                }
            }
            if (_SelectedCoverage != null)
            {
                for (int i = 0; i < chbProp.Items.Count; i++)
                {
                    chbProp.SetItemChecked(i, false);
                    var buf = _SelectedCoverage.ArealProperties.Where(p => p.PropertyName == (chbProp.Items[i] as ArealPropertyInfo).PropertyName);
                    if (buf.Any())
                    {
                        chbProp.SetItemChecked(i, true);
                    }
                }
            }
        }
        private void cmbMapLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var layer = cmbMapLayers.SelectedItem as MapLayerDescriptor;
            if (layer.DataSet is FeatureSet)
            {
                var fs = layer.DataSet as FeatureSet;
                var fields = from DataColumn dc in fs.DataTable.Columns select dc.ColumnName;
                cmbFields.DataSource = fields.ToList();
                cmbFields.Enabled = true;
            }
            else
            {
                cmbFields.DataSource = new string[] { "Raster Value" };
                cmbFields.SelectedIndex = 0;
                cmbFields.Enabled = false;
            }
        }
        private void cmbGridLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_SelectedCoverage != null)
                _SelectedCoverage.GridLayer = cmbGridLayer.SelectedIndex;
        }
        private void btnCreateLookupTable_Click(object sender, EventArgs e)
        {
            try
            {
                var pc = viewModel.Value.ProjectController;
                if (_SelectedPackage == null)
                {
                    MessageBox.Show("No package selected.", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (chbProp.CheckedItems.Count == 0)
                {
                    MessageBox.Show("No areal properties selected. Please select at least one.", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbMapLayers.SelectedItem == null)
                {
                    MessageBox.Show("A map layer must be selected", "Layer Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbFields.SelectedItem == null)
                {
                    MessageBox.Show("A field must be selected", "Field Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var fea = cmbMapLayers.SelectedItem as MapLayerDescriptor;

                if (_SelectedCoverage != null)
                {
                    if (MessageBox.Show("Lookup table existed. Do you really want to create new lookup table?",
                        "Layer Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        _SelectedCoverage.ArealProperties = chbProp.CheckedItems.Cast<ArealPropertyInfo>().ToList();
                        _SelectedCoverage.LegendText = tbCoverageName.Text;
                        _SelectedCoverage.Package = _SelectedPackage;
                        _SelectedCoverage.Source = fea.DataSet;
                        _SelectedCoverage.GridLayer = cmbGridLayer.SelectedIndex;
                        _SelectedCoverage.FieldName = cmbFields.SelectedItem.ToString();
                        _SelectedCoverage.CoverageFilePath = fea.DataSet.Filename;
                        _SelectedCoverage.InitLookupTable();
                        _SelectedCoverage.SaveLookupTable();
                        dataGridEx1.Bind(_SelectedCoverage.LookupTable);
                    }
                }
                else
                {
                    if (fea.DataSet is IFeatureSet)
                    {
                        _SelectedCoverage = new FeatureCoverage()
                       {
                           LegendText = tbCoverageName.Text,
                           ID = DirectoryHelper.GetUniqueString(),
                           ArealProperties = chbProp.CheckedItems.Cast<ArealPropertyInfo>().ToList(),
                           PackageName = _SelectedPackage.Name,
                           Package = _SelectedPackage,
                           Source = fea.DataSet,
                           GridLayer = cmbGridLayer.SelectedIndex,
                           CoverageFilePath = fea.DataSet.Filename,
                           FieldName = cmbFields.SelectedItem.ToString()
                       };
                        _SelectedCoverage.LookupTableFilePath = string.Format(".\\Processing\\fea_{0}_{1}.map", _SelectedCoverage.PackageName, _SelectedCoverage.ID);
                        _SelectedCoverage.InitLookupTable();
                        _SelectedCoverage.SaveLookupTable();
                        dataGridEx1.Bind(_SelectedCoverage.LookupTable);
                        pc.Project.FeatureCoverages.Add(_SelectedCoverage as FeatureCoverage);
                    }
                    else
                    {
                        _SelectedCoverage = new RasterCoverage()
                       {
                           LegendText = tbCoverageName.Text,
                           ID = DirectoryHelper.GetUniqueString(),
                           ArealProperties = chbProp.CheckedItems.Cast<ArealPropertyInfo>().ToList(),
                           PackageName = _SelectedPackage.Name,
                           Package = _SelectedPackage,
                           Source = fea.DataSet as IRaster,
                           GridLayer = cmbGridLayer.SelectedIndex,
                           CoverageFilePath = fea.DataSet.Filename,
                           FieldName = cmbFields.SelectedItem.ToString()
                       };
                        _SelectedCoverage.LookupTableFilePath = string.Format(".\\Processing\\fea_{0}_{1}.map", _SelectedCoverage.PackageName, _SelectedCoverage.ID);
                        _SelectedCoverage.InitLookupTable();
                        _SelectedCoverage.SaveLookupTable();
                        dataGridEx1.Bind(_SelectedCoverage.LookupTable);
                        pc.Project.RasterLayerCoverages.Add(_SelectedCoverage as RasterCoverage);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to creat lookup table", "Creation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chbProp.Items.Count; i++)
                chbProp.SetItemChecked(i, true);
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chbProp.Items.Count; i++)
                chbProp.SetItemChecked(i, false);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_SelectedCoverage != null && _SelectedCoverage.LookupTable != null)
                _SelectedCoverage.SaveLookupTable();
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (_SelectedCoverage != null && _SelectedCoverage.LookupTable != null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "csv file|*.csv|all files|*.*";
                if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _SelectedCoverage.ImportFrom(ofd.FileName);
                }
            }
            else
            {
                MessageBox.Show("You should creat lookup table at first", "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            if (_SelectedCoverage != null && _SelectedCoverage.LookupTable != null)
            {
                _SelectedCoverage.TargetFeatureSet = viewModel.Value.ProjectController.Project.CentroidLayer.FeatureSet;
                toolStripProgressBar1.Visible = true;
                toolStrip1.Enabled = false;
                tabControlLeft.Enabled = false;
                _SelectedCoverage.Processing += SelectedCoverage_Processing;
                _SelectedCoverage.Processed += SelectedCoverage_Processed;
                worker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("You should creat lookup table at first", "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void SelectedCoverage_Processing(object sender, int e)
        {
            worker.ReportProgress(e);
        }
        private void SelectedCoverage_Processed(object sender, string e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                toolStripProgressBar1.Visible = false;
                toolStrip1.Enabled = true;
                tabControlLeft.Enabled = true;
                labelStatus.Text = e;
            });
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _SelectedCoverage.Map();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= toolStripProgressBar1.Minimum && e.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            //do not update the text if a cancellation request is pending
            labelStatus.Text = string.Format("Processing  {0}%", e.ProgressPercentage);
        }
    }
}
