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
