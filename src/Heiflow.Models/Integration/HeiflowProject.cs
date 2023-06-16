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
using DotSpatial.Symbology;
using Heiflow.Core.Data;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;

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
            Description = "HEIFLOW model version 1.0.0 or version 1.1.0";
            Token = "HEIFLOW";
            SupportedVersions = new string[] { "v1.0.0", "v1.1.0" };
            SelectedVersion = SupportedVersions[0];
            MODFLOWVersion = Subsurface.MODFLOWVersion.MF2005;
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
            System.IO.Directory.CreateDirectory(PRMSInputDirectory);
            System.IO.Directory.CreateDirectory(ExtensionInputDirectory);
            System.IO.Directory.CreateDirectory(WQDirectory);
            System.IO.Directory.CreateDirectory(OutputDirectory);
            System.IO.Directory.CreateDirectory(DatabaseDirectory);

            RelativeMapFileName = Name + ".dspx";
            FullProjectFileName = Path.Combine(AbsolutePathToProjectFile, Name + ".vhfx");       

            if (!ImportFromExistingModel)
            {
                RelativeControlFileName = Name + ".control";
                var model = new Heiflow.Models.Integration.HeiflowModel()
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
            CheckBatchRunFile();
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
        protected override void SaveBatchRunFile()
        {
           var  filename = Path.Combine(AbsolutePathToProjectFile, "run.bat");
           StreamWriter sw = new StreamWriter(filename);
           var line = string.Format("{0} {1}", ModelExeFileName, Name + ".control");
           sw.WriteLine(line);
           sw.WriteLine("Pause");
           sw.Close();
        }

    }
}
