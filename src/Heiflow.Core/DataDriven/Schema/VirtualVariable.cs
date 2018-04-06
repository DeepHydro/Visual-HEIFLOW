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

using Heiflow.Core.Hydrology;
using System.ComponentModel;
using Heiflow.Core.Data;
namespace Heiflow.Core.Schema
{
    /// <summary>
    /// Express a virtual variable whose  values are summerized from single or multiple sites
    /// </summary>
    public class VirtualVariable : Variable
    {
        public VirtualVariable(int id)
            : base(id)
        {
            SitesMeasured = new List<HydroPoint>();
            SummationMethod = SummationMethod.ArithmeticAverage;
        }

        [CategoryAttribute("Forecasting"), DescriptionAttribute("The collection of sites measuring the variable")]
        public List<HydroPoint> SitesMeasured { get; set; }

         [CategoryAttribute("Forecasting"), DescriptionAttribute("The method used to sum time series from selected sites")]
         public SummationMethod SummationMethod { get; set; }

         public override IVectorTimeSeries<double> GetValues(ITimeSeriesQueryCriteria qc, ITimeSeriesTransform provider)
         {
             if (SitesMeasured.Count > 0)
             {
                 int count = SitesMeasured.Count;
                 IVectorTimeSeries<double>[] tss = new IVectorTimeSeries<double>[count];
                 int i = 0;
                 foreach (HydroPoint hp in SitesMeasured)
                 {
                     Variable v = new Variable(this.VariableID);
                     v.SiteID = hp.ID;
                     qc.SiteID = hp.ID;
                     qc.VariableID = this.VariableID;
                     tss[i] = v.GetValues(qc, provider);
                     i++;
                 }
                 int length = tss[0].Value.Length;
                 double[] values = new double[length];
                 DateTime[] time = new DateTime[length];
                 Array.Copy(tss[0].DateTimes, time, length);
                 for (int j = 0; j < length; j++)
                 {
                     double sum=0;
                     for (i = 0; i < tss.Length; i++)
                     {
                         sum+=tss[i].Value[j];
                     }
                     values[i] = sum / tss.Length;
                 }
                 IVectorTimeSeries<double> summedts = new NumericalTimeSeries(values, time);
                 return summedts;
             }
             else
             {
                 TimeSpan span = qc.End - qc.Start;
                 double[] values = new double[span.Days];
                 DateTime[] time = new DateTime[span.Days];
                 IVectorTimeSeries<double> summedts = new NumericalTimeSeries(values, time);
                 return summedts;
             }
         }

        public override object Clone()
        {
            return new VirtualVariable(VariableID)
            {
                BeginDate = BeginDate,
                Code = Code,
                EndDate = EndDate,
                GeneralCategory = GeneralCategory,
                ModelRole = ModelRole,
                Name = Name,
                NoDataValue = NoDataValue,
                NormalizationModel = NormalizationModel,
                PredicationSize = PredicationSize,
                SampleMedium = SampleMedium,
                SiteID = SiteID,
                Specification = Specification,
                TimeSupport = TimeSupport,
                TimeUnits = TimeUnits,
                Units = Units,
                ValueCount = ValueCount,
                WindowSize = WindowSize,
                SitesMeasured= SitesMeasured
            };
        }
    }
}
