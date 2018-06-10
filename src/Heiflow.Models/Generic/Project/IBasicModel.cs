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
        string Name { get; }
        string Description { get;}
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
        bool New(ICancelProgressHandler progress);
        bool Load(ICancelProgressHandler progress);
        void Attach(IMap map, string directory);
        void Save(ICancelProgressHandler progress);
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