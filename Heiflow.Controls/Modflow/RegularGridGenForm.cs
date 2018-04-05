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
using GeoAPI.Geometries;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Grid;
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Modflow
{
    public partial class RegularGridGenForm : Form
    {
        private IProjectController _Controller;
        private RegularGridGenerator _GridGenerator;

        public RegularGridGenForm(IProjectController contrl)
        {
            InitializeComponent();
            _Controller = contrl;
            _GridGenerator = new RegularGridGenerator();
        }

        private void RegularGridGenForm_Load(object sender, EventArgs e)
        {
            var map_layers = from layer in _Controller.MapAppManager.Map.Layers select new MapLayerDescriptor { LegendText = layer.LegendText, DataSet = layer.DataSet };
            if(map_layers != null)
                cmbLayers.DataSource = map_layers.ToArray();

            var raster_layers = from layer in _Controller.MapAppManager.Map.Layers select new MapLayerDescriptor { LegendText = layer.LegendText, DataSet = layer.DataSet };
            if (raster_layers != null)
                cmbRasterLayer.DataSource = raster_layers.ToArray();
            cmbAvMethod.SelectedIndex = 1;
        }

        private void cmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fea = (cmbLayers.SelectedItem as MapLayerDescriptor).DataSet as IFeatureSet;
            if (fea != null && fea.FeatureType == FeatureType.Polygon)
            {
                _GridGenerator.Domain = fea;
                tbMaxX.Text = fea.Extent.MaxX.ToString();
                tbMinX.Text = fea.Extent.MinX.ToString();
                tbMaxY.Text = fea.Extent.MaxY.ToString();
                tbMinY.Text = fea.Extent.MinY.ToString();
                tbOriginX.Text = tbMinX.Text;
                tbOriginY.Text = tbMaxY.Text;
                tbCellSize_TextChanged(tbXSize, null);
                tbCellSize_TextChanged(tbYSize, null);
            }
        }

        private void cmbRasterLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var raster = (cmbRasterLayer.SelectedItem as MapLayerDescriptor).DataSet as IRaster;
            _GridGenerator.DEM = raster;
        }

        private void rbtnByCellSize_CheckedChanged(object sender, EventArgs e)
        {
            if(rbtnByCellSize.Checked)
            {
                tbXNum.Enabled = false;
                tbYNum.Enabled = false;
                tbXSize.Enabled = true;
                tbYSize.Enabled = true;
            }
        }

        private void rbtnByCellNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnByCellNumber.Checked)
            {
                tbXNum.Enabled = true;
                tbYNum.Enabled = true;
                tbXSize.Enabled = false;
                tbYSize.Enabled = false;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var mf = (_Controller.Project.Model as Heiflow.Models.Integration.HeiflowModel).ModflowModel;
            double originx=0;
            double originy=0;
            Coordinate origin = null;
            try
            {
                originx = double.Parse(tbOriginX.Text);
                originy = double.Parse(tbOriginY.Text);
                origin = new Coordinate(originx, originy);
            }
            catch
            {
                MessageBox.Show("Invalid number");
                return;
            }

            if (_GridGenerator.Domain == null)
            {
                MessageBox.Show("You should select the domain layer");
                return;
            }

            if (_GridGenerator.DEM == null)
            {
                MessageBox.Show("You should select the DEM layer");
                return;
            }
            if (cmbAvMethod.SelectedIndex == 0)
                _GridGenerator.AveragingMethod = Core.MyMath.AveragingMethod.Mean;
            else
                _GridGenerator.AveragingMethod = Core.MyMath.AveragingMethod.Median;
            if (!mf.LayerGroupManager.Initialized)
                mf.LayerGroupManager.Initialize((int)numericUpDown1.Value);

            _GridGenerator.LayerCount = (int)this.numericUpDown1.Value;
            _GridGenerator.Source = _Controller.Project.Model.Grid as RegularGrid;
            _GridGenerator.Origin = origin;
            _GridGenerator.LayerGroups = mf.LayerGroupManager.LayerGroups;
          
            Cursor.Current = Cursors.WaitCursor;
            _GridGenerator.Generate();
            _Controller.Project.CreateGridFeature();
            _GridGenerator.Source.RaiseUpdate();
            _Controller.ShellService.ProjectExplorer.AddProject(_Controller.Project);
            _Controller.ProjectService.RaiseProjectOpenedOrCreated(_Controller.ShellService.MapAppManager.Map, _Controller.Project);
            Cursor.Current = Cursors.Default;
            this.Close();
        }

        private void tbCellSize_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            TextBox target = null;
            float tb_value = 50;
            try
            {
                tb_value = float.Parse(tb.Text);
            }
            catch
            {
                MessageBox.Show("Invalid number");
                return;
            }
            tb.TextChanged -= tbCellSize_TextChanged;

            if(sender.Equals(tbXNum))
            {
                _GridGenerator.ColumnCount = (int) Math.Floor(tb_value);
                _GridGenerator.XSize = (float)(_GridGenerator.Domain.Extent.Width / _GridGenerator.ColumnCount);
                target = tbXSize;
                target.TextChanged -= tbCellSize_TextChanged;
                tbXSize.Text = _GridGenerator.XSize.ToString();
            }
            else if (sender.Equals(tbYNum))
            {
                _GridGenerator.RowCount = (int)Math.Floor(tb_value);
                _GridGenerator.YSize = (float)(_GridGenerator.Domain.Extent.Height / _GridGenerator.RowCount);
                target = tbYSize;
                target.TextChanged -= tbCellSize_TextChanged;
                tbYSize.Text = _GridGenerator.YSize.ToString();
            }
            else if (sender.Equals(tbXSize))
            {
                _GridGenerator.XSize = tb_value;
                _GridGenerator.ColumnCount = (int) Math.Floor(_GridGenerator.Domain.Extent.Width / _GridGenerator.XSize);
                target = tbXNum;
                target.TextChanged -= tbCellSize_TextChanged;
                tbXNum.Text = _GridGenerator.ColumnCount.ToString();
            }
            else if (sender.Equals(tbYSize))
            {
                _GridGenerator.YSize = tb_value;
                _GridGenerator.RowCount = (int)Math.Floor(_GridGenerator.Domain.Extent.Height / _GridGenerator.YSize);
                target = tbYNum;
                target.TextChanged -= tbCellSize_TextChanged;
                tbYNum.Text = _GridGenerator.RowCount.ToString();
            }
            tb.TextChanged += tbCellSize_TextChanged;
            target.TextChanged += tbCellSize_TextChanged;
        }
        private void btnLayerGroup_Click(object sender, EventArgs e)
        {
            var mm = _Controller.Project.Model as Heiflow.Models.Integration.HeiflowModel;
            var mf = mm.ModflowModel;
            LayerGroupForm form = new LayerGroupForm(mf.LayerGroupManager);
            if(form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                numericUpDown1.Value = mf.LayerGroupManager.LayerGroups.Count;
            }
        }




    }
}
