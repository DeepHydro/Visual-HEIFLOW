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
        }
        public bool LoadDepth
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            base.Initialize();
        }
        public override bool Load()
        {
            if (File.Exists(FileName))
            {
                try
                {
                    FHDFile fhd = new FHDFile(FileName, Owner.Grid as IRegularGrid);
                    fhd.SetDataSource(Values);
                    fhd.Variables = this.Variables;
                    fhd.StepsToLoad = this.StepsToLoad;
                    fhd.IsLoadDepth = LoadDepth;
                    fhd.Loading += fhd_Loading;
                    fhd.Loaded += fhd_Loaded;
                    //TODO: require modifcation
                    fhd.Load();
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
            if (this.LoadDepth)
            {
                vv[0] = "Depth to Groudwater";
            }
            else
                vv[0] = "Groudwater Table";
            for (int ll = 0; ll < grid.ActualLayerCount; ll++)
            {
                if (this.LoadDepth)
                    vv[ll + 1] = string.Format("Layer {0} Depth", ll + 1);
                else
                    vv[ll + 1] = string.Format("Layer {0} Head", ll + 1);
            }
            FHDFile fhd = new FHDFile(FileName, Owner.Grid as IRegularGrid);
            fhd.Scan();
            this.NumTimeStep = fhd.NumTimeStep;
            Variables = vv;

            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;
            Start = TimeService.Start;
            End = EndOfLoading;
            return true;
        }

        public override bool Load(int var_index)
        {

            if (File.Exists(FileName))
            {
                try
                {
                    var grid = Owner.Grid as MFGrid;
                    if (Values == null)
                    {
                        Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount)
                        {
                            Name = "FHD",
                            TimeBrowsable = true,
                            AllowTableEdit = false
                        };
                    }
                    else
                    {
                        if (Values.Size[1] != StepsToLoad)
                            Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount)
                            {
                                Name = "FHD",
                                TimeBrowsable = true,
                                AllowTableEdit = false
                            };
                    }

                    FHDFile fhd = new FHDFile(FileName, grid);
                    fhd.SetDataSource(Values);
                    fhd.Variables = this.Variables;
                    fhd.StepsToLoad = this.StepsToLoad;
                    fhd.IsLoadDepth = LoadDepth;
                    fhd.Loading += fhd_Loading;
                    fhd.Loaded += fhd_Loaded;
                    fhd.Load(var_index);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    OnLoadFailed(ex.Message);
                    return false;
                }
            }
            else
            {

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
                        av += Values[0, count, index];
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
                well.TimeSeries = new DoubleTimeSeries(values, dates);
                well.TimeSeries.Name = well.Name;
            }
        }

        public MyLazy3DMat<float> ExtractTo(IEnumerable<IObservationsSite> wells, int selectedLayerIndex)
        {
            int nwell = wells.Count();
            var mfgrid = Owner.Grid as MFGrid;
            int step = Values.Size[1];
            MyLazy3DMat<float> mat = new MyLazy3DMat<float>(1, step, wells.Count())
            {
                Name = "HOB_Output",
                TimeBrowsable = true,
                AllowTableEdit = false
            };
            mat.Allocate(0);
            mat.DateTimes = new DateTime[step];

            for (int n = 0; n < nwell; n++)
            {
                var site = wells.ElementAt(n);
                var well = site as HeadObservation;
                var index = mfgrid.Topology.CellID2CellIndex[well.CellID];
                site.SpatialIndex = index;
                for (int t = 0; t < step; t++)
                {
                    mat.Value[0][t][n] = Values[selectedLayerIndex, t, index]; ;
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
        private void fhd_Loaded(object sender, MyLazy3DMat<float> e)
        {
            Values = e;
            Values.Topology = (this.Grid as RegularGrid).Topology;
            Values.TimeBrowsable = true;
            Values.DateTimes = new DateTime[Values.Size[1]];
            for (int i = 0; i < Values.Size[1]; i++)
            {
                Values.DateTimes[i] = TimeService.IOTimeline[i];
            }

            Values.Variables = this.Variables;
            OnLoaded(e);
        }
    }
}