// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public class SFRInputPackage : MFDataPackage, ISitesProvider
    {
        public static string CBCName = "SFRINPUT";

        private SFRPackage _SFRPackage;

        public SFRInputPackage(SFRPackage sfr)
        {
            _SFRPackage = sfr;
            Sites= new List<Core.IObservationsSite>();
        }

        public override string[] Variables
        {
            get
            {
                return _Variables;
            }
            protected set
            {
                _Variables = value;
                OnPropertyChanged("Variables");

            }
        }

        public IList<IObservationsSite> Sites
        {
            get;
            protected set;
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            _Initialized = true;
        }
        public override bool Scan()
        {
            NumTimeStep = 0;
            int nsite = _SFRPackage.InputFilesInfo.Count;
            var vv = new string[nsite];
            for (int i = 0; i < nsite; i++)
            {
                vv[i] = Path.GetFileNameWithoutExtension(_SFRPackage.InputFilesInfo[i].FileName);
            }
            if (_SFRPackage.InputFilesInfo.Count > 0)
            {
                StreamReader sr = new StreamReader(_SFRPackage.InputFilesInfo[0].FileName);
                string line = sr.ReadLine();
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
                    if (File.Exists(_SFRPackage.InputFilesInfo[i].FileName))
                    {
                        var site = new Site()
                        {
                            ID = i,
                            Name = _SFRPackage.InputFilesInfo[i].Name
                        };
                        site.Variables = new Heiflow.Core.Data.ODM.Variable[1];
                        site.Variables[0] = new Heiflow.Core.Data.ODM.Variable()
                        {
                            ID = 0,
                            Name = "Streamflow"
                        };
                        Sites.Add(site);
                    }
                }

                Variables = vv;
                return true;
            }
            else
            {
                return true;
            }
        }
        public override bool Load()
        {
            Scan();
            OnLoading(0);
            var grid = Owner.Grid as MFGrid;
            int nstep = StepsToLoad;
            int progress = 0;
 
            int nsite = _SFRPackage.InputFilesInfo.Count;
         
            for (int i = 0; i < nsite; i++)
            {
                if (File.Exists(_SFRPackage.InputFilesInfo[i].FileName))
                {
                    var dates = new DateTime[nstep];
                    var mat = new My3DMat<double>(1, nstep, nsite);
                    var site = (from ss in Sites where ss.Name == _SFRPackage.InputFilesInfo[i].Name select ss).First();
                    StreamReader sr = new StreamReader(_SFRPackage.InputFilesInfo[i].FileName);
                    string line = sr.ReadLine();
                    for (int t = 0; t < nstep; t++)
                    {
                        line = sr.ReadLine();
                        if (!TypeConverterEx.IsNull(line))
                        {
                            var vv = TypeConverterEx.Split<double>(line);
                            mat.Value[0][t][0] = vv[1];
                            dates[t] = TimeService.Timeline[(int)vv[0] - 1];
                        }
                    }

                    mat.Name = "Streamflow";
                    mat.Variables = new string[] { "Streamflow" };
                    mat.DateTimes = dates;

                    progress = Convert.ToInt32(i * 100 / nstep);
                    OnLoading(progress);
                    sr.Close();
              
                    site.TimeSeries = mat;
                }
            }

            OnLoaded(Sites);
            return true;
        }
        public override bool Load(int site_index)
        {
            Scan();
            OnLoading(0);
            var grid = Owner.Grid as MFGrid;
            int nstep = StepsToLoad;
            int progress = 0;

            if (File.Exists(_SFRPackage.InputFilesInfo[site_index].FileName))
            {
                var site = (from ss in Sites where ss.Name == _SFRPackage.InputFilesInfo[site_index].Name select ss).First();
                StreamReader sr = new StreamReader(_SFRPackage.InputFilesInfo[site_index].FileName);
                string line = sr.ReadLine();

                var mat = new My3DMat<double>(1, nstep, 1);
                var dates = new DateTime[nstep];
                for (int t = 0; t < nstep; t++)
                {
                    line = sr.ReadLine();

                    if (!TypeConverterEx.IsNull(line))
                    {
                        var vv = TypeConverterEx.Split<double>(line);
                        mat[0,t,0] = vv[1];
                        dates[t] = TimeService.Timeline[(int)vv[0] - 1];
                    }
                    if (progress % 10 == 0)
                        progress = Convert.ToInt32(t * 100 / nstep);
                    OnLoading(progress);
                }

                if (progress < 100)
                    OnLoading(100);

                mat.DateTimes = dates;
                mat.Name = "Streamflow";
                mat.Variables = new string[] { "Streamflow" };
                site.TimeSeries = mat;
                sr.Close();
                OnLoaded(site);
            }
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
    }
}
