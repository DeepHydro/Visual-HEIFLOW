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

using Heiflow.Applications;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.MenuItems
{
      [Export(typeof(IPEContextMenu))]
    public class MapLayerContextMenu : PEContextMenu, IMapLayerContextMenu
    {
          private PackageCoverage _PackageCoverage;

        public MapLayerContextMenu()
        {
          
        }

        public override Type ItemType
        {
            get
            {
                return typeof(MapLayerItem);
            }
        }

        public PackageCoverage Coverage 
        {
            get
            {
                return _PackageCoverage;
            }
            set
            {
                _PackageCoverage = value;
            }
        }

        public override void AddMenuItems()
        {
            ContextMenuItems.Add(new ExplorerMenuItem("Coverage Setup...", Resources.MapPackageTiledTPKFile16, CoverageSetup_Clicked));
          //  ContextMenuItems.Add(new ExplorerMenuItem("Attribute Table...", Resources.AttributesWindow16, AttributeTable_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem("Remove", null, Remove_Clicked));
        }

        private void AttributeTable_Clicked(object sender, EventArgs e)
        {
            var pc = (_AppManager as VHFAppManager).ParameterTableController;
            pc.ViewModel.Coverage = _PackageCoverage;
            pc.ViewModel.ShowView();
            
        }

        private void CoverageSetup_Clicked(object sender, EventArgs e)
        {
            var csc = (_AppManager as VHFAppManager).CoverageSetupController;
            csc.ViewModel.Coverage = _PackageCoverage;
            csc.ViewModel.ShowView();
        }

        private void Remove_Clicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to remove the coverage?","Coverage", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                _PackageCoverage.Clear();
                if (_PackageCoverage is FeatureCoverage)
                    _ProjectService.Project.FeatureCoverages.Remove(_PackageCoverage as FeatureCoverage);
                else if (_PackageCoverage is RasterCoverage)
                    _ProjectService.Project.RasterLayerCoverages.Remove(_PackageCoverage as RasterCoverage);
            }
        }

    }
}
