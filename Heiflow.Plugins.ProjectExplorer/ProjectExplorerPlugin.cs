using DotSpatial;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Controls.Options;
using Heiflow.Controls.Project;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Plugins.ProjectExplorer.Properties;
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
         ProjectExplorerControl _ProjectExplorer;
          DropDownActionItem _layerDropDown;

        public ProjectExplorerPlugin()
        {
            DeactivationAllowed = false;
            _ProjectExplorer = new ProjectExplorerControl();
            _ProjectExplorer.Dock = DockStyle.Fill;
                
        }

        [Import("PrjManager", typeof(ProjectManager))]
        public ProjectManager ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            App.DockManager.Add(new DockablePanel("kProjectExplorer", "Project Explorer",
                _ProjectExplorer, DockStyle.Left) { SmallImage = Properties.Resources.project16 });
            base.Activate();
            ProjectManager.ProjectExplorer = _ProjectExplorer;
            _ProjectExplorer.ProjectManger = ProjectManager;
            ProjectManager.SerializationManager.ProjectChanged += SerializationManager_ProjectChanged;
            AddMenuItems();
        }

        void SerializationManager_ProjectChanged(object sender, EventArgs e)
        {
            var prj = sender as IProject ;
            prj.ModelChanged += prj_ModelChanged;
        }

        void prj_ModelChanged(object sender, EventArgs e)
        {
            var prj = sender as IProject;
            prj.Model.GridChanged += Model_GridChanged;
        }

        void Model_GridChanged(object sender, EventArgs e)
        {
            var grid = (sender as IBasicModel).Grid;
            //RepositoryItemComboBox combo = (App.HeaderControl as RibbonHeader).ComboBoxes["kLayerDropDown"];

            //combo.Items.Clear();
            //for (int i = 0; i < grid.LayerCount; i++)
            //{
            //    combo.Items.Add("Layer " + i.ToString());
            //}
     
        }

             public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
          
            base.Deactivate();
        }

        private void AddMenuItems()
        {
            IHeaderControl header = App.HeaderControl;

            header.Add(new SimpleActionItem("kOptions", "Options", Option_Click)
            {
                GroupCaption = HeaderControl.ApplicationMenuKey,
                SortOrder = 100,
                SmallImage = Resources.options32,
                LargeImage = Resources.options32,
                ToolTipText = "Set options"
            });

            _layerDropDown = new DropDownActionItem()
           {
               Key = "kLayerDropDown",
               RootKey = "kModel",
               Width = 145,
               AllowEditingText = false,
               ToolTipText = "Select current subsurface layer",
               GroupCaption = "Subsurface",
               DisplayText = "Select a layer"
           };
           _layerDropDown.SelectedValueChanged += _layerDropDown_SelectedValueChanged;
           App.HeaderControl.Add(_layerDropDown);
        }

        void _layerDropDown_SelectedValueChanged(object sender, SelectedValueChangedEventArgs e)
        {
            ProjectManager.CurrentGridLayer = int.Parse(e.SelectedItem.ToString());
            
        }

        private void Option_Click(object sender, EventArgs e)
        {
            (ProjectManager.ConfigManager as  ConfigManager).OptionForm.ShowDialog();
        }
    }
}
