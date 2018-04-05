// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
                    SingleParam<int> gv = new SingleParam<int>(newline)
                    {
                        Dimension = 1,
                        VariableType = ParameterType.Dimension,
                        DimensionNames = dim_name
                    };
                    i++;
                    newline = lines[i].Trim();
                    gv.Value = int.Parse(newline);
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
                    if (ValueType == 0)
                    {
                        ArrayParam<short> gv = new ArrayParam<short>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Owner = owner,
                        };
                        gv.Values = TypeConverterEx.ChangeType<short>(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 1)
                    {
                        ArrayParam<int> gv = new ArrayParam<int>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Owner = owner,

                        };
                        gv.Values = TypeConverterEx.ChangeType<int>(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 2)
                    {
                        ArrayParam<float> gv = new ArrayParam<float>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Owner = owner
                        };
                        gv.Values = TypeConverterEx.ChangeType<float>(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 3)
                    {
                        ArrayParam<double> gv = new ArrayParam<double>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Owner = owner
                        };
                        gv.Values = TypeConverterEx.ChangeType<double>(lines, i + 1, i + ValueCount);
                        if (!parameters.Keys.Contains(gv.Name))
                            parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 4)
                    {
                        ArrayParam<string> gv = new ArrayParam<string>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = Dimension,
                            DimensionNames = dimensionNames,
                            Owner = owner
                        };
                        gv.Values = TypeConverterEx.ChangeType<string>(lines, i + 1, i + ValueCount);
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
