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
using Heiflow.Core.MyMath;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Statisitcs
{
    public class Trend : ModelTool
    {
        public Trend()
        {
            Name = "Trend";
            Category = "Tempo-Spatial Analysis";
            Description = "Calculating  trend at each cell";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            OutputDataCube = "trend";
        }

        [Category("Input")]
        [Description("The name of  matrix. The matrix name should be written as mat[0][:][:]")]
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
            int var_indexA = 0;
            var matA = Get3DMat(InputDataCube, ref var_indexA);
            double prg = 0;
            int count = 1;
            if (matA != null)
            {
                int nstep = matA.Size[1];
                int ncell = matA.Size[2];
             
                var mat_out = new DataCube<float>(1, 1, ncell);
                mat_out.Name = OutputDataCube;
                mat_out.Variables = new string[] { "Slope" };

                for (int c = 0; c < ncell; c++)
                {
                    var vec = matA.GetVector(var_indexA, ":", c.ToString());
                    var dou_vec = MatrixOperation.ToDouble(vec);
                    var steps = new double[nstep];
                    double rs, slope, yint;
                    for (int t = 1; t < nstep; t++)
                    {
                        steps[t] = t + 1;
                    }
                     MyStatisticsMath.LinearRegression(steps, dou_vec, 0, nstep, out rs, out yint, out slope);
                    mat_out[0, 0, c] = (float)slope;
                    prg = (c + 1) * 100.0 / ncell;
                    if (prg > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", (int)prg, "Caculating Cell: " + (c + 1));
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
