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
    public class Minus : ModelTool
    {
        public Minus()
        {
            Name = "Minus";
            Category = "Math";
            Description = "Perform Minus operation on given data cubes";
            OutputDataCube = "Difference";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The data cube that is to be subtracted. The data cube style should be mat[0][0][:]")]
        public string MinuendDataCube
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The subtraction data cube. The data cube should be mat[0][0][:]")]
        public string SubtrahendDataCube
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("The difference data cube")]
        public string OutputDataCube
        {
            get;
            set;
        }
        public override void Initialize()
        {
            var m1 = Validate(MinuendDataCube);
            var m2 = Validate(SubtrahendDataCube);
            this.Initialized = m1 && m2;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vec_minu = GetVector(MinuendDataCube);
            var vec_subt = GetVector(SubtrahendDataCube);
            if (vec_minu != null && vec_subt != null && vec_minu.Length == vec_subt.Length)
            {
                var vec = new DataCube<float>(1, 1, vec_minu.Length);
                for (int i = 0; i < vec_minu.Length; i++)
                {
                    vec[0, 0, i] = vec_minu[i] - vec_subt[i];
                }
                vec.Name = OutputDataCube;
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
