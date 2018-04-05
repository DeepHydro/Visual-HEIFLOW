// --------------------------------------------------------------------------------------------------------------------
// <copyright file="" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// <summary>
//   View Attribute Data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using DotSpatial.Controls;
    using DotSpatial.Controls.Docking;
    using DotSpatial.Symbology;
    using DotSpatial.Plugins.AttributeDataExplorer.Properties;

    public class AttributeDataExplorerPlugin : Extension
    {
        private const string STR_KDataExplorer = "kDataExplorer";
        private MainForm _MainForm;
        private DockablePanel _Panel;

        public override void Activate()
        {
            this._MainForm = new MainForm(this.App);
            this._Panel = new DockablePanel(STR_KDataExplorer, _MainForm.Text, _MainForm.hostpanel, DockStyle.Right)
            {
                SmallImage = Resources.datasheet16
            };
            App.DockManager.Add(_Panel);

            // capture the event when the user closes the pane and unload the plugin.
            //App.DockManager.PanelClosed += (sender, e) =>
            //{
            //    if (e.ActivePanelKey == STR_KDataExplorer && !isTerminating)
            //    {
            //        Deactivate();
            //    }
            //};
            _MainForm.UILoaded();
            _MainForm.TextChanged += delegate(object sender, EventArgs e)
                                     {
                                         _Panel.Caption = _MainForm.Text;
                                     };
            App.DockManager.ShowPanel(STR_KDataExplorer);
            base.Activate();
        }

        
        public override void Deactivate()
        {
            if (this._MainForm != null && !this._MainForm.IsDisposed)
            {
                this.App.DockManager.Remove(STR_KDataExplorer);
                this._MainForm.Dispose(); 
            }
            base.Deactivate();
        }
    }
}