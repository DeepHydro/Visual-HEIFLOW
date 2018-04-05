// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.IO
{
    public  class DataCubeStreamWriter
    {
        private string _FileName;
        private FileStream _FileStream;
        private BinaryWriter _BinaryWriter;
        private DataCubeDescriptor _Descriptor;
        public DataCubeStreamWriter(string filename)
        {
            _FileName = filename;
            _FileStream = new FileStream(_FileName, FileMode.Create, FileAccess.Write);
            _BinaryWriter = new BinaryWriter(_FileStream);
            _Descriptor = new DataCubeDescriptor();
        }

        public void WriteHeader(string[] varNames, int feaNum)
        {
            int varnum = varNames.Length;
            _BinaryWriter.Write(varnum);
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = varNames[i].Length;
                _BinaryWriter.Write(varname_len);
                _BinaryWriter.Write(varNames[i].ToCharArray());
                _BinaryWriter.Write(feaNum);
            }
        }

        public void WriteStep(int varnum, int feaNum, float [][][] data)
        {
            for (int s = 0; s < feaNum; s++)
            {
                for (int v = 0; v < varnum; v++)
                {
                    _BinaryWriter.Write(data[v][0][s]);
                }
            }
        }

        public void Close()
        {
            _BinaryWriter.Close();
            _FileStream.Close();
        }

        /// <summary>
        /// 3D array: [variable][step][feature]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <param name="varNames"></param>
        /// <param name="feaNum"></param>
        public void WriteAll(float[][][] data, string[] varNames)
        {        
            int steps = data[0].GetLength(0);
            int feaNum = data[0][0].Length;
            int varnum = varNames.Length;
            _BinaryWriter.Write(varnum);
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = varNames[i].Length;
                _BinaryWriter.Write(varname_len);
                _BinaryWriter.Write(varNames[i].ToCharArray());
                _BinaryWriter.Write(feaNum);
            }


            for (int i = 0; i < steps; i++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        _BinaryWriter.Write(data[v][i][s]);
                    }
                }
            }

            Close();
        }

        /// <summary>
        /// 3D array: [variable][step][feature]
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <param name="varNames"></param>
        /// <param name="feaNum"></param>
        public void WriteAll(My3DMat<float> mat)
        {
            var data = mat.Value;
            var varNames = mat.Variables;
            int steps = data[0].GetLength(0);
            int feaNum = data[0][0].Length;
            int varnum = varNames.Length;
            _BinaryWriter.Write(varnum);
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = varNames[i].Length;
                _BinaryWriter.Write(varname_len);
                _BinaryWriter.Write(varNames[i].ToCharArray());
                _BinaryWriter.Write(feaNum);
            }


            for (int i = 0; i < steps; i++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        _BinaryWriter.Write(data[v][i][s]);
                    }
                }
            }

            if (mat.DateTimes != null)
            {
                _Descriptor.NVar = mat.Size[0];
                _Descriptor.NTimeStep = mat.Size[1];
                _Descriptor.NCell = mat.Size[2];
                _Descriptor.TimeStamps = mat.DateTimes;
                DataCubeDescriptor.Serialize(_FileName + ".xml", _Descriptor);
            }

            Close();
        }

        public void WriteAll(My3DMat<float> mat, int[] var_index)
        {
            var data = mat.Value;
            var varNames = mat.Variables;
            int steps = data[var_index[0]].GetLength(0);
            int feaNum = data[var_index[0]][0].Length;
            int varnum = var_index.Length;

            _BinaryWriter.Write(varnum);
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = varNames[var_index[i]].Length;
                _BinaryWriter.Write(varname_len);
                _BinaryWriter.Write(varNames[var_index[i]].ToCharArray());
                _BinaryWriter.Write(feaNum);
            }

            for (int i = 0; i < steps; i++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        _BinaryWriter.Write(data[var_index[v]][i][s]);
                    }
                }
            }

            if (mat.DateTimes != null)
            {
                _Descriptor.NVar = varnum;
                _Descriptor.NTimeStep = steps;
                _Descriptor.NCell = feaNum;
                _Descriptor.TimeStamps = mat.DateTimes;
                DataCubeDescriptor.Serialize(_FileName + ".xml", _Descriptor);
            }

            Close();
        }
    }
}
