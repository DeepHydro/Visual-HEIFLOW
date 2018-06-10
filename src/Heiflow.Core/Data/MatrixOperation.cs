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
    public static class MatrixOperation
    {
        public static void Mulitple(float[] source, float scale)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] *= scale;
            }
        }
        public static void Mulitple(float[][] source, float scale)
        {
            int row = source.Length;
            int col = source[0].Length;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    source[r][c] *= scale;
                }
            }
        }
        public static void Mulitple(float[][][] source, float scale)
        {
            int n1 = source.GetLength(0);
            int n2 = source[0].Length;
            int n3 = source[0][0].Length;
            for (int i = 0; i < n1; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    for (int k = 0; k < n3; k++)
                    {
                        source[i][j][k] *= scale;
                    }
                }
            }
        }
        /// <summary>
        /// scale a 3D array by a 1D array
        /// </summary>
        /// <param name="source">3D array </param>
        /// <param name="scale">1D array</param>
        /// <param name="dimension">value should be set 1 or 2: 1, scale value by column;  2, scale value by row; </param>
        public static void Mulitple(float[][][] source, float[] scale, int dimension)
        {
            int n1 = source.GetLength(0);
            int n2 = source[0].Length;
            int n3 = source[0][0].Length;
            if (dimension == 1)
            {
                for (int i = 0; i < n1; i++)
                {
                    for (int k = 0; k < n3; k++)
                    {
                        for (int j = 0; j < n2; j++)
                        {
                            source[i][j][k] *= scale[j];
                        }
                    }
                }
            }
            else if (dimension == 2)
            {
                for (int i = 0; i < n1; i++)
                {
                    for (int j = 0; j < n2; j++)
                    {
                        for (int k = 0; k < n3; k++)
                        {
                            source[i][j][k] *= scale[k];
                        }
                    }
                }
            }
        }

        public static float[][][] Merge(float[][][] target, float[][][] source, int dimension)
        {
            int nt1 = target.GetLength(0);
            int ns1 = target.GetLength(0);

            float[][][] merged = new float[nt1][][];
            if (dimension == 1)
            {
                for (int i = 0; i < nt1; i++)
                {
                    merged[i] = Merge(target[i], source[i], 1);
                }
            }
            else if (dimension == 2)
            {
                for (int i = 0; i < nt1; i++)
                {
                    merged[i] = Merge(target[i], source[i], 2);
                }
            }
            return merged;
        }

        public static float[][] Merge(float[][] target, float[][] source, int dimension)
        {
            int nt1 = target.GetLength(0);
            int nt2 = target[0].Length;

            int ns1 = source.GetLength(0);
            int ns2 = source[0].Length;

            float[][] merged = null;
            if (dimension == 1)
            {
                if (nt2 != ns2)
                    throw new Exception("target and source must have the same size for the second dimension!");
                int ncol = nt1 + ns1;
                merged = new float[ncol][];
                for (int c = 0; c < ncol; c++)
                {
                    merged[c] = new float[nt2];
                }

                for (int c = 0; c < nt1; c++)
                {
                    System.Array.Copy(target[c], merged[c], nt2);
                }
                for (int c = 0; c < ns1; c++)
                {
                    System.Array.Copy(source[c], merged[nt1 + c], nt2);
                }
            }
            else if (dimension == 2)
            {
                if (nt1 != ns1)
                    throw new Exception("target and source must have the same size for the first dimension!");
                int nrow = ns1 + ns2;
                merged = new float[nt1][];
                for (int c = 0; c < nt1; c++)
                {
                    merged[c] = target[c].Union(source[c]).ToArray();
                }
            }
            return merged;
        }

        public static void Mulitple(double[] source, double scale)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] *= scale;
            }
        }

        public static float[] Minus(float[] source, float[] subtraction)
        {
            float[] result = new float[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = source[i] - subtraction[i];
            }
            return result;
        }

        public static float[] Add(float[] source, float[] target)
        {
            float[] result = new float[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = source[i] + target[i];
            }
            return result;
        }
        /// <summary>
        /// Extract sub matrix by given index of sub row or sub column
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">a 2D matrix</param>
        /// <param name="subIndex">sub index starting from 0</param>
        /// <param name="dimesion">0, by row; 1 by column</param>
        /// <returns>a 2D matrix</returns>
        public static T[][] SubMatrix<T>(T[][] source, int[] subIndex, int dimesion)
        {
            T[][] subm = null;
            if (dimesion == 0) //by row
            {
                subm = new T[subIndex.Length][];
                int nc = source[0].Length;
                for (int r = 0; r < subIndex.Length; r++)
                {
                    subm[r] = new T[nc];
                    for (int c = 0; c < nc; c++)
                    {
                        subm[r][c] = source[subIndex[r]][c];
                    }
                }
            }
            else if (dimesion == 1) // by column
            {
                int nr = source.Length;
                subm = new T[nr][];

                for (int r = 0; r < nr; r++)
                {
                    subm[r] = new T[subIndex.Length];
                    for (int c = 0; c < subIndex.Length; c++)
                    {
                        subm[r][c] = source[r][subIndex[c]];
                    }
                }
            }

            return subm;
        }
        public static double[] ToDouble<T>(T[] source)
        {
            double[] vec = new double[source.Length];
            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] = double.Parse(source[i].ToString());
            }
            return vec;
        }
    }
}