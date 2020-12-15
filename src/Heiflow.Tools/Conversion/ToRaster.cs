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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Models.Generic;
using System.ComponentModel;
using Heiflow.Controls.WinForm.Editors;
using System.Windows.Forms.Design;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Presentation.Services;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Integration;
using Heiflow.Models.Tools;

namespace Heiflow.Tools.Conversion
{
    public class ToRaster : MapLayerRequiredTool
    {
        public enum FilterMode { Maximum, Minimum, None }
        public ToRaster()
        {
            Name = "To raster";
            Category = "Conversion";
            SubCategory = "Raster";
            Description = "Convert data cube  to raster file with format of TIF";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            Filename = "raster.tif";
        }
        private IFeatureSet _grid_layer;

        [Category("Input")]
        [Description("The name of the datacube being exported. The expression of Source should be mat[0][0][:]")]
        public string Source
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The output file name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Filename
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }
        public override void Initialize()
        {
            var mat = Get3DMat(Source);
            Initialized = mat != null;

            if (GridFeatureLayer == null)
            {
                Initialized = false;
                return;
            }
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_grid_layer == null)
            {
                this.Initialized = false;
            }

            if (TypeConverterEx.IsNull(Filename))
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var mat = GetVector(Source);
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var grid = mf.Grid as RegularGrid;
                var raster = Raster.CreateRaster(Filename, string.Empty, grid.Topology.ColumnCount, grid.Topology.RowCount, 1, typeof(float), new[] { string.Empty });
                raster.NoDataValue = 0;
                raster.Bounds = new RasterBounds(grid.Topology.RowCount, grid.Topology.ColumnCount, new Extent(grid.BBox));
                raster.Projection = _grid_layer.Projection;

                for (int i = 0; i < grid.ActiveCellCount; i++)
                {
                    var loc = grid.Topology.ActiveCellLocation[i];
                    raster.Value[loc[0], loc[1]] = mat[i];
                    progress = i * 100 / grid.ActiveCellCount;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing cell: " + i);
                        count++;
                    }
                }
                raster.Save();
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
        }

    }
}