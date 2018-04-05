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
    public class AirLongWaveRadiation:LongWaveRadiation
    {
        public AirLongWaveRadiation()
        {
            ReflectionRate = 0.03;
            EmissivityContant = 0.89;
        }

        /// <summary>
        /// Calculate atmosphere emissivity
        /// </summary>
        /// <param name="temperature">in the unit of Celsius degree</param>
        /// <returns></returns>
        public override double Emissivity(double temperature)
        {
            return AtmosphereCE * Math.Pow(temperature + KelvinConstant, 2);
        }

        /// <summary>
        /// Calculate Long Wave Radiation Flux
        /// </summary>
        /// <param name="airTemperature">in the unit of Celsius degree</param>
        /// <param name="waterTemperature">not used</param>
        /// <param name="cloudCover">between 0 and 1</param>
        /// <returns></returns>
        public override double LongWaveRadiationFlux(double airTemperature, double waterTemperature, double cloudCover)
        {
            double emissivity = Emissivity(airTemperature);
            return emissivity * StefanBoltzmannContant * (1 + 0.17 * cloudCover * cloudCover)
                * Math.Pow(airTemperature + KelvinConstant, 4) * (1 - ReflectionRate);
        }
    }
}
