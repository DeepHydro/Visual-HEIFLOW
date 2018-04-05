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

namespace MiscUtil.Extensions.TimeRelated
{
    /// <summary>
    /// Extension methods producing DateTime values. These are useful
    /// when writing unit tests with fixed dates which should be easily understandable.
    /// </summary>
    public static class DateTimeBasedExt
    {
        /// <summary>
        /// Returns a DateTime representing the specified day in January
        /// in the specified year.
        /// </summary>
        public static DateTime January(this int day, int year)
        {
            return new DateTime(year, 1, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in February
        /// in the specified year.
        /// </summary>
        public static DateTime February(this int day, int year)
        {
            return new DateTime(year, 2, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in March
        /// in the specified year.
        /// </summary>
        public static DateTime March(this int day, int year)
        {
            return new DateTime(year, 3, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in April
        /// in the specified year.
        /// </summary>
        public static DateTime April(this int day, int year)
        {
            return new DateTime(year, 4, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in May
        /// in the specified year.
        /// </summary>
        public static DateTime May(this int day, int year)
        {
            return new DateTime(year, 5, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in June
        /// in the specified year.
        /// </summary>
        public static DateTime June(this int day, int year)
        {
            return new DateTime(year, 6, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in July
        /// in the specified year.
        /// </summary>
        public static DateTime July(this int day, int year)
        {
            return new DateTime(year, 7, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in August
        /// in the specified year.
        /// </summary>
        public static DateTime August(this int day, int year)
        {
            return new DateTime(year, 8, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in September
        /// in the specified year.
        /// </summary>
        public static DateTime September(this int day, int year)
        {
            return new DateTime(year, 9, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in October
        /// in the specified year.
        /// </summary>
        public static DateTime October(this int day, int year)
        {
            return new DateTime(year, 10, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in November
        /// in the specified year.
        /// </summary>
        public static DateTime November(this int day, int year)
        {
            return new DateTime(year, 11, day);
        }

        /// <summary>
        /// Returns a DateTime representing the specified day in December
        /// in the specified year.
        /// </summary>
        public static DateTime December(this int day, int year)
        {
            return new DateTime(year, 12, day);
        }
    }
}
