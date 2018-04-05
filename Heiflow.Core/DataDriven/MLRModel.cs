// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
