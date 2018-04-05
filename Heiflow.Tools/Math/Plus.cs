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
    public class Plus : ModelTool
    {
        public Plus()
        {
            Name = "Plus";
            Category = "Math";
            Description = "Plus two vectors";
            OutputMatrix = "plused";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The matrix that is to be subtracted. The matrix style should be a vector like mat[0][0][:]")]
        public string MatrixA
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The subtraction matirx. The matrix style should be a vector like mat[0][0][:]")]
        public string MatrixB
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("The output matrix")]
        public string OutputMatrix
        {
            get;
            set;
        }
        public override void Initialize()
        {
            var m1 = Validate(MatrixA);
            var m2 = Validate(MatrixB);
            this.Initialized = m1 && m2;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vec_minu = GetVector(MatrixA);
            var vec_subt= GetVector(MatrixB);
            if (vec_minu != null && vec_subt != null)
            {
               var vec = MyMath.Plus(vec_minu, vec_subt);
               vec.Name = OutputMatrix;
               Workspace.Add(vec);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
