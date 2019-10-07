using Heiflow.Applications;
using Heiflow.Core.Data;
using Heiflow.Models.Integration;
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

namespace Heiflow.Controls.WinForm.Project
{
    public partial class GlobalOptionForm : Form
    {
        public GlobalOptionForm()
        {
            InitializeComponent();
        }
        private HeiflowModel _Model;
        private void GlobalOptionForm_Load(object sender, EventArgs e)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            _Model = prj.Project.Model as HeiflowModel;
            radioRunoffLinear.Checked = _Model.MasterPackage.SurfaceRunoff == SurfaceRunoffModule.srunoff_carea_casc;
            radioRunoffNonLinear.Checked = _Model.MasterPackage.SurfaceRunoff == SurfaceRunoffModule.srunoff_smidx_casc;

            radioSRTemp.Checked = _Model.MasterPackage.SolarRadiation == SolarRadiationModule.ddsolrad_hru_prms;
            radioSRCloud.Checked = _Model.MasterPackage.SolarRadiation == SolarRadiationModule.ccsolrad_hru_prms;

            radioPETClimate.Checked = _Model.MasterPackage.PotentialET == PETModule.climate_hru;
            radioPETPM.Checked = _Model.MasterPackage.PotentialET == PETModule.potet_pm;

            var outvar_file = Path.Combine(VHFAppManager.Instance.ConfigManager.ConfigPath, "outvar_" + prj.Project.SelectedVersion + ".csv");
            if(File.Exists(outvar_file))
            {
                listVarDescriptions.Items.Clear();
                listVars.Items.Clear();
                StreamReader sr = new StreamReader(outvar_file);
                var line = sr.ReadLine();
                while(!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<string>(line.Trim());
                    if(_Model.MasterPackage.AniOutVarNames.Contains(buf[0]))
                        listVars.Items.Add(buf[0],true);
                    else
                        listVars.Items.Add(buf[0], false);
                    listVarDescriptions.Items.Add(buf[1]);
                }
                sr.Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listVars.CheckedItems.Count == 0)
            {
                MessageBox.Show("At least one output variable must be selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkMappedClimate.Checked && TypeConverterEx.IsNull(tbMapFilename.Text))
            {
                MessageBox.Show("The spatial mapping file can not be null since you have checked the Use mapped climate driving force", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listVars.CheckedItems.Count > 10)
            {
                if (MessageBox.Show("More than 10 variables are selected. The output file may be very large. Do you really want to output the selected variables?", "Output variables selection",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            int mxsziter = 15;
            int.TryParse(cmbmxsziter.Text, out mxsziter);
            _Model.MasterPackage.MaxSoilZoneIter = mxsziter;
            if (_Model.MasterPackage.AnimationOutOC)
            {
                _Model.MasterPackage.NumAniOutVar = listVars.CheckedItems.Count;
                _Model.MasterPackage.AniOutVarNames = new string[_Model.MasterPackage.NumAniOutVar];
                for (int i = 0; i < _Model.MasterPackage.NumAniOutVar; i++)
                {
                    _Model.MasterPackage.AniOutVarNames[i] = listVars.CheckedItems[i].ToString();
                }
            }
            if (radioRunoffLinear.Checked)
                _Model.MasterPackage.SurfaceRunoff = SurfaceRunoffModule.srunoff_carea_casc;
            else if (radioRunoffNonLinear.Checked)
                _Model.MasterPackage.SurfaceRunoff = SurfaceRunoffModule.srunoff_smidx_casc;

            if (radioSRTemp.Checked)
                _Model.MasterPackage.SolarRadiation = SolarRadiationModule.ddsolrad_hru_prms;
            else if (radioSRCloud.Checked)
                _Model.MasterPackage.SolarRadiation = SolarRadiationModule.ccsolrad_hru_prms;

            if (radioPETClimate.Checked)
                _Model.MasterPackage.PotentialET = PETModule.climate_hru;
            else if (radioPETPM.Checked)
                _Model.MasterPackage.PotentialET = PETModule.potet_pm;

            _Model.MasterPackage.IsDirty = true;
            _Model.MasterPackage.Save(null);

        }
        private void checkMappedClimate_CheckedChanged(object sender, EventArgs e)
        {
            tbMapFilename.Enabled = checkMappedClimate.Checked;
            btnMapFilename.Enabled = checkMappedClimate.Checked;
        }

        private void checkSM_CheckedChanged(object sender, EventArgs e)
        {
            tbSM.Enabled = checkSM.Checked;
            btnSM.Enabled = checkSM.Checked;
        }

        private void btnMapFilename_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Mapping Files (.map)|*.map|All Files (*.*)|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tbMapFilename.Text = dlg.FileName;
        }


        private void btnSM_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Animation Output Files (.dcx)|*.dcx|All Files (*.*)|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                tbSM.Text = dlg.FileName; 
        }
    }
}
