// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// Interface for a GeometryCollection. A GeometryCollection is a collection of 1 or more geometries.
    /// </summary>
    public interface IGeometryCollection : IGeometry
    {
        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        int NumGeometries { get; }

        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="N">Geometry index</param>
        /// <returns>Geometry at index N</returns>
        Geometry Geometry(int N);
    }
}