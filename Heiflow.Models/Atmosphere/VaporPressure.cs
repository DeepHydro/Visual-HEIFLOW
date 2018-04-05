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
    public interface  IVaporPressure
    {
        double VaporPressure(double temperature);
    }

    /// <summary>
    /// caculate vapor pressure(hPa) using Buck(1996) formular
    /// </summary>
    public class BuckVaporPressure: IVaporPressure
    {
        public BuckVaporPressure()
        {
           
        }

        #region IVaporPressure 成员
        /// <summary>
        /// caculate vapor pressure(hPa) using Buck(1996) formular
        /// </summary>
        /// <param name="temperature">temperature in celsius degree</param>
        /// <returns>hPa</returns>
        public double VaporPressure(double temperature)
        {
            double tc= (temperature-Radiation.KelvinConstant);
            double vp = 6.1121 * Math.Exp((18.678 - tc / 234.5) * tc / (temperature - 38.66));
            return vp;
        }

        #endregion
    }

    /// <summary>
    /// caculate vapor pressure(hPa) using Magnus-Tetens fomular
    /// </summary>
    public class MTVaporPressure :  IVaporPressure
    {
        public MTVaporPressure()
        {
          
        }

        #region IVaporPressure 成员
        /// <summary>
        /// caculate vapor pressure(hPa) using Magnus-Tetens formular
        /// </summary>
        /// <param name="temperature">temperature in celsius degree</param>
        /// <returns>hPa</returns>
        public double VaporPressure(double temperature)
        {
            double tc = (temperature - Radiation.KelvinConstant);
            double vp = 6.1078 * Math.Exp(17.269388 * tc / (temperature - 35.86));
            return vp;
        }
        public double VaporPressureK(double temperature)
        {
            double vp = 6.1078 * Math.Exp(17.269388 * (temperature-273.15) / (temperature - 35.86));
            return vp;
        }
        #endregion
    }
}
