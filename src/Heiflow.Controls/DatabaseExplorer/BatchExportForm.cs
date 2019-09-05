using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.IO;
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
    public partial class BatchExportForm : Form
    {
        public BatchExportForm(ODMSource odm )
        {
            InitializeComponent();
            _source = odm;
        }

        private ODMSource _source;
        private Site [] _sites;
        private QueryCriteria queryCriteria;

        private void BatchExportForm_Load(object sender, EventArgs e)
        {
            dateTimePickerStart.Value = new DateTime(2000, 1, 1);
            dateTimePickerEnd.Value = DateTime.Now;
            var varibs = _source.GetVariables();
            cmbVariables.DataSource = varibs;
            cmbVariables.DisplayMember = "Name";
            cmbVariables.ValueMember = "ID";
            cmbVariables.SelectedIndex = 0;
        }

        private void btnGetSites_Click(object sender, EventArgs e)
        {
            queryCriteria = new QueryCriteria()
            {
                 Start= dateTimePickerEnd.Value,
                 End= dateTimePickerStart.Value,
                 VariableName = (cmbVariables.SelectedItem as Variable).Name,
                 VariableID = (cmbVariables.SelectedItem as Variable).ID
            };

            _sites = _source.GetSites(queryCriteria);
            bindingSourceODM.DataSource = _sites;
            dg_site.DataSource = bindingSourceODM;
        }

        private void btnOpenDic_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                tbPath.Text= fbd.SelectedPath;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string dic = tbPath.Text;
            if(TypeConverterEx.IsNotNull(dic) && _sites != null)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (var site in _sites)
                {
                    var series = new ObservationSeries()
                    {
                        SiteID = site.ID,
                        VariableID = queryCriteria.VariableID,
                        Start = dateTimePickerStart.Value,
                        End = dateTimePickerEnd.Value
                    };
                    var ts = _source.GetTimeSereis(series);
                    if (ts != null)
                    {
                        var filename = Path.Combine(dic, site.Name + ".csv");
                        ts.Name = queryCriteria.VariableName;
                        CSVFileStream csv = new CSVFileStream(filename);
                        csv.Save(ts);
                    }
                }
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Finshied", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Can not export because no sites are selected or output directory is not set.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
