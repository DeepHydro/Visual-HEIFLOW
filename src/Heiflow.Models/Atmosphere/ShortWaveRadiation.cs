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
    public interface IShortWaveRadiation
    {
    }

    public class ShortWaveRadiation:Radiation
    {
        public ShortWaveRadiation(double lat, double lng)
        {
            SkyAttenuation = 0.7;
            Latitude = lat;
            Longitude = lng;
            WaterSurfaceAbsorptionRate = 0.98;
            mSunEarth = new SunEarth(Latitude, Longitude);
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double SkyAttenuation { get; set; }

        /// <summary>
        /// default value is 0.98
        /// </summary>
        public double WaterSurfaceAbsorptionRate { get; set; }
        SunEarth mSunEarth;

        public double WaterSurfaceReflectionRate(double lat,DateTime time)
        {
            double wsr = 0.08;
            if (lat > 0)
            {
                wsr += 0.02 * Math.Sin(2 * Math.PI * time.DayOfYear / 365 - Math.PI / 2);
            }
            else if (lat < 0)
            {
                wsr += 0.02 * Math.Sin(2 * Math.PI * time.DayOfYear / 365 - Math.PI / 2);
            }
            else
            {
                wsr = 0.08;
            }
            return wsr;
        }

        public double[] DailyRadiationIntoHourly(double [] dailyRadiations,DateTime start)
        {
            double[] hourlyRadiations = new double[dailyRadiations.Length * 24];
            for (int i = 0; i < dailyRadiations.Length; i++)
            {
                DateTime daytime = start.AddDays(i);
                var coeffs = HourlyAllocationCoefficient(daytime);
                for (int j = 0; j < 24; j++)
                {
                    hourlyRadiations[i * 24 + j] = dailyRadiations[i] * coeffs[j];
                }
            }
            return hourlyRadiations;
        }

        /// <summary>
        /// The distribution of hourly radiation is assumed to be Sin
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
        public double[] HourlyAllocationCoefficient(DateTime daytime)
        {
            double[] coefficients = new double[24];

            double[] radiations = HourlyIncomingRadiationIntensity(daytime, daytime.AddDays(1), 0);
            double sum= radiations.Sum();
            for (int i = 0; i < 24; i++)
            {
                coefficients[i] = radiations[i] / sum;
            }
            //SunEarth se = new SunEarth(Latitude, Longitude);

            //SunRiseSet srs = se.SunRiseAndSet(daytime);
            //int sunrise = (int) Math.Ceiling(srs.SunRise);
            //int sunset = (int)Math.Floor(srs.SunSet);
            //int hours = sunset - sunrise + 1;
            //for (int i = sunrise - 1; i < sunset - sunrise + 1; i++)
            //{

            //}

            return coefficients;
        }

        public double[] WaterSurfaceIrradiationIntensity(DateTime start, DateTime end, double cloudcover)
        {
            TimeSpan ts = end - start;
            int days = (int)ts.TotalDays;

            double[] radiations = HourlyIncomingRadiationIntensity(start, end, cloudcover);

            for (int i = 0; i < radiations.Length;i++ )
            {
                DateTime now = start.AddHours(i);
                double wsr = WaterSurfaceReflectionRate(Latitude,now);
                radiations[i] = radiations[i] * (1 - wsr) * WaterSurfaceAbsorptionRate;
            }
            return radiations;
        }

        public double[] WaterSurfaceIrradiationIntensity(DateTime start, DateTime end, double[] cloudcover)
        {
            double[] radiations = HourlyIncomingRadiationIntensity(start, end, cloudcover);

            for (int i = 0; i < radiations.Length; i++)
            {
                DateTime now = start.AddHours(i);
                double wsr = WaterSurfaceReflectionRate(Latitude, now);
                radiations[i] = radiations[i] * (1 - wsr) * WaterSurfaceAbsorptionRate;
            }
            return radiations;
        }

        public double[] HourlyIncomingRadiationIntensity(DateTime start, DateTime end,  double cloudcover)
        {
            SunEarth se = new SunEarth(Latitude, Longitude);
            TimeSpan ts = end - start;
            int hours = (int)ts.TotalHours;
            double[] radis = new double[hours];

            for (int i = 0; i < hours; i++)
            {
                DateTime dt = start.AddHours(i);
                SunCoordinates sc = se.SunPostion(dt);
                if (sc.Azimuth > 0)
                    radis[i] = 1366.2 * Math.Sin(sc.Azimuth * SunEarth.D2R) * (1 - 0.71 * cloudcover) * SkyAttenuation;
            }
            return radis;
        }

        public double[] HourlyIncomingRadiationIntensity(DateTime start, DateTime end, double[] cloudcovers)
        {
            SunEarth se = new SunEarth(Latitude, Longitude);
            TimeSpan ts = end - start;
            int hours= (int)ts.TotalHours;
            double[] radis = new double[hours];

            if (cloudcovers.Length != hours)
                throw new Exception("The number of cloud-cover values does not match with the hours");

            for (int i = 0; i < hours; i++)
            {
                DateTime dt = start.AddHours(i);
                SunCoordinates sc = se.SunPostion(dt);
                if(sc.Azimuth >0)
                    radis[i] = 1366.2 * Math.Sin(sc.Azimuth * SunEarth.D2R ) * (1 - 0.71 * cloudcovers[i]) * SkyAttenuation;
            }
            return radis;
        }


        public double DailyIncomingRadiationIntensity(double lat,double lng, DateTime day, double cloudcover = 0.1)
        {
            mSunEarth.Latitude = lat;
            mSunEarth.Longitude = lng;
            int hours = 24;
            double[] radis = new double[hours];

            for (int i = 0; i < hours; i++)
            {
                DateTime dt = day.AddHours(i);
                SunCoordinates sc = mSunEarth.SunPostion(dt);
                if (sc.Azimuth > 0)
                    radis[i] = 1366.2 * Math.Sin(sc.Azimuth * SunEarth.D2R) * (1 - 0.71 * cloudcover) * SkyAttenuation * 3600;
            }
            return radis.Sum();
        }

        public double DailyNetRadiation(double lat, double lng, DateTime day, double lightHour, double tmax,double tmin,double ea,
            double reflectionRate=0.23, double cloudcover = 0.1)
        {
             mSunEarth.Latitude = lat;
            mSunEarth.Longitude = lng;

            double lightRate= lightHour / mSunEarth.SunlightHours(day);
            double ns = DailyIncomingRadiationIntensity(lat,lng,day,cloudcover);
            double Rns = (1 - reflectionRate) * (0.2 + 0.79 *lightRate) * ns * 1e-6;

            double t1= (Math.Pow(tmax,4) +Math.Pow(tmin,4))*0.5;

            double Rnl = 4.903e-9 * t1 * (0.56 - 0.25 * Math.Sqrt(ea)) * (0.1 + 0.9 * lightRate);

            double Rnet = Rns-Rnl;
            return Rnet;
        }

        public double DailyNetRadiation(double lat, double lng, DateTime day, double tmax, double tmin, double ea, ref double Rns, ref double Rnl,
    double reflectionRate = 0.23, double cloudcover = 0.1)
        {
            mSunEarth.Latitude = lat;
            mSunEarth.Longitude = lng;

            double lightRate = 0.9;
            double ns = DailyIncomingRadiationIntensity(lat, lng, day, cloudcover);
            Rns = (1 - reflectionRate) * (0.2 + 0.79 * lightRate) * ns * 1e-6;

            double t1 = (Math.Pow(tmax, 4) + Math.Pow(tmin, 4)) * 0.5;

            Rnl = 4.903e-9 * t1 * (0.56 - 0.25 * Math.Sqrt(ea)) * (0.1 + 0.9 * lightRate);

            double Rnet = Rns - Rnl;
            return Rnet;
        }

        public double DailyLongWave(double lat, double lng, DateTime day, double lightHour, double tmax, double tmin, double ea, double reflectionRate = 0.23, double cloudcover = 0.1)
        {
            mSunEarth.Latitude = lat;
            mSunEarth.Longitude = lng;
            double lightRate = lightHour / mSunEarth.SunlightHours(day);
            double t1 = (Math.Pow(tmax, 4) + Math.Pow(tmin, 4)) * 0.5;

            double Rnl = 4.903e-9 * t1 * (0.56 - 0.25 * Math.Sqrt(ea)) * (0.1 + 0.9 * lightRate);
            return Rnl;
        }
    }
}
