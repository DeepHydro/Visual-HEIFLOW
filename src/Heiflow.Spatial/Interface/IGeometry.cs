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

using System;

namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// Defines basic interface for a Geometry
    /// </summary>
    public interface IGeometry
    {
        #region "Basic Methods on Geometry"

        /// <summary>
        ///  The inherent dimension of this <see cref="Geometry"/> object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        int Dimension { get; }

        /// <summary>
        /// The minimum bounding box for this Geometry, returned as a <see cref="Geometry"/>. The
        /// polygon is defined by the corner points of the bounding box ((MINX, MINY), (MAXX, MINY), (MAXX,
        /// MAXY), (MINX, MAXY), (MINX, MINY)).
        /// </summary>
        Geometry Envelope();

        /// <summary>
        /// The minimum <see cref="BoundingBox"/> for this <see cref="Geometry"/>.
        /// </summary>
        /// <returns><see cref="BoundingBox"/> for this <see cref="Geometry"/></returns>
        BoundingBox GetBoundingBox();

        /// <summary>
        /// Exports this <see cref="Geometry"/> to a specific well-known text representation of <see cref="Geometry"/>.
        /// </summary>
        string AsText();

        /// <summary>
        /// Exports this <see cref="Geometry"/> to a specific well-known binary representation of <see cref="Geometry"/>.
        /// </summary>
        byte[] AsBinary();

        /// <summary>
        /// Returns a WellKnownText representation of the <see cref="Geometry"/>
        /// </summary>
        /// <returns>Well-known text</returns>
        string ToString();

        /// <summary>
        /// If true, then this <see cref="Geometry"/> represents the empty point set, ? for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this <see cref="Geometry"/> is the empty geometry</returns>
        bool IsEmpty();

        /// <summary>
        ///  Returns 'true' if this <see cref="Geometry"/> has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the <see cref="Geometry"/> is simple</returns>
        bool IsSimple();

        /// <summary>
        /// Returns the closure of the combinatorial boundary of this <see cref="Geometry"/>. The
        /// combinatorial boundary is defined as described in section 3.12.3.2 of [1]. Because the result of this function
        /// is a closure, and hence topologically closed, the resulting boundary can be represented using
        /// representational geometry primitives
        /// </summary>
        /// <returns>Closure of the combinatorial boundary of this <see cref="Geometry"/></returns>
        Geometry Boundary();

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> is spatially related to another <see cref="Geometry"/>, by testing
        /// for intersections between the Interior, Boundary and Exterior of the two geometries
        /// as specified by the values in the intersectionPatternMatrix
        /// </summary>
        /// <param name="other"><see cref="Geometry"/> to relate to</param>
        /// <param name="intersectionPattern">Intersection Pattern</param>
        /// <returns>True if spatially related</returns>
        bool Relate(Geometry other, string intersectionPattern);

        #endregion

        #region "Methods for testing Spatial Relations between geometric objects"

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> is 'spatially equal' to another <see cref="Geometry"/>
        /// </summary>
        bool Equals(Geometry geom);

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> is 'spatially disjoint' from another <see cref="Geometry"/>
        /// </summary>
        bool Disjoint(Geometry geom);

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> 'spatially intersects' another <see cref="Geometry"/>
        /// </summary>
        bool Intersects(Geometry geom);

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> 'spatially touches' another <see cref="Geometry"/>.
        /// </summary>
        bool Touches(Geometry geom);

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> 'spatially crosses' another <see cref="Geometry"/>.
        /// </summary>
        bool Crosses(Geometry geom);

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> is 'spatially within' another <see cref="Geometry"/>.
        /// </summary>
        bool Within(Geometry geom);

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> 'spatially contains' another <see cref="Geometry"/>.
        /// </summary>
        bool Contains(Geometry geom);

        /// <summary>
        /// Returns 'true' if this <see cref="Geometry"/> 'spatially overlaps' another <see cref="Geometry"/>.
        /// </summary>
        bool Overlaps(Geometry geom);

        #endregion

        #region "Methods that support Spatial Analysis"

        /// <summary>
        /// Returns the shortest distance between any two points in the two geometries
        /// as calculated in the spatial reference system of this <see cref="Geometry"/>.
        /// </summary>
        /// <param name="geom"><see cref="Geometry"/> to calculate distance to</param>
        /// <returns>Shortest distance between any two points in the two geometries</returns>
        double Distance(Geometry geom);

        /// <summary>
        /// Returns a <see cref="Geometry"/> that represents all points whose distance from this <see cref="Geometry"/>
        /// is less than or equal to distance. Calculations are in the Spatial Reference
        /// System of this <see cref="Geometry"/>.
        /// </summary>
        /// <param name="d">Buffer distance</param>
        /// <returns>Buffer around <see cref="Geometry"/></returns>
        Geometry Buffer(double d);


        /// <summary>
        /// Returns a <see cref="Geometry"/> that represents the convex hull of this <see cref="Geometry"/>.
        /// </summary>
        /// <returns>The convex hull</returns>
        Geometry ConvexHull();

        /// <summary>
        /// Returns a <see cref="Geometry"/> that represents the point set intersection of this <see cref="Geometry"/>
        /// with another <see cref="Geometry"/>.
        /// </summary>
        /// <param name="geom"><see cref="Geometry"/> to intersect with</param>
        /// <returns>Returns a <see cref="Geometry"/> that represents the point set intersection of this <see cref="Geometry"/> with another <see cref="Geometry"/>.</returns>
        Geometry Intersection(Geometry geom);

        /// <summary>
        /// Returns a <see cref="Geometry"/> that represents the point set union of this <see cref="Geometry"/> with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to union with</param>
        /// <returns>Unioned <see cref="Geometry"/></returns>
        Geometry Union(Geometry geom);

        /// <summary>
        /// Returns a <see cref="Geometry"/> that represents the point set difference of this <see cref="Geometry"/> with anotherGeometry.
        /// </summary>
        /// <param name="geom"><see cref="Geometry"/> to compare to</param>
        /// <returns><see cref="Geometry"/></returns>
        Geometry Difference(Geometry geom);

        /// <summary>
        /// Returns a geometry that represents the point set symmetric difference of this <see cref="Geometry"/> with anotherGeometry.
        /// </summary>
        /// <param name="geom"><see cref="Geometry"/> to compare to</param>
        /// <returns><see cref="Geometry"/></returns>
        Geometry SymDifference(Geometry geom);

        #endregion
    }
}
