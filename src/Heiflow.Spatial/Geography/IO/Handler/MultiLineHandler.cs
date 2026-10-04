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
