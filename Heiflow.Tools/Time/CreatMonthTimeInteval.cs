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
