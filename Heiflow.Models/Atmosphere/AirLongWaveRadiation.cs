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
