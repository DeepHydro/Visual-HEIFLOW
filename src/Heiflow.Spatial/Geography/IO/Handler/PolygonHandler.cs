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
using System.Collections.Generic;


namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Converts a Shapefile point to a OGIS Polygon.
    /// </summary>
    public class PolygonHandler : ShapeHandler
    {
        /// <summary>
        /// The ShapeType this handler handles.
        /// </summary>
        public override ShapeGeometryType ShapeType
        {
            get { return ShapeGeometryType.Polygon; }
        }

     

        /// <summary>
        /// Writes a Geometry to the given binary wirter.
        /// </summary>
        /// <param name="geometry">The geometry to write.</param>
        /// <param name="file">The file stream to write to.</param>
        /// <param name="geometryFactory">The geometry factory to use.</param>
        public override void Write(Geometry geometry, BinaryWriter file)
        {
            // This check seems to be not useful and slow the operations...
            // if (!geometry.IsValid)    
            // Trace.WriteLine("Invalid polygon being written.");
            if (geometry is MultiPolygon)
            {
                WriteMutliPolygon((geometry as MultiPolygon), file);
            }
            else if  (geometry is Polygon)
            {
                MultiPolygon multi = new MultiPolygon();
                multi.Polygons.Add((geometry as Polygon));
                WriteMutliPolygon(multi, file);
            }
        }

        private void WriteMutliPolygon(MultiPolygon multi, BinaryWriter file)
        {

            file.Write((int)ShapeType);

            BoundingBox bounds = multi.GetBoundingBox();

            file.Write(bounds.MinX);
            file.Write(bounds.MinY);
            file.Write(bounds.MaxX);
            file.Write(bounds.MaxY);

            int numParts = GetNumParts(multi);
            int numPoints = multi.NumPoints;
            file.Write(numParts);
            file.Write(numPoints);

            // write the offsets to the points
            int offset = 0;
            for (int part = 0; part < multi.NumGeometries; part++)
            {
                // offset to the shell points
                Polygon polygon = (Polygon)multi[part];
                file.Write(offset);
                offset = offset + polygon.ExteriorRing.NumPoints;

                // offstes to the holes
                foreach (LinearRing ring in polygon.InteriorRings)
                {
                    file.Write(offset);
                    offset = offset + ring.NumPoints;
                }
            }

            // write the points 
            for (int part = 0; part < multi.NumGeometries; part++)
            {
                Polygon poly = (Polygon)multi[part];
                IList<Point> points = poly.ExteriorRing.Vertices;
                WriteCoords(points, file);
                foreach (LinearRing ring in poly.InteriorRings)
                {
                    WriteCoords(ring.Vertices, file);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="file"></param>
        /// <param name="geometryFactory"></param>
        private void WriteCoords(IList<Heiflow.Spatial.Geography.Point> points, BinaryWriter file)
        {
            foreach (Heiflow.Spatial.Geography.Point point in points)
            {
                // external = geometryFactory.PrecisionModel.ToExternal(point);
                //external = point;
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
            int numParts = GetNumParts(geometry);
            if(geometry is MultiPolygon)
                return (22 + (2 * numParts) + (geometry as MultiPolygon).NumPoints * 8); // 22 => shapetype(2) + bbox(4*4) + numparts(2) + numpoints(2)
            else
                return (22 + (2 * numParts) + (geometry as Polygon).NumPoints * 8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private int GetNumParts(Geometry geometry)
        {
            int numParts = 0;
            if (geometry is MultiPolygon)
            {
                MultiPolygon mpoly = geometry as MultiPolygon;
                foreach (Polygon poly in mpoly)
                    numParts = numParts + poly.InteriorRings.Count + 1;
            }
            else if (geometry is Polygon)
                numParts = ((Polygon)geometry).InteriorRings.Count + 1;
            else throw new InvalidOperationException("Should not get here.");
            return numParts;
        }

    }
}
