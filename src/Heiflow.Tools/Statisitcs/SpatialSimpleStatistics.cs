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

using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Statisitcs
{
    public class SpatialSimpleStatistics : ModelTool
    {
        public SpatialSimpleStatistics()
        {
            Name = "Spatial Simple Statistics";
            Category = "Statistics";
            Description = "  Calculate the temporal statistics at each spatial cell: mean, variance, skewness, kurtosis.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            OutputDataCube = "SpatialStat";
        }

        [Category("Input")]
        [Description("The input matrix being analyzed. The Data Cube style shoud be mat[0][:][:]")]
        public string InputDataCube { get; set; }

        [Category("Output")]
        [Description("The name of  output DataCube")]
        public string OutputDataCube { get; set; }

        public override void Initialize()
        {
            var mat = Get3DMat(InputDataCube);
            Initialized = mat != null;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int var_index = 0;
            var mat = Get3DMat(InputDataCube, ref var_index);
            int prg = 0;
            int count = 1;
            if (mat != null)
            {
                int nstep = mat.Size[1];
                int ncell = mat.Size[2];
                var mat_out = new DataCube<float>(4, 1, ncell);
                mat_out.Name = OutputDataCube;
                mat_out.Variables = new string[] { "Mean", "Variance", "Skewness", "kurtosis" };
                for (int c = 0; c < ncell; c++)
                {
                    double mean = 0, variance = 0, skewness = 0, kurtosis = 0;
                    var vec = mat.GetVector(var_index, ":", c.ToString());
                    var dou_vec = MatrixOperation.ToDouble(vec);
                    Heiflow.Core.Alglib.alglib.basestat.samplemoments(dou_vec, vec.Length, ref mean, ref variance, ref skewness, ref kurtosis);
                    mat_out[0, 0, c] =(float) mean;
                    mat_out[1, 0, c] = (float)variance;
                    mat_out[2, 0, c] = (float)skewness;
                    mat_out[3, 0, c] = (float)kurtosis;
                    prg = (c + 1) * 100 / ncell;
                    if (prg > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", prg, "Caculating Cell: " + (c + 1));
                        count++;
                    }
                }
                Workspace.Add(mat_out);
                return true;
            }
            else
            {

                return false;
            }
        }
    }
}
