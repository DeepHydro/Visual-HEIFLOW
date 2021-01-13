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
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        /// 
        [Browsable(false)]
        string RelativeMapFileName { get; set; }
        /// <summary>
        /// Full path  of the map file
        /// </summary>
        string FullMapFileName { get; }
        /// <summary>
        /// Relative path of the control file which will be saved in the project file
        /// </summary>
        /// 
        [Browsable(false)]
        string RelativeControlFileName { get; set; }
        /// <summary>
        /// Full path of the project file
        /// </summary>
        /// 
        [Browsable(false)]
        string FullProjectFileName { get; set; }
        /// <summary>
        /// The project name 
        /// </summary>
        /// 
        [Browsable(false)]
        string Name { get; set; }
        /// <summary>
        /// the  name to be shown in the project explorer
        /// </summary>
        /// 
        [Browsable(false)]
        string NameToShown { get; }
        /// <summary>
        /// Gets the description of the project
        /// </summary>
        /// 
        [Category("General")]
        string Description { get; set; }
        /// <summary>
        /// Gets the absolute path to project file.
        /// </summary>
        /// 
        [Browsable(false)]
        string AbsolutePathToProjectFile { get; set; }
        /// <summary>
        /// the directory for running a model
        /// </summary>
        /// 
        [Browsable(false)]
        string[] SupportedVersions { get; }
        [Browsable(false)]
        string SelectedVersion { get; set; }
        [Browsable(false)]
        string FullModelWorkDirectory { get; }
        [Browsable(false)]
        string RelativeModelWorkDirectory { get; set; }
        [Browsable(false)]
        string ModelExeFileName { get; }
        [Browsable(false)]
        string Token { get; set; }
        [Browsable(false)]
        string GeoSpatialDirectory { get; }
        [Browsable(false)]
        string ProcessingDirectory { get; }
        [Browsable(false)]
        string DatabaseDirectory { get; }
        [Browsable(false)]
        string GridFeatureFilePath { get; set; }
        [Browsable(false)]
        string CentroidFeatureFilePath { get; set; }
        [Browsable(false)]
        bool IsDirty { get; set; }
        [Browsable(false)]
        Image Icon { get; set; }
        [Browsable(false)]
        Image LargeIcon { get; set; }
        [Browsable(false)]
        IBasicModel Model { get; set; }
        [Browsable(false)]
        IMap Map { get; set; }
        [Browsable(false)]
        MapPolygonLayer GridLayer { get; }
        [Browsable(false)]
        MapPointLayer CentroidLayer { get; }
        [Browsable(false)]
        ObservableCollection<FeatureCoverage> FeatureCoverages { get; set; }
        [Browsable(false)]
        ObservableCollection<RasterCoverage> RasterLayerCoverages { get; set; }
        [Browsable(false)]
        ODMSource ODMSource { get; set; }
        [Browsable(false)]
        IGridFileFactory GridFileFactory { get; set; }
        IDataCubeFileFactory DataCubeFileFactory { get; set; }
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