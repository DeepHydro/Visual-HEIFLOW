// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;

namespace MiscUtil.Collections.Extensions
{
    /// <summary>
    /// Extensions to IComparer
    /// </summary>
    public static class ComparerExt
    {
        /// <summary>
        /// Reverses the original comparer; if it was already a reverse comparer,
        /// the previous version was reversed (rather than reversing twice).
        /// In other words, for any comparer X, X==X.Reverse().Reverse().
        /// </summary>
        public static IComparer<T> Reverse<T>(this IComparer<T> original)
        {
            ReverseComparer<T> originalAsReverse = original as ReverseComparer<T>;
            if (originalAsReverse != null)
            {
                return originalAsReverse.OriginalComparer;
            }
            return new ReverseComparer<T>(original);
        }

        /// <summary>
        /// Combines a comparer with a second comparer to implement composite sort
        /// behaviour.
        /// </summary>
        public static IComparer<T> ThenBy<T>(this IComparer<T> firstComparer, IComparer<T> secondComparer)
        {
            return new LinkedComparer<T>(firstComparer, secondComparer);
        }

        /// <summary>
        /// Combines a comparer with a projection to implement composite sort behaviour.
        /// </summary>
        public static IComparer<T> ThenBy<T, TKey>(this IComparer<T> firstComparer, Func<T, TKey> projection)
        {
            return new LinkedComparer<T>(firstComparer, new ProjectionComparer<T, TKey>(projection));
        }
    }
}
