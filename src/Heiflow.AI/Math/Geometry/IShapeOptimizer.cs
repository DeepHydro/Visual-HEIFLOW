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

namespace  Heiflow.AI.Math.Geometry
{
    using System;
    using System.Collections.Generic;
    using  Heiflow.AI;

    /// <summary>
    /// Interface for shape optimizing algorithms.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines set of methods, which should be implemented
    /// by shape optimizing algorithms. These algorithms take input shape, which is defined
    /// by a set of points (corners of convex hull, etc.), and remove some insignificant points from it,
    /// which has little influence on the final shape's look.</para>
    /// 
    /// <para>The shape optimizing algorithms can be useful in conjunction with such algorithms
    /// like convex hull searching, which usually may provide many hull points, where some
    /// of them are insignificant and could be removed.</para>
    ///
    /// <para>For additional details about shape optimizing algorithms, documentation of
    /// particular algorithm should be studied.</para>
    /// </remarks>
    /// 
    public interface IShapeOptimizer
    {
        /// <summary>
        /// Optimize specified shape.
        /// </summary>
        /// 
        /// <param name="shape">Shape to be optimized.</param>
        /// 
        /// <returns>Returns final optimized shape, which may have reduced amount of points.</returns>
        /// 
        List<IntPoint> OptimizeShape( List<IntPoint> shape );
    }
}
