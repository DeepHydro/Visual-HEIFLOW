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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Atmosphere
{
    /// <summary>
    /// Sun position,  sunrise and sunset caculator
    /// Please refer to http://pveducation.org/pvcdrom/properties-of-sunlight/sun-position-calculator
    /// </summary>
    public class SunEarth
    {
          double k1 = 360.0 / 365.0;
          public static double D2R = Math.PI / 180.0;
          public static double R2D = 180.0 / Math.PI;

          public double Latitude { get; set; }
          public double Longitude { get; set; }

          public SunEarth(double lat, double lng)
          {
              Latitude = lat;
              Longitude = lng;
          }

        /// <summary>
        /// Calculate declination angle in Radian
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public   double DeclinationAngle(int d)
        {
            double dec = 23.45 * Math.Sin(k1 * (d - 81) * D2R);
            double temp = k1 * (d - 81) * D2R;
            return Math.Asin(Math.Sin(23.45 * D2R) * Math.Sin(temp));
        }

        /// <summary>
        /// Local Standard Time Meridian
        /// </summary>
        /// <param name="time"></param>
        /// <returns>LSTM in degree</returns>
        public   double LSTM(DateTime time)
        {
            TimeSpan ts = time - time.ToUniversalTime();
            return 15 * ts.Hours;
        }

        /// <summary>
        /// Equation of Time
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public   double EoT(int d)
        {
            double b = k1 * (d - 81) * D2R;
            double eot = 9.87 * Math.Sin(2 * b) - 7.53 * Math.Cos(b) - 1.5 * Math.Sin(b);
            return eot;
        }

        /// <summary>
        /// Local Solar Time 
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public   double LST(double lng, DateTime time)
        {
            double tc = 4 * (lng - LSTM(time)) + EoT(time.DayOfYear);
            double lst = time.Hour + tc / 60.0;
            return lst;
        }

        /// <summary>
        /// converts the local solar time (LST) into the number of degrees which the sun moves across the sky
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public   double HRA(double lst)
        {
            return 15 * (lst - 12);
        }

        /// <summary>
        ///  the Altitude angular height of the sun in the sky measured from the horizontal. 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="time"></param>
        /// <returns>Solar atitude in degree</returns>
        public SunCoordinates SunPostion( DateTime time)
        {
            SunCoordinates sc = new SunCoordinates();
            double latR = Latitude * D2R;
            double lngR = Longitude * D2R;
            double declination = DeclinationAngle(time.DayOfYear);
            double lst = LST(Longitude, time);
            double hra = HRA(lst);
            double ele = Math.Sin(latR) * Math.Sin(declination) + Math.Cos(latR) * Math.Cos(declination) * Math.Cos(hra * D2R);
            double eleR = Math.Asin(ele);
            sc.Azimuth = eleR * R2D;

            double azi = (Math.Sin(declination) * Math.Cos(latR) - Math.Cos(declination) * Math.Sin(latR) * Math.Cos(hra * D2R)) / Math.Cos(eleR);
            sc.ZenithAngle = Math.Acos(azi) * R2D;
            //if (hra > 0 || lst > 0)
            //   sc.ZenithAngle = 360 - sc.ZenithAngle;
            return sc;
        }

        /// <summary>
        /// Please refer to http://en.wikipedia.org/wiki/Sunrise_equation
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public SunRiseSet SunRiseAndSet(DateTime time)
        {
            SunRiseSet sc = new SunRiseSet();
            double declination = DeclinationAngle(time.DayOfYear);
            double riseAngle = Math.Acos(-Math.Tan(Latitude * D2R) * Math.Tan(declination));
            double tc = 4 * (Longitude - LSTM(time)) + EoT(time.DayOfYear);
            sc.SunRise = 12 - riseAngle* R2D / 15 - tc / 60;
            sc.SunSet = 12 + riseAngle * R2D / 15 - tc / 60;
            return sc;
        }

        public double SunlightHours(DateTime time)
        {
            var ss = SunRiseAndSet(time);
            return ss.SunSet-ss.SunRise;
        }
    }


   
}
