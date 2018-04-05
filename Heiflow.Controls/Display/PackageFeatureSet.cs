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

using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Display
{

    public partial class PackageFeatureSet : Form
    {
        private IPackage _Package;
        private IMap _Map;
        private     ObservableCollection<FeatureElement>  _FeaElements;
        public PackageFeatureSet(IPackage pck, IMap map, ObservableCollection<FeatureElement> eles)
        {
            InitializeComponent();
            cmbLayers.DisplayMember = "LegendText";
            cmbLayers.ValueMember = "DataSet";
            _Package = pck;
            _Map = map;
            _FeaElements = eles;
        }

        private void PackageFeatureSet_Load(object sender, EventArgs e)
        {
            var map_layers = from layer in _Map.Layers where layer is IFeatureLayer select new FeatureMapLayer { LegendText = layer.LegendText, DataSet = (layer as IFeatureLayer).DataSet };
            cmbLayers.DataSource = map_layers.ToArray();
            lvLayer.Items.Clear();
            lvPackage.Items.Clear();
            if (_Package != null)
            {
                tb_pck.Text = _Package.Name;
                //if (!_Package.RequireFeature)
                //{
                //    btnOk.Enabled = false;
                //    btnNewFeature.Enabled = false;
                //}
                //else
                //{
                    foreach (var fe in _Package.Fields)
                    {
                        ListViewItem lvi = new ListViewItem(fe.FieldName);
                        lvi.SubItems.Add(fe.FieldType.ToString());
                        lvPackage.Items.Add(lvi);
                    }
                //}
            }
        }

        private void btnNewFeature_Click(object sender, EventArgs e)
        {

        }

        private void cmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fs = (cmbLayers.SelectedItem as FeatureMapLayer).DataSet;
            lvLayer.Items.Clear();
            foreach(DataColumn dc  in fs.DataTable.Columns)
            {
                ListViewItem lvi = new ListViewItem(dc.ColumnName);
                lvi.SubItems.Add(dc.DataType.ToString());
                lvLayer.Items.Add(lvi);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(cmbLayers.SelectedItem != null)
            {
                var fs = (cmbLayers.SelectedItem as FeatureMapLayer).DataSet;
                _Package.Feature = fs;
                FeatureElement fe = new FeatureElement()
                {
                    FeatureFilename = fs.Filename,
                    FeatureName = fs.Name,
                    PackageName =_Package.Name
                };

                var ff = from fea in _FeaElements where fea.FeatureFilename == fe.FeatureFilename select fea;
                if (ff.Count() > 0)
                {
                    _FeaElements.Remove(ff.First());
                    _FeaElements.Add(fe);
                }
                else
                {
                    _FeaElements.Add(fe);
                }
            }
            this.Close();
        }
    }
}
