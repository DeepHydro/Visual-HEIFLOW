using Heiflow.Core.Data;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Math
{
    public class Subset : ModelTool
    {
 
        public Subset()
        {
            Name = "Subset";
            Category = "Math";
            Description = "Derive time series from existing data cube";
            Version = "1.0.0.0";
            Output = "subset";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The input matrix which should be [0][-1][-1]")]
        public string Source
        {
            get;
            set;
        }


        [Category("Output")]
        [Description("The output data cube name")]
        public string Output
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
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            var dims = GetDims(Source);
            if (mat != null)
            {
                var buf = mat.ILArrays[var_index];
                var array = buf[dims[1],dims[2]];
                var size = array.Size.ToIntArray();
                DataCube<float> dc = new DataCube<float>(1, 1, 1, true);
                dc[0] = array;
                dc.Name = Output;
                Workspace.Add(dc);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
