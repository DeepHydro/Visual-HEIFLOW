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

namespace Heiflow.Models.Subsurface
{
    public class FHDPackage : MFDataPackage
    {
        public static string PackageName = "FHD";
        public FHDPackage()
        {
            Name = "FHD";
            _FullName = "Flow Head Data";
            _MaxTimeStep = -1;
            Layer = 0;
            NumTimeStep = 0;
            var vv = new string[] { "Water Table", "Groundwater Head" };
            Variables = vv;
            _PackageInfo.Format = FileFormat.Binary;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".fhd";
            _PackageInfo.ModuleName = "DATA";
            IsMandatory = true;
            Version = "FHD";
            _Layer3DToken = "RegularGrid";
            Description = "Groundwater head output";
        }
        [Category("File")]
        public bool LoadAsDepth
        {
            get;
            set;
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
        public override bool Load(ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            if (File.Exists(FileName))
            {
                try
                {
                    FHDFile fhd = new FHDFile(FileName, Owner.Grid as IRegularGrid);
                    fhd.DataCube = this.DataCube;
                    fhd.Variables = this.Variables;
                    fhd.MaxTimeStep = this.MaxTimeStep;
                    fhd.NumTimeStep = this.NumTimeStep;
                    fhd.IsLoadDepth = LoadAsDepth;
                    fhd.Loading += fhd_Loading;
                    fhd.DataCubeLoaded += fhd_DataCubeLoaded;
                    //TODO: require modifcation
                    fhd.LoadDataCube();
                    return true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool Scan()
        {
            var grid = Owner.Grid as MFGrid;
            var vv = new string[grid.ActualLayerCount + 1];
            if (this.LoadAsDepth)
            {
                vv[0] = "Depth to Groudwater";
            }
            else
                vv[0] = "Groudwater Table";
            for (int ll = 0; ll < grid.ActualLayerCount; ll++)
            {
                if (this.LoadAsDepth)
                    vv[ll + 1] = string.Format("Layer {0} Depth", ll + 1);
                else
                    vv[ll + 1] = string.Format("Layer {0} Head", ll + 1);
            }
            Variables = vv;
            var list = TimeService.GetIOTimeFromFile((Owner as Modflow).IOLogFile);
            if (list.Count > 0)
            {
                TimeService.IOTimeline = list;
                NumTimeStep = list.Count;
                _StartLoading = TimeService.Start;
                MaxTimeStep = list.Count;
            }
            return true;
        }

        public override bool Load(int var_index, ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            var list = TimeService.GetIOTimeFromFile((Owner as Modflow).IOLogFile);
            if (list.Count > 0)
            {
                TimeService.IOTimeline = list;
                NumTimeStep = list.Count;
            }
            if (File.Exists(LocalFileName))
            {
                try
                {
                    var grid = Owner.Grid as MFGrid;
                    if (DataCube == null || DataCube.Size[1] != StepsToLoad)
                    {
                        DataCube = new DataCube<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount, true)
                        {
                            Name = "FHD"
                        };
                        DataCube.Variables = this.Variables;
                    }
                    DataCube.Topology = (this.Grid as RegularGrid).Topology;
                    DataCube.DateTimes = this.TimeService.IOTimeline.Take(StepsToLoad).ToArray();
                    FHDFile fhd = new FHDFile(LocalFileName, grid);
                    fhd.Variables = this.Variables;
                    fhd.MaxTimeStep = this.StepsToLoad;
                    fhd.NumTimeStep = this.NumTimeStep;
                    fhd.DataCube = this.DataCube;
                    fhd.IsLoadDepth = LoadAsDepth;
                    fhd.Loading += fhd_Loading;
                    fhd.DataCubeLoaded += fhd_DataCubeLoaded;
                    fhd.LoadFailed += fhd_LoadFailed;
                    fhd.LoadDataCube(var_index);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    OnLoadFailed(ex.Message, progress);
                    return false;
                }
            }
            else
            {
                OnLoadFailed("The file does not exist: " + LocalFileName, progress);
                return false;
            }
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

        public void ExtractTo(IEnumerable<IObservationsSite> wells, int[] intevals, DateTime start)
        {
            int nwell = wells.Count();
            int step = intevals.Length;
            var mfgrid = Owner.Grid as MFGrid;
            DateTime[] dates = new DateTime[step];
            var current = start.AddDays(0);
            for (int t = 0; t < step; t++)
            {
                current = current.AddDays(intevals[t]);
                dates[t] = current;
            }
            foreach (var site in wells)
            {
                var well = site as HeadObservation;
                var index = mfgrid.Topology.CellID2CellIndex[well.CellID];
                int count = 0;
                var values = new double[step];
                for (int t = 0; t < step; t++)
                {
                    float av = 0;
                    for (int i = 0; i < intevals[t]; i++)
                    {
                        av += DataCube[0, count, index];
                        count++;
                    }
                    av /= intevals[t];
                    values[t] = av;
                }
                well.Variables = new Heiflow.Core.Data.ODM.Variable[1];
                well.Variables[0] = new Heiflow.Core.Data.ODM.Variable()
                {
                    Name = Variables[0]
                };
                well.TimeSeries = new DataCube<double>(values, dates);
                well.TimeSeries.Name = well.Name;
            }
        }

        public DataCube<float> ExtractTo(IEnumerable<IObservationsSite> wells, int selectedLayerIndex)
        {
            int nwell = wells.Count();
            var mfgrid = Owner.Grid as MFGrid;
            int step = DataCube.Size[1];
            DataCube<float> mat = new DataCube<float>(1, step, wells.Count(),false)
            {
                Name = "HOB_Output"
            };
            mat.DateTimes = new DateTime[step];

            for (int n = 0; n < nwell; n++)
            {
                var site = wells.ElementAt(n);
                var well = site as HeadObservation;
                var index = mfgrid.Topology.CellID2CellIndex[well.CellID];
                site.SpatialIndex = index;
                for (int t = 0; t < step; t++)
                {
                    mat[0,t,n] = DataCube[selectedLayerIndex, t, index]; ;
                }
            }
            for (int i = 0; i < step; i++)
                mat.DateTimes[i] = TimeService.IOTimeline[i];
            mat.Variables = new string[] { Variables[selectedLayerIndex] };
            return mat;
        }

        private void fhd_Loading(object sender, int e)
        {
            OnLoading(e);
        }

        private void fhd_DataCubeLoaded(object sender, DataCube<float> e)
        {        
            OnLoaded(_ProgressHandler);
        }
        private void fhd_LoadFailed(object sender, string e)
        {
            OnLoadFailed(e, _ProgressHandler);
        }
    }
}