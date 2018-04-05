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
using System.Collections;
using System.Collections.Generic;
using MiscUtil.Collections.Extensions;
using MiscUtil.Extensions;

namespace MiscUtil.Collections
{
    /// <summary>
    /// Iterates over a range. Despite its name, this implements IEnumerable{T} rather than
    /// IEnumerator{T} - it just sounds better, frankly.
    /// </summary>
    public class RangeIterator<T> : IEnumerable<T>
    {
        readonly Range<T> range;
        /// <summary>
        /// Returns the range this object iterates over
        /// </summary>
        public Range<T> Range
        {
            get { return range; }
        }

        readonly Func<T, T> step;
        /// <summary>
        /// Returns the step function used for this range
        /// </summary>
        public Func<T, T> Step
        {
            get { return step; }
        }

        readonly bool ascending;
        /// <summary>
        /// Returns whether or not this iterator works up from the start point (ascending)
        /// or down from the end point (descending)
        /// </summary>
        public bool Ascending
        {
            get { return ascending; }
        }

        /// <summary>
        /// Creates an ascending iterator over the given range with the given step function
        /// </summary>
        public RangeIterator(Range<T> range, Func<T, T> step)
            : this(range, step, true)
        {
        }

        /// <summary>
        /// Creates an iterator over the given range with the given step function,
        /// with the specified direction.
        /// </summary>
        public RangeIterator(Range<T> range, Func<T, T> step, bool ascending)
        {
            step.ThrowIfNull("step");

            if ((ascending && range.Comparer.Compare(range.Start, step(range.Start)) >= 0) ||
                (!ascending && range.Comparer.Compare(range.End, step(range.End)) <= 0))
            {
                throw new ArgumentException("step does nothing, or progresses the wrong way");
            }
            this.ascending = ascending;
            this.range = range;
            this.step = step;
        }

        /// <summary>
        /// Returns an IEnumerator{T} running over the range.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            // A descending range effectively has the start and end points (and inclusions)
            // reversed, and a reverse comparer.
            bool includesStart = ascending ? range.IncludesStart : range.IncludesEnd;
            bool includesEnd = ascending ? range.IncludesEnd : range.IncludesStart;
            T start = ascending ? range.Start : range.End;
            T end = ascending ? range.End : range.Start;
            IComparer<T> comparer = ascending ? range.Comparer : range.Comparer.Reverse();

            // Now we can use our local version of the range variables to iterate

            T value = start;

            if (includesStart)
            {
                // Deal with possibility that start point = end point
                if (includesEnd || comparer.Compare(value, end) < 0)
                {
                    yield return value;
                }
            }
            value = step(value);

            while (comparer.Compare(value, end) < 0)
            {
                yield return value;
                value = step(value);
            }

            // We've already performed a step, therefore we can't
            // still be at the start point
            if (includesEnd && comparer.Compare(value, end) == 0)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Returns an IEnumerator running over the range.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
