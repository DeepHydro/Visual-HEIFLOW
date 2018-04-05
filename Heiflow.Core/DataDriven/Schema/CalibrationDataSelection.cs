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
using System.Linq;
using System.Text;
using Heiflow.Core.Hydrology;
using Heiflow.Core.Data;

namespace Heiflow.Core.Schema
{
    public class CalibrationDatasets : ICalibratonDataSets
    {
        public CalibrationDatasets(double[] observedData, DateTime[] date)
        {
            mObservedData = observedData;
            mDate = date;
        }

        private double [] mObservedData;
        private double[] mSimulatedData;
        private DateTime[] mDate;
        
        #region ICalibratonDataSets 成员

        public double[][] InputData
        {
            get;
            set;
        }

        public double[] ObservedData
        {
            get
            {
                return mObservedData;
            }         
        }

        public double[] SimulatedData
        {
            get
            {
                return mSimulatedData;
            }
            set
            {
                mSimulatedData = value;
            }
        }

        public DateTime[] Date
        {
            get
            {
                return mDate;
            }
        }

        public int Length
        {
            get 
            {
                return ObservedData != null ? ObservedData.Length : 0;
            }
        }

        #endregion

        /// <summary>
        /// The input matrix must have two rows. The first row stores daily precipitaton vector in which each 
        /// value will be automatically dividied by four. The second row stores daily evaporation vector.
        /// </summary>
        /// <returns></returns>
        public double[] ConvertToSCEInput()
        {
            double[] result = null;
            if (InputData != null)
            {
                result= new double[ InputData[0].Length *5];

                for (int i = 0; i < InputData[0].Length; i++)
                {
                    result[i * 5] = InputData[0][i];

                    double r= InputData[1][i] / 4;
                    for (int j = 1; j < 5; j++)
                    {
                        result[i * 5 + j] = r;
                    }                
                }
            }
            return result;
        }

        public double[] ConvertToSCEDesiredOutput()
        {
            double[] result = null;
            if (mObservedData != null)
            {
                result = new double[mObservedData.Length * 2];
                for (int i = 0; i < mObservedData.Length; i++)
                {
                    result[i * 2 + 0] = mObservedData[i];
                    result[i * 2 + 1] = (Math.Pow(mObservedData[i] + 1.0, 0.3) - 1.0) / 0.3;
                }
            }
            return result;
        }
    }

    public class CalibrationDataSelection:ICalibrationDataSelection
    {
        public CalibrationDataSelection()
         {
             mOdmAdaptor = new ODMDataAdaptor(Configuration.DataBase);
         }
        ODMDataAdaptor mOdmAdaptor;

        #region ICalibrationDataSelection 成员

        public ICalibratonDataSets Select(ICalibrationSchema schema)
        {
            ICalibratonDataSets ds = null;
            if (schema.Responses.Length > 1)
            {
                throw new Exception("Only one output variable is required.");
            }
            else
            {
                IVariable outputv = schema.Responses[0];
                TimeSeriesQueryCriteria qc = new TimeSeriesQueryCriteria()
                {
                    Start = schema.Start,
                    End = schema.End,
                    SiteID = outputv.SiteID,
                    VariableID = outputv.VariableID
                };
                IVectorTimeSeries<double> ts = outputv.GetValues(qc, mOdmAdaptor);
                if (ts != null)
                {
                    ds = new CalibrationDatasets(ts.Value, ts.DateTimes);
                    schema.InstancesCount = ts.Value.Length;

                    double[][] input = new double[schema.Stimulus.Length][];
                    int i = 0;
                    foreach (IVariable v in schema.Stimulus)
                    {
                        qc = new TimeSeriesQueryCriteria()
                        {
                            Start = schema.Start,
                            End = schema.End,
                            SiteID = v.SiteID,
                            VariableID = v.VariableID
                        };
                        input[i] = v.GetValues(qc, mOdmAdaptor).Value;
                        i++;
                    }
                    ds.InputData = input;
                    schema.CalibratonDataSets = ds;
                }
            }
            return ds;
        }
     
        #endregion
    }
}
