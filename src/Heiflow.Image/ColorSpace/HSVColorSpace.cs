﻿//
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

using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Image.ColorSpace
{
    public class HSVColorSpace : IColorSpace
    {
        private Bitmap _Source;
        private int _nrow;
        private int _ncol;

        public HSVColorSpace()
        {
             
        }

        public Bitmap Source
        {
            get
            {
                return _Source;
            }
            set
            {
                _Source = value;
                _nrow = _Source.Size.Height;
                _ncol = _Source.Size.Width;
            }
        }

        public double[] WindowStat(int i, int j)
        {
            int index_p = 0;
            int new_r = 0, new_c = 0;
            var stat = new double[7];
            var hsv = ToDouble(i, j);
            stat[0] = hsv[0];
            stat[1] = hsv[1];
            stat[2] = hsv[2];

            double[][] window = new double[3][];

            for (int c = 0; c < 3; c++)
            {
                window[c] = new double[9];
            }
             
            for (int r = i-1; r <= i+1; r++)
            {
                for (int c = j - 1; c <= j + 1; c++)
                {
                    new_r = r;
                    new_c = c;

                    if (r < 0) new_r = 0;
                    if (r == _nrow) new_r = _nrow - 1;
                    if (c < 0) new_c = 0;
                    if (c == _ncol) new_c = _ncol - 1;

                    var pp = ToDouble(new_r, new_c);
                    window[0][index_p] = pp[0];
                    window[1][index_p] = pp[1];
                    window[2][index_p] = pp[2];
                    index_p++;
                }
            }

            stat[0] = window[0][5];
            stat[1] = window[1][5];
            stat[2] = window[2][5];

            stat[3] = MyStatisticsMath.StandardDeviation(window[0]);
            stat[4] = MyStatisticsMath.StandardDeviation(window[1]);
            stat[5] = MyStatisticsMath.StandardDeviation(window[2]);
            stat[6] = window[1].Average();
            return stat;
        }
        public float[] ToFloat(int i, int j)
        {
            var color = _Source.GetPixel(i, j);
            var Red = color.R;
            var Green = color.G;
            var Blue = color.B;
            float r = (Red / 255.0f);
            float g = (Green / 255.0f);
            float b = (Blue / 255.0f);

            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float delta = max - min;

            // get luminance value
            var Luminance = (max + min) / 2;
            float Hue = 0;
            float Saturation = 0;
            if (delta == 0)
            {
                // gray color
                Hue = 0;
                Saturation = 0.0f;
                return new float[] { Hue, Luminance, Saturation };
            }
            else
            {
                // get saturation value
                Saturation = (Luminance <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

                // get hue value
                float hue;

                if (r == max)
                {
                    hue = ((g - b) / 6) / delta;
                }
                else if (g == max)
                {
                    hue = (1.0f / 3) + ((b - r) / 6) / delta;
                }
                else
                {
                    hue = (2.0f / 3) + ((r - g) / 6) / delta;
                }

                // correct hue if needed
                if (hue < 0)
                    hue += 1;
                if (hue > 1)
                    hue -= 1;
                return new float[] { hue, Luminance, Saturation };
                //Hue = (int)(hue * 360);
            }

        }

        public double[] ToDouble(int i, int j)
        {
            System.Drawing.Color color;
            color = _Source.GetPixel(i, j);
            var Red = color.R;
            var Green = color.G;
            var Blue = color.B;
            double r = (Red / 255.0f);
            double g = (Green / 255.0f);
            double b = (Blue / 255.0f);

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;

            // get luminance value
            var Luminance = (max + min) / 2;
            double Hue = 0;
            double Saturation = 0;
            if (delta == 0)
            {
                // gray color
                Hue = 0;
                Saturation = 0.0f;
                return new double[] { Hue, Luminance, Saturation };
            }
            else
            {
                // get saturation value
                Saturation = (Luminance <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

                // get hue value
                double hue;

                if (r == max)
                {
                    hue = ((g - b) / 6) / delta;
                }
                else if (g == max)
                {
                    hue = (1.0f / 3) + ((b - r) / 6) / delta;
                }
                else
                {
                    hue = (2.0f / 3) + ((r - g) / 6) / delta;
                }

                // correct hue if needed
                if (hue < 0)
                    hue += 1;
                if (hue > 1)
                    hue -= 1;
                return new double[] { hue, Luminance, Saturation };
                //Hue = (int)(hue * 360);
            }

        }
  
        /// <summary>
        /// Decomposite given bitmaps into HSL components
        /// </summary>
        /// <param name="sources"></param>
        /// <returns>return a 3d mat [3][nrow][ncol]</returns>
        public  DataCube<double>[] To3DMatrix(Bitmap [] sources)
        {
            int nvar = sources.Length;
            var nrow = sources[0].Height;
            var ncol = sources[0].Width;
            DataCube<double>[] mats = new DataCube<double>[3];
            for (int i = 0; i < 3; i++)
            {
                mats[i] = new DataCube<double>(nvar, nrow, ncol);
            }

            for (int i = 0; i < nvar; i++)
            {
                for (int r = 0; r < nrow; r++)
                {
                    for (int c = 0; c < ncol; c++)
                    {
                        var color = sources[i].GetPixel(c, r);
                        var hsl = ToFloat(r,c);
                        mats[0][i,r,c] = hsl[0];
                        mats[1][i, r, c] = hsl[1];
                        mats[2][i, r, c] = hsl[2];
                    }
                }
            }
            return mats;
        }


    }
}