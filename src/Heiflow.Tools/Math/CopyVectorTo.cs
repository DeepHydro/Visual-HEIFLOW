﻿//
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
        [Description("The source Data Cube. The Data Cube style should be mat[0][0][:] ")]
        public string SourceDataCube
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The target Data Cube. The Data Cube style should be mat[0][0][:]")]
        public string TargetDataCube
        {
            get;
            set;
        }
        
        public override void Initialize()
        {
            var m1 = Validate(SourceDataCube);
            var m2 = Validate(TargetDataCube);
            this.Initialized = m1 && m2;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vec_src = GetVector(SourceDataCube);
            var mat_tar = Get3DMat(TargetDataCube);
            if (vec_src != null)
            {
               var dims =  GetDims(TargetDataCube);
               mat_tar.ILArrays[int.Parse(dims[0])][dims[1], dims[2]]= vec_src;
               if (mat_tar.DataOwner is IParameter)
                   (mat_tar.DataOwner as IParameter).UpdateFromFloatDataCube();
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
