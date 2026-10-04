using DotSpatial.Symbology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Visualization.Studio.Controls
{
    public partial class FormSymbol : Form
    {
        public FormSymbol(IRasterLayer layer)
        {
            _layer = layer;
         
            InitializeComponent();
            rasterCategoryControl1.Initialize(layer);
        }

        private IRasterLayer _layer;
        private void button1_Click(object sender, EventArgs e)
        {
            rasterCategoryControl1.ApplyChanges();
            this.Close();
        }
    }
}
