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

using Excel;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.DatabaseExplorer
{
    public partial class ImportODMData : Form
    {
        private ODMSource _ODM;
        private DataSet _DataSet;
        private DataTable _ODM_Table;
        private DataTable _External_Table;
        private BackgroundWorker worker;
        private IODMTable odm;

        public ImportODMData(ODMSource odm)
        {
            InitializeComponent();
            _ODM = odm;
            cmbTables.Items.AddRange(_ODM.GetODMTableNames());
            toolStripProgressBar1.Visible = false;
            lb_File.Text = "";
            splitContainer1.Panel2Collapsed = true;
            defaultExportToolStripMenuItem.Enabled = false;
            customExportToolStripMenuItem.Enabled = false;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        private void cmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ODM_Table = _ODM.GetDataTable(cmbTables.SelectedItem.ToString());
            this.bindingSourceODM.DataSource = _ODM_Table;
            this.dg_odm.DataSource = this.bindingSourceODM;
            tabControl1.SelectedIndex = 0;
            odm = _ODM.ODMTables[cmbTables.SelectedItem.ToString()];
            propertyGrid1.SelectedObject = odm.ExportSetting;
            if (odm.ExportSetting != null)
            {
                defaultExportToolStripMenuItem.Enabled = true;
                customExportToolStripMenuItem.Enabled = true;
            }
            else
            {
                defaultExportToolStripMenuItem.Enabled = true;
                customExportToolStripMenuItem.Enabled = false;
            }
        }

        private void btnOpenExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "excel 2007~2013 file|*.xlsx|excel 2003 file|*.xls";
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cmbSheet.Items.Clear();
                FileStream stream = File.Open(open.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                excelReader.IsFirstRowAsColumnNames = true;
                _DataSet = excelReader.AsDataSet();
                var tbs = from DataTable dt in _DataSet.Tables select dt.TableName;
                cmbSheet.Items.AddRange(tbs.ToArray());
                cmbSheet.SelectedIndex = 0;
                tabControl1.SelectedIndex = 1;
                lb_File.Text = open.FileName;
            }
        }

        private void cmbSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            _External_Table = _DataSet.Tables[cmbSheet.SelectedIndex];
            this.bindingSourceODM.DataSource = _External_Table;
            this.dg_external.DataSource = this.bindingSourceODM;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var dt = this.bindingSourceODM.DataSource as DataTable;
            if (cmbTables.SelectedItem != null && dt != null)
            {
                odm = _ODM.ODMTables[cmbTables.SelectedItem.ToString()];
                odm.Started += odm_Started;
                odm.Finished += odm_Finished;
                odm.ProgressChanged += odm_ProgressChanged;
                if (odm.Check(dt))
                {
                    worker.RunWorkerAsync(dt);
                    if(odm.Message != "")
                        MessageBox.Show(odm.Message);
                }
                else
                {
                    MessageBox.Show(odm.Message);
                }
            }
            else
            {
                MessageBox.Show("You need to select a table name at first!");
            }
        }

        private   void odm_ProgressChanged(object sender, int e)
        {
            worker.ReportProgress(e);
        }
        private void odm_Started(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate   
            {
                 toolStripProgressBar1.Visible = true;
                 nav_top.Enabled = false;
                 nav_bottom.Enabled = false;
            });       
        }

        private void odm_Finished(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
          {
              toolStripProgressBar1.Visible = false;
              odm.Started -= odm_Started;
              odm.Finished -= odm_Finished;
              odm.ProgressChanged -= odm_ProgressChanged;
              nav_top.Enabled = true;
              nav_bottom.Enabled = true;
              labelStatus.Text = "Ready";
          });
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 0)
            {
                this.bindingSourceODM.DataSource = _ODM_Table;
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                this.bindingSourceODM.DataSource = _External_Table;
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            odm.Save(e.Argument as DataTable);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= toolStripProgressBar1.Minimum && e.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            labelStatus.Text = string.Format("Saving  {0}%", e.ProgressPercentage);
          
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lb_File.Text = "";
        }

        private void btn_script_Click(object sender, EventArgs e)
        {
            if(! TypeConverterEx.IsNull(tb_script.Text))
            {
                _ODM_Table = _ODM.Execute(tb_script.Text);
                if (_ODM_Table != null)
                {
                    this.bindingSourceODM.DataSource = _ODM_Table;
                    this.dg_odm.DataSource = this.bindingSourceODM;
                }
            }
        }

        private void defaultExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cmbTables.SelectedItem != null && _ODM_Table != null)
            {
                odm = _ODM.ODMTables[cmbTables.SelectedItem.ToString()];
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "csv file|*.csv";
                if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    odm.Export(dlg.FileName,_ODM_Table);
                }
            }
        }

        private void customExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cmbTables.SelectedItem != null && _ODM_Table != null)
            {
                odm = _ODM.ODMTables[cmbTables.SelectedItem.ToString()];
                if (odm.ExportSetting == null)
                    return;

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "csv file|*.csv";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    odm.CustomExport(_ODM_Table);
                }
            }
        }

        private void btn_ShowScript_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;

        }

    }
}
