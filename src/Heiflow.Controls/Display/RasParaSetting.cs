using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Applications.Views;
using System.ComponentModel.Composition;
using Heiflow.Applications;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using Heiflow.Models.Generic;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Core.Data;
using Heiflow.Models.Generic.Parameters;

namespace Heiflow.Controls.WinForm.Display
{
    [Export(typeof(IRasParaSettingView))]
    public partial class RasParaSetting : Form, IRasParaSettingView
    {
        private VHFAppManager _app;
        private IPackage _Package;
        private MappingTable<float> _Mapping;
        private List<IParameter> _Paras;
        private IRaster _SelectedRaster;

        public RasParaSetting(VHFAppManager app,   IPackage pck)
        {
            _app = app;
            _Package = pck;
            _Paras = new List<IParameter>();   
            InitializeComponent();
        }
       

        public object DataContext
        {
            get;
            set;
        }

        private void RasParaSetting_Load(object sender, EventArgs e)
        {
            cmbLayers.DisplayMember = "LegendText";
            cmbLayers.ValueMember = "DataSet";
            var ras_layers = from layer in _app.MapAppManager.Map.Layers where layer is MapRasterLayer select layer as MapRasterLayer;
            cmbLayers.DataSource = ras_layers.ToArray();
            cmbPeriods.SelectedIndex = 0;

            var fea_layers = from layer in _app.MapAppManager.Map.Layers where layer is MapPointLayer select layer as MapPointLayer;
            cmbGrid.DisplayMember = "LegendText";
            cmbGrid.ValueMember = "DataSet";
            cmbGrid.DataSource = fea_layers.ToArray();

        }

        private void cmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
             _SelectedRaster = (cmbLayers.SelectedValue as MapRasterLayer).DataSet;
            bool outov = false;
            var uval = _SelectedRaster.GetUniqueValues( 100, out outov);
            int nhru = (_Package.Owner as PRMS).NHRU;
            _Paras.Clear();
            foreach (var pr in _Package.Parameters.Values)
            {
                if (pr.ValueCount == nhru)
                {
                    _Paras.Add(pr);
                }
            }
            var para_names = from pr in _Paras select pr.Name;
            var uid = from uu in uval select uu.ToString();
            var default_values = from pr in _Paras select  (float)pr.DefaultValue;
            _Mapping = new MappingTable<float>(para_names.ToArray(), uid.ToArray(), default_values.ToArray());

            dataGridView1.DataSource = _Mapping.ToDataTable();
        }

        public void ShowView(IWin32Window pararent)
        {
            this.ShowDialog();
        }

        public void ShowView()
        {
            this.ShowDialog();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            var fea = (cmbGrid.SelectedValue as MapPointLayer).DataSet;
            var coords = from ff in fea.Features select ff.Coordinates.First();
            _Mapping.FromDataTable(dataGridView1.DataSource as DataTable);
            foreach(var para in _Paras)
            {
                var array_para = para as ArrayParam<float>;
                int i=0;
                foreach(var cc in coords)
                {
                    var cell = _SelectedRaster.ProjToCell(cc);
                    if (cell != null && cell.Row >= 0 && cell.Column >= 0)
                    {
                        var uid= _SelectedRaster.Value[cell.Row, cell.Column].ToString();
                         array_para.Values[i] = _Mapping.Map(para.Name, uid);
                    }
                    i++;
                }
            }
        }
    }
}
