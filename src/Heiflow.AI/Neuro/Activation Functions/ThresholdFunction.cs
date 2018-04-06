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

namespace  Heiflow.AI.Neuro
{
    using System;

    /// <summary>
    /// Threshold activation function.
    /// </summary>
    ///
    /// <remarks><para>The class represents threshold activation function with
    /// the next expression:
    /// <code lang="none">
    /// f(x) = 1, if x >= 0, otherwise 0
    /// </code>
    /// </para>
    /// 
    /// <para>Output range of the function: <b>[0, 1]</b>.</para>
    /// 
    /// <para>Functions graph:</para>
    /// <img src="img/neuro/threshold.bmp" width="242" height="172" />
    /// </remarks>
    ///
    [Serializable]
    public class ThresholdFunction : IActivationFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdFunction"/> class.
        /// </summary>
        public ThresholdFunction( ) { }

        /// <summary>
        /// Calculates function value.
        /// </summary>
        ///
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function output value, <i>f(x)</i>.</returns>
        ///
        /// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
        ///
        public double Function( double x )
        {
            return ( x >= 0 ) ? 1 : 0;
        }

        /// <summary>
        /// Calculates function derivative (not supported).
        /// </summary>
        /// 
        /// <param name="x">Input value.</param>
        /// 
        /// <returns>Always returns 0.</returns>
        /// 
        /// <remarks><para><note>The method is not supported, because it is not possible to
        /// calculate derivative of the function.</note></para></remarks>
        ///
        public double Derivative( double x )
        {
            return 0;
        }

        /// <summary>
        /// Calculates function derivative (not supported).
        /// </summary>
        /// 
        /// <param name="y">Input value.</param>
        /// 
        /// <returns>Always returns 0.</returns>
        /// 
        /// <remarks><para><note>The method is not supported, because it is not possible to
        /// calculate derivative of the function.</note></para></remarks>
        /// 
        public double Derivative2( double y )
        {
            return 0;
        }
    }
}
