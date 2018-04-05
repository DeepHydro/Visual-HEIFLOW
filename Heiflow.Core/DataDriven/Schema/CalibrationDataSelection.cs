// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
