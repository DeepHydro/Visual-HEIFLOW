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

using Heiflow.Models.AI;
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Heiflow.Numerics.Local;

namespace Heiflow.Core.DataDriven
{
    /// <summary>
    /// Class providing access to Multiple Linear Regression model
    /// </summary>
    public class MLRModel:ForecastingModel
    {
        public MLRModel(ModelParameter para)
            : base(para)
        {
            para.Component = this;
            Orgnization = "HUST WREIS";
            ID = "8886B813-CD78-4A4B-9364-59792B35F88C";
            Name = "Multiple Linear Regression";
            Descriptions = "";
        }

       // private double[] mRegressionCoefficients;

        public double[] RegressionCoefficients
        {
            get;
            private set;
        }

        #region IForecastingModel 成员

        public override void Train(IForecastingDataSets datasets)
        {
            OnStartRunning(new ComponentRunEventArgs(datasets));
            ForecastingDataSets fds = datasets as ForecastingDataSets;
            //MultipleLinearRegression mlineRegrsn = new MultipleLinearRegression(datasets.InputData, fds.GetOutputDataColumn(0), true);
            //Matrix result = new Matrix();
            //mlineRegrsn.ComputeFactorCoref(result);
            //RegressionCoefficients = result[0, Matrix.mCol];
            int solutionSize = datasets.Length;
            datasets.ForecastedData = new double[solutionSize][];
            for (int i = 0; i < solutionSize; i++)
            {
                datasets.ForecastedData[i] = new double[1];
                datasets.ForecastedData[i][0] = Forecast(fds.InputData[i]);
            }
            OnFinishRunning(new ComponentRunEventArgs(datasets));
        }

        public override double Forecast(double[] inputVector)
        {
            double y=0;
            if (inputVector.Length != RegressionCoefficients.Length)
                throw new Exception("Invalid input vetor");
            for (int i = 0; i < inputVector.Length; i++)
            {
                y += inputVector[i] * RegressionCoefficients[i];
            }
            return y;
        }

        #endregion
    }
}
