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
    public class VisScatter3D : ModelTool
    {
        private bool isgenerated = false;
        public VisScatter3D()
        {
            Name = "Plot 3D Scatter";
            Category = "Visualization";
            Description = " ";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            TimeStep = 0;
            NoDataValue = 9999;
            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "scatter3d.html");
            MinHeight = 600;
            MinWidth = 800;
            MaxSymbolSize = 5;
            MinSymbolSize = 0.5f;
        }
        [Category("Data")]
        [Description("The input data cube. Its sytle should be mat[0][0][:]")]
        public string InputVector
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
        [Description("The grid layer starting from 0")]
        [Category("Data")]
        public int Layer
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
        [Description("The min size of symbol")]
        [Category("Symbol Effect")]
        public bool UseGridElevation
        {
            get;
            set;
        }

        [Description("The max size of symbol")]
        [Category("Symbol Effect")]
        public float MaxSymbolSize
        {
            get;
            set;
        }
        [Description("The min size of symbol")]
        [Category("Symbol Effect")]
        public float MinSymbolSize
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
            this.Initialized = Validate(InputVector);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int progress = 0;
            isgenerated = false;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            string echart = Path.Combine(Application.StartupPath, "External\\Echarts\\scatter3d.html");
            FileName = Path.Combine(Application.StartupPath, string.Format("External\\Echarts\\scatter3d{0}.html", TimeStep));
            EChartsFile template = new EChartsFile();
            template.Initialize(echart);

            string newline = "";
            var vecx = GetVector(InputVector);
            StreamWriter sw = new StreamWriter(FileName);
            try
            {
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
                //int k = 0;
                //var mat_x = grid.ToMatrix<float>(vecx, 0);

                if (UseGridElevation)
                {
                    for (int i = 0; i < vecx.Length; i++)
                    {
                        var loc = grid.Topology.ActiveCellLocation[i];
                        newline = string.Format("data[{0}] = [{1},{2},{3},{4}];", i, loc[0], loc[1], grid.Elevations[Layer,0,i], vecx[i]);
                        valMax = System.Math.Max(valMax, vecx[i]);
                        valMin = System.Math.Min(valMin, vecx[i]);
                        sw.WriteLine(newline);
                        progress = i * 100 / grid.ColumnCount;
                        //cancelProgressHandler.Progress("Package_Tool", progress, "Processing column: " + (i + 1));
                    }
                }
                else
                {
                    for (int i = 0; i < vecx.Length; i++)
                    {
                        var loc = grid.Topology.ActiveCellLocation[i];
                        newline = string.Format("data[{0}] = [{1},{2},{3},{4}];", i, loc[0], loc[1], 1, vecx[i]);
                        valMax = System.Math.Max(valMax, vecx[i]);
                        valMin = System.Math.Min(valMin, vecx[i]);
                        sw.WriteLine(newline);
                        progress = i * 100 / grid.ColumnCount;
                        //cancelProgressHandler.Progress("Package_Tool", progress, "Processing column: " + (i + 1));
                    }
                }
                //for (int i = 0; i < grid.ColumnCount; i++)
                //{
                //    for (int j = 0; j < grid.RowCount; j++)
                //    {
                //        var mag = System.Math.Round(mat_x[j][i], 4);
                //        if (mag != NoDataValue)
                //        {
                //            newline = string.Format("data[{0}] = [{1},{2},{3},{4}];", k, i, grid.RowCount - 1 - j, grid.GetElevationAt(j, i, Layer), mag);
                //            valMax = System.Math.Max(valMax, mag);
                //            valMin = System.Math.Min(valMin, mag);
                //            sw.WriteLine(newline);
                //            k++;
                //        }
                //    }
                //    progress = i * 100 / grid.ColumnCount;
                //    cancelProgressHandler.Progress("Package_Tool", progress, "Processing column: " + (i + 1));
                //}
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
                    if (line.Contains("symbolSize"))
                    {
                        newline = string.Format("symbolSize: [{0}, {1}],", MinSymbolSize, MaxSymbolSize);
                    }
                    sw.WriteLine(newline);
                }
                isgenerated = true;
            }
            catch(Exception ex)
            {
                isgenerated = false;
                cancelProgressHandler.Progress("Package_Tool", 100, "Faile. Error message: " + ex.Message);
            }
            finally
            {
                sw.Close();
            }      
            return true;
        }

        public override void AfterExecution(object args)
        {
            if (isgenerated)
            {
                string url = string.Format("file:///{0}", FileName);
                System.Diagnostics.Process.Start("explorer.exe", url);
            }
        }
    }
}
