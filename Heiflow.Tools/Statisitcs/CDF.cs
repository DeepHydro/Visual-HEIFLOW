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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using Heiflow.Core.Data;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Statisitcs
{
    public class CDFPlot : ModelTool
    {
        public CDFPlot()
        {
            Name = "Cumulative Distribution Functions";
            Category = "Statistics";
            Description = "Calculate and plot CDF";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The input matrix. The matrix style should be [-1][0][0] or [0][-1][0] or [0][0][:]")]
        public string Matrix
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = Validate(Matrix);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vec = GetVector(Matrix);
            if (vec != null)
            {
                var dou_vec = MyMath.ToDouble(vec);
                string vec_name = "CDF of " + GetName(Matrix);
                int nlen = dou_vec.Length;
                cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
                Array.Sort<double>(dou_vec);
                var cdf = MathNet.Numerics.Statistics.Statistics.EmpiricalCDFFunc(dou_vec);
                var cdf_value = new double[nlen];
                for (int i = 0; i < nlen; i++)
                {
                    cdf_value[i] = cdf(dou_vec[i]);
                }
                WorkspaceView.Plot<double>(dou_vec, cdf_value,  vec_name, Models.UI.MySeriesChartType.FastLine);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
