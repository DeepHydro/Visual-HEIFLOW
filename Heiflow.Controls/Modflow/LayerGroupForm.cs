// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
