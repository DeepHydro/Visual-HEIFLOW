// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// Extension methods for geodesic calculations.
    /// </summary>
    public static class Geodesic
    {
        public const double EarthRadius = 6378137; //meters. Change to miles to return all values in miles instead
        /// <summary>
        /// The coefficient of converting degree to radian
        /// </summary>
        public const double D2RCoeff = Math.PI / 180;

        /// <summary>
        /// The coefficient of converting radian  to degree
        /// </summary>
        public const double R2DCoeff = 180 / Math.PI;

        /// <summary>
        /// Gets the distance between two points in meters.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static double GetSphericalDistance(this Point start, Point end)
        {
            double lon1 = start.X / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            return 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((lat1 - lat2) / 2)), 2) +
             Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon1 - lon2) / 2), 2))) * EarthRadius;
        }

        /// <summary>
        /// Returns a polyline with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="distKM">Radius in kilometers.</param>
        // <returns></returns>
        public static LinearRing GetRadius(this Point center, double distKM)
        {
            LinearRing line = new LinearRing();      
            for (int i = 0; i < 360; i++)
            {
                //double angle = i / 180.0 * Math.PI;
                Point p = GetPointFromHeading(center, distKM, i);

                if (line.Vertices.Count > 0)
                {
                    Point lastPoint = line.Vertices[line.Vertices.Count - 1];
                    int sign = Math.Sign(p.X);
                    if (Math.Abs(p.X - lastPoint.X) > 180)
                    {   //We crossed the date line
                        double lat = LatitudeAtLongitude(lastPoint, p, sign * -180);
                        line.Vertices.Add(new Point(sign * -180, lat));
                        line.Vertices.Add(new Point(sign * 180, lat));
                    }
                }
                line.Vertices.Add(p);
            }
            return line;
        }


        /// <summary>
        /// Gets the shortest path line between two points. THe line will be following the great
        /// circle described by the two points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Curve GetGeodesicLine(this Point start, Point end)
        {
            LineString line = new LineString();
            if (Math.Abs(end.X - start.X) <= 180) // Doesn't cross dateline 
            {
                MultiPoint pnts = GetGeodesicPoints(start, end);
                foreach (Point p in pnts)
                {
                    line.Vertices.Add(p);
                }
            }
            else
            {
                double lon1 = start.X / 180 * Math.PI;
                double lon2 = end.X / 180 * Math.PI;
                double lat1 = start.Y / 180 * Math.PI;
                double lat2 = end.Y / 180 * Math.PI;
                double latA = LatitudeAtLongitude(lat1, lon1, lat2, lon2, Math.PI) / Math.PI * 180;
                //double latB = LatitudeAtLongitude(lat1, lon1, lat2, lon2, -180) / Math.PI * 180;
                MultiPoint pnts = GetGeodesicPoints(start, new Point(start.X < 0 ? -180 : 180, latA));
                foreach (Point p in pnts)
                {
                    line.Vertices.Add(p);
                }
                pnts= GetGeodesicPoints(new Point(start.X < 0 ? 180 : -180, latA), end);
                 foreach (Point p in pnts)
                {
                    line.Vertices.Add(p);
                }            }
            return line;

        }

        private static MultiPoint GetGeodesicPoints(Point start, Point end)
        {
            double lon1 = start.X / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            double dX = end.X - start.X;
            int points = (int)Math.Floor(Math.Abs(dX));
            dX = lon2 - lon1;
            MultiPoint pnts = new MultiPoint();
            pnts.Points.Add(start);
            for (int i = 1; i < points; i++)
            {
                double lon = lon1 + dX / points * i;
                double lat = LatitudeAtLongitude(lat1, lon1, lat2, lon2, lon);
                pnts.Points.Add(new Point(lon / Math.PI * 180, lat / Math.PI * 180));
            }
            pnts.Points.Add(end);
            return pnts;
        }

        /// <summary>
        /// Gets the latitude at a specific longitude for a great circle defined by p1 and p2.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="lon">The longitude in degrees.</param>
        /// <returns></returns>
        private static double LatitudeAtLongitude(Point p1, Point p2, double lon)
        {
            double lon1 = p1.X / 180 * Math.PI;
            double lon2 = p2.X / 180 * Math.PI;
            double lat1 = p1.Y / 180 * Math.PI;
            double lat2 = p2.Y / 180 * Math.PI;
            lon = lon / 180 * Math.PI;
            return LatitudeAtLongitude(lat1, lon1, lat2, lon2, lon) / Math.PI * 180;
        }

        /// <summary>
        /// Gets the latitude at a specific longitude for a great circle defined by lat1,lon1 and lat2,lon2.
        /// </summary>
        /// <param name="lat1">The start latitude in radians.</param>
        /// <param name="lon1">The start longitude in radians.</param>
        /// <param name="lat2">The end latitude in radians.</param>
        /// <param name="lon2">The end longitude in radians.</param>
        /// <param name="lon">The longitude in radians for where the latitude is.</param>
        /// <returns></returns>
        private static double LatitudeAtLongitude(double lat1, double lon1, double lat2, double lon2, double lon)
        {
            return Math.Atan((Math.Sin(lat1) * Math.Cos(lat2) * Math.Sin(lon - lon2)
     - Math.Sin(lat2) * Math.Cos(lat1) * Math.Sin(lon - lon1)) / (Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(lon1 - lon2)));
        }
        /// <summary>
        /// Gets the true bearing at a distance from the start point towards the new point.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The point to get the bearing towards.</param>
        /// <param name="distanceKM">The distance in kilometers travelled between start and end.</param>
        /// <returns></returns>
        public static double GetTrueBearing(Point start, Point end, double distanceKM)
        {
            double d = distanceKM / EarthRadius; //Angular distance in radians
            double lon1 = start.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            double tc1;
            if (Math.Sin(lon2 - lon1) < 0)
                tc1 = Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) / (Math.Sin(d) * Math.Cos(lat1)));
            else
                tc1 = 2 * Math.PI - Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) / (Math.Sin(d) * Math.Cos(lat1)));
            return tc1 / Math.PI * 180;
        }

        /// <summary>
        /// Gets the point based on a start point, a heading and a distance.
        /// </summary>
        /// <param name="start">The start point with (lon,lat) pairs in degrees </param>
        /// <param name="distanceKM">The distance M.</param>
        /// <param name="heading">The heading.</param>
        /// <returns>point (lon,lat)</returns>
        public static Point GetPointFromHeading(Point start, double distance, double heading)
        {
            double brng = heading *D2RCoeff;
            double lon1 = start.X * D2RCoeff;
            double lat1 = start.Y * D2RCoeff;
            double dR = distance / EarthRadius; //Angular distance in radians
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(lat1), Math.Cos(dR) - Math.Sin(lat1) * Math.Sin(lat2));
            double lon = lon2 * R2DCoeff;
            double lat = lat2  * R2DCoeff;
            while (lon < -180) lon += 360;
            while (lat < -90) lat += 180;
            while (lon > 180) lon -= 360;
            while (lat > 90) lat -= 180;
            return new Point(lon, lat);
        }
    
        /// <summary>
        /// Gets the point based on a start point, a heading and a distance.
        /// </summary>
        /// <param name="lon1">longitude of start point in radian</param>
        /// <param name="lat1">latitude of start point in radian</param>
        /// <param name="distance">The distance M.</param>
        /// <param name="brng">The heading in radian</param>
        /// <returns>point (lon,lat) in radians</returns>
        public static Point GetPointFromHeading(double lon1, double lat1, double distance, double brng)
        {
            double dR = distance / EarthRadius; //Angular distance in radians
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(lat1), Math.Cos(dR) - Math.Sin(lat1) * Math.Sin(lat2));
            double lon = lon2 * R2DCoeff;
            double lat = lat2 * R2DCoeff;
            while (lon < -180) lon += 360;
            while (lat < -90) lat += 180;
            while (lon > 180) lon -= 360;
            while (lat > 90) lat -= 180;
            lon =  lon * D2RCoeff;
            lat = lat * D2RCoeff;
            return new Point(lon, lat);
        }
    }
}
