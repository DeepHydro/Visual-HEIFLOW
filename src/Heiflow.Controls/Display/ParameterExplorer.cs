using Heiflow.Applications;
using Heiflow.Models.Generic;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Models.UI;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Display
{
    public partial class ParameterExplorer : Form, IParameterExplorerView
    {
        private IProjectService _ProjectService;
        private PRMS _PRMS;
        private IEnumerable<ParaCat> _ModuleCat;
        private IEnumerable<ParaCat> _DimCat;

        public class ParaCat
        {
            public string Key { get; set; }
            public IEnumerable<IParameter> Parameters { get; set; }
        }

        public ParameterExplorer()
        {
            InitializeComponent();
        }
        public object DataContext
        {
            get;
            set;
        }
        public string ChildName
        {
            get
            {
                return "ParameterExplorerView";
            }
        }

        public void BindParameters(IEnumerable<IParameter> paras)
        {
            this.bindingSource1.DataSource = paras;
            this.dataGridView1.DataSource = this.bindingSource1;
        }

        private void ParameterExplorer_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnCount = 10;
            dataGridView1.Columns[0].Name = "Name";
            dataGridView1.Columns[0].DataPropertyName = "Name";

            dataGridView1.Columns[1].Name = "Value Type";
            dataGridView1.Columns[1].DataPropertyName = "ValueType";

            dataGridView1.Columns[2].Name = "Module Name";
            dataGridView1.Columns[2].DataPropertyName = "ModuleName";

            dataGridView1.Columns[3].Name = "Default Value";
            dataGridView1.Columns[3].DataPropertyName = "DefaultValue";

            dataGridView1.Columns[4].Name = "Maximum";
            dataGridView1.Columns[4].DataPropertyName = "Maximum";

            dataGridView1.Columns[5].Name = "Minimum";
            dataGridView1.Columns[5].DataPropertyName = "Minimum";


            dataGridView1.Columns[6].Name = "Value Type";
            dataGridView1.Columns[6].DataPropertyName = "ValueType";

            dataGridView1.Columns[7].Name = "Dimension";
            dataGridView1.Columns[7].DataPropertyName = "Dimension";

            dataGridView1.Columns[8].Name = "Dimension Names";
            dataGridView1.Columns[8].DataPropertyName = "DimensionCat";

            dataGridView1.Columns[9].Name = "Description";
            dataGridView1.Columns[9].DataPropertyName = "Description";

            _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            if (_ProjectService.Project.Model is Heiflow.Models.Integration.HeiflowModel)
            {
                _PRMS = (_ProjectService.Project.Model as Heiflow.Models.Integration.HeiflowModel).PRMSModel;
                _ModuleCat = from par in _PRMS.MMSPackage.Parameters.Values
                             group par by par.ModuleName into pp
                             select new ParaCat
                             {
                                 Key = pp.Key.ToString(),
                                 Parameters = pp.ToArray()
                             };
                _DimCat = from par in _PRMS.MMSPackage.Parameters.Values
                          group par by par.DimensionCat into pp
                          select new ParaCat
                          {
                              Key = pp.Key,
                              Parameters = pp.ToArray()
                          };
            }
            cmbFilterCat.SelectedIndex = 0;
        }

        private void ParameterExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        public void ShowView(IWin32Window pararent)
        {
            this.Show(pararent);
        }

        public void ClearContent()
        {
            this.bindingSource1.DataSource = null;
            this.dataGridView1.DataSource = bindingSource1;
        }

        public void InitService()
        {

        }

        private void cmbFilterCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFilterCat.SelectedIndex == 0)
            {
                var modnames = from mm in _ModuleCat select mm.Key;
                listItems.DataSource = modnames.ToList();
                listItems.SelectedIndex = 0;
            }
            else if (cmbFilterCat.SelectedIndex == 1)
            {
                var dimnames = from mm in _DimCat select mm.Key;
                listItems.DataSource = dimnames.ToList();
                listItems.SelectedIndex = 0;
            }
            else if (cmbFilterCat.SelectedIndex == 2)
            {
                listItems.DataSource = null;
                var paras = _PRMS.MMSPackage.Parameters.Values.ToList();
                BindParameters(paras);
            }
        }

        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFilterCat.SelectedIndex == 0)
            {
                var paras = from pp in _ModuleCat where pp.Key == listItems.SelectedItem.ToString() select pp.Parameters;
                BindParameters(paras.First());
            }
            else if (cmbFilterCat.SelectedIndex == 1)
            {
                var paras = from pp in _DimCat where pp.Key == listItems.SelectedItem.ToString() select pp.Parameters;
                BindParameters(paras.First());
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var configfile = Path.Combine(VHFAppManager.Instance.ConfigManager.ConfigPath, "mms_config_" +prj.Project.SelectedVersion + ".xml");
            var modelpara = this.bindingSource1.DataSource as IEnumerable<IParameter>;
            if (File.Exists(configfile) && modelpara != null)
            {
                var mms = new MMSPackage("PRMS");
                mms.Deserialize(configfile);
                
                foreach (var para in  modelpara)
                {
                    var buf = from pp in mms.Parameters.Values where pp.Name == para.Name select pp;
                    if(buf.Any())
                    {
                        var mmspara = buf.First();
                        mmspara.DefaultValue = para.DefaultValue;
                        mmspara.Maximum = para.Maximum;
                        mmspara.Minimum = para.Minimum;
                        mmspara.Description = para.Description;
                        mmspara.ModuleName = para.ModuleName;
                    }
                }
                mms.Serialize(configfile);
            }
        }
    }
}
