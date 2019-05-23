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
using Heiflow.Core.Hydrology;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public class FHDFile : BaseDataCubeStream
    {
        private string _FileName;
        private IRegularGrid _Grid;
        public FHDFile(string filename, IRegularGrid grid)
        {
            _FileName = filename;
            _Grid = grid;
            IsLoadDepth = false;
        }

        public bool IsLoadDepth
        {
            get;
            set;
        }

        public override void Scan()
        {
            if (File.Exists(_FileName))
            {
                int step = 0;
                FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);

                long layerbyte = 32 + 4 * 3 + _Grid.RowCount * _Grid.ColumnCount * 4;

                while (fs.Position < fs.Length)
                {
                    for (int l = 0; l < _Grid.ActualLayerCount; l++)
                    {
                        fs.Seek(layerbyte, SeekOrigin.Current);
                    }
                    step++;
                }
                br.Close();
                fs.Close();
                NumTimeStep = step;
            }
        }

        public void LoadVertDis(int var_index)
        {
            if (var_index == 0)
            {
                 ReadBinTotalVertDis();
            }
            else
            {
                 ReadBinLayerHead(var_index);
            }
        }

        public void ReadTxtWaterTable()
        {
            if (File.Exists(_FileName))
            {
                var grid = _Grid as MFGrid;
                StreamReader srFhd = new StreamReader(_FileName);
                string headline = srFhd.ReadLine();
                string[] strs = TypeConverterEx.Split<string>(headline);
                int step = int.Parse(strs[0]);
                int sp = int.Parse(strs[1]);
                int col = int.Parse(strs[5]);
                int row = int.Parse(strs[6]);
                srFhd.Close();
                int nlayer = grid.ActualLayerCount;
                string line = "";
                int stepIndex = 0;
                int nstep = StepsToLoad;
                srFhd = new StreamReader(_FileName);

                int colLine = (int)Math.Ceiling(col / 10.0);
                float head = 0;
                float[][] heads = new float[grid.ActualLayerCount][];
                List<float[]> headLst = new List<float[]>();
                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    heads[l] = new float[grid.ActiveCellCount];
                }

                while (!srFhd.EndOfStream)
                {
                    float[] wt = new float[grid.ActiveCellCount];
                    for (int l = 0; l < nlayer; l++)
                    {
                        headline = srFhd.ReadLine();
                        strs = Regex.Split(headline.Trim(), @"[ ]+");
                        step = int.Parse(strs[0]);
                        sp = int.Parse(strs[1]);
                        int index = 0;
                        for (int r = 0; r < row; r++)
                        {
                            line = "";
                            for (int i = 0; i < colLine; i++)
                            {
                                line += srFhd.ReadLine() + " ";
                            }
                            strs = TypeConverterEx.Split<string>(line);
                            for (int c = 0; c < strs.Length; c++)
                            {
                                head = float.Parse(strs[c]);
                                if (grid.IBound[0, r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }
                        float[] lwt = new float[grid.ActualLayerCount];
                        for (int i = 0; i < grid.ActiveCellCount; i++)
                        {
                            for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                            {
                                lwt[ll] = heads[ll][i];
                            }
                            wt[i] = lwt.Max();
                        }
                    }
                    headLst.Add(wt);
                    stepIndex++;
                    if (stepIndex >= nstep)
                        break;
                }
                if (DataCube == null)
                    DataCube = new DataCube<float>(Variables.Length, nstep, grid.ActualLayerCount, true);
                DataCube.Allocate(0);
                for (int i = 0; i < headLst.Count; i++)
                {
                    DataCube.ILArrays[0][i] = headLst[i];
                }

                srFhd.Close();
                heads = null;
                headLst.Clear();
                OnDataCubedLoaded(DataCube);
            }
            else
            {
                OnLoadFailed("The FHD file dose not exist: " + _FileName);
            }
        }
        /// <summary>
        /// read layer head 
        /// </summary>
        /// <param name="layer">index starting from 1</param>
        /// <returns></returns>
        public void ReadBinLayerHead(int layer)
        {
            if (File.Exists(_FileName))
            {
                OnLoading(0);
                if (MaxTimeStep <= 0 || NumTimeStep == 0)
                {
                    Scan();
                    MaxTimeStep = NumTimeStep;
                }
                var grid = _Grid as MFGrid;
                FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                // KSTP,KPER,PERTIM,TOTIM,TEXT,NCOL,NROW,ILAY
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                int nstep = StepsToLoad;
                float head = 0;
                int progress = 0;

                layer = layer - 1;
                if (layer < 0)
                    layer = 0;
               // MyLazy3DMat<float> mat = new MyLazy3DMat<float>(Variables.Length, nstep, grid.ActiveCellCount);
                if (DataCube == null)
                    DataCube = new DataCube<float>(Variables.Length, nstep, grid.ActualLayerCount, true);
                DataCube.Allocate(layer + 1);
                for (int t = 0; t < nstep; t++)
                {
                    for (int l = 0; l < layer; l++)
                    {
                        fs.Seek(layerbyte, SeekOrigin.Current);
                    }
                    fs.Seek(32, SeekOrigin.Current);
                    var vv = br.ReadInt32();
                    vv = br.ReadInt32();
                    vv = br.ReadInt32();
                    int index = 0;
                    var buf = new float[grid.ActiveCellCount];
                    for (int r = 0; r < grid.RowCount; r++)
                    {
                        for (int c = 0; c < grid.ColumnCount; c++)
                        {
                            head = br.ReadSingle();
                            if (grid.IBound[0, r, c] != 0)
                            {
                                buf[index] = head;
                                index++;
                            }
                        }
                    }
                    DataCube.ILArrays[layer + 1][t, ":"] = buf;
                    for (int l = layer +1 ; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(layerbyte, SeekOrigin.Current);
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
            else
            {
                OnLoadFailed("The FHD file dose not exist: " + _FileName);
            }
        }
        /// <summary>
        /// read layer head 
        /// </summary>
        /// <param name="layer">index starting from 1</param>
        /// <returns></returns>
        public void ReadBinLayerDepth(int layer)
        {
            if (File.Exists(_FileName))
            {
                OnLoading(0);
                if (MaxTimeStep <= 0 || NumTimeStep == 0)
                {
                    Scan();
                    MaxTimeStep = NumTimeStep;
                }
                var grid = _Grid as MFGrid;
                FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                int nstep = StepsToLoad;
                float head = 0;
                int progress = 0;

                layer = layer - 1;
                if (layer < 0)
                    layer = 0;
                if (DataCube == null)
                    DataCube = new DataCube<float>(Variables.Length, nstep, grid.ActualLayerCount, true);
                DataCube.Allocate(layer + 1);
                for (int t = 0; t < nstep; t++)
                {
                    for (int l = 0; l < layer; l++)
                    {
                        fs.Seek(layerbyte, SeekOrigin.Current);
                    }
                    fs.Seek(32, SeekOrigin.Current);
                    var vv = br.ReadInt32();
                    vv = br.ReadInt32();
                    vv = br.ReadInt32();
                    int index = 0;
                    var buf = new float[grid.ActiveCellCount];
                    for (int r = 0; r < grid.RowCount; r++)
                    {
                        for (int c = 0; c < grid.ColumnCount; c++)
                        {
                            head = br.ReadSingle();
                            if (grid.IBound[0, r, c] != 0)
                            {
                                buf[index] = grid.Elevations[0, 0, index] - head;
                                index++;
                            }
                        }
                    }
                    DataCube.ILArrays[layer + 1][t, ":"] = buf;
                    for (int l = layer + 1; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(layerbyte, SeekOrigin.Current);
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
            else
            {
                OnLoadFailed("The FHD file dose not exist: " + _FileName);
            }
        }
        public void ReadBinTotalVertDis()
        {
            if (File.Exists(_FileName))
            {
                OnLoading(0);
                if (MaxTimeStep <= 0 || NumTimeStep == 0)
                {
                    Scan();
                    MaxTimeStep = NumTimeStep;
                }
                var grid = _Grid as MFGrid;
                FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                int nstep = StepsToLoad;
                float head = 0;
                int progress = 0;
                float[][] vts = new float[grid.ActualLayerCount][];

                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    vts[l] = new float[grid.ActiveCellCount];
                }

                if (DataCube == null)
                    DataCube = new DataCube<float>(Variables.Length, nstep, grid.ActualLayerCount, true);
                DataCube.Allocate(0);
                float total_vt = 0;
                for (int t = 0; t < nstep; t++)
                {
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(32, SeekOrigin.Current);
                        var vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        int index = 0;
                        for (int r = 0; r < grid.RowCount; r++)
                        {
                            for (int c = 0; c < grid.ColumnCount; c++)
                            {
                                head = br.ReadSingle();
                                if (grid.IBound[0, r, c] != 0)
                                {
                                    vts[l][index] = head;
                                    index++;
                                }
                            }
                        }
                    }
                  
                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        total_vt = 0;
                        for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                        {
                            total_vt += vts[ll][i];
                        }
                        DataCube[0, t, i] = total_vt / grid.ActualLayerCount;
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
            else
            {

            }
        }
        public void ReadBinWaterTable()
        {
            if (File.Exists(_FileName))
            {
                OnLoading(0);
                if (MaxTimeStep <= 0 || NumTimeStep == 0)
                {
                    Scan();
                    MaxTimeStep = NumTimeStep;
                }
                var grid = _Grid as MFGrid;
                FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                int nstep = StepsToLoad;
                float head = 0;
                int progress = 0;
                float[][] heads = new float[grid.ActualLayerCount][];

                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    heads[l] = new float[grid.ActiveCellCount];
                }

                if (DataCube == null)
                    DataCube = new DataCube<float>(Variables.Length, nstep, grid.ActualLayerCount, true);
                DataCube.Allocate(0);
                float[] lwt = new float[grid.ActualLayerCount];
                for (int t = 0; t < nstep; t++)
                {
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(32, SeekOrigin.Current);
                        var vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        int index = 0;
                        for (int r = 0; r < grid.RowCount; r++)
                        {
                            for (int c = 0; c < grid.ColumnCount; c++)
                            {
                                head = br.ReadSingle();
                                if (grid.IBound[0, r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }
                        var buf = new float[grid.ActiveCellCount];
                        for (int i = 0; i < grid.ActiveCellCount; i++)
                        {
                            for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                            {
                                lwt[ll] = heads[ll][i];
                            }
                            buf[i] = lwt.Max();
                        }
                        DataCube.ILArrays[0][t, ":"] = buf;
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
            else
            {
                OnLoadFailed("The FHD file dose not exist: " + _FileName);
            }
        }

        public void ReadBinWaterDepth()
        {
            if (File.Exists(_FileName))
            {
                OnLoading(0);
                if (MaxTimeStep <= 0 || NumTimeStep == 0)
                {
                    Scan();
                    MaxTimeStep = NumTimeStep;
                }
                var grid = _Grid as MFGrid;
                FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                long layerbyte = 32 + 4 * 3 + grid.RowCount * grid.ColumnCount * 4;
                int nstep = StepsToLoad;
                float head = 0;
                int progress = 0;
                float[][] heads = new float[grid.ActualLayerCount][];

                for (int l = 0; l < grid.ActualLayerCount; l++)
                {
                    heads[l] = new float[grid.ActiveCellCount];
                }

                if (DataCube == null)
                    DataCube = new DataCube<float>(Variables.Length, nstep, grid.ActualLayerCount,true);
                DataCube.Allocate(0);
                float[] lwt = new float[grid.ActualLayerCount];
                for (int t = 0; t < nstep; t++)
                {
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        fs.Seek(32, SeekOrigin.Current);
                        var vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        vv = br.ReadInt32();
                        int index = 0;
                        for (int r = 0; r < grid.RowCount; r++)
                        {
                            for (int c = 0; c < grid.ColumnCount; c++)
                            {
                                head = br.ReadSingle();
                                if (grid.IBound[0, r, c] != 0)
                                {
                                    heads[l][index] = head;
                                    index++;
                                }
                            }
                        }
                    }
                    var buf = new float[grid.ActiveCellCount];
                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        for (int ll = 0; ll < grid.ActualLayerCount; ll++)
                        {
                            lwt[ll] = heads[ll][i];
                        }
                       buf[i] = grid.Elevations[0, 0, i] - lwt.Max();
                    }
                    DataCube.ILArrays[0][t, ":"] = buf;
                    progress = Convert.ToInt32(t * 100 / nstep);
                    OnLoading(progress);
                }
                if (progress < 100)
                    OnLoading(100);
                br.Close();
                fs.Close();
                OnDataCubedLoaded(DataCube);
            }
            else
            {
                OnLoadFailed("The FHD file dose not exist: " + _FileName);
            }
        }

        public override void LoadDataCube()
        {
         
        }

        public override void LoadDataCube(int var_index)
        {
            try
            {
                if (var_index == 0)
                {
                    if (IsLoadDepth)
                         ReadBinWaterDepth();
                    else
                         ReadBinWaterTable();
                }
                else
                {
                    if (IsLoadDepth)
                         ReadBinLayerDepth(var_index);
                    else
                         ReadBinLayerHead(var_index);
                }
            }
            catch (Exception ex)
            {
                OnLoadFailed(ex.Message);
            }
        }
    }
}
