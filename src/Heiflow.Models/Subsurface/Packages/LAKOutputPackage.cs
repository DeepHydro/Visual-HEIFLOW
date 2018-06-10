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

using DotSpatial.Data;
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
            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep; 
            if (File.Exists(OutputFilesInfo[0].FileName))
            {
                Sites.Clear();
                for (int i = 0; i < nsite; i++)
                {
                    if (File.Exists(OutputFilesInfo[i].FileName))
                    {
                        var site = new Site()
                        {
                            ID = i,
                            Name = Path.GetFileNameWithoutExtension(OutputFilesInfo[i].Name)
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
            StartOfLoading = TimeService.Start;
            EndOfLoading = TimeService.End;
            NumTimeStep = TimeService.IOTimeline.Count;
            State = ModelObjectState.Ready;
            _Initialized = true;
         
        }
        public override bool Load(ICancelProgressHandler progresshandler)
        {
            if (Sites.Count == 0)
                Scan();
            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            int progress = 0;
            int nstep = StepsToLoad;

            DataCube = new DataCube<float>(2, nstep, Sites.Count);
            OnLoading(0);

            DataCube.Variables = new string[] { "Stage", "Volume" };
            DataCube.Name = "Lake Output";

            for (int i = 0; i < Sites.Count; i++)
            {
                if (File.Exists(OutputFilesInfo[i].FileName))
                {        
                    string line = "";
                    FileStream fs = new FileStream(OutputFilesInfo[i].FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();

                    for (int t = 0; t < nstep; t++)
                    {
                        line = sr.ReadLine();
                        if (!TypeConverterEx.IsNull(line))
                        {
                            var vv = TypeConverterEx.Split<float>(line);
                            DataCube[0,t,i] = vv[1];
                            DataCube[1,t,i] = vv[2];
                        }
                    }
                    sr.Close();
                    progress = Convert.ToInt32((i + 1) * 100 / Sites.Count);
                    OnLoading(progress);
                }
            }
            DataCube.TimeBrowsable = true;
            DataCube.AllowTableEdit = false;
            DataCube.DateTimes = TimeService.IOTimeline.Take(nstep).ToArray();
            OnLoaded(progresshandler);

            return true;
        }

        public override bool Load(int site_index, ICancelProgressHandler progress)
        {
            return Load(progress);
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