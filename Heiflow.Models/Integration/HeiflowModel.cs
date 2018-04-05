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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Serialization;

namespace Heiflow.Models.Integration
{
    [Export(typeof(IBasicModel))]
    [Serializable]
    [ModelItem]
    public class HeiflowModel : IntegratedModel
    {
        public const string MasterPackageName = "MASTER";
        private PRMS _PRMS;
        private Modflow _Modflow;
        private MasterPackage _MasterPackage;

        public HeiflowModel()
        {
            Name = "HEIFLOW";
            Description = "HEIFLOW model version 1.0.0";
            this.Icon = Resources.RasterImageAnalysisPanSharpen16;
            this.LargeIcon = Resources.RasterImageAnalysisPanSharpen32;
            _IsDirty = false;

            _MasterPackage = new MasterPackage(MasterPackageName);
            _MasterPackage.Owner = this;


            _PRMS = new PRMS();
            _PRMS.Owner = this;
            Children.Add(_PRMS.Name, _PRMS);

            _Modflow = new Modflow();
            _Modflow.Owner = this;
            Children.Add(_Modflow.Name, _Modflow);
        }

        [XmlIgnore]
        [Browsable(false)]
        public PRMS PRMSModel
        {
            get
            {
                return _PRMS;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Modflow ModflowModel
        {
            get
            {
                return _Modflow;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public MasterPackage MasterPackage
        {
            get
            {
                return _MasterPackage;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public new IProject Project
        {
            get
            {
                return _Project;
            }
            set
            {
                _Project = value;
                if (ModflowModel != null)
                    ModflowModel.Project = _Project;
                if (PRMSModel != null)
                    PRMSModel.Project = _Project;
                _Project.Model = this;
            }
        }

        public override void Initialize()
        {
            TimeServiceList.Clear();
            _MasterPackage.TimeService.Updated += _MasterPackage.OnTimeServiceUpdated;
            this.TimeService = _MasterPackage.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;

            _PRMS.TimeService = _MasterPackage.TimeService;
            _PRMS.TimeService.Updated += _PRMS.OnTimeServiceUpdated;
            _PRMS.WorkDirectory = this.WorkDirectory;

            _Modflow.TimeService.Updated += _Modflow.OnTimeServiceUpdated;
            _Modflow.WorkDirectory = this.WorkDirectory;

            _PRMS.Grid = _Modflow.Grid;
            this.Grid = _Modflow.Grid;
            _PRMS.Grid.Updated += _PRMS.OnGridUpdated;
            _Modflow.Grid.Updated += _Modflow.OnGridUpdated;

            this.TimeServiceList.Add(_MasterPackage.TimeService.Name, _MasterPackage.TimeService);
            this.TimeServiceList.Add(_Modflow.TimeService.Name, _Modflow.TimeService);
        }

        public override bool Load(IProgress progress)
        {
            //try
            //{           
            bool succ = true;
            _MasterPackage.FileName = ControlFileName;
            _MasterPackage.Initialize();

            succ = _MasterPackage.Load();
            if (succ)
            {
                ModelService.Model = this;
                ModelService.Start = _MasterPackage.TimeService.Start;
                ModelService.End = _MasterPackage.TimeService.End;

                _PRMS.MasterPackage = _MasterPackage;
                _PRMS.ControlFileName = _MasterPackage.ParameterFilePath;
                _PRMS.Initialize();

                _Modflow.TimeService.Start = _MasterPackage.TimeService.Start;
                _Modflow.TimeService.End = _MasterPackage.TimeService.End;
                _Modflow.TimeService.Timeline = _MasterPackage.TimeService.Timeline;
                _Modflow.ControlFileName = _MasterPackage.ModflowFilePath;
                _Modflow.Initialize();

                progress.Progress("Loading Modflow packages...");
                succ = _Modflow.Load(progress);
                if (!succ)
                {
                    var msg = string.Format("Failed to load Modflow.");
                    progress.Progress(msg);
                }
                progress.Progress("Loading PRMS packages...");
                succ = _PRMS.Load(progress);
                if (!succ)
                {
                    var msg = string.Format("Failed to load PRMS.");
                    progress.Progress(msg);
                }

                if (succ)
                {
                    Packages.Clear();
                    AddInSilence(_MasterPackage);
                }
                else
                {
                    var msg = string.Format("Failed to load HEIFLOW.");
                    progress.Progress(msg);
                }
            }
            else
            {
                var msg = string.Format("Failed to load Control file from " + _MasterPackage.FileName);
                progress.Progress(msg);
            }
            return succ;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        public override bool Validate()
        {
            return base.Validate();
        }

        public override bool New(IProgress progress)
        {
            bool succ = true;
            _MasterPackage.FileName = ControlFileName;
            _MasterPackage.Initialize();
            succ = _MasterPackage.New();
            if (!succ)
            {
                var msg = string.Format("Failed to create Control file. Error message: {0}", _MasterPackage.Message);
                progress.Progress(msg);
            }
            AddInSilence(_MasterPackage);

            _PRMS.WorkDirectory = this.WorkDirectory;
            _PRMS.ControlFileName = _MasterPackage.ParameterFilePath;
            _PRMS.TimeService = _MasterPackage.TimeService;
            _PRMS.MasterPackage = _MasterPackage;
            _PRMS.Initialize();

            _Modflow.WorkDirectory = this.WorkDirectory;
            _Modflow.ControlFileName = string.Format("{0}{1}.nam", Modflow.InputDic, Name);
            _Modflow.Initialize();
            _PRMS.Grid = _Modflow.Grid;
            this.Grid = _Modflow.Grid;

            succ = _PRMS.New(progress);
            if (!succ)
            {
                var msg = string.Format("Failed to create PRMS.");
                progress.Progress(msg);
            }
            succ = _Modflow.New(progress);
            if (!succ)
            {
                var msg = string.Format("Failed to create MODFLOW.");
                progress.Progress(msg);
            }
            _IsDirty = true;
            return succ;
        }

        public override void Save(IProgress progress)
        {
            _MasterPackage.Save(progress);
            progress.Progress("Saving PRMS input files...");
            _PRMS.Save(progress);
            progress.Progress("Saving MODFLOW input files...");
            _Modflow.Save(progress);
        }

        public override bool Exsit(string filename)
        {
            bool existed = false;
            var master = new MasterPackage();
            master.FileName = filename;
            if (master.Load())
            {
                var mfn = Path.Combine(ModelService.WorkDirectory, master.ModflowFilePath);
                if (File.Exists(mfn))
                    existed = true;
            }
            return existed;
        }

        public override void Add(IPackage pck)
        {
            if (pck is IMFPackage)
            {
                _Modflow.Add(pck);
            }
        }

        public override List<IPackage> GetPackages()
        {
            List<IPackage> list = new List<IPackage>();
            list.AddRange(PRMSModel.Packages.Values);
            list.AddRange(ModflowModel.Packages.Values);
            return list;
        }

        public override void OnTimeServiceUpdated(ITimeService sender)
        {
            ModelService.Start = sender.Start;
            ModelService.End = sender.End;
        }

        public override void Clear()
        {
            TimeServiceList.Clear();
            _PRMS.Clear();
            _Modflow.Clear();
        }

        private void MF2PRMS()
        {
            var sfr = _Modflow.Select(SFRPackage.PackageName);
            if (sfr != null)
            {
                var sfr_pck = sfr as SFRPackage;
                sfr_pck.RiverNetwork.PropertyChanged += RiverNetwork_PropertyChanged;
            }
        }

        private void RiverNetwork_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var net = sender as RiverNetwork;
             if(e.PropertyName == "RiverCount")
             {
                 var nseg_para =  _PRMS.MMSPackage.Select("nsegment");
                 if(nseg_para != null)
                 {
                     nseg_para.SetValue(net.RiverCount, 0);
                 }
             }
             else if (e.PropertyName == "ReachCount")
             {
                 var nseg_para = _PRMS.MMSPackage.Select("nreach");
                 if (nseg_para != null)
                 {
                     nseg_para.SetValue(net.ReachCount, 0);
                 }
             }
        }

    }
}