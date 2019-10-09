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
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Project
{
    public interface IProject
    {
        event EventHandler ModelChanged;
        /// <summary>
        /// Relative path of the map file which will be saved in the project file
        /// </summary>
        string RelativeMapFileName { get; set; }
        /// <summary>
        /// Full path  of the map file
        /// </summary>
        string FullMapFileName { get;}
        /// <summary>
        /// Relative path of the control file which will be saved in the project file
        /// </summary>
        string RelativeControlFileName { get; set; }
        ///// <summary>
        ///// Full path  of the control file
        ///// </summary>
        //string ControlFileName { get; set; }
        /// <summary>
        /// Full path of the project file
        /// </summary>
        string FullProjectFileName { get; set; }
        /// <summary>
        /// The project name 
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// the  name to be shown in the project explorer
        /// </summary>
        string NameToShown { get; }
        /// <summary>
        /// Gets the description of the project
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Gets the absolute path to project file.
        /// </summary>
        string AbsolutePathToProjectFile { get; set; }
        /// <summary>
        /// the directory for running a model
        /// </summary>
        /// 
        string [] SupportedVersions { get; }
        string SelectedVersion { get; set; }
        string FullModelWorkDirectory { get; }
        string RelativeModelWorkDirectory { get; set; }
        string ModelExeFileName { get; }
        string Token { get; set; }
        string GeoSpatialDirectory { get; }
        string ProcessingDirectory { get; }
        string GridFeatureFilePath { get; set; }
        string CentroidFeatureFilePath { get; set; }
        bool IsDirty { get; set; }
        Image Icon { get; set; }
        Image LargeIcon { get; set; }
        IBasicModel Model { get; set; }
        IMap Map { get; set; }
        MapPolygonLayer GridLayer { get; }
        MapPointLayer CentroidLayer { get; }
        ObservableCollection<FeatureCoverage> FeatureCoverages { get; set; }
        ObservableCollection<RasterCoverage> RasterLayerCoverages {get; set; } 
        ODMSource ODMSource {get; set; }
        void Initialize();
        bool New(ICancelProgressHandler progress, bool ImportFromExistingModel);
        void Clear();
        /// <summary>
        /// Bind grid feature
        /// </summary>
        void AttachFeatures();
        void CreateGridFeature();
        bool ContainsCoverage(string legendtext);
    }
}
