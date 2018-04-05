// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications;
using Heiflow.Core.IO;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Presentation.Services;
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

namespace Heiflow.Controls.WinForm.Processing
{
    public partial class CascadeForm : Form
    {
        private Cascade _Cascade;
        private IProjectService _ProjectService;
        public CascadeForm()
        {
            InitializeComponent();
            _Cascade = new Cascade();
            this.Load += CascadeForm_Load;
        }

        private void CascadeForm_Load(object sender, EventArgs e)
        {
            _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            this.bindingSource1.DataSource = _Cascade.NewOutletTable();
            this.dataGridView1.DataSource = bindingSource1;
            if(_ProjectService != null)
                _Cascade.Initialize(_ProjectService.Project);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var crt_path = Path.Combine(Application.StartupPath, "Models\\CRT_1.1.1.exe");
            if (_ProjectService.Project == null)
                return;

            if (!File.Exists(crt_path))
            {
                MessageBox.Show("CRT_1.1.1.exe dose not exist. Please repair it", "Cascade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            var dt = this.bindingSource1.DataSource as DataTable;

            _Cascade.OutflowID = _Cascade.GetOutlets(dt);
            _Cascade.Property.HRUFLG = 0;
            _Cascade.Save(_ProjectService.Project.ProcessingDirectory);
            if (!_Cascade.Run(crt_path, _ProjectService.Project.ProcessingDirectory))
            {
                MessageBox.Show("Failed to calculate cascade.", "Cascade", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _Cascade.UpdateParameter(_ProjectService.Project.ProcessingDirectory);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_ProjectService.Project == null)
                return;
            _Cascade.UpdateParameter();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog sd = new OpenFileDialog();
            sd.Filter = "csv file|*.csv|txt file|*.txt";
            if (sd.ShowDialog(this) == DialogResult.OK)
            {
                TxtFileStream txt = new TxtFileStream(sd.FileName);
                var mat = txt.LoadAsIntMatrix();
                var dt = bindingSource1.DataSource as DataTable;
                dt.Rows.Clear();

                for (int i = 0; i < mat.GetLength(0); i++)
                {
                    var dr = dt.NewRow();
                    for (int j = 0; j < 3; j++)
                    {
                        dr[j] = mat[i][j];
                    }
                    dt.Rows.Add(dr);
                }
                this.bindingSource1.DataSource = dt;
                this.dataGridView1.DataSource = bindingSource1;
            }
       
        }
    }
}
