// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Math
{
    public class Derive : ModelTool
    {
        private NumericalDataType mNumericalDataType;
        private TimeUnits mTimeUnit;
        public Derive()
        {
            Name = "Derive";
            Category = "Math";
            Description = "Derive time series from existing data cube";
            Version = "1.0.0.0";
            Derived = "Derived";
            this.Author = "Yong Tian";
            mNumericalDataType = Core.NumericalDataType.Average;
            mTimeUnit = TimeUnits.Month;
        }

        [Category("Input")]
        [Description("The input matrix which should be [0][-1][-1]")]
        public string Source
        {
            get;
            set;
        }

        [Category("Parameter")]
        public NumericalDataType NumericalDataType
        {
            get
            {
                return mNumericalDataType;
            }
            set
            {
                mNumericalDataType = value;
            }
        }


        [Category("Parameter")]
        public TimeUnits TimeUnits
        {
            get
            {
                return mTimeUnit;
            }
            set
            {
                mTimeUnit = value;
            }
        }


        [Category("Output")]
        [Description("The derived matrix name")]
        public string Derived
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var m1 = Validate(Source);
            this.Initialized = m1;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            double prg = 0;

            if (mat != null)
            {
                int nstep = mat.Size[1];
                int ncell = mat.Size[2];
                var vec = mat.GetSeriesAt(var_index, 0);
                var dou_vec = MyMath.ToDouble(vec);

                var date_source = new DateTime[nstep];
                if (mat.DateTimes != null && mat.DateTimes.Length >= nstep)
                {
                    for (int i = 0; i < nstep; i++)
                    {
                        date_source[i] = mat.DateTimes[i];
                    }
                }
                else
                {
                    for (int i = 0; i < nstep; i++)
                    {
                        date_source[i] = ModelService.Start.AddDays(i);
                    }
                }
                var ts = new FloatTimeSeries(vec, date_source);
                var derieved_ts = TimeSeriesAnalyzer.Derieve(ts, NumericalDataType, TimeUnits);
                var derieved_steps = derieved_ts.DateTimes.Length;
                var mat_out = new My3DMat<float>(1, derieved_steps, ncell);

                mat_out.Name = Derived;
                mat_out.Variables = new string[] { "Derived" };
                mat_out.DateTimes = derieved_ts.DateTimes.ToArray();
                mat_out.TimeBrowsable = true;
                for (int c = 0; c < ncell; c++)
                {
                    vec = mat.GetSeriesAt(var_index, c);
                    ts = new FloatTimeSeries(vec, date_source);
                    derieved_ts = TimeSeriesAnalyzer.Derieve(ts, NumericalDataType, TimeUnits);
                    for (int t = 0; t < derieved_steps; t++)
                    {
                        mat_out.Value[0][t][c] = derieved_ts.Value[t];
                    }
                    prg = (c + 1) * 100.0 / ncell;
                    if (prg % 10 == 0)
                        cancelProgressHandler.Progress("Package_Tool", (int)prg, "Caculating Cell: " + (c + 1));
                }

                Workspace.Add(mat_out);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
