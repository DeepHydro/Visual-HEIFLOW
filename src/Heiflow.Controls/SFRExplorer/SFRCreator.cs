//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.SFRExplorer
{
    [Export(typeof(IPackageOptionalView))]
    public partial class SFRCreator : Form, IPackageOptionalView
    {
        private SFRPackage _SFRPackage;

        public SFRCreator()
        {
            InitializeComponent();
            this.FormClosing += SFRCreator_FormClosing;

        }
        public string ChildName
        {
            get { return "SFRCreator"; }
        }
        public string PackageName
        {
            get
            {
                return SFRPackage.PackageName;
            }
        }

        public Models.Generic.IPackage Package
        {
            get
            {
                return _SFRPackage;
            }
            set
            {
                _SFRPackage = value as SFRPackage;
            }
        }
        public object DataContext
        {
            get;
            set;
        }

        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
                this.Show(pararent);
        }


        private void SFRCreator_FormClosing(object sender, FormClosingEventArgs e)
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
        public void ClearContent()
        {

        }
        public void InitService()
        {

        }

        private void SFRCreator_Load(object sender, EventArgs e)
        {
            var riv_ids = from rv in _SFRPackage.RiverNetwork.Rivers select rv.ID;
            cmbStartID.DataSource = riv_ids.ToArray();
        }

        private void cmbStartID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStartID.SelectedItem != null)
            {
                var river = (int)cmbStartID.SelectedItem;
                var profiles = _SFRPackage.RiverNetwork.BuildProfile(river);
                var riv_ids = (from rv in profiles select rv.ID).ToArray();
                cmbEndID.DataSource = riv_ids;
            }
        }

        private void cmbPropertyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEndID.SelectedItem != null)
            {
                tabControl_Chart.SelectedTab = this.tabPageProfile;
                var river_start = (int)cmbStartID.SelectedItem;
                var river_end = (int)cmbEndID.SelectedItem;

                if (cmbPropertyName.SelectedIndex < 0)
                    return;
                var prof = _SFRPackage.RiverNetwork.GetProfileProperty(river_start, river_end, cmbPropertyName.SelectedItem.ToString());
                string series = string.Format("{0} from {1} to {2}", cmbPropertyName.SelectedItem.ToString(), river_start, river_end);
                winChart_proflie.Plot(prof, series);
            }
        }
    }
}
