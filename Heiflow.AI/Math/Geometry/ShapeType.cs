// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
