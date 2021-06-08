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
            _mf.SpeciesManager.LoadCollectionFromDB(dbfile);
            olvSpeciesList.DataSource = _mf.SpeciesManager.SpeciesCollection;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var btnpck = _mf.GetPackage(BTNPackage.PackageName) as BTNPackage;
          var selected = from sp in _mf.SpeciesManager.SpeciesCollection where sp.Selected select sp;
          if (selected.Count() > 0)
          {
              btnpck.NCOMP = selected.Count();
              var initcomp = from comp in selected select comp.InitialConcentration;
              btnpck.InitComponents(initcomp.ToArray());
              if(_mf.Packages.Keys.Contains(PHCPackage.PackageName ))
              {
                  var phcpck = _mf.GetPackage(PHCPackage.PackageName) as PHCPackage;
                  phcpck.NumAquComponents = btnpck.NCOMP;
                  var compname = from comp in selected select comp.Name;
                  phcpck.AllComponentsNames = compname.ToArray();

              }
              this.Close();
          }
          else
          {
              MessageBox.Show("No species selected. At least one species must be selected.", "Species Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }


    }
}
