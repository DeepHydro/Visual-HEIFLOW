// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Symbology;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Project;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Controls.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.MenuItems
{
    [Export(typeof(IPEContextMenu))]
    public class ModelContextMenu : PEContextMenu, IModelContextMenu
    {
        protected IBasicModel _Model;
        public event EventHandler PackageAdded;

        public ModelContextMenu()
        {
            LegendSymbolMode = SymbolMode.GroupSymbol;
            LegendType = LegendType.Group;
        }

        public IBasicModel Model
        {
            get
            {
                return _Model;
            }
            set
            {
                _Model = value;
                LegendText = _Model.Name;
                Icon = _Model.Icon;
            }
        }

        public override Type ItemType
        {
            get
            {
                return typeof(ModelItem);
            }
        }

        public override void AddMenuItems()
        {
            if (MyAppManager.Instance.AppMode == AppMode.VHF)
            {
                ContextMenuItems.Add(new ExplorerMenuItem("Add Package...", Resources.MapPackageTiledTPKFile16, ProjectItem_NewPackageClicked));
                ContextMenuItems.Add(new ExplorerMenuItem(PEContextMenu.MenuSeparator, null, null));
            }
            ContextMenuItems.Add(new ExplorerMenuItem("Expand All", null, ExpandAll_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem("Collapse All", null, CollapseAll_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem(PEContextMenu.MenuSeparator, null, null));
        }

        protected override void ProjectItem_PropertyClicked(object sender, EventArgs e)
        {
            base.ProjectItem_PropertyClicked(sender, e);
            _ShellService.PropertyView.SelectedObject = _Model;       
        }

        protected void OnNewPackageClicked(object sender)
        {
            if (PackageAdded != null)
            {
                PackageAdded(sender, EnvelopeArgs.Empty);
            }
        }

        protected void ProjectItem_NewPackageClicked(object sender, EventArgs e)
        {
            NewProjectionItemForm form = new NewProjectionItemForm(this.Model);
            form.ShowDialog();
            //NewEcoPackageForm form = new NewEcoPackageForm();
            //form.ShowDialog();
        }

        protected void AddGrid_Clicked(object sender, EventArgs e)
        {
            if (_ProjectService.Project == null)
                return;
            var fs = _ProjectService.Project.Model.Grid.FeatureSet;
            if (fs != null)
            {
                if (_ShellService.MessageService.ShowQuestion(null, "Model grid has been exisited. Do you want to rebuild the grid?").Value)
                {
                    _ProjectService.Project.CreateGridFeature();
                }
                else
                {

                }
            }
            else
            {
                _ProjectService.Project.CreateGridFeature();
            }
        }

        private void CollapseAll_Clicked(object sender, EventArgs e)
        {
            ProjectExplorerControl.SelectedNode.CollapseAll();
        }

        private void ExpandAll_Clicked(object sender, EventArgs e)
        {
            ProjectExplorerControl.SelectedNode.ExpandAll();
        }
    }
}
