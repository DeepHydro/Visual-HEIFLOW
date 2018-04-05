// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace DotSpatial.Plugins.DockManager
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;
    using DevExpress.XtraBars.Docking;
    using DevExpress.XtraBars.Docking2010.Views.Tabbed;
    using DotSpatial.Controls.Docking;

    /// <summary>
    /// Persists docking state to a file in the assembly's folder.
    /// </summary>
    public class PersistDockingState
    {
        private const string FileName = "DotSpatial.Plugins.DockManager.Layout.xml";
        private DevExpress.XtraBars.Docking.DockManager dockManager;
        private MemoryStream defaultLayout;

        /// <summary>
        /// Initializes a new instance of the PersistDockingState class.
        /// </summary>
        /// <param name="dockManager"></param>
        public PersistDockingState(DevExpress.XtraBars.Docking.DockManager dockManager)
        {
            this.dockManager = dockManager;
        }

        private static string GetFilePath()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(directory, FileName);
        }

        /// <summary>
        /// Restores the layout.
        /// </summary>
        public void RestoreLayout()
        {
            string path = GetFilePath();
            if (File.Exists(path))
            {
                try
                {
                    dockManager.RestoreLayoutFromXml(path);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // There were more plugins the last time the layout was saved than there are now.
                    Trace.WriteLine("ArgumentOutOfRangeException while restoring dock layout.");
                }
                catch (InvalidCastException)
                {
                    // There were more plugins the last time the layout was saved than there are now.
                    Trace.WriteLine("InvalidCastException while restoring dock layout.");

                    ResetLayout();
                }
            }
        }

        /// <summary>
        /// Saves the layout.
        /// </summary>
        public void SaveLayout()
        {
            string path = GetFilePath();
            dockManager.SaveLayoutToXml(path);
        }

        /// <summary>
        /// Initializes the default layout.
        /// </summary>
        internal void InitializeDefaultLayout()
        {
            defaultLayout = new MemoryStream();

            dockManager.SaveLayoutToStream(defaultLayout);
        }

        /// <summary>
        /// Resets the layout of the dock panels to a developer specified location.
        /// </summary>
        public void ResetLayout()
        {
            // todo: consider plugins that have closed panels and don't want them displayed at this point when the layout is reset.
            // also: consider whether a plugin has been unloaded since the layout was persisted.
            if (defaultLayout != null)
            {
                defaultLayout.Position = 0;
                try
                {
                    dockManager.RestoreLayoutFromStream(defaultLayout);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // There were more plugins the last time the layout was saved than there are now.
                    Trace.WriteLine("ArgumentOutOfRangeException while resetting dock layout.");
                }
                catch (InvalidCastException)
                {
                    // There were more plugins the last time the layout was saved than there are now.
                    Trace.WriteLine("InvalidCastException while resetting dock layout.");
                }
            }
        }
    }
}