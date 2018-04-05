// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
