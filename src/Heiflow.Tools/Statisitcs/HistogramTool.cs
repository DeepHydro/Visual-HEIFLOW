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

namespace Heiflow.Tools.Statisitcs
{
    public class HistogramTool : ModelTool
    {
        public HistogramTool()
        {
            Name = "Histogram";
            Category = "Statistics";
            Description = "Calculate and histogram";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            Number = 10;
        }

        [Category("Input")]
        [Description("The input matrix")]
        public string Matrix
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("The number of histogram")]
        public int Number
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
            var var_name = GetName(Matrix);
            if (vec != null)
            {
                if (Number <= 0)
                    Number = 10;
                if (Number >= 100)
                    Number = 100;
                var vec_name = "Histogram of " + GetName(Matrix);
                cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
                var dou_vec = MatrixOperation.ToDouble(vec);
                var hist = new Histogram(dou_vec, Number);
                int nhist = hist.BucketCount;
                int[] xx = new int[nhist];
                int[] yy = new int[nhist];
                for (int i = 0; i < nhist; i++)
                {
                    xx[i] = (int)((hist[i].LowerBound + hist[i].UpperBound) * 0.5);
                    yy[i] = (int)hist[i].Count;
                }
                WorkspaceView.Plot<int>(xx, yy,  vec_name, Models.UI.MySeriesChartType.Column);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
