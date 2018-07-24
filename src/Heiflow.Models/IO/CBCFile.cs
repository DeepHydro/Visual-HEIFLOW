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
    public class CBCFile : BaseDataCubeStream
    {
        private string _FileName;
        private IRegularGrid _Grid;
        public CBCFile(string filename, IRegularGrid grid)
        {
            _FileName = filename;
            _Grid = grid;
            Layer = 0;
            Scale = 1;
            LoadingBehavior =  MFLoadingLayersBehavior.None;
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
        public MFLoadingLayersBehavior LoadingBehavior
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

        public override void LoadDataCube()
        {
            if (MaxTimeStep <= 0 || NumTimeStep == 0)
            {
                Scan();
                MaxTimeStep = NumTimeStep;
            }
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            int nstep = StepsToLoad;
            float vv = 0;
            long layerbyte = _Grid.RowCount * _Grid.ColumnCount * 4;
            fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            br = new BinaryReader(fs);


            DataCube = new DataCube<float>(Variables.Length, nstep, _Grid.ActiveCellCount);

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
                            var buf = new float[_Grid.ActiveCellCount];
                            for (int r = 0; r < _Grid.RowCount; r++)
                            {
                                for (int c = 0; c < _Grid.ColumnCount; c++)
                                {
                                    vv = br.ReadSingle();
                                    if (_Grid.IBound[Layer, r, c] != 0)
                                    {
                                        buf[index] = vv;
                                        index++;
                                    }
                                }
                            }
                            DataCube.ILArrays[v][s, ":"] = buf;
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

        }

        public override void LoadDataCube(int var_index)
        {
            OnLoading(0);
            if (MaxTimeStep <= 0 || NumTimeStep == 0)
            {
                Scan();
                MaxTimeStep = NumTimeStep;
            }
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            int nstep = StepsToLoad;
            float vv = 0;
            long layerbyte = _Grid.RowCount * _Grid.ColumnCount * 4;
            long var_byte = 8 + 16 + 12 + layerbyte * _Grid.ActualLayerCount;
            int progress = 0;

            fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            br = new BinaryReader(fs);
            if (DataCube == null)
                DataCube = new DataCube<float>(Variables.Length, nstep, _Grid.ActualLayerCount, true);
            DataCube.Allocate(var_index);
            for (int t = 0; t < nstep; t++)
            {
                for (int v = 0; v < var_index; v++)
                {
                    fs.Seek(var_byte, SeekOrigin.Current);
                }
                fs.Seek(4 * 2, SeekOrigin.Current);
                var vn = new string(br.ReadChars(16)).Trim();
                fs.Seek(4 * 3, SeekOrigin.Current);
                if ( LoadingBehavior == MFLoadingLayersBehavior.Sum)
                {
                    var buf = new float[_Grid.ActiveCellCount];
                    for (int l = 0; l < _Grid.ActualLayerCount; l++)
                    {
                        int index = 0;
                        for (int r = 0; r < _Grid.RowCount; r++)
                        {
                            for (int c = 0; c < _Grid.ColumnCount; c++)
                            {
                                vv = br.ReadSingle();
                                if (_Grid.IBound[Layer, r, c] != 0)
                                {
                                    buf[index] = buf[index] + vv * Scale;
                                    index++;
                                }
                            }
                        }
                    }
                    DataCube.ILArrays[var_index][t, ":"] = buf;
                }
                else if (LoadingBehavior == MFLoadingLayersBehavior.Average)
                {
                    var buf = new float[_Grid.ActiveCellCount];
                    for (int l = 0; l < _Grid.ActualLayerCount; l++)
                    {
                        int index = 0;
                        for (int r = 0; r < _Grid.RowCount; r++)
                        {
                            for (int c = 0; c < _Grid.ColumnCount; c++)
                            {
                                vv = br.ReadSingle();
                                if (_Grid.IBound[Layer, r, c] != 0)
                                {
                                    buf[index] = buf[index] + vv * Scale;
                                    index++;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < _Grid.ActiveCellCount; i++)
                    {
                        buf[i] = buf[i] / _Grid.ActualLayerCount;
                    }
                    DataCube.ILArrays[var_index][t, ":"] = buf;
                }
                else if (LoadingBehavior == MFLoadingLayersBehavior.None)
                {
                    for (int l = 0; l < _Grid.ActualLayerCount; l++)
                    {
                        if (l == Layer)
                        {
                            int index = 0;
                            var buf = new float[_Grid.ActiveCellCount];
                            for (int r = 0; r < _Grid.RowCount; r++)
                            {
                                for (int c = 0; c < _Grid.ColumnCount; c++)
                                {
                                    vv = br.ReadSingle();
                                    if (_Grid.IBound[Layer, r, c] != 0)
                                    {
                                        buf[index] = vv * Scale;
                                        index++;
                                    }
                                }
                            }
                            DataCube.ILArrays[var_index][t, ":"] = buf;
                        }
                        else
                        {
                            fs.Seek(layerbyte, SeekOrigin.Current);
                        }
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
            OnDataCubedLoaded(DataCube);
        }
    }
}
