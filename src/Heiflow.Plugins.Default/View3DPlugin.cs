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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Plugins.Default.Properties;
using Heiflow.Presentation;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Heiflow.Plugins.View3DPanel
{
    public class View3DPlugin : Extension
    {
        private Win3DView _Win3DView;
        private GridProfileViewer _GridProfileViewer;
        public View3DPlugin()
        {
            DeactivationAllowed = false;
            _Win3DView = new Win3DView();
            _GridProfileViewer = new GridProfileViewer();
        }


        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            var showView3D = new SimpleActionItem("kView", Resources.View_3d, delegate(object sender, EventArgs e)
            { ProjectManager.ShellService.SurfacePlot.ShowView(ProjectManager.ShellService.MainForm); })
            {
                Key = "kShowView3D",
                ToolTipText = Resources.View_3d_tips,
                GroupCaption = Resources.Data_Group,
                LargeImage = Resources._3d_plot_printing_printer
            };
            App.HeaderControl.Add(showView3D);

            var terrainViewer = new SimpleActionItem("kView", Resources.Terrain_Viewer, ShowTerrainViewer)
            {
                Key = "kTerrainViewer",
                ToolTipText = Resources.Terrain_Viewer_tips,
                GroupCaption = Resources.Data_Group,
                LargeImage = Resources.rotation3d32
            };
            App.HeaderControl.Add(terrainViewer);

            var gridprofileViewer = new SimpleActionItem("kView", Resources.Vertical_Profile_Viewer, delegate(object sender, EventArgs e)
            { ProjectManager.ShellService.VerticalProfileView.ShowView(ProjectManager.ShellService.MainForm); })
            {
                Key = "kGridProfileViewer",
                ToolTipText =  Resources.Vertical_Profile_Viewer_tips,
                GroupCaption = Resources.Data_Group,
                LargeImage = Resources.LasRGB32
            };
            App.HeaderControl.Add(gridprofileViewer);

            base.Activate();
            ProjectManager.ShellService.SurfacePlot = _Win3DView;
            ProjectManager.ShellService.AddChild(_Win3DView);

            ProjectManager.ShellService.VerticalProfileView = _GridProfileViewer;
            ProjectManager.ShellService.AddChild(_GridProfileViewer);

        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowView3D");
            App.HeaderControl.Remove("kTerrainViewer");
            App.HeaderControl.Remove("kGridProfileViewer");
            base.Deactivate();
        }

        public void ShowTerrainViewer(object sender, EventArgs e)
        {
            var workdic = Path.Combine(Application.StartupPath, "External\\TerrainViewer");
            var exefile = Path.Combine(Application.StartupPath, "External\\TerrainViewer\\TerrainViewer.exe");
            var mapfile = Path.Combine(ProjectManager.Project.GeoSpatialDirectory, "map.xml");
            if (!File.Exists(mapfile))
            {
                MessageBox.Show(mapfile + " for Terrain Viewer not found.");
                return;
            }
            if (File.Exists(exefile))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = exefile;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.WorkingDirectory = workdic;
                startInfo.Arguments = Path.Combine(ProjectManager.Project.GeoSpatialDirectory, "map.xml");
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    Process exeProcess = Process.Start(startInfo);

                }
                catch
                {
                    // Log error.
                }
            }
            else
            {
                MessageBox.Show(exefile + " not found.");
            }
        }
    }
}
