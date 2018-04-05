// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Core.IO;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Modflow
{
    [Export(typeof(IPackageOptionalView))]
    public partial class DISCreator : Form, IPackageOptionalView
    {
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripButton btnCorrectElev;
        private DISPackage _DISPackage;
        private float[][] _Value_Cat;
        public DISCreator()
        {
            InitializeComponent();
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.SaveButtion);
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.ImportButtion);
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.Save2ExcelButtion);
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.CmbPage);
            dataGridEx1.Navigator.Items.RemoveByKey(DataGridEx.CmbPrimaryKey);

            this.btnImport = new ToolStripButton();
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.Image = global::Heiflow.Controls.WinForm.Properties.Resources.TmImportFeatures16;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(24, 25);
            this.btnImport.Text = "Import lookup table from an existing file";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);

            this.btnCorrectElev = new ToolStripButton();
            this.btnCorrectElev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCorrectElev.Image = global::Heiflow.Controls.WinForm.Properties.Resources.Go_24px;
            this.btnCorrectElev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCorrectElev.Name = "btnCorrectElev";
            this.btnCorrectElev.Size = new System.Drawing.Size(24, 25);
            this.btnCorrectElev.Text = "Correct elevation based on the lookup table";
            this.btnCorrectElev.Click += new System.EventHandler(this.btnCorrectElev_Click);

            dataGridEx1.Navigator.Items.Add(this.btnImport);
            dataGridEx1.Navigator.Items.Add(this.btnCorrectElev);

            this.FormClosing += DISCreator_FormClosing;
        }

        public string PackageName
        {
            get
            {
                return DISPackage.PackageName;
            }
        }

        public Models.Generic.IPackage Package
        {
            get
            {
                return _DISPackage;
            }
            set
            {
                _DISPackage = value as DISPackage;
            }
        }

        public string ChildName
        {
            get { return "DISCreator"; }
        }
        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
                this.Show(pararent);
        }
        public bool CloseRequired
        {
            get;
            set;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog sd = new OpenFileDialog();
            sd.Filter = "csv file|*.csv|txt file|*.txt";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                TxtFileStream txt = new TxtFileStream(sd.FileName);
                var dt = txt.LoadAsTable();
                _Value_Cat = txt.LoadAsFloatMatrix();
                dataGridEx1.DataTable = dt;
            }
        }

        private void btnCorrectElev_Click(object sender, EventArgs e)
        {
            if (_Value_Cat != null)
            {
                int ncat = _Value_Cat[0].Length;
                int nratio = ncat - 2;

                if (nratio != _DISPackage.Elevation.Size[0] - 1)
                {
                    return;
                }
                float[,] cat = new float[ncat, 2];
                float[,] ratio = new float[ncat, nratio];

                for (int i = 0; i < ncat; i++)
                {
                    cat[i, 0] = _Value_Cat[i][0];
                    cat[i, 1] = _Value_Cat[i][1];

                    for (int j = 0; j < nratio; j++)
                    {
                        ratio[i, j] = _Value_Cat[i][2 + j];
                    }
                }
                _DISPackage.CorrectElevation(cat, ratio);
            }
        }

        private void DISCreator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CloseRequired)
            {
                this.Hide();
                e.Cancel = true;
                return;
            }
            if (!this.Visible)
                e.Cancel = false;
        }


        public object DataContext
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public void ClearContent()
        {
           
        }
        public void InitService()
        {

        }
    }
}
