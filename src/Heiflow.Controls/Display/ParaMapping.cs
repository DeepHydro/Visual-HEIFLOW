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

using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Applications.ViewModels;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
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
    public partial class ParaMapping : Form, ICoverageSetupView
    {
        private IPackage _package;
        private PackageCoverage _coverage;
        private BackgroundWorker worker;

        public ParaMapping()
        {
            InitializeComponent();
            cmbPackages.DisplayMember = "Name";
            chbProp.DisplayMember = "PropertyName";
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
            get { return "ParaMappingView"; }
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
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var pcks = prj.Project.Model.GetPackages();
            int layer_count = prj.Project.Model.Grid.ActualLayerCount;
            var cov_pcks = new List<IPackage>();
            labelStatus.Text = "Ready";
            foreach (var pck in pcks)
            {
                var atr = pck.GetType().GetCustomAttributes(typeof(CoverageItem), false);
                if (atr.Length == 1)
                {
                    cov_pcks.Add(pck);
                }
            }
            cmbPackages.DataSource = cov_pcks;
            chbProp.Items.Clear();

            _coverage = new RasterCoverage()
                      {
                          ID = DirectoryHelper.GetUniqueString()
                      };
        }
        private void cmbPackages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPackages.SelectedItem == null)
                return;

            chbProp.Items.Clear();
            _package = cmbPackages.SelectedItem as IPackage;

            var props = _package.GetType().GetProperties();
            foreach (var pr in props)
            {
                var atr = pr.GetCustomAttributes(typeof(ArealProperty), false);
                if (atr.Length == 1)
                {
                    var ap = atr[0] as ArealProperty;
                    if (ap.ElementType == typeof(IParameter))
                    {
                        var paras = pr.GetValue(_package) as Dictionary<string, IParameter>;
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

        private void SelectedCoverage_Processing(object sender, int e)
        {
            worker.ReportProgress(e);
        }
        private void SelectedCoverage_Processed(object sender, string e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                toolStripProgressBar1.Visible = false;
                btnMap.Enabled = true;
                labelStatus.Text = e;
            });
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] objs = e.Argument as object[];
            _coverage.MapByZone(objs[0] as Tuple<int, int, int>[], objs[1] as Dictionary<string, Tuple<int, int, float>[]>);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= toolStripProgressBar1.Minimum && e.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            //do not update the text if a cancellation request is pending
            labelStatus.Text = string.Format("Processing  {0}%", e.ProgressPercentage);
        }

        private void btnImportZone_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "csv file|*.csv|all files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CSVFileStream csv = new CSVFileStream(ofd.FileName);
                csv.HasHeader = true;
                var dt = csv.Load();
                bindingSourceZone.DataSource = dt;
                dataGridViewZone.DataSource = bindingSourceZone;
            }
        }

        private void btnImportLookup_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "csv file|*.csv|all files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CSVFileStream csv = new CSVFileStream(ofd.FileName);
                csv.HasHeader = true;
                var dt = csv.Load();
                _coverage.LookupTable = dt;
                bindingSourceLookup.DataSource = _coverage.LookupTable;
                dataGridViewLookup.DataSource = bindingSourceLookup;
            }
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            Dictionary<string, Tuple<int, int, float>[]> lookup_dt = null;
            Tuple<int, int, int>[] zone_dt = null;
            var dtzone = bindingSourceZone.DataSource as DataTable;

            if (chbProp.CheckedItems.Count == 0)
            {
                MessageBox.Show("No areal properties selected", "Areal Properties Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_coverage.LookupTable == null)
            {
                MessageBox.Show("Lookup Table not imported", "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                lookup_dt = _coverage.ConvertLookupTable(_coverage.LookupTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Lookup Table. Error message: " + ex.Message, "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtzone == null)
            {
                MessageBox.Show("Zone ID Table not imported", "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                zone_dt = _coverage.ConvertZoneTable(dtzone);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Zone ID Table . Error message: " + ex.Message, "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var props = chbProp.CheckedItems.Cast<ArealPropertyInfo>().ToList();
            string[] paras = null;
            if (props[0].IsParameter)
            {
                paras = (from pr in props select pr.ParameterName).ToArray();
            }
            else
            {
                paras = (from pr in props select pr.PropertyName).ToArray();
            }
            bool has_all = true;
            bool do_map = true;
            string missing_pa = "";
            foreach(var pa in paras)
            {
                if(!lookup_dt.Keys.Contains(pa))
                {
                    has_all = false;
                    missing_pa += pa + ",";
                }
            }
            if (!has_all)
            {
                missing_pa = missing_pa.Trim(new char[] { ',' });
                if (MessageBox.Show("The follwing parameters are not found in the lookup talbe: " + missing_pa + ". Do yo really want to continue?", "Template File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    do_map = true;
                }
                else
                {
                    do_map = false;
                }
            }

            if (do_map)
            {
                _coverage.ArealProperties = props;
                _coverage.PackageName = _package.Name;
                _coverage.Package = _package;

                btnMap.Enabled = false;
                _coverage.Processing += SelectedCoverage_Processing;
                _coverage.Processed += SelectedCoverage_Processed;
                worker.RunWorkerAsync(new object[] { zone_dt, lookup_dt });
            }
        }

        private void btnLocateLookTemplate_Click(object sender, EventArgs e)
        {
            var file = Path.GetFullPath(Path.Combine(Application.StartupPath, Heiflow.Controls.WinForm.Properties.Settings.Default.LookupTableTemplateFile));
            if (File.Exists(file))
                System.Diagnostics.Process.Start(file);
            else
                MessageBox.Show("The template file is misssing: " + file, "Template File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnZoneTemplateFile_Click(object sender, EventArgs e)
        {
            var file = Path.GetFullPath(Path.Combine(Application.StartupPath, Heiflow.Controls.WinForm.Properties.Settings.Default.ZoneMapTemplateFile));
            if (File.Exists(file))
                System.Diagnostics.Process.Start(file);
            else
                MessageBox.Show("The template file is misssing: " + file, "Template File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnLocateDic_Click(object sender, EventArgs e)
        {
            var file = Path.Combine(Application.StartupPath, Heiflow.Controls.WinForm.Properties.Settings.Default.LookupTableTemplateFile);
            if (File.Exists(file))
                System.Diagnostics.Process.Start("Explorer", "/select," + file);
            else
                MessageBox.Show("The template file is misssing: " + file, "Template File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnLocateZoneDic_Click(object sender, EventArgs e)
        {
            var file = Path.Combine(Application.StartupPath, Heiflow.Controls.WinForm.Properties.Settings.Default.ZoneMapTemplateFile);
            if (File.Exists(file))
                System.Diagnostics.Process.Start("Explorer", "/select," + file);
            else
                MessageBox.Show("The template file is misssing: " + file, "Template File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void bntExportLook_Click(object sender, EventArgs e)
        {
            if (chbProp.CheckedItems.Count == 0)
            {
                MessageBox.Show("No areal properties selected", "Areal Properties Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.FileName = "Lookup_table_" + _package.Name + ".csv";
            ofd.Filter = "csv file|*.csv|all files|*.*";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               var props =  chbProp.CheckedItems.Cast<ArealPropertyInfo>().ToList();
               string[] paras = null;
                if(props[0].IsParameter)
                {
                    paras = (from pr in props select pr.ParameterName).ToArray();
                }
                else
                {
                    paras = (from pr in props select pr.PropertyName).ToArray();
                }
                StreamWriter sw = new StreamWriter(ofd.FileName);
                string header = string.Format("LAYER_ID,ZONE_ID,{0}", string.Join(",", paras));
                sw.WriteLine(header);
                sw.Close();
            }
        }

        private void btnExportZone_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.FileName = "Zone_table_" + _package.Name + ".csv";
            ofd.Filter = "csv file|*.csv|all files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(ofd.FileName);
                string header = "LAYER_ID,HRU_ID,ZONE_ID";
                sw.WriteLine(header);
                sw.Close();
            }
        }
    }
}