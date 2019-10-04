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
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.ConceptualModel
{
    public class UZF : MapLayerRequiredTool
    {
        private IFeatureSet _watershedfs;
        private IMapLayerDescriptor _WatershedLayer;

        public UZF()
        {
            Name = "Unsaturated-Zone Flow Package";
            Category = "Conceptual Model";
            Description = "Translate point shapefile into FHB package";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        [Category("Input")]
        [Description("FHB source layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor WatershedLayer
        {
            get
            {
                return _WatershedLayer;
            }
            set
            {
                _WatershedLayer = value;
                _watershedfs = _WatershedLayer.DataSet as IFeatureSet;
            }
        }


        public override void Initialize()
        {    
            if (_watershedfs == null )
            {
                this.Initialized = false;
                return;
            }
            this.Initialized = !(_watershedfs == null || _watershedfs.FeatureType != FeatureType.Polygon);
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf =  model as Modflow;
            if (mf != null)
            {
                var pck = mf.GetPackage(UZFPackage.PackageName) as UZFPackage;
                var mfgrid = mf.Grid as RegularGrid;
                Coordinate centroid = null;
                for (int i = 0; i < mfgrid.ActiveCellCount; i++)
                {
                    centroid = mfgrid.LocateCentroid(mfgrid.Topology.ActiveCellLocation[i][1] + 1, mfgrid.Topology.ActiveCellLocation[i][0] + 1);
                    for (int j = 0; j < _watershedfs.Features.Count; j++)
                    {
                        var cell = _watershedfs.Features[j].Geometry.Coordinates;
                        if (SpatialRelationship.PointInPolygon(cell, centroid))
                        {
                            pck.IRUNBND[0, 0, i] = int.Parse(_watershedfs.Features[j].DataRow["BasinID"].ToString()) + 1;
                            break;
                        }
                    }
                    progress = i * 100 / mfgrid.ActiveCellCount;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing cell: " + i);
                        count++;
                    }
                }
                pck.Save(cancelProgressHandler);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed to run.");
                return false;
            }
        }
    }
}
