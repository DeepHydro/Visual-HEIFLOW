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
using Heiflow.Models.Generic.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public  class MmsParameterFile
    {
        private string[] dim_name = new string[] { "Dimension" };
        private string _FileName;
        public MmsParameterFile(string filename)
        {
            _FileName = filename;
        }

        public bool IsLoaded { get; set; }

        public string Message { get; set; }

        public  Dictionary<string, IParameter> Read(IPackage owner)
        {
            Dictionary<string, IParameter> parameters = new Dictionary<string, IParameter>();
            if (File.Exists(_FileName))
            {
            
                StreamReader sr = new StreamReader(_FileName);
                var txt = sr.ReadToEnd().Trim();
                var lines = txt.Split(new char[] { '\n' });
                string newline = "";
                int dimRow = 0;
                int paraRow = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].ToLower().Contains("dimensions"))
                    {
                        dimRow = i;
                    }
                    if (lines[i].ToLower().Contains("parameters"))
                    {
                        paraRow = i;
                        break;
                    }
                }
                //read dimensions
                for (int i = dimRow + 1; i < paraRow; )
                {
                    i++;
                    newline = lines[i].Trim();
                    DataCubeParameter<int> gv = new DataCubeParameter<int>(1,1,1)
                    {
                        Dimension = 1,
                        VariableType = ParameterType.Dimension,
                        DimensionNames = dim_name
                    };
                    i++;
                    newline = lines[i].Trim();
                    gv[0, 0, 0] = int.Parse(newline);
                    if (!parameters.Keys.Contains(gv.Name))
                        parameters.Add(gv.Name, gv);
                    i++;
                }
                if (paraRow == 0)
                    paraRow = -1;
                //read parameters
                for (int i = paraRow + 1; i < lines.Length; )
                {
                    i++;
                    newline = TypeConverterEx.Split<string>(lines[i], 1)[0];
                    string name = newline.Trim();
                    i++;
                    newline = lines[i];
                    int Dimension = int.Parse(newline.Trim());
                    string[] dimensionNames = new string[Dimension];
                    for (int d = 0; d < Dimension; d++)
                    {
                        i++;
                        dimensionNames[d] = lines[i].Trim();
                    }
                    i++;
                    newline = lines[i];
                    int ValueCount = int.Parse(newline.Trim());
                    i++;
                    newline = lines[i];
                    int ValueType = int.Parse(newline.Trim());
                    int nrow = int.Parse(parameters[dimensionNames[0]].GetValue(0, 0, 0).ToString());
                    int ncol = 1;
                    if (Dimension > 1)
                        ncol = int.Parse(parameters[dimensionNames[1]].GetValue(0, 0, 0).ToString());
                    if (ValueType == 0)
                    {
                        DataCubeParameter<short> gv = new DataCubeParameter<short>(1,nrow,ncol)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Name=name,
                            Owner = owner,
                        };
                        gv.FromStringArrays(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 1)
                    {
                        DataCubeParameter<int> gv = new DataCubeParameter<int>(1, nrow, ncol)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Name=name,
                            Owner = owner,

                        };
                        gv.FromStringArrays(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 2)
                    {
                        DataCubeParameter<float> gv = new DataCubeParameter<float>(1,nrow,ncol)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Name=name,
                            Owner = owner
                        };
                        gv.FromStringArrays(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 3)
                    {
                        DataCubeParameter<double> gv = new DataCubeParameter<double>(1,nrow,ncol)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Name = name,
                            Owner = owner
                        };
                        gv.FromStringArrays(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 4)
                    {
                        DataCubeParameter<string> gv = new DataCubeParameter<string>(1,nrow,ncol)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Name=name,
                            Owner = owner
                        };
                        gv.FromStringArrays(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }

                    i += ValueCount + 1;
                }
                IsLoaded = true;

                foreach (var pr in parameters.Values)
                    pr.Owner = owner;

                return parameters;
            }
            else
            {
                Message = string.Format("\r\n Failed to load . The package file does not exist: {0}", _FileName);
                IsLoaded = false;
                return parameters;
            }
        }
    }
}
