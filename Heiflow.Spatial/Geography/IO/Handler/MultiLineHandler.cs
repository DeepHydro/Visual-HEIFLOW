// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

using System.IO;
namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Converts a Shapefile multi-line to a OGIS LineString/MultiLineString.
    /// </summary>
    public class MultiLineHandler : ShapeHandler
    {
        /// <summary>
        /// Returns the ShapeType the handler handles.
        /// </summary>
        public override ShapeGeometryType ShapeType
        {
            get { return ShapeGeometryType.LineString; }
        }

        /// <summary>
        /// Writes to the given stream the equilivent shape file record given a Geometry object.
        /// </summary>
        /// <param name="geometry">The geometry object to write.</param>
        /// <param name="file">The stream to write to.</param>
        /// <param name="geometryFactory">The geometry factory to use.</param>
        public override void Write(Geometry geometry, BinaryWriter file)
        {
            if (geometry is MultiLineString)
            {

                // Slow and maybe not useful...
                // if (!geometry.IsValid)
                // Trace.WriteLine("Invalid multipoint being written.");
                WriteMultiLine((geometry as MultiLineString), file);
            }
            else if (geometry is Point)
            {
                MultiLineString ml = new MultiLineString();
                ml.LineStrings.Add((geometry as LineString));
                WriteMultiLine(ml, file);
            }

        }

        private void WriteMultiLine(MultiLineString multi, BinaryWriter file)
        {
            file.Write((int)ShapeType);

            BoundingBox box = multi.GetBoundingBox();
            file.Write(box.MinX);
            file.Write(box.MinY);
            file.Write(box.MaxX);
            file.Write(box.MaxY);

            int numParts = multi.NumGeometries;
            int numPoints = multi.NumPoints;

            file.Write(numParts);
            file.Write(numPoints);

            // Write the offsets
            int offset = 0;
            for (int i = 0; i < numParts; i++)
            {
                LineString g = multi[i];
                file.Write(offset);
                offset = offset + g.NumPoints;
            }


            foreach (LineString ls in multi.LineStrings)
            {
                foreach (Point external in ls.Vertices)
                {
                    file.Write(external.X);
                    file.Write(external.Y);
                }
            }
        }

        /// <summary>
        /// Gets the length in bytes the Geometry will need when written as a shape file record.
        /// </summary>
        /// <param name="geometry">The Geometry object to use.</param>
        /// <returns>The length in bytes the Geometry will use when represented as a shape file record.</returns>
        public override int GetLength(Geometry geometry)
        {
            MultiLineString multi = (MultiLineString)geometry;
            int numParts = GetNumParts(geometry);
            return (22 + (2 * numParts) + multi.NumPoints * 8); // 22 => shapetype(2) + bbox(4*4) + numparts(2) + numpoints(2)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private int GetNumParts(IGeometry geometry)
        {
            int numParts = 1;
            if (geometry is MultiLineString)
                numParts = ((MultiLineString)geometry).LineStrings.Count;
            return numParts;
        }
    }
}
