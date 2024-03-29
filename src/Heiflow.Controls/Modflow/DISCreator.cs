﻿//
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
