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
