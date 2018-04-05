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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Drawing
{
    /// <summary>Represents an AHSL color.</summary>
    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("\\{AHSL = ({A}, {H}, {S}, {L})\\}")]
    public struct ColorHsl : IEquatable<Color>
    {
        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorHsl"/> structure
        /// from the specified double values.</summary>
        /// <param name="alpha">The alpha component value. Valid values are 0 through 1.</param>
        /// <param name="hue">The hue component value. Valid values are 0 through 360.</param>
        /// <param name="saturation">The saturation component value. Valid values are 0 through 1.</param>
        /// <param name="lightness">The lightness component value. Valid values are 0 through 1.</param>
        public ColorHsl(double alpha, double hue, double saturation, double lightness)
        {
            ColorRgb.Checkdouble(alpha, "alpha");
            ColorRgb.Checkdouble(hue, "hue", 0.0, 360.0);
            if (hue == 360.0)
            {
                hue = 0.0;
            }
            ColorRgb.Checkdouble(saturation, "saturation");
            ColorRgb.Checkdouble(lightness, "lightness");
            this.A = alpha;
            this.H = hue;
            this.S = saturation;
            this.L = lightness;
        }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Drawing.ColorHsl"/> structure
        /// from the specified double values. The alpha value is implicitly 1 (fully opaque).</summary>
        /// <param name="hue">The hue component value. Valid values are 0 through 360.</param>
        /// <param name="saturation">The saturation component value. Valid values are 0 through 1.</param>
        /// <param name="lightness">The lightness component value. Valid values are 0 through 1.</param>
        public ColorHsl(double hue, double saturation, double lightness)
            : this(1.0, hue, saturation, lightness) { }

        /// <summary>Gets the alpha component value.</summary>
        public readonly double A;
        /// <summary>Gets the hue component value.</summary>
        public readonly double H;
        /// <summary>Gets the saturation component value.</summary>
        public readonly double S;
        /// <summary>Gets the lightness component value.</summary>
        public readonly double L;

        /// <summary>Converts this <see cref="T:LukeSw.Drawing.ColorHsl" /> structure to a human-readable string.</summary>
        /// <returns>String that consists of the AHSL component names and their values.</returns>
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
            builder.Append(", L=");
            builder.Append(this.L);
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
            return A.GetHashCode() ^ H.GetHashCode() ^ S.GetHashCode() ^ L.GetHashCode();
        }
        public Color ToColor()
        {
            return ColorRgb.FromColor(this).ToColor();
        }

        public ColorRgb ToColorRgb()
        {
            return ColorRgb.FromColor(this);
        }

        public ColorHsv ToColorHsv()
        {
            return ColorRgb.FromColor(this).ToColorHsv();
        }

        public static ColorHsl FromColor(Color color)
        {
            return ColorRgb.FromColor(color).ToColorHsl();
        }
    }
}
