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

using DotSpatial.Symbology;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Controls.WinForm.TimeSeriesExplorer;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using Heiflow.Presentation;
using Heiflow.Presentation.Services;
using Microsoft.Research.Science.Data.NetCDF4;
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
    public partial class SFRExplorerView : Form, IPackageOptionalView
    {
        private IPackage _Package;

        public SFRExplorerView()
        {
            InitializeComponent();
            this.FormClosing += SFRExplorerView_FormClosing;
            this.Load += SFRExplorerView_Load;
        }

        public object DataContext
        {
            get;
            set;
        }
        public IPackage Package
        {
            get
            {
                return _Package;
            }
            set
            {
                _Package = value;
                sfrExplorer1.SFROutput = _Package as SFROutputPackage;
            }
        }
        public string PackageName
        {
            get { return SFROutputPackage.PackageName; }
        }
        public string ChildName
        {
            get { return "SFRExplorerView"; }
        }
        private void SFRExplorerView_Load(object sender, EventArgs e)
        {
        }

        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
            {
                if (MyAppManager.Instance.AppMode == Presentation.Controls.AppMode.VHF)
                {
                    var control = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectController>();
                    var map_layers = from layer in control.MapAppManager.Map.Layers where layer is IFeatureLayer select new FeatureMapLayer { LegendText = layer.LegendText, DataSet = (layer as IFeatureLayer).DataSet };
                    sfrExplorer1.FeatureLayers = map_layers.ToArray();
                    this.Show(pararent);
                }
            }
        }
        private  void SFRExplorerView_FormClosing(object sender, FormClosingEventArgs e)
        {
           if(e.CloseReason== CloseReason.UserClosing)
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
            this.sfrExplorer1.ClearContent();
        }
        public void InitService()
        {
        
        }
    }  
}
