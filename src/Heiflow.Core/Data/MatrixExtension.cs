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

using Heiflow.Core;
using Heiflow.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Data;
using Heiflow.Core.Data;

namespace Heiflow.Core.Data
{
    public  static  class MatrixExtension<T>
    {
        public static T[] GetRow(int row, T[,] matrix)
        {
            int colnum = matrix.GetLength(1);
            T[] rowData = new T[colnum];
            for (int i = 0; i < colnum; i++)
            {
                rowData[i] = matrix[row, i];
            }
            return rowData;
        }

        public static T[] GetColumn(int col, T[,] matrix)
        {
            int rownum = matrix.GetLength(0);
            T[] colData = new T[rownum];
            for (int i = 0; i < rownum; i++)
            {
                colData[i] = matrix[i, col];
            }
            return colData;
        }
        public static void AssignRow(T[,] matrix, int row, T [] rowVector)
        {
            int colnum = matrix.GetLength(1);
            for (int i = 0; i < colnum; i++)
            {
                matrix[row, i] = rowVector[i];
            }
        }
        public static void AssignColumn(T[,] matrix, int col, T[] colVector)
        {
            int rownum = matrix.GetLength(0);
            for (int i = 0; i < rownum; i++)
            {
                matrix[i, col] = colVector[i];
            }
        }
        public static T[] SubArray( T[] data, int index, int length)
        {
            T[] result = new T[length];
            System.Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] DeepCloneSubArray(T[] data, int index, int length)
        {
            T[] arrCopy = new T[length];
            System.Array.Copy(data, index, arrCopy, 0, length);
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, arrCopy);
                ms.Position = 0;
                return (T[])bf.Deserialize(ms);
            }
        }
        public static void DeepClone(T[,] source, T[,] target)
        {
            int rownum = source.GetLength(0);
            int colnum = source.GetLength(1);
            if (target == null)
                target = new T[rownum, colnum];                    
            for (int r = 0; r < rownum; r++)
            {
                for (int c = 0; c < colnum; c++)
                {
                    target[r, c] = source[r, c];
                }
            }
        }

        public static T[,] LoadMatrix(string filename)
        {
            T[,] matrix = null;
            StreamReader sr = new StreamReader(filename);
            string content = sr.ReadToEnd().Trim(new char[] { ' ', '\n' });
            string[] lines = content.Split(StreamReaderSequence.cEnter);
            var vv = TypeConverterEx.Split<T>(lines[0]);
            matrix = new T[lines.Length, vv.Count()];
            for (int i = 0; i < lines.Length; i++)
            {
                if(i != 0)
                    vv = TypeConverterEx.Split<T>(lines[0]);
                for (int j = 0; j < vv.Count(); j++)
                {
                    matrix[i, j] = vv[j];
                }
            }
            sr.Close();
            return matrix;
        }

        public static T[][] LoadLayeredArray(string filename)
        {
            T[][] matrix = null;
            StreamReader sr = new StreamReader(filename);
            string content = sr.ReadToEnd().Trim(new char[] { ' ', '\n' });
            string[] lines = content.Split(StreamReaderSequence.cEnter);
            matrix = new T[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                matrix[i] = TypeConverterEx.Split<T>(lines[i]);
            }
            sr.Close();
            return matrix;
        }

        /// <summary>
        /// load multi-dimensional array [col][row]
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static double[][] Load(string file, int skiplines = 0, int maxline = -1)
        {
            double[][] data = null;
            StreamReader sr = new StreamReader(file);
            string line = sr.ReadLine();
            var vv = TypeConverterEx.Split<double>(line);
            sr.Close();

            sr = new StreamReader(file);
            int nr = 0;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line != "")
                    nr++;
            }
            sr.Close();
            nr -= skiplines;

            if (maxline > 0 && nr > maxline)
                nr = maxline;

            int nc = vv.Length;
            data = new double[nc][];
            for (int i = 0; i < nc; i++)
            {
                data[i] = new double[nr];
            }

            sr = new StreamReader(file);
            for (int i = 0; i < skiplines; i++)
            {
                line = sr.ReadLine();
            }


            for (int i = 0; i < nr; i++)
            {
                line = sr.ReadLine();
                vv = TypeConverterEx.Split<double>(line);
                for (int j = 0; j < nc; j++)
                {
                    data[j][i] = vv[j];
                }
            }
            sr.Close();
            return data;
        }

        public static void Save(T[,] source, StreamWriter sw)
        {
            int row = source.GetLength(0);
            string line = "";
            for (int r = 0; r < row; r++)
            {
                var rr = GetRow(r, source);
                line += string.Join<T>("\t", rr) + "\n";
            }
            sw.Write(line);
        }

        public static void Save(T[] source, StreamWriter sw)
        {
            int row = source.Length;
            string line = "";
            for (int r = 0; r < row; r++)
            {
                line += source[r] + "\n";
            }
            sw.Write(line);
        }
        /// <summary>
        /// save to txt file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filename"></param>
        public static void Save(T[][] source, string filename, string separator="\t")
        {
            StreamWriter  sw = new StreamWriter(filename);
            int row = source.Length;
            int col = source[0].Length;
            string line = "";
            for (int r = 0; r < row; r++)
            {
                line = string.Join(separator, source[r]);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        public static void SaveColFirst(T[][] source, StreamWriter sw, string separator = "\t")
        {
            int col = source.Length;
            int row = source[0].Length;
            string line = "";
            for (int r = 0; r < row; r++)
            {
                T[] temp = new T[col];
                for (int c = 0; c < col; c++)
                {
                    temp[c] = source[c][r];
                }
                line = string.Join(separator, temp);
                sw.WriteLine(line);
            }
        }
        public static void SaveColFirst(T[][] source, string filename, string separator = "\t")
        {
            StreamWriter sw = new StreamWriter(filename);
            int col = source.Length;
            int row = source[0].Length;
            string line = "";
            for (int r = 0; r < row; r++)
            {
                T[] temp = new T[col];
                for (int c=0;c<col;c++)
                {
                    temp[c] = source[c][r];
                }
                line = string.Join(separator, temp);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        /// <summary>
        ///  3D array: [zone][var][value]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tableNames"></param>
        /// <param name="colNames"></param>
        /// <returns></returns>
        public static DataSet ToDataSet(T[][][] source, string[] tableNames, string[] colNames)
        {
            DataSet ds = new DataSet();
            int n1 = source.GetLength(0);
            int n2 = source[0].Length;
            int n3 = source[0][0].Length;
            for (int i = 0; i < n1; i++)
            {
                DataTable dt = new DataTable(tableNames[i]);
                ds.Tables.Add(dt);
                for (int c = 0; c < colNames.Length; c++)
                {
                    dt.Columns.Add(new DataColumn(colNames[c], Type.GetType("System.Single")));
                }

                for (int k = 0; k < n3; k++)
                {
                    var dr = dt.NewRow();
                    for (int j = 0; j < n2; j++)
                    {
                        dr[j] = source[i][j][k];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return ds;
        }

        public  static void Reshape(T[,] source)
        {

        }

        public static void Set(T[,] source, T constant)
        {
            int rownum = source.GetLength(0);
            int colnum = source.GetLength(1);
            for (int r = 0; r < rownum; r++)
            {
                for (int c = 0; c < colnum; c++)
                {
                    source[r, c] = constant;
                }
            }
        }
        public static void Set(T[] source, T constant)
        {
            int rownum = source.Length;
            for (int r = 0; r < rownum; r++)
            {
                source[r] = constant;
            }
        }

        public static void Set(T[][] source, T constant)
        {
            for (int i = 0; i < source.Length; i++)
            {
                for (int j = 0; j < source[0].Length; j++)
                {
                    source[i][j] = constant;
                }
            }
        }

        public static T[] Retrive(T [][,] source, int dim, int sr, int sc)
        {
            T[] result = null;
            if(dim == 1)
            {
                int d1 = source.GetLength(0);
                int d2 = source[0].GetLength(0);
                int d3 = source[0].GetLength(1);
                result = new T[d1];
                for(int i=0;i<d1;i++)
                {
                    result[i] = source[i][sr, sc];
                }
            }
            return result;
        }

        /// <summary>
        /// Summerize given matrix
        /// </summary>
        /// <param name="matrix">3D array: [var][step][feature]</param>
        /// <param name="inteval">time inteval</param>
        /// <param name="flag">1 mean;2 sum</param>
        /// <returns>2D array: [var][step]</returns>
        public static float[][] Summerize(float[][][] matrix, int inteval, int flag)
        {
            int len = matrix[0].Length;
            int steps = len / inteval;
            int nfea = matrix[0][0].Length;
            int nvar = matrix.Length;
            float[][] stat = new float[nvar][];
            for (int v = 0; v < nvar; v++)
            {
                stat[v] = new float[steps];
            }
            float[] temp = new float[nfea];
            for (int v = 0; v < nvar; v++)
            {
                for (int s = 0; s < steps; s++)
                {
                    for (int t = 0; t < inteval; t++)
                    {
                        for (int i = 0; i < nfea; i++)
                        {
                            temp[i] += matrix[v][s * inteval + t][i];
                        }
                    }
                    if (flag == 1)
                    {
                        stat[v][s] = temp.Average() / inteval;
                    }
                    else if (flag == 2)
                    {
                        stat[v][s] = temp.Sum() / inteval;
                    }
                    MatrixExtension<float>.Set(temp, 0);
                }
            }

            return stat;
        }

        public static float[][] Summerize(float[][][] matrix, int[] intevals, int flag)
        {
            int len = matrix[0].Length;
            int steps = intevals.Length;
            int nfea = matrix[0][0].Length;
            int nvar = matrix.Length;
            float[][] stat = new float[nvar][];
            for (int v = 0; v < nvar; v++)
            {
                stat[v] = new float[steps];
            }
            float[] temp = new float[nfea];
            int cur = 0;
            for (int v = 0; v < nvar; v++)
            {
                for (int s = 0; s < steps; s++)
                {
                    int inteval = intevals[s];
                    for (int t = 0; t < inteval; t++)
                    {
                        for (int i = 0; i < nfea; i++)
                        {
                            temp[i] += matrix[v][cur][i];
                        }
                        cur++;
                    }
                    if (flag == 1)
                    {
                        stat[v][s] = temp.Average() / inteval;
                    }
                    else if (flag == 2)
                    {
                        stat[v][s] = temp.Sum() / inteval;
                    }
                    MatrixExtension<float>.Set(temp, 0);
                }
            }

            return stat;
        }

        /// <summary>
        /// extract subset from the given matrix and save it to a ac file
        /// </summary>
        /// <param name="serialMatrix">2D serial matrix</param>
        /// <param name="indexes">indexes of subset</param>
        /// <param name="varName">name of variable</param>
        /// <param name="acfile">ac filename</param>
        public static void Extract2AcFile(MatrixCube<float> serialMatrix, int[] indexes, string varName, string acfile)
        {
            int steps = serialMatrix.LayeredSerialValue.Length;
            int nfea = indexes.Length;
            float[][][] ac = new float[1][][];
            ac[0] = new float[steps][];
            for (int s = 0; s < steps; s++)
            {
                ac[0][s] = new float[nfea];
            }
            for (int s = 0; s < steps; s++)
            {
                for (int i = 0; i < nfea; i++)
                {
                    ac[0][s][i] = serialMatrix.LayeredSerialValue[s][indexes[i]];
                }
            }
            AcFile acf = new AcFile();
            acf.Save(acfile, ac, new string[] { varName });
            ac = null;
        }

        public static void Extract2AcFile(float[][][] matrix, int[] indexes, string varName, int varIndex, string acfile)
        {
            int steps = matrix[0].Length;
            int nfea = indexes.Length;
            float[][][] ac = new float[1][][];
            ac[0] = new float[steps][];
            for (int s = 0; s < steps; s++)
            {
                ac[0][s] = new float[nfea];
            }
            for (int s = 0; s < steps; s++)
            {
                for (int i = 0; i < nfea; i++)
                {
                    ac[0][s][i] = matrix[varIndex][s][indexes[i]];
                }
            }
            AcFile acf = new AcFile();
            acf.Save(acfile, ac, new string[] { varName });
            ac = null;
        }

        //public static double[] GetDataColumn(double[][] data, int column)
        //{
        //    double[] colData = new double[data.Length];
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        colData[i] = data[i][column];
        //    }
        //    return colData;
        //}

        //public static double RowSum(this double[,] value, int index)
        //{
        //    double result = 0;
        //    for (int i = 0; i <= value.GetUpperBound(1); i++)
        //    {
        //        result += value[index, i];
        //    }
        //    return result;
        //}

        //public static double ColumnSum(this double[,] value, int index)
        //{
        //    double result = 0;
        //    for (int i = 0; i <= value.GetUpperBound(0); i++)
        //    {
        //        result += value[i, index];
        //    }
        //    return result;
        //}

        //public static float RowSum(this float[,] value, int index)
        //{
        //    float result = 0;
        //    for (int i = 0; i <= value.GetUpperBound(1); i++)
        //    {
        //        result += value[index, i];
        //    }
        //    return result;
        //}

        //public static float ColumnSum(this float[,] value, int index)
        //{
        //    float result = 0;
        //    for (int i = 0; i <= value.GetUpperBound(0); i++)
        //    {
        //        result += value[i, index];
        //    }
        //    return result;
        //}
    }

  
}
