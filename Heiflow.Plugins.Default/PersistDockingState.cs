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