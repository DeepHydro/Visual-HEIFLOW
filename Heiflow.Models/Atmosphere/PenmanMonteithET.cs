// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Atmosphere
{
    public class PenmanMonteithET
    {

        public PenmanMonteithET()
        {
            MonthTemperature = new double[] { -7.99, -4.316666667, 2.286666667, 9.793333333, 16.92666667, 22.02333333, 24.34333333, 22.31, 15.34333333, 8.27, 0.063333333, -9.953333333 };
        }
        /// <summary>
        /// 多年月平均温度
        /// </summary>
        public double[] MonthTemperature { get; set; }
        MTVaporPressure vp = new MTVaporPressure();
        ShortWaveRadiation sw = new ShortWaveRadiation(0, 0);

        /// <summary>
        /// calculate ET0 using FAO Model
        /// </summary>
        /// <param name="lat">in degree</param>
        /// <param name="lng">in degree</param>
        /// <param name="tavrg">in K</param>
        /// <param name="tmax">in K</param>
        /// <param name="tmin">in K</param>
        /// <param name="rh">between 0 and 1</param>
        /// <param name="airP">in Kpa</param>
        /// <param name="wind2">in m/s</param>
        /// <param name="lightHour">between 0 and 12</param>
        /// <param name="day">datetime</param>
        /// <param name="cloudCover">between 0 and 1</param>
        /// <returns>et0 mm/d</returns>
        public double ET0(double lat, double lng, double tavrg, double tmax, double tmin, double rh, double airP,
           double wind2, double lightHour, DateTime day, double cloudCover)
        {
            sw.Latitude = lat;
            sw.Longitude = lng;

            double Es = vp.VaporPressureK(tavrg) * 0.1;
            double Ea = Es * rh;
            double delta = 4098 * Es / Math.Pow((tavrg - 35.85), 2.0);
            double garma = 0.00163 * airP / (2.150025 - 0.002365 * (tavrg - 273.15));
            double et0 = 0;

            int month = day.Month;
            double G = 0;
            if (month == 1)
            {
                G = 0.14 * (MonthTemperature[0] - MonthTemperature[11]);
            }
            else
            {
                G = 0.14 * (MonthTemperature[month - 1] - MonthTemperature[month - 2]);
            }

            double Rnet = sw.DailyNetRadiation(lat, lng, day, tmax, tmin, Ea, 0.23, cloudCover);

            double d = 0.408 * delta * (Rnet - G) + garma * 900 * wind2 * (Es - Ea) / (tavrg - 0.15);
            et0 = d / (delta + garma * (1 + 0.34 * wind2));
            if (et0 < 0) et0 = 0.01;
            return et0;
        }

        public double ET0(double lat, double lng, double tavrg, double tmax, double tmin, double rh, double airP, double wind2, DateTime day, double cloudCover)
        {
            sw.Latitude = lat;
            sw.Longitude = lng;

            double Es = vp.VaporPressureK(tavrg) * 0.1;
            double Ea = Es * rh;
            double delta = 4098 * Es / Math.Pow((tavrg - 35.85), 2.0);
            double garma = 0.00163 * airP / (2.150025 - 0.002365 * (tavrg - 273.15));
            double et0 = 0;

            int month = day.Month;
            double G = 0;
            if (month == 1)
            {
                G = 0.14 * (MonthTemperature[0] - MonthTemperature[11]);
            }
            else
            {
                G = 0.14 * (MonthTemperature[month - 1] - MonthTemperature[month - 2]);
            }

            double Rnet = sw.DailyNetRadiation(lat, lng, day, tmax, tmin, Ea, 0.23, cloudCover);

            double d = 0.408 * delta * (Rnet - G) + garma * 900 * wind2 * (Es - Ea) / (tavrg - 0.15);
            et0 = d / (delta + garma * (1 + 0.34 * wind2));
            if (et0 < 0) et0 = 0.01;
            return et0;
        }
        /// <summary>
        /// calcuate et0 based on PM equation
        /// </summary>
        /// <param name="lat">in degree</param>
        /// <param name="lng">in degree</param>
        /// <param name="tavrg">in Kelvin</param>
        /// <param name="tmax">in Kelvin</param>
        /// <param name="tmin">in Kelvin</param>
        /// <param name="rh">between 0 and 1</param>
        /// <param name="airP">in kPa</param>
        /// <param name="wind2">in m/s</param>
        /// <param name="Rnet">in MJ/m-2/d</param>
        /// <param name="day">date</param>
        /// <returns>et0 mm/d</returns>
        public double ET0(double lat, double lng, double tavrg, double tmax, double tmin, double rh, double airP, double wind2, double Rnet, DateTime day)
        {
            double Es = vp.VaporPressureK(tavrg) * 0.1;
            double Ea = Es * rh;
            double delta = 4098 * Es / Math.Pow((tavrg - 35.85), 2.0);
            double garma = 0.00163 * airP / (2.150025 - 0.002365 * (tavrg - 273.15));
            double et0 = 0;

            int month = day.Month;
            double G = 0;
            if (month == 1)
            {
                G = 0.14 * (MonthTemperature[0] - MonthTemperature[11]);
            }
            else
            {
                G = 0.14 * (MonthTemperature[month - 1] - MonthTemperature[month - 2]);
            }
            double nl = sw.DailyLongWave(lat, lng, day, 6, tmax, tmin, Ea, 0.23, 0.15);
            //double Rnet = sw.DailyNetRadiation(lat, lng, day, lightHour, tmax, tmin, Ea, 0.23, cloudCover);
            Rnet = Rnet -nl;
            double d = 0.408 * delta * (Rnet - G) + garma * 900 * wind2 * (Es - Ea) / (tavrg - 0.15);
            et0 = d / (delta + garma * (1 + 0.34 * wind2));
            if (et0 < 0) et0 = 0.01;
            return et0;
        }
    }
}