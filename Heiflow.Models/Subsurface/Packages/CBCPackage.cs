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
using Heiflow.Models.Generic;
using System.IO;
using ILNumerics;
using System.ComponentModel;
using System.Diagnostics;
using Heiflow.Core.Data;
using Heiflow.Models.IO;
using System.ComponentModel.Composition;

namespace Heiflow.Models.Subsurface
{
    public class CBCPackage : MFDataPackage
    {
        public static string CBCName = "CBC";
        public CBCPackage()
        {
            Name = CBCName;
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
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
        }
        public override bool Load()
        {
            if (File.Exists(FileName))
            {
                var grid = Owner.Grid as MFGrid;
                CBCFile cbc = new CBCFile(FileName, grid);
                //cbc.Source = Values;
                cbc.Layer = this.Layer;
                cbc.Loading += cbc_Loading;
                cbc.Loaded += cbc_Loaded;
                cbc.Load();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Scan()
        {
            var grid = Owner.Grid as MFGrid;
            CBCFile cbc = new CBCFile(FileName, grid);
            cbc.StepsToLoad = this.MaxTimeStep;
            cbc.Scan();
            this.NumTimeStep = cbc.NumTimeStep;
            this.Variables = cbc.Variables;

            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;
            Start = TimeService.Start;
            End = EndOfLoading;
            return true;
        }

        public override bool Load(int var_index)
        {
            var grid = Owner.Grid as MFGrid;

            if (Values == null)
            {
                Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount)
                {
                    Name = "CBC",
                    TimeBrowsable = true,
                    AllowTableEdit = false
                };
            }
            else
            {
                if (Values.Size[1] != StepsToLoad)
                    Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount)
                    {
                        Name = "CBC",
                        TimeBrowsable = true,
                        AllowTableEdit = false
                    };
            }

            CBCFile cbc = new CBCFile(FileName, grid);
            cbc.Layer = this.Layer;
            cbc.Scale = (float)this.ScaleFactor;
            cbc.StepsToLoad = this.StepsToLoad;
            cbc.SetDataSource(Values);
            cbc.Loading += cbc_Loading;
            cbc.Loaded += cbc_Loaded;
            cbc.Load(var_index);
            return true;
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

        private void cbc_Loaded(object sender, MyLazy3DMat<float> e)
        {
            Values.Topology = (this.Grid as RegularGrid).Topology;

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