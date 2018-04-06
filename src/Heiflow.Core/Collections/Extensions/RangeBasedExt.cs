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

namespace MiscUtil.Collections.Extensions
{
    /// <summary>
    /// Extension methods to do with ranges.
    /// </summary>
    public static class RangeBasedExt
    {
        /// <summary>
        /// Creates an inclusive range between two values. The default comparer is used
        /// to compare values.
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="start">Start of range.</param>
        /// <param name="end">End of range.</param>
        /// <returns>An inclusive range between the start point and the end point.</returns>
        public static Range<T> To<T>(this T start, T end)
        {
            return new Range<T>(start, end);
        }

        /// <summary>
        /// Returns a RangeIterator over the given range, where the stepping function
        /// is to step by the given number of characters.
        /// </summary>
        /// <param name="range">The range to create an iterator for</param>
        /// <param name="step">How many characters to step each time</param>
        /// <returns>A RangeIterator with a suitable stepping function</returns>
        public static RangeIterator<char> StepChar(this Range<char> range, int step)
        {
            return range.Step(c => (char)(c + step));
        }
    }
}
