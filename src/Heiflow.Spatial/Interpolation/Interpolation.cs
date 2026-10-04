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

namespace Heiflow.Spatial.Interpolation
{
    public class Interpolation
    {
         protected double[,] mSoure;
         protected int srcWidth;
         protected int srcHeight;

        public Interpolation(double[,] source)
        {
            mSoure = source;
           srcWidth = source.GetLength(0);
            srcHeight = source.GetLength(1);
        }
    }

    public class LinearInterpolation : Interpolation
    {
        public LinearInterpolation(double[,] source)
            : base(source)
        {
        }

        public double[,] Interpolation(int newWidth, int newHeight)
        {
            double[,] result = new double[newWidth, newHeight];

            double scalX = (double)srcWidth / newWidth;
            double scalY = (double)srcHeight / newHeight;

            for (int i = 0; i < newWidth ; i++)
            {
                for (int j = 0; j < newHeight ; j++)
                {
                    double srcX = i * scalX;
                    double srcY = j * scalY;

                    int srci = (int)srcX;
                    int srcj = (int)srcY;
                    //在原像素中对应的小数部分
                    double u = 0, v = 0;
                    u = srcX - srci;
                    v = srcY - srcj;
                    //if ((i + 1) < srcWidth && ( j + 1) < srcHeight)
                    //{
                        result[i, j] = (1 - u) * (1 - v) * mSoure[i, j] + (1 - u) * v * mSoure[i, j + 1] + u * (1 - v) * mSoure[i + 1, j] + u * v * mSoure[i + 1, j + 1];
                  //  }
                }
            }

            return result;
        }
    }

    public class CubicInterpolation:Interpolation
    {
        public CubicInterpolation(double[,] source):base
            (source)
        {
        }

        public double[,] Interpolation(int newWidth, int newHeight)
        {
            double[,] result = new double[newWidth, newHeight];
            double scalX = (double)srcWidth / newWidth;
            double scalY = (double)srcHeight / newHeight;

            for (int i = 1; i < newWidth+1; i++)
            {
                for (int j = 1; j < newHeight+1; j++)
                {
                    double srcX = i * scalX;
                    double srcy = j * scalY;
                    result[i-1, j-1] = CubicHeight(srcX, srcy);
                   
                }
            }

            return result;
        }

        double MutipleMatrix(double[] A, double[,] B, double[] C)
        {
            double result = 0.0;
            double temp;
            for (int i = 0; i < 4; i++)
            {
                temp = 0.0;
                for (int j = 0; j < 4; j++)
                    temp += A[j] * B[j, i];
                result += C[i] * temp;
            }
            return result;
        }

        private double MySin(double x)
        {
            double absx;
            absx = Math.Abs(x);
            if (absx < 1) return (1 - 2 * Math.Pow(absx, 2) + Math.Pow(absx, 3));
            else if (absx < 2) return (4 - 8 * absx + 5 * Math.Pow(absx, 2) - Math.Pow(absx, 3));
            else return 0;
        }


        private double CubicHeight(double srcX, double srcY)
        {
            //在原像素中对应的整数部分
            int i = (int)srcX;
            int j = (int)srcY;
            //在原像素中对应的小数部分
            double u = 0, v = 0;
            int m = 0, n = 0;
            double[] A = new double[4];
            double[,] B = new double[4, 4];
            double[] C = new double[4];
            u = srcX - i;
            v = srcY - j;
            for (m = 0; m < 4; m++)
            {
                A[m] = MySin(u - m + 1);
                C[m] = MySin(v - m + 1);
                for (n = 0; n < 4; n++)
                {
                    int i1 = (i - 2 + m);
                    int j1 = (j - 2 + n);
                    if (i1 >= 0 && i1 < srcWidth && j1 >= 0 && j1 < srcHeight)
                        B[m, n] = mSoure[i1, j1];
                }
            }
            return MutipleMatrix(A, B, C);
        }

    }
}
