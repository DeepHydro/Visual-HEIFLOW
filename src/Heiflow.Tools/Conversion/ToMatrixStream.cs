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
using Heiflow.Core.MyMath;

namespace Heiflow.Tools.Conversion
{
    public class ToMatrixStream : ModelTool
    {
        private string _OutputFileName;
        public enum CS { PCS, GCS };
        public ToMatrixStream()
        {
            Name = "To Matrix Cube";
            Category = "Conversion";
            Description = "Save data cube as Matrix Cube file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            CoornidateSystem = CS.GCS;
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

        public CS CoornidateSystem
        {
            get;
            set;
        }


        public override void Initialize()
        {
            this.Initialized = Validate(Source);
            if (ProjectService == null)
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            FileStream fs = new FileStream(_OutputFileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            int progress = 0;
            int nsteps = mat.Size[1];
            var grid = ProjectService.Project.Model.Grid as RegularGrid;
            if (grid != null)
            {
                float[][] lonlat = null;
                if (CoornidateSystem == CS.GCS)
                    lonlat = grid.GetLonLatAxis();
                else if (CoornidateSystem == CS.PCS)
                    lonlat = grid.GetPCSAxis();
                var nrow = grid.RowCount;
                var ncol = grid.ColumnCount;
                var times = new float[nsteps];
                if (mat.DateTimes != null)
                {
                    for (int t = 0; t < nsteps; t++)
                    {
                        times[t] = (float)mat.DateTimes[t].ToOADate();
                    }
                }
                else
                {
                    for (int t = 0; t < nsteps; t++)
                    {
                        times[t] = (float)DateTime.Now.AddDays(t).ToOADate();
                    }
                }

                bw.Write(nrow);
                bw.Write(ncol);
                bw.Write(nsteps);

                for (int i = 0; i < ncol; i++)
                {
                    bw.Write(lonlat[0][i]);
                }
                for (int i = 0; i < nrow; i++)
                {
                    bw.Write(lonlat[1][i]);
                }

                for (int t = 0; t < nsteps; t++)
                {
                    bw.Write(times[t]);
                    var vec = mat[var_index,t.ToString(),":"];
                    bw.Write(vec.Max());
                    bw.Write(vec.Min());
                    bw.Write(vec.Average());
                    bw.Write(MyStatisticsMath.StandardDeviation(vec));
                    var mat_step = grid.ToMatrix<float>(vec, 0);
                    for (int r = 0; r < nrow; r++)
                    {
                        for (int c = 0; c < ncol; c++)
                        {
                            bw.Write(mat_step[r][c]);
                        }
                    }
                    progress = t * 100 / nsteps;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + t);
                }

                fs.Close();
                bw.Close();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}