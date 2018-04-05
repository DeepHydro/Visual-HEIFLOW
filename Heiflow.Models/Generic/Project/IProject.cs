// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
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
        string FullModelWorkDirectory { get; }
        string RelativeModelWorkDirectory { get; set; }
        string ModelExeFileName { get; set; }
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
        bool New(IProgress progress, bool ImportFromExistingModel);
        void Clear();
        /// <summary>
        /// Bind grid feature
        /// </summary>
        void AttachFeatures();
        void CreateGridFeature();
    }
}
