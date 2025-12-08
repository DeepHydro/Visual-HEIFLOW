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
using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Models.UI;
using Heiflow.Models.WRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Heiflow.Models.Integration
{
    [Export(typeof(IBasicModel))]
    [Serializable]
    [ModelItem]
    public class HeiflowModel : IntegratedModel
    {
        private PRMS _PRMS;
        private Modflow _Modflow;
        private WaterManagementModel _WaterManagementModel;
        private MasterPackage _MasterPackage;
        private ExtensionManPackage _ExtensionManPackage;

        public HeiflowModel()
        {
            Name = "HEIFLOW";
            Description = "HEIFLOW model version 1.0.0";
            Version = "1.0.0";
            if (!ModelService.SafeMode)
            {
                this.Icon = Resources.RasterImageAnalysisPanSharpen16;
                this.LargeIcon = Resources.RasterImageAnalysisPanSharpen32;
            }
            _IsDirty = false;

            _MasterPackage = new MasterPackage(MasterPackage.PackageName);
            _MasterPackage.Owner = this;
            _ExtensionManPackage = new ExtensionManPackage();
            _ExtensionManPackage.Owner = this;

            _PRMS = new PRMS();
            _PRMS.Owner = this;
            _PRMS.LoadFailed += this.OnLoadFailed;
            Children.Add(_PRMS.Name, _PRMS);

            _Modflow = new Modflow();
            _Modflow.Owner = this;
            _Modflow.LoadFailed += this.OnLoadFailed;
            Children.Add(_Modflow.Name, _Modflow);

            _WaterManagementModel = new WaterManagementModel();
            _WaterManagementModel.Owner = this;
            _WaterManagementModel.LoadFailed += this.OnLoadFailed;
            Children.Add(_WaterManagementModel.Name, _WaterManagementModel);
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
        public WaterManagementModel WaterManagementModel
        {
            get
            {
                return _WaterManagementModel;
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
        [XmlIgnore]
        [Browsable(false)]
        public ExtensionManPackage ExtensionManPackage
        {
            get
            {
                return _ExtensionManPackage;
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
                if (WaterManagementModel != null)
                    WaterManagementModel.Project = _Project;
                _Project.Model = this;
            }
        }

        public override void Initialize()
        {
            TimeServiceList.Clear();
            //_MasterPackage.LoadFailed += this.OnLoadFailed;
            //_ExtensionManPackage.LoadFailed += this.OnLoadFailed;

            this.TimeService = _MasterPackage.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;

            _PRMS.TimeService = _MasterPackage.TimeService;
            _MasterPackage.TimeService.Updated += _PRMS.OnTimeServiceUpdated;
            _PRMS.WorkDirectory = this.WorkDirectory;

            _Modflow.WorkDirectory = this.WorkDirectory;

            _WaterManagementModel.TimeService = _MasterPackage.TimeService;
            _WaterManagementModel.WorkDirectory = this.WorkDirectory;

            this.Grid = _Modflow.Grid;
            _PRMS.Grid = _Modflow.Grid;
            _WaterManagementModel.Grid = _Modflow.Grid;
            this.Grid.Updated += _PRMS.OnGridUpdated;
            this.Grid.Updated += _Modflow.OnGridUpdated;
            this.Grid.Updated += _WaterManagementModel.OnGridUpdated;

            this.TimeServiceList.Add(_MasterPackage.TimeService.Name, _MasterPackage.TimeService);
            this.TimeServiceList.Add(_Modflow.TimeService.Name, _Modflow.TimeService);
        }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            try
            {
                _MasterPackage.FileName = ControlFileName;
                _MasterPackage.Initialize();
                if (_MasterPackage.Load(progress) == LoadingState.Normal)
                {
                    ModelService.Model = this;
                    ModelService.Start = _MasterPackage.TimeService.Start;
                    ModelService.End = _MasterPackage.TimeService.End;

                    _ExtensionManPackage.FileName = _MasterPackage.ExtensionManagerFile;
                    _ExtensionManPackage.Initialize();
                    _ExtensionManPackage.Load(progress);

                    _PRMS.MasterPackage = _MasterPackage;
                    _PRMS.ControlFileName = _MasterPackage.ParameterFilePath;
                    _PRMS.Initialize();

                    _Modflow.TimeService.Start = _MasterPackage.TimeService.Start;
                    _Modflow.TimeService.End = _MasterPackage.TimeService.End;
                    _Modflow.TimeService.Timeline = _MasterPackage.TimeService.Timeline;
                    _Modflow.ControlFileName = _MasterPackage.ModflowFilePath;
                    _Modflow.Initialize();

                    _WaterManagementModel.MasterPackage = _MasterPackage;
                    _WaterManagementModel.Initialize();

                    progress.Progress("Heiflow", 1, "Loading Modflow packages...");
                    if (_Modflow.Load(progress) != LoadingState.FatalError)
                    {
                        _Modflow.IOLogFile = _ExtensionManPackage.MF_IOLOG_File;
                        progress.Progress("Heiflow", 1, "Loading PRMS packages...");
                        if (_PRMS.Load(progress) != LoadingState.FatalError)
                        {
                            progress.Progress("Heiflow", 1, "Loading WMM packages...");
                            if (_WaterManagementModel.Load(progress) != LoadingState.FatalError)
                            {
                                Packages.Clear();
                                AddInSilence(_MasterPackage);
                                AddInSilence(_ExtensionManPackage);
                                return LoadingState.Normal;
                            }
                            else
                            {
                                return LoadingState.FatalError;
                            }
                        }
                        else
                        {
                            return LoadingState.FatalError;
                        }
                    }
                    else
                    {
                        return LoadingState.FatalError;
                    }
                }
                else
                {
                    return LoadingState.FatalError;
                }
            }
            catch(Exception ex)
            {
                OnLoadFailed(this, ex.Message);
                return LoadingState.FatalError;
            }
        }

        public override bool Validate()
        {
            return base.Validate();
        }

        public override bool New(ICancelProgressHandler progress)
        {
            bool succ = true;
            _MasterPackage.FileName = ControlFileName;
            _MasterPackage.Initialize();       
             _MasterPackage.New();
            if (!succ)
            {
                var msg = string.Format("Failed to create Control file. Error message: {0}", _MasterPackage.Message);
                progress.Progress(this.Name, 1, msg);
            }
            AddInSilence(_MasterPackage);

            _ExtensionManPackage.FileName = _MasterPackage.ExtensionManagerFile;
            _ExtensionManPackage.New();
            if (!succ)
            {
                var msg = string.Format("Failed to create extension manager file. Error message: {0}", _ExtensionManPackage.Message);
                progress.Progress(this.Name, 5, msg);
            }
            AddInSilence(_ExtensionManPackage);

            _PRMS.WorkDirectory = this.WorkDirectory;
            _PRMS.ControlFileName = _MasterPackage.ParameterFilePath;
            _PRMS.TimeService = _MasterPackage.TimeService;
            _PRMS.MasterPackage = _MasterPackage;
            _PRMS.Initialize();

            _Modflow.WorkDirectory = this.WorkDirectory;
            _Modflow.ControlFileName = string.Format("{0}{1}.nam", Modflow.InputDic, Project.Name);
            _Modflow.Initialize();

            _WaterManagementModel.WorkDirectory = this.WorkDirectory;
            _WaterManagementModel.TimeService = _MasterPackage.TimeService;
            _WaterManagementModel.MasterPackage = _MasterPackage;
            _WaterManagementModel.Initialize();

            _PRMS.Grid = _Modflow.Grid;
            _WaterManagementModel.Grid = _Modflow.Grid;
            this.Grid = _Modflow.Grid;

            succ = _PRMS.New(progress);
            if (!succ)
            {
                var msg = string.Format("Failed to create PRMS.");
                if (progress != null)
                    progress.Progress(this.Name, 1, msg);
            }
            succ = _Modflow.New(progress);
            _Modflow.IOLogFile = _ExtensionManPackage.MF_IOLOG_File;
            if (!succ)
            {
                var msg = string.Format("Failed to create MODFLOW.");
                if (progress != null)
                    progress.Progress(this.Name, 1, msg);
            }
            succ = _WaterManagementModel.New(progress);
            if (!succ)
            {
                var msg = string.Format("Failed to create WMM.");
                if (progress != null)
                    progress.Progress(this.Name, 1, msg);
            }
 
            NewDatabase();
            _IsDirty = true;
            return succ;
        }

        public override void Save(ICancelProgressHandler progress)
        {
            _MasterPackage.Save(progress);
            _ExtensionManPackage.IsDirty = true;
            _ExtensionManPackage.Save(progress);
            progress.Progress(this.Name, 1, "Saving PRMS input files...");
            _PRMS.Save(progress);
            progress.Progress(this.Name, 1, "Saving MODFLOW input files...");
            _Modflow.Save(progress);
            progress.Progress(this.Name, 1, "Saving WMM input files...");
            _WaterManagementModel.Save(progress);
        }

        public override bool Exsit(string filename)
        {
            bool existed = false;
            var master = new MasterPackage
            {
                FileName = filename
            };
            if (master.Load(null) != LoadingState.FatalError)
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
            list.AddRange(WaterManagementModel.Packages.Values); 
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
            _WaterManagementModel.Clear();
        }

        private void NewDatabase()
        {
            string odmfile = Path.Combine(Application.StartupPath, "Data\\ODM.accdb");
            string odm_templ_file = Path.Combine(Application.StartupPath, "Data\\template.xlsx");

            string local_odmfile = Path.Combine(Project.DatabaseDirectory, "ODM.accdb");
            string local_templfile = Path.Combine(Project.DatabaseDirectory, "template.xlsx");
            File.Copy(odmfile, local_odmfile, true);
            File.Copy(odm_templ_file, local_templfile, true);

            Project.ODMSource = new Core.Data.ODM.ODMSource()
            {
                DatabaseFilePath = "Database\\ODM.accdb",
                Name = Project.Name
            };
        }

        //private void MF2PRMS()
        //{
        //    var sfr = _Modflow.Select(SFRPackage.PackageName);
        //    if (sfr != null)
        //    {
        //        var sfr_pck = sfr as SFRPackage;
        //        sfr_pck.RiverNetwork.PropertyChanged += RiverNetwork_PropertyChanged;
        //    }
        //}

        //private void RiverNetwork_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    var net = sender as RiverNetwork;
        //     if(e.PropertyName == "RiverCount")
        //     {
        //         var nseg_para =  _PRMS.MMSPackage.Select("nsegment");
        //         if(nseg_para != null)
        //         {
        //             nseg_para.SetValue(0, 0, 0, net.RiverCount);
        //         }
        //     }
        //     else if (e.PropertyName == "ReachCount")
        //     {
        //         var nseg_para = _PRMS.MMSPackage.Select("nreach");
        //         if (nseg_para != null)
        //         {
        //             nseg_para.SetValue(0, 0, 0, net.ReachCount);
        //         }
        //     }
        //}

    }
}