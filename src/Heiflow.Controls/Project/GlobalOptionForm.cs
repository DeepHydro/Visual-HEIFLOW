using Heiflow.Applications;

using Heiflow.Core.Data;
using Heiflow.Models.Generic;
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
            propertyGrid1.SelectedObject = _Model.ExtensionManPackage;
            radioRunoffLinear.Checked = _Model.MasterPackage.SurfaceRunoff == SurfaceRunoffModule.srunoff_carea_casc;
            radioRunoffNonLinear.Checked = _Model.MasterPackage.SurfaceRunoff == SurfaceRunoffModule.srunoff_smidx_casc;
            radioSRTemp.Checked = _Model.MasterPackage.SolarRadiation == SolarRadiationModule.ddsolrad_hru_prms;
            radioSRCloud.Checked = _Model.MasterPackage.SolarRadiation == SolarRadiationModule.ccsolrad_hru_prms;
            radioPETClimate.Checked = _Model.MasterPackage.PotentialET == PETModule.climate_hru;
            radioPETPM.Checked = _Model.MasterPackage.PotentialET == PETModule.potet_pm;
            cmbmxsziter.Text = _Model.MasterPackage.MaxSoilZoneIter.ToString();
            cmbClimateFormat.SelectedIndex = _Model.MasterPackage.ClimateInputFormat == Models.Generic.FileFormat.Binary ? 0 : 1;
            checkMappedClimate.Checked = _Model.MasterPackage.UseGridClimate;

            tbMapFilename.Text = _Model.MasterPackage.GridClimateFile;
            checkSM.Checked = _Model.MasterPackage.SaveSoilWaterFile;
            checkPringDebug.Checked = _Model.MasterPackage.PrintDebug;

            var outvar_file = Path.Combine(VHFAppManager.Instance.ConfigManager.ConfigPath, "outvar_" + prj.Project.SelectedVersion + ".csv");

            if (File.Exists(outvar_file))
            {
                listVars.Items.Clear();
                StreamReader sr = new StreamReader(outvar_file);
                var line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<string>(line.Trim());
                    if (_Model.MasterPackage.AniOutVarNames.Contains(buf[0]))
                        listVars.Items.Add(buf[0], true);
                    else
                        listVars.Items.Add(buf[0], false);
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
            _Model.MasterPackage.UseGridClimate = checkMappedClimate.Checked;

            int mxsziter = 15;
            int.TryParse(cmbmxsziter.Text, out mxsziter);
            _Model.MasterPackage.MaxSoilZoneIter = mxsziter;

            var anivars = new string[listVars.CheckedItems.Count];
            for (int i = 0; i < listVars.CheckedItems.Count; i++)
            {
                anivars[i]=listVars.CheckedItems[i].ToString();
            }
            _Model.MasterPackage.AniOutVarNames = anivars;

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

            if (cmbClimateFormat.SelectedIndex == 0)
            {
                _Model.MasterPackage.ClimateInputFormat = Models.Generic.FileFormat.Binary;
            }
            else
            {
                _Model.MasterPackage.ClimateInputFormat = Models.Generic.FileFormat.Text;
            }
            _Model.MasterPackage.CheckClimateFilename();

            _Model.MasterPackage.SaveSoilWaterFile = checkSM.Checked;
            _Model.MasterPackage.PrintDebug = checkPringDebug.Checked;
            _Model.MasterPackage.IsDirty = true;
            _Model.MasterPackage.Save(null);
            _Model.ExtensionManPackage.IsDirty = true;
            _Model.ExtensionManPackage.Save(null);
        }

        private void checkMappedClimate_CheckedChanged(object sender, EventArgs e)
        {
            tbMapFilename.Enabled = checkMappedClimate.Checked;
            btnCreateMapFile.Enabled = checkMappedClimate.Checked;

        }

        private void btnCreateMapFilename_Click(object sender, EventArgs e)
        {
            _Model.MasterPackage.WriteDefaultClimateMapFile(ModelService.NHRU);

        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedTab = tabOutVars;
        }

        private void listVars_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

    }

}