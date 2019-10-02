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
            Description = "Multiply the given Data Cube by a uniform factor";
            Version = "1.0.0.0";
            OutputDataCube = "Scaled";
            Scale = 1.0f;
            this.Author = "Yong Tian";
        }



        [Category("Input")]
        [Description("The input Data Cube which should be mat[:][:][:]")]
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
        public string OutputDataCube
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
                var tag = new DataCube<float>(source.Size[0], source.Size[1], source.Size[2], false);
                for (int i = 0; i < source.Size[0];i++ )
                {
                    if(source[i] != null)
                    {
                        tag[i][":", ":"] = source[i][":", ":"] * Scale;
                    }
                }
                tag.Name = OutputDataCube;
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
