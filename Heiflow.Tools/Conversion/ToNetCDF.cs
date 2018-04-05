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