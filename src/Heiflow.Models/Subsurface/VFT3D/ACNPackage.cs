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
using Heiflow.Models.Generic;
using System.IO;
using ILNumerics;
using System.ComponentModel;
using System.Diagnostics;
using Heiflow.Core.Data;
using System.Text.RegularExpressions;
using Heiflow.Models.IO;
using Heiflow.Core.Data.ODM;
using Heiflow.Core;
using System.Windows.Forms;
using DotSpatial.Data;
using Heiflow.Models.Subsurface;

namespace Heiflow.Models.Subsurface.VFT3D
{
    public class ACNPackage : MFDataPackage
    {
        public static string PackageName = "ACN";
        public ACNPackage()
        {
            Name = "ACN";
            _FullName = "Component Concentration";
            _MaxTimeStep = -1;
            Layer = 0;
            NumTimeStep = 0;
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".acn";
            _PackageInfo.ModuleName = "ACN";
            IsMandatory = true;
            Version = "ACN";
            _Layer3DToken = "RegularGrid";
            Description = "";
            Category = Modflow.PHTCategory;
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            StartOfLoading = TimeService.Start;
            EndOfLoading = TimeService.End;
            NumTimeStep = TimeService.IOTimeline.Count;
            base.Initialize();
        }

        public override bool Scan()
        {
            var grid = Owner.Grid as MFGrid;
            var mf = Owner as Modflow;
            var phc = Owner.GetPackage(PHCPackage.PackageName) as PHCPackage;
            Variables = phc.AllComponentsNames;
            NumTimeStep = TimeService.Timeline.Count;
            _StartLoading = TimeService.Start;
            MaxTimeStep = TimeService.Timeline.Count;
            _StartLoading = TimeService.Start;
            return true;
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            if (File.Exists(FileName))
            {
                try
                {
                    return LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ShowWarning(ex.Message, progress);
                    return LoadingState.Warning;
                }
            }
            else
            {
                return LoadingState.Warning;
            }
        }

        public override LoadingState Load(int var_index, ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            _ProgressHandler = progress;
            var list = TimeService.Timeline;
            TimeService.IOTimeline = list;
            NumTimeStep = list.Count;
            var grid = Owner.Grid as MFGrid;
            if (DataCube == null || DataCube.Size[1] != StepsToLoad)
            {
                if (LoadAllLayers)
                {
                    DataCube = new DataCube<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount * grid.ActualLayerCount, true)
                    {
                        Name = "ACN"
                    };
                }
                else
                {
                    DataCube = new DataCube<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount, true)
                    {
                        Name = "ACN"
                    };
                }
                DataCube.Variables = this.Variables;
            }
            DataCube.Allocate(var_index, StepsToLoad, grid.ActiveCellCount * grid.ActualLayerCount);
            DataCube.Topology = (this.Grid as RegularGrid).Topology;
            DataCube.DateTimes = this.TimeService.Timeline.Take(StepsToLoad).ToArray();
            var fn = string.Format("PHT3D{0}.ACN", (var_index + 1).ToString().PadLeft(3, '0'));
            //   var file = Path.Combine(Path.GetDirectoryName(this.FileName), fn);
            var file = Path.Combine(Owner.Project.AbsolutePathToProjectFile, fn);
            if (File.Exists(file))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(file);
                    var cell_index = grid.Topology.GetIndexIn2DMat();

                    string line = "";
                    float[] cells = new float[grid.RowCount * grid.ColumnCount];
                    int prog = 0;
                    for (int t = 0; t < StepsToLoad; t++)
                    {
                        for (int k = 0; k < grid.ActualLayerCount; k++)
                        {
                            int c = 0;
                            for (int i = 0; i < grid.RowCount; i++)
                            {
                                for (int j = 0; j < grid.ColumnCount; j++)
                                {
                                    line = sr.ReadLine();
                                    cells[c] = float.Parse(line.Trim());
                                    c++;
                                }
                            }
                            for (int a = 0; a < grid.ActiveCellCount; a++)
                            {
                                DataCube[var_index, t, k * grid.ActiveCellCount + a] = cells[cell_index[a]];
                            }
                        }
                        prog = Convert.ToInt32(t * 100 / StepsToLoad);
                        OnLoading(prog);
                    }
                    result = LoadingState.Normal;
                    OnLoaded(progress, new LoadingObjectState() { Message = Message, Object = this, State = result });
                }
                catch (Exception ex)
                {
                    ShowWarning(ex.Message, progress);
                    result = LoadingState.Warning;
                    acn_LoadFailed(this, ex.Message);
                }
                finally
                {
                    sr.Close();
                }
            }
            else
            {
                Message = "The file does not exist: " + file;
                ShowWarning(Message, progress);
                result = LoadingState.Warning;
                acn_LoadFailed(this, Message);
            }
            return result;
        }

        public override void Clear()
        {
            if (_Initialized)
            {
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            }
            base.Clear();
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }


        private void fhd_Loading(object sender, int e)
        {
            OnLoading(e);
        }

        private void fhd_DataCubeLoaded(object sender, DataCube<float> e)
        {
            OnLoaded(_ProgressHandler, new LoadingObjectState());
        }
        private void acn_LoadFailed(object sender, string e)
        {
            OnLoaded(_ProgressHandler, new LoadingObjectState() { Message = e, State = LoadingState.Warning });
        }
    }
}