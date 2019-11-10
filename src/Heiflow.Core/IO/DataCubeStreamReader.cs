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
using Heiflow.Core.Data;
using System.IO;
using System.Xml.Serialization;

namespace Heiflow.Core.IO
{
    public class DataCubeStreamReader : BaseDataCubeStream
    {
        private string _FileName;
        private DataCubeDescriptor _Descriptor;
        private FileStream _FileStream;
        private BinaryReader _BinaryReader;
        private string _ErrorMessage = "";

        public DataCubeStreamReader(string filename)
        {
            _FileName = filename;
            Scale = 1;
            _Descriptor = new DataCubeDescriptor();
            NumTimeStep = 0;
            MaxTimeStep = 0;
        }

        public int FeatureCount
        {
            get;
            private set;
        }

        public float Scale
        {
            get;
            set;
        }

        public void Open()
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            Scan();

            _FileStream = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _BinaryReader = new BinaryReader(_FileStream);

            var varnum = _BinaryReader.ReadInt32();
            Variables = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = _BinaryReader.ReadInt32();
                Variables[i] = new string(_BinaryReader.ReadChars(varname_len)).Trim();
                FeatureCount = _BinaryReader.ReadInt32();
            }
        }

        public void Close()
        {
            _FileStream.Close();
            _BinaryReader.Close();
        }

        public DataCube<float> LoadStep()
        {
            var varnum = Variables.Length;
            DataCube<float> mat = new DataCube<float>(varnum, 1, FeatureCount);
            for (int s = 0; s < FeatureCount; s++)
            {
                for (int v = 0; v < varnum; v++)
                {
                    mat[v,0,s] = _BinaryReader.ReadSingle() * Scale;
                }
            }
            return mat;
        }

        public string[] GetVariables()
        {
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            int varnum = br.ReadInt32();
            var variables = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                variables[i] = new string(br.ReadChars(varname_len)).Trim();
                FeatureCount = br.ReadInt32();
            }
            br.Close();
            fs.Close();

            return variables;
        }
        public override void Scan()
        {
            if (File.Exists(_FileName))
            {
                FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                int varnum = br.ReadInt32();
                Variables = new string[varnum];
                NumTimeStep = 0;

                for (int i = 0; i < varnum; i++)
                {
                    int varname_len = br.ReadInt32();
                    Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                    FeatureCount = br.ReadInt32();
                }

                long stepbyte = Variables.Length * FeatureCount * 4;
                while (!(fs.Position == fs.Length))
                {
                    if (fs.Position > fs.Length)
                    {
                        NumTimeStep--;
                        break;

                    }
                    fs.Seek(stepbyte, SeekOrigin.Current);
                    NumTimeStep++;
                }
                br.Close();
                fs.Close();
                _ErrorMessage = "";
            }
            else
            {
                _ErrorMessage = string.Format("The {0} doesn't exist.", _FileName);
            }
        }

        public override void LoadDataCube()
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if (MaxTimeStep <= 0 || NumTimeStep == 0)
            {
                Scan();
                MaxTimeStep = NumTimeStep;
            }
            int feaNum = 0;
            int varnum = 0;
            int nstep = StepsToLoad;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            try
            {
                varnum = br.ReadInt32();
                Variables = new string[varnum];
                for (int i = 0; i < varnum; i++)
                {
                    int varname_len = br.ReadInt32();
                    Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                    feaNum = br.ReadInt32();
                }
                if (DataCube == null)
                    DataCube = new DataCube<float>(varnum, nstep, FeatureCount);
                OnLoading(0);
                for (int t = 0; t < nstep; t++)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varnum; v++)
                        {
                            DataCube[v, t, s] = br.ReadSingle() * Scale;
                        }
                    }
                    int progress = Convert.ToInt32(t * 100 / NumTimeStep);
                    OnLoading(progress);
                }
                if (_Descriptor.TimeStamps != null)
                {
                    DataCube.DateTimes = new DateTime[nstep];
                    for (int t = 0; t < nstep; t++)
                    {
                        DataCube.DateTimes[t] = _Descriptor.TimeStamps[t];
                    }
                }
                br.Close();
                fs.Close();
                OnDataCubedLoaded(DataCube);
            }
            catch (Exception ex)
            {
                br.Close();
                fs.Close();
                OnLoadFailed("Failed to load. Error message: " + ex.Message);
            }
        }
        public override void LoadDataCube(int var_index)
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if (MaxTimeStep <= 0 || NumTimeStep == 0)
            {
                Scan();
                MaxTimeStep = NumTimeStep;
            }

            int feaNum = 0;
            int varnum = 0;
            int nstep = StepsToLoad;
            int progress = 0;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            varnum = br.ReadInt32();
            Variables = new string[varnum];
            try
            {
                for (int i = 0; i < varnum; i++)
                {
                    int varname_len = br.ReadInt32();
                    Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                    feaNum = br.ReadInt32();
                }
                OnLoading(0);
                if (DataCube == null)
                {
                    DataCube = new DataCube<float>(Variables.Length, nstep, feaNum, true);
                    DataCube.Variables = Variables;
                }
                if (!DataCube.IsAllocated(var_index) || DataCube.Size[1] != nstep)
                    DataCube.Allocate(var_index);
              
                for (int t = 0; t < nstep; t++)
                {
                    var buf = new float[feaNum];
                    for (int s = 0; s < feaNum; s++)
                    {
                        br.ReadBytes(4 * var_index);
                        buf[s] = br.ReadSingle() * Scale;
                        br.ReadBytes(4 * (varnum - var_index - 1));
                    }
                    DataCube.ILArrays[var_index][t, ":"] = buf;
                    progress = Convert.ToInt32(t * 100 / nstep);
                    OnLoading(progress);
                }
                br.Close();
                fs.Close();
                if (_Descriptor.TimeStamps != null)
                {
                    DataCube.DateTimes = new DateTime[nstep];
                    for (int t = 0; t < nstep; t++)
                    {
                        DataCube.DateTimes[t] = _Descriptor.TimeStamps[t];
                    }
                }
                OnDataCubedLoaded(DataCube);
            }
            catch (Exception ex)
            {
                br.Close();
                fs.Close();
                OnLoadFailed("Failed to load. Error message: " + ex.Message);
            }
        }
        public void LoadDataCubeSingle(int var_index)
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if (MaxTimeStep <= 0 || NumTimeStep == 0)
            {
                Scan();
                MaxTimeStep = NumTimeStep;
            }
            int feaNum = 0;
            int varnum = 0;
            int nstep = StepsToLoad;
            int progress = 0;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            try
            {
                varnum = br.ReadInt32();
                Variables = new string[varnum];

                for (int i = 0; i < varnum; i++)
                {
                    int varname_len = br.ReadInt32();
                    Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                    feaNum = br.ReadInt32();
                }
                OnLoading(0);
                if (DataCube == null)
                {
                    DataCube = new DataCube<float>(Variables.Length, nstep, feaNum, true);
                    DataCube.Variables = Variables;
                }
                if (!DataCube.IsAllocated(var_index) || DataCube.Size[1] != nstep)
                    DataCube.Allocate(var_index);
                for (int t = 0; t < nstep; t++)
                {
                    for (int s = 0; s < feaNum; s++)
                    {
                        DataCube[var_index, t, s] = br.ReadSingle() * Scale;
                    }
                    progress = Convert.ToInt32(t * 100 / nstep);
                    OnLoading(progress);
                }
                br.Close();
                fs.Close();
                if (_Descriptor.TimeStamps != null)
                {
                    DataCube.DateTimes = new DateTime[nstep];
                    for (int t = 0; t < nstep; t++)
                    {
                        DataCube.DateTimes[t] = _Descriptor.TimeStamps[t];
                    }
                }
                OnDataCubedLoaded(DataCube);
            }
            catch (Exception ex)
            {
                br.Close();
                fs.Close();
                OnLoadFailed("Failed to load. Error message: " + ex.Message);
            }
        }
        public void LoadDataCubeSingle(Dictionary<int, int> mapping, int var_index)
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if (MaxTimeStep <= 0 || NumTimeStep == 0)
            {
                Scan();
                MaxTimeStep = NumTimeStep;
            }
            int feaNum = 0;
            int varnum = 0;
            int nstep = StepsToLoad;
            int nhru = mapping.Keys.Count;
            int progress = 0;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            try
            {
                varnum = br.ReadInt32();
                Variables = new string[varnum];

                for (int i = 0; i < varnum; i++)
                {
                    int varname_len = br.ReadInt32();
                    Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                    feaNum = br.ReadInt32();
                }

                OnLoading(0);
                if (DataCube == null)
                {
                    DataCube = new DataCube<float>(Variables.Length, nstep, nhru, true);
                    DataCube.Variables = Variables;
                }
                if (!DataCube.IsAllocated(var_index) || DataCube.Size[1] != nstep)
                    DataCube.Allocate(var_index, nstep, nhru);
                var vv = new float[feaNum];
                for (int t = 0; t < nstep; t++)
                {
                    var buf = new float[nhru];
                    for (int i = 0; i < feaNum; i++)
                    {
                        vv[i] = br.ReadSingle() * Scale;                  
                    }
                    for (int i = 0; i < nhru; i++)
                    {
                        buf[i] = vv[mapping[i + 1] - 1];
                    }
                    //for (int i = 0; i < nhru; i++)
                    //{
                    //    //DataCube[var_index, t, i] = vv[mapping[i + 1] - 1];
                    //    DataCube[var_index][t, i] = vv[mapping[i + 1] - 1];
                    //}
                    DataCube[var_index][t, ":"] = buf;
                    progress = Convert.ToInt32(t * 100 / nstep);
                    OnLoading(progress);
                }
                br.Close();
                fs.Close();
                if (_Descriptor.TimeStamps != null)
                {
                    DataCube.DateTimes = new DateTime[nstep];
                    for (int t = 0; t < nstep; t++)
                    {
                        DataCube.DateTimes[t] = _Descriptor.TimeStamps[t];
                    }
                }
                OnDataCubedLoaded(DataCube);
            }
            catch (Exception ex)
            {
                br.Close();
                fs.Close();
                OnLoadFailed("Failed to load. Error message: " + ex.Message);
            }
        }
    }
}
