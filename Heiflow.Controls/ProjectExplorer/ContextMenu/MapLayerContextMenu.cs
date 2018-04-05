// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
