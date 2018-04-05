// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        }

        public override bool New(IProgress progress, bool ImportFromExistingModel)
        {
            var succ = true;
            System.IO.Directory.CreateDirectory(GeoSpatialDirectory);
            System.IO.Directory.CreateDirectory(ProcessingDirectory);
            System.IO.Directory.CreateDirectory(InputDirectory);
            System.IO.Directory.CreateDirectory(MFInputDirectory);
            System.IO.Directory.CreateDirectory(PRMSInputDirectory);
            System.IO.Directory.CreateDirectory(OutputDirectory);

            RelativeMapFileName = Name + ".dspx";    
            FullProjectFileName = Path.Combine(AbsolutePathToProjectFile, Name + ".vhfx");

            if (!ImportFromExistingModel)
            {
                RelativeControlFileName = Name + ".nam";
                var model = new Modflow()
                {
                    Name = Name,
                    Project = this,
                    WorkDirectory = FullModelWorkDirectory,
                    ControlFileName = RelativeControlFileName
                };
                model.Initialize();
                succ = model.New(progress);
                this.Model = model;
            }
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

        public override void Clear()
        {
           
        }
    }
}
