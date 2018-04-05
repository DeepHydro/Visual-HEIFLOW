// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using Heiflow.Core.Data;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Heiflow.Models.Integration
{
    [Serializable]
    [Export(typeof(IProject))]
    public class HeiflowProject : BaseProject
    {
        public HeiflowProject()
        {
            this.Name = "HEIFLOW Project";
            this.NameToShown = "HEIFLOW";
            this.Icon = Resources.RasterImageAnalysisPanSharpen16;
            this.LargeIcon = Resources.RasterImageAnalysisPanSharpen32;
            Description = "HEIFLOW model version 1.0.0";
            Token = "HEIFLOW";
            ModelExeFileName = Path.Combine(Application.StartupPath, "Models\\heiflow.exe");
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
                RelativeControlFileName = Name + ".control";
                var model = new Heiflow.Models.Integration.HeiflowModel()
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
            SaveBatchRunFile();
            _IsDirty = true;
            return succ;
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
            this.GridFeatureFilePath = ".\\GeoSpatial\\Grid.shp";
            this.CentroidFeatureFilePath = ".\\GeoSpatial\\Centroid.shp";
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
            if(this.Model != null)
                this.Model.Clear();
        }

        private void SaveBatchRunFile()
        {
           var  filename = Path.Combine(AbsolutePathToProjectFile, "run.bat");
           StreamWriter sw = new StreamWriter(filename);
           var line = string.Format("{0} {1} {2}", ModelExeFileName, Name + ".control", Name + ".xml");
           sw.WriteLine(line);
           sw.WriteLine("Pause");
           sw.Close();
        }
    }
}
