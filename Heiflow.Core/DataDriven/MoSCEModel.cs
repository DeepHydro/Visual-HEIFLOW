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
using System.Runtime.InteropServices;
using System.ComponentModel;
using Heiflow.Models.AI;
using Heiflow.Core.Schema;

namespace Heiflow.Core.DataDriven
{
  
    public class MoSCEModel : Component, ICalibrationModel, IConceptualModel
    {
        //[DllImport(".\\model\\MOSCEM.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern void MoSceFunc(int nOptPar, int nOptObj, int nSamples, int nComplex, int nMaxIteration, int MAXTSTEP
        //    , double[] InputData, double[] ValData, int objfun,
        //    [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8)] double[] praVector,
        //    [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8)] double[] objVector,
        //    [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8)] double[] simulateVector);

        [DllImport(".\\model\\MOSCEM.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void MoSceFunc(int nOptPar, int nOptObj, int nSamples, int nComplex, int nMaxIteration, int MAXTSTEP
            , double[] InputData, double[] ValData, int objfun,
          double[] praVector,
          double[] objVector,
          double[] simulateVector);

        public MoSCEModel(MoSCEModelParameter para)
            : base(para)
        {
            Orgnization = "HUST WREIS";
            ID = "2F710E58-91E8-4CF4-A721-A7FA243D0D69";
            Name = "Multi-Object Shuffled Complex Evolution (MOSCE)";
            Descriptions = "";
            mSCEParameter = para;     
        }

        private MoSCEModelParameter mSCEParameter;

        private double[] mInputDataVector;
        private double[] mDesiredOutputDataVector;
        private double[][] mPrameterMatrix;
        private double[][] mObjMatrix;
        private double[][] mSlnMatrix;
        private double[][] mSimlateMatrix;

        [Browsable(false)]
        public double[][] PrameterMatrix
        {
            get
            {
                return mPrameterMatrix;
            }
        }

        [Browsable(false)]
        public double[][] ObjectiveMatrix
        {
            get
            {
                return mObjMatrix;
            }
        }

        [Browsable(false)]
        public double[][] SolutionMatrix
        {
            get
            {
                return mSlnMatrix;
            }
        }

        [Browsable(false)]
        public double[][] SimulatedMatrix
        {
            get
            {
                return mSimlateMatrix;
            }
        }


        #region ICalibrationModel 成员

        public void Calibrate(ICalibratonDataSets datasets)
        {
            OnStartRunning(new ComponentRunEventArgs());
            mSCEParameter.SimulatedLength = datasets.Length;

            //mInputDataVector = new double[datasets.Length * 5];
            //mDesiredOutputDataVector = new double[datasets.Length * 2];

            //for (int i = 0; i < datasets.Length; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //    {
            //        mInputDataVector[i * 5 + j] = datasets.InputData[i][j];
            //    }
            //    mDesiredOutputDataVector[i * 2 + 0] = datasets.ObservedData[i];
            //    mDesiredOutputDataVector[i * 2 + 1] = (Math.Pow(datasets.ObservedData[i] + 1.0, 0.3) - 1.0) / 0.3;
            //}
            var input = (datasets as CalibrationDatasets).ConvertToSCEInput();
            var desired = (datasets as CalibrationDatasets).ConvertToSCEDesiredOutput();

            int cvgCount = mSCEParameter.MaximumIteration / 100 - 5;
            mPrameterMatrix = new double[mSCEParameter.Samples][];
            mObjMatrix = new double[mSCEParameter.Samples][];
            mSlnMatrix = new double[cvgCount][];
            mSimlateMatrix = new double[mSCEParameter.SimulatedLength][];
            for (int i = 0; i < mSCEParameter.Samples; i++)
            {
                mPrameterMatrix[i] = new double[13];
                mObjMatrix[i] = new double[3];
            }
            for (int i = 0; i < mSCEParameter.SimulatedLength; i++)
            {
                mSimlateMatrix[i] = new double[1];
            }
            for (int i = 0; i < cvgCount; i++)
            {
                mSlnMatrix[i] = new double[13];
            }
            double[] praVector = new double[mSCEParameter.Samples * 13];
            double[] objVector = new double[mSCEParameter.Samples * 3];

            double[] simulateVector = new double[mSCEParameter.SimulatedLength * 1];
            //MoSceFunc(mSCEParameter.ParametersCount, mSCEParameter.ObjectivesCount, mSCEParameter.Samples,
            //    mSCEParameter.Complex, mSCEParameter.MaximumIteration, mSCEParameter.SimulatedLength, input,
            //    desired, (int)mSCEParameter.ErrorFunction, praVector, objVector, simulateVector);

            MoSceFunc(mSCEParameter.ParametersCount, mSCEParameter.ObjectivesCount, mSCEParameter.Samples,
              mSCEParameter.Complex, mSCEParameter.MaximumIteration, mSCEParameter.SimulatedLength, input,
              desired, 1, praVector, objVector, simulateVector);

            for (int i = 0; i < mSCEParameter.Samples; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    mPrameterMatrix[i][j] = praVector[i * 13 + j];
                }
                for (int j = 0; j < 3; j++)
                {
                    mObjMatrix[i][j] = objVector[i * 3 + j];
                }
            }
            for (int i = 0; i < mSCEParameter.SimulatedLength; i++)
            {
                mSimlateMatrix[i][0] = simulateVector[i];
            }
            OnFinishRunning(new ComponentRunEventArgs());
        }
        #endregion

        #region IConceptualModel 成员


        private void Destroy()
        {
            mInputDataVector = null;
            mDesiredOutputDataVector = null;
        }

        public double Simulate(double[] inputVector)
        {
            //TODO:
            throw new NotImplementedException();
        }
        #endregion

        public void SetInputVecotr(double [] input)
        {
              mInputDataVector = input;
        }

        public void SetDesiredOutputVecotr(double[] output)
        {
            mDesiredOutputDataVector = output;
        }
    }
}
