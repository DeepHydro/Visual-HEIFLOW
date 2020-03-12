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
    public enum StreamGenerator { VHF,SWAT, Other};
    public class SFR2Tool : MapLayerRequiredTool
    {
        private IFeatureSet _stream_layer;
        private IFeatureSet _grid_layer;
        private IFeatureSet _sfr_insct_layer;
        private IRaster _dem_layer;
        private IRaster _ad_layer;
        private double _minum_slope = 0.0001;
        private double _maximum_slope = 0.02;
        private IMapLayerDescriptor _StreamGridInctLayerDescriptor;
        private IMapLayerDescriptor _StreamFeatureLayerDescriptor;
        private StreamGenerator _StreamGenerator;
        private bool issucuess = false;
        public SFR2Tool()
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
            Slope = 0.001;
            Offset = -2;
            
            BedThickness = 2;
            STRHC1 = 0.1;
            THTS = 0.3;
            THTI = 0.2;
            EPS = 3.5;
    
            IgnoreMinorReach = true;
            UseAccumulativeRaster = false;
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
        [Description("DEM")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor DEMLayer
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

        [Category("Optional GIS Layer")]
        [Description("Accumulated drainage map layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor AccumulatedDrainageLayer
        {
            get;
            set;
        }
        [Category("Optional GIS Layer")]
        [Description("Stream-Grid intersection layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor StreamGridInctLayer
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
                    SegIDOffset = -1;
                    SegmentField = "LINKNO";
                    OutSegmentField = "DSLINKNO";
                    IsManualSegmentField = "Order";
                    IgnoreMinorReach = true;
                    UseAccumulativeRaster = false;
                    ReverseOrder = true;
                   // UseSWATElevation = false;
                    WidthField = "Width";
                    IUPSEGField = "IUPSEG";
                    IPRIORField = "IPRIO";
                    FlowField = "Flow";
                    RunoffField = "Runoff";
                    ETField = "ET";
                    RainfallField = "Rainfall";
                    RoughnessField = "Roughness";
                    BedThicknessField = "BedThick";
                    //MinElevationField = "MinEl";
                    //MaxElevationField = "MaxEl";
                    SlopeField = "Slope";
                    OffsetField = "Offset";
                    VKField = "VK";
                    THTIField = "THTI";
                    THTSField = "THTS";
                    EPSField = "EPS";
                }
                else if (_StreamGenerator == StreamGenerator.SWAT)
                {
                    SegIDOffset = 0;
                    SegmentField = "FROM_NODE";
                    OutSegmentField = "TO_NODE";
                    IsManualSegmentField = "ARCID";
                    IgnoreMinorReach = true;
                    UseAccumulativeRaster = false;
                    ReverseOrder = false;
                  // UseSWATElevation = false;
                    WidthField = "Wid2";
                    IUPSEGField = "IUPSEG";
                    IPRIORField = "IPRIO";
                    FlowField = "Flow";
                    ETField = "ET";
                    RainfallField = "Rainfall";
                    RoughnessField = "Roughness";
                    BedThicknessField = "BedThick";
                    //MinElevationField = "MinEl";
                    //MaxElevationField = "MaxEl";
                    SlopeField = "Slo2";
                    OffsetField = "Offset";
                    VKField = "VK";
                    THTIField = "THTI";
                    THTSField = "THTS";
                    EPSField = "EPS";
                }
            }
        }

        //[Category("Stream Network")]
        //[Description("When ArcSWAT stream generator, stream elevations automatically generated will be used if setting to True value")]
        //public bool UseSWATElevation
        //{
        //    get;
        //    set;
        //}

        [Category("Stream Network")]
        [Description("Ignore reach whose length is very small")]
        public bool IgnoreMinorReach
        {
            get;
            set;
        }
        [Category("Stream Network")]
        [Description("If VHF stream generator used, Accumulative Raster will be used to determine reach sequences if setting to True value")]
        public bool UseAccumulativeRaster
        {
            get;
            set;
        }
        [Category("Stream Network")]
        [Description("Offset to segment ID. When using the watershed delineation tool of VHF, the offset must be 1. If other tools used like ArcSWAT, the offset must be 0")]
        public int SegIDOffset
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
        public string SegmentField
        {
            get;
            set;
        }
        [Category("Mandatory Segment Field Binding")]
        [Description("Out (or down) segment ID")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string OutSegmentField
        {
            get;
            set;
        }

        [Category("Mandatory Segment Field Binding")]
        [Description("0 indicates that the segment is manually edited. Otherwise the segment is automatically generated")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string IsManualSegmentField
        {
            get;
            set;
        }
        [Category("Mandatory Segment Field Binding")]
        [Description("Segment width")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string WidthField
        {
            get;
            set;
        }
        [Category("Optional Segment Field Binding")]
        [Description("IUPSEG is an integer value of the downstream stream segment that receives tributary inflow from the last downstream reach of this segment.")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string IUPSEGField
        {
            get;
            set;
        }
        //IPRIOR An integer value that only is specified if IUPSEG > 0 (do not specify a value in this field if IUPSEG = 0 or IUPSEG < 0). IPRIOR defines the prioritization system for diversion, such as when insufficient water is available to meet all diversion stipulations, and is used in conjunction with the value of FLOW (specified below).
        //When IPRIOR = 0, then if the specified diversion flow (FLOW) is greater than the flow available in the stream segment from which the diversion is made, the diversion is reduced to the amount available, which will leave no flow available for tributary flow into a downstream tributary of segment IUPSEG.
        //When IPRIOR = -1, then if the specified diversion flow (FLOW) is greater than the flow available in the stream segment from which the diversion is made, no water is diverted from the stream. This approach assumes that once flow in the stream is sufficiently low, diversions from the stream cease, and is the “priority” algorithm that originally was programmed into the STR1 Package (Prudic, 1989).
        //When IPRIOR = -2, then the amount of the diversion is computed as a fraction of the available flow in segment IUPSEG; in this case, 0.0 < FLOW < 1.0.
        //When IPRIOR = -3, then a diversion is made only if the streamflow leaving segment IUPSEG exceeds the value of FLOW. If this occurs, then the quantity of water diverted is the excess flow and the quantity that flows from the last reach of segment IUPSEG into its downstream tributary (OUTSEG) is equal to FLOW. This represents a flood-control type of diversion, as described by Danskin and Hanson (2002).
        [Category("Optional Segment Field Binding")]
        [Description("An integer value that only is specified if IUPSEG > 0 (do not specify a value in this field if IUPSEG = 0 or IUPSEG < 0). ")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string IPRIORField
        {
            get;
            set;
        }
//•	If the stream is a headwater stream, FLOW defines the total inflow to the first reach of the segment. The value can be any number ≥ 0.
//•	If the stream is a tributary stream, FLOW defines additional specified inflow to or withdrawal from the first reach of the segment (that is, in addition to the discharge from the upstream segment of which this is a tributary). This additional flow does not interact with the groundwater system. For example, a positive number might be used to represent direct outflow into a stream from a sewage treatment plant, whereas a negative number might be used to represent pumpage directly from a stream into an intake pipe for a municipal water treatment plant. (Also see additional explanatory notes below.)
//•	If the stream is a diversionary stream, and the diversion is from another stream segment, FLOW defines the streamflow diverted from the last reach of stream segment IUPSEG into the first reach of this segment. The diversion is computed or adjusted according to the value of IPRIOR.
//•	If the stream is a diversionary stream, and the diversion is from a lake, FLOW defines a fixed rate of discharge diverted from the lake into the first reach of this stream segment (unless the lake goes dry) and flow from the lake is not dependent on the value of ICALC. However, if FLOW = 0, then the lake outflow into the first reach of this segment will be calculated on the basis of lake stage relative to the top of the streambed for the first reach using one of the methods defined by ICALC.
        [Category("Optional Segment Field Binding")]
        [Description("FLOW A real number that is the streamflow (in units of volume per time) entering or leaving the upstream end of a stream segment (that is, into the first reach).")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string FlowField
        {
            get;
            set;
        }
        [Category("Optional Segment Field Binding")]
        [Description("")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string RunoffField
        {
            get;
            set;
        }
        [Category("Optional Segment Field Binding")]
        [Description("")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ETField
        {
            get;
            set;
        }
        [Category("Optional Segment Field Binding")]
        [Description("")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string RainfallField
        {
            get;
            set;
        }
        [Category("Optional Segment Field Binding")]
        [Description("")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string RoughnessField
        {
            get;
            set;
        }
        //[Category("Segment Field Binding")]
        //[Description("Segment min elevation")]
        //[EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        //[DropdownListSource("Fields")]
        //public string MinElevationField
        //{
        //    get;
        //    set;
        //}
        //[Category("Segment Field Binding")]
        //[Description("Segment max elevation field")]
        //[EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        //[DropdownListSource("Fields")]
        //public string MaxElevationField
        //{
        //    get;
        //    set;
        //}
        [Category("Optional Reach Field Binding")]
        [Description("Segment mean slope field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SlopeField
        {
            get;
            set;
        }
        [Category("Optional Reach Field Binding")]
        [Description("Thickness of the streambed")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string BedThicknessField
        {
            get;
            set;
        }
        [Category("Optional Reach Field Binding")]
        [Description("Offset to  elevation of the cell coresponding to the reach")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string OffsetField
        {
            get;
            set;
        }
        [Category("Optional Reach Field Binding")]
        [Description("VK is a real number equal to the hydraulic conductivity of the streambed. This variable is read when ISFROPT is 1, 2, or 3.")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string VKField
        {
            get;
            set;
        }
        [Category("Optional Reach Field Binding")]
        [Description("THTS is the saturated volumetric water content in the unsaturated zone.")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string THTSField
        {
            get;
            set;
        }
        [Category("Optional Reach Field Binding")]
        [Description("THTI is the initial volumetric water content")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string THTIField
        {
            get;
            set;
        }
        [Category("Optional Reach Field Binding")]
        [Description("EPS is the Brooks-Corey exponent used in the relation between water content and hydraulic conductivity within the unsaturated zone (Brooks and Corey, 1966). This variable is read when ISFROPT is 2 or 3.")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string EPSField
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
        [Description("The slope of reach")]
        public double Slope
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
            if (GridFeatureLayer == null || StreamFeatureLayer == null || DEMLayer == null)
            {
                this.Initialized = false;
                return;
            }
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            _dem_layer = DEMLayer.DataSet as IRaster;
            _stream_layer = StreamFeatureLayer.DataSet as IFeatureSet;
            if (UseAccumulativeRaster)
            {
                if (AccumulatedDrainageLayer != null)
                    _ad_layer = AccumulatedDrainageLayer.DataSet as IRaster;
                else
                    this.Initialized = false;
             }
            if (_grid_layer == null || _dem_layer == null)
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
            string msg = "";
            Dictionary<int, ReachFeatureCollection> fea_list = new Dictionary<int, ReachFeatureCollection>();
            cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
            if (StreamGridInctLayer != null)
                _sfr_insct_layer = StreamGridInctLayer.DataSet as FeatureSet;
            else
            {
                _sfr_insct_layer = _stream_layer.Intersection1(_grid_layer, FieldJoinType.All, null);
                _sfr_insct_layer.Projection = _stream_layer.Projection;
                _sfr_insct_layer.SaveAs(out_fn, true);
                StreamGridInctLayer = new MapLayerDescriptor()
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
                var temp = int.Parse(row[SegmentField].ToString()) + SegIDOffset;
                segid.Add(temp);
            }
            var distinct_segs = segid.Distinct();

            foreach (var id in distinct_segs)
            {
                var rch= new ReachFeatureCollection(id);
                if(ReverseOrder)
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
            if (UseAccumulativeRaster)
                PrePro(fea_list, out msg);
            else
            {
                PreProByOrder(fea_list, out msg);
            }
            cancelProgressHandler.Progress("Package_Tool", 70, "Calculation of reach parameters finished");
            if (msg != "")
                cancelProgressHandler.Progress("Package_Tool", 80, "Warnings: " + msg);
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
                DataColumn dc = new DataColumn(WidthField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[WidthField] = this.Width;
                }
            }
            if (!dt_insct.Columns.Contains(IUPSEGField))
            {
                DataColumn dc = new DataColumn(IUPSEGField, typeof(int));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[IUPSEGField] = 0;
                }
            }
            if (!dt_insct.Columns.Contains(IPRIORField))
            {
                DataColumn dc = new DataColumn(IPRIORField, typeof(int));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[IPRIORField] = 0;
                }
            }
            if (!dt_insct.Columns.Contains(FlowField))
            {
                DataColumn dc = new DataColumn(FlowField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[FlowField] = this.Flow;
                }
            }
            if (!dt_insct.Columns.Contains(RunoffField))
            {
                DataColumn dc = new DataColumn(RunoffField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[RunoffField] = this.Runoff;
                }
            }
            if (!dt_insct.Columns.Contains(ETField))
            {
                DataColumn dc = new DataColumn(ETField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[ETField] = this.ET;
                }
            }
            if (!dt_insct.Columns.Contains(RainfallField))
            {
                DataColumn dc = new DataColumn(RainfallField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[RainfallField] = this.Rainfall;
                }
            }
            if (!dt_insct.Columns.Contains(RoughnessField))
            {
                DataColumn dc = new DataColumn(RoughnessField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[RoughnessField] = this.Roughness;
                }
            }

            if (!dt_insct.Columns.Contains(OffsetField))
            {
                DataColumn dc = new DataColumn(OffsetField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[OffsetField] = this.Offset;
                }
            }
            if (!dt_insct.Columns.Contains(BedThicknessField))
            {
                DataColumn dc = new DataColumn(BedThicknessField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[BedThicknessField] = this.BedThickness;
                }
            }
            if (!dt_insct.Columns.Contains(SlopeField))
            {
                DataColumn dc = new DataColumn(SlopeField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[SlopeField] = this.Slope;
                }
            }
            if (!dt_insct.Columns.Contains(VKField))
            {
                DataColumn dc = new DataColumn(VKField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[VKField] = this.STRHC1;
                }
            }
            if (!dt_insct.Columns.Contains(THTIField))
            {
                DataColumn dc = new DataColumn(THTIField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[THTIField] = this.THTI;
                }
            }
            if (!dt_insct.Columns.Contains(THTSField))
            {
                DataColumn dc = new DataColumn(THTSField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[THTSField] = this.THTS;
                }
            }
            if (!dt_insct.Columns.Contains(EPSField))
            {
                DataColumn dc = new DataColumn(EPSField, typeof(float));
                dt_insct.Columns.Add(dc);
                foreach (DataRow row in dt_insct.Rows)
                {
                    row[EPSField] = this.EPS;
                }
            }
            _sfr_insct_layer.Save();
        }
        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as Heiflow.Models.Integration.HeiflowModel;

            if (model != null && issucuess)
            {
                var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
                model.PRMSModel.MMSPackage.Parameters["nreach"].SetValue(0, 0, 0, sfr.NSTRM);
                model.PRMSModel.MMSPackage.Parameters["nsegment"].SetValue(0, 0, 0, sfr.NSS);
                model.PRMSModel.MMSPackage.IsDirty = true;
                model.PRMSModel.MMSPackage.Save(null);
                sfr.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                shell.ProjectExplorer.ClearContent();
                shell.ProjectExplorer.AddProject(prj.Project);
            }

           // shell.MapAppManager.Map.AddLayer(_sfr_insct_layer.Filename);
        }

        private void PreProByOrder(Dictionary<int, ReachFeatureCollection> fealist, out string msg)
        {
            msg = "";
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
            for (int i = 0; i < _stream_layer.Features.Count; i++)
            {
                var fea_stream = _stream_layer.GetFeature(i);
                var dr_stream = fea_stream.DataRow;
                var geo_stream = fea_stream.Geometry;
                int segid = int.Parse(dr_stream[SegmentField].ToString()) + SegIDOffset;
                int k = 0;
                var npt_stream = geo_stream.Coordinates.Count();
                var reaches = from fs in _sfr_insct_layer.Features where (int.Parse(fs.DataRow[SegmentField].ToString()) + SegIDOffset) == segid select fs;
                var isman_seg = int.Parse(dr_stream[IsManualSegmentField].ToString()) == 0;
                var order_count = new int[npt_stream];

                foreach (var rch in reaches)
                {
                    var rch_geo = rch.Geometry;
                    int order = 0;
                    var npt = rch_geo.Coordinates.Count();

                    if (rch_geo.Length <= _dem_layer.CellHeight && IgnoreMinorReach)
                    {
                        continue;
                    }
                    if (isman_seg)
                    {
                        order = k;
                    }
                    else
                    {
                        var distance = rch_geo.Coordinates[0].Distance(geo_stream.Coordinates[0]);
                        for (int j = 1; j < npt_stream; j++)
                        {
                            var dist = rch_geo.Coordinates[0].Distance(geo_stream.Coordinates[j]);
                            if (dist < distance)
                            {
                                distance = dist;
                                order = j;
                            }
                        }
                        order_count[order]++;
                    }

                    var dr = rch.DataRow;
                    //double rs = 0, yint = 0;
                    //double[] dis = new double[npt];
                    //double[] ac_dis = new double[npt];
                    //double[] elvs = new double[npt];
                    //double elev_av = 0;
                    //var pt0 = rch_geo.Coordinates[0];
                    //var cell = _dem_layer.ProjToCell(pt0.X, pt0.Y);

                    int row = int.Parse(dr["ROW"].ToString());
                    int col = int.Parse(dr["COLUMN"].ToString());
                    if (grid.IsActive(row - 1, col - 1, 0))
                    {
                        double elev_av = grid.GetElevationAt(row - 1, col - 1, 0);
                        double slope = MinSlope;
                        var cellindex = grid.Topology.GetSerialIndex(row-1,col-1);
                        var vk = lpf.HK[0, 0, cellindex] / lpf.VKA[0, 0, cellindex];
                        //dis[0] = 0;
                        //elvs[0] = _dem_layer.Value[cell.Row, cell.Column];
                        ////elvs[0] = _dem_layer.GetNearestValue(pt0);
                        //for (int j = 1; j < npt; j++)
                        //{
                        //    cell = _dem_layer.ProjToCell(rch_geo.Coordinates[j].X, rch_geo.Coordinates[j].Y);
                        //    elvs[j] = _dem_layer.Value[cell.Row, cell.Column];
                        //    dis[j] = SpatialDistance.DistanceBetween(rch_geo.Coordinates[j - 1], rch_geo.Coordinates[j]);
                        //}
                        //for (int j = 0; j < npt; j++)
                        //{
                        //    ac_dis[j] = dis.Take(j + 1).Sum();
                        //}
                        //MyStatisticsMath.LinearRegression(ac_dis, elvs, 0, elvs.Length, out rs, out yint, out slope);
                        //if (slope <= 0)
                        //{
                        //    //msg += "minus slope found...\n at reach " + (i + 1);
                        //    slope = MinSlope;
                        //    elvs = elvs.Reverse().ToArray();
                        //    MyStatisticsMath.LinearRegression(ac_dis, elvs, 0, elvs.Length, out rs, out yint, out slope);
                        //    if (slope == 0)
                        //        slope = MinSlope;
                        //}
                        //for (int j = 0; j < npt; j++)
                        //{
                        //    elvs[j] = yint + slope * ac_dis[j];
                        //}
                        //elev_av = elvs.Average();

                        //if (slope < MinSlope)
                        //    slope = MinSlope;
                        //if (slope > MaxSlope)
                        //    slope = MaxSlope;

                        var reach = new ReachFeature()
                        {
                            Row = row,
                            Column = col,
                            DataRow = dr,
                            Elevation = elev_av,
                            Slope = slope,
                            Length = rch_geo.Length
                        };
                        double key = order;
                        if (fealist[segid].Reaches.Keys.Contains(key))
                            key = order + order_count[order] * 0.01;
                        reach.Width = TypeConverterEx.IsNotNull(dr[WidthField].ToString()) ? double.Parse(dr[WidthField].ToString()) : this.Width;
                        reach.UpRiverID = TypeConverterEx.IsNotNull(dr[IUPSEGField].ToString()) ? int.Parse(dr[IUPSEGField].ToString()) : 0;
                        reach.IPrior = TypeConverterEx.IsNotNull(dr[IPRIORField].ToString()) ? int.Parse(dr[IPRIORField].ToString()) : 0;
                        reach.Flow = TypeConverterEx.IsNotNull(dr[FlowField].ToString()) ? double.Parse(dr[FlowField].ToString()) : this.Flow;
                        reach.Runoff = TypeConverterEx.IsNotNull(dr[RunoffField].ToString()) ? double.Parse(dr[RunoffField].ToString()) : this.Runoff;
                        reach.Rainfall = TypeConverterEx.IsNotNull(dr[RainfallField].ToString()) ? double.Parse(dr[RainfallField].ToString()) : this.Rainfall;
                        reach.ET = TypeConverterEx.IsNotNull(dr[ETField].ToString()) ? double.Parse(dr[ETField].ToString()) : ET;
                        reach.Roughness = TypeConverterEx.IsNotNull(dr[RoughnessField].ToString()) ? double.Parse(dr[RoughnessField].ToString()) : Roughness;
                        reach.Slope = TypeConverterEx.IsNotNull(dr[SlopeField].ToString()) ? double.Parse(dr[SlopeField].ToString()) : Slope;

                        if(UseLPFVK)
                        {
                            reach.STRCH1 = vk;
                        }
                        else
                        {
                            reach.STRCH1 = TypeConverterEx.IsNotNull(dr[VKField].ToString()) ? double.Parse(dr[VKField].ToString()) : STRHC1;
                        }
                        reach.Offset = TypeConverterEx.IsNotNull(dr[OffsetField].ToString()) ? double.Parse(dr[OffsetField].ToString()) : Offset;                   
                        reach.BedThickness = TypeConverterEx.IsNotNull(dr[BedThicknessField].ToString()) ? double.Parse(dr[BedThicknessField].ToString()) : BedThickness;
                        reach.THTI = TypeConverterEx.IsNotNull(dr[THTIField].ToString()) ? double.Parse(dr[THTIField].ToString()) : THTI;
                        reach.THTS = TypeConverterEx.IsNotNull(dr[THTSField].ToString()) ? double.Parse(dr[THTSField].ToString()) : THTS;
                        reach.EPS = TypeConverterEx.IsNotNull(dr[EPSField].ToString()) ? double.Parse(dr[EPSField].ToString()) : EPS;

                        reach.OrderKey = key;
                        fealist[segid].Reaches.Add(key, reach);
                        fealist[segid].OutSegmentID = int.Parse(dr[OutSegmentField].ToString()) + SegIDOffset;
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
            //if (UseSWATElevation)
            //{
            //    foreach (var segid in fealist.Keys)
            //    {
            //        var reaches = fealist[segid].Reaches.Values;
            //        var seglength = (from rch in reaches select rch.Length).Sum();
            //        var last = reaches.Last();
            //        var firstreach = reaches.First();
            //        var delta_el = firstreach.MaxSegElevation - firstreach.MinSegElevation;
            //        if (delta_el <= 0)
            //            delta_el = 1;
            //        var slope = firstreach.Slope;// delta_el / seglength;
            //        double curlen = 0;
            //        firstreach.Elevation = firstreach.MaxSegElevation;
            //        for (int i = 1; i < reaches.Count; i++)
            //        {
            //            curlen += reaches[i].Length;
            //            reaches[i].Elevation = reaches[i].MaxSegElevation - curlen * slope;
            //            reaches[i].Slope = slope;
            //        }
            //        if (last.MinSegElevation != last.Elevation)
            //        {
            //            msg += "Waring: elevation error at ";
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var segid in fealist.Keys)
            //    {
            //        var reaches = fealist[segid].Reaches.Values;
            //        var len = new double[reaches.Count];
            //        var elevs = new double[reaches.Count];
            //        double rs = 0, slope = 0, yint = 0;
            //        double curlen = 0;
            //        for (int i = 0; i < reaches.Count; i++)
            //        {
            //            curlen += reaches[i].Length;
            //            len[i] = curlen;
            //            reaches[i].Elevation = grid.GetElevationAt(reaches[i].Row - 1, reaches[i].Column - 1, 0);
            //            elevs[i] = reaches[i].Elevation;
            //        }
            //        if (reaches.Count == 1)
            //        {
            //            slope = MinSlope;
            //        }
            //        else
            //        {
            //            MyStatisticsMath.LinearRegression(len, elevs, 0, elevs.Length, out rs, out yint, out slope);
            //            slope = System.Math.Abs(slope);
            //            if (slope > MaxSlope)
            //                slope = MaxSlope;
            //            else if (slope < MinSlope)
            //                slope = MinSlope;
            //            if (slope == double.NaN)
            //                slope = MinSlope;
            //        }
            //        for (int i = 0; i < reaches.Count; i++)
            //        {
            //            reaches[i].Slope = slope;
            //        }
            //    }
            //}
        }
        private void PrePro(Dictionary<int, ReachFeatureCollection> fealist, out string msg)
        {
            double rs = 0, slope = 0, yint = 0;
            var dt = _sfr_insct_layer.DataTable;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            msg = "";
            for (int i = 0; i < _sfr_insct_layer.Features.Count; i++)
            {
                try
                {
                    var dr = dt.Rows[i];
                    var geo = _sfr_insct_layer.GetFeature(i).Geometry;
                    if (geo.Length <= _dem_layer.CellHeight && IgnoreMinorReach)
                    {
                        continue;
                    }
                    var npt = geo.Coordinates.Count();
                    int segid = int.Parse(dr[SegmentField].ToString()) + SegIDOffset;
                    double[] dis = new double[npt];
                    double[] ac_dis = new double[npt];
                    double[] elvs = new double[npt];
                    double elev_av = 0;
                    var pt0 = geo.Coordinates[0];
                    var cell = _dem_layer.ProjToCell(pt0.X, pt0.Y);
                    double ad = 0;
                    int row = int.Parse(dr["ROW"].ToString());
                    int col = int.Parse(dr["COLUMN"].ToString());
                    if (grid.IsActive(row - 1, col - 1, 0))
                    {
                        dis[0] = 0;
                        elvs[0] = _dem_layer.Value[cell.Row, cell.Column];
                        for (int j = 0; j < npt; j++)
                        {
                            cell = _ad_layer.ProjToCell(geo.Coordinates[j].X, geo.Coordinates[j].Y);
                            if (cell.Row > 0 && cell.Column > 0)
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
                            slope = MinSlope;
                        }

                        for (int j = 0; j < npt; j++)
                        {
                            elvs[j] = yint + slope * ac_dis[j];
                        }
                        elev_av = elvs.Average();

                        if (slope < MinSlope)
                            slope = MinSlope;
                        if (slope > MaxSlope)
                            slope = MaxSlope;

                        var rch = new ReachFeature()
                        {
                            DataRow = dr,
                            Elevation = elev_av,
                            Slope = slope,
                            Length= geo.Length
                        };
                        if (fealist[segid].Reaches.ContainsKey(ad))
                        {
                            ad += i * 0.001;
                        }
                        fealist[segid].Reaches.Add(ad, rch);
                        fealist[segid].OutSegmentID = int.Parse(dr[OutSegmentField].ToString());
                    }
                }
                catch (Exception ex)
                {
                    msg += ex.Message + "\n";
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
