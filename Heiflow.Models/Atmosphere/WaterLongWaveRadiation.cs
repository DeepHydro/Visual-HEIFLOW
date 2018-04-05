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
    /// <summary>
    /// Caculate Water net Long-Wave Radiation using Stefan-Boltzmann law
    /// </summary>
    public class WaterNetLongWaveRadiation:LongWaveRadiation
    {
        public WaterNetLongWaveRadiation()
        {
            //Name = "S-B Water LongWave Radiation Model";
            //Descriptions = "Stefan-Boltzmann law based Water LongWave Radiation Model";
            //ID = "AC112";
            EmissivityContant = 0.96;
            ReflectionRate = 0.03;
            
            AirLongWaveRadiationModel = new AirLongWaveRadiation();
            ReflectionRateForAirLongWave = 0.03;
        }

        public ILongWaveRadiation AirLongWaveRadiationModel { get; set; }

        public double ReflectionRateForAirLongWave { get; set; }

        /// <summary>
        /// Calculate water emissivity
        /// </summary>
        /// <param name="temperature">in the unit of Celsius degree</param>
        /// <returns></returns>
        public override double Emissivity(double temperature)
        {
            return EmissivityContant;
        }

        /// <summary>
        ///  Calculate net Long-Wave Radiation Flux
        /// </summary>
        /// <param name="airTemperature">not used</param>
        /// <param name="waterTemperature">in the unit of Celsius degree</param>
        /// <param name="cloudCover">not used</param>
        /// <returns></returns>
        public override double LongWaveRadiationFlux(double airTemperature, double waterTemperature, double cloudCover)
        {
            double essimited = EmissivityContant * base.LongWaveRadiationFlux(waterTemperature, waterTemperature, cloudCover);
            double absorbed = AirLongWaveRadiationModel.LongWaveRadiationFlux(airTemperature, waterTemperature, cloudCover);

            return absorbed - essimited;
        }
    }

    /// <summary>
    /// Caculate  Water net Long-Wave Radiation using Berliand formula
    /// </summary>
    public class BerliandWaterNetLongWaveRadiation : LongWaveRadiation
    {
        public BerliandWaterNetLongWaveRadiation()
        {
            //Name = "Berliand Water LongWave Radiation Model";
            //Descriptions = "Berliand formula based Water LongWave Radiation Model";
            //ID = "AC113";
            EmissivityContant = 0.975;
            VaporPressureModel = new BuckVaporPressure();
        }

        public IVaporPressure VaporPressureModel { get; set; }

        /// <summary>
        /// Calculate water emissivity
        /// </summary>
        /// <param name="temperature">in the unit of Celsius degree</param>
        /// <returns></returns>
        public override double Emissivity(double temperature)
        {
            return EmissivityContant;
        }

        /// <summary>
        /// Calculate net Long-Wave Radiation Flux
        /// </summary>
        /// <param name="temperature">in the unit of Celsius degree</param>
        /// <param name="cloudCover">not used</param>
        /// <returns></returns>
        public override double LongWaveRadiationFlux(double airTemperature, double waterTemperature, double cloudCover)
        {
            double es = VaporPressureModel.VaporPressure(airTemperature);
            double air = EmissivityContant * StefanBoltzmannContant * Math.Pow(airTemperature, 4) * (0.05 * Math.Sqrt(es) - 0.39) * (1 - 0.72 * cloudCover);
            double water=4 * EmissivityContant * StefanBoltzmannContant * Math.Pow(waterTemperature, 3) * (waterTemperature - airTemperature);
            return air - water;
        }
    }
}
