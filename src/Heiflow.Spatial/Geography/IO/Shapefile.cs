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
using System.Diagnostics;

namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Class that represents a shape file header record.
    /// </summary>
    public class ShapefileHeader
    {
        private int _fileCode = Shapefile.ShapefileId;
        private int _fileLength = -1;
        private int _version = 1000;
        private ShapeGeometryType _shapeType = ShapeGeometryType.NullShape;
        private BoundingBox _bounds;

        /// <summary>
        /// Initializes a new instance of the ShapefileHeader class with values read in from the stream.
        /// </summary>
        /// <remarks>Reads the header information from the stream.</remarks>
        /// <param name="shpBinaryReader">BigEndianBinaryReader stream to the shapefile.</param>
        public ShapefileHeader(BigEndianBinaryReader shpBinaryReader)
        {
            if (shpBinaryReader == null)
                throw new ArgumentNullException("shpBinaryReader");

            _fileCode = shpBinaryReader.ReadInt32BE();
            if (_fileCode != Shapefile.ShapefileId)
                throw new Exception("The first four bytes of this file indicate this is not a shape file.");

            // skip 5 unsed bytes
            shpBinaryReader.ReadInt32BE();
            shpBinaryReader.ReadInt32BE();
            shpBinaryReader.ReadInt32BE();
            shpBinaryReader.ReadInt32BE();
            shpBinaryReader.ReadInt32BE();

            _fileLength = shpBinaryReader.ReadInt32BE();

            _version = shpBinaryReader.ReadInt32();
            Debug.Assert(_version == 1000, "Shapefile version", String.Format("Expecting only one version (1000), but got {0}", _version));
            int shapeType = shpBinaryReader.ReadInt32();
            _shapeType = (ShapeGeometryType)Enum.Parse(typeof(ShapeGeometryType), shapeType.ToString(), true);

            //read in and store the bounding box
            double[] coords = new double[4];
            for (int i = 0; i < 4; i++)
                coords[i] = shpBinaryReader.ReadDouble();
            _bounds = new BoundingBox(coords[0], coords[2], coords[1], coords[3]);

            // skip z and m bounding boxes.
            for (int i = 0; i < 4; i++)
                shpBinaryReader.ReadDouble();
        }

        /// <summary>
        /// Initializes a new instance of the ShapefileHeader class.
        /// </summary>
        public ShapefileHeader() { }

        /// <summary>
        /// Gets and sets the bounds of the shape file.
        /// </summary>
        public BoundingBox Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
            }
        }

        /// <summary>
        /// Gets and sets the shape file type i.e. polygon, point etc...
        /// </summary>
        public ShapeGeometryType ShapeType
        {
            get
            {
                return _shapeType;
            }
            set
            {
                _shapeType = value;
            }
        }

        /// <summary>
        /// Gets and sets the shapefile version.
        /// </summary>
        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        /// <summary>
        /// Gets and sets the length of the shape file in words.
        /// </summary>
        public int FileLength
        {
            get
            {
                return _fileLength;
            }
            set
            {
                _fileLength = value;
            }
        }

        /// <summary>
        /// Writes a shapefile header to the given stream;
        /// </summary>
        /// <param name="file">The binary writer to use.</param>
        public void Write(BigEndianBinaryWriter file)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (_fileLength == -1)
                throw new InvalidOperationException("The header properties need to be set before writing the header record.");
            int pos = 0;
            file.WriteIntBE(_fileCode);
            pos += 4;
            for (int i = 0; i < 5; i++)
            {
                file.WriteIntBE(0);//Skip unused part of header
                pos += 4;
            }
            file.WriteIntBE(_fileLength);
            pos += 4;
            file.Write(_version);
            pos += 4;

            file.Write((int)_shapeType);

            pos += 4;
            // Write the bounding box
            file.Write(_bounds.MinX);
            file.Write(_bounds.MinY);
            file.Write(_bounds.MaxX);
            file.Write(_bounds.MaxY);
            pos += 8 * 4;

            // Skip remaining unused bytes
            for (int i = 0; i < 4; i++)
            {
                file.Write(0.0); // Skip unused part of header
                pos += 8;
            }
        }
    }

    /// <summary>
    /// This class is used to read and write ESRI Shapefiles.
    /// </summary>
    public class Shapefile
    {
        public const int ShapefileId = 9994;
        public const int Version = 1000;

        /// <summary>
        /// Given a geomtery object, returns the equilivent shape file type.
        /// </summary>
        /// <param name="geom">A Geometry object.</param>
        /// <returns>The equilivent for the geometry object.</returns>
        public static ShapeGeometryType GetShapeType(IGeometry geom)
        {
            if (geom is Point)
                return ShapeGeometryType.Point;
            if (geom is Polygon)
                return ShapeGeometryType.Polygon;
            if (geom is MultiPolygon)
                return ShapeGeometryType.Polygon;
            if (geom is LineString)
                return ShapeGeometryType.LineString;
            if (geom is MultiLineString)
                return ShapeGeometryType.LineString;
            if (geom is MultiPoint)
                return ShapeGeometryType.MultiPoint;
            return ShapeGeometryType.NullShape;
        }

        /// <summary>
        /// Returns the appropriate class to convert a shaperecord to an OGIS geometry given the type of shape.
        /// </summary>
        /// <param name="type">The shapefile type.</param>
        /// <returns>An instance of the appropriate handler to convert the shape record to a Geometry object.</returns>
        public static ShapeHandler GetShapeHandler(ShapeGeometryType type)
        {
            switch (type)
            {
                case ShapeGeometryType.Point:
                case ShapeGeometryType.PointM:
                case ShapeGeometryType.PointZ:
                case ShapeGeometryType.PointZM:
                    return new PointHandler();

                case ShapeGeometryType.Polygon:
                case ShapeGeometryType.PolygonM:
                case ShapeGeometryType.PolygonZ:
                case ShapeGeometryType.PolygonZM:
                    return new PolygonHandler();

                case ShapeGeometryType.LineString:
                case ShapeGeometryType.LineStringM:
                case ShapeGeometryType.LineStringZ:
                case ShapeGeometryType.LineStringZM:
                    return new MultiLineHandler();

                case ShapeGeometryType.MultiPoint:
                case ShapeGeometryType.MultiPointM:
                case ShapeGeometryType.MultiPointZ:
                case ShapeGeometryType.MultiPointZM:
                    return new MultiPointHandler();

                default:
                    return null;
            }
        }
    }
}
