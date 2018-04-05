// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Generic
{
    public interface IBasicModel
    {
        event EventHandler<IPackage> PackageAdded;
        event EventHandler<IPackage> PackageRemoved;
        event EventHandler<IPackage> PackageStatechanged;

        Dictionary<string, IPackage> Packages { get; }
        string Name { get; set; }
        string Description { get; set; }
        //string WorkDirectory { get; set; }
        IGrid Grid { get; set; }
        Image Icon { get; set; }
        Image LargeIcon { get; set; }
        IPackageFileNameProvider PackageFileNameProvider { get; set; }
        IProject Project { get; set; }
        IBasicModel Owner { get; }
        Dictionary<string, IBasicModel> Children { get; }
        ITimeService TimeService { get; set; }
        Dictionary<string, ITimeService> TimeServiceList { get; }
        string WorkDirectory { get; set; }
        bool IsDirty { get; set; }
        /// <summary>
        /// do something after necessary environment variables are set
        /// </summary>
        void Initialize();
        bool Exsit(string filename);
        bool New(IProgress progress);
        bool Load(IProgress progress);
        void Attach(IMap map, string directory);
        void Save(IProgress progress);
        void Clear();
        void Add(IPackage pck);
        void Remove(IPackage pck);
        List<IPackage> GetPackages();
        IPackage GetPackage(string pck_name);
        void OnTimeServiceUpdated(ITimeService time);
        void OnGridUpdated(IGrid sender);
        void OnPackageAdded(IPackage pck);
        void OnPackageRemoved(IPackage pck);
         void OnPackageStateChanged(IPackage pck);
    }
}