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
    public class Trend : ModelTool
    {
        public Trend()
        {
            Name = "Trend";
            Category = "Tempo-Spatial Analysis";
            Description = "Calculating  trend at each cell";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            OutputMatrix = "Trend matrix";
        }

        [Category("Input")]
        [Description("The name of  matrix. The matrix name should be written as A[var_index][-1][-1]")]
        public string Matrix { get; set; }

        [Category("Output")]
        [Description("The name of  output matrix")]
        public string OutputMatrix { get; set; }

        public override void Initialize()
        {
            var mat = Get3DMat(Matrix);
            Initialized = mat != null;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int var_indexA = 0;
            var matA = Get3DMat(Matrix, ref var_indexA);
            double prg = 0;

            if (matA != null)
            {
                int nstep = matA.Size[1];
                int ncell = matA.Size[2];
             
                var mat_out = new My3DMat<float>(1, 1, ncell);
                mat_out.Name = OutputMatrix;
                mat_out.Variables = new string[] { "Slope" };

                for (int c = 0; c < ncell; c++)
                {
                    var vec = matA.GetVector(var_indexA, MyMath.full, c);
                    var dou_vec = MyMath.ToDouble(vec);
                    var steps = new double[nstep];
                    double rs, slope, yint;
                    for (int t = 1; t < nstep; t++)
                    {
                        steps[t] = t + 1;
                    }
                     MyStatisticsMath.LinearRegression(steps, dou_vec, 0, nstep, out rs, out yint, out slope);
                    mat_out[0, 0, c] = (float)slope;
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
