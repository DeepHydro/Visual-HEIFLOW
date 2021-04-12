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
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Heiflow.Tools.ConceptualModel
{
    public enum StreamGenerator { VHF,SWAT, Other};
    public class CreateSFRTool : MapLayerRequiredTool
    {
        private IFeatureSet _stream_layer;
        private IFeatureSet _grid_layer;
        private IFeatureSet _sfr_insct_layer;
        private double _minum_slope = 0.0001;
        private double _maximum_slope = 0.02;
        private IMapLayerDescriptor _StreamGridInctLayerDescriptor;
        private IMapLayerDescriptor _StreamFeatureLayerDescriptor;
        private StreamGenerator _StreamGenerator;
        private bool issucuess = false;
        public CreateSFRTool()
        {
            Name = "Create SFR Package";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Create SFR Package based on Line Feature Layer";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;

            Width = 50;
            Flow = 0;
            Runoff = 0;
            ET = 0;
            Rainfall = 0;
            Roughness = 0.05;
            Offset = -2;
            
            BedThickness = 2;
            STRHC1 = 0.1;
            THTS = 0.3;
            THTI = 0.2;
            EPS = 3.5;
            SlopeScale = 0.01;

            MinReachLength = 10;
    
            IgnoreMinorReach = false;
            UseLPFVK = true;
            StreamGenerator = ConceptualModel.StreamGenerator.VHF;
        }

        #region GIS Layers
        [Category("Input GIS Layer")]
        [Description("Model grid layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Category("Input GIS Layer")]
        [Description("Stream layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor StreamFeatureLayer
        {
            get
            {
                return _StreamFeatureLayerDescriptor;
            }
            set
            {
                _StreamFeatureLayerDescriptor = value;
                if (_StreamFeatureLayerDescriptor != null)
                {
                    var sourcefs = _StreamFeatureLayerDescriptor.DataSet as IFeatureSet;
                    if (sourcefs != null)
                    {
                        var buf = from DataColumn dc in sourcefs.DataTable.Columns select dc.ColumnName;
                        Fields = buf.ToArray();
                    }
                }
            }
        }

        [Category("Input GIS Layer (Optional)")]
        [Description("Stream-Grid intersection layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor StreamGridIntersectionLayer
        {
            get
            {
                return _StreamGridInctLayerDescriptor;
            }
            set
            {
                _StreamGridInctLayerDescriptor = value;
                if (_StreamGridInctLayerDescriptor != null)
                {
                    var sourcefs = _StreamGridInctLayerDescriptor.DataSet as IFeatureSet;
                    if (sourcefs != null)
                    {
                        var buf = from DataColumn dc in sourcefs.DataTable.Columns select dc.ColumnName;
                        Fields = buf.ToArray();
                    }
                }
            }
        }
        #endregion

        #region Stream Network
        [Category("Stream Network")]
        [Description("the generator used to generate stream network.")]

        public StreamGenerator StreamGenerator
        {
            get
            {
                return _StreamGenerator;
            }
            set
            {
                _StreamGenerator = value;
                if (_StreamGenerator == StreamGenerator.VHF)
                {
                    SegmentIDOffset = 1;
                    SegmentIDField = "WSNO";
                    OutSegmentIDField = "DSLINKNO";
                    IgnoreMinorReach = true;
                    ReverseOrder = true;
                    WidthField = "Width";
                    SlopeField = "Slope";
                }
                else if (_StreamGenerator == StreamGenerator.SWAT)
                {
                    SegmentIDOffset = 0;
                    SegmentIDField = "FROM_NODE";
                    OutSegmentIDField = "TO_NODE";
                    IgnoreMinorReach = true;
                    ReverseOrder = false;
                    WidthField = "Wid2";
                    SlopeField = "Slo2";
                }
            }
        }


        [Category("Stream Network")]
        [Description("Ignore reach whose length is very small")]
        public bool IgnoreMinorReach
        {
            get;
            set;
        }
        [Category("Stream Network")]
        [Description("Offset to segment ID. When using the watershed delineation tool of VHF, the offset must be 1. If other tools used like ArcSWAT, the offset must be 0")]
        public int SegmentIDOffset
        {
            get;
            set;
        }
        [Category("Stream Network")]
        [Description("Reverse orders of points in a reach")]
        public bool ReverseOrder
        {
            get;
            set;
        }

        [Category("Stream Network")]
        [Description("Set hydroconductivity of streambed using VK values in LPF")]
        public bool UseLPFVK
        {
            get;
            set;
        }
        #endregion

        #region Field Binding
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }
        [Category("Mandatory Segment Field Binding")]
        [Description("Segment ID")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SegmentIDField
        {
            get;
            set;
        }
        [Category("Mandatory Segment Field Binding")]
        [Description("Out (or down) segment ID")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string OutSegmentIDField
        {
            get;
            set;
        }

        //[Category("Mandatory Segment Field Binding")]
        //[Description("0 indicates that the segment is manually edited. Otherwise the segment is automatically generated")]
        //[EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        //[DropdownListSource("Fields")]
        //public string IsManualSegmentField
        //{
        //    get;
        //    set;
        //}
        [Category("Mandatory Segment Field Binding")]
        [Description("Segment width")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string WidthField
        {
            get;
            set;
        }
        [Category("Mandatory Segment Field Binding")]
        [Description("Segment slope")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SlopeField
        {
            get;
            set;
        }
        [Category("Mandatory Segment Field Binding")]
        [Description("Segment slope")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public double SlopeScale
        {
            get;
            set;
        }
     
        #endregion

        #region Default Value
        [Category("Reach Default Value")]
        [Description("Offset to the top elevation of the streambed")]
        public double Offset
        {
            get;
            set;
        }

        [Category("Reach Default Value")]
        [Description("The thickness of the streambed")]
        public double BedThickness
        {
            get;
            set;
        }

        [Category("Reach Default Value")]
        [Description("The defualt hydraulic conductivity of the streambed")]
        public double STRHC1
        {
            get;
            set;
        }
        [Category("Reach Default Value")]
        [Description("The saturated volumetric water content in the unsaturated zone")]
        public double THTS
        {
            get;
            set;
        }
        [Category("Reach Default Value")]
        [Description("The initial volumetric water content. THTI must be less than or equal to THTS and greater than or equal to THTS minus the specific yield defined in either LPF or BCF. This variable is read when ISFROPT is 2 or 3.")]
        public double THTI
        {
            get;
            set;
        }
        [Category("Reach Default Value")]
        [Description("The Brooks-Corey exponent used in the relation between water content and hydraulic conductivity within the unsaturated zone (Brooks and Corey, 1966). This variable is read when ISFROPT is 2 or 3.")]
        public double EPS
        {
            get;
            set;
        }
           
        [Category("Reach Default Value")]
        [Description("The minimum length of a reach. Reach whose lenght is smaller than this value will be ignored.")]
        public double MinReachLength
        {
            get;
            set;
        }

        [Category("Segment Default Value")]
        [Description("Offset to the top elevation of the streambed")]
        public double Width
        {
            get;
            set;
        }
        [Category("Segment Default Value")]
        [Description("")]
        public double Flow
        {
            get;
            set;
        }

        [Category("Segment Default Value")]
        [Description("")]
        public double Runoff
        {
            get;
            set;
        }

        [Category("Segment Default Value")]
        [Description("")]
        public double ET
        {
            get;
            set;
        }

        [Category("Segment Default Value")]
        [Description("")]
        public double Rainfall
        {
            get;
            set;
        }

        [Category("Segment Default Value")]
        [Description("")]
        public double Roughness
        {
            get;
            set;
        }


        [Category("Segment Default Value")]
        [Description("The maximum slope")]
        public double MaxSlope
        {
            get
            {
                return _maximum_slope;
            }
            set
            {
                _maximum_slope = value;
            }
        }

        [Category("Segment Default Value")]
        [Description("The minimum slope")]
        public double MinSlope
        {
            get
            {
                return _minum_slope;
            }
            set
            {
                _minum_slope = value;
            }
        }
        #endregion

        public override void Initialize()
        {
            if (GridFeatureLayer == null || StreamFeatureLayer == null)
            {
                this.Initialized = false;
                return;
            }
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            _stream_layer = StreamFeatureLayer.DataSet as IFeatureSet;
            if (_grid_layer == null)
            {
                this.Initialized = false;
                return;
            }
            this.Initialized = !(_grid_layer == null || _grid_layer.FeatureType != FeatureType.Polygon);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            issucuess = false;
            var dic = Path.GetDirectoryName(_stream_layer.FilePath);
            var out_fn = Path.Combine(dic, "sfr_cpm.shp");
            Dictionary<int, ReachFeatureCollection> fea_list = new Dictionary<int, ReachFeatureCollection>();
            cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
            if (StreamGridIntersectionLayer != null)
                _sfr_insct_layer = StreamGridIntersectionLayer.DataSet as FeatureSet;
            else
            {
                _sfr_insct_layer = _stream_layer.Intersection1(_grid_layer, FieldJoinType.All, null);
                _sfr_insct_layer.Projection = _stream_layer.Projection;
                _sfr_insct_layer.SaveAs(out_fn, true);
                StreamGridIntersectionLayer = new MapLayerDescriptor()
                {
                    DataSet = _sfr_insct_layer,
                    LegendText = "sfr_cpm"
                };
            }
            PreproIntersection();
            var dt_insct = _sfr_insct_layer.DataTable;
            var segid = new List<int>();
            foreach (DataRow row in dt_insct.Rows)
            {
                var temp = int.Parse(row[SegmentIDField].ToString()) + SegmentIDOffset;
                segid.Add(temp);
            }
            var distinct_segs = segid.Distinct();

            foreach (var id in distinct_segs)
            {
                var rch = new ReachFeatureCollection(id);
                if (ReverseOrder)
                {
                    var descendingComparer = Comparer<double>.Create((x, y) => y.CompareTo(x));
                    rch.Reaches = new SortedList<double, ReachFeature>(descendingComparer);
                }
                else
                {
                    rch.Reaches = new SortedList<double, ReachFeature>();
                }
                fea_list.Add(id, rch);
            }

            cancelProgressHandler.Progress("Package_Tool", 30, "Calculation of intersectons between Grid and Stream finished");
            PreProByOrder(fea_list, cancelProgressHandler);
            cancelProgressHandler.Progress("Package_Tool", 70, "Calculation of reach parameters finished");
            Save2SFRFile(fea_list, cancelProgressHandler);
            issucuess = true;
            cancelProgressHandler.Progress("Package_Tool", 90, "SFR file saved");
            return true;
        }

        private void PreproIntersection()
        {
            var dt_insct = _sfr_insct_layer.DataTable;
            if (!dt_insct.Columns.Contains(WidthField))
            {
                DataColumn dc = new DataColumn(WidthField, typeof(double));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[WidthField] = this.Width;
                }
            }
            if (!dt_insct.Columns.Contains(SlopeField))
            {
                DataColumn dc = new DataColumn(SlopeField, typeof(double));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[SlopeField] = 0.0001;
                }
            }
            _sfr_insct_layer.Save();
        }
        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            if (issucuess)
            {
                if (prj.Project.Model is HeiflowModel)
                {
                    var model = prj.Project.Model as HeiflowModel;
                    var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
                    model.PRMSModel.MMSPackage.Parameters["nreach"].SetValue(0, 0, 0, sfr.NSTRM);
                    model.PRMSModel.MMSPackage.Parameters["nsegment"].SetValue(0, 0, 0, sfr.NSS);
                    model.PRMSModel.MMSPackage.IsDirty = true;
                    model.PRMSModel.MMSPackage.Save(null);
                    sfr.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                    shell.ProjectExplorer.ClearContent();
                    shell.ProjectExplorer.AddProject(prj.Project);
                    sfr.UpdateFeature(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                }
                else if (prj.Project.Model is Modflow )
                {
                    var model = prj.Project.Model as Modflow;
                    var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
                    sfr.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                    shell.ProjectExplorer.ClearContent();
                    shell.ProjectExplorer.AddProject(prj.Project);
                    sfr.UpdateFeature(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                }

            }

        }

        private double FindNearestPointIndex(Coordinate[] path, Coordinate target)
        {
            int nseg = path.Count() - 1;
            double[] seglen = new double[nseg];
            double accum_lenth = 0;
            double mindis = double.MaxValue;
            int nearest_index = 0;
            Coordinate nearestpt=null;
            for (int i = 0; i < nseg; i++)
            {
                seglen[i] = path[i].Distance(path[i + 1]);
                var dis = FindDistanceToSegment(target, path[i], path[i + 1], out nearestpt);
                if(dis < mindis)
                {
                    mindis = dis;
                    nearest_index = i;
                }
            }

            if(nearest_index > 0)
            {
                for (int i = 0; i < nearest_index; i++)
                {
                    accum_lenth += seglen[i];
                }
                accum_lenth += target.Distance(path[nearest_index]);
            }
            else
            {
                accum_lenth = target.Distance(path[nearest_index]);
            }
            return accum_lenth;
        }

        /// <summary>
        ///  Calculate the distance between point pt and the segment p1 --> p2.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="closest"></param>
        /// <returns></returns>
        public static double FindDistanceToSegment(Coordinate pt, Coordinate p1, Coordinate p2, out Coordinate closest)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return System.Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Coordinate(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Coordinate(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Coordinate(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        private void PreProByOrder(Dictionary<int, ReachFeatureCollection> fealist, ICancelProgressHandler cancelProgressHandler)
        {
            var dt = _sfr_insct_layer.DataTable;
            var dt_stream = _stream_layer.DataTable;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();     
            var grid = prj.Project.Model.Grid as MFGrid;
            Modflow mf = null;
            if (prj.Project.Model is HeiflowModel)
                mf = (prj.Project.Model as HeiflowModel).ModflowModel;
            else if (prj.Project.Model is Modflow)
                mf = prj.Project.Model as Modflow;
            var lpf = mf.FlowPropertyPackage;
            var duplicated_rch = 0;
            var ignored_rch = 0;
            Dictionary<int, double> cell_reach = new Dictionary<int, double>();
            for (int i = 0; i < _stream_layer.Features.Count; i++)
            {
                var fea_stream = _stream_layer.GetFeature(i);
                var dr_stream = fea_stream.DataRow;
                var geo_stream = fea_stream.Geometry;
                int segid = int.Parse(dr_stream[SegmentIDField].ToString()) + SegmentIDOffset;
                int k = 0;
                var npt_stream = geo_stream.Coordinates.Count();
                var reaches = from fs in _sfr_insct_layer.Features where (int.Parse(fs.DataRow[SegmentIDField].ToString()) + SegmentIDOffset) == segid select fs;
                var reach_orders = new int[reaches.Count()];
                cell_reach.Clear();

                foreach (var rch in reaches)
                {
                    var rch_geo = rch.Geometry;
                    double order = 0;
                    var dr = rch.DataRow;
                    int row = int.Parse(dr["ROW"].ToString());
                    int col = int.Parse(dr["COLUMN"].ToString());
                    int cellid = grid.Topology.GetID(row - 1, col - 1);
                    order = FindNearestPointIndex(geo_stream.Coordinates, rch_geo.Coordinates[0]);

                    if (rch_geo.Length <= MinReachLength && IgnoreMinorReach)
                    {
                        ignored_rch++;
                        continue;
                    }
                    if (fealist[segid].Reaches.Keys.Contains(order))
                    {
                        duplicated_rch++;
                        continue;
                    }
                    if(cell_reach.Keys.Contains(cellid))
                    {
                        duplicated_rch++;
                        var buf = from r in  fealist[segid].Reaches.Values where r.CellID == cellid select r;
                        if (buf.Any())
                            buf.First().Length += rch_geo.Length;
                        continue;
                    }

                    if (grid.IsActive(row - 1, col - 1, 0))
                    {
                        double elev_av = grid.GetElevationAt(row - 1, col - 1, 0);
                        double slope = MinSlope;
                        var cellindex = grid.Topology.GetSerialIndex(row - 1, col - 1);
                        var vk = lpf.HK[0, 0, cellindex] / lpf.VKA[0, 0, cellindex];
                        var reach = new ReachFeature()
                        {
                            Row = row,
                            Column = col,
                            DataRow = dr,
                            Elevation = elev_av,
                            Slope = slope,
                            Length = rch_geo.Length,
                            CellID = cellid
                        };
                      
                        reach.Width = TypeConverterEx.IsNotNull(dr[WidthField].ToString()) ? double.Parse(dr[WidthField].ToString()) : this.Width;
                        reach.Slope = TypeConverterEx.IsNotNull(dr[SlopeField].ToString()) ? double.Parse(dr[SlopeField].ToString()) * SlopeScale : 0.0001;

                        reach.UpRiverID = 0;
                        reach.IPrior = 0;
                        reach.Flow = this.Flow;
                        reach.Runoff = this.Runoff;
                        reach.Rainfall = this.Rainfall;
                        reach.ET = this.ET;
                        reach.Roughness = this.Roughness;

                        if (UseLPFVK)
                        {
                            reach.STRCH1 = vk;
                        }
                        else
                        {
                            reach.STRCH1 = this.STRHC1;
                        }

                        reach.Offset = this.Offset;
                        reach.BedThickness = this.BedThickness;
                        reach.THTI = this.THTI;
                        reach.THTS = this.THTS;
                        reach.EPS = this.EPS;

                        reach.OrderKey = order;
                        fealist[segid].Reaches.Add(order, reach);
                        fealist[segid].OutSegmentID = int.Parse(dr[OutSegmentIDField].ToString());
                        k++;
                    }
                }
            }

            var list = new List<int>();
            int nseg = fealist.Keys.Count;
            var oldid_newid = new Dictionary<int, int>();
            var newfealist = new Dictionary<int, ReachFeatureCollection>();
            foreach (var seg in fealist.Values)
            {
                if (seg.Reaches.Count == 0)
                    list.Add(seg.SegmentID);
            }
            foreach (var segid in list)
            {
                fealist.Remove(segid);
            }
            var keys = fealist.Keys;
            var sortedkeys = keys.OrderBy(x => x);

            for (int i = 0; i < sortedkeys.Count(); i++)
            {
                oldid_newid.Add(sortedkeys.ElementAt(i), i + 1);
            }
            foreach (var seg in fealist.Values)
            {
                seg.SegmentID = oldid_newid[seg.SegmentID];
                if (sortedkeys.Contains(seg.OutSegmentID))
                    seg.OutSegmentID = oldid_newid[seg.OutSegmentID];
                else
                    seg.OutSegmentID = -1;
                newfealist.Add(seg.SegmentID, seg);
            }
            fealist.Clear();
            foreach (var segid in newfealist.Keys)
            {
                fealist.Add(segid, newfealist[segid]);
            }

            if(ignored_rch > 0)
            {
                cancelProgressHandler.Progress("Package_Tool", 70, "The number of ignored reaches is: " + ignored_rch);
            }
            if (duplicated_rch > 0)
            {
                cancelProgressHandler.Progress("Package_Tool", 70, "The number of duplicated reaches is: " + duplicated_rch);
            }
        }
       
        private void Save2SFRFile(Dictionary<int, ReachFeatureCollection> fea_list, ICancelProgressHandler cancelProgressHandler)
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
                    sfr.SPInfo[0, 0] = nseg;
                    for (int i = 1; i < nsp; i++)
                    {
                        sfr.SPInfo[i, 0] = -1;
                    }

                    for (int i = 0; i < nseg; i++)
                    {
                        try
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

                            var firstreach = dr_reaches.Values.First();
                            river.Runoff = firstreach.Runoff;
                            river.ETSW = firstreach.ET;
                            river.PPTSW = firstreach.Rainfall;
                            river.ROUGHCH = firstreach.Roughness;
                            river.Width = firstreach.Width;
                            river.Width1 = firstreach.Width;
                            river.Width2 = firstreach.Width;
                            river.UpRiverID = firstreach.UpRiverID;
                            river.IPrior = firstreach.IPrior;
                            river.Flow = firstreach.Flow;
                            for (int c = 0; c < dr_reaches.Count; c++)
                            {
                                var fea = dr_reaches.ElementAt(c).Value;
                                var dr = fea.DataRow;
                                int row = int.Parse(dr["ROW"].ToString());
                                int col = int.Parse(dr["COLUMN"].ToString());
                                var index = grid.Topology.GetSerialIndex(row - 1, col - 1);
                                if (grid.IsActive(row - 1, col - 1, 0))
                                {
                                    Reach rch = new Reach(reach_id)
                                    {
                                        KRCH = 1,
                                        IRCH = row,
                                        JRCH = col,
                                        ISEG = seg_id,
                                        IREACH = reach_local_id,
                                        Length = fea.Length,
                                        TopElevation = fea.Elevation + fea.Offset,
                                        Slope = fea.Slope,
                                        BedThick = fea.BedThickness,
                                        STRHC1 = fea.STRCH1,
                                        THTS = fea.THTS,
                                        THTI = fea.THTI,
                                        EPS = fea.EPS,
                                        Name = reach_id.ToString(),
                                        SubID = reach_local_id,
                                        SubIndex = reach_local_id - 1
                                    };
                                    if (rch.Slope <= 0)
                                        rch.Slope = MinSlope;

                                    if ((rch.TopElevation - rch.BedThick) < grid.Elevations[1, 0, index])
                                    {
                                        rch.TopElevation = grid.Elevations[1, 0, index] + rch.BedThick + System.Math.Abs(Offset);
                                        string msg = string.Format("The top elevation of the reach {0} {1} is modified ", rch.ISEG, rch.IREACH);
                                        cancelProgressHandler.Progress("Package_Tool", 80, "Warnings:  " + msg);
                                    }

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
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

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
