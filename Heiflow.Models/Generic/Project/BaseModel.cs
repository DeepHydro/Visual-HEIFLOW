﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic
{
    public abstract class BaseModel : IBasicModel
    {
        private IGrid _Grid;
        protected IProject _Project;
        private string _ControlFileName;
        protected bool _IsDirty;
        public event EventHandler<IPackage> PackageAdded;
        public event EventHandler<IPackage> PackageRemoved;
        public event EventHandler<IPackage> PackageStatechanged;

        public BaseModel()
        {
            Packages = new Dictionary<string, IPackage>();
            TimeServiceList = new Dictionary<string, ITimeService>();
            Children = new Dictionary<string, IBasicModel>();
        }

        public static string ConfigPath { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, IPackage> Packages
        {
            get;
            protected set;
        }

        [Browsable(false)]
        [XmlIgnore]
        public IGrid Grid
        {
            get
            {
                return _Grid;
            }
            set
            {
                _Grid = value;
                _Grid.Owner = this;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public IPackageFileNameProvider PackageFileNameProvider
        {
            get;
            set;
        }

        [XmlElement]
        [Category("General")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement]
        [Category("General")]
        public string ControlFileName
        {
            get
            {
                return Path.Combine(ModelService.WorkDirectory, _ControlFileName);
            }
            set
            {
                _ControlFileName = value;
            }
        }

        [XmlElement]
        [Category("General")]
        public string Description
        {
            get;
            set;
        }
        [XmlIgnore]
        [Category("General")]
        public string WorkDirectory
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public Image Icon
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public Image LargeIcon
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public IProject Project
        {
            get
            {
                return _Project;
            }
            set
            {
                _Project = value;
            }
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
        public ITimeService TimeService
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public Dictionary<string, ITimeService> TimeServiceList
        {
            get;
            protected set;
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
        public Dictionary<string, IBasicModel> Children
        {
            get;
            protected set;
        }
        public abstract void Initialize();
        public abstract bool Load(IProgress progress);
        public abstract bool LoadGrid(IProgress progress);
        public abstract bool Validate();
        public abstract bool New(IProgress progress);
        public abstract void Clear();
        public abstract void Save(IProgress progress);
        public abstract void Attach(IMap map,  string directory);

        public virtual List<IPackage> GetPackages()
        {
            return Packages.Values.ToList();
        }

        public virtual IPackage GetPackage(string pck_name)
        {
            IPackage found_pck = null;
            foreach(var pck in Packages.Values)
            {
                if(pck.Name == pck_name)
                {
                    found_pck = pck;
                    break;
                }
                else
                {
                    foreach(var pp in pck.Children)
                    {
                        if(pp.Name == pck_name)
                        {
                            found_pck = pp;
                            break;
                        }
                    }
                }
            }
            return found_pck;
        }

        public virtual bool Exsit(string filename)
        {
            return false;
        }


        public bool Load(string masterfile, IProgress progress)
        {
            return false;
        }

        public virtual void OnTimeServiceUpdated(ITimeService sender)
        {

        }

        public virtual void OnGridUpdated(IGrid sender)
        {

        }
        /// <summary>
        ///  Add the package and trigger the Added event
        /// </summary>
        /// <param name="pck"></param>
        public virtual void Add(IPackage pck)
        {
            OnPackageAdded( pck);
        }
        /// <summary>
        ///  Remove the package and trigger the Removed event
        /// </summary>
        /// <param name="pck"></param>
        public virtual void Remove(IPackage pck)
        {
            OnPackageRemoved(pck);
        }
        /// <summary>
        /// Add the package without triggering the Added event
        /// </summary>
        /// <param name="pck"></param>
        public virtual void AddInSilence(IPackage pck)
        {
            if (!Packages.Keys.Contains(pck.Name))
            {
                Packages.Add(pck.Name, pck);
                pck.Owner = this;
            }
        }

        public virtual bool CheckDirty()
        {
            var dirty = false;
            foreach (var pck in Packages.Values)
            {
                if (pck.IsDirty)
                {
                    dirty = true;
                    break;
                }
            }
            dirty = dirty || this._IsDirty;
            return dirty;
        }

        public void OnPackageAdded(IPackage pck)
        {
            if (PackageAdded != null)
                PackageAdded(this, pck);
        }
        public void OnPackageRemoved(IPackage pck)
        {
            if (PackageRemoved != null)
                PackageRemoved(this, pck);
        }
        public void OnPackageStateChanged(IPackage pck)
        {
            if (PackageStatechanged != null)
                PackageStatechanged(this, pck);
        }
    }
}
