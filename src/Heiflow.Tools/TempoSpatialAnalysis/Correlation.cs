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
    public class Correlation : ModelTool
    {
        public Correlation()
        {
            Name = "Correlation";
            Category = "Tempo-Spatial Analysis";
            Description = "Calculating correlation coefficients between two given variables at each grid cell";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            OutputMatrix = "Correlation coefficients matrix";
        }

        [Category("Input")]
        [Description("The name of  matrix A. The matrix name should be written as A[var_index][-1][-1]")]
        public string MatrixA { get; set; }

        [Category("Input")]
        [Description("The name of  matrix B. The matrix name should be written as B[var_index][-1][-1]")]
        public string MatrixB { get; set; }

        [Category("Output")]
        [Description("The name of  output matrix")]
        public string OutputMatrix { get; set; }

        public override void Initialize()
        {
            var mat = Get3DMat(MatrixA);
            Initialized = mat != null;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int var_indexA = 0;
            int var_indexB = 0;
            var matA = Get3DMat(MatrixA, ref var_indexA);
            var matB = Get3DMat(MatrixB, ref var_indexB);
            double prg = 0;
           
            if (matA != null && matB != null)
            {
                int nstep = System.Math.Min(matA.Size[1], matB.Size[1]);
                int ncell = matA.Size[2];
                var mat_out = new DataCube<float>(1, 1, ncell, false);
                mat_out.Name = OutputMatrix;
                mat_out.Variables = new string[] { "CorrelationCoefficients"};
                for (int c = 0; c < ncell; c++)
                {  
                    var vecA = matA.GetVector(var_indexA, ":", c.ToString());
                    var dou_vecA = MatrixOperation.ToDouble(vecA);
                    var vecB = matB.GetVector(var_indexB, ":", c.ToString()); ;
                    var dou_vecB = MatrixOperation.ToDouble(vecB);
                    var len=System.Math.Min(vecA.Length,vecB.Length);
                 //   var cor = MyStatisticsMath.Correlation(dou_vecA, dou_vecB);
                    var cor = Heiflow.Core.Alglib.alglib.basestat.pearsoncorrelation(dou_vecA, dou_vecB, len);
                    if (double.IsNaN(cor) || double.IsInfinity(cor))
                        cor = 0;
                    mat_out[0, 0, c] = (float)cor;
                    prg = (c + 1) * 100.0 / ncell;
                    if (prg % 10 == 0)
                        cancelProgressHandler.Progress("Package_Tool", (int)prg, "Caculating Cell: " + (c + 1));
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
