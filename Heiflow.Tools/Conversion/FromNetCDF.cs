// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Models.Generic;
using System.ComponentModel;
using Heiflow.Controls.WinForm.Editors;
using System.Windows.Forms.Design;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Microsoft.Research.Science.Data.NetCDF4;
using Microsoft.Research.Science.Data;

namespace Heiflow.Tools.Conversion
{
    public class FromNetCDF : ModelTool
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

        public FromNetCDF()
        {
            Name = "From NetCDF";
            Category = "Conversion";
            Description = "Extract data cube from nc file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
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

        [Category("Input")]
        [Description("The shpfile name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PointFeatureFileName { get; set; }

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
        [Description("Dimension name of the time")]
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
        [Description("The name of the output matrix")]
        public string OutputMatrix { get; set; }


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

        public override void Setup()
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var map_layers = (from layer in shell.MapAppManager.Map.Layers select new MapLayerDescriptor { LegendText = layer.LegendText, DataSet = layer.DataSet }).ToArray();

        }

        public override void Initialize()
        {
            Initialized = true;
            if (TypeConverterEx.IsNull(PointFeatureFileName))
            {
                Initialized = false;
            }

            if (!File.Exists(NCFileName))
                Initialized = false;

            if (_YVariable == null || _XVariable == null || _SelectedVariable == null)
                Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            IFeatureSet fs = null;
            if (!TypeConverterEx.IsNull(PointFeatureFileName) && File.Exists(PointFeatureFileName))
            {
                fs = FeatureSet.Open(PointFeatureFileName);
            }
            if (fs != null)
            {
                var npt = fs.NumRows();
                Coordinate[] coors = new Coordinate[npt];
                int progress = 0;
                for (int i = 0; i < npt; i++)
                {
                    var geo_pt = fs.GetFeature(i).Geometry;
                    coors[i] = geo_pt.Coordinate;
                }
                var time = _TimeVariable.GetData() as float[];
                var xx = _XVariable.GetData() as float[];
                var yy = _YVariable.GetData() as float[];
                var nc_array= _SelectedVariable.GetData() as float[,,];
                int nstep = time.Count();
                var mat_out = new My3DMat<float>(1, time.Length, npt);
                mat_out.Name = OutputMatrix;
                mat_out.Variables = new string[] { _SelectedVariableName };
                mat_out.DateTimes = new DateTime[nstep];

                var pt_index = GetIndex(xx, yy, coors);
                for (int t = 0; t < nstep; t++)
                {
                    for (int i = 0; i < npt; i++)
                    {
                        mat_out.Value[0][t][i] = nc_array[t, pt_index[i][1], pt_index[i][0]];
                    }
                    progress = t * 100 / nstep;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing time step:" + t);

                    mat_out.DateTimes[t] = DateTime.FromOADate(time[t]);
                }

                Workspace.Add(mat_out);
                return true;

            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 50, "Failed to run. The input parameters are incorrect.");
                return false;
            }
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