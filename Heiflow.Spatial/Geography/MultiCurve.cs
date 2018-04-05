// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// A MultiCurve is a one-dimensional GeometryCollection whose elements are Curves
    /// </summary>
    public abstract class MultiCurve : GeometryCollection
    {
        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override int Dimension
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns true if this MultiCurve is closed (StartPoint=EndPoint for each curve in this MultiCurve)
        /// </summary>
        public abstract bool IsClosed { get; }

        /// <summary>
        /// The Length of this MultiCurve which is equal to the sum of the lengths of the element Curves.
        /// </summary>
        public abstract double Length { get; }

        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        public new abstract int NumGeometries { get; }

        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="N">Geometry index</param>
        /// <returns>Geometry at index N</returns>
        public new abstract Geometry Geometry(int N);
    }
}