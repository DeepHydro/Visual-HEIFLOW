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
    public class SequenceMonitorItem : MonitorItem
    {

        public SequenceMonitorItem(string name)
            : base(name)
        {
             Derivable = true;
        }

        public MonitorItem Source
        {
            get;
            set;
        }

        public override double[] Derive(ListTimeSeries<double> sourcedata)
        {
            if (Source != null)
            {
                if (Source.Derivable && Source.DerivedValues == null)
                {
                    Source.DerivedValues = Source.Derive(sourcedata);
                }
                double[] values = new double[sourcedata.Dates.Count];

                for (int i = 1; i < sourcedata.Dates.Count; i++)
                {
                    values[i] = Source.DerivedValues[i] - Source.DerivedValues[i - 1];
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
