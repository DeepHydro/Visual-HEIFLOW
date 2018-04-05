// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiscUtil.Collections.Extensions
{
    /// <summary>
    /// Wrapper methods for SmartEnumerable[T].
    /// </summary>
    public static class SmartEnumerableExt
    {
        /// <summary>
        /// Extension method to make life easier.
        /// </summary>
        /// <typeparam name="T">Type of enumerable</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <returns>A new SmartEnumerable of the appropriate type</returns>
        public static SmartEnumerable<T> AsSmartEnumerable<T>(this IEnumerable<T> source)
        {
            return new SmartEnumerable<T>(source);
        }
    }

}
