// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using Heiflow.Applications;

namespace Heiflow.Tools
{
    [InheritedExport(typeof(IModelTool))]
    public abstract class MapLayerRequiredTool : ModelTool, IMapLayerRequiredTool
    {

        protected string[] _FieldsOfSelectedLayer;

        public MapLayerRequiredTool()
        {

        }
        [Browsable(false)]
        public List<IMapLayerDescriptor> MapLayers
        {
            get;
            set;
        }
        [Browsable(false)]
        public string[] FieldsOfSelectedLayer
        {
            get
            {
                return _FieldsOfSelectedLayer;
            }
        }
        public override void Setup()
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var map_layers = (from layer in shell.MapAppManager.Map.Layers select new MapLayerDescriptor { LegendText = layer.LegendText, DataSet = layer.DataSet }).ToArray();
            this.MapLayers = map_layers.Cast<IMapLayerDescriptor>().ToList();
        }

        public override void Initialize()
        {
          
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            return false;
        }
 
    }
}