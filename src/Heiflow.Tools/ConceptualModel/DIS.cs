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
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using Heiflow.Tools.SpatialAnalyst;
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
    public class DIS : MapLayerRequiredTool
    {
        private IFeatureSet _boreholefs;
        private IFeatureSet _grid_layer;
        private string _LayerHeightField;
        private string _LayerField;
        private IMapLayerDescriptor _BoreholeLayer;
        public DIS()
        {
            Name = "Discretization Package";
            Category = "Conceptual Model";
            Description = "Translate point shapefile into FHB package";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            Neighbors = 5;
            Power = 2;
        }


        [Category("Input")]
        [Description("Layer representing borehole. The attribute table of the layer must contain two fields: Layer and DepthToGround")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor BoreholeLayer
        {
            get
            {
                return _BoreholeLayer;
            }
            set
            {
                _BoreholeLayer = value;
                _boreholefs = _BoreholeLayer.DataSet as IFeatureSet;
                if (_boreholefs != null)
                {
                    var buf = from DataColumn dc in _boreholefs.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }

        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Field of the flux rate")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string LayerHeightField
        {
            get
            {
                return _LayerHeightField;
            }
            set
            {
                _LayerHeightField = value;

            }
        }
        [Category("Parameter")]
        [Description("Field of the layer")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string LayerField
        {
            get
            {
                return _LayerField;
            }
            set
            {
                _LayerField = value;
            }
        }

        [Category("Parameter")]
        [Description("The number of neighbors. If Neighbors<=0, all source sites will be used")]
        public int Neighbors { get; set; }

        [Category("Parameter")]
        [Description("The power used to calcuate weights")]
        public int Power { get; set; }

        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }
        public override void Initialize()
        {    
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_boreholefs == null  || _grid_layer == null)
            {
                this.Initialized = false;
                return;
            }

            this.Initialized = !(_grid_layer == null || _grid_layer.FeatureType != FeatureType.Polygon);
            this.Initialized = !(_boreholefs == null || _boreholefs.FeatureType != FeatureType.Point);
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            int progress = 0;
            int count = 1;
            double sumOfDis = 0;
            double sumOfVa = 0;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var pck = mf.GetPackage(FHBPackage.PackageName) as DISPackage;
                int npt = _boreholefs.Features.Count;
                int nlayer = mf.Grid.ActualLayerCount;
                var known_sites = new Site[npt];
                var ncell = _grid_layer.DataTable.Rows.Count;
                var height_dc = new DataCube<float>(pck.Grid.ActualLayerCount, 1, ncell);
                InverseDistanceWeighting idw = new InverseDistanceWeighting();
                for (int i = 0; i < npt; i++)
                {
                    var cor = _boreholefs.Features[i].Geometry.Coordinate;
                    known_sites[i] = new Site()
                    {
                        LocalX = cor.X,
                        LocalY = cor.Y,
                        ID = i,                        
                    };
                }
                if (Neighbors > npt || Neighbors < 0)
                    Neighbors = npt;

                for (int i = 0; i < ncell; i++)
                {
                    for (int j = 0; j < nlayer; j++)
                    {
                        var cor = _grid_layer.Features[i].Geometry.Coordinate;
                        var site_intep = new Site()
                        {
                            LocalX = cor.X,
                            LocalY = cor.Y,
                            ID = i
                        };
                        var neighborSites =  idw.FindNeareastSites(Neighbors, known_sites, site_intep);
                        sumOfDis = 0;
                        sumOfVa = 0;
                        foreach (var nsite in neighborSites)
                        {
                            var vv = ts_data.GetValue(j, nsite.ID);
                            if (vv < MaximumValue)
                            {
                                double temp = 1 / System.Math.Pow(nsite.Distance, Power);
                                sumOfVa += vv * temp;
                                sumOfDis += temp;
                            }
                        }
                        if (sumOfDis != 0)
                        {
                            mat[0, j, i] = (float)(sumOfVa / sumOfDis);
                        }

                    }
                }
               
                pck.IsDirty = true;
                pck.Save(null);
                pck.ChangeState(Models.Generic.ModelObjectState.Ready);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow is used by this tool.");
                return false;
            }

        }

        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as Heiflow.Models.Integration.HeiflowModel;

            if (model != null)
            {
                var fhb = prj.Project.Model.GetPackage(FHBPackage.PackageName) as FHBPackage;
                fhb.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                shell.ProjectExplorer.ClearContent();
                shell.ProjectExplorer.AddProject(prj.Project);
            }
        }
    }
}
