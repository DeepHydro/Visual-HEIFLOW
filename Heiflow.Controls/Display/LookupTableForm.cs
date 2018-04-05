// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
