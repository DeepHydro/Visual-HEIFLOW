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
using Heiflow.Models.AI;
using Heiflow.Core.Data;
using Heiflow.Core;
using Heiflow.Core.Schema;

namespace Heiflow.Core.Data
{
     public class RollingSelection : IForecastingDataSetsSelection
    {
         public RollingSelection()
         {
             mOdmAdaptor = new ODMDataAdaptor(Configuration.DataBase);
         }

         ODMDataAdaptor mOdmAdaptor;

        #region IForecastingDataSetsSelection members

         public IForecastingDataSets Select(IPredicationSchema schema)
        {
            IVectorTimeSeries<double>[] tsList = new IVectorTimeSeries<double>[schema.Stimulus.Length];
            int [] startIndexOfStimulus = new int[schema.Stimulus.Length];
            int i=0;
            int valuesCount = 0;
            DateTime[] dates = null;
            foreach (IVariable v in schema.Stimulus)
            {
                TimeSeriesQueryCriteria qc = new TimeSeriesQueryCriteria()
                {
                    Start = schema.Start,
                    End = schema.End,
                    SiteID = v.SiteID,
                    VariableID = v.VariableID
                };
                tsList[i] = v.GetValues(qc, mOdmAdaptor);
                if (v.NormalizationModel != null)
                    v.NormalizationModel.Normalize(tsList[i].Value);
                if (i == 0)
                {
                    valuesCount = tsList[i].Value.Length;
                }
                else
                {
                    if (valuesCount != tsList[i].Value.Length)
                        throw new Exception("The value counts of the  input variables are not identical!");
                }
                startIndexOfStimulus[i] = schema.MaximumWindowSize - v.WindowSize;
                i++;
            }
           schema.InstancesCount = valuesCount - schema.MaximumWindowSize - schema.PredicationSize;

            dates = new DateTime[schema.InstancesCount];

            ForecastingSetsBuilder builder = new ForecastingSetsBuilder(schema);
            double [][] inputData = builder.BuildInputSets(startIndexOfStimulus,tsList);

            i = 0;
            tsList = new IVectorTimeSeries<double>[schema.Responses.Length];
           int [] startIndexOfResponses = new int[schema.Responses.Length];
            foreach (IVariable v in schema.Responses)
            {
                TimeSeriesQueryCriteria qc = new TimeSeriesQueryCriteria()
                {
                    Start = schema.Start,
                    End = schema.End,
                     SiteID = v.SiteID,
                    VariableID = v.VariableID
                };
                tsList[i] = v.GetValues(qc, mOdmAdaptor);
                if (v.NormalizationModel != null)
                    v.NormalizationModel.Normalize(tsList[i].Value);
                if (valuesCount != tsList[i].Value.Length)
                    throw new Exception("The value counts of the  input variables are not identical!");
                startIndexOfResponses[i] = schema.MaximumWindowSize + v.PredicationSize;
                if (i == 0)
                {
                    System.Array.Copy(tsList[i].DateTimes, startIndexOfResponses[i], dates, 0, schema.InstancesCount);
                }
                i++;
            }
            double[][]  outputData = builder.BuildOutputSets(startIndexOfResponses, tsList);

            ForecastingDataSets result = new ForecastingDataSets(inputData, outputData)
            {
                Date = dates
            };
            return result;
        }

        #endregion
    }    

    public class CorrespondingPeriodSelection : IForecastingDataSetsSelection
    {
        #region IForecastingDataSetsSelection members

        public IForecastingDataSets Select(IPredicationSchema schema)
        {
            int startYear = schema.Start.Year;
            int startMonth = schema.Start.Month;
            int startDay = schema.Start.Day;
            int endYear = schema.End.Year;
            int endMonth = schema.End.Month;
            int endDay = schema.End.Day;
           
            IForecastingDataSets[] sets = new IForecastingDataSets[endYear - startYear + 1];
            for (int i = startYear; i <= endYear; i++)
            {
                DateTime start = new DateTime(i, startMonth, startDay);
                DateTime end =  new DateTime(i, endMonth, endDay);
                PredicationSchema clonedSchema = new PredicationSchema(schema.Stimulus, schema.Responses)
                {
                    Start = start,
                    End = end,
                    TimePeriod = schema.TimePeriod
                };
                RollingSelection rollSlct = new RollingSelection();
                sets[i - startYear] = rollSlct.Select(clonedSchema);
            }
           ForecastingDataSets result =  ForecastingDataSets.Merge(sets);
           schema.InstancesCount = result.InputData.Length;
            return result;
        }

        #endregion
    }

    public class ForecastingSetsBuilder
    {
        public ForecastingSetsBuilder(IPredicationSchema schema)
        {
            mSchema = schema;
        }

        private IPredicationSchema mSchema;

        public double[][] BuildInputSets(int[] startIndexOfStimulus, IVectorTimeSeries<double>[] tsList)
        {
            double[][] inputs = new double[mSchema.InstancesCount][];

            int extraInputCount = (from inv in mSchema.Stimulus where inv.IncludeIntradayValue == true select inv).Count();

            for (int r = 0; r < mSchema.InstancesCount; r++)
            {
                inputs[r] = new double[mSchema.TotalWindowSize + extraInputCount];
                int i = 0;
                int c = 0;
                foreach (IVariable v in mSchema.Stimulus)
                {
                    int size = !v.IncludeIntradayValue ? v.WindowSize : v.WindowSize + 1;
                    for (int t = 0; t < size; t++)
                    {
                        inputs[r][c] = tsList[i].Value[startIndexOfStimulus[i] + t];
                        c++;
                    }
                    startIndexOfStimulus[i]++;
                    i++;
                }
            }
            return inputs;
        }

        public double[][] BuildOutputSets(int[] startIndexOfResponses, IVectorTimeSeries<double>[] tsList)
        {
            double[][] outputs = new double[mSchema.InstancesCount][];
            for (int r = 0; r < mSchema.InstancesCount; r++)
            {
                outputs[r] = new double[tsList.Length];
                int i = 0;
                foreach (IVariable v in mSchema.Responses)
                {
                    outputs[r][i] = tsList[i].Value[startIndexOfResponses[i]];
                    startIndexOfResponses[i]++;
                    i++;
                }
            }
            return outputs;
        }
    }
}
