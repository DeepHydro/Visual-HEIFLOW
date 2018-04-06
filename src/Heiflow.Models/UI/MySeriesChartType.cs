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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.UI
{
    public  enum MySeriesChartType
    {
        // 摘要: 
        //     点图类型。
        Point = 0,
        //
        // 摘要: 
        //     快速点图类型。
        FastPoint = 1,
        //
        // 摘要: 
        //     气泡图类型。
        Bubble = 2,
        //
        // 摘要: 
        //     折线图类型。
        Line = 3,
        //
        // 摘要: 
        //     样条图类型。
        Spline = 4,
        //
        // 摘要: 
        //     阶梯线图类型。
        StepLine = 5,
        //
        // 摘要: 
        //     快速扫描线图类型。
        FastLine = 6,
        //
        // 摘要: 
        //     条形图类型。
        Bar = 7,
        //
        // 摘要: 
        //     堆积条形图类型。
        StackedBar = 8,
        //
        // 摘要: 
        //     百分比堆积条形图类型。
        StackedBar100 = 9,
        //
        // 摘要: 
        //     柱形图类型。
        Column = 10,
        //
        // 摘要: 
        //     堆积柱形图类型。
        StackedColumn = 11,
        //
        // 摘要: 
        //     百分比堆积柱形图类型。
        StackedColumn100 = 12,
        //
        // 摘要: 
        //     面积图类型。
        Area = 13,
        //
        // 摘要: 
        //     样条面积图类型。
        SplineArea = 14,
        //
        // 摘要: 
        //     堆积面积图类型。
        StackedArea = 15,
        //
        // 摘要: 
        //     百分比堆积面积图类型。
        StackedArea100 = 16,
        //
        // 摘要: 
        //     饼图类型。
        Pie = 17,
        //
        // 摘要: 
        //     圆环图类型。
        Doughnut = 18,
        //
        // 摘要: 
        //     股价图类型。
        Stock = 19,
        //
        // 摘要: 
        //     K 线图类型。
        Candlestick = 20,
        //
        // 摘要: 
        //     范围图类型。
        Range = 21,
        //
        // 摘要: 
        //     样条范围图类型。
        SplineRange = 22,
        //
        // 摘要: 
        //     范围条形图类型。
        RangeBar = 23,
        //
        // 摘要: 
        //     范围柱形图类型。
        RangeColumn = 24,
        //
        // 摘要: 
        //     雷达图类型。
        Radar = 25,
        //
        // 摘要: 
        //     极坐标图类型。
        Polar = 26,
        //
        // 摘要: 
        //     误差条形图类型。
        ErrorBar = 27,
        //
        // 摘要: 
        //     盒须图类型。
        BoxPlot = 28,
        //
        // 摘要: 
        //     砖形图类型。
        Renko = 29,
        //
        // 摘要: 
        //     新三值图类型。
        ThreeLineBreak = 30,
        //
        // 摘要: 
        //     卡吉图类型。
        Kagi = 31,
        //
        // 摘要: 
        //     点数图类型。
        PointAndFigure = 32,
        //
        // 摘要: 
        //     漏斗图类型。
        Funnel = 33,
        //
        // 摘要: 
        //     棱锥图类型。
        Pyramid = 34,
    }
}
