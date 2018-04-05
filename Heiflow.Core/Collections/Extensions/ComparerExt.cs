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
