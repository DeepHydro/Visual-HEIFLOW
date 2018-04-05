// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
    public class DataCubeStreamReader : BaseDataCube
    {
        private string _FileName;
        private DataCubeDescriptor _Descriptor;
        private FileStream _FileStream;
        private BinaryReader _BinaryReader;
        private string _ErrorMessage = "";
        private bool _Scaned;

        public DataCubeStreamReader(string filename)
        {
            _FileName = filename;
            StepsToLoad = -1;
            Scale = 1;
            _Descriptor = new DataCubeDescriptor();
            _Scaned = false;
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

        public My3DMat<float> LoadStep()
        {
            var varnum = Variables.Length;
            My3DMat<float> mat = new My3DMat<float>(varnum, 1, FeatureCount);
            for (int s = 0; s < FeatureCount; s++)
            {
                for (int v = 0; v < varnum; v++)
                {
                    mat.Value[v][0][s] = _BinaryReader.ReadSingle() * Scale;
                }
            }
            return mat;
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
                _Scaned = true;
            }
            else
            {
                _ErrorMessage = string.Format("The {0} doesn't exist.", _FileName);
            }
        }
        /// <summary>
        /// return a 3d mat
        /// </summary>
        /// <returns></returns>
        public override My3DMat<float> Load()
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if(!_Scaned)
                Scan();
            int feaNum = 0;
            int varnum = 0;
            int nstep = NumTimeStep;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
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
            varnum = br.ReadInt32();
            Variables = new string[varnum];
            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;
            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            if(Source == null)
                Source = new MyLazy3DMat<float>(varnum, nstep, FeatureCount);

            for (int v = 0; v < varnum; v++)
            {
                Source.Allocate(v, nstep, FeatureCount);
            }
            OnLoading(0);

            for (int t = 0; t < nstep; t++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        Source.Value[v][t][s] = br.ReadSingle() * Scale;
                    }
                }
                int progress = Convert.ToInt32(t * 100 / NumTimeStep);
                OnLoading(progress);
            }
            if (_Descriptor.TimeStamps != null)
            {
                Source.DateTimes = new DateTime[nstep];
                for (int t = 0; t < nstep; t++)
                {
                    Source.DateTimes [t]= _Descriptor.TimeStamps[t];
                }
            }
            OnLoading(100);
            br.Close();
            fs.Close();
            OnLoaded(Source);
            return Source;
        }

        public override My3DMat<float> Load(int var_index)
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if (!_Scaned)
                Scan();
            int feaNum = 0;
            int varnum = 0;
            int nstep = NumTimeStep;
            int progress = 0;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
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
            varnum = br.ReadInt32();
            Variables = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;

            OnLoading(0);
            if (Source == null)
            {
                Source = new MyLazy3DMat<float>(Variables.Length, nstep, feaNum);
                Source.Variables = Variables;
            }
            if (!Source.IsAllocated(var_index) || Source.Size[1] != nstep)
                Source.Allocate(var_index, nstep, FeatureCount);
            for (int t = 0; t < nstep; t++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    br.ReadBytes(4 * var_index);
                    Source.Value[var_index][t][s] = br.ReadSingle() * Scale;
                    br.ReadBytes(4 * (varnum - var_index - 1));
                }
                progress = Convert.ToInt32(t * 100 / nstep);
                OnLoading(progress);
            }
            OnLoading(100);
            br.Close();
            fs.Close();
            if (_Descriptor.TimeStamps != null)
            {
                Source.DateTimes = new DateTime[nstep];
                for (int t = 0; t < nstep; t++)
                {
                    Source.DateTimes[t] = _Descriptor.TimeStamps[t];
                }
            }
            OnLoaded(Source);
            return Source;
        }
        public  My3DMat<float> LoadSingle(int var_index)
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if (!_Scaned)
                Scan();
            int feaNum = 0;
            int varnum = 0;
            int nstep = NumTimeStep;
            int progress = 0;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
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
            varnum = br.ReadInt32();
            Variables = new string[varnum];

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;

            OnLoading(0);
            if (Source == null)
            {
                Source = new MyLazy3DMat<float>(Variables.Length, nstep, feaNum);
                Source.Variables = Variables;
            }
            if (!Source.IsAllocated(var_index) || Source.Size[1] != nstep)
                Source.Allocate(var_index, nstep, FeatureCount);
            for (int t = 0; t < nstep; t++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    Source.Value[var_index][t][s] = br.ReadSingle() * Scale;
                }
                progress = Convert.ToInt32(t * 100 / nstep);
                OnLoading(progress);
            }
            OnLoading(100);
            br.Close();
            fs.Close();
            if (_Descriptor.TimeStamps != null)
            {
                Source.DateTimes = new DateTime[nstep];
                for (int t = 0; t < nstep; t++)
                {
                    Source.DateTimes[t] = _Descriptor.TimeStamps[t];
                }
            }
            OnLoaded(Source);
            return Source;
        }
        public My3DMat<float> LoadSingle(Dictionary<int, int> mapping, int var_index)
        {
            var xml = _FileName + ".xml";
            if (File.Exists(xml))
            {
                _Descriptor = DataCubeDescriptor.Deserialize(xml);
            }
            if(!_Scaned)
                Scan();
            int feaNum = 0;
            int varnum = 0;
            int nstep = NumTimeStep;
            int nhru = mapping.Keys.Count;
            int progress = 0;
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
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
            varnum = br.ReadInt32();
            Variables = new string[varnum];

            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;

            for (int i = 0; i < varnum; i++)
            {
                int varname_len = br.ReadInt32();
                Variables[i] = new string(br.ReadChars(varname_len)).Trim();
                feaNum = br.ReadInt32();
            }

            OnLoading(0);
            if (Source == null)
            {
                Source = new MyLazy3DMat<float>(Variables.Length, nstep, nhru);
                Source.Variables = Variables;
            }
            if (!Source.IsAllocated(var_index) || Source.Size[1] != nstep)
                Source.Allocate(var_index, nstep, nhru);
            var vv = new float[feaNum];
            for (int t = 0; t < nstep; t++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    vv[s] = br.ReadSingle() * Scale;
                }
                for (int i = 0; i < nhru; i++)
                {
                    Source.Value[var_index][t][i] = vv[mapping[i + 1]];
                }
                progress = Convert.ToInt32(t * 100 / nstep);
                OnLoading(progress);
            }
            OnLoading(100);
            br.Close();
            fs.Close();
            if (_Descriptor.TimeStamps != null)
            {
                Source.DateTimes = new DateTime[nstep];
                for (int t = 0; t < nstep; t++)
                {
                    Source.DateTimes[t] = _Descriptor.TimeStamps[t];
                }
            }
            OnLoaded(Source);
            return Source;
        }
    }
}
