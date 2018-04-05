// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// A Surface is a two-dimensional geometric object.
    /// </summary>
    /// <remarks>
    /// The OpenGIS Abstract Specification defines a simple Surface as consisting of a single ‘patch?that is
    /// associated with one ‘exterior boundary?and 0 or more ‘interior?boundaries. Simple surfaces in threedimensional
    /// space are isomorphic to planar surfaces. Polyhedral surfaces are formed by ‘stitching?together
    /// simple surfaces along their boundaries, polyhedral surfaces in three-dimensional space may not be planar as
    /// a whole.
    /// </remarks>
    public abstract class Surface : Geometry
    {
        /// <summary>
        /// The area of this Surface, as measured in the spatial reference system of this Surface.
        /// </summary>
        public abstract double Area { get; }

        /// <summary>
        /// The mathematical centroid for this Surface as a Point.
        /// The result is not guaranteed to be on this Surface.
        /// </summary>
        public virtual Point Centroid
        {
            get { return GetBoundingBox().GetCentroid(); }
        }

        /// <summary>
        /// A point guaranteed to be on this Surface.
        /// </summary>
        public abstract Point PointOnSurface { get; }

        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal
        ///  to the coordinate dimension. This specification is restricted to geometries in two-dimensional coordinate
        ///  space.
        /// </summary>
        public override int Dimension
        {
            get { return 2; }
        }
    }
}