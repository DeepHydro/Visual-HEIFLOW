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
    public class PreproSFRTool : MapLayerRequiredTool
    {
        private IFeatureSet _stream_layer;
        private IMapLayerDescriptor _StreamFeatureLayerDescriptor;
        public PreproSFRTool()
        {
            Name = "Preprocess River Shapefie for SFR";
            Category = "Preprocessing";
            Description = "Preprocess River Shapefie for SFR";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = false;

            //Width1 = 50;
            //Width2 = 50;
            Width = 50;
            Flow = 0;
            Runoff = 0;
            ET = 0;
            Rainfall = 0;
            Roughness = 0.05;
            Offset = -2;
            Slope = 0.001;
            BedThickness = 2;
            STRHC1 = 0.1;
            THTS = 0.3;
            THTI = 0.2;
            EPS = 3.5;

            WidthField = "Width";
            IUPSEGField = "IUPSEG";
            IPRIORField = "IPRIO";
            FlowField = "Flow";
            RunoffField = "Runoff";
            ETField = "ET";
            RainfallField = "Rainfall";
            RoughnessField = "Roughness";

            BedThicknessField = "BedThick";
            SlopeField = "Slope";
            OffsetField = "Offset";
            VKField = "VK";
            THTIField = "THTI";
            THTSField = "THTS";
            EPSField = "EPS";
        }

        #region GIS Layers

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
                    _stream_layer = _StreamFeatureLayerDescriptor.DataSet as IFeatureSet;
                    if (_stream_layer != null)
                    {
                        var buf = from DataColumn dc in _stream_layer.DataTable.Columns select dc.ColumnName;
                        Fields = buf.ToArray();
                    }
                }
            }
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
      
        #endregion
        public override void Initialize()
        {
            this.Initialized = true;
            if (StreamFeatureLayer == null )
            {
                this.Initialized = false;
                return;
            } 
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            PreproIntersection();
            cancelProgressHandler.Progress("Package_Tool", 100, "The river shapefile has been prepared.");
            return true;
        }

        private void PreproIntersection()
        {
            var dt_insct = _stream_layer.DataTable;

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
            _stream_layer.Save();
        }
  
    }
}
