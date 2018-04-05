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
            olvLayerGroup.SetObjects(_Layers);
            olvLayersUniformProp.SetObjects(_Layers);
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _LayerGroupManager.Initialize((int)numericUpDown1.Value);
            olvLayerGroup.SetObjects(_Layers);
            olvLayersUniformProp.SetObjects(_Layers);
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            var layer = olvLayerGroup.SelectedObject as LayerGroup;
            if (layer != null)
                _LayerGroupManager.Remove(layer);
            olvLayerGroup.SetObjects(_Layers);
            olvLayersUniformProp.SetObjects(_Layers);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var index = olvLayerGroup.SelectedIndex;
            if (index < 0)
                index = olvLayerGroup.Items.Count - 1;
            _LayerGroupManager.Add(index);
            olvLayerGroup.SetObjects(_Layers);
            olvLayersUniformProp.SetObjects(_Layers);
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

        private void olvLayerGroup_ItemsChanged(object sender, BrightIdeasSoftware.ItemsChangedEventArgs e)
        {
             
        }

        private void olvLayerGroup_CellEditFinished(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            _LayerGroupManager.OnItemChanged(e.RowObject as LayerGroup,e.Column.AspectName);
        }


    }
}
