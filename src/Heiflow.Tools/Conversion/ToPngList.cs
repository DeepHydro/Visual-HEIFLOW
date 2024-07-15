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
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using Heiflow.Core.Data.Classification;

namespace Heiflow.Tools.Conversion
{
    public class ToPngList : MapLayerRequiredTool
    {
        public enum TimeIntevalunits { Second, Hour, Day, Month, Year }
        public ToPngList()
        {
            Name = "To png imag list";
            Category = "Conversion";
            SubCategory = "Raster";
            Description = "Convert data cube  to png image";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            DateFormat = "yyyy-MM-dd";
            VariableName = "dc";
            Direcotry = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TimeInteval = 86400;
            Start = new DateTime(2000, 1, 1);
            TimeIntevalunit = TimeIntevalunits.Day;
        }
        private IFeatureSet _grid_layer;

        [Category("Input")]
        [Description("The name of the datacube being exported. The Source should be mat[0][0][:]")]
        public string Source
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

        [Category("Input")]
        [Description("The name of exported variable")]
        public string VariableName
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The file folder directory for output")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Direcotry
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Specify the date format used to generate the output files automatically. Its style should be yyyy-MM-dd or yyyy-MM")]
        public string DateFormat
        {
            get;
            set;
        }

        [Category("Optional")]
        [Description("Specify the start date time.")]
        public DateTime Start
        {
            get;
            set;
        }

        [Category("Optional")]
        [Description("Specify the time inteval in the coresponding time unit.")]
        public int TimeInteval
        {
            get;
            set;
        }

        [Category("Optional")]
        [Description("The time inteval unit")]
        public TimeIntevalunits TimeIntevalunit
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The number of breaks")]
        public int NumBreaks { get; set; }
        public IntervalMethod IntervalMethod
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var mat = Get3DMat(Source);
            Initialized = mat != null;

            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_grid_layer == null)
            {
                this.Initialized = false;
            }

            if (TypeConverterEx.IsNull(Direcotry))
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var ntime = mat.Size[1];
                var grid = mf.Grid as RegularGrid;
                if (mat.DateTimes == null)
                {
                    mat.DateTimes = new DateTime[ntime];
                    mat.DateTimes[0] = Start;
                    for (int t = 1; t < ntime; t++)
                    {
                        if (TimeIntevalunit == TimeIntevalunits.Second)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddSeconds(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Hour)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddHours(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Day)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddDays(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Month)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddMonths(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Year)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddYears(TimeInteval);
                        }
                    }
                }
                for (int t = 0; t < ntime; t++)
                {
                    var Filename = "";
                    Filename = string.Format("{0}_{1}.png", VariableName, mat.DateTimes[t].ToString(DateFormat));
                    Filename = Path.Combine(Direcotry, Filename);
                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        var loc = grid.Topology.ActiveCellLocation[i];
                        
                    }

                    var raster = Raster.CreateRaster(Filename, string.Empty, grid.Topology.ColumnCount, grid.Topology.RowCount, 1, typeof(float), new[] { string.Empty });
                    raster.NoDataValue = -9999;
                    raster.Bounds = new RasterBounds(grid.Topology.RowCount, grid.Topology.ColumnCount, new Extent(grid.BBox));
                    raster.Projection = _grid_layer.Projection;
                    var vec = mat.GetVector(var_index, t.ToString(), ":");
                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        var loc = grid.Topology.ActiveCellLocation[i];
                        raster.Value[loc[0], loc[1]] = vec[i];
                    }
                    raster.Save();
                    progress = t * 100 / ntime;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing step: " + t);
                        count++;
                    }
                }

                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
        }

        public  void Create(string filename, int cols, int rows, float[] vec, MFGrid grid)
        {
            ImageInfo imi = new ImageInfo(cols, rows, 8, false); // 8 bits per channel, no alpha 
            // open image for writing 
            PngWriter png = FileHelper.CreatePngWriter(filename, imi, true);
            // add some optional metadata (chunks)
            png.GetMetadata().SetDpi(100.0);
            png.GetMetadata().SetTimeNow(0); // 0 seconds fron now = now
            png.GetMetadata().SetText(PngChunkTextVar.KEY_Title, "Just a text image");
            PngChunk chunk = png.GetMetadata().SetText("my key", "my text .. bla bla");
            chunk.Priority = true; // this chunk will be written as soon as possible
            ImageLine iline = new ImageLine(imi);
            for (int col = 0; col < imi.Cols; col++)
            { // this line will be written to all rows  
                int r = 255;
                int g = 127;
                int b = 255 * col / imi.Cols;
                ImageLineHelper.SetPixel(iline, col, r, g, b, 0); // orange-ish gradient
            }
            for (int row = 0; row < png.ImgInfo.Rows; row++)
            {
                png.WriteRow(iline, row);
            }
            png.End();

        }


    }
}