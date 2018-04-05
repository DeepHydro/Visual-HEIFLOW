// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
