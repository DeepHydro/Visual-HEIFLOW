// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
                var mat_out = new My3DMat<float>(1, 1, ncell);
                mat_out.Name = OutputMatrix;
                mat_out.Variables = new string[] { "CorrelationCoefficients"};
                for (int c = 0; c < ncell; c++)
                {  
                    var vecA = matA.GetSeriesAt(var_indexA, c);
                    var dou_vecA = MyMath.ToDouble(vecA);
                    var vecB = matB.GetSeriesAt(var_indexB, c);
                    var dou_vecB = MyMath.ToDouble(vecB);
                    var len=System.Math.Min(vecA.Length,vecB.Length);
                 //   var cor = MyStatisticsMath.Correlation(dou_vecA, dou_vecB);
                    var cor = Heiflow.Core.Alglib.alglib.basestat.pearsoncorrelation(dou_vecA, dou_vecB, len);
                    if (double.IsNaN(cor) || double.IsInfinity(cor))
                        cor = 0;
                    mat_out.Value[0][0][c] = (float) cor;
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
