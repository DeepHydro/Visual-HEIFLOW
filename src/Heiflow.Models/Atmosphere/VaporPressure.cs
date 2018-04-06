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
