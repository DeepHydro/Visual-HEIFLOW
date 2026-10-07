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

using Heiflow.Models.Subsurface;
using Heiflow.Models.Subsurface.Packages;
using Heiflow.Presentation;
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
using Heiflow.Controls.WinForm.Controls;

namespace Heiflow.Controls.WinForm.Modflow
{
    public partial class LayerGroupForm : Form
    {
        private LayerGroupManager _LayerGroupManager;
        private ObservableCollection<LayerGroup> _Layers;

        public LayerGroupForm(LayerGroupManager man)
        {
            InitializeComponent();
            _LayerGroupManager = man;
            _Layers = _LayerGroupManager.LayerGroups;
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
        }

        private void LayerGroupForm_Load(object sender, EventArgs e)
        {
            if (_Layers.Count == 0)
            {
                _LayerGroupManager.Initialize(3);
            }
            else
            {
                numericUpDown1.ValueChanged -= numericUpDown1_ValueChanged;
                numericUpDown1.Value = _Layers.Count;
                numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            }
            BindLayers();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _LayerGroupManager.Initialize((int)numericUpDown1.Value);
            BindLayers();
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            var layer = NativeList.SelectedRow<LayerGroup>(olvLayerGroup);
            if (layer != null)
                _LayerGroupManager.Remove(layer);
            BindLayers();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var index = olvLayerGroup.SelectedRows.Count > 0 ? olvLayerGroup.SelectedRows[0].Index : -1;
            if (index < 0)
                index = olvLayerGroup.Rows.Count - 1;
            _LayerGroupManager.Add(index);
            BindLayers();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void olvLayerGroup_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            _LayerGroupManager.OnItemChanged(
                olvLayerGroup.Rows[e.RowIndex].DataBoundItem as LayerGroup,
                olvLayerGroup.Columns[e.ColumnIndex].DataPropertyName);
        }

        private void olvLayersUniformProp_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            _LayerGroupManager.OnItemChanged(
                olvLayersUniformProp.Rows[e.RowIndex].DataBoundItem as LayerGroup,
                olvLayersUniformProp.Columns[e.ColumnIndex].DataPropertyName);
        }

        /// <summary>Rebinds both grids. An ObservableCollection does not raise the list change
        /// notifications a WinForms binding listens to, so rebinding is what refreshes them.</summary>
        private void BindLayers()
        {
            olvLayerGroup.DataSource = null;
            olvLayersUniformProp.DataSource = null;
            olvLayerGroup.DataSource = _Layers;
            olvLayersUniformProp.DataSource = _Layers;
        }

        private void btnSetToUniform_Click(object sender, EventArgs e)
        {
            double LayerHeight = 20;
            double HK = 10;
            double VKA = 100;
            double SS = 0.0001;
            double SY = 0.1;
            double.TryParse(tbLayerHeight.Text, out LayerHeight);
            double.TryParse(tbHK.Text, out HK);
            double.TryParse(tbVKA.Text, out VKA);
            double.TryParse(tbSS.Text, out SS);
            double.TryParse(tbSY.Text, out SY);
            foreach (var layer in _Layers)
            {
                layer.HK = HK;
                layer.VKA = VKA;
                layer.LayerHeight = LayerHeight;
                layer.SS = SS;
                layer.SY = SY;
            }
        }
    }
}