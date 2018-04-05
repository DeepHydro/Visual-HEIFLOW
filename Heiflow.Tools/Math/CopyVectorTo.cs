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
    public class CopyVectorTo : ModelTool
    {
        public CopyVectorTo()
        {
            Name = "Copy Vector To";
            Category = "Math";
            Description = "Copy vector to the specified dimension of a data cube";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The source matrix. The matrix style should be [0][0][:] ")]
        public string SourceMatrix
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The target matrix. The matrix style should be [0][0][:]")]
        public string TargetMatrix
        {
            get;
            set;
        }
        
        public override void Initialize()
        {
            var m1 = Validate(SourceMatrix);
            var m2 = Validate(TargetMatrix);
            this.Initialized = m1 && m2;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vec_src = GetVector(SourceMatrix);
            var vec_tar = GetVector(TargetMatrix);
            var mat_tar = Get3DMat(TargetMatrix);
            if (vec_src != null && vec_tar != null && vec_src.Length == vec_tar.Length)
            {
               var dims =  GetDims(TargetMatrix);
               mat_tar.SetBy(vec_src, dims[0], dims[1], dims[2]);
               cancelProgressHandler.Progress("Package_Tool", 100, "Successful");
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed. The source or target matrix style is wrong.");
                return false;
            }
        }
    }
}
