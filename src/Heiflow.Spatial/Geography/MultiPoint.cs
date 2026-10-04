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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// A MultiPoint is a 0 dimensional geometric collection. The elements of a MultiPoint are
    /// restricted to Points. The points are not connected or ordered.
    /// </summary>
    public class MultiPoint : GeometryCollection
    {
        private IList<Point> _Points;

        /// <summary>
        /// Initializes a new MultiPoint collection
        /// </summary>
        public MultiPoint()
        {
            _Points = new Collection<Point>();
        }

        /// <summary>
        /// Initializes a new MultiPoint collection
        /// </summary>		
        public MultiPoint(IEnumerable<double[]> points)
        {
            _Points = new Collection<Point>();
            foreach (double[] point in points)
                _Points.Add(new Point(point[0], point[1]));
        }

        public static MultiPoint Creates(string[] pointsStr, char splitor)
        {
            MultiPoint result = new MultiPoint();
            if (pointsStr != null)
            {
                foreach (string pstr in pointsStr)
                {
                    string[] point = pstr.Split(new char[] { splitor});
                    result.Points.Add(new Point(double.Parse(point[0]), double.Parse(point[1])));

                }
            }
            return result;
        }

        /// <summary>
        /// Gets the n'th point in the MultiPoint collection
        /// </summary>
        /// <param name="n">Index in collection</param>
        /// <returns>Point</returns>
        public new Point this[int n]
        {
            get { return _Points[n]; }
        }

        /// <summary>
        /// Gets or sets the MultiPoint collection
        /// </summary>
        public IList<Point> Points
        {
            get { return _Points; }
            set { _Points = value; }
        }

        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        public override int NumGeometries
        {
            get { return _Points.Count; }
        }


        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        public  int NumPoints
        {
            get { return _Points.Count; }
        }

        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override int Dimension
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="N">Geometry index</param>
        /// <returns>Geometry at index N</returns>
        public new Point Geometry(int N)
        {
            return _Points[N];
        }

        /// <summary>
        /// If true, then this Geometry represents the empty point set, ? for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override bool IsEmpty()
        {
            return (_Points != null && _Points.Count == 0);
        }

        /// <summary>
        ///  Returns 'true' if this Geometry has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the geometry is simple</returns>
        public override bool IsSimple()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The boundary of a MultiPoint is the empty set (null).
        /// </summary>
        /// <returns></returns>
        public override Geometry Boundary()
        {
            return null;
        }

        /// <summary>
        /// Returns the shortest distance between any two points in the two geometries
        /// as calculated in the spatial reference system of this Geometry.
        /// </summary>
        /// <param name="geom">Geometry to calculate distance to</param>
        /// <returns>Shortest distance between any two points in the two geometries</returns>
        public override double Distance(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents all points whose distance from this Geometry
        /// is less than or equal to distance. Calculations are in the Spatial Reference
        /// System of this Geometry.
        /// </summary>
        /// <param name="d">Buffer distance</param>
        /// <returns>Buffer around geometry</returns>
        public override Geometry Buffer(double d)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Geometry—Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        /// <returns>The convex hull</returns>
        public override Geometry ConvexHull()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set intersection of this Geometry
        /// with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to intersect with</param>
        /// <returns>Returns a geometry that represents the point set intersection of this Geometry with anotherGeometry.</returns>
        public override Geometry Intersection(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set union of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to union with</param>
        /// <returns>Unioned geometry</returns>
        public override Geometry Union(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override Geometry Difference(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set symmetric difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override Geometry SymDifference(Geometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns></returns>
        public override BoundingBox GetBoundingBox()
        {
            if (_Points == null || _Points.Count == 0)
                return null;
            BoundingBox bbox = new BoundingBox(_Points[0], _Points[0]);
            for (int i = 1; i < _Points.Count; i++)
            {
                bbox.Min.X = _Points[i].X < bbox.Min.X ? _Points[i].X : bbox.Min.X;
                bbox.Min.Y = _Points[i].Y < bbox.Min.Y ? _Points[i].Y : bbox.Min.Y;
                bbox.Max.X = _Points[i].X > bbox.Max.X ? _Points[i].X : bbox.Max.X;
                bbox.Max.Y = _Points[i].Y > bbox.Max.Y ? _Points[i].Y : bbox.Max.Y;
            }
            return bbox;
        }

        /// <summary>
        /// Return a copy of this geometry
        /// </summary>
        /// <returns>Copy of Geometry</returns>
        public new MultiPoint Clone()
        {
            MultiPoint geoms = new MultiPoint();
            for (int i = 0; i < _Points.Count; i++)
                geoms.Points.Add(_Points[i].Clone());
            return geoms;
        }

        /// <summary>
        /// Gets an enumerator for enumerating the geometries in the GeometryCollection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Geometry> GetEnumerator()
        {
            foreach (Point p in _Points)
                yield return p;
        }
    }
}