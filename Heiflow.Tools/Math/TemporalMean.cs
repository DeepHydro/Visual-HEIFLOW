// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Math
{
    public class TemporalMean : ModelTool
    {
        public TemporalMean()
        {
            Multiplier = 1;
            Name = "Temporal Mean";
            Category = "Math";
            Description = "Calculate temporal mean value for each spatial cell";
            Version = "1.0.0.0";
            OutputMatrix = "Average";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The input matrix which should be [0][-1][-1]")]
        public string InputMatrix
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("The universal factor that will be multilied to every matrix element")]
        public float Multiplier
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The matrix that stores mean values")]
        public string OutputMatrix
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var m1 = Validate(InputMatrix);
            this.Initialized = m1;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
             var var_index=0;
            var mat = Get3DMat(InputMatrix,ref var_index);

            if (mat != null)
            {
                var av_mat = MyMath.TemporalMean(mat, var_index, Multiplier);
                av_mat.Name = OutputMatrix;
                av_mat.TimeBrowsable = true;
                Workspace.Add(av_mat);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
