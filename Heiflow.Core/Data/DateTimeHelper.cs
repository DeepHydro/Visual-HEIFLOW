// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public  class DateTimeHelper
    {
        /// <summary>
        ///  calculates the date off a year (yyyy) and week number (ww) and day of week 
        /// </summary>
        /// <param name="jan1">the date off a year</param>
        /// <param name="ww">week number</param>
        /// <param name="d">day of week </param>
        /// <returns></returns>
        public static DateTime GetBy(int yyyy, int ww, int d = 1)
        {
            var jan1 = new DateTime(yyyy, 1, 1);
            int daysOffset = DayOfWeek.Tuesday - jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = ww;

            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstMonday.AddDays(weekNum * 7 + d - 1);

            return result;
        }
    }
}
