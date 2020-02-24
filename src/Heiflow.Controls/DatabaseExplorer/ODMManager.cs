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
using DotSpatial.Projections;
using Excel;
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.DatabaseExplorer
{
    public partial class ODMManager : Form
    {
        private ODMSource _ODM;
        private System.Data.DataSet _DataSet;
        private DataTable _ODM_Table;
        private DataTable _External_Table;
        private BackgroundWorker worker;
        private IODMTable odm;
        private string _currentTableName = "";
        private int _ExcelRowCount = 0;

        public ODMManager(ODMSource odm)
        {
            InitializeComponent();
            _ODM = odm;
            cmbTable.Items.AddRange(_ODM.GetODMTableNames());
            var tables= _ODM.GetODMTableNames();
            foreach (var table in tables)
                lvDataTables.Items.Add(table);
            toolStripProgressBar1.Visible = false;
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

 
        private void lvDataTables_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvDataTables.SelectedItems.Count > 0)
            {
                ListViewItem item = lvDataTables.SelectedItems[0];
                _currentTableName = item.Text.ToString();
                _ODM_Table = _ODM.GetDataTable(_currentTableName);
                this.bindingSourceODM.DataSource = _ODM_Table;
                this.dg_odm.DataSource = bindingSourceODM;
                btnExportSiteAsShp.Enabled = _currentTableName == "Sites";
            }
        }
        private void btnOpenExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "excel 2007~2013 file|*.xlsx|excel 2003 file|*.xls";
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cmbSheets.Items.Clear();
                FileStream stream = File.Open(open.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                excelReader.IsFirstRowAsColumnNames = true;
                _DataSet = excelReader.AsDataSet();
                var tbs = from DataTable dt in _DataSet.Tables select dt.TableName;
                cmbSheets.Items.AddRange(tbs.ToArray());
                cmbSheets.SelectedIndex = 0;
                tabControl1.SelectedIndex = 1;
                tbExcelFile.Text = open.FileName;
            }
        }

        private void cmbSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_DataSet != null)
            {
                _External_Table = _DataSet.Tables[cmbSheets.SelectedIndex];
                this.bindingSourceExcel.DataSource = _External_Table;
                this.dg_external.DataSource = this.bindingSourceExcel;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var dt = this.bindingSourceExcel.DataSource as DataTable;
            if (cmbTable.SelectedItem != null && dt != null)
            {
                _ExcelRowCount = dt.Rows.Count;
                odm = _ODM.ODMTables[cmbTable.SelectedItem.ToString()];
                odm.Started += odm_Started;
                odm.Finished += odm_Finished;
                odm.ProgressChanged += odm_ProgressChanged;
                if (odm.Check(dt))
                {
                    worker.RunWorkerAsync(dt);
                    if (odm.Message != "")
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

        private void odm_ProgressChanged(object sender, int e)
        {
            worker.ReportProgress(e);
        }
        private void odm_Started(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                toolStripProgressBar1.Visible = true;
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
              lbStatus.Text = _ExcelRowCount + " records are saved to Database";
          });
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            odm.Save(e.Argument as DataTable);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= toolStripProgressBar1.Minimum && e.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = e.ProgressPercentage;
            lbStatus.Text = string.Format("Saving  {0}%", e.ProgressPercentage); 

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lbStatus.Text = "";
        }

        private void btn_script_Click(object sender, EventArgs e)
        {
            if (!TypeConverterEx.IsNull(tb_script.Text))
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
            if (_currentTableName != ""  && _ODM_Table != null)
            {
                odm = _ODM.ODMTables[_currentTableName];
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "csv file|*.csv";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    odm.Export(dlg.FileName, _ODM_Table);
                }
            }
        }

        private void customExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchExportForm form = new BatchExportForm(_ODM);
            form.ShowDialog();
        }

        private void btn_ShowScript_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;

        }

        private void btnUpdateSeriesCata_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            _ODM.UpdateSeriesCatalog();
            Cursor.Current = Cursors.Default;
        }

        private void btnExportSiteAsShp_Click(object sender, EventArgs e)
        {
            if (_ODM_Table != null && _ODM_Table.Columns.Contains("SiteID"))
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "site.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var wgs84 = ProjectionInfo.FromEpsgCode(4326);
                    IFeatureSet fs = new FeatureSet(FeatureType.Point);
                    fs.Name = "sites";
                    //      "SiteID", "Longitude", "Latitude", "Elevation_m",  "SiteType", "SiteName",  "State"
                    fs.DataTable.Columns.Add(new DataColumn("SiteID", typeof(int)));
                    fs.DataTable.Columns.Add(new DataColumn("SiteName", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("Longitude", typeof(double)));
                    fs.DataTable.Columns.Add(new DataColumn("Latitude", typeof(double)));
                    fs.DataTable.Columns.Add(new DataColumn("Elevation_m", typeof(double)));
                    fs.DataTable.Columns.Add(new DataColumn("SiteType", typeof(string)));
                    fs.Projection = wgs84;
                    var lon = 0.0;
                    var lat = 0.0;
                    foreach (DataRow dr in _ODM_Table.Rows)
                    {
                        lon = double.Parse(dr["Longitude"].ToString());
                        lat = double.Parse(dr["Latitude"].ToString());
                        NetTopologySuite.Geometries.Point geom = new NetTopologySuite.Geometries.Point(lon, lat);
                        IFeature feature = fs.AddFeature(geom);
                        feature.DataRow.BeginEdit();
                        feature.DataRow["SiteID"] = dr["SiteID"];
                        feature.DataRow["SiteName"] = dr["SiteName"];
                        feature.DataRow["Longitude"] = dr["Longitude"];
                        feature.DataRow["Latitude"] = dr["Latitude"];
                        feature.DataRow["Elevation_m"] = dr["Elevation_m"];
                        feature.DataRow["SiteType"] = dr["SiteType"];
                        feature.DataRow.EndEdit();
                    }
                    fs.SaveAs(dlg.FileName, true);
                }
            }
        }

        private void btnTemplate_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "data");
            Process.Start("explorer.exe", path);
        }


    }

}