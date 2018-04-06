﻿//
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

using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using Heiflow.Core.Data;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic
{
    [Serializable]
    public abstract class Package : IPackage, INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<int> Loading;
        public event EventHandler<object> Loaded;
        public event EventHandler<string> LoadFailed;
        public event EventHandler<int> Saving;
        public event EventHandler Saved;
        public event EventHandler<ModelObjectState> StateChanged;
        public event EventHandler Removed;

        protected string _Name;
        protected string _FullName;
        protected string _Description;
        protected string _FileName;
        protected bool _Loaded;
        protected bool _Initialized = false;
        protected bool _IsDirty;
        protected bool _IsUsed = false;
        protected string _Layer3DToken = "RegularGrid";
        protected Dictionary<string, IParameter> _Parameters;
        protected Parameter[] _DefaultParameters;
        protected List<IPackage> _Childs = new List<IPackage>();
        protected PackageInfo _PackageInfo;
        protected ModelObjectState _State;
        protected IPackage _Parent;
        public Package(string name)
        {
            Name = name;
            Parameters = new Dictionary<string, IParameter>();
            Description = "This is a package";
            IsDirty = false;
            Icon = Resources.MapPackageTiledTPKFile16;
            LargeIcon = Properties.Resources.MapPackageTiledTPKFile16;
            Fields = new List<PackageFeatureField>();
            Version = "1.0.0";
            IsMandatory = false;
            _IsDirty = false;
            _Initialized = false;
            _State = ModelObjectState.Ready;
        }
        public Package()
        {
            Name = "Package";
            Parameters = new Dictionary<string, IParameter>();
            Description = "This is a package";
            IsDirty = false;
            Icon = Resources.MapPackageTiledTPKFile16;
            LargeIcon = Properties.Resources.MapPackageTiledTPKFile16;
            Fields = new List<PackageFeatureField>();
            Version = "1.0.0";
        }

        [XmlElement]
        [Category("General")]
        [Browsable(false)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }
        [XmlElement]
        [Category("General")]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }
           [XmlElement]
        [Category("General")]
        public string Version
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public IBasicModel Owner
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public IGrid Grid
        { 
            get; 
            set; 
        }
        /// <summary>
        /// Full file name. Relative file name should be used when setting this property.
        /// </summary>
        [XmlIgnore]
        [Category("General")]
        [Description("The full file path of the package")]
        public string FileName
        {
            get
            {
                if (TypeConverterEx.IsNull(_FileName))
                    return "";
                else
                {
                    var full = Path.Combine(ModelService.WorkDirectory, _FileName);
                    return full;
                }
            }
            set
            {
                _FileName = value;
                OnPropertyChanged("FileName");
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Dictionary<string, IParameter> Parameters
        {
            get
            {
                return _Parameters;
            }
            private set
            {
                _Parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

        [XmlArrayItem]
        [Browsable(false)]
        public Parameter[] DefaultParameters
        {
            get
            {
                return _DefaultParameters;
            }
            set
            {
                _DefaultParameters = value;
                OnPropertyChanged("DefaultParameters");
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public bool IsDirty
        {
            get
            {
                return CheckDirty();
            }
            set
            {
                _IsDirty = value;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public bool IsUsed
        {
            get
            {
                return _IsUsed;
            }
            set
            {
                _IsUsed = value;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public ModelObjectState State
        {
            get
            {
                return _State;
            }
            protected set
            {
                if (value != _State)
                {
                    _State = value;
                }
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string Message
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public virtual List<IPackage> Children
        {
            get
            {
                return _Childs;
            }
            set
            {
                _Childs = value;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public System.Drawing.Image Icon
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public System.Drawing.Image LargeIcon
        {
            get;
            set;
        }

        [Browsable(false)]
        public PackageInfo PackageInfo
        {
            get
            {
                return _PackageInfo;
            }
            set
            {
                _PackageInfo = value;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public PackageCoverage Coverage
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public virtual IPackageOptionalView OptionalView
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public virtual DotSpatial.Data.IFeatureSet Feature
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public IMapFeatureLayer FeatureLayer
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public List<PackageFeatureField> Fields
        {
            get;
            protected set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public ITimeService TimeService
        {
            get;
            set;
        }
        [XmlIgnore]
        [Category("General")]
        public bool IsMandatory
        {
            get;
            protected set;
        }
        [Browsable(false)]
        [XmlIgnore]
        public IPackage Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                _Parent = value;
                if (!_Parent.Children.Contains(this))
                    _Parent.Children.Add(this);
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Visualization.I3DLayer Layer3D
        {
            get;
            set;
        }
         [Browsable(false)]
        public string Layer3DToken
        {
            get
            {
                return _Layer3DToken;
            }
        }
        /// <summary>
        /// set model environments like grid, time service
        /// </summary>
        public virtual void Initialize()
        {
            Message = "";
            _Initialized = true;
            _IsDirty = false;
        }
        /// <summary>
        /// Load from  an  exsiting file
        /// </summary>
        /// <returns></returns>
        public virtual bool Load()
        {
            return true;
        }
        /// <summary>
        /// do something after loaded
        /// </summary>
        public virtual void AfterLoad()
        {
            IsDirty = false;
        }
        /// <summary>
        /// save to the default file
        /// </summary>
        /// <returns></returns>
        public virtual bool Save(IProgress progress)
        {
            if (IsDirty && State == ModelObjectState.Ready)
            {
                return SaveAs(FileName,progress);
            }
            else
            {
                if (progress != null)
                {
                    string msg = string.Format("\t{0} unchanged", this.Name);
                    progress.Progress(msg);
                }
                return true;
            }
        }

        /// <summary>
        /// save to the specified file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public virtual bool SaveAs(string filename, IProgress progress)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool New()
        {
            State = ModelObjectState.Standby;
            _IsDirty = true;
            _IsUsed = true;
            return true;
        }


        public virtual void Serialize(string filename)
        {

        }

        public virtual void Deserialize(string filename)
        {

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
            IsDirty = true;
        }

        protected void OnLoading(int percent)
        {
            if (Loading != null)
                Loading(this, percent);
        }

        protected void OnLoaded(object msg)
        {
            IsDirty = false;
            _IsUsed = true;
            State = ModelObjectState.Ready;
            if (Loaded != null)
            {
                Loaded(this, msg);
            }
        }

        protected void OnLoadFailed(string msg)
        {
            _IsUsed = false;
            State = ModelObjectState.Error;
            if (LoadFailed != null)
            {
                LoadFailed(this, msg);
            }
        }

        protected void OnSaving(int percent)
        {
            if (Saving != null)
                Saving(this, percent);
        }

        protected void OnSaved(IProgress progress)
        {
            if (Saved != null)
            {
                Saved(this, EventArgs.Empty);
            }
            IsDirty = false;
            string msg = string.Format("{0} saved", this.Name);
            if(progress != null)
                progress.Progress(msg);
        }

        public List<IParameter> GetParameters()
        {
            return Parameters.Values.ToList();
        }

        public virtual void Clear()
        {
            Children.Clear();
            Message = "";
            _Initialized = false;
            _IsUsed = false;
            _IsDirty = false;
            _State = ModelObjectState.Standby;
        }

        public virtual void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            string filename = Path.Combine(directory, this.Name + ".shp");
            this.FeatureLayer = MapHelper.Select(filename, map, ModelService.ProjectDirectory) as IMapFeatureLayer;
            if (this.FeatureLayer != null)
                this.Feature = this.FeatureLayer.DataSet;
            if (this.Feature == null)
            {
                NewFeatureLayer(map, directory);
            }
        }

        public virtual string CreateFeature(ProjectionInfo proj_info, string directory)
        {
            return null;
        }

        private void NewFeatureLayer(DotSpatial.Controls.IMap map, string directory)
        {
            string filename = Path.Combine(directory, this.Name + ".shp");
            if (!File.Exists(filename))
            {
                CreateFeature(map.Projection, directory);
            }

            this.Feature = FeatureSet.Open(filename);
            if (this.Feature.FeatureType == FeatureType.Polygon)
            {
                var layer = new MapPolygonLayer(this.Feature);
                map.Layers.Add(layer);
                this.FeatureLayer = layer;
            }
            else if (this.Feature.FeatureType == FeatureType.Line)
            {
                var layer = new MapLineLayer(this.Feature);
                map.Layers.Add(layer);
                this.FeatureLayer = layer;
            }
            else if (this.Feature.FeatureType == FeatureType.Point)
            {
                var layer = new MapPointLayer(this.Feature);
                map.Layers.Add(layer);
                this.FeatureLayer = layer;
            }
        }
    
        public void RaiseStateChanged(ModelObjectState state)
        {
            if (StateChanged != null)
                StateChanged(this, state);
            if (this.Owner != null)
                this.Owner.OnPackageStateChanged(this);
        }
        public void AddChild(IPackage pck)
        {
            if (!Children.Contains(pck))
            {
                Children.Add(pck);
                pck.Parent = this;
            }
        }
        public virtual void OnGridUpdated(IGrid sender)
        {
            this.Feature = sender.FeatureSet;
            this.FeatureLayer = sender.FeatureLayer;
            _IsDirty = true;
            this.State = ModelObjectState.Ready;
        }
        public virtual void UpdateTimeService()
        {

        }
        public virtual bool Check(out string msg)
        {
            msg = "No error found.";
            return true;
        }
        public virtual bool CheckDirty()
        {
            return _IsDirty;
        }
        public virtual void OnTimeServiceUpdated(ITimeService time)
        {
            _IsDirty = true;
            //this.State = ModelObjectState.Ready;
        }
        /// <summary>
        /// Remove self package by its owner. Must be called in override function
        /// </summary>
        public virtual void Remove()
        {
            if (Owner.Packages.ContainsKey(this.Name))
            {
                Owner.Packages.Remove(this.Name);
                OnRemoved();
            }
        }
        /// <summary>
        /// Trigger removed event
        /// </summary>
        public virtual void OnRemoved()
        {
            if(Removed !=null)
            {
                Removed(this, EventArgs.Empty);
            }
        }
        public void ChangeState(ModelObjectState state)
        {
            _State = state;
            RaiseStateChanged(state);
        }


        public IPackage SelectChild(string name)
        {
            var pck = from ch in Children where ch.Name == name select ch;
            if (pck.Any())
                return pck.First();
            else
                return null;
        }

        public virtual void ResetToDefault()
        {

        }
    }
}