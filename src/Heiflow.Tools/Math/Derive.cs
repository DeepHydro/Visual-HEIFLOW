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
    public class Derive : ModelTool
    {
        private NumericalDataType mNumericalDataType;
        private TimeUnits mTimeUnit;
        public Derive()
        {
            Name = "Derive";
            Category = "Math";
            Description = "Derive time series from existing data cube";
            Version = "1.0.0.0";
            Derived = "Derived";
            this.Author = "Yong Tian";
            mNumericalDataType = Core.NumericalDataType.Average;
            mTimeUnit = TimeUnits.Month;
        }

        [Category("Input")]
        [Description("The input matrix which should be [0][-1][-1]")]
        public string Source
        {
            get;
            set;
        }

        [Category("Parameter")]
        public NumericalDataType NumericalDataType
        {
            get
            {
                return mNumericalDataType;
            }
            set
            {
                mNumericalDataType = value;
            }
        }


        [Category("Parameter")]
        public TimeUnits TimeUnits
        {
            get
            {
                return mTimeUnit;
            }
            set
            {
                mTimeUnit = value;
            }
        }


        [Category("Output")]
        [Description("The derived matrix name")]
        public string Derived
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
            double prg = 0;
            int count = 1;
            if (mat != null)
            {
                int nstep = mat.Size[1];
                int ncell = mat.Size[2];
                var vec = mat[var_index, ":", "0"];
                var dou_vec = MatrixOperation.ToDouble(vec);

                var date_source = new DateTime[nstep];
                if (mat.DateTimes != null && mat.DateTimes.Length >= nstep)
                {
                    for (int i = 0; i < nstep; i++)
                    {
                        date_source[i] = mat.DateTimes[i];
                    }
                }
                else
                {
                    for (int i = 0; i < nstep; i++)
                    {
                        date_source[i] = ModelService.Start.AddDays(i);
                    }
                }
                var ts = new DataCube<float>(vec, date_source);
                var derieved_ts = TimeSeriesAnalyzer.Derieve(ts, NumericalDataType, TimeUnits);
                var derieved_steps = derieved_ts.DateTimes.Length;
                var mat_out = new DataCube<float>(1, derieved_steps, ncell, false);

                mat_out.Name = Derived;
                mat_out.Variables = new string[] { "Derived" };
                mat_out.DateTimes = derieved_ts.DateTimes.ToArray();
                for (int c = 0; c < ncell; c++)
                {
                    vec = mat[var_index, ":", c.ToString()];
                    ts = new DataCube<float>(vec, date_source);
                    derieved_ts = TimeSeriesAnalyzer.Derieve(ts, NumericalDataType, TimeUnits);
                    for (int t = 0; t < derieved_steps; t++)
                    {
                        mat_out[0, t, c] = derieved_ts[0,t,0];
                    }
                    prg = (c + 1) * 100.0 / ncell;
                    if (prg > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", (int)prg, "Caculating Cell: " + (c + 1));
                        count++;
                    }
                }

                Workspace.Add(mat_out);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
