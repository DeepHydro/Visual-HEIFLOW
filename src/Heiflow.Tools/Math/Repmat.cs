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
    public enum DataCubeDimension { Time,Space}
    public class Repmat : ModelTool
    {
        public Repmat()
        {
            Name = "Repeat Data Cube";
            Category = "Math";
            Description = "Repeat a specified dimension of a data cube";
            Version = "1.0.0.0";
            RepeatedDimension = DataCubeDimension.Space;
            OutDataCubeName = "rep";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The source Data Cube. The Data Cube style should be mat[0][:][0]")]
        public string SourceDataCube
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The dimension to be repeated")]
        public DataCubeDimension RepeatedDimension
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The times to be repeated")]
        public int RepeatedTimes
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The name of repeated Data Cube.")]
        public string OutDataCubeName
        {
            get;
            set;
        }
        
        public override void Initialize()
        {
            var m1 = Validate(SourceDataCube);
            var m2 = Validate(OutDataCubeName);
            this.Initialized = m1 && m2;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var src_mat = Get3DMat(SourceDataCube);
            var vec_src = GetVector(SourceDataCube);
            var dims_src = src_mat.Size;
            if (vec_src != null && RepeatedTimes > 0)
            {
                int prg = 0;
                if (RepeatedDimension == DataCubeDimension.Space)
                {
                    var mat_tar = new DataCube<float>(1, dims_src[1], RepeatedTimes);
                    for (int i = 0; i < mat_tar.Size[1]; i++)
                    {
                        var vec = new float[vec_src.Length];
                        vec_src.CopyTo(vec, 0);
                        mat_tar.ILArrays[0][i, ":"] = vec;
                        prg = (i + 1) / mat_tar.Size[1];
                        cancelProgressHandler.Progress("Package_Tool", prg, "Processing cell: " + (i + 1));
                    }
                }
                if (RepeatedDimension == DataCubeDimension.Time)
                {
                    var mat_tar = new DataCube<float>(1, RepeatedTimes, dims_src[2]);
                    for (int i = 0; i < mat_tar.Size[2]; i++)
                    {
                        var vec = new float[vec_src.Length];
                        vec_src.CopyTo(vec, 0);
                        mat_tar.ILArrays[0][":", i] = vec;
                        prg = (i + 1) / mat_tar.Size[2];
                        cancelProgressHandler.Progress("Package_Tool", prg, "Processing time: " + (i + 1));
                    }
                }
              
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
