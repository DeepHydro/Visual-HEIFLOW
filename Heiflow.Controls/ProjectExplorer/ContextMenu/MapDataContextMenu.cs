// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications;
using Heiflow.Controls.WinForm;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Generic.Attributes;
using System.ComponentModel.Composition;
using Heiflow.Presentation.Controls.Project;

namespace Heiflow.Controls.WinForm.MenuItems
{
     [Export(typeof(IPEContextMenu))]
    public class MapDataContextMenu : PEContextMenu
    {    
        public MapDataContextMenu()
        {
    
        }

        public override Type ItemType
        {
            get
            {
                return typeof(MapDataItem);
            }
        }

        public override void AddMenuItems()
        {
            ContextMenuItems.Add(new ExplorerMenuItem("New Coverage...", Resources.MapPackageTiledTPKFile16, NewCoverage_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem("Expand All", null, ExpandAll_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem("Collapse All", null, CollapseAll_Clicked));    
        }

        private void CollapseAll_Clicked(object sender, EventArgs e)
        {
            ProjectExplorerControl.SelectedNode.CollapseAll();
        }

        private void ExpandAll_Clicked(object sender, EventArgs e)
        {
            ProjectExplorerControl.SelectedNode.ExpandAll();
        }

        private void NewCoverage_Clicked(object sender, EventArgs e)
        {
            var csc = (_AppManager as VHFAppManager).CoverageSetupController;
            csc.ViewModel.Coverage = null;
            csc.ViewModel.ShowView();
        }


    }
}
