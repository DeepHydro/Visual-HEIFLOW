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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Plot
{
    public class PlotProfile : ModelTool
    {
        public enum VerticalPlot { Row, Column };

        public PlotProfile()
        {
            Name = "PlotProfile";
            Category = "Plot";
            Description = "Plot Vertical Profile";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            VerticalPlotType = VerticalPlot.Row;
            RowNumber = 1;
            ColumnNumber = 1;
            TimeNumber = 1;
            Save2Csv = false;
            OutputFileName = "D:\\profiledata.csv";
        }
        [Category("Input")]
        [Description("The input data cube name")]
        public string DataSource
        {
            get;
            set;
        }
        [Category("Para")]
        [Description("The row number starting from 1. This value is used when VerticalPlotType=Row")]
        public int RowNumber
        {
            get;
            set;
        }
        [Category("Para")]
        [Description("The column number starting from 1. This value is used when VerticalPlotType=Column")]
        public int ColumnNumber
        {
            get;
            set;
        }
        [Category("Para")]
        [Description("The time number starting from 1.")]
        public int TimeNumber
        {
            get;
            set;
        }

        [Category("Para")]
        [Description("The row number starting from 1. This value is used when VerticalPlotType=Row")]
        public VerticalPlot VerticalPlotType
        {
            get;
            set;
        }

        [Category("File")]
        [Description("The output filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get;
            set;
        }

        public bool Save2Csv
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = !string.IsNullOrEmpty(DataSource);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var mat = Workspace.Get(DataSource);
            StreamWriter sw = null;
            if (Save2Csv)
            {
                sw = new StreamWriter(OutputFileName);
            }
            if (mat != null && mat.Topology != null)
            {
                int nrow = mat.Topology.RowCount;
                int ncol = mat.Topology.ColumnCount;
                var index = 0;
                var profiledata = new float[mat.Topology.LayerCount][];
             

                if (VerticalPlotType == VerticalPlot.Row)
                {
                    for (int k = 0; k < mat.Topology.LayerCount; k++)
                    {
                        profiledata[k] = new float[ncol];
                        if (mat.IsAllocated(k))
                        {
                            for (int i = 0; i < mat.Topology.ColumnCount; i++)
                            {
                                index = mat.Topology.GetSerialIndex(RowNumber - 1, i);
                                if (index < 0)
                                    profiledata[k][i] = 0;
                                else
                                    profiledata[k][i] = mat[k, TimeNumber - 1, index];
                            }
                        }
                        var SeriesTitle = string.Format("Row Profile of Layer {0}", k + 1);
                        WorkspaceView.Plot<float>(profiledata[k], SeriesTitle, Models.UI.MySeriesChartType.FastLine);
                   
                        if(Save2Csv)
                        {
                            var line = string.Format("Layer {0},{1}", k + 1, string.Join(",", profiledata[k]));
                            sw.WriteLine(line);
                        }                   
                    }
                }
                else if (VerticalPlotType == VerticalPlot.Column)
                {
                    for (int k = 0; k < mat.Topology.LayerCount; k++)
                    {
                        profiledata[k] = new float[nrow];
                        if (mat.IsAllocated(k))
                        {
                            for (int i = 0; i < mat.Topology.RowCount; i++)
                            {
                                index = mat.Topology.GetSerialIndex(i, ColumnNumber - 1);
                                if (index < 0)
                                    profiledata[k][i] = 0;
                                else
                                    profiledata[k][i] = mat[k, TimeNumber - 1, index];
                            }
                        }
                        var seriesTitle = string.Format("Column Profile of Layer {0}", k + 1);
                        WorkspaceView.Plot<float>(profiledata[k], seriesTitle, Models.UI.MySeriesChartType.FastLine);
                        if (Save2Csv)
                        {
                            var line = string.Format("Layer {0},{1}", k + 1, string.Join(",", profiledata[k]));
                            sw.WriteLine(line);
                        }                   
                    }
                }
                cancelProgressHandler.Progress("Package_Tool", 100, "Finished.");
                if (Save2Csv)
                    sw.Close();
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed to plot. The input DataCube is invalid.");
                return false;
            }
        }
    }
}
