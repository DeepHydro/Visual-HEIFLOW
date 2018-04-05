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

using DevExpress.XtraEditors.Repository;
using DotSpatial;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Applications.Controllers;
using Heiflow.Controls.Options;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Plugins.Default.Properties;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Controls.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Plugins.ProjectExplorer
{
    public class ProjectExplorerPlugin : Extension
    {
        private Control _ProjectExplorer;
        private DropDownActionItem _layerDropDown;
       // private DropDownActionItem _MainTimeStepDropDown;

        public ProjectExplorerPlugin()
        {
            DeactivationAllowed = false;


        }

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            _ProjectExplorer = ProjectManager.ShellService.ProjectExplorer as Control;
            _ProjectExplorer.Dock = DockStyle.Fill;
            App.DockManager.Add(new DockablePanel("kProjectExplorer", "Project",
                _ProjectExplorer, DockStyle.Left) { SmallImage = Heiflow.Plugins.Default.Properties.Resources.CatalogShowTree16 });

            var showPE = new SimpleActionItem("kView", "Project Explorer",
                delegate(object sender, EventArgs e)
                { App.DockManager.ShowPanel("kProjectExplorer"); }
           )
            {
                Key = "kShowProjectExplorer",
                ToolTipText = "Show Project Explorer",
                GroupCaption = "Model",
                LargeImage = Resources.CatalogShowTree32
            };
            App.HeaderControl.Add(showPE);

            base.Activate();
            ProjectManager.ProjectService.Serializer.ProjectOpened += SerializationManager_ProjectChanged;
            AddMenuItems();

        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowProjectExplorer");
            App.DockManager.Remove("kProjectExplorer");
            base.Deactivate();
        }

        private void AddMenuItems()
        {
            IHeaderControl header = App.HeaderControl;

            header.Add(new SimpleActionItem("kOptions", "Options", Option_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 100,
                SmallImage = Resources.MetadataCreateUpdate32,
                LargeImage = Resources.MetadataCreateUpdate32,
                ToolTipText = "Set options"
            });

            _layerDropDown = new DropDownActionItem()
            {
                Key = "kLayerDropDown",
                RootKey = "kModel",
                Width = 145,
                AllowEditingText = false,
                ToolTipText = "Select current subsurface layer",
                GroupCaption = "Display",
                DisplayText = "Select a layer",
                Caption = "Current Layer",
            };
            _layerDropDown.SelectedValueChanged += _layerDropDown_SelectedValueChanged;
            App.HeaderControl.Add(_layerDropDown);
        }

        private void SerializationManager_ProjectChanged(object sender, bool e)
        {
            var prj = (sender as ProjectSerialization).CurrentProject;
            var grid = prj.Model.Grid;
            RepositoryItemComboBox combo = (App.HeaderControl as RibbonHeader).ComboBoxes["kLayerDropDown"];
            combo.Items.Clear();
            for (int i = 1; i <= grid.ActualLayerCount; i++)
            {
                combo.Items.Add(i.ToString());
            }
            //ProjectManager.ShellService.AnimationPlayer.TimeServices = ProjectManager.ProjectService.Project.Model.TimeServiceList.Values.ToList();
        }

        private void ProjectManager_AnimationSourceChanged(object sender, MyArray<float> e)
        {
            var ds = e as My3DMat<float>;
            if (ds != null)
            {
                //RepositoryItemComboBox combo = (App.HeaderControl as RibbonHeader).ComboBoxes["kTimeStepDropDown"];
                //combo.Items.Clear();
                //for (int i = 1; i <= ds.Size[1]; i++)
                //{
                //    combo.Items.Add(i.ToString());
                //}
            }
        }

        private void _TimeStepDropDown_SelectedValueChanged(object sender, SelectedValueChangedEventArgs e)
        {
          //  ProjectManager.Project.TimeServices
            //ModelService.CurrentTimeStep = int.Parse(e.SelectedItem.ToString()) - 1;
        }

        private void _layerDropDown_SelectedValueChanged(object sender, SelectedValueChangedEventArgs e)
        {
            foreach(var ts in ProjectManager.Project.Model.TimeServiceList.Values)
            {
                ts.CurrentGridLayer = int.Parse(e.SelectedItem.ToString()) - 1;
            }
        }

        private void Option_Click(object sender, EventArgs e)
        {
            VHFAppManager.Instance.ConfigManager.OptionForm.ShowDialog();
        }
    }
}
