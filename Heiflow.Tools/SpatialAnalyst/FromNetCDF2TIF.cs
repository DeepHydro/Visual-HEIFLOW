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

using DotSpatial.Data;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using Heiflow.Controls.WinForm.Editors;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.NetCDF4;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Conversion
{
    public class FromNetCDF2TIF : ModelTool
    {
        private string _ncFileName;
        private string _SelectedVariableName;
        private string _XDimensionName;
        private string _YDimensionName;
        private string _TimeDimensionName;
        private int _SelectedVarIndex = 0;
        private ReadOnlyVariableCollection _Variables;
        private Variable _XVariable;
        private Variable _YVariable;
        private Variable _TimeVariable;
        private Variable _SelectedVariable;

        public FromNetCDF2TIF()
        {
            Name = "Convert from NetCDF to TIF";
            Category = "Conversion";
            Description = "Convert nc file to tif files";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            OutputFolder = Environment.SpecialFolder.MyDocuments.ToString();
            Projection = ProjectionInfo.FromEpsgCode(4326); 
        }

        [Category("Input")]
        [Description("The nc filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string NCFileName
        {
            get
            {
                return _ncFileName;
            }
            set
            {
                _ncFileName = value;
                GetNCInfo();
            }
        }


        [Category("Parameter")]
        [Description("Name of the variable")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("NCVariables")]
        public string Variable
        {
            get
            {
                return _SelectedVariableName;
            }
            set
            {
                _SelectedVariableName = value;
                if (NCVariables != null)
                {
                    for (int i = 0; i < NCVariables.Length; i++)
                    {
                        if (_SelectedVariableName == NCVariables[i])
                        {
                            _SelectedVarIndex = i;
                            _SelectedVariable = _Variables[_SelectedVarIndex];
                            var dim = from dd in _SelectedVariable.Dimensions select dd.Name;
                            Dimensions = dim.ToArray();
                            break;
                        }
                    }
                }
            }
        }

        [Category("Parameter")]
        [Description("Dimension name of the X coordinate")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Dimensions")]
        public string XDimension
        {
            get
            {
                return _XDimensionName;
            }
            set
            {
                _XDimensionName = value;
                _XVariable = (from vv in _Variables where vv.Name == _XDimensionName select vv).First();
            }
        }

        [Category("Parameter")]
        [Description("Dimension name of the Y coordinate")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Dimensions")]
        public string YDimension
        {
            get
            {
                return _YDimensionName;
            }
            set
            {
                _YDimensionName = value;
                _YVariable = (from vv in _Variables where vv.Name == _YDimensionName select vv).First();
            }
        }

        [Category("Parameter")]
        [Description("Dimension name of the Y coordinate")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Dimensions")]
        public string TimeDimension
        {
            get
            {
                return _TimeDimensionName;
            }
            set
            {
                _TimeDimensionName = value;
                _TimeVariable = (from vv in _Variables where vv.Name == _TimeDimensionName select vv).First();
            }
        }

        [Category("Output")]
        [Description("The  folder where the tif will be saved")]
        public string OutputFolder
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("The spatial reference of the grid")]
        [EditorAttribute(typeof(SRSEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ProjectionInfo Projection
        {
            get;
            set;
        }

        [Browsable(false)]
        public string[] NCVariables
        {
            get;
            protected set;
        }

        [Browsable(false)]
        public string[] Dimensions
        {
            get;
            protected set;
        }

        public override void Initialize()
        {
            Initialized = true;
            if (!File.Exists(NCFileName))
                Initialized = false;

            if (_YVariable == null || _XVariable == null || _SelectedVariable == null || _TimeVariable == null)
                Initialized = false;

            if (!Directory.Exists(OutputFolder))
                Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int nstep = 0;
            var xx = _XVariable.GetData() as float[];
            var yy = _YVariable.GetData() as float[];
            var nc_array = _SelectedVariable.GetData() as float[, ,];
            var time = _TimeVariable.GetData() as float[];
            if (time != null)
                nstep = time.Count();
            else
            {
                var tt = _TimeVariable.GetData() as double[];
                if (tt != null)
                    nstep = tt.Length;
                else
                {
                    var t1 = _TimeVariable.GetData() as int[];
                    if (t1 != null)
                        nstep = t1.Length;
                }
            }
            int progress = 0;
            var numColumns = xx.Length;
            var numRows = yy.Length;
            Extent extent = new Extent(0, -90, 360, 90);
            RasterBounds bound = new RasterBounds(numRows, numColumns, extent);

            if (nc_array != null)
            {
                for (int t = 0; t < nstep; t++)
                {
                    var fn = Path.Combine(OutputFolder, (t + 1) + ".tif");
                    var raster = Raster.CreateRaster(fn, string.Empty, numColumns, numRows, 1, typeof(float), new[] { string.Empty });
                    raster.Bounds = bound;
                    raster.Projection = Projection;
                    for (int row = 0; row < numRows; row++)
                    {
                        for (int j = 0; j < numColumns; j++)
                        {
                            if (nc_array[t, row, j] > 0)
                                raster.Value[row, j] = nc_array[t, row, j];
                        }
                    }
                    raster.Save();
                    progress = t * 100 / nstep;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing time step:" + t);
                }
            }
            return true;
        }

        private void GetNCInfo()
        {
            if (File.Exists(NCFileName))
            {
                NetCDFDataSet test = new NetCDFDataSet(NCFileName);
                _Variables = test.Variables;
                var buf = from vv in _Variables select vv.Name;
                NCVariables = buf.ToArray();
            }
        }

        public int[][] GetIndex(float[] xAxis, float[] yAxis, Coordinate [] points)
        {
            int npt = points.Length;
            int[][] coords = new int[npt][];

            for (int i = 0; i < npt; i++)
            {
                coords[i] = new int[2];
                for (int x = 0; x < xAxis.Length - 1; x++)
                {
                    if (points[i].X >= xAxis[x] && points[i].X < xAxis[x + 1])
                    {
                        var x1 = System.Math.Abs(points[i].X - xAxis[x]) - System.Math.Abs(points[i].X - xAxis[x + 1]);
                        if (x1 > 0)
                            coords[i][0] = x;
                        else
                            coords[i][0] = x + 1;
                        break;
                    }
              
                }
                for (int y = 0; y < yAxis.Length - 1; y++)             
                {
                    if (points[i].Y >= yAxis[y] && points[i].Y < yAxis[y + 1])
                    {
                        var x1 = System.Math.Abs(points[i].Y - yAxis[y]) - System.Math.Abs(points[i].Y - yAxis[y + 1]);
                        if (x1 > 0)
                            coords[i][1] = y;
                        else
                            coords[i][1] = y + 1;
                        break;
                    }
                }
            }

            return coords;
        }
    }
}