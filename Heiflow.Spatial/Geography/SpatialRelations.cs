﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;

namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// Class defining a set of named spatial relationship operators for geometric shape objects.
    /// </summary>
    public class SpatialRelations
    {
        /// <summary>
        /// Returns true if otherGeometry is wholly contained within the source geometry. This is the same as
        /// reversing the primary and comparison shapes of the Within operation.
        /// </summary>
        /// <param name="sourceGeometry"></param>
        /// <param name="otherGeometry"></param>
        /// <returns>True if otherGeometry is wholly contained within the source geometry.</returns>
        public static bool Contains(Geometry sourceGeometry, Geometry otherGeometry)
        {
            return (otherGeometry.Within(sourceGeometry));
        }

        /// <summary>
        /// Returns true if the intersection of the two geometries results in a geometry whose dimension is less than
        /// the maximum dimension of the two geometries and the intersection geometry is not equal to either.
        /// geometry.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static bool Crosses(Geometry g1, Geometry g2)
        {
            Geometry g = g2.Intersection(g1);
            return (g.Intersection(g1).Dimension < Math.Max(g1.Dimension, g2.Dimension) && !g.Equals(g1) &&
                    !g.Equals(g2));
        }

        /// <summary>
        /// Returns true if otherGeometry is disjoint from the source geometry.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static bool Disjoint(Geometry g1, Geometry g2)
        {
            return !g2.Intersects(g1);
        }

        /// <summary>
        /// Returns true if otherGeometry is of the same type and defines the same point set as the source geometry.
        /// </summary>
        /// <param name="g1">source geometry</param>
        /// <param name="g2">other Geometry</param>
        /// <returns></returns>
        public static bool Equals(Geometry g1, Geometry g2)
        {
            if (g1 == null && g2 == null)
                return true;
            if (g1 == null || g2 == null)
                return false;
            if (g1.GetType() != g2.GetType())
                return false;
            if (g1 is Point)
                return (g1 as Point).Equals(g2 as Point);
            else if (g1 is LineString)
                return (g1 as LineString).Equals(g2 as LineString);
            else if (g1 is Polygon)
                return (g1 as Polygon).Equals(g2 as Polygon);
            else if (g1 is MultiPoint)
                return (g1 as MultiPoint).Equals(g2 as MultiPoint);
            else if (g1 is MultiLineString)
                return (g1 as MultiLineString).Equals(g2 as MultiLineString);
            else if (g1 is MultiPolygon)
                return (g1 as MultiPolygon).Equals(g2 as MultiPolygon);
            else
                throw new ArgumentException("The method or operation is not implemented on this geometry type.");
        }


        /// <summary>
        /// Returns true if there is any intersection between the two geometries.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static bool Intersects(Geometry g1, Geometry g2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the intersection of the two geometries results in an object of the same dimension as the
        /// input geometries and the intersection geometry is not equal to either geometry.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static bool Overlaps(Geometry g1, Geometry g2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the only points in common between the two geometries lie in the union of their boundaries.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static bool Touches(Geometry g1, Geometry g2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the primary geometry is wholly contained within the comparison geometry.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static bool Within(Geometry g1, Geometry g2)
        {
            return g1.Contains(g2);
        }
    }
}