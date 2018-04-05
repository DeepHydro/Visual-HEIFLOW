// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
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
    public class Multiply : ModelTool
    {

        public Multiply()
        {
            Name = "Multiply";
            Category = "Math";
            Description = "Multiply the given matrix by a uniform factor";
            Version = "1.0.0.0";
            OutputMatrix = "Scaled";
            Scale = 1.0f;
            this.Author = "Yong Tian";
        }



        [Category("Input")]
        [Description("The input matrix which should be [-1][-1][-1]")]
        public string Source
        {
            get;
            set;
        }


        [Category("Input")]
        [Description("The scale factor")]
        public float Scale
        {
            get;
            set;
        }
 

        [Category("Output")]
        [Description("The multiplied matrix name")]
        public string OutputMatrix
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var m1 = Validate(Source);
            this.Initialized = m1;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var source = Get3DMat(Source);
            
            if (source != null)
            {
                var tag = MyMath.Scale(source, Scale);
                tag.Name = OutputMatrix;
                Workspace.Add(tag);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
