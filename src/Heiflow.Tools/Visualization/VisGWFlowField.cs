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

using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Models.Visualization;
using Heiflow.Presentation.Services;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Visualization
{
    public class VisGWFlowField : ModelTool
    {
        public VisGWFlowField()
        {
            Name = "Visualize Groundwater Flow Field";
            Category = "Visualization";
            Description = " ";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            DimX = 1;
            DimY = 2;
            TimeStep = 0;
            TimeInteval = 8640;
            NoDataValue = 9999;
            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GWFlowField.html");
            MinHeight = 600;
            MinWidth = 800;
            ParticleDensity = 256;
            ParticleOpacity = 1;
            ParticleSize = 2;
        }

 
        [Description("The dimension representing X component. It starts from 0.")]
        [Category("Data")]
        public int DimX
        {
            get;
            set;
        }
        [Description("The dimension representing X component. Its value starts from 0.")]
        [Category("Data")]
        public int DimY
        {
            get;
            set;
        }
        [Description("The time step at which the data cube slice will be exported. Its value starts from 0.")]
        [Category("Data")]
        public int TimeStep
        {
            get;
            set;
        }
        [Description("The time intel in the unit of seconds.")]
        [Category("Data")]
        public int TimeInteval
        {
            get;
            set;
        }
        [Description("The time intel in the unit of seconds.")]
        [Category("Data")]
        public double NoDataValue
        {
            get;
            set;
        }
        [Description("The minimum width of the plot")]
        [Category("Layout")]
        public int MinWidth
        {
            get;
            set;
        }
        [Description("The minimum height of the plot")]
        [Category("Layout")]
        public int MinHeight
        {
            get;
            set;
        }
        [Description("The minimum height of the plot")]
        [Category("Layout")]
        public float LayoutFactor
        {
            get;
            set;
        }
        [Description("The density of particles")]
        [Category("Particle Effect")]
        public int ParticleDensity
        {
            get;
            set;
        }
        [Description("The size of particles")]
        [Category("Particle Effect")]
        public float ParticleSize
        {
            get;
            set;
        }
        [Description("The size of particles")]
        [Category("Particle Effect")]
        public float ParticleOpacity
        {
            get;
            set;
        }
        [Category("File")]
        [Description("The output file name")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(false)]
        public string FileName
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int progress = 0;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var cbcpck = prj.Project.Model.GetPackage(CBCPackage.PackageName) as CBCPackage;
            var cbc = cbcpck.DataCube;
            string echart = Path.Combine(Application.StartupPath, "External\\Echarts\\flowGL.html");
            FileName = Path.Combine(Application.StartupPath, string.Format("External\\Echarts\\flowGL{0}.html", TimeStep));
            EChartsFile template = new EChartsFile();
            template.Initialize(echart);
            if ( cbc != null && cbc.IsAllocated(DimX) && cbc.IsAllocated(DimY))
            {
                 int nsteps = cbc.Size[1];
                 if (TimeStep < 0 || TimeStep > (nsteps-1))
                     TimeStep = 0;
                 if (TimeInteval <= 0)
                     TimeInteval = 864;

                 string newline = "";
                var vecx = cbc[DimX,TimeStep.ToString(),":"];
                 var vecy = cbc[DimY,TimeStep.ToString(),":"];
                 StreamWriter sw = new StreamWriter(FileName);
                 foreach (var line in template.TopSection)
                 {
                     newline = line;
                     if (line.Contains("container") && line.Contains("div"))
                     {
                         int width = (int)(grid.RowCount * LayoutFactor);
                         int height = (int)(grid.ColumnCount * LayoutFactor);
                         width = System.Math.Max(width, MinWidth);
                         height = System.Math.Max(height, MinHeight);
                         newline = string.Format(@"<div id=""container"" style=""height: {0}pt;width: {1}pt""></div>", height, width);
                     }
                     sw.WriteLine(newline);
                 }
                sw.WriteLine("function generateData() {");
                sw.WriteLine("data = [];");

                double valMax = float.MinValue;
                double valMin = float.MaxValue;
                double mag=0;
                double xx = 0;
                double yy = 0;
                int k = 0;
               var mat_x = grid.ToMatrix<float>(vecx, 0);
               var mat_y = grid.ToMatrix<float>(vecy, 0);
               for (int i = 0; i < grid.ColumnCount; i++)
               {
                   for (int j = 0; j < grid.RowCount; j++)
                   {
                       xx = System.Math.Round(mat_x[j][i] / TimeInteval, 4);
                       yy = System.Math.Round(-mat_y[j][i] / TimeInteval, 4);
                       mag = System.Math.Sqrt(xx * xx + yy *yy);
                       mag = System.Math.Round(mag,4);
                       if (mag != NoDataValue)
                       {
                           newline = string.Format("data[{0}] = [{1},{2},{3},{4},{5}];", k, i, grid.RowCount - 1 - j, xx, yy, mag);
                           valMax = System.Math.Max(valMax, mag);
                           valMin = System.Math.Min(valMin, mag);
                           sw.WriteLine(newline);
                           k++;
                       }
                   }
                   progress = i * 100 / grid.ColumnCount;
                   cancelProgressHandler.Progress("Package_Tool", progress, "Processing column: " + (i + 1));
               }
               sw.WriteLine("return data;");
               sw.WriteLine("}");
               newline = string.Format("var valMax = {0};", valMax);
               sw.WriteLine(newline);
               newline = string.Format("var valMin = {0};", valMin);
               sw.WriteLine(newline);
               newline = string.Format("var nrow = {0};", grid.RowCount);
               sw.WriteLine(newline);
               newline = string.Format("var ncol = {0};", grid.ColumnCount);
               sw.WriteLine(newline);

               sw.WriteLine("var data = generateData();");
               sw.WriteLine("//set options");
               foreach (var line in template.EndSection)
               {
                   newline = line;
                   if (line.Contains("particleDensity"))
                   {
                       newline = string.Format("particleDensity: {0},", ParticleDensity);
                   }
                   else if (line.Contains("particleSize"))
                   {
                       newline = string.Format("particleSize: {0},", ParticleSize);
                   }
                   sw.WriteLine(newline);
               }
                sw.Close();
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Invalid parameters");
                return false;
            }

        }

        public override void AfterExecution(object args)
        {
               string url = string.Format("file:///{0}", FileName);
               System.Diagnostics.Process.Start("explorer.exe", url);
        }
    }
}
