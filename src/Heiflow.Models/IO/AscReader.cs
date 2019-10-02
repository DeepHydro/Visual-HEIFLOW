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
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Heiflow.Core.IO;

namespace Heiflow.Models.IO
{
    public class AscReader : FileProvider
    {
        public AscReader()
        {
            Extension = ".asc";
            FileTypeDescription = "ASC file";
        }
 
        public float NoDataValue
        {
            get;
            set;
        }

        public float CellSize
        {
            get;
            private set;
        }

        public string[] Variables
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            FileTypeDescription = "2d array file";
            Extension = ".asc";
        }
 
        /// <summary>
        /// load 2d mat
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public DataCube<float> Load(string filename)
        {
            Variables = new string[] { Path.GetFileNameWithoutExtension(filename) };
            FileName = filename;
            DataCube<float> mat = null;

            StreamReader sr = new StreamReader(FileName);
            string line = sr.ReadLine();
            var temp = TypeConverterEx.Split<string>(line);
            int ncol = int.Parse(temp[1]);
            line = sr.ReadLine();
            temp = TypeConverterEx.Split<string>(line);
            int nrow = int.Parse(temp[1]);

            line = sr.ReadLine();
            line = sr.ReadLine();
            line = sr.ReadLine();
            temp = TypeConverterEx.Split<string>(line);
            CellSize = float.Parse(temp[1]);

            line = sr.ReadLine();
            temp = TypeConverterEx.Split<string>(line);
            NoDataValue = float.Parse(temp[1]);
            mat = new DataCube<float>(1, nrow, ncol, false);

            for (int i = 0; i < nrow; i++)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<float>(line);
                for (int j = 0; j < ncol; j++)
                {
                    mat[0,i, j] = buf[j];
                }
            }
            sr.Close();
            return mat;
        }

        public float[] LoadSerial(string filename, object arg)
        {
            Variables = new string[] { Path.GetFileNameWithoutExtension(filename) };
            FileName = filename;

            StreamReader sr = new StreamReader(FileName);
            string line = sr.ReadLine();
            var temp = TypeConverterEx.Split<string>(line);
            int ncol = int.Parse(temp[1]);
            line = sr.ReadLine();
            temp = TypeConverterEx.Split<string>(line);
            int nrow = int.Parse(temp[1]);

            line = sr.ReadLine();
            line = sr.ReadLine();
            line = sr.ReadLine();
            temp = TypeConverterEx.Split<string>(line);
            CellSize = float.Parse(temp[1]);

            line = sr.ReadLine();
            temp = TypeConverterEx.Split<string>(line);
            NoDataValue = float.Parse(temp[1]);
            List<float> list = new List<float>();

            if (arg == null)
            {          
                for (int i = 0; i < nrow; i++)
                {
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<float>(line);
                    for (int j = 0; j < ncol; j++)
                    {
                        if (buf[j] != NoDataValue)
                            list.Add(buf[j]);
                    }
                }
            }
            else if (arg is IRegularGrid)
            {
                var grid = arg as IRegularGrid;
                for (int i = 0; i < nrow; i++)
                {
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<float>(line);
                    for (int j = 0; j < ncol; j++)
                    {
                        if (grid.IBound[0, i, j] != 0)
                        {
                            list.Add(buf[j]);
                        }
                    }
                }
            }
            list.Clear();
            sr.Close();
            return list.ToArray();
        }

        public DataCube<float> LoadSerial(string filename, int nrow, int ncol)
        {
            Variables = new string[] { Path.GetFileNameWithoutExtension(filename) };
            FileName = filename;
            StreamReader sr = new StreamReader(FileName);
            List<float> list = new List<float>();
            string line = "";

            for (int i = 0; i < nrow; i++)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<float>(line);
                for (int j = 0; j < ncol; j++)
                {
                    if (buf[j] != NoDataValue)
                        list.Add(buf[j]);
                }
            }
            var array = new DataCube<float>(1, 1, list.Count, false);
           array[0, "0", ":"] = list.ToArray();
            list.Clear();
            sr.Close();
            return array;
        }

        public DataCube<float> Load(string filename, DataCube<float> ibound)
        {
            Variables = new string[] { Path.GetFileNameWithoutExtension(filename) };
            FileName = filename;

            StreamReader sr = new StreamReader(FileName);
            List<float> list = new List<float>();
            string line = "";

            for (int i = 0; i < ibound.Size[0]; i++)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<float>(line);
                for (int j = 0; j < ibound.Size[1]; j++)
                {
                    if (ibound[i, j, 0] != 0)
                        list.Add(buf[j]);
                }
            }
            var array = new DataCube<float>(1, 1, list.Count, false);
            array[0, "0", ":"] = list.ToArray();
            list.Clear();
            sr.Close();
            return array;
        }
    }
}
