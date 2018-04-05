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

namespace  Heiflow.AI.Math.Geometry
{
    using System;
    using System.Collections.Generic;

    using  Heiflow.AI;

    /// <summary>
    /// Interface defining methods for algorithms, which search for convex hull of the specified points' set.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines a method, which should be implemented by different classes
    /// performing convex hull search for specified set of points.</para>
    /// 
    /// <para><note>All algorithms, implementing this interface, should follow two rules for the found convex hull:
    /// <list type="bullet">
    /// <item>the first point in the returned list is the point with lowest X coordinate (and with lowest Y if
    /// there are several points with the same X value);</item>
    /// <item>points in the returned list are given in counter clockwise order
    /// (<a href="http://en.wikipedia.org/wiki/Cartesian_coordinate_system">Cartesian
    /// coordinate system</a>).</item>
    /// </list>
    /// </note></para>
    /// </remarks>
    /// 
    public interface IConvexHullAlgorithm
    {
        /// <summary>
        /// Find convex hull for the given set of points.
        /// </summary>
        /// 
        /// <param name="points">Set of points to search convex hull for.</param>
        /// 
        /// <returns>Returns set of points, which form a convex hull for the given <paramref name="points"/>.</returns>
        /// 
        List<IntPoint> FindHull( List<IntPoint> points );
    }
}
