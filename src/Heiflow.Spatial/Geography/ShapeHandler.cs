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
using System.IO;
 
namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Abstract class that defines the interfaces that other 'Shape' handlers must implement.
    /// </summary>
    public abstract class ShapeHandler
    {
        protected int bbindex = 0;
        protected double[] bbox;
        protected ShapeGeometryType type;
        protected Geometry geom;

        /// <summary>
        /// Returns the ShapeType the handler handles.
        /// </summary>
        public abstract ShapeGeometryType ShapeType { get; }


        /// <summary>
        /// Writes to the given stream the equilivent shape file record given a Geometry object.
        /// </summary>
        /// <param name="geometry">The geometry object to write.</param>
        /// <param name="file">The stream to write to.</param>
        /// <param name="geometryFactory">The geometry factory to use.</param>
        public abstract void Write(Geometry geometry, BinaryWriter file);

        /// <summary>
        /// Gets the length in bytes the Geometry will need when written as a shape file record.
        /// </summary>
        /// <param name="geometry">The Geometry object to use.</param>
        /// <returns>The length in 16bit words the Geometry will use when represented as a shape file record.</returns>
        public abstract int GetLength(Geometry geometry);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool HasZValue()
        {
            return HasZValue(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        public static bool HasZValue(ShapeGeometryType shapeType)
        {
            return shapeType == ShapeGeometryType.PointZ ||
                    shapeType == ShapeGeometryType.PointZM ||
                    shapeType == ShapeGeometryType.LineStringZ ||
                    shapeType == ShapeGeometryType.LineStringZM ||
                    shapeType == ShapeGeometryType.PolygonZ ||
                    shapeType == ShapeGeometryType.PolygonZM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool HasMValue()
        {
            return HasMValue(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        public static bool HasMValue(ShapeGeometryType shapeType)
        {
            return shapeType == ShapeGeometryType.PointM ||
                    shapeType == ShapeGeometryType.PointZM ||
                    shapeType == ShapeGeometryType.LineStringM ||
                    shapeType == ShapeGeometryType.LineStringZM ||
                    shapeType == ShapeGeometryType.PolygonM ||
                    shapeType == ShapeGeometryType.PolygonZM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsPoint()
        {
            return IsPoint(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        public static bool IsPoint(ShapeGeometryType shapeType)
        {
            return shapeType == ShapeGeometryType.Point ||
                   shapeType == ShapeGeometryType.PointZ ||
                   shapeType == ShapeGeometryType.PointM ||
                   shapeType == ShapeGeometryType.PointZM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsMultiPoint()
        {
            return IsMultiPoint(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        public static bool IsMultiPoint(ShapeGeometryType shapeType)
        {
            return shapeType == ShapeGeometryType.MultiPoint ||
                   shapeType == ShapeGeometryType.MultiPointZ ||
                   shapeType == ShapeGeometryType.MultiPointM ||
                   shapeType == ShapeGeometryType.MultiPointZM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsLineString()
        {
            return IsLineString(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        public static bool IsLineString(ShapeGeometryType shapeType)
        {
            return shapeType == ShapeGeometryType.LineString ||
                   shapeType == ShapeGeometryType.LineStringZ ||
                   shapeType == ShapeGeometryType.LineStringM ||
                   shapeType == ShapeGeometryType.LineStringZM;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <returns></returns>
        protected bool IsPolygon()
        {
            return IsPolygon(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeType"></param>
        /// <returns></returns>
        public static bool IsPolygon(ShapeGeometryType shapeType)
        {
            return shapeType == ShapeGeometryType.Polygon ||
                   shapeType == ShapeGeometryType.PolygonZ ||
                   shapeType == ShapeGeometryType.PolygonM ||
                   shapeType == ShapeGeometryType.PolygonZM;
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <returns></returns>
        protected int GetBoundingBoxLength()
        {
            bbindex = 0;
            int bblength = 4;
            if (HasZValue())
                bblength += 2;
            if (HasMValue())
                bblength += 2;
            return bblength;
        }
    }
}
