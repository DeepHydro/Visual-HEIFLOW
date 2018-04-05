// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
    public class AggregatedMonitorItem : MonitorItem
    {
        public AggregatedMonitorItem(string name)
            : base(name)
        {
            Source = new List<MonitorItem>();
            SourceSign = new List<int>();
            Derivable = true;
        }

        public List<MonitorItem> Source
        {
            get;
            set;
        }

        public List<int> SourceSign
        {
            get;
            set;
        }

        public override double[] Derive(ListTimeSeries<double> sourcedata)
        {
            int c = 0;
            if (Source.Count > 0)
            {
                foreach (var item in Source)
                {
                    if (item.Derivable && item.DerivedValues == null)
                    {
                        item.DerivedValues = item.Derive(sourcedata);
                    }
                }

                double[] values = new double[sourcedata.Dates.Count];

                for (int i = 0; i < sourcedata.Dates.Count; i++)
                {
                    c = 0;
                    foreach (var item in Source)
                    {
                        if (item.Derivable)
                        {
                            values[i] += item.DerivedValues[i] * SourceSign[c];
                        }
                        else
                        {
                            values[i] += sourcedata.Values[item.VariableIndex][i] * SourceSign[c];
                        }
                        c++;
                    }
                }
                return values;
            }
            else
            {
                return null;
            }
        }
    }
}
