// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Toolbox
{

    public partial class LayerSelectionDialog : Form
    {
 

        public LayerSelectionDialog()
        {
            InitializeComponent();

            cmbLayers.DisplayMember = "LegendText";
            cmbLayers.ValueMember = "DataSet";
        }

        public IDataSet SelectedLayer
        {
            get;
            protected set;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbLayers.SelectedItem == null)
            {
                MessageBox.Show("A map layer must be selected", "Layer Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                var fea = cmbLayers.SelectedItem as MapLayerDescriptor;
                SelectedLayer = fea.DataSet;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void LayerSelectionDialog_Load(object sender, EventArgs e)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var map_layers =( from layer in shell.MapAppManager.Map.Layers select new MapLayerDescriptor { LegendText = layer.LegendText, DataSet = layer.DataSet }).ToArray();
            cmbLayers.DataSource = map_layers;
        }
    }
}
