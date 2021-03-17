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
    //public class SetReachByFeatureLayer : MapLayerRequiredTool
    //{
    //    private IMapLayerDescriptor _StreamFeatureLayerDescriptor;
    //    private IFeatureSet _stream_layer;
    //    public SetReachByFeatureLayer()
    //    {
    //        Name = "Set Reach Parameters From Feature Layer";
    //        Category = Cat_CMG;
    //        SubCategory = "SFR";
    //        Description = "Set Reach Parameters by Feature Layer";
    //        Version = "1.0.0.0";
    //        this.Author = "Yong Tian";
    //        MultiThreadRequired = true;

    //        ElevationField = "TopElev";
    //        BedThicknessField = "BedThick";
    //        SlopeField = "Slope";
    //        OffsetField = "Offset";
    //        VKField = "STRHC1";
    //        THTSField = "THTS";
    //        THTIField = "THTI";
    //        EPSField = "EPS";

    //    }

    //    #region GIS Layers

    //    [Category("Input GIS Layer")]
    //    [Description("Reach layer")]
    //    [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    public IMapLayerDescriptor ReachFeatureLayer
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
    //    [Description("Segment ID")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string SegmentIDField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Basic Field Binding")]
    //    [Description("Reach ID")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string ReachIDField
    //    {
    //        get;
    //        set;
    //    }

    //    [Category("Reach Field Binding")]
    //    [Description("Top elevation of streambed")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string ElevationField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Reach Field Binding")]
    //    [Description("Segment mean slope field")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string SlopeField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Reach Field Binding")]
    //    [Description("Thickness of the streambed")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string BedThicknessField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Reach Field Binding")]
    //    [Description("Offset to  elevation of the cell coresponding to the reach")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string OffsetField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Reach Field Binding")]
    //    [Description("VK is a real number equal to the hydraulic conductivity of the streambed. This variable is read when ISFROPT is 1, 2, or 3.")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string VKField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Reach Field Binding")]
    //    [Description("THTS is the saturated volumetric water content in the unsaturated zone.")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string THTSField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Reach Field Binding")]
    //    [Description("THTI is the initial volumetric water content")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string THTIField
    //    {
    //        get;
    //        set;
    //    }
    //    [Category("Reach Field Binding")]
    //    [Description("EPS is the Brooks-Corey exponent used in the relation between water content and hydraulic conductivity within the unsaturated zone (Brooks and Corey, 1966). This variable is read when ISFROPT is 2 or 3.")]
    //    [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
    //    [DropdownListSource("Fields")]
    //    public string EPSField
    //    {
    //        get;
    //        set;
    //    }
    //    #endregion

    //    public override void Initialize()
    //    {
    //        this.Initialized = !(_stream_layer == null || _stream_layer.FeatureType != FeatureType.Polygon);
    //    }

    //    public void CheckFields()
    //    {
    //        var dt_insct = _stream_layer.DataTable;
    //        if (!dt_insct.Columns.Contains(OffsetField))
    //        {
    //            DataColumn dc = new DataColumn(OffsetField, typeof(float));
    //            dt_insct.Columns.Add(dc);
    //            foreach (DataRow row in dt_insct.Rows)
    //            {
    //                row[OffsetField] = 0;
    //            }
    //        }
    //        if (!dt_insct.Columns.Contains(BedThicknessField))
    //        {
    //            DataColumn dc = new DataColumn(BedThicknessField, typeof(float));
    //            dt_insct.Columns.Add(dc);
    //            foreach (DataRow row in dt_insct.Rows)
    //            {
    //                row[BedThicknessField] = 2;
    //            }
    //        }
    //        if (!dt_insct.Columns.Contains(SlopeField))
    //        {
    //            DataColumn dc = new DataColumn(SlopeField, typeof(float));
    //            dt_insct.Columns.Add(dc);
    //            foreach (DataRow row in dt_insct.Rows)
    //            {
    //                row[SlopeField] = 0.0001;
    //            }
    //        }
    //        if (!dt_insct.Columns.Contains(VKField))
    //        {
    //            DataColumn dc = new DataColumn(VKField, typeof(float));
    //            dt_insct.Columns.Add(dc);
    //            foreach (DataRow row in dt_insct.Rows)
    //            {
    //                row[VKField] = 0.1;
    //            }
    //        }
    //        if (!dt_insct.Columns.Contains(THTIField))
    //        {
    //            DataColumn dc = new DataColumn(THTIField, typeof(float));
    //            dt_insct.Columns.Add(dc);
    //            foreach (DataRow row in dt_insct.Rows)
    //            {
    //                row[THTIField] = 0.2;
    //            }
    //        }
    //        if (!dt_insct.Columns.Contains(THTSField))
    //        {
    //            DataColumn dc = new DataColumn(THTSField, typeof(float));
    //            dt_insct.Columns.Add(dc);
    //            foreach (DataRow row in dt_insct.Rows)
    //            {
    //                row[THTSField] = 0.3;
    //            }
    //        }
    //        if (!dt_insct.Columns.Contains(EPSField))
    //        {
    //            DataColumn dc = new DataColumn(EPSField, typeof(float));
    //            dt_insct.Columns.Add(dc);
    //            foreach (DataRow row in dt_insct.Rows)
    //            {
    //                row[EPSField] = 3.5;
    //            }
    //        }
    //        _stream_layer.Save();
    //    }

    //    public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
    //    {
    //        var dt = _stream_layer.DataTable;
    //        var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
    //        var grid = prj.Project.Model.Grid as MFGrid;
    //        var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
    //        if (sfr != null)
    //        {
    //            if (sfr.RiverNetwork.ReachCount != dt.Rows.Count)
    //            {
    //                cancelProgressHandler.Progress("Package_Tool", 90, "Reach count in SFR package is not equal to the row count of the GIS layer");
    //                return false;
    //            }

    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                var segid = int.Parse(dr["ISEG"].ToString());
    //                var reachid = int.Parse(dr["IREACH"].ToString());
    //                var buf = from rr in sfr.RiverNetwork.Reaches where rr.SubID == reachid && rr.ISEG == segid select rr;
    //                if (buf.Any())
    //                {
    //                    var reach = buf.First();
    //                    var river = reach.Parent;

    //                    reach.TopElevation = TypeConverterEx.IsNotNull(dr[ElevationField].ToString()) ? double.Parse(dr[ElevationField].ToString()) : 100;
    //                    reach.Slope = TypeConverterEx.IsNotNull(dr[SlopeField].ToString()) ? double.Parse(dr[SlopeField].ToString()) : 0.001;
    //                    reach.STRHC1 = TypeConverterEx.IsNotNull(dr[VKField].ToString()) ? double.Parse(dr[VKField].ToString()) : 0.05;
    //                    reach.Offset = TypeConverterEx.IsNotNull(dr[OffsetField].ToString()) ? double.Parse(dr[OffsetField].ToString()) : -2;
    //                    reach.BedThick = TypeConverterEx.IsNotNull(dr[BedThicknessField].ToString()) ? double.Parse(dr[BedThicknessField].ToString()) : 2;
    //                    reach.THTI = TypeConverterEx.IsNotNull(dr[THTIField].ToString()) ? double.Parse(dr[THTIField].ToString()) : 0.1;
    //                    reach.THTS = TypeConverterEx.IsNotNull(dr[THTSField].ToString()) ? double.Parse(dr[THTSField].ToString()) : 0.3;
    //                    reach.EPS = TypeConverterEx.IsNotNull(dr[EPSField].ToString()) ? double.Parse(dr[EPSField].ToString()) : 3.5;
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
