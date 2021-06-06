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

using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Core.Hydrology;
using Heiflow.Core.MyMath;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;

namespace Heiflow.Tools.ConceptualModel
{
    public class CreateClimateMappingTable : MapLayerRequiredTool
    {
        public CreateClimateMappingTable()
        {
            Name = "Create Climate Mapping Table";
            Category = "Driving Forces";
            Description = "Create Climate Mapping Table";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true; 
        }
        private IMapLayerDescriptor _CimateZoneLayer;

        [Category("Input")]
        [Description("Model grid centroid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor CentroidFeatureLayer
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("Climate zone layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor CimateZoneLayer
        {
            get
            {
                return _CimateZoneLayer;
            }
            set
            {
                _CimateZoneLayer = value;
                var sourcefs = _CimateZoneLayer.DataSet as IFeatureSet;
                if (sourcefs != null)
                {
                    var buf = from DataColumn dc in sourcefs.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }

        [Category("Field")]
        [Description("Field name of climate zone ID")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ZoneIDField
        {
            get;
            set;
        }
        public override void Initialize()
        {
            Initialized = true;
            if (CentroidFeatureLayer == null || CimateZoneLayer == null)
            {
                this.Initialized = false;
                return;
            }
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var _sourcefs = this._CimateZoneLayer.DataSet as IFeatureSet;
            var _centroid_layer = this.CentroidFeatureLayer.DataSet as IFeatureSet;
            var master = model.GetPackage(MasterPackage.PackageName) as MasterPackage;
            var mapfile = Path.Combine(ModelService.WorkDirectory, master.GridClimateFile);
            int progress = 0;
            int count = 1;
            int ncell= _centroid_layer.Features.Count;
            int nzone=_sourcefs.Features.Count;
            StreamWriter sw = new StreamWriter(mapfile);
            string line = "# Clmate Zone ID, HRU ID";
            sw.WriteLine(line);
            line = string.Format("{0} {1} # climate_zone_count hru_count", nzone, ncell);
            sw.WriteLine(line);
            try
            {
                for (int i = 0; i < ncell; i++)
                {
                    line = "";
                    var zone_id = 1;
                    var hru_id = 1;
                    for (int j = 0; j < nzone; j++)
                    {
                        //var zone_bund = _sourcefs.Features[j].Geometry.Coordinates;
                        //if (SpatialRelationship.PointInPolygon(zone_bund, _centroid_layer.Features[i].Geometry.Coordinate))
                        if (_centroid_layer.Features[i].Geometry.Within(_sourcefs.Features[j].Geometry))
                        {
                            zone_id = int.Parse(_sourcefs.Features[j].DataRow[ZoneIDField].ToString());
                            hru_id = int.Parse(_centroid_layer.Features[i].DataRow["HRU_ID"].ToString());
                            line = string.Format("{0} {1}", zone_id, hru_id);
                            break;
                        }
                    }
                    if (line == "")
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Warning: no climate zone found for the HRU: " + hru_id);
                    }
                    sw.WriteLine(line);
                    progress = (i + 1) * 100 / ncell;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing HRU: " + hru_id);
                        count++;
                    }

                }
            }
            catch
            {

            }
            finally
            {
                sw.Close();
            }
            return true;
        }
    }
}