// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.IO;

using Heiflow.Spatial.Geography;
using System.Text;

namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Converts a Shapefile point to a OGIS Point.
    /// </summary>
    public class PointHandler : ShapeHandler
    {
        /// <summary>
        /// The shape type this handler handles (point).
        /// </summary>
        public override ShapeGeometryType ShapeType
        {
            get { return ShapeGeometryType.Point; }
        }


        /// <summary>
        /// Writes to the given stream the equilivent shape file record given a Geometry object.
        /// </summary>
        /// <param name="geometry">The geometry object to write.</param>
        /// <param name="file">The stream to write to.</param>
        /// <param name="geometryFactory">The geometry factory to use.</param>
        public override void Write(Geometry geometry, BinaryWriter file)
        {
            file.Write((int)this.ShapeType);
            Point external = geometry as Point;
            file.Write(external.X);
            file.Write(external.Y);
        }

        /// <summary>
        /// Gets the length in bytes the Geometry will need when written as a shape file record.
        /// </summary>
        /// <param name="geometry">The Geometry object to use.</param>
        /// <returns>The length in bytes the Geometry will use when represented as a shape file record.</returns>
        public override int GetLength(Geometry geometry)
        {
            return 10; // 10 => shapetyppe(2)+ xy(4*2)
        }
    }
}
