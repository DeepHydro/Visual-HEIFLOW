// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.IO;
using Heiflow.Spatial.Geography;

namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Contains methods for writing a single <c>Geometry</c> in binary ESRI shapefile format.
    /// </summary>
    public class ShapeWriter
    {
        /// <summary>
        /// Standard byte size for each complex point.
        /// Each complex point (LineString, Polygon, ...) contains
        ///     4 bytes for ShapeTypes and
        ///     32 bytes for Boundingbox.      
        /// </summary>
        protected const int InitCount = 36;

        /// <summary> 
        /// Creates a <coordinate>ShapeWriter</coordinate> that creates objects using a basic GeometryFactory.
        /// </summary>
        public ShapeWriter() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="writer"></param>
        public void Write(Point point, BinaryWriter writer)
        {
            writer.Write((int)ShapeGeometryType.Point);
            writer.Write((double)point.X);
            writer.Write((double)point.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="writer"></param>
        public void Write(LineString lineString, BinaryWriter writer)
        {
            writer.Write((int)ShapeGeometryType.LineString);

            // Write BoundingBox            
            WriteBoundingBox(lineString, writer);

            // Write NumParts and NumPoints
            writer.Write((int)1);
            writer.Write((int)lineString.NumPoints);

            // Write IndexParts
            writer.Write((int)0);

            // Write Coordinates
            for (int i = 0; i < lineString.NumPoints; i++)
                Write(lineString.Vertices[i], writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="writer"></param>
        public void Write(Polygon polygon, BinaryWriter writer)
        {
            writer.Write((int)ShapeGeometryType.Polygon);

            // Write BoundingBox            
            WriteBoundingBox(polygon, writer);

            // Write NumParts and NumPoints            
            writer.Write((int)(polygon.NumInteriorRing + 1));
            writer.Write((int)polygon.NumPoints);

            // Write IndexParts
            int count = 0;
            writer.Write((int)count);
            if (polygon.NumInteriorRing != 0)
            {
                // Write external shell index
                count += polygon.ExteriorRing.NumPoints;
                writer.Write((int)count);
                for (int i = 1; i < polygon.NumInteriorRing; i++)
                {
                    // Write public holes index
                    count += polygon.InteriorRings[i - 1].NumPoints;
                    writer.Write((int)count);
                }
            }

            // Write Coordinates
            for (int i = 0; i < polygon.NumPoints; i++)
                Write(polygon.ExteriorRing.Vertices[i], writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <param name="writer"></param>
        public void Write(MultiPoint multiPoint, BinaryWriter writer)
        {
            writer.Write((int)ShapeGeometryType.MultiPoint);

            // Write BoundingBox            
            WriteBoundingBox(multiPoint, writer);

            // Write NumPoints            
            writer.Write((int)multiPoint.NumPoints);

            // Write Coordinates
            for (int i = 0; i < multiPoint.NumPoints; i++)
                Write(multiPoint.Points[i], writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <param name="writer"></param>
        public void Write(MultiLineString multiLineString, BinaryWriter writer)
        {
            writer.Write((int)ShapeGeometryType.LineString);

            // Write BoundingBox            
            WriteBoundingBox(multiLineString, writer);

            // Write NumParts and NumPoints
            writer.Write((int)multiLineString.NumGeometries);
            writer.Write((int)multiLineString.NumPoints);

            // Write IndexParts
            int count = 0;
            writer.Write((int)count);

            // Write linestrings index                                
            for (int i = 0; i < multiLineString.NumGeometries; i++)
            {
                // Write public holes index
                count += multiLineString.LineStrings[i].NumPoints;
                if (count == multiLineString.NumPoints)
                    break;
                writer.Write((int)count);
            }

            // Write Coordinates
            //for (int i = 0; i < multiLineString.NumPoints; i++)
            //    Write(multiLineString.[i], writer);
            foreach (LineString ls in multiLineString.LineStrings)
            {
                Write(ls, writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <param name="writer"></param>
        public void Write(MultiPolygon multiPolygon, BinaryWriter writer)
        {
            writer.Write((int)ShapeGeometryType.Polygon);

            // Write BoundingBox            
            WriteBoundingBox(multiPolygon, writer);

            // Write NumParts and NumPoints
            int numParts = multiPolygon.NumGeometries;              // Exterior rings count
            for (int i = 0; i < multiPolygon.NumGeometries; i++)    // Adding interior rings count            
                numParts += ((Polygon)multiPolygon.Geometry(i)).NumInteriorRing;

            writer.Write((int)numParts);
            writer.Write((int)multiPolygon.NumPoints);

            // Write IndexParts
            int count = 0;
            writer.Write((int)count);

            for (int i = 0; i < multiPolygon.NumGeometries; i++)
            {
                Polygon polygon = (Polygon)multiPolygon.Geometry(i);
                LineString shell = polygon.ExteriorRing;
                count += shell.NumPoints;
                if (count == multiPolygon.NumPoints)
                    break;
                writer.Write((int)count);
                for (int j = 0; j < polygon.NumInteriorRing; j++)
                {
                    LineString hole = (LineString)polygon.InteriorRings[j];
                    count += hole.NumPoints;
                    if (count == multiPolygon.NumPoints)
                        break;
                    writer.Write((int)count);
                }
              
            }

            // Write Coordinates
            //for (int i = 0; i < multiPolygon.NumPoints; i++)
            //    Write(multiPolygon.Coordinates[i], writer);
            foreach (Polygon p in multiPolygon)
            {
                Write(p, writer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="writer"></param>
        public void WriteBoundingBox(Geometry geometry, BinaryWriter writer)
        {
            BoundingBox boundingBox = geometry.GetBoundingBox();
            writer.Write((double)boundingBox.MinX);
            writer.Write((double)boundingBox.MinY);
            writer.Write((double)boundingBox.MaxX);
            writer.Write((double)boundingBox.MaxY);
        }

        /// <summary>
        /// Sets correct length for Byte Stream.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public byte[] GetBytes(Geometry geometry)
        {
            return new byte[GetBytesLength(geometry)];
        }

        /// <summary>
        /// Return correct length for Byte Stream.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public int GetBytesLength(Geometry geometry)
        {
            if (geometry is Point)
                return SetByteStreamLength(geometry as Point);
            else if (geometry is LineString)
                return SetByteStreamLength(geometry as LineString);
            else if (geometry is Polygon)
                return SetByteStreamLength(geometry as Polygon);
            else if (geometry is MultiPoint)
                return SetByteStreamLength(geometry as MultiPoint);
            else if (geometry is MultiLineString)
                return SetByteStreamLength(geometry as MultiLineString);
            else if (geometry is MultiPolygon)
                return SetByteStreamLength(geometry as MultiPolygon);
            else if (geometry is IGeometryCollection)
                throw new NotSupportedException("GeometryCollection not supported!");
            else throw new ArgumentException("ShouldNeverReachHere!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <returns></returns>
        protected int SetByteStreamLength(MultiPolygon multiPolygon)
        {
            int numParts = multiPolygon.NumGeometries;               // Exterior rings count            
            foreach (Polygon polygon in multiPolygon)    // Adding interior rings count            
                numParts += polygon.InteriorRings.Count;
            int numPoints = multiPolygon.NumPoints;
            return CalculateLength(numParts, numPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <returns></returns>
        protected int SetByteStreamLength(MultiLineString multiLineString)
        {
            int numParts = multiLineString.NumGeometries;
            int numPoints = multiLineString.NumPoints;
            return CalculateLength(numParts, numPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <returns></returns>
        protected int SetByteStreamLength(MultiPoint multiPoint)
        {
            int numPoints = multiPoint.NumPoints;
            return CalculateLength(numPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        protected int SetByteStreamLength(Polygon polygon)
        {
            int numParts = polygon.InteriorRings.Count + 1;
            int numPoints = polygon.NumPoints;
            return CalculateLength(numParts, numPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        protected int SetByteStreamLength(LineString lineString)
        {
            int numPoints = lineString.NumPoints;
            return CalculateLength(1, numPoints);   // ASSERT: IndexParts.Length == 1;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected int SetByteStreamLength(Point point)
        {
            return 20;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numParts"></param>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        private static int CalculateLength(int numParts, int numPoints)
        {
            int count = InitCount;
            count += 8;                         // NumParts and NumPoints
            count += 4 * numParts;
            count += 8 * 2 * numPoints;
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        private static int CalculateLength(int numPoints)
        {
            int count = InitCount;
            count += 4;                         // NumPoints
            count += 8 * 2 * numPoints;
            return count;
        }
    }

    /// <summary>
    /// Feature type enumeration
    /// </summary>
    public enum ShapeGeometryType
    {
        /// <summary>
        /// Null Shape
        /// </summary>
        NullShape = 0,

        /// <summary>
        /// Point
        /// </summary>
        Point = 1,

        /// <summary>
        /// LineString
        /// </summary>
        LineString = 3,

        /// <summary>
        /// Polygon
        /// </summary>
        Polygon = 5,

        /// <summary>
        /// MultiPoint
        /// </summary>
        MultiPoint = 8,

        /// <summary>
        /// PointMZ
        /// </summary>
        PointZM = 11,

        /// <summary>
        /// PolyLineMZ
        /// </summary>
        LineStringZM = 13,

        /// <summary>
        /// PolygonMZ
        /// </summary>
        PolygonZM = 15,

        /// <summary>
        /// MultiPointMZ
        /// </summary>
        MultiPointZM = 18,

        /// <summary>
        /// PointM
        /// </summary>
        PointM = 21,

        /// <summary>
        /// LineStringM
        /// </summary>
        LineStringM = 23,

        /// <summary>
        /// PolygonM
        /// </summary>
        PolygonM = 25,

        /// <summary>
        /// MultiPointM
        /// </summary>
        MultiPointM = 28,

        /// <summary>
        /// MultiPatch
        /// </summary>
        MultiPatch = 31,

        /// <summary>
        /// PointZ
        /// </summary>
        PointZ = 9,

        /// <summary>
        /// LineStringZ
        /// </summary>
        LineStringZ = 10,

        /// <summary>
        /// PolygonZ
        /// </summary>
        PolygonZ = 19,

        /// <summary>
        /// MultiPointZ
        /// </summary>
        MultiPointZ = 20,
    }        
}
