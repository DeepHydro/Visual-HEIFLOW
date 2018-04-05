// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Time
{
    public class CreatMonthTimeInteval : ModelTool
    {
        //private NumericalDataType mNumericalDataType;
        private TimeUnits mTimeUnit;
        public CreatMonthTimeInteval()
        {
            Name = "Creat Monthly Time Inteval";
            Category = "Time";
            Description = "Creat Monthly Time Inteval";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
         //   mNumericalDataType = Core.NumericalDataType.Average;
            mTimeUnit = TimeUnits.Month;
            Start = new DateTime(2000, 1, 1);
            End = new DateTime(2012, 12, 31);
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
        [Category("Parameter")]
        public DateTime Start
        {
            get;
            set;
        }
        [Category("Parameter")]
        public DateTime End
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The output filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = !TypeConverterEx.IsNull(OutputFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamWriter sw = new StreamWriter(OutputFileName);
            int nmon = (End.Year - Start.Year + 1) * 12;
            sw.WriteLine("Animation output control file created on " + DateTime.Now);
            sw.WriteLine(nmon);
            for (int y = Start.Year; y <= End.Year; y++)
            {
                for (int m = 1; m <= 12; m++)
                {
                    int days = DateTime.DaysInMonth(y, m);
                    sw.WriteLine(days);
                }
            }
            sw.Close();
            return true;
        }
    }
}
