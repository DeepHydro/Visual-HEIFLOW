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
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Subsurface.MT3DMS;
using Heiflow.Models.Subsurface.VFT3D;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.ConceptualModel
{
    public class WellTool: MapLayerRequiredTool
    {
        public WellTool()
        {
            Name = "Create Well Package";
            Category = Cat_CMG;
            SubCategory = "WEL";
            Description = "Create WEL package by using point feature layer";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        private IMapLayerDescriptor _SourceLayer;
        private IFeatureSet _sourcefs;
        private IFeatureSet _grid_layer;
        private string _LayerField;
        private int _SelectedLayerIndex = 0;
        private bool _issuccess = false;


        [Category("Input")]
        [Description("WEL source layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor WELSourceLayer
        {
            get
            {
                return _SourceLayer;
            }
            set
            {
                _SourceLayer = value;
                _sourcefs = _SourceLayer.DataSet as IFeatureSet;
                if (_sourcefs != null)
                {
                    var buf = from DataColumn dc in _sourcefs.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }

        [Category("Input")]
        [Description("Model grid layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("A list that contains field name of the pumping rate in each stress period. An example list is: rate1,rate2")]
        public string PumpingRateFieldList
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("A integer flag list. If flag = 1, all features in the source shapefile will be recgonized as wells; otherwise, no wells are used. An example list is: 1,0")]
        public string WellNumFlagList
        {
            get;
            set;
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
                if (Fields != null)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (_LayerField == Fields[i])
                        {
                            _SelectedLayerIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }

        public override void Initialize()
        {
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_sourcefs == null || _grid_layer == null || TypeConverterEx.IsNull(WellNumFlagList) || TypeConverterEx.IsNull(PumpingRateFieldList))
            {
                this.Initialized = false;
                return;
            }

            this.Initialized = !(_grid_layer == null || _grid_layer.FeatureType != FeatureType.Polygon);
            this.Initialized = !(_sourcefs == null || _sourcefs.FeatureType != FeatureType.Point);

            try
            {
                var rate_fileds_list = TypeConverterEx.Split<string>(PumpingRateFieldList);
                foreach (var field in rate_fileds_list)
                {
                    if (!Fields.Contains(field))
                    {
                        Initialized = false;
                        break;
                    }
                }
                TypeConverterEx.Split<int>(WellNumFlagList);
            }
            catch
            {
                Initialized = false;
            }
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();

            var model = prj.Project.Model;
            int progress = 0;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            var welnum_list = TypeConverterEx.Split<int>(WellNumFlagList);
            var rate_fileds_list = TypeConverterEx.Split<string>(PumpingRateFieldList);
            int np = mf.TimeService.StressPeriods.Count;
            if (welnum_list == null || welnum_list.Length != np)
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: WellNumFlagList is wrong, please check it");
                return false;
            }
            else
            {
                if (mf != null)
                {
                    if (!mf.Packages.ContainsKey(WELPackage.PackageName))
                    {
                        var wel = mf.Select(WELPackage.PackageName);
                        mf.Add(wel);
                    }
                    var mfout = mf.GetPackage(MFOutputPackage.PackageName) as MFOutputPackage;
                    var pck = mf.GetPackage(WELPackage.PackageName) as WELPackage;
                    int nwel = _sourcefs.Features.Count;
                    pck.MXACTW = nwel;
                    pck.IWELCB = 0;
                    pck.FluxRates = new DataCube<float>(4, np, nwel)
                    {
                        DateTimes = new System.DateTime[np],
                        Variables = new string[4] { "Layer", "Row", "Column", "Q" }
                    };
                    int layer = 1;
                    float rate = 0;
                    //Coordinate pt = null;
                    for (int n = 0; n < np; n++)
                    {
                        if (welnum_list[n] > 0)
                        {
                            pck.FluxRates.Flags[n] = TimeVarientFlag.Individual;
                            pck.FluxRates.Multipliers[n] = 1;
                            pck.FluxRates.IPRN[n] = -1;

                            for (int i = 0; i < nwel; i++)
                            {
                                int.TryParse(_sourcefs.DataTable.Rows[i][LayerField].ToString(), out layer);
                                float.TryParse(_sourcefs.DataTable.Rows[i][rate_fileds_list[n]].ToString(), out rate);
                                //pt = _sourcefs.Features[i].Geometry.Coordinate;
                                for (int j = 0; j < _grid_layer.Features.Count; j++)
                                {
                                    //var cell = _grid_layer.Features[j].Geometry.Coordinates;
                                    // if (SpatialRelationship.PointInPolygon(cell, pt))
                                    if (_sourcefs.Features[i].Geometry.Within(_grid_layer.Features[j].Geometry))
                                    {
                                        pck.FluxRates[0, n, i] = layer;
                                        pck.FluxRates[1, n, i] = int.Parse(_grid_layer.DataTable.Rows[j]["ROW"].ToString());
                                        pck.FluxRates[2, n, i] = int.Parse(_grid_layer.DataTable.Rows[j]["COLUMN"].ToString());
                                        pck.FluxRates[3, n, i] = rate;
                                        break;
                                    }
                                }
                            }
                    
                        }
                        else if (welnum_list[n] == 0)
                        {
                            pck.FluxRates.Flags[n] = TimeVarientFlag.Constant;
                            pck.FluxRates.Multipliers[n] = 1;
                            pck.FluxRates.IPRN[n] = -1;
                        }
                        else if (welnum_list[n] < 0)
                        {
                            pck.FluxRates.Flags[n] = TimeVarientFlag.Repeat;
                            pck.FluxRates.Multipliers[n] = 1;
                            pck.FluxRates.IPRN[n] = -1;
                            //var size = pck.FluxRates.GetVariableSize(n - 1);
                            //var buf = new float[size[1]];
                            //pck.FluxRates[n - 1, "0", ":"].CopyTo(buf, 0);
                            //pck.FluxRates[n, "0", ":"] = buf;
                        }
                        pck.FluxRates.DateTimes[n] = mf.TimeService.StressPeriods[n].End;
                        progress = n * 100 / np;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Process stress period: " + (n + 1));
                    }
                    pck.CompositeOutput(mfout);
                    pck.CreateFeature(shell.MapAppManager.Map.Projection, prj.Project.GeoSpatialDirectory);
                    pck.BuildTopology();
                    pck.IsDirty = true;
                    pck.Save(null);
                    pck.ChangeState(Models.Generic.ModelObjectState.Ready);

                    _issuccess = true;
                    return true;
                }
                else
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow is used by this tool.");
                    _issuccess = false;
                    return false;
                }
            }
        }

        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();

            if (prj.Project.Model is Heiflow.Models.Integration.HeiflowModel)
            {
                var wel = prj.Project.Model.GetPackage(WELPackage.PackageName) as WELPackage;
                if (wel != null && _issuccess)
                {
                    wel.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                    shell.ProjectExplorer.ClearContent();
                    shell.ProjectExplorer.AddProject(prj.Project);
                }
            }
        }
    }
}