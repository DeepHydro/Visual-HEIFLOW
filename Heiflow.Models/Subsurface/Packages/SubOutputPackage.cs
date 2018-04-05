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
using System.Text.RegularExpressions;
using Heiflow.Models.IO;
using Heiflow.Core.Data.ODM;
using Heiflow.Core;

namespace Heiflow.Models.Subsurface
{
    public class SubOutputPackage : MFDataPackage
    {
        public static string PackageName = "Vertical Displacement";
        public SubOutputPackage()
        {
            Name = PackageName;
            _FullName = "Vertical Displacement";
            _MaxTimeStep = -1;
            Layer = 0;
            NumTimeStep = 0;
            _PackageInfo.Format = FileFormat.Binary;
            _PackageInfo.IOState = IOState.REPLACE;
            _PackageInfo.FileExtension = ".vert_dis";
            _PackageInfo.ModuleName = "DATA";
            IsMandatory = true;
            Version = "VD1";
            _Layer3DToken = "RegularGrid";
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
                FHDFile fhd = new FHDFile(FileName, Owner.Grid as IRegularGrid);
                fhd.SetDataSource(Values);
                fhd.Variables = this.Variables;
                fhd.StepsToLoad = this.StepsToLoad;
                fhd.Loading += fhd_Loading;
                fhd.Loaded += fhd_Loaded;
                fhd.LoadFailed += fhd_LoadFailed;
                //TODO: require modifcation
                fhd.Load();
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
            var vv = new string[grid.ActualLayerCount+1];
            vv[0] = "Total subsidence";
            for (int ll = 1; ll <= grid.ActualLayerCount; ll++)
            {
                    vv[ll] = string.Format("Layer {0} Vert Disp", ll );
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
                var grid = Owner.Grid as MFGrid;
                if (Values == null)
                {
                    Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount)
                    {
                        Name = "vert_dis",
                        TimeBrowsable = true,
                        AllowTableEdit = false
                    };
                }
                else
                {
                    if (Values.Size[1] != StepsToLoad)
                        Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount)
                        {
                            Name = "vert_dis",
                            TimeBrowsable = true,
                            AllowTableEdit = false
                        };
                }

                FHDFile fhd = new FHDFile(FileName, grid);
                fhd.SetDataSource(Values);
                fhd.Variables = this.Variables;
                fhd.StepsToLoad = this.StepsToLoad;
                fhd.Loading += fhd_Loading;
                fhd.Loaded += fhd_Loaded;
                fhd.LoadVertDis(var_index);
                return true;
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
        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
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

        void fhd_LoadFailed(object sender, string e)
        {
            OnLoadFailed(e);
        }
    }
}