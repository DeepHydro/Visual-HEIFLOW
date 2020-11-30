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
            NoDataValue = 0;
            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "scatter3d.html");
            MinHeight = 600;
            MinWidth = 800;
            MaxSymbolSize = 25;
            MinSymbolSize = 5;
            Beta = 80;
            MaxDistance = 130;
            MinDistance = 90;
        }
        [Category("Data")]
        [Description("The input data cube. Its sytle should be mat[:][0][:]")]
        public string InputDataCube
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
        [Description("")]
        [Category("View")]
        public float Beta
        {
            get;
            set;
        }
        [Description("")]
        [Category("View")]
        public float MaxDistance
        {
            get;
            set;
        }
        [Description("")]
        [Category("View")]
        public float MinDistance
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
            this.Initialized = Validate(InputDataCube);
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
            var var_index = 0;
            var vecx = Get3DMat(InputDataCube, ref var_index);
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
                float cellv = 0;
                //int k = 0;
                //var mat_x = grid.ToMatrix<float>(vecx, 0);

                for (int j = 0; j < grid.ColumnCount; j++)
                {
                    for (int i = 0; i < grid.RowCount; i++)
                    {
                        var hruid = grid.Topology.GetSerialIndex(i, j);
                        if (hruid < 0)
                        {
                            for (int k = 0; k < grid.ActualLayerCount; k++)
                            {
                                newline = string.Format("data.push([{0}, {1}, {2}, {3}]);", i, j, k, NoDataValue);
                                sw.WriteLine(newline);
                            }
                        }
                        else
                        {
                            for (int k = 0; k < grid.ActualLayerCount; k++)
                            {
                                cellv = vecx[var_index, TimeStep, hruid + (grid.ActualLayerCount - k - 1) * grid.ActiveCellCount];
                                valMax = System.Math.Max(valMax, cellv);
                                valMin = System.Math.Min(valMin, cellv);
                                newline = string.Format("data.push([{0}, {1}, {2}, {3}]);", i, j, k, cellv);
                                sw.WriteLine(newline);
                            }
                        }
                    }
                    progress = j * 100 / grid.RowCount;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing column: " + (j + 1));
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
                    if (line.Contains("symbolSize"))
                    {
                        newline = string.Format("\t\t\t\tsymbolSize: [{0}, {1}],", MinSymbolSize, MaxSymbolSize);
                    }
                    if (line.Contains("min:"))
                    {
                        newline = string.Format("\t\t\t\tmin: {0},", valMin);
                    }
                    if (line.Contains("max:"))
                    {
                        newline = string.Format("\t\t\t\tmax: {0},", valMax);
                    }
                    if (line.Contains("beta:"))
                    {
                        newline = string.Format("\t\t\tbeta: {0},", Beta);
                    }
                    if (line.Contains("maxDistance:"))
                    {
                        newline = string.Format("\t\t\tmaxDistance: {0},", MaxDistance);
                    }
                    if (line.Contains("minDistance:"))
                    {
                        newline = string.Format("\t\t\tminDistance: {0},", MinDistance);
                    }
                    sw.WriteLine(newline);
                }
                isgenerated = true;
            }
            catch (Exception ex)
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
