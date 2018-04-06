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
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Heiflow.Core.Drawing
{
    /// <summary>Represents an AHSV color.</summary>
    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("\\{AHSV = ({A}, {H}, {S}, {V})\\}")]
    public struct ColorHsv : IEquatable<Color>
    {
        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorHsv"/> structure
        /// from the specified double values.</summary>
        /// <param name="alpha">The alpha component value. Valid values are 0 through 1.</param>
        /// <param name="hue">The hue component value. Valid values are 0 through 360.</param>
        /// <param name="saturation">The saturation component value. Valid values are 0 through 1.</param>
        /// <param name="value">The value component value. Valid values are 0 through 1.</param>
        public ColorHsv(double alpha, double hue, double saturation, double value)
        {
            ColorRgb.Checkdouble(alpha, "alpha");
            ColorRgb.Checkdouble(hue, "hue", 0.0, 360.0);
            if (hue == 360.0)
            {
                hue = 0.0;
            }
            ColorRgb.Checkdouble(saturation, "saturation");
            ColorRgb.Checkdouble(value, "value");
            this.A = alpha;
            this.H = hue;
            this.S = saturation;
            this.V = value;
        }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorHsv"/> structure
        /// from the specified double values. The alpha value is implicitly 1 (fully opaque).</summary>
        /// <param name="hue">The hue component value. Valid values are 0 through 360.</param>
        /// <param name="saturation">The saturation component value. Valid values are 0 through 1.</param>
        /// <param name="value">The value component value. Valid values are 0 through 1.</param>
        public ColorHsv(double hue, double saturation, double value)
            : this(1.0, hue, saturation, value) { }

        /// <summary>Gets the alpha component value.</summary>
        public readonly double A;
        /// <summary>Gets the hue component value.</summary>
        public readonly double H;
        /// <summary>Gets the saturation component value.</summary>
        public readonly double S;
        /// <summary>Gets the value component value.</summary>
        public readonly double V;

        /// <summary>Converts this <see cref="T:LukeSw.Drawing.ColorHsv" /> structure to a human-readable string.</summary>
        /// <returns>String that consists of the AHSV component names and their values.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append(GetType().Name);
            builder.Append(" [");
            builder.Append("A=");
            builder.Append(this.A);
            builder.Append(", H=");
            builder.Append(this.H);
            builder.Append(", S=");
            builder.Append(this.S);
            builder.Append(", V=");
            builder.Append(this.V);
            builder.Append("]");
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            return ToColorRgb().Equals(obj);
        }

        public bool Equals(Color other)
        {
            return ToColorRgb() == ColorRgb.FromColor(other);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ H.GetHashCode() ^ S.GetHashCode() ^ V.GetHashCode();
        }

        public Color ToColor()
        {
            return ColorRgb.FromColor(this).ToColor();
        }

        public ColorRgb ToColorRgb()
        {
            return ColorRgb.FromColor(this);
        }

        public ColorHsl ToColorHsl()
        {
            return ColorRgb.FromColor(this).ToColorHsl();
        }

        public static ColorHsv FromColor(Color color)
        {
            return ColorRgb.FromColor(color).ToColorHsv();
        }
    }
}
