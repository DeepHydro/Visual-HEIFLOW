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

    /// <summary>
    /// Enumeration of some basic shape types.
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// Unknown shape type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Circle shape.
        /// </summary>
        Circle,

        /// <summary>
        /// Triangle shape.
        /// </summary>
        Triangle,

        /// <summary>
        /// Quadrilateral shape.
        /// </summary>
        Quadrilateral,
    }

    /// <summary>
    /// Some common sub types of some basic shapes.
    /// </summary>
    public enum PolygonSubType
    {
        /// <summary>
        /// Unrecognized sub type of a shape (generic shape which does not have
        /// any specific sub type).
        /// </summary>
        Unknown,

        /// <summary>
        /// Quadrilateral with one pair of parallel sides.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Quadrilateral with two pairs of parallel sides.
        /// </summary>
        Parallelogram,

        /// <summary>
        /// Parallelogram with perpendicular adjacent sides.
        /// </summary>
        Rectangle,

        /// <summary>
        /// Parallelogram with all sides equal.
        /// </summary>
        Rhombus,

        /// <summary>
        /// Rectangle with all sides equal.
        /// </summary>
        Square,

        /// <summary>
        /// Triangle with all sides/angles equal.
        /// </summary>
        EquilateralTriangle,

        /// <summary>
        /// Triangle with two sides/angles equal.
        /// </summary>
        IsoscelesTriangle,

        /// <summary>
        /// Triangle with a 90 degrees angle.
        /// </summary>
        RectangledTriangle,

        /// <summary>
        /// Triangle with a 90 degrees angle and other two angles are equal.
        /// </summary>
        RectangledIsoscelesTriangle
    }
}
