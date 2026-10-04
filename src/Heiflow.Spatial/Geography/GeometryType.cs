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
    /// Enumeration of Simple Features Geometry types
    /// </summary>
    public enum GeometryType2
    {
        /// <summary>
        /// Geometry is the root class of the hierarchy. Geometry is an abstract (non-instantiable) class.
        /// </summary>
        Geometry = 0,
        /// <summary>
        /// A Point is a 0-dimensional geometry and represents a single location in coordinate space.
        /// </summary>
        Point = 1,
        /// <summary>
        /// A curve is a one-dimensional geometric object usually stored as a sequence of points,
        /// with the subtype of curve specifying the form of the interpolation between points.
        /// </summary>
        Curve = 2,
        /// <summary>
        /// A LineString is a curve with linear interpolation between points. Each consecutive
        /// pair of points defines a line segment.
        /// </summary>
        LineString = 3,
        /// <summary>
        /// A Surface is a two-dimensional geometric object.
        /// </summary>
        Surface = 4,
        /// <summary>
        /// A Polygon is a planar surface, defined by 1 exterior boundary and 0 or more interior
        /// boundaries. Each interior boundary defines a hole in the polygon.
        /// </summary>
        Polygon = 5,
        /// <summary>
        /// A GeometryCollection is a geometry that is a collection of 1 or more geometries.
        /// </summary>
        GeometryCollection = 6,
        /// <summary>
        /// A MultiPoint is a 0 dimensional geometric collection. The elements of a MultiPoint
        /// are restricted to Points. The points are not connected or ordered.
        /// </summary>
        MultiPoint = 7,
        /// <summary>
        /// A MultiCurve is a one-dimensional GeometryCollection whose elements are Curves.
        /// </summary>
        MultiCurve = 8,
        /// <summary>
        /// A MultiLineString is a MultiCurve whose elements are LineStrings.
        /// </summary>
        MultiLineString = 9,
        /// <summary>
        /// A MultiSurface is a two-dimensional geometric collection whose elements are
        /// surfaces. The interiors of any two surfaces in a MultiSurface may not intersect.
        /// The boundaries of any two elements in a MultiSurface may intersect at most at a
        /// finite number of points.
        /// </summary>
        MultiSurface = 10,
        /// <summary>
        /// A MultiPolygon is a MultiSurface whose elements are Polygons.
        /// </summary>
        MultiPolygon = 11,
    }
}