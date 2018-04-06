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
