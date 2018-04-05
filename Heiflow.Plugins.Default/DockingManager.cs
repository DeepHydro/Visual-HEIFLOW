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

using DevExpress.Utils;

namespace DotSpatial.Plugins.DockManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using DevExpress.XtraBars.Docking;
    using DotSpatial.Controls.Docking;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using System.Reflection;
    using DevExpress.XtraBars.Docking2010.Views.Tabbed;

    public class DockingManager : IDockManager, IPartImportsSatisfiedNotification
    {
        [Import( "Shell", typeof( ContainerControl ) )]
        public ContainerControl Shell { get; set; }

        private bool isFormLoaded;

        private DevExpress.XtraBars.Docking.DockManager dockManager;
        private DevExpress.XtraBars.Docking2010.DocumentManager documentManager;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView tabbedView;
        List<DockablePanel> loadingDeferredDockPanels = new List<DockablePanel>();
        private PersistDockingState persistDockingState;

        /// <summary>
        /// Initializes a new instance of the DockingManager class.
        /// </summary>
        public DockingManager()
        {

        }

       public DevExpress.XtraBars.Docking.DockManager DevDockManager
        {
           get
            {
                return dockManager;
            }
        }

        public event EventHandler<DockablePanelEventArgs> PanelHidden;

        private void dockManager_ActivePanelChanged( object sender, DevExpress.XtraBars.Docking.ActivePanelChangedEventArgs e )
        {
            if ( e.Panel != null )
                OnActivePanelChanged( new DockablePanelEventArgs( e.Panel.Name ) );
        }

        private DockPanel CreatePanel( DockingStyle style )
        {
            foreach ( DockPanel panel in dockManager.Panels )
            {
                if ( panel.Dock == style )
                {
                    // todo: allow plugin developer to specify other orientations:
                    // if we call dockManager.AddPanel(style), we would create a vertical split
                    // if we call panel.AddPanel, we create a horizontal split.

                    // create a new tab.
                    var newPanel = panel.AddPanel();
                    if ( panel.ParentPanel != null )
                        panel.ParentPanel.Tabbed = true;
               
                    return newPanel;
                }
            }

            return dockManager.AddPanel( style );
        }

        private void UpdateSmallImage( DockablePanel panel, DockPanel dockPanel )
        {
            if ( panel.SmallImage != null )
            {
                ImageCollection images = (ImageCollection)dockManager.Images;
                images.AddImage( panel.SmallImage, panel.Key );
                dockPanel.ImageIndex = images.Images.Count - 1;
            }
        }

        public void Add( DockablePanel panel )
        {
            Guard.ArgumentNotNull( panel, "panel" );
            // we can't generate the Name (key) since we need it to persist with the dock panel layout.
            Guard.ArgumentNotNull( panel.Key, "Key" );
            Guard.ArgumentNotNull( panel.InnerControl, "Panel" );

            DockingStyle style = ConvertToDockingStyle( panel.Dock );
            DockPanel dockPanel;
            if ( panel.Dock == DockStyle.Fill )
            {
                // A dock panel cannot be created and docked to the form using the DockingStyle.Fill style,
                // that is a panel cannot occupy the form entirely. It can only be docked to the form's edge
                // or float. 

                if ( isFormLoaded )
                {
                    dockPanel = dockManager.AddPanel( DockingStyle.Float );
                }
                else
                {
                    loadingDeferredDockPanels.Add( panel );
                    return;
                }
            }
            else
            {
                dockPanel = CreatePanel( style );

            }

            dockPanel.Text = panel.Caption;
            dockPanel.Width = panel.InnerControl.Width;
            dockPanel.Height = panel.InnerControl.Height;
            dockPanel.Name = panel.Key;
            dockPanel.ID = StringToGuid( panel.Key ); // allows us to serialize layout.
            UpdateSmallImage( panel, dockPanel );
            if (style == DockingStyle.Float)
            {
                dockPanel.FloatSize = new System.Drawing.Size(panel.InnerControl.Width, panel.InnerControl.Height);
                dockPanel.FloatLocation = new System.Drawing.Point(Shell.Location.X + 200, Shell.Location.Y + 100);
                dockPanel.Visibility = DockVisibility.Hidden;
            }
            dockPanel.Controls.Add( panel.InnerControl );
            panel.InnerControl.Dock = DockStyle.Fill;

            if ( panel.Dock == DockStyle.Fill )
            {
                tabbedView.Controller.Dock( dockPanel );
            }
            panel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler( panel_PropertyChanged );
            OnPanelAdded( new DockablePanelEventArgs( panel.Key ) );
        }

        void panel_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
        {
            var item = sender as DockablePanel;
            var guiItem = dockManager.Panels[item.Key];
            if (guiItem == null)
                return;
            switch ( e.PropertyName )
            {
                case "Caption":
                    guiItem.Text = item.Caption;
                    break;

                case "Dock":
                    break;

                case "SmallImage":
                    break;

                default:
                    break;
            }
        }

        private static Guid StringToGuid( string value )
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            using ( MD5 md5Hasher = MD5.Create() )
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash( Encoding.Default.GetBytes( value ) );
                return new Guid( data );
            }
        }


        /// <summary>
        /// Removes the specified key. This method should be called in Form Shown or later
        /// (After the panels have been restored).
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove( string key )
        {
            foreach ( DockPanel panel in dockManager.Panels )
            {
                if ( panel.Name == key )
                {
                    dockManager.RemovePanel( panel );
                    OnPanelRemoved( new DockablePanelEventArgs( key ) );
                    break;
                }
            }
        }

        private DockingStyle ConvertToDockingStyle( DockStyle dockStyle )
        {
            switch ( dockStyle )
            {
                case DockStyle.Bottom:
                    return DockingStyle.Bottom;
                case DockStyle.Fill:
                    return DockingStyle.Fill;
                case DockStyle.Left:
                    return DockingStyle.Left;
                case DockStyle.None:
                    return DockingStyle.Float;
                case DockStyle.Right:
                    return DockingStyle.Right;
                case DockStyle.Top:
                    return DockingStyle.Top;
                default:
                    throw new NotSupportedException();
            }
        }

        public void OnImportsSatisfied()
        {
            Form mainForm = Shell as Form;

            this.dockManager = new DevExpress.XtraBars.Docking.DockManager();
            this.documentManager = new DevExpress.XtraBars.Docking2010.DocumentManager();
            this.tabbedView = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView();
            ( (System.ComponentModel.ISupportInitialize)( this.dockManager ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.documentManager ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.tabbedView ) ).BeginInit();
            Shell.SuspendLayout();
            // 
            // dockManager1
            // 
            this.dockManager.Form = Shell;
            this.dockManager.TopZIndexControls.AddRange( new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl" } );
            this.dockManager.Images = new ImageCollection();
            // 
            // documentManager1
            // 

            this.documentManager.MdiParent = mainForm;
            this.documentManager.View = this.tabbedView;
            this.documentManager.ViewCollection.AddRange( new DevExpress.XtraBars.Docking2010.Views.BaseView[] {
            this.tabbedView } );

            tabbedView.DocumentGroupProperties.ClosePageButtonShowMode =
                DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;

            ( (System.ComponentModel.ISupportInitialize)( this.dockManager ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.documentManager ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.tabbedView ) ).EndInit();
            Shell.ResumeLayout( false );

            dockManager.ActivePanelChanged += new ActivePanelChangedEventHandler( dockManager_ActivePanelChanged );
            dockManager.ClosedPanel += new DockPanelEventHandler( dockManager_ClosedPanel );

            persistDockingState = new PersistDockingState( dockManager );
            mainForm.Load += delegate( object sender, EventArgs e )
            {
                isFormLoaded = true;
                foreach ( var panel in loadingDeferredDockPanels )
                {
                    Add( panel );
                }
                loadingDeferredDockPanels = null;
                //persistDockingState.InitializeDefaultLayout();
              //  persistDockingState.RestoreLayout();
            };

            mainForm.FormClosing += delegate( object sender, FormClosingEventArgs e )
            {
                persistDockingState.SaveLayout();
            };
        }

        void dockManager_ClosedPanel( object sender, DockPanelEventArgs e )
        {
            OnPanelClosed( new DockablePanelEventArgs( e.Panel.Name ) );
        }

        /// <summary>
        /// Occurs when the active panel is changed.
        /// </summary>
        public event EventHandler<DockablePanelEventArgs> ActivePanelChanged;

        #region OnActivePanelChanged
        /// <summary>
        /// Triggers the ActivePanelChanged event.
        /// </summary>
        public virtual void OnActivePanelChanged( DockablePanelEventArgs ea )
        {
            if ( ActivePanelChanged != null )
                ActivePanelChanged( this, ea );
        }
        #endregion

        public event EventHandler<DockablePanelEventArgs> PanelAdded;

        #region OnPanelAdded
        /// <summary>
        /// Triggers the PanelAdded event.
        /// </summary>
        public virtual void OnPanelAdded( DockablePanelEventArgs ea )
        {
            if ( PanelAdded != null )
                PanelAdded( this, ea );
        }
        #endregion

        public event EventHandler<DockablePanelEventArgs> PanelClosed;

        #region OnPanelClosed
        /// <summary>
        /// Triggers the PanelClosed event.
        /// </summary>
        public virtual void OnPanelClosed( DockablePanelEventArgs ea )
        {
            if ( PanelClosed != null )
                PanelClosed( this, ea );
        }
        #endregion

        public event EventHandler<DockablePanelEventArgs> PanelRemoved;

        #region OnPanelRemoved
        /// <summary>
        /// Triggers the PanelRemoved event.
        /// </summary>
        public virtual void OnPanelRemoved( DockablePanelEventArgs ea )
        {
            if ( PanelRemoved != null )
                PanelRemoved( this, ea );
        }
        #endregion

        /// <summary>
        /// Selects the panel.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SelectPanel( string key )
        {
            DockPanel panel = dockManager.Panels[key];
            if ( panel == null )
            {
                Debug.WriteLine( "No panel with key: " + key );
                return;
            }

            ShowIfHidden( panel );

            if ( panel.Dock == DockingStyle.Float )
            {
                documentManager.View.ActivateDocument( panel.FloatForm );
            }
            else
            {
                ShowIfHidden( panel.RootPanel );
                dockManager.ActivePanel = panel;
            }
        }

        private static void ShowIfHidden( DockPanel panel )
        {
            if ( panel == null ) return;

            if ( panel.Visibility == DockVisibility.Hidden )
                panel.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
        }

        /// <summary>
        /// Resets the layout.
        /// </summary>
        public void ResetLayout()
        {
            persistDockingState.ResetLayout();
        }

        /// <summary>
        /// Hides the panel. A subsequent call to SelectPanel will show this panel in the same place it was when hidden.
        /// </summary>
        /// <param name="key">The key.</param>
        public void HidePanel( string key )
        {
            DockPanel panel = dockManager.Panels[key];
            if ( panel == null )
            {
                Debug.WriteLine( "No panel with key: " + key );
                return;
            }

            OnPanelHidden( new DockablePanelEventArgs( "key" ) );
            panel.Visibility = DockVisibility.Hidden;
        }

        // todo: should probably fire this event if the user hides a panel
        protected void OnPanelHidden( DockablePanelEventArgs e )
        {
            var handler = PanelHidden;
            if ( handler != null )
            {
                handler( this, e );
            }
        }

        public void ShowPanel( string key )
        {
            // hack: this method should show the panel without selecting it.
            SelectPanel( key );
        }
    }
}
