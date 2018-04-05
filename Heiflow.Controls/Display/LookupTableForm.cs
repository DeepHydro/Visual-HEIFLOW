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
using Heiflow.Applications.Views;
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Core.IO;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Waf.Applications;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Display
{
    [Export(typeof(ILookupTableView))]
    public partial class LookupTableForm : Form, ILookupTableView
    {
        private readonly Lazy<LookupTableViewModel> viewModel;
        public LookupTableForm()
        {
            InitializeComponent();
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.SaveButtion);
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.ImportButtion);
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.Save2ExcelButtion);
            dataGridEx1.Navigator.Items.Add(this.labelSP);
            dataGridEx1.Navigator.Items.Add(this.cmbStressPeriod);
            dataGridEx1.Navigator.Items.Add(this.btnSave);
            dataGridEx1.Navigator.Items.Add(this.btnExport);
            dataGridEx1.Navigator.Items.Add(this.btnImport);
            dataGridEx1.Navigator.Items.Add(this.btnClear);
            viewModel = new Lazy<LookupTableViewModel>(() => ViewHelper.GetViewModel<LookupTableViewModel>(this));
            this.Load += ParameterTableForm_Load;
        }
        public string ChildName
        {
            get { return "LookupTableView"; }
        }
        public object DataContext
        {
            get;
            set;
        }
        private void ParameterTableForm_Load(object sender, EventArgs e)
        {
         //  this.dataGridEx1.DataTable = viewModel.Value.LookupTable;
        }

        public void ShowView(IWin32Window pararent)
        {
            this.ShowDialog();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var vm = viewModel.Value;
            vm.Coverage.Processing += LayerParameter_Processing;
            vm.Coverage.Processed += LayerParameter_Processed;
            vm.ShellService.ProgressWindow.DoWork += ProgressPanel_DoWork;
            vm.ShellService.ProgressWindow.DefaultStatusText = "Processing";
            vm.ShellService.ProgressWindow.Run(viewModel.Value.ProjectService.Project.CentroidLayer.DataSet);
        }

        private void LayerParameter_Processing(object sender, int e)
        {
            string msg = string.Format("Processing {0}%", e);
            viewModel.Value.ShellService.ProgressWindow.Progress(e, msg);
        }

        private void LayerParameter_Processed(object sender, object e)
        {
            var vm = viewModel.Value;
            vm.Coverage.Processing -= LayerParameter_Processing;
            vm.Coverage.Processed -= LayerParameter_Processed;
            vm.ShellService.ProgressWindow.DoWork -= ProgressPanel_DoWork;
            this.Invoke((MethodInvoker)delegate { this.Close(); });
        }

        private void ProgressPanel_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var ds = e.Argument as IFeatureSet;
            var lp = viewModel.Value.Coverage as PackageCoverage;
            lp.TargetFeatureSet = ds;
            lp.Map();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //var vm = viewModel.Value;
            //vm.Coverage.Save();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var dt = dataGridEx1.DataTable;
            if (dt == null)
                return;
            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = "csv files|*.csv";
            if (opd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var vm = viewModel.Value;
              //  vm.Coverage.LookupTable = vm.LookupTable;
                vm.ImportFrom(opd.FileName, dt);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var dt = dataGridEx1.DataTable;
            if (dt == null)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "csv file|*.csv";
            sd.FileName = "table.csv";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                CSVFileStream csv = new CSVFileStream(sd.FileName);
                csv.Save(dt);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            var vm = viewModel.Value;
            //vm.Coverage.LookupTable = vm.LookupTable;
            vm.Coverage.Clear();
        }

        public void ClearContent()
        {
            btnClear_Click(null, null);
        }
        public void InitService()
        {

        }
    }
}
