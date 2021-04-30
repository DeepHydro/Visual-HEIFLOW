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
           // _mf.SpeciesManager.LoadCollectionFromDB(dbfile);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
          var btnpck =  _mf.GetPackage(BTNPackage.PackageName);
        //    btnpck.NCOMP=
        }


    }
}
