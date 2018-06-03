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

using Heiflow.Applications;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using Heiflow.Presentation.Services;
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

namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    [Export(typeof(IPackageOptionalView))]
    public partial class LakeExplorerView : Form, IPackageOptionalView
    {
        private IProjectService _ProjectService;
        public LakeExplorerView()
        {
            InitializeComponent();
            this.FormClosing += LakeExplorerView_FormClosing;
            this.Load += LakeExplorerView_Load;
        }

        public bool CloseRequired
        {
            get;
            set;
        }

        public string PackageName
        {
            get
            { 
                return LakePackage.PackageName; 
            }
        }

        public IPackage Package
        {
            get;
            set;
        }
        public object DataContext
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return "LakeExplorerView"; }
        }
        public void ShowView(IWin32Window pararent)
        {
            this.Show(pararent);
        }

        private void LakeExplorerView_Load(object sender, EventArgs e)
        {
            this.timeSeriesExplorer1.Package = Package;
            _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            timeSeriesExplorer1.ODM = _ProjectService.Project.ODMSource;
        }

        private void LakeExplorerView_FormClosing(object sender, FormClosingEventArgs e)
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
            timeSeriesExplorer1.ClearContent();
        }
        public void InitService()
        {

        }
    }
}
