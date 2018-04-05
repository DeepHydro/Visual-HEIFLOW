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

namespace Heiflow.Tools.Conversion
{
    public class ToNetCDF : ModelTool
    {
        private string _OutputFileName;
        private NetCDFDataSet nc_out;
        public ToNetCDF()
        {
            Name = "Save As NetCDF File";
            Category = "Conversion";
            Description = "Save data cube as NetCDF file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            VariableName = "unknown";
            MultiThreadRequired = false;
        }

        [Category("Input")]
        [Description("The name of the input matrix")]
        public string Source { get; set; }


        [Category("Output")]
        [Description("The output filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get
            {
                return _OutputFileName;
            }
            set
            {
                _OutputFileName = value;
            }
        }
        [Category("Output")]
        [Description("The variable name")]
        public string VariableName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            //this.Initialized = Validate(Source);
            //if (ProjectService == null)
            //    this.Initialized = false;

            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {

            //nc_out = new NetCDFDataSet("e:\\test.nc", Microsoft.Research.Science.Data.ResourceOpenMode.Create);
            //float[] xx = new float[10];
            //float[] yy = new float[10];
            //float[] dates = new float[2];
            //var var_name = "test";
            //var mat1 = new float[2, 10, 10];

            //nc_out.AddVariable(typeof(float), "longitude", xx, new string[] { "longitude", });
            //nc_out.AddVariable(typeof(float), "latitude", yy, new string[] { "latitude" });
            //var dt = nc_out.AddVariable(typeof(float), "time", dates, new string[] { "time" });
            //var test = nc_out.AddVariable(typeof(float), var_name, mat1, new string[] { "time", "latitude", "longitude" });
            //nc_out.Commit();
            //var newmat = new float[1, 10, 10];

            //dt.Append(new float[1]);
            //test.Append(newmat);
            //nc_out.Commit();
            //    return true;

            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            int progress = 0;
            int nsteps = mat.Size[1];

            var grid = ProjectService.Project.Model.Grid as RegularGrid;
            var lonlat = grid.GetLonLatAxis();

            var times = new float[nsteps];
            if (mat.DateTimes != null)
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = mat.DateTimes[t].ToFileTime();
                }
            }
            else
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = DateTime.Now.AddDays(t).ToFileTime();
                }
            }
            var mat_step = grid.To3DMatrix<float>(mat.Value[var_index][0], 0);

            nc_out = new NetCDFDataSet("e:\\test.nc");
         //   nc_out = new NetCDFDataSet(OutputFileName);
            nc_out.AddVariable(typeof(float), "longitude", lonlat[0], new string[] { "longitude", });
            nc_out.AddVariable(typeof(float), "latitude", lonlat[1], new string[] { "latitude" });
            var nc_dt = nc_out.AddVariable(typeof(float), "time", new float[] { times[0] }, new string[] { "time" });
            var nc_var = nc_out.AddVariable(typeof(float), VariableName, mat_step, new string[] { "time", "latitude", "longitude" });
            nc_out.Commit();
            for (int t = 1; t < nsteps; t++)
            {
                mat_step = grid.To3DMatrix<float>(mat.Value[var_index][0], t);
                nc_var.Append(mat_step);
                nc_dt.Append(new float[] { times[t] });
                nc_out.Commit();

                progress = t * 100 / nsteps;
                cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + t);
            }
            return true;
        }
    }
}