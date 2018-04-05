// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.IO;
using Heiflow.Models.UI;
using Heiflow.Models.Visualization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Generic
{
    public interface IPackage : INotifyPropertyChanged
    {
        event EventHandler<int> Loading;
        event EventHandler<object> Loaded;
        event EventHandler<string> LoadFailed;
        event EventHandler<int> Saving;
        event EventHandler Saved;
        event EventHandler Removed;
        string Name { get; }
        string FullName { get; }
        string Description { get; }
        string Version { get; set; }
        bool IsDirty { get; set; }
        bool IsUsed { get; set; }
        /// <summary>
        /// Full file name of the package
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// Holds any information when loading or saving
        /// </summary>
        string Message { get; set; }
        bool IsMandatory { get; }
        IBasicModel Owner { get; set; }
        IPackage Parent { get; set; }
        List<IPackage> Children { get; set; }
        Image Icon { get; set; }
        Image LargeIcon { get; set; }
        ModelObjectState State { get; }
        PackageInfo PackageInfo { get; set; }
        PackageCoverage Coverage { get; set; }
        Dictionary<string, IParameter> Parameters { get; }
        IPackageOptionalView OptionalView { get; set; }
        IFeatureSet Feature { get; set; }
        IMapFeatureLayer FeatureLayer { get; set; }
        I3DLayer Layer3D { get; set; }
        string Layer3DToken { get; }
        /// <summary>
        /// The fields required by the package feature
        /// </summary>
        List<PackageFeatureField> Fields { get; }
        ITimeService TimeService { get; set; }
        IGrid Grid { get; set; }
        void Initialize();
        bool New();
        /// <summary>
        /// Load package from an exsiting file
        /// </summary>
        /// <returns></returns>
        bool Load();
        /// <summary>
        /// do something after loaded
        /// </summary>
        void AfterLoad();
        bool Save(IProgress progress);
        bool SaveAs(string filename, IProgress progress);
        void Clear();
        bool Check(out string msg);
        void Remove();
        void UpdateTimeService();
        void AddChild(IPackage pck);
        IPackage SelectChild(string name);
        void Attach(DotSpatial.Controls.IMap map,  string directory);
        List<IParameter> GetParameters();
        void RaiseStateChanged(ModelObjectState state);
        void OnGridUpdated(IGrid sender);
        void OnTimeServiceUpdated(ITimeService time);
        void ChangeState(ModelObjectState state);
        void ResetToDefault();
    }
}
