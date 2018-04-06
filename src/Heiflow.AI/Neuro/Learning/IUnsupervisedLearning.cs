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

namespace  Heiflow.AI.Neuro.Learning
{
    using System;

    /// <summary>
    /// Unsupervised learning interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface describes methods, which should be implemented
    /// by all unsupervised learning algorithms. Unsupervised learning is such
    /// type of learning algorithms, where system's desired output is not known on
    /// the learning stage. Given sample input values, it is expected, that
    /// system will organize itself in the way to find similarities betweed provided
    /// samples.</para></remarks>
    /// 
    public interface IUnsupervisedLearning
    {
        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns learning error.</returns>
        /// 
        double Run( double[] input );

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        ///
        /// <returns>Returns sum of learning errors.</returns>
        /// 
        double RunEpoch( double[][] input );
    }
}
