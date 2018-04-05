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

using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Image.ImageSets
{
    public class ImageStatistics
    {
        public ImageStatistics()
        {

        }

        public My3DMat<double> Average(My3DMat<double> source)
        {
            int nvar = source.Size[0];
            int nrow = source.Size[1];
            int ncol = source.Size[2];
            var mat = new My3DMat<double>(nvar, nrow, ncol);
      
            int ncol_1 = ncol - 1;
            int nrow_1 = nrow - 1;

            for (int v = 0; v < nvar; v++)
            {
                var mat_source = source.Value[v];
                var vec5 = new double[5];
                // the first and the last columns
                for (int r = 1; r < nrow - 1; r++)
                {
                    vec5[0] = mat_source[r - 1][0];
                    vec5[1] = mat_source[r - 1][1];
                    vec5[2] = mat_source[r][1];
                    vec5[3] = mat_source[r + 1][1];
                    vec5[4] = mat_source[r + 1][0];
                    mat.Value[v][r][0] = vec5.Average();

                    vec5[0] = mat_source[r - 1][ncol_1];
                    vec5[1] = mat_source[r - 1][ncol_1 - 1];
                    vec5[2] = mat_source[r][ncol_1 - 1];
                    vec5[3] = mat_source[r + 1][ncol_1 - 1];
                    vec5[4] = mat_source[r + 1][ncol_1];
                    mat.Value[v][r][ncol_1] = vec5.Average();
                }
                // the first and the last rows
                for (int c = 1; c < ncol - 1; c++)
                {
                    vec5[0] = mat_source[0][c - 1];
                    vec5[1] = mat_source[1][c - 1];
                    vec5[2] = mat_source[1][c];
                    vec5[3] = mat_source[1][c + 1];
                    vec5[4] = mat_source[0][c + 1];
                    mat.Value[v][0][c] = vec5.Average();

                    vec5[0] = mat_source[nrow_1][c - 1];
                    vec5[1] = mat_source[nrow_1 - 1][c - 1];
                    vec5[2] = mat_source[nrow_1 - 1][c];
                    vec5[3] = mat_source[nrow_1 - 1][c + 1];
                    vec5[4] = mat_source[nrow_1][c + 1];

                    mat.Value[v][nrow_1][c] = vec5.Average();
                }

                double[] vec8 = new double[8];
                for (int r = 1; r < nrow - 1; r++)
                {
                    for (int c = 1; c < ncol - 1; c++)
                    {
                        vec8[0] = mat_source[r - 1][c - 1];
                        vec8[1] = mat_source[r - 1][c];
                        vec8[2] = mat_source[r - 1][c + 1];
                        vec8[3] = mat_source[r][c - 1];
                        vec8[4] = mat_source[r][c + 1];
                        vec8[5] = mat_source[r + 1][c - 1];
                        vec8[6] = mat_source[r + 1][c];
                        vec8[7] = mat_source[r + 1][c + 1];

                        mat.Value[v][r][c] = vec8.Average();
                    }
                }
            }
            return mat;
        }
        public My3DMat<double> StandardDeviation(My3DMat<double> source)
        {
            int nvar = source.Size[0];
            int nrow = source.Size[1];
            int ncol = source.Size[2];
            var mat = new My3DMat<double>(nvar, nrow, ncol);
            int ncol_1 = ncol - 1;
            int nrow_1 = nrow - 1;

            for (int v = 0; v < nvar; v++)
            {
                var mat_source = source.Value[v];
                var vec5 = new double[5];
                // the first and the last columns
                for (int r = 1; r < nrow - 1; r++)
                {
                    vec5[0] = mat_source[r - 1][0];
                    vec5[1] = mat_source[r - 1][1];
                    vec5[2] = mat_source[r][1];
                    vec5[3] = mat_source[r + 1][1];
                    vec5[4] = mat_source[r + 1][0];
                    mat.Value[v][r][0] = MyStatisticsMath.StandardDeviation(vec5);

                    vec5[0] = mat_source[r - 1][ncol_1];
                    vec5[1] = mat_source[r - 1][ncol_1 - 1];
                    vec5[2] = mat_source[r][ncol_1 - 1];
                    vec5[3] = mat_source[r + 1][ncol_1 - 1];
                    vec5[4] = mat_source[r + 1][ncol_1];
                    mat.Value[v][r][ncol_1] = MyStatisticsMath.StandardDeviation(vec5);
                }
                // the first and the last rows
                for (int c = 1; c < ncol - 1; c++)
                {
                    vec5[0] = mat_source[0][c - 1];
                    vec5[1] = mat_source[1][c - 1];
                    vec5[2] = mat_source[1][c];
                    vec5[3] = mat_source[1][c + 1];
                    vec5[4] = mat_source[0][c + 1];
                    mat.Value[v][0][c] = MyStatisticsMath.StandardDeviation(vec5);

                    vec5[0] = mat_source[nrow_1][c - 1];
                    vec5[1] = mat_source[nrow_1 - 1][c - 1];
                    vec5[2] = mat_source[nrow_1 - 1][c];
                    vec5[3] = mat_source[nrow_1 - 1][c + 1];
                    vec5[4] = mat_source[nrow_1][c + 1];

                    mat.Value[v][nrow_1][c] = MyStatisticsMath.StandardDeviation(vec5);
                }

                double[] vec8 = new double[8];
                for (int r = 1; r < nrow - 1; r++)
                {
                    for (int c = 1; c < ncol - 1; c++)
                    {
                        vec8[0] = mat_source[r - 1][c - 1];
                        vec8[1] = mat_source[r - 1][c];
                        vec8[2] = mat_source[r - 1][c + 1];
                        vec8[3] = mat_source[r][c - 1];
                        vec8[4] = mat_source[r][c + 1];
                        vec8[5] = mat_source[r + 1][c - 1];
                        vec8[6] = mat_source[r + 1][c];
                        vec8[7] = mat_source[r + 1][c + 1];

                        mat.Value[v][r][c] = MyStatisticsMath.StandardDeviation(vec8);
                    }
                }
            }
            return mat;
        }

        public My3DMat<float> Average(My3DMat<float> source)
        {
            var mat = new My3DMat<float>(source.Size[0], source.Size[1], source.Size[2]);
            int nrow = source.Size[1];
            int ncol = source.Size[2];
            int ncol_1 = ncol - 1;
            int nrow_1 = nrow - 1;

            var mat_source = source.Value[0];
            var vec5 = new float[5];
            // the first and the last columns
            for (int r = 1; r < nrow - 1; r++)
            {
                vec5[0] = mat_source[r - 1][0];
                vec5[1] = mat_source[r - 1][1];
                vec5[2] = mat_source[r][1];
                vec5[3] = mat_source[r + 1][1];
                vec5[4] = mat_source[r + 1][0];
                mat.Value[0][r][0] = vec5.Average();

                vec5[0] = mat_source[r - 1][ncol_1];
                vec5[1] = mat_source[r - 1][ncol_1 - 1];
                vec5[2] = mat_source[r][ncol_1 - 1];
                vec5[3] = mat_source[r + 1][ncol_1 - 1];
                vec5[4] = mat_source[r + 1][ncol_1];
                mat.Value[0][r][ncol_1] = vec5.Average();
            }
            // the first and the last rows
            for (int c = 1; c < ncol - 1; c++)
            {
                vec5[0] = mat_source[0][c - 1];
                vec5[1] = mat_source[1][c - 1];
                vec5[2] = mat_source[1][c];
                vec5[3] = mat_source[1][c + 1];
                vec5[4] = mat_source[0][c + 1];
                mat.Value[0][0][c] = vec5.Average();

                vec5[0] = mat_source[nrow_1][c - 1];
                vec5[1] = mat_source[nrow_1 - 1][c - 1];
                vec5[2] = mat_source[nrow_1 - 1][c];
                vec5[3] = mat_source[nrow_1 - 1][c + 1];
                vec5[4] = mat_source[nrow_1][c + 1];

                mat.Value[0][nrow_1][c] = vec5.Average();
            }

            float[] vec8 = new float[8];
            for (int r = 1; r < nrow - 1; r++)
            {
                for (int c = 1; c < ncol - 1; c++)
                {
                    vec8[0] = mat_source[r - 1][c - 1];
                    vec8[1] = mat_source[r - 1][c];
                    vec8[2] = mat_source[r - 1][c + 1];
                    vec8[3] = mat_source[r][c - 1];
                    vec8[4] = mat_source[r][c + 1];
                    vec8[5] = mat_source[r + 1][c - 1];
                    vec8[6] = mat_source[r + 1][c];
                    vec8[7] = mat_source[r + 1][c + 1];

                    mat.Value[0][r][c] = vec8.Average();
                }
            }

            return mat;
        }
        public My3DMat<float> StandardDeviation(My3DMat<float> source)
        {
            var mat = new My3DMat<float>(source.Size[0], source.Size[1], source.Size[2]);
            int nrow = source.Size[1];
            int ncol = source.Size[2];
            int ncol_1 = ncol - 1;
            int nrow_1 = nrow - 1;

            var mat_source = source.Value[0];
            var vec5 = new float[5];
            // the first and the last columns
            for (int r = 1; r < nrow - 1; r++)
            {
                vec5[0] = mat_source[r - 1][0];
                vec5[1] = mat_source[r - 1][1];
                vec5[2] = mat_source[r][1];
                vec5[3] = mat_source[r + 1][1];
                vec5[4] = mat_source[r + 1][0];
                mat.Value[0][r][0] = MyStatisticsMath.StandardDeviation(vec5);

                vec5[0] = mat_source[r - 1][ncol_1];
                vec5[1] = mat_source[r - 1][ncol_1 - 1];
                vec5[2] = mat_source[r][ncol_1 - 1];
                vec5[3] = mat_source[r + 1][ncol_1 - 1];
                vec5[4] = mat_source[r + 1][ncol_1];
                mat.Value[0][r][ncol_1] = MyStatisticsMath.StandardDeviation(vec5);
            }
            // the first and the last rows
            for (int c = 1; c < ncol - 1; c++)
            {
                vec5[0] = mat_source[0][c - 1];
                vec5[1] = mat_source[1][c - 1];
                vec5[2] = mat_source[1][c];
                vec5[3] = mat_source[1][c + 1];
                vec5[4] = mat_source[0][c + 1];
                mat.Value[0][0][c] = MyStatisticsMath.StandardDeviation(vec5);

                vec5[0] = mat_source[nrow_1][c - 1];
                vec5[1] = mat_source[nrow_1 - 1][c - 1];
                vec5[2] = mat_source[nrow_1 - 1][c];
                vec5[3] = mat_source[nrow_1 - 1][c + 1];
                vec5[4] = mat_source[nrow_1][c + 1];

                mat.Value[0][nrow_1][c] = MyStatisticsMath.StandardDeviation(vec5);
            }

            float[] vec8 = new float[8];
            for (int r = 1; r < nrow - 1; r++)
            {
                for (int c = 1; c < ncol - 1; c++)
                {
                    vec8[0] = mat_source[r - 1][c - 1];
                    vec8[1] = mat_source[r - 1][c];
                    vec8[2] = mat_source[r - 1][c + 1];
                    vec8[3] = mat_source[r][c - 1];
                    vec8[4] = mat_source[r][c + 1];
                    vec8[5] = mat_source[r + 1][c - 1];
                    vec8[6] = mat_source[r + 1][c];
                    vec8[7] = mat_source[r + 1][c + 1];

                    mat.Value[0][r][c] = MyStatisticsMath.StandardDeviation(vec8);
                }
            }

            return mat;
        }
    }
}