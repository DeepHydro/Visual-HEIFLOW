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
            OutputDataCube = "Average";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The input DataCube should be mat[0][:][:]")]
        public string InputDataCube
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
        public string OutputDataCube
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var m1 = Validate(InputDataCube);
            this.Initialized = m1;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var mat = Get3DMat(InputDataCube);
            var dims = GetDims(InputDataCube);
            if (mat != null)
            {
                int nvar = mat.Size[0];
                int ncell = mat.Size[2];
                DataCube<float> mean_mat = null;
                if (dims[0] == ":")
                {
                    mean_mat = new DataCube<float>(nvar, 1, ncell, false);
                    for (int i = 0; i < nvar; i++)
                    {
                        if (mat.ILArrays[i] != null)
                        {
                            for (int j = 0; j < ncell; j++)
                            {
                                var vec = mat.GetVector(i, ":", j.ToString());
                                var av = vec.Average();
                                mean_mat[i, 0, j] = av * Multiplier;
                            }
                        }
                    }
                }
                else
                {
                    mean_mat = new DataCube<float>(1, 1, ncell, false);
                    int var_index = int.Parse(dims[0]);
                    if (mat.ILArrays[var_index] != null)
                    {
                        for (int j = 0; j < ncell; j++)
                        {
                            var vec = mat.GetVector(var_index, ":", j.ToString());
                            var av = vec.Average();
                            mean_mat[0, 0, j] = av * Multiplier;
                        }
                    }
                }
                mean_mat.Name = OutputDataCube;
                Workspace.Add(mean_mat);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}