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
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;

namespace Heiflow.Models.Subsurface
{
    [Serializable]
    [Export(typeof(IProject))]
    public class ModflowProject : BaseProject
    {
        public ModflowProject()
        {
            this.Name = "Modflow2005 Project";
            this.NameToShown = "Modflow2005";
            this.Icon = Resources.mf16;
            this.LargeIcon = Resources.mf32;
            Description = "Modflow model version 2005";
            Token = "Modflow2005";
            SupportedVersions = new string[] { "v2005" };
            SelectedVersion = "v2005";
            MODFLOWVersion = Subsurface.MODFLOWVersion.MF2005;

            _removedHeaderItemKeys = new string[] { "kGlobalSet", "kParaViewer", "kParaMapping", "kRunCascade", "kGlobalSet" };
        }
        [Category("Model")]
        public MODFLOWVersion MODFLOWVersion
        {
            get;
            set;
        }
        public override bool New(ICancelProgressHandler progress, bool ImportFromExistingModel)
        {
            var succ = true;
            System.IO.Directory.CreateDirectory(GeoSpatialDirectory);
            System.IO.Directory.CreateDirectory(ProcessingDirectory);
            System.IO.Directory.CreateDirectory(InputDirectory);
            System.IO.Directory.CreateDirectory(MFInputDirectory);
            System.IO.Directory.CreateDirectory(OutputDirectory);

            RelativeMapFileName = Name + ".dspx";
            FullProjectFileName = Path.Combine(AbsolutePathToProjectFile, Name + ".vhfx");

            if (!ImportFromExistingModel)
            {
                RelativeControlFileName = Name + ".nam";
                var model = new Modflow()
                {
                    Project = this,
                    WorkDirectory = FullModelWorkDirectory,
                    ControlFileName = RelativeControlFileName
                };
                model.Initialize();
                succ = model.New(progress);
                model.Version = this.SelectedVersion;
                this.Model = model;
            }
            SaveBatchRunFile();
            SaveIHMProjectFile();
            _IsDirty = true;
            return true;
        }

        public override void AttachFeatures()
        {
            var gridfea_file = Path.Combine(this.AbsolutePathToProjectFile, GridFeatureFilePath);
            var centroidfea_file = Path.Combine(this.AbsolutePathToProjectFile, CentroidFeatureFilePath);

            _GridLayer = MapHelper.Select(gridfea_file, Map, this.AbsolutePathToProjectFile) as MapPolygonLayer;
            _CentroidLayer = MapHelper.Select(centroidfea_file, Map, this.AbsolutePathToProjectFile) as MapPointLayer;

            if (_GridLayer != null && _CentroidLayer != null)
            {
                Model.Grid.FeatureSet = _GridLayer.DataSet;
                Model.Grid.CentroidFeature = _CentroidLayer.DataSet;
                Model.Grid.FeatureLayer = _GridLayer;
                Model.Grid.CentroidFeatureLayer = _CentroidLayer;
            }
            else
            {
                CreateGridFeature();
            }

            Model.Attach(this.Map, this.GeoSpatialDirectory);
            CheckBatchRunFile();
        }

        public override void CreateGridFeature()
        {
            this.GridFeatureFilePath = "GeoSpatial\\Grid.shp";
            this.CentroidFeatureFilePath = "GeoSpatial\\Centroid.shp";
            var full_gridfea_file = Path.Combine(this.AbsolutePathToProjectFile, GridFeatureFilePath);
            var full_centroid_file = Path.Combine(this.AbsolutePathToProjectFile, CentroidFeatureFilePath);

            this.Model.Grid.Build(full_gridfea_file);
            var fs_grid = FeatureSet.Open(full_gridfea_file);
            _GridLayer = new MapPolygonLayer(fs_grid);

            this.Model.Grid.FeatureSet = fs_grid;
            this.Model.Grid.FeatureLayer = _GridLayer;
            this.Map.Layers.Add(_GridLayer);

            this.Model.Grid.BuildCentroid(full_centroid_file);
            var fs_centroid = FeatureSet.Open(full_centroid_file);
            _CentroidLayer = new MapPointLayer(fs_centroid);
            this.Model.Grid.CentroidFeature = fs_centroid;
            this.Model.Grid.CentroidFeatureLayer = _CentroidLayer;
            this.Map.Layers.Add(_CentroidLayer);

            this.Map.Invalidate();
        }
        protected override void SaveBatchRunFile()
        {
            var filename = Path.Combine(AbsolutePathToProjectFile, "run.bat");
            StreamWriter sw = new StreamWriter(filename);
            var line = string.Format("{0} {1}", ModelExeFileName, Name + ".nam");
            sw.WriteLine(line);
            sw.WriteLine("Pause");
            sw.Close();
        }
    }
}
