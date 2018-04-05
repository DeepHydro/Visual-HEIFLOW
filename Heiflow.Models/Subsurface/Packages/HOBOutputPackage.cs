﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public class HOBOutputPackage : MFDataPackage, ISitesProvider
    {
        public static string PackageName = "HOB Output";
        private int _nsite = 0;

        public HOBOutputPackage()
        {
            Name = PackageName;
            _FullName = "Head Observation Output";
            Sites = new List<IObservationsSite>();
            ODMVariableID = 51;
            TimeUnits = TimeUnits.Month;
            IsMandatory = false;
            _Layer3DToken = "Well";
        }

        [Browsable(false)]
        public IList<IObservationsSite> Sites
        {
            get;
            set;
        }

        [Browsable(false)]
        [PackageOptionalViewItem("HOB Output")]
        public override IPackageOptionalView OptionalView
        {
            get;
            set;
        }

        [Browsable(false)]
        public MyLazy3DMat<float> ComparingValues
        {
            get;
            set;
        }

        public int SelectedLayerIndex
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
        public override bool Scan()
        {
            if (File.Exists(FileName))
            {
                Variables = new string[] { "Groundwater Table"};
                var buf = from pp in Owner.Packages[MFOutputPackage.PackageName].Children where pp.Name == FHDPackage.PackageName select pp;
                if (buf.Count() == 1)
                {
                    var _fhd = buf.First() as FHDPackage;
                    NumTimeStep = _fhd.NumTimeStep;
                    StartOfLoading = _fhd.StartOfLoading;
                    EndOfLoading = _fhd.EndOfLoading;
                    MaxTimeStep = _fhd.MaxTimeStep;
                    Variables = new string[] { _fhd.Variables[0] };
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Load()
        {
            return ExtractTo();
        }
        public override bool Load(int var_index)
        {
            return ExtractTo();
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
            this.Feature = Parent.Feature;
            this.FeatureLayer = Parent.FeatureLayer;
        }
        public bool ExtractTo()
        {
            if (NumTimeStep == 0)
                Scan();
            if (Feature != null)
            {
                var hob = Parent as HOBPackage;
                if (Sites == null || Sites.Count == 0)
                    Sites = hob.Observations;
                if (Sites == null)
                {
                    goto error;
                }
                var buf = Owner.GetPackage(FHDPackage.PackageName);
                if (buf != null)                
                {
                    var fhd = buf as FHDPackage;
                    foreach (HeadObservation site in Sites)
                    {
                        site.Variables = new Core.Data.ODM.Variable[] { new Core.Data.ODM.Variable() { Name = fhd.Variables[SelectedLayerIndex] } };
                    }
                    if (fhd.Values != null && fhd.Values.Value[SelectedLayerIndex] != null)
                    {
                        int nstep = StepsToLoad;
                        OnLoading(50);
                        Values = fhd.ExtractTo(Sites, SelectedLayerIndex);
                        Values.Topology = hob.Topology;
                        OnLoaded(Values);
                        return true;
                    }
                    else
                    {
                        goto error;
                    }
                }
                else
                {
                    goto error;
                }
            }
            else
            {
                goto error;
            }
            error:
            {
                State = ModelObjectState.Error;
                OnLoaded(null);
                return false;
            }
        }
        public void LoadComparingValues()
        {
            OnLoading(0);
            var grid = Owner.Grid as MFGrid;
            int nstep = _nsite;
            int progress = 0;

            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                ComparingValues = new MyLazy3DMat<float>(2, 1, _nsite);
                ComparingValues.Allocate(0, 1, _nsite);
                ComparingValues.Allocate(1, 1, _nsite);
                string line = sr.ReadLine();
                for (int i = 0; i < _nsite; i++)
                {
                    line = sr.ReadLine();
                    if (!TypeConverterEx.IsNull(line))
                    {
                        var vv = TypeConverterEx.Split<float>(line);
                        ComparingValues.Value[0][0][i] = vv[0];
                        ComparingValues.Value[1][0][i] = vv[1];
                    }
                    progress = Convert.ToInt32(i * 100 / nstep);
                    OnLoading(progress);
                }
                if (progress < 100)
                    OnLoading(100);
                ComparingValues.DateTimes = new DateTime[] { TimeService.Timeline[0] };
                OnLoaded(ComparingValues);
                sr.Close();
            }
        }
        public float[] GetHeads(int var_index, int time_index)
        {
            if (Values != null && Sites != null)
            {
                int nsites = Sites.Count;
                float[] heads = new float[nsites];

                for (int i = 0; i < nsites; i++)
                {
                    heads[i] = Values.GetSeriesAt(var_index, i)[time_index];
                }
                return heads;
            }
            else
            {
                return null;
            }
        }
 
    }
}