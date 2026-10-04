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