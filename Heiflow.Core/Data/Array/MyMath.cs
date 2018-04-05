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

namespace Heiflow.Core.Data
{
    public class MyMath
    {
        private static int _Full = -1;

        public static int full
        {
            get
            {
                return _Full;
            }
        }
        public static int none
        {
            get
            {
                return -2;
            }
        }

        public static int end
        {
            get
            {
                return -3;
            }
        }

        public static int[] GetSize(object array)
        {
            return (int[])array.GetType().GetProperty("Size").GetValue(array);
        }

        public static int[] GetSize<T>(T[][] value)
        {
            int[] size = new int[] { value.Length, value[0].Length };
            return size;
        }

        public static int[] GetSize<T>(T[][][] value)
        {
            int[] size = new int[] { value.Length, value[0].Length, value[0][0].Length };
            return size;
        }

        /// <summary>
        /// perform minus
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subtraction"></param>
        /// <returns>3d mat: [1][1][nlen]</returns>
        public static My3DMat<float> Minus(float[] source, float [] subtraction)
        {
            My3DMat<float> mat = new My3DMat<float>(1, 1, source.Length);
            for (int i = 0; i < source.Length; i++)
            {
                mat.Value[0][0][i] = source[i] - subtraction[i];
            }
            return mat;
        }

        /// <summary>
        /// perform plus
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subtraction"></param>
        /// <returns>3d mat: [1][1][nlen]</returns>
        public static My3DMat<float> Plus(float[] source, float[] subtraction)
        {
            My3DMat<float> mat = new My3DMat<float>(1, 1, source.Length);
            for (int i = 0; i < source.Length; i++)
            {
                mat.Value[0][0][i] = source[i] + subtraction[i];
            }
            return mat;
        }

        public static My3DMat<float> Plus(My3DMat<float> a, My3DMat<float> b)
        {
            var size = a.Size;
            var mat = new My3DMat<float>(size[0], size[1], size[2]);
            for (int i = 0; i < size[0]; i++)
                for (int j = 0; j < size[1]; j++)
                    for (int k = 0; k < size[2]; k++)
                    {
                        mat.Value[i][j][k] = a.Value[i][j][k] + b.Value[i][j][k];
                    }
            return mat;
        }

        public static My3DMat<float> TemporalMean(My3DMat<float> source, int zero_index, float multiplier = 1)
        {
            var mean_mat = new My3DMat<float>(1, 1, source.Size[2]);
            for (int i = 0; i < source.Size[2]; i++)
            {
                var vec =  source.GetVector(zero_index, MyMath.full, i);
                var av =vec.Average();
                mean_mat.Value[0][0][i] = av * multiplier;
            }
            return mean_mat;
        }

        public static My3DMat<float> SpatialMean(My3DMat<float> source, int zero_index, float multiplier = 1)
        {
            var mean_mat = new My3DMat<float>(1, source.Size[1], 1);
            for (int i = 0; i < source.Size[1]; i++)
            {
                mean_mat.Value[0][i][0] = source.Value[zero_index][i].Average() * multiplier;
            }
            return mean_mat;
        }

        public static My3DMat<float> Scale(My3DMat<float> source, float scale)
        {
            var size = source.Size;
            var target = new My3DMat<float>(size[0], size[1], size[2]);
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    for (int k = 0; k < size[2]; k++)
                    {
                        target.Value[i][j][k] = source.Value[i][j][k] * scale;
                    }
                }
            }

            return target;
        }

        public static double[] ToDouble(float[] vec)
        {
            var dou_vec = Array.ConvertAll<float, double>(vec, s => s);
            return dou_vec;
        }
        public static double[] ToDouble(int[] vec)
        {
            var dou_vec = Array.ConvertAll<int, double>(vec, s => s);
            return dou_vec;
        }

       
    }
}
