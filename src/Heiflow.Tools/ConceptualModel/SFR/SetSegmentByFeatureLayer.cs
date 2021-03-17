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
using Heiflow.Core.Hydrology;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Heiflow.Tools.ConceptualModel
{

    //public class SetSegmentByFeatureLayer : MapLayerRequiredTool
    //{
    //    private IMapLayerDescriptor _StreamFeatureLayerDescriptor;
    //    private IFeatureSet _stream_layer;
    //    public SetSegmentByFeatureLayer()
    //    {
    //        Name = "Set Segment Parameters by Feature Layer";
    //        Category = Cat_CMG;
    //        SubCategory = "SFR";
    //        Description = "Set Segment Parameters by Feature Layer";
    //        Version = "1.0.0.0";
    //        this.Author = "Yong Tian";
    //        MultiThreadRequired = true;
    //        SegmentIDField = "GRID_CODE";
    //    }

    //    #region GIS Layers

    //    [Category("GIS Layer")]
    //    [Description("Stream layer")]
    //    [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    public IMapLayerDescriptor StreamFeatureLayer
    //    {
    //        get
    //        {
    //            return _StreamFeatureLayerDescriptor;
    //        }
    //        set
    //        {
    //            _StreamFeatureLayerDescriptor = value;
    //            if (_StreamFeatureLayerDescriptor != null)
    //            {
    //                _stream_layer = _StreamFeatureLayerDescriptor.DataSet as IFeatureSet;
    //                if (_stream_layer != null)
    //                {
    //                    var buf = from DataColumn dc in _stream_layer.DataTable.Columns select dc.ColumnName;
    //                    Fields = buf.ToArray();
    //                }
    //            }
    //        }
    //    }
    //    #endregion

    //    #region Field Binding
    //    [Browsable(false)]
    //    public string[] Fields
    //    {
    //        get;
    //        protected set;
    //    }
    //    [Category("Basic Field Binding")]
    //    [Description("The segment ID field")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string SegmentIDField
    //    {
    //        get;
    //        set;
    //    }

    //    [Category("Segment Field Binding")]
    //    [Description("Segment width")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string WidthField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Segment Field Binding")]
    //    [Description("IUPSEG is an integer value of the downstream stream segment that receives tributary inflow from the last downstream reach of this segment.")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string IUPSEGField
    //    {
    //        get;
    //        set;
    //    }
    //    //IPRIOR An integer value that only is specified if IUPSEG > 0 (do not specify a value in this field if IUPSEG = 0 or IUPSEG < 0). IPRIOR defines the prioritization system for diversion, such as when insufficient water is available to meet all diversion stipulations, and is used in conjunction with the value of FLOW (specified below).
    //    //When IPRIOR = 0, then if the specified diversion flow (FLOW) is greater than the flow available in the stream segment from which the diversion is made, the diversion is reduced to the amount available, which will leave no flow available for tributary flow into a downstream tributary of segment IUPSEG.
    //    //When IPRIOR = -1, then if the specified diversion flow (FLOW) is greater than the flow available in the stream segment from which the diversion is made, no water is diverted from the stream. This approach assumes that once flow in the stream is sufficiently low, diversions from the stream cease, and is the “priority” algorithm that originally was programmed into the STR1 Package (Prudic, 1989).
    //    //When IPRIOR = -2, then the amount of the diversion is computed as a fraction of the available flow in segment IUPSEG; in this case, 0.0 < FLOW < 1.0.
    //    //When IPRIOR = -3, then a diversion is made only if the streamflow leaving segment IUPSEG exceeds the value of FLOW. If this occurs, then the quantity of water diverted is the excess flow and the quantity that flows from the last reach of segment IUPSEG into its downstream tributary (OUTSEG) is equal to FLOW. This represents a flood-control type of diversion, as described by Danskin and Hanson (2002).
    //    [Category("Segment Field Binding")]
    //    [Description("An integer value that only is specified if IUPSEG > 0 (do not specify a value in this field if IUPSEG = 0 or IUPSEG < 0). ")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string IPRIORField
    //    {
    //        get;
    //        set;
    //    }
    //    //•	If the stream is a headwater stream, FLOW defines the total inflow to the first reach of the segment. The value can be any number ≥ 0.
    //    //•	If the stream is a tributary stream, FLOW defines additional specified inflow to or withdrawal from the first reach of the segment (that is, in addition to the discharge from the upstream segment of which this is a tributary). This additional flow does not interact with the groundwater system. For example, a positive number might be used to represent direct outflow into a stream from a sewage treatment plant, whereas a negative number might be used to represent pumpage directly from a stream into an intake pipe for a municipal water treatment plant. (Also see additional explanatory notes below.)
    //    //•	If the stream is a diversionary stream, and the diversion is from another stream segment, FLOW defines the streamflow diverted from the last reach of stream segment IUPSEG into the first reach of this segment. The diversion is computed or adjusted according to the value of IPRIOR.
    //    //•	If the stream is a diversionary stream, and the diversion is from a lake, FLOW defines a fixed rate of discharge diverted from the lake into the first reach of this stream segment (unless the lake goes dry) and flow from the lake is not dependent on the value of ICALC. However, if FLOW = 0, then the lake outflow into the first reach of this segment will be calculated on the basis of lake stage relative to the top of the streambed for the first reach using one of the methods defined by ICALC.
    //    [Category("Segment Field Binding")]
    //    [Description("FLOW A real number that is the streamflow (in units of volume per time) entering or leaving the upstream end of a stream segment (that is, into the first reach).")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string FlowField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Segment Field Binding")]
    //    [Description("")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string RunoffField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Segment Field Binding")]
    //    [Description("")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string ETField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Segment Field Binding")]
    //    [Description("")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string RainfallField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Segment Field Binding")]
    //    [Description("")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string RoughnessField
    //    {
    //        get;
    //        set;
    //    }
    //    #endregion

    //    public override void Initialize()
    //    {
    //        this.Initialized = _stream_layer != null;
    //    }

    //    public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
    //    {
    //        var dt = _stream_layer.DataTable;
    //        var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
    //        var grid = prj.Project.Model.Grid as MFGrid;
    //        var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
    //        if (sfr != null)
    //        {
    //            if (sfr.RiverNetwork.RiverCount != dt.Rows.Count)
    //            {
    //                cancelProgressHandler.Progress("Package_Tool", 1, "Warning: Segment count in SFR package is not equal to the row count of the GIS layer");
    //                return false;
    //            }
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                var segid = int.Parse(dr["ISEG"].ToString());
    //                var river = sfr.RiverNetwork.GetRiver(segid);
    //                if (river != null)
    //                {
    //                    river.Width = TypeConverterEx.IsNotNull(dr[WidthField].ToString()) ? double.Parse(dr[WidthField].ToString()) : 100;
    //                    river.UpRiverID = TypeConverterEx.IsNotNull(dr[IUPSEGField].ToString()) ? int.Parse(dr[IUPSEGField].ToString()) : 0;
    //                    river.IPrior = TypeConverterEx.IsNotNull(dr[IUPSEGField].ToString()) ? int.Parse(dr[IPRIORField].ToString()) : 0;
    //                    river.Flow = TypeConverterEx.IsNotNull(dr[FlowField].ToString()) ? double.Parse(dr[FlowField].ToString()) : 0;
    //                    river.Runoff = TypeConverterEx.IsNotNull(dr[RunoffField].ToString()) ? double.Parse(dr[RunoffField].ToString()) : 0;
    //                    river.ETSW = TypeConverterEx.IsNotNull(dr[ETField].ToString()) ? double.Parse(dr[ETField].ToString()) : 0;
    //                    river.PPTSW = TypeConverterEx.IsNotNull(dr[RainfallField].ToString()) ? double.Parse(dr[RainfallField].ToString()) : 0;
    //                    river.ROUGHCH = TypeConverterEx.IsNotNull(dr[RoughnessField].ToString()) ? double.Parse(dr[RoughnessField].ToString()) : 0.05;
    //                }
    //            }

    //            sfr.NetworkToMat();
    //            return true;
    //        }
    //        else
    //        {
    //            cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
    //            return false;
    //        }

    //    }
    //}
}
