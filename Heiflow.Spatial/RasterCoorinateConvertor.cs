// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Spatial
{
    public class RasterCoorinateConvertor
    {
        public static PointLatLng GetGeoCoorinate(int pixelX, int pixelY, double[] tf)
        {
            double Xgeo = tf[0] + pixelX * tf[1] + pixelY * tf[2];
            double Ygeo = tf[3] + pixelX * tf[4] + pixelY * tf[5];

            return new PointLatLng(Ygeo, Xgeo);
        }

        public static GPoint GetPixelCoorinate(double lat, double lon, double[] tf)
        {
            double lon1 = lon - tf[0];
            double lat1 = lat - tf[3];
            double x = (tf[5] * lon1 - tf[2] * lat1) / (tf[1] * tf[5] - tf[2] * tf[4]);
            double y = (tf[4] * lon1 - tf[1] * lat1) / (tf[2] * tf[4] - tf[1] * tf[5]);

            return new GPoint((int)Math.Round(x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(y, 0, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Compute the tile number (used in file names) for given latitude and tile size.
        /// </summary>
        /// <param name="latitude">Latitude (decimal degrees)</param>
        /// <param name="tileSize">Tile size  (decimal degrees)</param>
        /// <returns>The tile number</returns>
        public static int GetRowFromLatitude(double latitude, double tileSize)
        {
            return (int)System.Math.Round((System.Math.Abs(-90.0 - latitude) % 180) / tileSize, 1);
        }

        /// <summary>
        /// Compute the tile number (used in file names) for given longitude and tile size.
        /// </summary>
        /// <param name="longitude">Longitude (decimal degrees)</param>
        /// <param name="tileSize">Tile size  (decimal degrees)</param>
        /// <returns>The tile number</returns>
        public static int GetColFromLongitude(double longitude, double tileSize)
        {
            return (int)System.Math.Round((System.Math.Abs(-180.0 - longitude) % 360) / tileSize, 1);
        }
    }
}
