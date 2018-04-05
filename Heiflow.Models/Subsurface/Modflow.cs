// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface.Packages;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface
{
    [Export(typeof(IBasicModel))]
    [ModelItem]
    public class Modflow : BaseModel
    {
        private LayerGroupManager _LayerGroupManager;
        protected MFGrid _MFGrid;
        /// <summary>
        /// .\Input\MODFLOW\
        /// </summary>
        public const string InputDic = ".\\Input\\MODFLOW\\";
        /// <summary>
        /// .\Output\
        /// </summary>
        public const string OutputDic = ".\\Output\\";
        private MFNameManager _MFNameManager;
        public Modflow()
        {
            Name = "Modflow";
            PackageFileNameProvider = new MFPackFileNameProvider(this);
            this.Icon = Resources.mf16;
            this.TimeService = new TimeService("Subsurface Timeline")
            {
                UseStressPeriods = true
            };
            _MFGrid = new MFGrid();
            Grid = _MFGrid;
            _LayerGroupManager = new LayerGroupManager();
            _MFNameManager = new MFNameManager();
            _IsDirty = false;

            TimeUnit = 4;
            LengthUnit = 2;
        }
        public int TimeUnit { get; set; }
        public int LengthUnit { get; set; }

        public MFNameManager NameManager
        {
            get
            {
                return _MFNameManager;
            }
        }

        public LayerGroupManager LayerGroupManager
        {
            get
            {
                return _LayerGroupManager;
            }
        }

        public override void Initialize()
        {
            TimeServiceList.Add(this.TimeService.Name, this.TimeService);
        }
        public override bool New(IProgress progress)
        {
            bool succ = true;
            MFOutputPackage mfout = new MFOutputPackage()
            {
                Owner = this
            };
            AddInSilence(mfout);

            var list_info = _MFNameManager.GetList(100, Project.Name);
            _MFNameManager.Add(list_info);
            foreach (var pck in ModflowService.SupportedPackages)
            {
                if (pck.IsMandatory && pck is IMFPackage)
                {
                    var pckinfo = pck.PackageInfo;
                    pckinfo.FID = _MFNameManager.NextFID();
                    pckinfo.FileName = string.Format("{0}{1}{2}", InputDic, Project.Name, pckinfo.FileExtension);
                    pckinfo.WorkDirectory = this.WorkDirectory;
                    //must be called here
                    _MFNameManager.Add(pck.PackageInfo);
                    pck.FileName = pckinfo.FileName;
                    pck.Owner = this;
                    pck.Clear();
                    pck.Initialize();
                    pck.New();
                    AddInSilence(pck);
                }
            }
            foreach(var pck in Packages.Values)
            {
                if (pck is IMFPackage)
                {
                    (pck as IMFPackage).CompositeOutput(mfout);
                }
            }
            mfout.Initialize();
            return succ;
        }
        public override bool Load(IProgress progress)
        {
            string masterfile = ControlFileName;
            var succ = true;
            if (File.Exists(masterfile))
            {
                Subscribe(masterfile, ModelService.WorkDirectory);
               succ = LoadGrid(progress);
               if (succ)
               {
                   LoadPackages(progress);
               }
                else
               {
                   progress.Progress("\tFailed to load model grid." );
               }
               return succ;
            }
            else
            {
                progress.Progress( "\tThe modflow name file dose not exist: " + ControlFileName);
                return false;
            }
        }
        public override void Clear()
        {
            foreach(var pck in Packages.Values)
            {
                pck.Clear();
            }
            _MFNameManager.Clear();
            Packages.Clear();
            TimeServiceList.Clear();
        }
        public override bool Validate()
        {
            return true;
        }
        public override void Save(IProgress progress)
        {
            string msg = "";
            if (_MFNameManager.IsDirty)
            {
                _MFNameManager.Save(ControlFileName);
                progress.Progress("\tNam file saved");
            }
            foreach (var pck in Packages.Values)
            {
                if (pck is IMFPackage)
                {
                    if (pck.IsDirty && pck.State == ModelObjectState.Ready)
                    {
                        var succ = pck.Save(progress);
                        if (!succ)
                        {
                            msg = string.Format("\tFailed to save {0}. Error message: {1}",pck.Name,pck.Message);
                            progress.Progress(msg);
                        }
                    }
                    else
                    {
                        if (!pck.IsDirty)
                        {
                            msg = string.Format("\t{0} unchanged.", pck.Name);
                            progress.Progress(msg);
                        }
                        if (pck.State != ModelObjectState.Ready)
                        {
                            msg = string.Format("\ttSkipped to save {0}, it is not ready", pck.Name);
                            progress.Progress(msg);
                        }
                    }
                }
            }
        }
        public override bool LoadGrid(IProgress progress)
        {
            var dis = (Packages["DIS"] as DISPackage);
            var bas = (Packages["BAS6"] as BASPackage);
            var oc = (Packages[OCPackage.PackageName] as OCPackage);
            var succ = true;
            dis.GetGridInfo(_MFGrid);
        
            if (!bas.Load())
            {
                progress.Progress("\tFailed to load BAS6 package. " + ". Error message: " + bas.Message);
                succ = false;
            }
            else
            {
                progress.Progress("\tBAS6 loaded");
            }
            if (!dis.Load())
            {
                progress.Progress("\tFailed to load DIS package. " + ". Error message: " + dis.Message);
                succ = false;
            }
            else
            {
                progress.Progress("\tDIS loaded");
            }
            
            if (!oc.Load())
            {
                progress.Progress("\tFailed to load OC package. " + ". Error message: " + oc.Message);
                succ = false;
            }
            else
            {
                progress.Progress("\tOC loaded");
            }
            if (succ)
            {
                _MFGrid.Topology = new RegularGridTopology();
                _MFGrid.Topology.Build();
                bas.STRT.Topology = _MFGrid.Topology;
                dis.Elevation.Topology = _MFGrid.Topology;
            }
            return succ;
        }
        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            foreach (var pck in Packages.Values)
            {
                if (pck.State == ModelObjectState.Ready)
                {
                    pck.Attach(map, directory);
                    foreach (var ch in pck.Children)
                    {
                        ch.Attach(map, directory);
                    }
                }
            }
            Grid.Projection = Grid.FeatureLayer.Projection;
        }
        public override bool Exsit(string filename)
        {
            return true;
        }
        public override void Add(IPackage pck)
        {
            if (!this.Packages.Keys.Contains(pck.Name))
            {
                var pckinfo = pck.PackageInfo;
                pckinfo.FID = this.NameManager.NextFID();
                pckinfo.FileName = string.Format("{0}{1}{2}", InputDic, Project.Name, pckinfo.FileExtension);
                pckinfo.WorkDirectory = this.WorkDirectory;
                pck.FileName = pckinfo.FileName;
                pck.Owner = this;
                pck.Clear();
                pck.Initialize();
                pck.New();
                Packages.Add(pck.Name, pck);
                NameManager.Add(pckinfo);
                MFOutputPackage mfout = this.Select(MFOutputPackage.PackageName) as MFOutputPackage;
                (pck as IMFPackage).CompositeOutput(mfout);
                if (this.Owner != null)
                    Owner.OnPackageAdded(pck);
                else
                    this.OnPackageAdded(pck);
            }
        }

        public override void Remove(IPackage pck)
        {
            if (this.Packages.Keys.Contains(pck.Name))
            {
                pck.Remove();
            }
            if (this.Owner != null)
                Owner.OnPackageRemoved(pck);
            else
                this.OnPackageRemoved(pck);
        }
        /// <summary>
        /// select package from supported package list
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IPackage Select(string name)
        {
            IPackage pck = null;
            var pck_tar = from pp in ModflowService.SupportedPackages where pp.Name == name select pp;
            if (pck_tar != null && pck_tar.Count() == 1)
            {
                pck = pck_tar.First();
                pck.Owner = this;
            }
            return pck;
        }

        private void Subscribe(string masterfile, string masterDic)
        {
            string line = "";
            StreamReader sr = new StreamReader(masterfile);
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (!line.StartsWith("#"))
                {
                    var strs = TypeConverterEx.Split<string>(line);
                    if (strs != null && strs.Length >= 3)
                    {
                        var pckinfo = new PackageInfo();
                        pckinfo.ModuleName = strs[0].ToUpper();
                        pckinfo.FID = int.Parse(strs[1]);
                        pckinfo.Format = FileFormat.Text;
                        pckinfo.FileName = strs[2].ToLower();
                        pckinfo.FileExtension = Path.GetExtension(pckinfo.FileName);
                        pckinfo.Name = Path.GetFileName(pckinfo.FileName);
                        pckinfo.WorkDirectory = masterDic;
                        if (strs.Length == 4)
                        {
                            pckinfo.IOState = EnumHelper.FromString<IOState>(strs[3].ToUpper());
                        }
                        else
                        {
                            pckinfo.IOState = IOState.OLD;
                        }

                        var module_name = pckinfo.ModuleName.ToUpper();
                        if (module_name.Contains("DATA"))
                        {
                            pckinfo.Format = FileFormat.Text;
                            if (module_name.Contains("BINARY"))
                                pckinfo.Format = FileFormat.Binary;
                            pckinfo.ModuleName = "DATA";
                        }
                        _MFNameManager.AddInSilence(pckinfo);
                        var pck = Select(pckinfo.ModuleName);
                        if (pck != null)
                        {
                            pck.PackageInfo = pckinfo;
                            pck.FileName = pckinfo.FileName;
                            pck.Clear();
                            pck.Initialize();
                            if (!Packages.Keys.Contains(pck.Name))
                            {
                                Packages.Add(pck.Name, pck);
                                pck.Owner = this;
                            }
                        }
                    }
                }
            }
            sr.Close();
        }
        private void LoadPackages(IProgress progress)
        {
            MFOutputPackage mfout = new MFOutputPackage()
            {
                Owner = this
            };
            AddInSilence(mfout);

            foreach (var pck in Packages.Values)
            {
                if (pck is IMFPackage)
                {
                    try
                    {
                        if (pck.State != ModelObjectState.Ready)
                        {
                            pck.Clear();
                            pck.Initialize();
                            var loaded = pck.Load();
                            if (loaded)
                            {
                                (pck as IMFPackage).CompositeOutput(mfout);
                                pck.AfterLoad();
                                progress.Progress("\t" + pck.Name + " loaded");
                            }
                            else
                            {
                                var msg = string.Format("\tFailed to load {0}. Error message: {1}", pck.Name, pck.Message);
                                progress.Progress(msg);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = string.Format("\tFailed to load {0}. Error message: {1}", pck.Name, ex.Message);
                        progress.Progress(msg);
                    }
                }
            }
            var oc = (Packages[OCPackage.PackageName] as OCPackage);
            oc.CompositeOutput(mfout);
            mfout.Initialize();
        }
      
        public override void OnTimeServiceUpdated(ITimeService time)
        {
        }
        public override void OnGridUpdated(IGrid sender)
        {

        }

        public void Extract(string mfn_file, string fs_file, int lurow, int lucol, int rlrow, int rlcol)
        {
            IFeatureSet fs = FeatureSet.Open(fs_file);
            Modflow mf = new Modflow();
            mf.TimeUnit = this.TimeUnit;
            mf.LengthUnit = this.LengthUnit;

            MFGrid rawgrid = this.Grid as MFGrid;
            mf.Grid = rawgrid.Extract(fs, lurow, lucol, rlrow, rlcol);
            FileInfo finfo = new FileInfo(mfn_file);
            string mfn = Path.GetFileNameWithoutExtension(mfn_file);

            foreach (var pckname in this.Packages.Keys)
            {
                var pck = this.Packages[pckname] as IExtractMFPackage;
                if (pck != null)
                {
                    var newpck = pck.Extract(mf);
                    if (newpck != null)
                    {
                        newpck.FileName = Path.Combine(finfo.DirectoryName, mfn + newpck.PackageInfo.FileExtension);
                        mf.AddInSilence(newpck);
                    }
                }
            }

            foreach (var pck in mf.Packages.Values)
            {
                pck.Save(null);
            }

            _MFNameManager.Save(ControlFileName);
        }

    }
}
