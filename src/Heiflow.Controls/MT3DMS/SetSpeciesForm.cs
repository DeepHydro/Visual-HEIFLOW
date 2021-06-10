using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Models.Subsurface;
using System.IO;
using Heiflow.Models.Subsurface.MT3DMS;
using Heiflow.Models.Subsurface.VFT3D;

namespace Heiflow.Controls.WinForm.MT3DMS
{
    public partial class SetSpeciesForm : Form
    {
        public SetSpeciesForm(Heiflow.Models.Subsurface.Modflow mf)
        {
            InitializeComponent();
            _mf = mf;
        }

        private Heiflow.Models.Subsurface.Modflow _mf;
        private void SetSpeciesForm_Load(object sender, EventArgs e)
        {
            string dbfile = Path.Combine(Application.StartupPath, "data\\pht3d_datab.dat");

            if (_mf.Packages.Keys.Contains(PHCPackage.PackageName))
            {
                var phcpck = _mf.GetPackage(PHCPackage.PackageName) as PHCPackage;
                if (phcpck.NumAquComponents == 0)
                {
                    _mf.MobileSpeciesManager.LoadCollectionFromDB(dbfile);
                    _mf.MobileSpeciesManager.SetDefaultMobileSpecies();
                }
                olvMobileSpeciesList.DataSource = _mf.MobileSpeciesManager.SpeciesCollection;

                if (phcpck.NumExchSpecies == 0)
                {
                    _mf.ExchangeSpeciesManager.LoadCollectionFromDB(dbfile);
                    _mf.ExchangeSpeciesManager.SetDefaultExchangeSpecies();
                }
                olvExchangeSpeciesList.DataSource = _mf.ExchangeSpeciesManager.SpeciesCollection;
                olvMineralSpeciesList.DataSource = _mf.MineralSpeciesManager.SpeciesCollection;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var btnpck = _mf.GetPackage(BTNPackage.PackageName) as BTNPackage;
            _mf.MobileSpeciesManager.CheckMandaryMobileSpecies();

            var selected_mobile = from sp in _mf.MobileSpeciesManager.SpeciesCollection where sp.Selected select sp;
            var selected_ex = from sp in _mf.ExchangeSpeciesManager.SpeciesCollection where sp.Selected select sp;
            if (selected_mobile.Count() < 2)
            {
                MessageBox.Show("No species selected. At least two species must be selected.", "Species Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nmobie = selected_mobile.Count();
            var nex = selected_ex.Count();
            btnpck.NCOMP = nmobie + nex;
            btnpck.MCOMP = nmobie - 2;

            var initcomp_mob = (from comp in selected_mobile select comp.InitialConcentration).ToList();
            var initcomp_ex = (from comp in selected_ex select comp.InitialConcentration).ToList();
            initcomp_mob.AddRange(initcomp_ex);
            var _default_mobile_species_con = new float[] { 0.009f, 0.03f, 0.0017f, 0.02f, 0.0067f, 0.0003f, 0.0007f, 7.9f, 8, 0.0001f, 0.0001f, 0.0001f };

            //btnpck.InitComponents(initcomp_mob.ToArray());
            btnpck.InitComponents(_default_mobile_species_con);

            this.Close();

        }

    }
}