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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Atmosphere
{
    public struct SunRiseSet
    {
        public double SunRise;
        public double SunSet;
    }

    public struct SunCoordinates
    {
        public double ZenithAngle;
        public double Azimuth;
       
         public SunCoordinates(double zenith, double azimuth)
        {
            ZenithAngle = zenith;
            Azimuth = azimuth;
        }
    };

    public struct SolarTime
    {
        public int iYear;
        public int iMonth;
        public int iDay;
        public double dHours;
        public double dMinutes;
        public double dSeconds;

        public SolarTime(int year, int mon, int day, double hour, double min, double sec)
        {
            iYear = year;
            iMonth = mon;
            iDay = day;
            dHours = hour;
            dMinutes = min;
            dSeconds = sec;
        }
    };

    /// <summary>
    /// PSA algorithm for High Accuracy Tracking of the Sun;The PSA algorithm uses Universal Time (UT) 
    /// to remove the uncertainty caused by local time zones. 
    /// </summary>
    public class SolarPostion
    {
        //Please refer to http://pveducation.org/properties-of-sunlight/sun-position-high-accuracy
        //public static double 
        public SolarPostion()
        {
            twopi = 2 * pi;
            rad = pi / 180;
        }

        double pi = 3.14159265358979323846;
        double twopi;
        double rad;
        double dEarthMeanRadius = 6371.01;// In km
        double dAstronomicalUnit = 149597890;	// In km
        public struct Location
        {
            public double dLongitude;
            public double dLatitude;

            public Location(double lng, double lat)
            {
                dLongitude = lng;
                dLatitude = lat;
            }
        };

        public SunCoordinates SunPostion(SolarTime udtTime, Location udtLocation)
        {
            SunCoordinates udtSunCoordinates = new SunCoordinates();
            // Main variables
            double dElapsedJulianDays;
            double dDecimalHours;
            double dEclipticLongitude;
            double dEclipticObliquity;
            double dRightAscension;
            double dDeclination;

            // Auxiliary variables
            double dY;
            double dX;

            // Calculate difference in days between the current Julian Day 
            // and JD 2451545.0, which is noon 1 January 2000 Universal Time

            double dJulianDate;
            long liAux1;
            long liAux2;
            // Calculate time of the day in UT decimal hours
            dDecimalHours = udtTime.dHours + (udtTime.dMinutes
                + udtTime.dSeconds / 60.0) / 60.0;
            // Calculate current Julian Day
            liAux1 = (udtTime.iMonth - 14) / 12;
            liAux2 = (1461 * (udtTime.iYear + 4800 + liAux1)) / 4 + (367 * (udtTime.iMonth
                - 2 - 12 * liAux1)) / 12 - (3 * ((udtTime.iYear + 4900
            + liAux1) / 100)) / 4 + udtTime.iDay - 32075;
            dJulianDate = (double)(liAux2) - 0.5 + dDecimalHours / 24.0;
            // Calculate difference between current Julian Day and JD 2451545.0 
            dElapsedJulianDays = dJulianDate - 2451545.0;


            // Calculate ecliptic coordinates (ecliptic longitude and obliquity of the 
            // ecliptic in radians but without limiting the angle to be less than 2*Pi 
            // (i.e., the result may be greater than 2*Pi)

            double dMeanLongitude;
            double dMeanAnomaly;
            double dOmega;
            dOmega = 2.1429 - 0.0010394594 * dElapsedJulianDays;
            dMeanLongitude = 4.8950630 + 0.017202791698 * dElapsedJulianDays; // Radians
            dMeanAnomaly = 6.2400600 + 0.0172019699 * dElapsedJulianDays;
            dEclipticLongitude = dMeanLongitude + 0.03341607 * Math.Sin(dMeanAnomaly)
                + 0.00034894 * Math.Sin(2 * dMeanAnomaly) - 0.0001134
                - 0.0000203 * Math.Sin(dOmega);
            dEclipticObliquity = 0.4090928 - 6.2140e-9 * dElapsedJulianDays
                + 0.0000396 * Math.Cos(dOmega);


            // Calculate celestial coordinates ( right ascension and declination ) in radians 
            // but without limiting the angle to be less than 2*Pi (i.e., the result may be 
            // greater than 2*Pi)

            double dSin_EclipticLongitude;
            dSin_EclipticLongitude = Math.Sin(dEclipticLongitude);
            dY = Math.Cos(dEclipticObliquity) * dSin_EclipticLongitude;
            dX = Math.Cos(dEclipticLongitude);
            dRightAscension = Math.Atan2(dY, dX);
            if (dRightAscension < 0.0) dRightAscension = dRightAscension + twopi;
            dDeclination = Math.Asin(Math.Sin(dEclipticObliquity) * dSin_EclipticLongitude);


            // Calculate local coordinates ( azimuth and zenith angle ) in degrees

            double dGreenwichMeanSiderealTime;
            double dLocalMeanSiderealTime;
            double dLatitudeInRadians;
            double dHourAngle;
            double dCos_Latitude;
            double dSin_Latitude;
            double dCos_HourAngle;
            double dParallax;
            dGreenwichMeanSiderealTime = 6.6974243242 +
                0.0657098283 * dElapsedJulianDays
                + dDecimalHours;
            dLocalMeanSiderealTime = (dGreenwichMeanSiderealTime * 15
                + udtLocation.dLongitude) * rad;
            dHourAngle = dLocalMeanSiderealTime - dRightAscension;
            dLatitudeInRadians = udtLocation.dLatitude * rad;
            dCos_Latitude = Math.Cos(dLatitudeInRadians);
            dSin_Latitude = Math.Sin(dLatitudeInRadians);
            dCos_HourAngle = Math.Cos(dHourAngle);
            udtSunCoordinates.ZenithAngle = (Math.Acos(dCos_Latitude * dCos_HourAngle
                * Math.Cos(dDeclination) + Math.Sin(dDeclination) * dSin_Latitude));
            dY = -Math.Sin(dHourAngle);
            dX = Math.Tan(dDeclination) * dCos_Latitude - dSin_Latitude * dCos_HourAngle;
            udtSunCoordinates.Azimuth = Math.Atan2(dY, dX);
            if (udtSunCoordinates.Azimuth < 0.0)
                udtSunCoordinates.Azimuth = udtSunCoordinates.Azimuth + twopi;
            udtSunCoordinates.Azimuth = udtSunCoordinates.Azimuth / rad;
            // Parallax Correction
            dParallax = (dEarthMeanRadius / dAstronomicalUnit)
                * Math.Sin(udtSunCoordinates.ZenithAngle);
            udtSunCoordinates.ZenithAngle = (udtSunCoordinates.ZenithAngle + dParallax) / rad;

            return udtSunCoordinates;
        }
    }
}
