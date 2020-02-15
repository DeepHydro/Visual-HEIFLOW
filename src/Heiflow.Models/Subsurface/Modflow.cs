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
using Heiflow.Models.Generic;
using Heiflow.Models.Properties;
using System;
using System.ComponentModel;
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
        private string _IOLogFile;
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
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            _MFGrid = new MFGrid();
            Grid = _MFGrid;
            _LayerGroupManager = new LayerGroupManager();
            _MFNameManager = new MFNameManager();
            _IsDirty = false;

            TimeUnit = 4;
            LengthUnit = 2;
            Description = "the U.S. Geological Survey modular finite-difference flow model, which is a computer code that solves the groundwater flow equation.";
        }
        [Category("Units")]
        public int TimeUnit { get; set; }
        [Category("Units")]
        public int LengthUnit { get; set; }
        [Browsable(false)]
        public MFNameManager NameManager
        {
            get
            {
                return _MFNameManager;
            }
        }
        [Browsable(false)]
        public LayerGroupManager LayerGroupManager
        {
            get
            {
                return _LayerGroupManager;
            }
        }
        [Browsable(false)]
        public string IOLogFile
        {
            get
            {
                return _IOLogFile;
            }
            set
            {
                _IOLogFile = value;
            }
        }
        public override void Initialize()
        {
            TimeServiceList.Add(this.TimeService.Name, this.TimeService);
        }
        public override bool New(ICancelProgressHandler progress)
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
            foreach (var pck in Packages.Values)
            {
                if (pck is IMFPackage)
                {
                    (pck as IMFPackage).CompositeOutput(mfout);
                }
            }
            mfout.Initialize();
            return succ;
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            string masterfile = ControlFileName;
            string msg = "";
            if (File.Exists(masterfile))
            {
                if (Subscribe(masterfile, ModelService.WorkDirectory, ref msg))
                {
                    if (LoadGrid(progress))
                    {
                        return LoadPackages(progress);
                    }
                    else
                    {
                        return LoadingState.FatalError;
                    }
                }
                else
                {
                    OnLoadFailed(this, msg);
                    return LoadingState.FatalError;
                }
            }
            else
            {
                msg = "The modflow name file dose not exist: " + ControlFileName;
                OnLoadFailed(this, msg);
                return LoadingState.FatalError;
            }
        }
        public override void Clear()
        {
            foreach (var pck in Packages.Values)
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
        public override void Save(ICancelProgressHandler progress)
        {
            string msg = "";
            //if (_MFNameManager.IsDirty)
            //{
                _MFNameManager.Save(ControlFileName);
                progress.Progress("Modflow", 1, "\tNam file saved");
            //}
            foreach (var pck in Packages.Values)
            {
                if (pck is IMFPackage)
                {
                    if (pck.IsDirty && pck.State == ModelObjectState.Ready)
                    {
                        pck.Save(progress);
                    }
                    else
                    {
                        if (!pck.IsDirty)
                        {
                            msg = string.Format("\t{0} unchanged.", pck.Name);
                            progress.Progress("Modflow", 1, msg);
                        }
                        if (pck.State != ModelObjectState.Ready)
                        {
                            msg = string.Format("\ttSkipped to save {0}, it is not ready", pck.Name);
                            progress.Progress("Modflow", 1, msg);
                        }
                    }
                }
            }
        }
        public override bool LoadGrid(ICancelProgressHandler progress)
        {
            var dis = (Packages["DIS"] as DISPackage);
            var bas = (Packages["BAS6"] as BASPackage);
            var oc = (Packages[OCPackage.PackageName] as OCPackage);
            dis.GetGridInfo(_MFGrid);
            if(bas.Load(progress) != LoadingState.FatalError)
            {
                if (dis.Load(progress) != LoadingState.FatalError)
                {
                    if (oc.Load(progress) != LoadingState.FatalError)
                    {
                        _MFGrid.Topology = new RegularGridTopology();
                        _MFGrid.Topology.Build();
                        bas.STRT.Topology = _MFGrid.Topology;
                        dis.Elevation.Topology = _MFGrid.Topology;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
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
                MFOutputPackage mfout = this.GetPackage(MFOutputPackage.PackageName) as MFOutputPackage;
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

        private bool Subscribe(string masterfile, string masterDic, ref string msg)
        {
            bool result = false;
            string line = "";
            msg = "";
            FileStream fs = new FileStream(masterfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            try
            {
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
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                msg = string.Format("Failed to load name file of MODFLOW. Error message: {0}", ex.Message);
                OnLoadFailed(this, msg);
            }
            finally
            {
                fs.Close();
                sr.Close();
            }
            return result;
        }
        private LoadingState LoadPackages(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
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
                            result = pck.Load(progress);
                        }
                        if (pck.State == ModelObjectState.Ready)
                        {
                            (pck as IMFPackage).CompositeOutput(mfout);
                            pck.AfterLoad();
                            progress.Progress("Modflow", 1, pck.Name + " loaded");
                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = string.Format("Failed to load {0}. Error message: {1}", pck.Name, ex.Message);
                        OnLoadFailed(this, msg);
                        result = LoadingState.Warning;
                    }
                }
            }
            var oc = (Packages[OCPackage.PackageName] as OCPackage);
            oc.CompositeOutput(mfout);
            mfout.Initialize();
            return result;
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