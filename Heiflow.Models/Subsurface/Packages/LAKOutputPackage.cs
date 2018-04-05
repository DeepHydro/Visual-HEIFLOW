// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface
{
    public class LAKOutputPackage : MFDataPackage, ISitesProvider
    {
        public static string PackageName = "LAK Output";
        public LAKOutputPackage()
        {
            Name = "LAK Output";
            Sites = new List<Core.IObservationsSite>();
            OutputFilesInfo = new List<PackageInfo>();
        }
 
        [Browsable(false)]
        public IList<Core.IObservationsSite> Sites
        {
            get;
            private set;
        }

        public List<PackageInfo> OutputFilesInfo { get; set; }

        [Browsable(false)]
        [PackageOptionalViewItem("LAK")]
        public override IPackageOptionalView OptionalView
        {
            get;
            set;
        }
        public override bool Scan()
        {
            Variables = new string[] { "Stage", "Volume" };
            int nsite = OutputFilesInfo.Count;
            NumTimeStep = 0;
            if (File.Exists(OutputFilesInfo[0].FileName))
            {
                StreamReader sr = new StreamReader(OutputFilesInfo[0].FileName);
                string line = sr.ReadLine();
                line = sr.ReadLine();
                line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    NumTimeStep++;
                }
                if (TypeConverterEx.IsNull(line))
                    NumTimeStep--;
                sr.Close();

                Sites.Clear();
                for (int i = 0; i < nsite; i++)
                {
                    if (File.Exists(OutputFilesInfo[i].FileName))
                    {
                        var site = new Site()
                        {
                            ID = i,
                            Name = OutputFilesInfo[i].Name,
                        };
                        site.Variables = new Variable[2];
                        site.Variables[0] = new Variable()
                        {
                            ID = 0,
                            Name = "Stage"
                        };
                        site.Variables[1] = new Variable()
                        {
                            ID = 1,
                            Name = "Volume"
                        };
                        Sites.Add(site);
                    }
                }
                StartOfLoading = TimeService.IOTimeline.First();
                EndOfLoading = TimeService.IOTimeline.Last();
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.Owner.TimeServiceList["Base Timeline"];
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
         
        }
        public override bool Load()
        {
            if (Sites.Count == 0)
                Scan();
 
            int progress = 0;
            int nstep = StepsToLoad;

            Values = new MyLazy3DMat<float>(2, nstep, Sites.Count);
            Values.Allocate(0);
            Values.Allocate(1);
            OnLoading(0);

            Values.Variables = new string[] { "Stage", "Volume" };
            Values.Name = "Lake Output";

            for (int i = 0; i < Sites.Count; i++)
            {
                if (File.Exists(OutputFilesInfo[i].FileName))
                {        
                    string line = "";
                    StreamReader sr = new StreamReader(OutputFilesInfo[i].FileName);
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();

                    for (int t = 0; t < nstep; t++)
                    {
                        line = sr.ReadLine();
                        if (!TypeConverterEx.IsNull(line))
                        {
                            var vv = TypeConverterEx.Split<float>(line);
                            Values.Value[0][t][i] = vv[1];
                            Values.Value[1][t][i] = vv[2];
                        }
                    }
                    sr.Close();
                    progress = Convert.ToInt32((i + 1) * 100 / Sites.Count);
                    OnLoading(progress);
                }
            }
            Values.TimeBrowsable = true;
            Values.AllowTableEdit = false;
            Values.DateTimes = TimeService.IOTimeline.ToArray();
            OnLoaded(Values);

            return true;
        }
        
        public override bool Load(int site_index)
        {
            return Load();
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
        public override void Attach(DotSpatial.Controls.IMap map,string directory)
        {
            this.Feature = Parent.Feature;
            this.FeatureLayer = Parent.FeatureLayer;
        }
    }
}