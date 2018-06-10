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
using System.IO;
using Heiflow.Core.Data;

namespace Heiflow.Core.IO
{
    public class AcFile
    {
        public AcFile()
        {

        }

        public string FileTypeDescription
        {
            get
            {
                return "Arry Cube File";
            }
        }

        public string Extension
        {
            get
            {
                return ".ac";
            }
        }

        public string FileName
        {
            get;
            set;
        }
        public string[] Variables
        {
            get;
            private set;
        }

        public  void GetACInfo(string filename,ref string[] varNames)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            int feaNum = 0;
            int varnum = br.ReadInt32();
            varNames = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                varNames[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }
            br.Close();
            fs.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="flag"></param>
        /// <param name="varIndex"></param>
        /// <param name="step"></param>
        /// <param name="varNames"></param>
        /// <param name="maxsteps"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public  float[][][] Read(string filename, int flag, int varIndex,ref int step, ref string[] varNames, int maxsteps = -1, float scale = 1)
        {
            float[][][] acmatrix = null;
            step = 0;
            int feaNum = 0;
            // 4字节，integer, 变量个数
            // 对变量个数循环
            // 4字节，变量名字符长度var_len
            //var_len长字节，为字符
            //  4字节, 网格数
            //结束循环

            //时间步长循环
            //网格数循环
            //变量循环
            //4字节, 浮点数
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            int varnum = br.ReadInt32();
            varNames = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                varNames[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            if (flag > 0)
            {
                acmatrix = new float[1][][];

                float[][] matrix = new float[1][];
                for (int i = 0; i < 1; i++)
                {
                    acmatrix[i] = new float[1][];
                    acmatrix[i][0] = new float[feaNum];
                }
                while (!(fs.Position == fs.Length))
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varIndex; v++)
                        {
                           br.ReadSingle();
                        }
                        for (int v = varIndex; v < varIndex+1; v++)
                        {
                            acmatrix[v][0][s] += br.ReadSingle() * scale;
                        }
                        for (int v = varIndex + 1; v < varnum; v++)
                        {
                            br.ReadSingle();
                        }
                    }
                    step++;
                    if (maxsteps > 0)
                    {
                        if (step >= maxsteps)
                            break;
                    }
                }
                if (flag == 1)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < 1; v++)
                        {
                            acmatrix[v][0][s] /= step;
                        }
                    }
                }
            }
            else if (flag == 0) // step by  step
            {
                acmatrix = new float[1][][];
                List<float[]>[] cbcLst = new List<float[]>[1];
                for (int i = 0; i < 1; i++)
                {
                    cbcLst[i] = new List<float[]>();
                }
                while (!(fs.Position == fs.Length))
                {
                    float[][] matrix = new float[1][];
                    for (int i = 0; i < 1; i++)
                    {
                        matrix[i] = new float[feaNum];
                    }
                    for (int s = 0; s < feaNum; s++)
                    {
                        if(varIndex == 0)
                        {
                            matrix[0][s] = br.ReadSingle() * scale;
                        }
                        else if (varIndex == varnum-1)
                        {
                            br.ReadBytes(4 * (varnum - 1));
                        }
                        else
                        {
                            br.ReadBytes(4 * (varIndex));
                            matrix[0][s] = br.ReadSingle() * scale;
                            br.ReadBytes(4 * (varnum - 1 - varIndex));
                        }
                    }
                    for (int v = 0; v < 1; v++)
                    {
                        cbcLst[v].Add(matrix[v]);
                    }
                    step++;
                    if (maxsteps > 0)
                    {
                        if (step >= maxsteps)
                            break;
                    }
                    if (step % 100 == 0)
                        Console.WriteLine("Reading " + step);
                }
                for (int v = 0; v < 1; v++)
                {
                    acmatrix[v] = cbcLst[v].ToArray();
                }
            }
            br.Close();
            fs.Close();

            return acmatrix;
        }

        /// <summary>
        ///  read 3D matrix from binary file with the given filename. 3D array: [variable][step][feature]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="flag">flag: 0 normal (step by step); 1 mean; 2 sum; </param>
        /// <param name="step"></param>
        /// <param name="varNames"></param>
        /// <param name="maxsteps"></param>
        /// <param name="scale"></param>
        /// <returns>3D array: [variable][step][feature]</returns>
        public  float[][][]Read(string filename, int flag, ref int step, ref string[] varNames, int maxsteps = -1, float scale = 1)
        {
            float[][][] acmatrix = null;
            step = 0;
            int feaNum = 0;
            // 4字节，integer, 变量个数
            // 对变量个数循环
               // 4字节，变量名字符长度var_len
               //var_len长字节，为字符
               //  4字节, 网格数
            //结束循环

            //时间步长循环
              //网格数循环
                //变量循环
                   //4字节, 浮点数
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            int varnum = br.ReadInt32();
            varNames = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                varNames[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            if (flag > 0)
            {
                acmatrix = new float[varnum][][];

                float[][] matrix = new float[varnum][];
                for (int i = 0; i < varnum; i++)
                {
                    acmatrix[i] = new float[1][];
                    acmatrix[i][0] = new float[feaNum];
                }
                while (!(fs.Position == fs.Length))
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            acmatrix[v][0][s] += br.ReadSingle() * scale;
                        }
                    }
                    step++;
                    if (maxsteps > 0)
                    {
                        if (step >= maxsteps)
                            break;
                    }
                }
                if (flag == 1)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            acmatrix[v][0][s] /= step;
                        }
                    }
                }
            }
            else if (flag == 0) // step by  step
            {
                acmatrix = new float[varnum][][];
                List<float[]>[] cbcLst = new List<float[]>[varnum];
                for (int i = 0; i < varnum; i++)
                {
                    cbcLst[i] = new List<float[]>();
                }
                while (!(fs.Position == fs.Length))
                {
                    float[][] matrix = new float[varnum][];
                    for (int i = 0; i < varnum; i++)
                    {
                        matrix[i] = new float[feaNum];
                    }
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            matrix[v][s] = br.ReadSingle() * scale;
                        }
                    }
                    for (int v = 0; v < varnum; v++)
                    {
                        cbcLst[v].Add(matrix[v]);
                    }
                    step++;
                    if (maxsteps > 0)
                    {
                        if (step >= maxsteps)
                            break;
                    }
                }
                for (int v = 0; v < varnum; v++)
                {
                    acmatrix[v] = cbcLst[v].ToArray();
                }
            }
            br.Close();
            fs.Close();

            return acmatrix;
        }

        /// <summary>
        ///  read 3D matrix from binary file with the given filename. 3D array: [variable][step][feature]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="flag">flag: 1 mean; 2 sum; </param>
        /// <param name="step"></param>
        /// <param name="varNames"></param>
        /// <param name="maxsteps"></param>
        /// <param name="scale"></param>
        /// <returns>3D array: [variable][step][feature]</returns>
        public  float[][][] Read(string filename, int flag, int[] intevals, ref int step, ref string[] varNames, int maxsteps = -1, float scale = 1)
        {
            float[][][] acmatrix = null;
            step = 0;
            int feaNum = 0;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            int ninteval = intevals.Length;
            int varnum = br.ReadInt32();
            varNames = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                varNames[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            if (flag > 0)
            {
                acmatrix = new float[varnum][][];
                float[][] derived = new float[varnum][];

                for (int i = 0; i < varnum; i++)
                {
                    acmatrix[i] = new float[ninteval][];
                    derived[i] = new float[feaNum];
                    for (int t = 0; t < ninteval; t++)
                    {
                        acmatrix[i][t] = new float[feaNum];
                    }
                }

                for (int n = 0; n < ninteval; n++)
                {
                    MatrixExtension<float>.Set(derived, 0);

                    for (int t = 0; t < intevals[n]; t++)
                    {
                        for (int s = 0; s < feaNum; s++)
                        {
                            for (int v = 0; v < varnum; v++)
                            {
                                //if(!(fs.Position == fs.Length))
                                //{
                                    derived[v][s] += br.ReadSingle() * scale;
                                //}
                            }
                        }
                    }
                    if (flag == 1)
                    {
                        for (int s = 0; s < feaNum; s++)
                        {
                            for (int v = 0; v < varnum; v++)
                            {
                                derived[v][s] = derived[v][s] / intevals[n];
                            }
                        }
                    }
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            acmatrix[v][n][s] = derived[v][s];
                        }
                    }

                    step++;
                    if (maxsteps > 0)
                    {
                        if (step >= maxsteps)
                            break;
                    }
                }
            }


            br.Close();
            fs.Close();

            return acmatrix;
        }
        
        /// <summary>
        /// 3D array: [variable][step][feature]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <param name="varNames"></param>
        /// <param name="feaNum"></param>
        public  void Save(string filename, float[][][] data, string[] varNames)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            int steps = data[0].GetLength(0);
            int feaNum = data[0][0].Length;
            int varnum = varNames.Length;
            bw.Write(varnum);
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = varNames[i].Length;
                bw.Write(varname_len);
                bw.Write(varNames[i].ToCharArray());
                bw.Write(feaNum);
            }


            for (int i = 0; i < steps; i++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        bw.Write(data[v][i][s]);
                    }
                }
            }

            bw.Close();
            fs.Close();
        }

        /// <summary>
        /// Provide quick summerization on all cells or on specified cells. return 2D array: [var][step]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="intevals"></param>
        /// <param name="step"></param>
        /// <param name="varNames"></param>
        /// <param name="subindex">perform summerizing on subset</param>
        /// <param name="statFlag"></param>
        /// <returns>2D array: [var][step]</returns>
        public  float[][] Summerize(string filename, int[] intevals, ref int step, ref string[] varNames, int[] subindex = null, NumericalDataType statFlag = NumericalDataType.Average)
        {
            step = 0;
            int feaNum = 0;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            int varnum = br.ReadInt32();
            varNames = new string[varnum];
            float[][] stat = new float[varnum][];
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                varNames[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
                stat[i] = new float[intevals.Length];
            }

            float[][] matrix = new float[varnum][];
            for (int i = 0; i < varnum; i++)
            {
                matrix[i] = new float[feaNum];
            }

            for (int i = 0; i < intevals.Length; i++)
            {
                MatrixExtension<float>.Set(matrix, 0);
                for (int d = 0; d < intevals[i]; d++)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            matrix[v][s] += br.ReadSingle();
                        }
                    }
                    step++;
                }
                if (subindex == null)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        if (statFlag == NumericalDataType.Average)
                            stat[v][i] = matrix[v].Average() / intevals[i];
                        else if (statFlag == NumericalDataType.Cumulative)
                            stat[v][i] = matrix[v].Average();
                    }
                }
                else
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        float temp = 0;
                        foreach (var id in subindex)
                        {
                            temp += matrix[v][id];
                        }
                        if (statFlag == NumericalDataType.Average)
                            stat[v][i] = temp / subindex.Length / intevals[i];
                        else if (statFlag == NumericalDataType.Cumulative)
                            stat[v][i] = temp / subindex.Length;
                    }
                }
            }
            br.Close();
            fs.Close();
            return stat;
        }
        /// <summary>
        /// Provide quick summerization. Return 3D array: [subset zone][var][step]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="intevals"></param>
        /// <param name="step"></param>
        /// <param name="varNames"></param>
        /// <param name="subindex"></param>
        /// <param name="statFlag"></param>
        /// <returns></returns>
        public  float[][][] Summerize(string filename, int[] intevals, ref int step, ref string[] varNames, int[][] subindex, NumericalDataType statFlag =  NumericalDataType.Average)
        {
            step = 0;
            int feaNum = 0;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            int varnum = br.ReadInt32();
            int nzone = subindex.Length;
            varNames = new string[varnum];
            float[][][] stat = new float[nzone][][];

            for (int i = 0; i < nzone; i++)
            {
                stat[i] = new float[varnum][];
            }

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                varNames[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
                for (int z = 0; z < nzone; z++)
                {
                    stat[z][i] = new float[intevals.Length];
                }
            }

            float[][] matrix = new float[varnum][];
            for (int i = 0; i < varnum; i++)
            {
                matrix[i] = new float[feaNum];
            }

            for (int i = 0; i < intevals.Length; i++)
            {
                MatrixExtension<float>.Set(matrix, 0);
                for (int d = 0; d < intevals[i]; d++)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            matrix[v][s] += br.ReadSingle();
                        }
                    }
                    step++;
                }
                for (int z = 0; z < nzone; z++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        float temp = 0;
                        foreach (var id in subindex[z])
                        {
                            temp += matrix[v][id];
                        }
                        if (statFlag ==  NumericalDataType.Average)
                            stat[z][v][i] = temp / subindex[z].Length / intevals[i];
                        else if (statFlag ==  NumericalDataType.Cumulative)
                            stat[z][v][i] = temp / subindex[z].Length;

                    }
                }
            }
            br.Close();
            fs.Close();
            return stat;
        }
        /// <summary>
        /// Provide quick summerization  on specified cells.. Add an offeset when reading. Return 3D array: [subset zone][var][step]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="intevals"></param>
        /// <param name="step"></param>
        /// <param name="varNames"></param>
        /// <param name="subindex"></param>
        /// <param name="offset"></param>
        /// <param name="statFlag"></param>
        /// <returns></returns>
        public  float[][][] Summerize(string filename, int[] intevals, ref int step, ref string[] varNames, int[][] subindex, float [] offset, NumericalDataType statFlag = NumericalDataType.Average)
        {
            step = 0;
            int feaNum = 0;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            int varnum = br.ReadInt32();
            int nzone = subindex.Length;
            varNames = new string[varnum];
            float[][][] stat = new float[nzone][][];

            for (int i = 0; i < nzone; i++)
            {
                stat[i] = new float[varnum][];
            }

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                varNames[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
                for (int z = 0; z < nzone; z++)
                {
                    stat[z][i] = new float[intevals.Length];
                }
            }

            float[][] matrix = new float[varnum][];
            for (int i = 0; i < varnum; i++)
            {
                matrix[i] = new float[feaNum];
            }

            for (int i = 0; i < intevals.Length; i++)
            {
                MatrixExtension<float>.Set(matrix, 0);
                for (int d = 0; d < intevals[i]; d++)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            matrix[v][s] += (br.ReadSingle() + offset[s]);
                        }
                    }
                    step++;
                }
                for (int z = 0; z < nzone; z++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        float temp = 0;
                        foreach (var id in subindex[z])
                        {
                            temp += matrix[v][id];
                        }
                        if (statFlag == NumericalDataType.Average)
                            stat[z][v][i] = temp / subindex[z].Length / intevals[i];
                        else if (statFlag == NumericalDataType.Cumulative)
                            stat[z][v][i] = temp / subindex[z].Length;

                    }
                }
            }
            br.Close();
            fs.Close();
            return stat;
        }

    }
}
