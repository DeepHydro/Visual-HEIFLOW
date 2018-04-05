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

using Heiflow.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Data
{
    public enum MatrixCubeType { Constant, Matrix, Hybrid, Serial };
    public class MatrixCube<T>
    {
        public MatrixCube(T[][,] lm)
        {
            LayeredValues = lm;
            MatrixType = MatrixCubeType.Matrix;
        }
        /// <summary>
        /// constant
        /// </summary>
        /// <param name="cn"></param>
        public MatrixCube(T[] cn)
        {
            LayeredConstantValues = cn;
            IsConstant = new bool[cn.Length];
            MatrixType = MatrixCubeType.Constant;
            for (int i = 0; i < cn.Length; i++)
            {
                IsConstant[i] = true;
            }
        }

        public MatrixCube(int layer)
        {
            LayeredValues = new T[layer][,];
            IsConstant = new bool[layer];
            LayeredConstantValues = new T[layer];
            MatrixType = MatrixCubeType.Matrix;
        }

        public MatrixCube(int layer, bool serial = true)
        {
            LayeredSerialValue = new T[layer][];
            IsConstant = new bool[layer];
            LayeredConstantValues = new T[layer];
            MatrixType = MatrixCubeType.Serial;
        }

        public string Name { get; set; }
        public MatrixCubeType MatrixType { get; set; }
        /// <summary>
        /// each layer is a matrix
        /// </summary>
        public T[][,] LayeredValues { get; private set; }

        public T[] LayeredConstantValues { get; private set; }
        /// <summary>
        /// determine value in each layer is constant
        /// </summary>
        public bool[] IsConstant { get; private set; }
        /// <summary>
        /// each layer is a 1D array that stores serial value
        /// </summary>
        public T[][] LayeredSerialValue { get;  set; }

        //public VectorStats[] LayeredStats { get; private set; }

        public int Layer
        {
            get
            {
                return GetLength(0);
            }
        }
        public int Row
        {
            get
            {
                return GetLength(1);
            }
        }
        public int Column
        {
            get
            {
                return GetLength(2);
            }
        }

        /// <summary>
        /// return length of specified dimension. return -1 for incorrected inquery
        /// </summary>
        /// <param name="dim">use 0,1,2 to query layer, row, column, respectively</param>
        /// <returns></returns>
        public int GetLength(int dim)
        {
            if (LayeredValues != null)
            {

                if (dim == 0)
                    return LayeredValues.GetLength(0);
                else if (dim == 1)
                    return LayeredValues[0].GetLength(0);
                else if (dim == 2)
                    return LayeredValues[0].GetLength(1);
                else
                    return -1;
            }
            else if (LayeredSerialValue != null)
            {
                if (dim == 0)
                    return LayeredSerialValue.GetLength(0);
                else
                    return LayeredSerialValue[0].GetLength(0);
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// changing cell value at the specified row
        /// </summary>
        /// <param name="values">vector</param>
        /// <param name="l">layer</param>
        /// <param name="rIndex">specified row index</param>
        /// <param name="colNum">column count</param>
        public void SetRowVector(T[] values, int l, int rIndex, int colNum)
        {
            if (LayeredValues != null)
            {
                for (int c = 0; c < colNum; c++)
                {
                    LayeredValues[l][rIndex, c] = values[c];
                }
            }
        }
        /// <summary>
        ///  changing cell value at the specified column
        /// </summary>
        /// <param name="values"></param>
        /// <param name="l"></param>
        /// <param name="rowNum"></param>
        /// <param name="cIndex"></param>
        public void SetColumnVector(T[] values, int l, int rowNum, int cIndex)
        {
            if (LayeredValues != null)
            {
                for (int r = 0; r < rowNum; r++)
                {
                    LayeredValues[l][r, cIndex] = values[r];
                }
            }
        }

        public void SetLayerValue(T[,] values, int l)
        {
            if (LayeredValues != null)
            {
                LayeredValues[l] = values;
                IsConstant[l] = false;
            }
        }

        public void SetLayerValue(T constant, int l)
        {
            if (LayeredValues != null)
            {
                IsConstant[l] = true;
                LayeredConstantValues[l] = constant;
            }
        }

        public float[] DistinctValues(int layer)
        {
            var dist = LayeredValues[layer].Cast<float>().Distinct().ToArray();
            return dist;
        }
        public Dictionary<float, float[]> Distinct(int layer)
        {
            var dic = new Dictionary<float, float[]>();
            if (LayeredValues != null && LayeredValues[layer] != null)
            {
                var dist = LayeredValues[layer].Cast<float>().Distinct().ToArray();
                foreach (var d in dist)
                {
                    dic.Add(d, new float[] { d });
                }
            }
            return dic;
        }


        public T[] GetStepValues(int step)
        {
            if (LayeredSerialValue != null)
                return LayeredSerialValue[step];
            else
                return null;
        }

        public static MatrixCube<T> ToArrayCube(T[][] source)
        {
            int layer = source.GetLength(0);
            MatrixCube<T> cube = new MatrixCube<T>(layer, true);
            for (int i = 0; i < layer; i++)
            {
                cube.LayeredSerialValue[i] = source[i];
            }
            return cube;
        }


   
    }
}
