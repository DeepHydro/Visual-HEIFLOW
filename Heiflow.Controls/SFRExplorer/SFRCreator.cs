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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
                return _SFRPackage ;
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

        private void btnApplyEleOffset_Click(object sender, EventArgs e)
        {
            float offset = 0;
            float.TryParse(tbEleOffset.Text, out offset);

            _SFRPackage.CorrectElevation(offset);
            lbState.Text = "Streambed elevations are modified successfully";
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
    }
}
