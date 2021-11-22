﻿//
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
using Heiflow.Models.IO;
using System.ComponentModel.Composition;
using DotSpatial.Data;
using Heiflow.Models.Properties;
using System.Xml.Serialization;

namespace Heiflow.Models.Subsurface
{
    public class CBCPackage : MFDataPackage
    {
        public static string PackageName = "CBC";
        public CBCPackage()
        {
            Name = PackageName;
            _MaxTimeStep = 10;
            Layer = 0;
            NumTimeStep = 0;
            Variables = new string[] { "FLOW RIGHT FACE", "FLOW FRONT FACE", "FLOW LOWER FACE",
                "STREAM LEAKAGE", "UZF RECHARGE", "SURFACE LEAKAGE", "GW ET", "CONSTANT HEAD" };
            _PackageInfo.Format = FileFormat.Binary;
            _PackageInfo.IOState = IOState.REPLACE;
            _PackageInfo.FileExtension = ".cbc";
            _PackageInfo.ModuleName = "DATA";
            _PackageInfo.FID = 9;
            IsMandatory = true;
            _Layer3DToken = "RegularGrid";
            LoadingBehavior =  MFLoadingLayersBehavior.None;
            Category = Resources.ModelOutput;
            Filter = Subsurface.Filter.None;
            FilterThreshold = 0;
        }
        [Category("Layer")]
        [XmlIgnore]
        public MFLoadingLayersBehavior LoadingBehavior
        {
            get;
            set;
        }
        [Category("Filter")]
        [XmlIgnore]
        public Filter Filter
        {
            get;
            set;
        }
        [Category("Filter")]
        [XmlIgnore]
        public float FilterThreshold
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            if (File.Exists(FileName))
            {
                _ProgressHandler = progress;
                var grid = Owner.Grid as MFGrid;
                CBCFile cbc = new CBCFile(FileName, grid);
                //cbc.Source = Values;
                cbc.Variables = this.Variables;
                cbc.Layer = this.Layer;
                cbc.Loading += cbc_Loading;
                cbc.DataCubeLoaded += cbc_DataCubeLoaded;
                cbc.LoadingBehavior = this.LoadingBehavior;
                cbc.Filter = this.Filter;
                cbc.FilterThreshold = this.FilterThreshold;
                cbc.LoadDataCube();
                return  LoadingState.Normal;
            }
            else
            {
                return LoadingState.Warning;
            }
        }

        public override bool Scan()
        {
            if (File.Exists(LocalFileName))
            {
                var grid = Owner.Grid as MFGrid;
                CBCFile cbc = new CBCFile(LocalFileName, grid);
                this.Variables = cbc.GetVariables();

                var list = TimeService.GetIOTimeFromFile((Owner as Modflow).IOLogFile);
                if (list.Count > 0)
                {
                    TimeService.IOTimeline = list;
                    NumTimeStep = list.Count;
                    _StartLoading = TimeService.Start;
                    MaxTimeStep = list.Count;
                }
                else
                {
                    cbc.Scan();
                    NumTimeStep = cbc.NumTimeStep;
                    _StartLoading = TimeService.Start;
                    MaxTimeStep = cbc.MaxTimeStep;
                    
                }
                return true;
            }
            else
            {
                OnScanFailed("The file does not exist: " + LocalFileName);
                return false;
            }
        }

        public override LoadingState Load(int var_index, ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            var result = LoadingState.Normal;
            if (File.Exists(LocalFileName))
            {
                try
                {
                    //var list = TimeService.GetIOTimeFromFile((Owner as Modflow).IOLogFile);
                    //if (list.Count > 0)
                    //{
                    //    TimeService.IOTimeline = list;
                    //    NumTimeStep = list.Count;
                    //}

                    var grid = Owner.Grid as MFGrid;
                    int nstep = StepsToLoad;
                    if (DataCube == null || DataCube.Size[1] != nstep)
                    {
                        DataCube = new DataCube<float>(Variables.Length, nstep, grid.ActiveCellCount, true)
                        {
                            Name = "CBC",
                            Variables = this.Variables
                        };
                    }
                    DataCube.Topology = (this.Grid as RegularGrid).Topology;
                    DataCube.DateTimes = this.TimeService.IOTimeline.Take(StepsToLoad).ToArray();

                    CBCFile cbc = new CBCFile(LocalFileName, grid);
                    cbc.Layer = this.Layer;
                    cbc.LoadingBehavior = this.LoadingBehavior;
                    cbc.Filter = this.Filter;
                    cbc.FilterThreshold = this.FilterThreshold;
                    cbc.Variables = this.Variables;
                    cbc.Scale = (float)this.ScaleFactor;
                    cbc.MaxTimeStep = nstep;
                    cbc.NumTimeStep = this.NumTimeStep;
                    cbc.DataCube = this.DataCube;
                    cbc.Loading += cbc_Loading;
                    cbc.DataCubeLoaded += cbc_DataCubeLoaded;
                    cbc.LoadFailed += cbc_LoadFailed;
                    cbc.LoadDataCube(var_index);
                    result = LoadingState.Normal;
                }
                catch(Exception ex)
                {
                    Message = ex.Message;
                    ShowWarning(ex.Message, progress);
                    result = LoadingState.Warning;
                    cbc_LoadFailed(this, Message);
                }
            }
            else
            {
                Message="The file does not exist: " + LocalFileName;
                ShowWarning(Message, progress);
                result = LoadingState.Warning;
                cbc_LoadFailed(this, Message);
            }

            return result;
        }

        public override void Clear()
        {
            if (_Initialized)
            {
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            }
            State = ModelObjectState.Standby;
            _Initialized = false;
        }
        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        private void cbc_Loading(object sender, int e)
        {
            OnLoading(e);
        }
        private void cbc_LoadFailed(object sender, string e)
        {
            ShowWarning(e, _ProgressHandler);
            OnLoaded(_ProgressHandler, new LoadingObjectState() { Message = e, State = LoadingState.Warning });
        }
        private void cbc_DataCubeLoaded(object sender, DataCube<float> e)
        {
            OnLoaded(_ProgressHandler, new LoadingObjectState() { DataCube = e, State = LoadingState.Normal });
        }
    }
}