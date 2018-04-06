using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using Heiflow.Applications.Controllers;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Plugins.DataGridPanel.Properties;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Plugins.DataGridPanel
{
    public class DataGridPanel : Extension
    {
        DataGridEx _DataGridEx;
        public DataGridPanel()
        {
            DeactivationAllowed = false;

            _DataGridEx = new DataGridEx();
        }
    

        [Import("PrjManager", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            App.DockManager.Add(new DockablePanel("kDataGridPanel", "Data Table",
                _DataGridEx, DockStyle.Fill) { SmallImage = Resources.data16 });
            base.Activate();
            ProjectManager.ShellService.DataGridPanel = _DataGridEx;

            AddMenuItems();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        private void AddMenuItems()
        {
         
        }

        private void Option_Click(object sender, EventArgs e)
        {
          
        }
    }
}
