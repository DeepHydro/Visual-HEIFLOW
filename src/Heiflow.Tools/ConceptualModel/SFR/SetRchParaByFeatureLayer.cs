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
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Heiflow.Tools.ConceptualModel
{
    public enum ReachParameter {  BedThick, TopElevation, Slope, Length, STRHC1, THTS, THTI, EPS };
    public class SetReachParaByFeatureLayer : MapLayerRequiredTool
    {
        private IMapLayerDescriptor _StreamFeatureLayerDescriptor;
        private IFeatureSet _stream_layer;
        public SetReachParaByFeatureLayer()
        {
            Name = "Set Reach Parameter by Feature Layer";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Set Reach Parameter by Feature Layer";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            SegmentIDField = "GRID_CODE";
            ParameterName = ReachParameter.BedThick;
        }

        #region GIS Layers

        [Category("GIS Layer")]
        [Description("Reach layer")]
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
        [Category("Field Binding")]
        [Description("The segment ID field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SegmentIDField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The reach ID field.")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ReachIDField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The field of the feature layer used to set the segment parameter")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ParameterField
        {
            get;
            set;
        }
        [Category("SFR Parameter")]
        [Description("The name of reach parameter")]

        public ReachParameter ParameterName
        {
            get;
            set;
        }
        #endregion

        public override void Initialize()
        {
            this.Initialized = _stream_layer != null;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var dt = _stream_layer.DataTable;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            if (sfr != null)
            {
                if (SFRPackage.ReachPara.Contains(ParameterName.ToString()))
                {
                    //if (sfr.RiverNetwork.ReachCount != dt.Rows.Count)
                    //{
                    //    cancelProgressHandler.Progress("Package_Tool", 1, "Warning: Reach count in SFR package is not equal to the row count of the GIS layer");
                    //    return false;
                    //}
                    if (dt.Columns.Contains(SegmentIDField) && dt.Columns.Contains(ReachIDField))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            var segid = int.Parse(dr[SegmentIDField].ToString());
                            var rchid = int.Parse(dr[ReachIDField].ToString());
                            var reach = sfr.RiverNetwork.GetReach(segid, rchid);
                            if (reach != null)
                            {
                                reach.GetType().GetProperty(ParameterName.ToString()).SetValue(reach, dr[ParameterField]);
                            }
                        }
                        sfr.NetworkToMat();
                        return true;
                    }
                    else
                    {
                        cancelProgressHandler.Progress("Package_Tool", 1, "Warning: the GIS layer must contain Segment ID field and Reach ID field");
                        return false;
                    }
                }
                else
                {
                    cancelProgressHandler.Progress("Package_Tool", 1, "Warning: the parameter name is invalid");
                    return false;
                }
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }
        }
    }
}