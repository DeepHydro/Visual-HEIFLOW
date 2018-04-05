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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Plot
{
    public class Scatter : ModelTool
    {
        public Scatter()
        {
            Name = "Scatter";
            Category = "Plot";
            Description = "Plot scatter";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            SeriesTitle = "Scatter Plot";
        }
        [Category("Input")]
        [Description("The title of the plotted ")]
        public string SeriesTitle
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The X matrix. The matrix style should be [-1][0][0] or [0][-1][0] or [0][0][:]")]
        public string MatrixX
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The Y matrix. The matrix style should be [-1][0][0] or [0][-1][0] or [0][0][:]")]
        public string MatrixY
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var mx = Validate(MatrixX);
            var my = Validate(MatrixY);
            this.Initialized = mx && my;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vecX = GetVector(MatrixX);
            var vecY = GetVector(MatrixY);
            if (vecX != null && vecY != null)
            {
                WorkspaceView.Plot<float>(vecX, vecY,  SeriesTitle, Models.UI.MySeriesChartType.Point);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
