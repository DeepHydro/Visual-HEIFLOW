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
using Heiflow.Core.Hydrology;
using Heiflow.Core.MyMath;
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
    public class SFR2 : MapLayerRequiredTool
    {
        private IFeatureSet _stream_layer;
        private IFeatureSet _grid_layer;
        private IFeatureSet  _out_sfr_layer;
        private IRaster _dem_layer;
          private IRaster _ad_layer;
        private double _minum_slope = 0.001;
        private double _maximum_slope = 0.02;
        
        public SFR2()
        {
            Name = "Streamflow Routing";
            Category = "Conceptual Model";
            Description = "Translate stream shapefile to SFR2 package";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            BedThickness = 2;
            STRHC1 = 0.1;
            THTS = 0.3;
            THTI = 0.2;
            EPS = 3.5;
            Offset = -2;
            ROUGHCH = 0.05;
            Width1 = 50;
            Width2 = 50;
            SegmentField = "WSNO";
        }

        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("DEM")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor DEMLayer
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("Accumulated drainage map layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor AccumulatedDrainageLayer
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("Stream layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor StreamFeatureLayer
        {
            get;
            set;
        }


        [Category("Optional")]
        [Description("Stream-Grid intersection layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor StreamGridInctLayer
        {
            get;
            set;
        }
        [Category("River Parameter")]
        [Description("")]
        public string SegmentField
        {
            get;
            set;
        }
        [Category("Reach Parameter")]
        [Description("Offset to the top elevation of the streambed")]
        public double Offset
        {
            get;
            set;
        }

        [Category("Reach Parameter")]
        [Description("The uniform thickness of the streambed")]
        public double BedThickness
        {
            get;
            set;
        }

        [Category("Reach Parameter")]
        [Description("The uniform hydraulic conductivity of the streambed")]
        public double STRHC1 
        {
            get;
            set;
        }
        [Category("Reach Parameter")]
        [Description("The saturated volumetric water content in the unsaturated zone")]
        public double THTS 
        {
            get;
            set;
        }
        [Category("Reach Parameter")]
        [Description("The initial volumetric water content. THTI must be less than or equal to THTS and greater than or equal to THTS minus the specific yield defined in either LPF or BCF. This variable is read when ISFROPT is 2 or 3.")]
        public double THTI 
        {
            get;
            set;
        }
        [Category("Reach Parameter")]
        [Description("The Brooks-Corey exponent used in the relation between water content and hydraulic conductivity within the unsaturated zone (Brooks and Corey, 1966). This variable is read when ISFROPT is 2 or 3.")]
        public double EPS 
        {
            get;
            set;
        }

        [Category("River Parameter")]
        [Description("")]
        public double Flow
        {
            get;
            set;
        }

        [Category("River Parameter")]
        [Description("")]
        public double Runoff
        {
            get;
            set;
        }

        [Category("River Parameter")]
        [Description("")]
        public double ETSW
        {
            get;
            set;
        }

        [Category("River Parameter")]
        [Description("")]
        public double PPTSW
        {
            get;
            set;
        }

        [Category("River Parameter")]
        [Description("")]
        public double ROUGHCH
        {
            get;
            set;
        }

        [Category("River Parameter")]
        [Description("")]
        public double Width1
        {
            get;
            set;
        }
        [Category("River Parameter")]
        [Description("")]
        public double Width2
        {
            get;
            set;
        }

        public override void Initialize()
        {
            if (GridFeatureLayer == null || StreamFeatureLayer == null || DEMLayer== null)
            {
                this.Initialized = false;
                return;
            }
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            _stream_layer = StreamFeatureLayer.DataSet as IFeatureSet;
            _dem_layer = DEMLayer.DataSet as IRaster;
            _ad_layer = AccumulatedDrainageLayer.DataSet as IRaster;
            if (  _grid_layer == null || _dem_layer == null)
            {
                this.Initialized = false;
                return;
            }
           
            this.Initialized = !(_grid_layer == null || _grid_layer.FeatureType != FeatureType.Polygon);  
         //   this.Initialized = !(_stream_layer == null || _stream_layer.FeatureType != FeatureType.Line);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var dt_stream = _stream_layer.DataTable;
            var segid = from dr in dt_stream.AsEnumerable() select (dr.Field<int>(SegmentField) + 1);
            var dic = Path.GetDirectoryName(_stream_layer.FilePath);
            var out_fn = Path.Combine(dic, "sfr_cpm.shp");
        string msg="";
            Dictionary<int, ReachFeatureCollection> fea_list = new Dictionary<int, ReachFeatureCollection>();

            foreach(var id in segid)
            {
                fea_list.Add(id, new ReachFeatureCollection(id));
            }
              cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
              if (StreamGridInctLayer != null)
                  _out_sfr_layer = StreamGridInctLayer.DataSet as FeatureSet;
              else
              {
                  _out_sfr_layer = _stream_layer.Intersection1(_grid_layer, FieldJoinType.All, null);
                  _out_sfr_layer.Projection = _stream_layer.Projection;
                  _out_sfr_layer.SaveAs(out_fn, true);
              }
            cancelProgressHandler.Progress("Package_Tool", 30, "Calculation of intersectons between Grid and Stream finished");
            PrePro(fea_list, out msg);
            cancelProgressHandler.Progress("Package_Tool", 70, "Calculation of reach parameters finished");
            if(msg != "")
                cancelProgressHandler.Progress("Package_Tool", 80, "Warnings: " + msg);
            Save2SFRFile(fea_list);
            
            cancelProgressHandler.Progress("Package_Tool", 90, "SFR file saved");
            return true;
        }

        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as Heiflow.Models.Integration.HeiflowModel;
          
            if (model != null)
            {
                var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
                model.PRMSModel.MMSPackage.Parameters["nreach"].SetValue(sfr.NSTRM, 0);
                model.PRMSModel.MMSPackage.Parameters["nsegment"].SetValue(sfr.NSS, 0);
                model.PRMSModel.MMSPackage.IsDirty = true;
                model.PRMSModel.MMSPackage.Save(null);
                sfr.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                shell.ProjectExplorer.ClearContent();
                shell.ProjectExplorer.AddProject(prj.Project); 
            }
      
            shell.MapAppManager.Map.AddLayer(_out_sfr_layer.Filename);
        }

        private void PrePro(Dictionary<int, ReachFeatureCollection> fealist, out string msg)
        {
            double rs = 0, slope = 0, yint = 0;
            var dt = _out_sfr_layer.DataTable;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            msg = "";
            for (int i = 0; i < _out_sfr_layer.Features.Count; i++)
            {
                try
                {
                    var dr = dt.Rows[i];
                    var geo = _out_sfr_layer.GetFeature(i).Geometry;
                    if (geo.Length <= _dem_layer.CellHeight)
                    {
                        continue;
                    }
                    var npt = geo.Coordinates.Count();
                    int segid = int.Parse(dr[SegmentField].ToString()) + 1;
                    double[] dis = new double[npt];
                    double[] ac_dis = new double[npt];
                    double[] elvs = new double[npt];
                    double elev_av = 0;
                    var pt0 = geo.Coordinates[0];
                    var cell = _dem_layer.ProjToCell(pt0.X, pt0.Y);
                    double ad = 0;
                    dis[0] = 0;
                    elvs[0] = _dem_layer.Value[cell.Row, cell.Column];
                    for (int j = 0; j < npt; j++)
                    {
                       cell = _ad_layer.ProjToCell(geo.Coordinates[j].X, geo.Coordinates[j].Y);
                        if(cell.Row > 0 && cell.Column > 0)
                            ad += _ad_layer.Value[cell.Row, cell.Column];
                    }
                    ad = ad / npt;
                    for (int j = 1; j < npt; j++)
                    {
                        cell = _dem_layer.ProjToCell(geo.Coordinates[j].X, geo.Coordinates[j].Y);
                        elvs[j] = _dem_layer.Value[cell.Row, cell.Column];
                        dis[j] = SpatialDistance.DistanceBetween(geo.Coordinates[j - 1], geo.Coordinates[j]);
                    }
                    for (int j = 0; j < npt; j++)
                    {
                        ac_dis[j] = dis.Take(j + 1).Sum();
                    }

                    MyStatisticsMath.LinearRegression(ac_dis, elvs, 0, elvs.Length, out rs, out yint, out slope);

                    if (slope < 0)
                    {
                        slope = -slope;
                    }
                    else if (slope == 0)
                    {
                        slope = _minum_slope;
                    }

                    for (int j = 0; j < npt; j++)
                    {
                        elvs[j] = yint + slope * ac_dis[j];
                    }
                    elev_av = elvs.Average();

                    if (slope < _minum_slope)
                        slope = _minum_slope;
                    if (slope > _maximum_slope)
                        slope = _maximum_slope;

                    var rch = new ReachFeature()
                    {
                        Row = dr,
                        Elevation = elev_av,
                        Slope = slope
                    };

                    if (fealist[segid].Reaches.Count > 0 && fealist[segid].Reaches.ContainsKey(ad))
                    {
                        ad += i * 0.001;
                    }
                    fealist[segid].Reaches.Add(ad, rch);
                    fealist[segid].OutSegmentID = int.Parse(dr["DSLINKNO"].ToString());
                    dr["Length"] = geo.Length;
                }
                catch (Exception ex)
                {
                    msg += ex.Message + "\n";
                }
            }          
        }

        private void Save2SFRFile(Dictionary<int, ReachFeatureCollection> fea_list)
        {
            var ps = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            if (ps.Project != null)
            {
                var sfr = ps.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
                var grid = ps.Project.Model.Grid as MFGrid;
                var mfout = ps.Project.Model.GetPackage(MFOutputPackage.PackageName) as MFOutputPackage;
                if (sfr != null)
                {
                    var net = new RiverNetwork();
                    var nseg = fea_list.Keys.Count;
                    var nreach = (from fea in fea_list.Values select fea.NReach).Sum();
                   var reach_id = 1;
                   var reach_count = 0;

                   net.ReachCount = nreach;
                   net.RiverCount = nseg;

                   sfr.NSTRM = net.ReachCount;
                   sfr.NSS = net.RiverCount;
                   sfr.CONST = 86400;
                   sfr.DLEAK = 0.01f;
                   sfr.RiverNetwork = net;

                   var nsp = sfr.TimeService.StressPeriods.Count;
                   sfr.SPInfo = new int[nsp, 3];
                    sfr.SPInfo[0,0]=nseg;
                   for (int i = 1; i < nsp; i++)
                   {
                       sfr.SPInfo[i, 0] = -1;
                   }
                   
                   for (int i = 0; i < nseg; i++)
                   {
                       var seg_id = i + 1;
                       River river = new River(seg_id)
                       {
                           Name = seg_id.ToString(),
                           SubIndex = i,
                           ICALC = 1
                       };
                       var dr_reaches = fea_list[seg_id].Reaches;
                       var out_segid = fea_list[seg_id].OutSegmentID;
                       var reach_local_id = 1;
                       out_segid = out_segid < 0 ? 0 : out_segid;
                       river.OutRiverID = out_segid;
                       river.UpRiverID = 0;
                       river.Flow = this.Flow;
                       river.Runoff = this.Runoff;
                       river.ETSW = this.ETSW;
                       river.PPTSW = this.PPTSW;
                       river.ROUGHCH = this.ROUGHCH;
                       river.Width1 = this.Width1;
                       river.Width2 = this.Width2;

                       for (int c = 0; c < dr_reaches.Count; c++)
                       {
                           var fea=dr_reaches.ElementAt(c).Value;
                           var dr = fea.Row;
                           int row = int.Parse(dr["ROW"].ToString());
                           int col = int.Parse(dr["COLUMN"].ToString());
                           if (grid.IsActive(row - 1, col - 1, 0))
                           {
                               Reach rch = new Reach(reach_id)
                               {
                                   KRCH = 1,
                                   IRCH = int.Parse(dr["ROW"].ToString()),
                                   JRCH = int.Parse(dr["COLUMN"].ToString()),
                                   ISEG = seg_id,
                                   IREACH = reach_local_id,
                                   Length = double.Parse(dr["Length"].ToString()),
                                   TopElevation = fea.Elevation + Offset,
                                   Slope = fea.Slope,
                                   BedThick = BedThickness,
                                   STRHC1 = STRHC1,
                                   THTS = THTS,
                                   THTI = THTI,
                                   EPS = EPS,
                                   Name = reach_id.ToString(),
                                   SubID = reach_local_id,
                                   SubIndex = reach_local_id - 1
                               };
                               river.Reaches.Add(rch);
                               net.Reaches.Add(rch);
                               reach_local_id++;
                               reach_id++;
                           }
                       }
                       if (river.Reaches.Count == 0)
                       {
                           Console.WriteLine("SFR warning: ");
                       }
                       else
                       {
                           net.AddRiver(river);
                           reach_count += river.Reaches.Count;
                       }
                   }
                   sfr.NSTRM = reach_count;
                   sfr.NSS = net.Rivers.Count;

                   sfr.ConnectRivers(net);
                   sfr.NetworkToMat();
                   sfr.BuildTopology();
                   sfr.CompositeOutput(mfout);
                   sfr.ChangeState(Models.Generic.ModelObjectState.Ready);
                   sfr.IsDirty = true;
                   sfr.Save(null);
                   sfr.CreateFeature(shell.MapAppManager.Map.Projection, ps.Project.GeoSpatialDirectory);
                }
            }
        }
    }
}
