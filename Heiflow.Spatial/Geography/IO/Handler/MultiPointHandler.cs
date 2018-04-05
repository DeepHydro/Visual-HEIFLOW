// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

using System.IO;
using System;

namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Converts a Shapefile point to a OGIS Polygon.
    /// </summary>
    public class MultiPointHandler : ShapeHandler
    {


        /// <summary>
        /// The ShapeType this handler handles.
        /// </summary>
        public override ShapeGeometryType ShapeType
        {
            get { return ShapeGeometryType.MultiPoint; }
        }

      
        /// <summary>
        /// Writes a Geometry to the given binary wirter.
        /// </summary>
        /// <param name="geometry">The geometry to write.</param>
        /// <param name="file">The file stream to write to.</param>
        /// <param name="geometryFactory">The geometry factory to use.</param>
        public override void Write(Geometry geometry, BinaryWriter file)
        {
            //if (!(geometry is MultiPoint))
            //    throw new ArgumentException("Geometry Type error: MultiPoint expected, but the type retrieved is " + geometry.GetType().Name);

            if (geometry is MultiPoint)
            {

                // Slow and maybe not useful...
                // if (!geometry.IsValid)
                // Trace.WriteLine("Invalid multipoint being written.");
                WriteMultiPoint((geometry as MultiPoint), file);
            }
            else if (geometry is Point)
            {
                MultiPoint ml = new MultiPoint();
                ml.Points.Add((geometry as Point));
                WriteMultiPoint(ml, file);
            }
        }

        private void WriteMultiPoint(MultiPoint mpoint, BinaryWriter file)
        {
            file.Write((int)ShapeType);

            BoundingBox bounds = mpoint.GetBoundingBox();

            file.Write(bounds.MinX);
            file.Write(bounds.MinY);
            file.Write(bounds.MaxX);
            file.Write(bounds.MaxY);

            int numPoints = mpoint.NumPoints;
            file.Write(numPoints);

            // write the points 
            for (int i = 0; i < numPoints; i++)
            {
                Point point = mpoint[i];
                file.Write(point.X);
                file.Write(point.Y);
            }
        }
        /// <summary>
        /// Gets the length of the shapefile record using the geometry passed in.
        /// </summary>
        /// <param name="geometry">The geometry to get the length for.</param>
        /// <returns>The length in bytes this geometry is going to use when written out as a shapefile record.</returns>
        public override int GetLength(Geometry geometry)
        {

            return (20 + (geometry as MultiPoint).NumPoints * 8); // 20 => shapetype(2) + bbox (4*4) + numpoints
        }
    }
}
