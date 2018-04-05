// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Atmosphere
{
    public enum TemperatureUnit { Celsius, Kelvin, Fahrenheit }
    public enum LengthUnit { mm, inch }
    public class UnitConversion
    {
        static UnitConversion()
        {
            Inch2mm = 25.4;
            mm2Inch = 0.039370079;
        }

        public static double Fahrenheit2Kelvin(double f)
        {
            return (f - 32) / 1.8 + 273.15;
        }

        public static double Celsius2Kelvin(double f)
        {
            return f + 273.15;
        }

        public static double Fahrenheit2Celsius(double f)
        {
            return (f - 32) / 1.8;
        }

        public static double Kelvin2Fahrenheit(double f)
        {
            return (f - 273.15) * 1.8 + 32;
        }

        public static float Kelvin2Fahrenheit(float f)
        {
            return (f - 273.15f) * 1.8f + 32;
        }

        public static double Kelvin2Celsius(double f)
        {
            return f - 273.15;
        }

        public static float Celsius2Fahrenheit(float f)
        {
            return f * 1.8f + 32;
        }

        public static double Celsius2Fahrenheit(double f)
        {
            return f * 1.8 + 32;
        }

        public static double mm2Inch { get; private set; }

        public static double Inch2mm { get; private set; }

    }
}
