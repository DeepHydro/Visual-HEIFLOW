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
        event EventHandler<LoadingObjectState> Loaded;
       // event EventHandler<string> LoadFailed;
        event EventHandler<string> ScanFailed;
        event EventHandler<int> Saving;
        event EventHandler Saved;
        event EventHandler Removed;
        string Name { get; }
        string FullName { get; }
        string Description { get; }
        string Version { get;}
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
        void New();
        /// <summary>
        /// Load package from an exsiting file
        /// </summary>
        /// <returns></returns>
        LoadingState Load(ICancelProgressHandler progess);
        /// <summary>
        /// do something after loaded
        /// </summary>
        void AfterLoad();
        void Save(ICancelProgressHandler progress);
        void SaveAs(string filename, ICancelProgressHandler progress);
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
        //bool IsLoadFailedRegistered(Delegate prospectiveHandler);
        bool ContainChild(string pck_name);
    }
}
