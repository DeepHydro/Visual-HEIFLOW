// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public class CBCFile : BaseDataCube
    {
        private string _FileName;
        private IRegularGrid _Grid;
        public CBCFile(string filename, IRegularGrid grid)
        {
            _FileName = filename;
            _Grid = grid;
            Layer = 0;
            Scale = 1;
        }

        public int Layer
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public override void Scan()
        {
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            List<string> vnLst = new List<string>();
            long layerbyte = _Grid.RowCount * _Grid.ColumnCount * 4;
            NumTimeStep = 0;

            while (fs.Position != fs.Length)
            {
                fs.Seek(4 * 2, SeekOrigin.Current);
                var vn = new string(br.ReadChars(16)).Trim();
                fs.Seek(4 * 3, SeekOrigin.Current);
                if (vnLst.Contains(vn))
                {
                    NumTimeStep++;
                     fs.Seek(layerbyte * _Grid.ActualLayerCount, SeekOrigin.Current);
                }
                else
                {
                    fs.Seek(layerbyte * _Grid.ActualLayerCount, SeekOrigin.Current);
                    vnLst.Add(vn);
                }
            }

            NumTimeStep = NumTimeStep / vnLst.Count;
            Variables = vnLst.ToArray();

            br.Close();
            fs.Close();
        }

        public override My3DMat<float> Load()
        {
            Scan();
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            int nstep = NumTimeStep;
            float vv = 0;
            long layerbyte = _Grid.RowCount * _Grid.ColumnCount * 4;
            fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            br = new BinaryReader(fs);

            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;

            var mat = Source;
            mat.Name = "CBC";

            for (int v = 0; v < Variables.Length; v++)
            {
                mat.Allocate(v, nstep, _Grid.ActiveCellCount);
            }

            for (int s = 0; s < NumTimeStep; s++)
            {
                for (int v = 0; v < Variables.Length; v++)
                {
                    fs.Seek(4 * 2, SeekOrigin.Current);
                    var vn = new string(br.ReadChars(16)).Trim();
                    fs.Seek(4 * 3, SeekOrigin.Current);
                    for (int l = 0; l < _Grid.ActualLayerCount; l++)
                    {
                        if (l == Layer)
                        {
                            int index = 0;
                            for (int r = 0; r < _Grid.RowCount; r++)
                            {
                                for (int c = 0; c < _Grid.ColumnCount; c++)
                                {
                                    vv = br.ReadSingle();
                                    if (_Grid.IBound[Layer, r, c] != 0)
                                    {
                                        mat.Value[v][s][index] = vv;
                                        index++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            fs.Seek(layerbyte, SeekOrigin.Current);
                        }
                    }
                }
            }
       
            br.Close();
            fs.Close();
            return mat;
        }

        public override Core.Data.My3DMat<float> Load(int var_index)
        {
            OnLoading(0);
            Scan();
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            int nstep = NumTimeStep;
            float vv = 0;
            long layerbyte = _Grid.RowCount * _Grid.ColumnCount * 4;
            long var_byte = 8 + 16 + 12 + layerbyte * _Grid.ActualLayerCount;
            int progress = 0;

            fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            br = new BinaryReader(fs);

            if (StepsToLoad < NumTimeStep && StepsToLoad > 0)
                nstep = StepsToLoad;

            if(Source == null)
                Source = new MyLazy3DMat<float>(Variables.Length, nstep, _Grid.ActiveCellCount);
            var mat = Source;
            mat.Allocate(var_index, nstep, _Grid.ActiveCellCount);
            
            for (int t = 0; t < nstep; t++)
            {
                for (int v = 0; v < var_index; v++)
                {
                    fs.Seek(var_byte, SeekOrigin.Current);
                }
                fs.Seek(4 * 2, SeekOrigin.Current);
                var vn = new string(br.ReadChars(16)).Trim();
                fs.Seek(4 * 3, SeekOrigin.Current);
                for (int l = 0; l < _Grid.ActualLayerCount; l++)
                {
                    if (l == Layer)
                    {
                        int index = 0;
                        for (int r = 0; r < _Grid.RowCount; r++)
                        {
                            for (int c = 0; c < _Grid.ColumnCount; c++)
                            {
                                vv = br.ReadSingle();
                                if (_Grid.IBound[Layer, r, c] != 0)
                                {
                                    mat.Value[var_index][t][index] = vv * Scale;
                                    index++;
                                }
                            }
                        }
                    }
                    else
                    {
                        fs.Seek(layerbyte, SeekOrigin.Current);
                    }
                }

                for (int v = var_index + 1; v < Variables.Length; v++)
                {
                    fs.Seek(var_byte, SeekOrigin.Current);
                }
                progress = Convert.ToInt32(t * 100 / nstep);
                OnLoading(progress);
            }
            if (progress < 100)
                OnLoading(100);
            br.Close();
            fs.Close();
            OnLoaded(mat);
            return mat;
        }
    }
}
