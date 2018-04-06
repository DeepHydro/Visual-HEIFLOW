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
