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
using System.Linq;
namespace  Heiflow.AI.GeneticProgramming
{
    /// <summary>
    /// This class provides training and testing data for GP.
    /// </summary>
    [Serializable]
    public class GPTerminalSet
    {   //Structure of Terminal set: first independent variables x, than random constant R and last index position is Output Y
        //x1,x2, ... , xn, R1,R2, ... ,Rn, Y - one row in dataarray
        public double[][] TrainingData { get; set; }
        public double[][] TestingData { get; set; }

        public int NumVariables { get; set; }
        public int NumConstants { get; set; }
        public double AverageValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public bool IsTimeSeries { get; set; }
        public int RowCount { get; set; }

        public GPTerminalSet()
        {
            TrainingData = null;
            TestingData = null;
            IsTimeSeries = false;
        }

        //Calculate statistic data for TerminalSet.
        public void CalculateStat()
        {
            if (TrainingData == null)
                throw new Exception("Terminal set is empty!");
            if (NumVariables == 0)
                throw new Exception("The number of variables is 0!");

            int yindex = TrainingData[0].Length - NumConstants + NumConstants - 1;
            RowCount = (short)TrainingData.Length;

            var stat = from p1 in Enumerable.Range(0, RowCount)
                       from p2 in TrainingData
                       select p2[yindex];

            double maxValue = stat.Max();
            double minValue = stat.Min();
            double averageValue = stat.Average();
        }
    }
}
